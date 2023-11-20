using Walgelijk;

namespace MadnessMicroactive;

public class MusicSystem : Walgelijk.System
{
    private static Sound splitmek = SoundCache.Instance.LoadMusicNonLoop(Resources.Load<StreamAudioData>("music/splitmek.ogg"));
    private static Sound jebus = SoundCache.Instance.LoadMusicNonLoop(Resources.Load<StreamAudioData>("music/jebus.ogg"));
    private static Sound monks = SoundCache.Instance.LoadMusicNonLoop(Resources.Load<StreamAudioData>("music/monks.ogg"));

    public override void FixedUpdate()
    {
        if (Scene.HasSystem<MenuScene.MainMenuSystem>())
        {
            Audio.Stop(jebus);
            Audio.Stop(monks);
            if (!Audio.IsPlaying(splitmek))
                Audio.Play(splitmek);
        }
        else if (Scene.FindAnyComponent<DeathSequenceComponent>(out var _))
        {
            Audio.Stop(splitmek);
            Audio.Stop(jebus);
            Audio.Stop(monks);
        }
        else if (Scene.FindAnyComponent<BossControllerComponent>(out _))
        {
            Audio.Stop(splitmek);
            Audio.Stop(monks);
            if (!Audio.IsPlaying(jebus))
                Audio.Play(jebus);
        }
        else
        {
            Audio.Stop(jebus);

            if (!Audio.IsPlaying(splitmek) && !Audio.IsPlaying(monks))
                Audio.Play(Utilities.PickRandom(splitmek, monks));
        }
    }
}