using Godot;

public partial class PauseMenuController : CanvasLayer
{
    [Export] public NodePath PanelPath = "Panel";
    [Export] public NodePath ResumeButtonPath = "Panel/VBox/ResumeButton";
    [Export] public NodePath RestartButtonPath = "Panel/VBox/RestartButton";
    [Export] public NodePath LevelSelectButtonPath = "Panel/VBox/LevelSelectButton";
    [Export] public NodePath MainMenuButtonPath = "Panel/VBox/MainMenuButton";
    [Export] public NodePath GameManagerPath = "../GameManager";

    private Control _panel;
    private GameManager _gameManager;

    public override void _Ready()
    {
        // Must run both while playing and while paused to toggle with Esc.
        ProcessMode = ProcessModeEnum.Always;
        _panel = GetNodeOrNull<Control>(PanelPath);
        _gameManager = GetNodeOrNull<GameManager>(GameManagerPath);

        if (GetNodeOrNull<Button>(ResumeButtonPath) is Button resumeButton)
            resumeButton.Pressed += OnResumePressed;

        if (GetNodeOrNull<Button>(RestartButtonPath) is Button restartButton)
            restartButton.Pressed += OnRestartPressed;

        if (GetNodeOrNull<Button>(LevelSelectButtonPath) is Button levelSelectButton)
            levelSelectButton.Pressed += OnLevelSelectPressed;

        if (GetNodeOrNull<Button>(MainMenuButtonPath) is Button mainMenuButton)
            mainMenuButton.Pressed += OnMainMenuPressed;

        SetMenuVisible(false);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            TogglePause();
            GetViewport().SetInputAsHandled();
        }
    }

    private void TogglePause()
    {
        var paused = !GetTree().Paused;
        GetTree().Paused = paused;
        SetMenuVisible(paused);
    }

    private void SetMenuVisible(bool isVisible)
    {
        Visible = isVisible;
        if (_panel != null)
            _panel.Visible = isVisible;
    }

    private void OnResumePressed()
    {
        GetTree().Paused = false;
        SetMenuVisible(false);
    }

    private void OnRestartPressed()
    {
        _gameManager?.RestartCurrentLevel();
        GetTree().Paused = false;
        SetMenuVisible(false);
    }

    private void OnLevelSelectPressed()
    {
        GetTree().Paused = false;
        _gameManager?.GoToLevelSelect();
    }

    private void OnMainMenuPressed()
    {
        GetTree().Paused = false;
        _gameManager?.GoToMainMenu();
    }
}
