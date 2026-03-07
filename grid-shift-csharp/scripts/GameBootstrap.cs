using Godot;

public partial class GameBootstrap : Node
{
    public override void _Ready()
    {
        EnsureActionKey("move_up", Key.W);
        EnsureActionKey("move_down", Key.S);
        EnsureActionKey("move_left", Key.A);
        EnsureActionKey("move_right", Key.D);
        EnsureActionKey("undo", Key.Z);
        EnsureActionKey("restart", Key.R);
        EnsureActionKey("pause", Key.Escape);

        if (GetTree().CurrentScene is CanvasItem canvasItem)
        {
            canvasItem.Modulate = new Color(1f, 1f, 1f, 0f);
            var tween = CreateTween();
            tween.TweenProperty(canvasItem, "modulate", new Color(1f, 1f, 1f, 1f), 0.2f);
        }
    }

    private static void EnsureActionKey(string actionName, Key key)
    {
        if (!InputMap.HasAction(actionName))
            InputMap.AddAction(actionName);

        foreach (var inputEvent in InputMap.ActionGetEvents(actionName))
        {
            if (inputEvent is InputEventKey keyEvent && keyEvent.PhysicalKeycode == key)
                return;
        }

        var newEvent = new InputEventKey
        {
            PhysicalKeycode = key
        };

        InputMap.ActionAddEvent(actionName, newEvent);
    }
}
