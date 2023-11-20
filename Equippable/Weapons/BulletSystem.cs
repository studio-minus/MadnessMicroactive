using System.Numerics;
using Walgelijk;
using Walgelijk.Physics;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public class BulletSystem : Walgelijk.System
{
    public override void FixedUpdate()
    {
        foreach (var b in Scene.GetAllComponentsOfType<BulletComponent>())
        {
            b.LastPosition = b.Position;
            if (Scene.GetSystem<PhysicsSystem>().Raycast(b.Position, b.Velocity, out var result, b.Velocity.Length()))
                OnBulletHit(b, result);
            else
            {
                b.Position += b.Velocity;
                b.Velocity = b.Position - b.LastPosition;
            }

            b.Velocity.Y -= Time.FixedInterval * 100;
            b.Velocity *= 0.99f;

            if (b.Position.Y <= LevelComponent.FloorLevel)
            {
                b.Position.Y = LevelComponent.FloorLevel; // TODO line intersect
                b.Velocity = Vector2.Reflect(b.Velocity, Vector2.UnitY) * 0.1f;
                b.Damage = 0; // becomes harmless
            }

            b.Time += Time.FixedInterval;
            if (b.Time > 10 || b.Velocity.LengthSquared() <= 1)
                Scene.RemoveEntity(b.Entity);
        }
    }

    private void OnBulletHit(BulletComponent b, in RaycastResult result)
    {
        if (b.Damage > 0 && Scene.TryGetComponentFrom<LimbComponent>(result.Entity, out var bodyPart) && bodyPart.Character.TryGet(Scene, out var hitChar))
        {
            if (Utilities.RandomFloat() > 0.3f && Scene.GetSystem<PhysicsSystem>().Raycast(result.Position - result.Normal, b.Velocity, out var penetration))
            {
                b.Position = penetration.Position - result.Normal * 2;
                b.Velocity = (b.Position - b.LastPosition) * Utilities.RandomFloat(0.1f, 0.8f);
            }
            else
            {
                b.Position = result.Position + result.Normal;
                b.Velocity = b.Position - b.LastPosition;
            }

            hitChar.Damage(Scene, b.Damage);
            hitChar.DamageJump += Utilities.RandomFloat(5, 10);
        }
        else
            b.Time = float.MaxValue;

        b.Damage = 0; // becomes harmless
    }

    public override void Render()
    {
        Draw.Reset();
        Draw.Colour = Colors.White;

        foreach (var item in Scene.GetAllComponentsOfType<BulletComponent>())
        {
            Draw.Line(item.Position, item.LastPosition, 4);
        }
    }
}
