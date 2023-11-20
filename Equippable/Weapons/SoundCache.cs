using Walgelijk;

namespace MadnessMicroactive;

public class SoundCache : Cache<(AudioData data, bool loops), Sound>
{
    public static readonly SoundCache Instance = new();

    protected override Sound CreateNew((AudioData data, bool loops) raw)
    {
        Logger.Log($"Sound cached: {raw.GetHashCode()}");
        return new Sound(raw.data, raw.loops);
    }

    public Sound LoadMusic(AudioData data)
    {
        return Load((data, true));
    }

    public Sound LoadMusicNonLoop(AudioData data)
    {
        return Load((data, false));
    }

    public Sound LoadSoundEffect(AudioData data)
    {
        return Load((data, false));
    }

    public Sound LoadUISoundEffect(AudioData data)
    {
        return Load((data, false));
    }

    protected override void DisposeOf(Sound loaded) { }
}