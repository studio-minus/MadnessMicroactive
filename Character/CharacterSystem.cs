using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class CharacterSystem : Walgelijk.System
{
    public override void FixedUpdate()
    {
        foreach (var character in Scene.GetAllComponentsOfType<CharacterComponent>())
        {
            var pose = character.Pose;

            foreach (var v in character.LimbPositions.Values)
            {
                v.PositionVelocity *= 0.6f;
                v.RotationVelocity *= 0.6f;
            }

            if (!character.IsAlive)
            {
                character.DeadTime += Time.FixedInterval;
                character.AimDirection = Vector2.UnitX;

                if (Scene.HasComponent<DespawnWhenDeadComponent>(character.Entity) &&
                    character.DeadTime > 5)
                    character.RemoveFromScene(Scene);
            }
            else
            {
                character.DeadTime = 0;
                ProcessWalking(character);

                character.RegenCooldownTimer += Time.FixedInterval;

                if (character.Health < character.Stats.MaxHealth && character.RegenCooldownTimer > 1)
                {
                    character.Health += character.Stats.RegenSpeed * Time.FixedInterval;
                    character.Health = Math.Min(character.Health, character.Stats.MaxHealth);
                }
            }

            PositionBody(character, pose);
            PositionHead(character, pose);
            PositionHands(character, pose);
            PositionFeet(character, pose);
            SetSkinColour(character);

            character.Recoil *= 0.5f;
            character.DamageJump *= 0.8f;

            if (character.Body.TryGet(Scene, out var body))
            {
                var v = character.LimbPositions[Limb.Body].Update(body.Position, body.Rotation);
                body.Position += v.PositionVelocity;
                body.Rotation += v.RotationVelocity;
            }

            if (character.Head.TryGet(Scene, out var head))
            {
                var v = character.LimbPositions[Limb.Head].Update(head.Position, head.Rotation, 0.3f);
                head.Position += v.PositionVelocity;
                head.Rotation += v.RotationVelocity;
            }

            if (character.Hand1.TryGet(Scene, out var hand1) && character.Hand2.TryGet(Scene, out var hand2))
            {
                var v1 = character.LimbPositions[Limb.Hand1].Update(hand1.Position, hand1.Rotation);
                hand1.Position += v1.PositionVelocity;
                hand1.Rotation += v1.RotationVelocity;

                if (character.Equipped.TryGet(Scene, out var equipped) && equipped.HoldPoints.Length > 1)
                {
                    //                                       subtracting because we are adding hand1.Position later, so the origin is effectively the first hold point
                    var hp = equipped.GetHoldPoint(1, character.Flipped) - equipped.GetHoldPoint(0, character.Flipped);
                    hand2.Position = Utilities.RotatePoint(hp, hand1.Rotation) + hand1.Position;
                    hand2.Rotation = hand1.Rotation;
                }
                else
                {
                    var v2 = character.LimbPositions[Limb.Hand2].Update(hand2.Position, hand2.Rotation);
                    hand2.Rotation += v2.RotationVelocity;
                    hand2.Position += v2.PositionVelocity;
                }
            }

            if (character.Foot1.TryGet(Scene, out var foot1) && character.Foot2.TryGet(Scene, out var foot2))
            {
                var v1 = character.LimbPositions[Limb.Foot1].Update(foot1.Position, foot1.Rotation);
                var v2 = character.LimbPositions[Limb.Foot2].Update(foot2.Position, foot2.Rotation);

                foot1.Position += v1.PositionVelocity;
                foot2.Position += v2.PositionVelocity;

                foot1.Rotation += v1.RotationVelocity;
                foot2.Rotation += v2.RotationVelocity;
            }
        }
    }

    private void SetSkinColour(CharacterComponent character)
    {
        if (Scene.TryGetComponentFrom<BatchedSpriteComponent>(character.Head.Entity, out var head))
            head.Color = character.SkinColour;

        if (Scene.TryGetComponentFrom<BatchedSpriteComponent>(character.Hand1.Entity, out var hand1))
            hand1.Color = character.SkinColour;

        if (Scene.TryGetComponentFrom<BatchedSpriteComponent>(character.Hand2.Entity, out var hand2))
            hand2.Color = character.SkinColour;
    }

    private void ProcessWalking(CharacterComponent c)
    {
        ref var s = ref c.WalkState;

        s.MainTimer += Time.FixedInterval;

        if (!s.Flying)
        {
            if (s.InHop)
            {
                s.HopTimer += Time.FixedInterval / s.HopDuration;

                if (s.HopTimer > 1)
                {
                    c.BottomCenter.Y = LevelComponent.FloorLevel;

                    s.Origin = c.BottomCenter;
                    s.Destination = s.Origin + c.WalkAcceleration * Utilities.RandomFloat(0.9f, 1);
                    s.HopTimer = 0;
                    s.HopDuration = 0.25f;

                    s.InHop = c.WalkAcceleration.LengthSquared() > float.Epsilon;
                }

                var t = Utilities.Lerp(Easings.Quad.InOut(s.HopTimer), s.HopTimer, 0.5f);
                c.BottomCenter = Utilities.Lerp(s.Origin, s.Destination, t);
                var hopCurve = 1 - (4 * (s.HopTimer - 0.5f) * (s.HopTimer - 0.5f));
                c.BottomCenter.Y += Easings.Cubic.In(hopCurve) * 7;
            }
            else
            {
                s.HopTimer = float.MaxValue;
                s.InHop = c.WalkAcceleration.LengthSquared() > float.Epsilon;
            }

            if (Scene.FindAnyComponent<LevelComponent>(out var level))
                c.BottomCenter.X = Math.Clamp(c.BottomCenter.X, level.WalkingRange.X, level.WalkingRange.Y);
            else
                c.BottomCenter.X = Math.Clamp(c.BottomCenter.X, -600, 600);
        }
    }

    private void PositionFeet(CharacterComponent character, in CharacterPose pose)
    {
        var foot1Pos = character.Flipped ? pose.Foot1.Flipped : pose.Foot1;
        var foot2Pos = character.Flipped ? pose.Foot2.Flipped : pose.Foot2;
        var foot1Sprite = Scene.GetComponentFrom<BatchedSpriteComponent>(character.Foot1.Entity);
        var foot2Sprite = Scene.GetComponentFrom<BatchedSpriteComponent>(character.Foot2.Entity);

        foot1Sprite.RenderOrder = new RenderOrder(character.RenderLayer, foot1Pos.Order);
        foot2Sprite.RenderOrder = new RenderOrder(character.RenderLayer, foot2Pos.Order);

        foot1Sprite.VerticalFlip = character.Flipped;
        foot2Sprite.VerticalFlip = character.Flipped;

        var pos1 = character.BottomCenter + foot1Pos.Translation;
        var pos2 = character.BottomCenter + foot2Pos.Translation;

        if (character.WalkState.InHop)
        {
            var t = Utilities.Clamp(character.WalkState.HopTimer) * -MathF.Tau * Math.Sign(character.WalkAcceleration.X);
            pos1.X += MathF.Cos(t) * 4;
            pos1.Y += MathF.Sin(t) * 4;

            pos2.X += MathF.Cos(t + MathF.PI) * 4;
            pos2.Y += MathF.Sin(t + MathF.PI) * 4;
        }

        character.LimbPositions[Limb.Foot1].SetTarget(pos1, foot1Pos.Rotation);
        character.LimbPositions[Limb.Foot2].SetTarget(pos2, foot2Pos.Rotation);
    }

    private void PositionHands(CharacterComponent character, in CharacterPose pose)
    {
        var hand1Pos = character.Flipped ? pose.Hand1.Flipped : pose.Hand1;
        var hand2Pos = character.Flipped ? pose.Hand2.Flipped : pose.Hand2;
        var hand1Sprite = Scene.GetComponentFrom<BatchedSpriteComponent>(character.Hand1.Entity);
        var hand2Sprite = Scene.GetComponentFrom<BatchedSpriteComponent>(character.Hand2.Entity);

        if (character.Equipped.TryGet(Scene, out var equipped) && equipped.HoldPoints.Length > 1)
            hand2Pos.Order = hand1Pos.Order;

        hand1Sprite.RenderOrder = new RenderOrder(character.RenderLayer, hand1Pos.Order);
        hand2Sprite.RenderOrder = new RenderOrder(character.RenderLayer, hand2Pos.Order);

        var recoil = (character.Flipped ? -character.Recoil : character.Recoil);

        var a = character.BottomCenter + hand1Pos.Translation;
        var b = character.BottomCenter + hand2Pos.Translation;
        var th = (Utilities.VectorToAngle(character.AimDirection) + (character.Flipped ? 180 : 0)) + recoil;

        a = pose.AimAdjust.Item1 ? Utilities.RotatePoint(a, th, character.BottomCenter + new Vector2(0, 120)) : a;
        b = pose.AimAdjust.Item2 ? Utilities.RotatePoint(b, th, character.BottomCenter + new Vector2(0, 120)) : b;

        if (pose.AimAdjust.Item1)
        {
            a -= character.AimDirection * character.Recoil * 2;
            hand1Pos.Rotation = th;
        }

        if (pose.AimAdjust.Item2)
        {
            b -= character.AimDirection * character.Recoil * 2;
            hand2Pos.Rotation = th;
        }

        character.LimbPositions[Limb.Hand1].SetTarget(a, hand1Pos.Rotation + recoil * 2);
        character.LimbPositions[Limb.Hand2].SetTarget(b, hand2Pos.Rotation + recoil * 2);
    }

    private void PositionHead(CharacterComponent character, in CharacterPose pose)
    {
        var sprite = Scene.GetComponentFrom<BatchedSpriteComponent>(character.Head.Entity);
        var pos = character.Facing is Facing.Right ? pose.Head : pose.Head.Flipped;

        sprite.RenderOrder = new RenderOrder(character.RenderLayer, pos.Order);

        sprite.VerticalFlip = character.Flipped;
        var p = character.BottomCenter + pos.Translation;
        var r = pos.Rotation;

        var v = character.AimDirection;
        v.Y *= 0.3f * Math.Sign(v.X);
        v.X = 1;
        p.X += v.Y * -50;
        r += Utilities.VectorToAngle(v);

        var recoil = Math.Clamp(character.Recoil * 0.5f, 0, 5) * (character.Flipped ? -1 : 1);
        r -= recoil;
        p.X -= recoil * 0.2f;

        var damageJump = character.DamageJump * (character.Flipped ? -1 : 1); ;
        r += damageJump * 2.5f;
        p.X -= damageJump * 2;

        character.LimbPositions[Limb.Head].SetTarget(p, r);
    }

    private void PositionBody(CharacterComponent character, in CharacterPose pose)
    {
        var sprite = Scene.GetComponentFrom<BatchedSpriteComponent>(character.Body.Entity);
        var pos = character.Flipped ? pose.Body.Flipped : pose.Body;

        sprite.RenderOrder = new RenderOrder(character.RenderLayer, pos.Order);

        sprite.VerticalFlip = character.Flipped;
        var p = character.BottomCenter + pos.Translation;
        var r = pos.Rotation;

        var v = character.AimDirection;
        v.Y *= 0.1f * Math.Sign(v.X);
        v.X = 1;
        p.X += v.Y * -40;
        r += Utilities.VectorToAngle(v);

        var recoil = Math.Clamp(character.Recoil * 0.5f, 0, 5) * (character.Flipped ? -1 : 1);
        r += recoil;
        p.X -= recoil * 0.5f;

        var damageJump = character.DamageJump * (character.Flipped ? -1 : 1); ;
        r += damageJump;
        p.X -= damageJump;

        character.LimbPositions[Limb.Body].SetTarget(p, r);
    }

    public override void Update()
    {
#if DRAW_HEALTH
        Draw.Reset();

        foreach (var item in Scene.GetAllComponentsOfType<CharacterComponent>())
        {
            var p = item.Health / item.Stats.MaxHealth;

            var a = item.NeckPoint + new Vector2(-50, 150);
            var b = item.NeckPoint + new Vector2(50, 150);

            Draw.Colour = Colors.Black;
            Draw.Line(a, b, 10, 5);
            Draw.Colour = Colors.GreenYellow;
            Draw.Line(a, Utilities.Lerp(a, b, p), 10, 5);
        }
#endif
    }
}


public class DespawnWhenDeadComponent : Component
{

}
