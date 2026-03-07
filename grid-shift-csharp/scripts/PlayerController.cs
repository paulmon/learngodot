using Godot;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class PlayerController : Node2D
{
    [Signal] public delegate void LevelCompletedEventHandler();

    [Export] public int TileSize = 32;
    [Export] public NodePath WallsRootPath = "../Walls";
    [Export] public NodePath CratesRootPath = "../Crates";
    [Export] public NodePath GoalsRootPath = "../Goals";
    [Export] public NodePath WinLabelPath = "../UI/WinLabel";
    [Export] public NodePath AudioManagerPath = "../AudioManager";
    [Export(PropertyHint.File, "*.txt")] public string LevelFilePath = "res://levels/level01.txt";

    private GameState _gameState;
    private GameState _startState;
    private Dictionary<Vector2I, Node2D> _cratesByPos = new();
    private Stack<GameState> _undoStack = new();
    private Label _winLabel;
    private AudioManager _audioManager;
    private bool _isLevelComplete;

    public override void _Ready()
    {
        _winLabel = GetNodeOrNull<Label>(WinLabelPath);
        _audioManager = GetNodeOrNull<AudioManager>(AudioManagerPath);
        SetWinLabelVisible(false);

        if (!LoadLevel(LevelFilePath))
        {
            // Fallback to scene-authored nodes when no level file is available.
            InitializeFromSceneNodes();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("restart"))
        {
            _gameState = _startState.Clone();
            _undoStack.Clear();
            RebuildCratesFromState();
            SnapToGrid();
            UpdateWinState();
            return;
        }

        if (@event.IsActionPressed("undo"))
        {
            UndoLastMove();
            return;
        }

        if (_isLevelComplete)
            return;

        var delta = Vector2I.Zero;

        if (@event.IsActionPressed("move_up"))
            delta = Vector2I.Up;
        else if (@event.IsActionPressed("move_down"))
            delta = Vector2I.Down;
        else if (@event.IsActionPressed("move_left"))
            delta = Vector2I.Left;
        else if (@event.IsActionPressed("move_right"))
            delta = Vector2I.Right;

        if (delta == Vector2I.Zero)
            return;

        var snapshot = _gameState.Clone();
        if (!_gameState.TryApplyMove(delta, out var movedCrate, out var crateFrom, out var crateTo))
        {
            _audioManager?.PlayFail();
            return;
        }

        _undoStack.Push(snapshot);

        _audioManager?.PlayMove();

        if (movedCrate)
        {
            MoveCrateNode(crateFrom, crateTo);
            _audioManager?.PlayPush();

            if (_gameState.GoalPositions.Contains(crateTo))
                _audioManager?.PlayGoalComplete();
        }

        SnapToGrid();
        UpdateWinState();
    }

    private void UndoLastMove()
    {
        if (_undoStack.Count == 0)
            return;

        _gameState = _undoStack.Pop();
        RebuildCratesFromState();

        SnapToGrid();
        UpdateWinState();
    }

    private void UpdateWinState()
    {
        var wasLevelComplete = _isLevelComplete;

        _isLevelComplete = _gameState != null && _gameState.IsComplete();
        SetWinLabelVisible(_isLevelComplete);

        if (_isLevelComplete && !wasLevelComplete)
        {
            GD.Print("Level complete: all crates are on goals.");
            EmitSignal(SignalName.LevelCompleted);
        }
    }

    private void SetWinLabelVisible(bool isVisible)
    {
        if (_winLabel != null)
            _winLabel.Visible = isVisible;
    }

    public bool LoadLevel(string levelFilePath)
    {
        LevelFilePath = levelFilePath;

        if (!TryLoadLevelFromFile())
            return false;

        InitializeFromSceneNodes();
        return true;
    }

    private bool TryLoadLevelFromFile()
    {
        if (string.IsNullOrWhiteSpace(LevelFilePath))
            return false;

        if (!FileAccess.FileExists(LevelFilePath))
        {
            GD.PushWarning($"Level file not found: {LevelFilePath}");
            return false;
        }

        using var file = FileAccess.Open(LevelFilePath, FileAccess.ModeFlags.Read);
        if (file == null)
            return false;

        var text = file.GetAsText();

        if (!TryParseLevelText(text, out var playerStart, out var walls, out var crates, out var goals))
            return false;

        BuildLevelNodes(playerStart, walls, crates, goals);
        return true;
    }

    private void InitializeFromSceneNodes()
    {
        var wallPositions = ReadGridPositionsFromNodeChildren(WallsRootPath);
        var goalPositions = ReadGridPositionsFromNodeChildren(GoalsRootPath);
        _cratesByPos = ReadCratesFromScene();

        var playerStart = WorldToGrid(GlobalPosition);
        var cratePositions = new HashSet<Vector2I>(_cratesByPos.Keys);

        _gameState = new GameState(playerStart, wallPositions, goalPositions, cratePositions);
        _startState = _gameState.Clone();
        _undoStack.Clear();
        _isLevelComplete = false;
        SetWinLabelVisible(false);

        SnapToGrid();
        UpdateWinState();
    }

    private bool TryParseLevelText(
        string text,
        out Vector2I playerStart,
        out List<Vector2I> walls,
        out List<Vector2I> crates,
        out List<Vector2I> goals)
    {
        playerStart = Vector2I.Zero;
        walls = new List<Vector2I>();
        crates = new List<Vector2I>();
        goals = new List<Vector2I>();

        var normalizedText = text.Replace("\r", string.Empty).TrimEnd('\n');
        if (string.IsNullOrWhiteSpace(normalizedText))
        {
            GD.PushWarning("Level file is empty.");
            return false;
        }

        var lines = normalizedText.Split('\n', StringSplitOptions.None);
        var width = lines[0].Length;
        if (width == 0)
        {
            GD.PushWarning("Level file has an empty first row.");
            return false;
        }

        for (var row = 0; row < lines.Length; row++)
        {
            if (lines[row].Length != width)
            {
                GD.PushWarning("Level file must be rectangular (all rows equal width).");
                return false;
            }
        }

        var playerCount = 0;
        var maxY = lines.Length - 1;
        var maxX = width - 1;

        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                var cell = new Vector2I(x, y);
                var symbol = line[x];

                if (symbol != '#' && symbol != '.' && symbol != ' ' && symbol != 'P' && symbol != 'C' && symbol != 'G' && symbol != '*')
                {
                    GD.PushWarning($"Invalid level symbol '{symbol}' at ({x}, {y}).");
                    return false;
                }

                if (x == 0 || y == 0 || x == maxX || y == maxY)
                {
                    if (symbol != '#')
                    {
                        GD.PushWarning("Level border must be fully enclosed by walls '#'.");
                        return false;
                    }
                }

                switch (symbol)
                {
                    case '#':
                        walls.Add(cell);
                        break;
                    case 'P':
                        playerStart = cell;
                        playerCount++;
                        break;
                    case 'C':
                        crates.Add(cell);
                        break;
                    case 'G':
                        goals.Add(cell);
                        break;
                    case '*':
                        crates.Add(cell);
                        goals.Add(cell);
                        break;
                }
            }
        }

        if (playerCount != 1)
        {
            GD.PushWarning($"Invalid level: expected exactly one player marker 'P', found {playerCount}.");
            return false;
        }

        if (crates.Count != goals.Count)
        {
            GD.PushWarning($"Invalid level: crates ({crates.Count}) must match goals ({goals.Count}).");
            return false;
        }

        return true;
    }

    private void BuildLevelNodes(Vector2I playerStart, List<Vector2I> walls, List<Vector2I> crates, List<Vector2I> goals)
    {
        var wallsRoot = EnsureRootNode(WallsRootPath, "Walls");
        var cratesRoot = EnsureRootNode(CratesRootPath, "Crates");
        var goalsRoot = EnsureRootNode(GoalsRootPath, "Goals");

        ClearChildren(wallsRoot);
        ClearChildren(cratesRoot);
        ClearChildren(goalsRoot);

        foreach (var wall in walls)
        {
            var wallNode = new Node2D { Name = $"Wall_{wall.X}_{wall.Y}", Position = GridToWorld(wall) };
            var wallVisual = new Polygon2D
            {
                Color = new Color(0.28f, 0.34f, 0.42f, 1f),
                Polygon = new Vector2[]
                {
                    new Vector2(-14, -14),
                    new Vector2(14, -14),
                    new Vector2(14, 14),
                    new Vector2(-14, 14)
                }
            };
            wallNode.AddChild(wallVisual);
            wallsRoot.AddChild(wallNode);
        }

        foreach (var crate in crates)
        {
            var crateNode = new Node2D { Name = $"Crate_{crate.X}_{crate.Y}", Position = GridToWorld(crate) };
            var crateVisual = new Polygon2D
            {
                Color = new Color(0.78f, 0.45f, 0.21f, 1f),
                Polygon = new Vector2[]
                {
                    new Vector2(-12, -12),
                    new Vector2(12, -12),
                    new Vector2(12, 12),
                    new Vector2(-12, 12)
                }
            };
            crateNode.AddChild(crateVisual);
            cratesRoot.AddChild(crateNode);
        }

        foreach (var goal in goals)
        {
            var goalNode = new Node2D { Name = $"Goal_{goal.X}_{goal.Y}", Position = GridToWorld(goal) };
            var goalVisual = new Polygon2D
            {
                Color = new Color(0.24f, 0.74f, 0.44f, 1f),
                Polygon = new Vector2[]
                {
                    new Vector2(-8, -8),
                    new Vector2(8, -8),
                    new Vector2(8, 8),
                    new Vector2(-8, 8)
                }
            };
            goalNode.AddChild(goalVisual);
            goalsRoot.AddChild(goalNode);
        }

        GlobalPosition = GridToWorld(playerStart);
    }

    private Node2D EnsureRootNode(NodePath path, string fallbackName)
    {
        if (GetNodeOrNull<Node2D>(path) is Node2D existing)
            return existing;

        var root = new Node2D { Name = fallbackName };
        if (GetParent() is Node parent)
            parent.AddChild(root);

        return root;
    }

    private static void ClearChildren(Node parent)
    {
        var children = parent.GetChildren();
        foreach (var child in children)
        {
            if (child is Node node)
                node.Free();
        }
    }

    private HashSet<Vector2I> ReadGridPositionsFromNodeChildren(NodePath rootPath)
    {
        var positions = new HashSet<Vector2I>();
        if (GetNodeOrNull<Node2D>(rootPath) is not Node2D root)
            return positions;

        foreach (var child in root.GetChildren())
        {
            if (child is not Node2D node)
                continue;

            positions.Add(WorldToGrid(node.GlobalPosition));
        }

        return positions;
    }

    private Dictionary<Vector2I, Node2D> ReadCratesFromScene()
    {
        var cratesByPos = new Dictionary<Vector2I, Node2D>();
        if (GetNodeOrNull<Node2D>(CratesRootPath) is not Node2D cratesRoot)
            return cratesByPos;

        foreach (var child in cratesRoot.GetChildren())
        {
            if (child is not Node2D crate)
                continue;

            var crateGrid = WorldToGrid(crate.GlobalPosition);
            crate.GlobalPosition = GridToWorld(crateGrid);
            cratesByPos[crateGrid] = crate;
        }

        return cratesByPos;
    }

    private void MoveCrateNode(Vector2I from, Vector2I to)
    {
        if (!_cratesByPos.TryGetValue(from, out var crate))
            return;

        _cratesByPos.Remove(from);
        _cratesByPos[to] = crate;
        crate.GlobalPosition = GridToWorld(to);
    }

    private void RebuildCratesFromState()
    {
        if (GetNodeOrNull<Node2D>(CratesRootPath) is not Node2D cratesRoot)
            return;

        ClearChildren(cratesRoot);
        _cratesByPos.Clear();

        foreach (var position in _gameState.CratePositions.OrderBy(p => p.Y).ThenBy(p => p.X))
        {
            var crate = CreateCrateNode(position);
            cratesRoot.AddChild(crate);
            crate.GlobalPosition = GridToWorld(position);
            _cratesByPos[position] = crate;
        }
    }

    private Node2D CreateCrateNode(Vector2I position)
    {
        var crateNode = new Node2D { Name = $"Crate_{position.X}_{position.Y}", Position = GridToWorld(position) };
        var crateVisual = new Polygon2D
        {
            Color = new Color(0.78f, 0.45f, 0.21f, 1f),
            Polygon = new Vector2[]
            {
                new Vector2(-12, -12),
                new Vector2(12, -12),
                new Vector2(12, 12),
                new Vector2(-12, 12)
            }
        };
        crateNode.AddChild(crateVisual);
        return crateNode;
    }

    private void SnapToGrid()
    {
        if (_gameState != null)
            GlobalPosition = GridToWorld(_gameState.PlayerGridPos);
    }

    private Vector2I WorldToGrid(Vector2 world)
    {
        return new Vector2I(
            Mathf.RoundToInt(world.X / TileSize),
            Mathf.RoundToInt(world.Y / TileSize)
        );
    }

    private Vector2 GridToWorld(Vector2I grid)
    {
        return new Vector2(grid.X * TileSize, grid.Y * TileSize);
    }
}
