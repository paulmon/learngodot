using Godot;

public partial class AudioManager : Node
{
    [Export] public AudioStream BackgroundMusic;
    [Export] public AudioStream MoveSfx;
    [Export] public AudioStream PushSfx;
    [Export] public AudioStream GoalSfx;
    [Export] public AudioStream WinSfx;
    [Export] public AudioStream FailSfx;

    private AudioStreamPlayer _bgmPlayer;
    private AudioStreamPlayer _sfxPlayer;

    public override void _Ready()
    {
        _bgmPlayer = GetNodeOrNull<AudioStreamPlayer>("BgmPlayer");
        _sfxPlayer = GetNodeOrNull<AudioStreamPlayer>("SfxPlayer");

        if (_bgmPlayer != null)
        {
            _bgmPlayer.Stream = BackgroundMusic;
            _bgmPlayer.Autoplay = false;
            _bgmPlayer.Bus = "Master";
            _bgmPlayer.StreamPaused = false;
            _bgmPlayer.VolumeDb = -6.0f;
            _bgmPlayer.Play();
        }
    }

    public void PlayMove() => PlaySfx(MoveSfx);
    public void PlayPush() => PlaySfx(PushSfx);
    public void PlayGoalComplete() => PlaySfx(GoalSfx);
    public void PlayWin() => PlaySfx(WinSfx);
    public void PlayFail() => PlaySfx(FailSfx);

    private void PlaySfx(AudioStream stream)
    {
        if (_sfxPlayer == null || stream == null)
            return;

        _sfxPlayer.Stream = stream;
        _sfxPlayer.Play();
    }
}
