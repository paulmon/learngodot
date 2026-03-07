using Godot;

public partial class GameManager : Node
{
    [Export] public NodePath PlayerPath = "../Player";
    [Export]
    public Godot.Collections.Array<string> LevelFiles = new()
    {
        "res://levels/level01.txt",
        "res://levels/level02.txt",
        "res://levels/level03.txt",
        "res://levels/level04.txt",
        "res://levels/level05.txt",
        "res://levels/level06.txt",
        "res://levels/level07.txt",
        "res://levels/level08.txt",
        "res://levels/level09.txt",
        "res://levels/level10.txt",
        "res://levels/level11.txt",
        "res://levels/level12.txt"
    };
    [Export] public int CurrentLevelIndex = 0;

    private PlayerController _player;

    public override void _Ready()
    {
        _player = GetNodeOrNull<PlayerController>(PlayerPath);
        if (_player == null)
        {
            GD.PushError("GameManager could not find PlayerController node.");
            return;
        }

        _player.LevelCompleted += OnLevelCompleted;
        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        if (LevelFiles.Count == 0)
        {
            GD.PushWarning("No level files configured in GameManager.");
            return;
        }

        CurrentLevelIndex = Mathf.Clamp(CurrentLevelIndex, 0, LevelFiles.Count - 1);
        var levelPath = LevelFiles[CurrentLevelIndex];

        if (!_player.LoadLevel(levelPath))
            GD.PushError($"Failed to load level: {levelPath}");
    }

    private void OnLevelCompleted()
    {
        if (CurrentLevelIndex < LevelFiles.Count - 1)
        {
            CurrentLevelIndex++;
            LoadCurrentLevel();
            return;
        }

        GD.Print("All configured levels are complete.");
    }
}
