using Godot;

public partial class PlayerController : Node2D
{
    [Export] public int TileSize = 32;

    private Vector2I _gridPos;
    private Vector2I _startGridPos;

    public override void _Ready()
    {
        _gridPos = WorldToGrid(GlobalPosition);
        _startGridPos = _gridPos;
        SnapToGrid();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("restart"))
        {
            _gridPos = _startGridPos;
            SnapToGrid();
            return;
        }

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

        _gridPos += delta;
        SnapToGrid();
    }

    private void SnapToGrid()
    {
        GlobalPosition = GridToWorld(_gridPos);
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
