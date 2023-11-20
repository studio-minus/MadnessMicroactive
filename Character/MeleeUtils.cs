using Walgelijk;
using Walgelijk.Physics;

namespace MadnessMicroactive;

public static class MeleeUtils
{
    private readonly static Sound[] swishes =
    {
        SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("melee/swish_1.wav")),
        SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("melee/swish_2.wav")),
        SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("melee/swish_3.wav"))
    };  
    
    private readonly static Sound[] punches =
    {
        SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("melee/punch_1.wav")),
        SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("melee/punch_2.wav")),
        SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("melee/punch_3.wav")),
        SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("melee/punch_4.wav")),
        SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("melee/punch_6.wav"))
    };

    private static QueryResult[] results = new QueryResult[8];

    public static IEnumerator<IRoutineCommand> MeleeSequence(CharacterComponent character)
    {
        var d = character.AimDirection * 100;
        if (character.Flipped)
            d.X *= -1;
        float t = 0;
        Game.Main.AudioRenderer.PlayOnce(Utilities.PickRandom(swishes));
        DoMeleeDamage(Game.Main.Scene, character);
        while (t < 0.1f)
        {
            if (character.MeleeFlipFlop)
                character.Pose.Hand2.Translation += d;
            else
                character.Pose.Hand1.Translation += d;

            t += Game.Main.State.Time.DeltaTime;
            yield return new RoutineFrameDelay();
        }
    }

    public static void DoMeleeDamage(Scene scene, CharacterComponent actor)
    {
        var pos = actor.NeckPoint + actor.AimDirection * 100;
        var radius = 50;
        //scene.Game.DebugDraw.Circle(pos, radius, Colors.Red, 1);

        if (scene.TryGetSystem<PhysicsSystem>(out var sys))
        {
            int hitCount = sys.QueryCircle(pos, radius, ref results);
            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    var hit = results[i];
                    if (scene.TryGetComponentFrom<LimbComponent>(hit.Entity, out var limb))
                    {
                        var character = limb.Character.Get(scene);
                        if (character.IsAlive && actor.Faction.Enemies.Contains(character.Faction))
                        {
                            character.DamageJump += 5;
                            character.Damage(scene, 20);
                            Game.Main.AudioRenderer.PlayOnce(Utilities.PickRandom(punches));
                            return;
                    }
                    }
                }
            }
        }
    }
}