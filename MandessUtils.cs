using System.Numerics;
using Walgelijk;
using Walgelijk.ParticleSystem.ParticleInitialisers;
using Walgelijk.ParticleSystem;
using Walgelijk.ParticleSystem.Modules;

namespace MadnessMicroactive;

public class MandessUtils
{
    public static readonly Tag CasingParticlesTag = Tag.CreateUnique();

    public static void EjectCasing(Scene scene, TransformComponent transform, Vector2 pos, Vector2 vel)
    {
        if (scene.TryGetEntityWithTag(CasingParticlesTag, out var ent) && scene.TryGetComponentFrom<ParticlesComponent>(ent, out var p))
        {
            var particle = p.GenerateParticleObject(scene.Game.State, transform);
            particle.Rotation = transform.Rotation;
            particle.Size = 8;
            particle.Position = pos;
            particle.Velocity = vel;
            scene.GetSystem<ParticleSystem>().CreateParticle(p, particle);
        }
    }

    public static ParticlesComponent CreateCasingEjectionParticleSystem(Scene scene, Material ejectedMaterial)
    {
        var ent = scene.CreateEntity();
        scene.AttachComponent(ent, new TransformComponent());
        scene.SetTag(ent, CasingParticlesTag);
        var particles = scene.AttachComponent(ent, new ParticlesComponent
        {
            WorldSpace = true,
            Material = ejectedMaterial,
            Depth = new RenderOrder(-2),
            SimulationSpeed = 2
        });

        particles.Initalisers.Add(new RandomLifespan(new(10, 20)));
        particles.Initalisers.Add(new RandomStartSize(new(400)));
        particles.Initalisers.Add(new RandomStartRotVel(new(-1500, 1500)));
        particles.Initalisers.Add(new RandomStartVelocity(new(new Vector2(-500, 2500), new Vector2(500, 9500))));

        var floorCollision = new FloorCollision(0.4f, 0.95f);
        floorCollision.OnCollide.AddListener(collisionHandler);
        particles.Modules.Add(new GravityModule(new Vec2Range(new Vector2(0, -2500))));
        particles.Modules.Add(floorCollision);

        void collisionHandler(Particle p)
        {
            if (p.Velocity.LengthSquared() > 200000) // epic trail-and-error picked number
                Game.Main.AudioRenderer.PlayOnce(SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("wep/casing.wav")));
        }

        return particles;
    }
}
