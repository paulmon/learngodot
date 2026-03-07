using Godot;

public partial class LevelSelectController : Control
{
    [Export] public NodePath GridPath = "Center/VBox/LevelsGrid";
    [Export] public NodePath BackButtonPath = "Center/VBox/BackButton";

    private GridContainer _levelsGrid;

    public override void _Ready()
    {
        _levelsGrid = GetNodeOrNull<GridContainer>(GridPath);
        BuildLevelButtons();

        if (GetNodeOrNull<Button>(BackButtonPath) is Button backButton)
            backButton.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");

        Modulate = new Color(1f, 1f, 1f, 0f);
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 1f), 0.2f);
    }

    private void BuildLevelButtons()
    {
        if (_levelsGrid == null)
            return;

        foreach (var child in _levelsGrid.GetChildren())
        {
            if (child is Node node)
                node.Free();
        }

        var highestUnlocked = SaveData.LoadHighestUnlockedLevelIndex();

        for (var i = 0; i < LevelCatalog.Count; i++)
        {
            var levelIndex = i;
            var button = new Button
            {
                Text = $"Level {levelIndex + 1}",
                Disabled = levelIndex > highestUnlocked,
                CustomMinimumSize = new Vector2(120, 36)
            };

            if (!button.Disabled)
            {
                button.Pressed += () =>
                {
                    LevelSession.StartLevelIndex = levelIndex;
                    GetTree().ChangeSceneToFile("res://scenes/Main.tscn");
                };
            }

            _levelsGrid.AddChild(button);
        }
    }
}
