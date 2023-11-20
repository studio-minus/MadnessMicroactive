using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class EquippableSystem : Walgelijk.System
{
    public override void FixedUpdate()
    {
        foreach (var equippable in Scene.GetAllComponentsOfType<EquippableComponent>())
        {
            var transform = Scene.GetComponentFrom<TransformComponent>(equippable.Entity);
            var sprite = Scene.GetComponentFrom<BatchedSpriteComponent>(equippable.Entity);
            var pos = transform.Position;

            if (equippable.Wielder.TryGet(Scene, out var wielder))
            {
                if (wielder.Hand1.TryGet(Scene, out var hand1))
                {
                    pos = Utilities.RotatePoint(hand1.Position - equippable.GetHoldPoint(0, wielder.Flipped), hand1.Rotation, hand1.Position);
                    transform.Rotation = hand1.Rotation;
                    sprite.VerticalFlip = wielder.Flipped;
                    sprite.RenderOrder = new RenderOrder(wielder.RenderLayer, wielder.Pose.Hand1.Order - 1);
                }

                equippable.DecayTimer = 0;
            }
            else
            {
                if (!equippable.Enabled)
                {
                    equippable.DecayTimer += Time.FixedInterval;
                    sprite.Color.A = 0.3f;

                    if (equippable.DecayTimer > 5)
                        Scene.RemoveEntity(equippable.Entity);
                }
                else
                {
                    equippable.DecayTimer = 0;
                    sprite.Color.A = 1;
                }

                equippable.Velocity *= 0.95f;
                pos += equippable.Velocity;

                equippable.Velocity.Y += 200 * -Time.FixedInterval;
                if (transform.Position.Y <= LevelComponent.FloorLevel)
                {
                    equippable.Velocity = default;
                    transform.Rotation = 0;
                    pos = new Vector2(pos.X, LevelComponent.FloorLevel);
                }
            }

            transform.Position = pos;
        }
    }
}
