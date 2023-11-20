using System.Numerics;
using Walgelijk;
using Walgelijk.ParticleSystem;

namespace MadnessMicroactive;

public struct FloorCollision : IParticleModule
{
    public bool Disabled { get; set; }

    public float BounceFactor = 0.4f;
    public float DampeningFactor = 0.9f;
    public FloatRange FloorOffset = new FloatRange(-40, 10);

    public Hook<Particle> OnCollide = new();

    public FloorCollision(float bounceFactor, float dampeningFactor) : this()
    {
        BounceFactor = bounceFactor;
        DampeningFactor = dampeningFactor;
    }

    public void Process(int index, ref Particle particle, in GameState gameState, ParticlesComponent component, TransformComponent transform)
    {
        var floor = LevelComponent.FloorLevel + Utilities.Lerp(FloorOffset.Min, FloorOffset.Max, Utilities.Hash(index * 0.52314f));


        var isHit = particle.Position.Y <= floor;
        if (isHit)
        {
            particle.Position.Y = floor;
            var normal = Vector2.UnitY;

            if (particle.Velocity.LengthSquared() > 10000)
            {
                particle.RotationalVelocity *= -1;
                OnCollide.Dispatch(particle);
                particle.Velocity = Vector2.Reflect(particle.Velocity, normal) * BounceFactor;
                particle.Rotation = Utilities.VectorToAngle(normal) + 90;
                particle.RotationalVelocity *= 0.5f;
            }
            else
            {
                particle.RotationalVelocity = default;
                particle.Velocity = default;
            }
        }
    }
}
