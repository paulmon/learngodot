using Godot;

public partial class GameManager : Node
{
    [Export] public NodePath PlayerPath = "../Player";
    [Export] public NodePath AudioManagerPath = "../AudioManager";
    [Export] public NodePath LevelLabelPath = "../UI/LevelLabel";
    [Export] public int CurrentLevelIndex = 0;

    private PlayerController _player;
    private AudioManager _audioManager;
    private Label _levelLabel;
    private int _highestUnlockedLevelIndex;

    public override void _Ready()
    {
        _player = GetNodeOrNull<PlayerController>(PlayerPath);
        _audioManager = GetNodeOrNull<AudioManager>(AudioManagerPath);
        _levelLabel = GetNodeOrNull<Label>(LevelLabelPath);
        if (_player == null)
        {
            GD.PushError("GameManager could not find PlayerController node.");
            return;
        }

        _highestUnlockedLevelIndex = SaveData.LoadHighestUnlockedLevelIndex();
        var requestedStart = Mathf.Clamp(LevelSession.StartLevelIndex, 0, LevelCatalog.Count - 1);
        CurrentLevelIndex = Mathf.Min(requestedStart, _highestUnlockedLevelIndex);

        _player.LevelCompleted += OnLevelCompleted;
        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        if (LevelCatalog.Count == 0)
        {
            GD.PushWarning("No level files configured in GameManager.");
            return;
        }

        CurrentLevelIndex = Mathf.Clamp(CurrentLevelIndex, 0, LevelCatalog.Count - 1);
        var levelPath = LevelCatalog.Files[CurrentLevelIndex];

        if (!_player.LoadLevel(levelPath))
        {
            GD.PushError($"Failed to load level: {levelPath}");
            return;
        }

        UpdateLevelLabel();
    }

    private void OnLevelCompleted()
    {
        _audioManager?.PlayWin();

        if (CurrentLevelIndex < LevelCatalog.Count - 1)
        {
            UnlockLevel(CurrentLevelIndex + 1);
            CurrentLevelIndex++;
            LoadCurrentLevel();
            return;
        }

        UnlockLevel(CurrentLevelIndex);
        GD.Print("All configured levels are complete.");
    }

    public void RestartCurrentLevel()
    {
        LoadCurrentLevel();
    }

    public void GoToLevelSelect()
    {
        LevelSession.StartLevelIndex = CurrentLevelIndex;
        GetTree().ChangeSceneToFile("res://scenes/LevelSelect.tscn");
    }

    public void GoToMainMenu()
    {
        LevelSession.StartLevelIndex = 0;
        GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
    }

    private void UnlockLevel(int levelIndex)
    {
        var clamped = Mathf.Clamp(levelIndex, 0, LevelCatalog.Count - 1);
        if (clamped <= _highestUnlockedLevelIndex)
            return;

        _highestUnlockedLevelIndex = clamped;
        SaveData.SaveHighestUnlockedLevelIndex(_highestUnlockedLevelIndex);
    }

    private void UpdateLevelLabel()
    {
        if (_levelLabel == null)
            return;

        _levelLabel.Text = $"Level {CurrentLevelIndex + 1}/{LevelCatalog.Count}";
    }
}
