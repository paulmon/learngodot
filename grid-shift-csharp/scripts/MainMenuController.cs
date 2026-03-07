using Godot;

public partial class MainMenuController : Control
{
    [Export] public NodePath StartButtonPath = "Center/VBox/StartButton";
    [Export] public NodePath LevelSelectButtonPath = "Center/VBox/LevelSelectButton";
    [Export] public NodePath QuitButtonPath = "Center/VBox/QuitButton";

    public override void _Ready()
    {
        if (GetNodeOrNull<Button>(StartButtonPath) is Button startButton)
            startButton.Pressed += OnStartPressed;

        if (GetNodeOrNull<Button>(LevelSelectButtonPath) is Button levelSelectButton)
            levelSelectButton.Pressed += OnLevelSelectPressed;

        if (GetNodeOrNull<Button>(QuitButtonPath) is Button quitButton)
            quitButton.Pressed += OnQuitPressed;

        Modulate = new Color(1f, 1f, 1f, 0f);
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 1f), 0.2f);
    }

    private void OnStartPressed()
    {
        LevelSession.StartLevelIndex = 0;
        GetTree().ChangeSceneToFile("res://scenes/Main.tscn");
    }

    private void OnLevelSelectPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/LevelSelect.tscn");
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
