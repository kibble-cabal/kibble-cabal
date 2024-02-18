using Godot;
using NathanHoad;

public sealed partial class MusicSubSystemBase : DependentSubSystem
{
    public override ISubSystem[] Dependencies => [
        LocationSubSystem.Instance
    ];

    public override void OnDependenciesReady()
    {
        LocationSubSystem.Instance.Connect(
            LocationSubSystemBase.SignalName.LocationChanged,
            Callable.From(OnLocationChanged)
        );
        OnLocationChanged();
    }

    // TODO: user setting
    public float GetMusicVolume() => 0.0f;
    // TODO: user setting
    public float GetMusicFadeDuration() => 1.0f;

    private void OnLocationChanged()
    {
        if (LocationSubSystem.GetLocation() is RLocation location)
        {
            var music = location.GetMusic();
            PlayMusic(music?.GetSong());
        }
    }

    private void PlayMusic(AudioStream? song)
    {
        if (song is null) StopMusic();
        else SoundManager.PlayMusicAtVolume(song, GetMusicVolume(), GetMusicFadeDuration());
    }

    private void StopMusic() => SoundManager.StopMusic(GetMusicFadeDuration());
}

public sealed partial class MusicSubSystem : Singleton<MusicSubSystemBase> { }