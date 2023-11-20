using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class PlayerControllerSystem : Walgelijk.System
{
    public override void Update()
    {
        foreach (var player in Scene.GetAllComponentsOfType<PlayerControllerComponent>())
        {
            const float Speed = 160;
            var character = Scene.GetComponentFrom<CharacterComponent>(player.Entity);

            Window.CursorAppearance = default;

            if (!character.IsAlive)
                return;

            character.Pose = CharacterPoses.Idle;

            character.WalkAcceleration = default;
            if (Input.IsKeyPressed(Key.D) || Input.IsKeyReleased(Key.D))
                character.WalkState.HopTimer = float.MaxValue;
            else if (Input.IsKeyHeld(Key.D))
                character.WalkAcceleration.X += Speed;

            if (Input.IsKeyPressed(Key.A) || Input.IsKeyReleased(Key.A))
                character.WalkState.HopTimer = float.MaxValue;
            else if (Input.IsKeyHeld(Key.A))
                character.WalkAcceleration.X -= Speed;

            if (Input.IsKeyPressed(Key.E))
            {
                var maxDistanceFromPlayer = character.Equipped.IsValid(Scene) ? 50 * 50 : 200 * 200;
                var minDist = float.MaxValue; ;
                EquippableComponent? nearest = null;
                foreach (var eq in Scene.GetAllComponentsOfType<EquippableComponent>())
                {
                    if (eq.Entity == character.Equipped.Entity || !eq.Enabled || eq.IsEquipped(Scene))
                        continue;
                    var transform = Scene.GetComponentFrom<TransformComponent>(eq.Entity);
                    var distanceFromPlayer = Vector2.DistanceSquared(character.BottomCenter, transform.Position);

                    if (distanceFromPlayer < maxDistanceFromPlayer)
                    {
                        var dist = Vector2.DistanceSquared(transform.Position, Input.WorldMousePosition);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearest = eq;
                        }
                    }
                }
                if (nearest != null)
                    character.Equip(Scene, nearest);
                else
                    character.Drop(Scene);
            }

            if (character.Equipped.TryGet(Scene, out var equipped))
            {
                if (Input.IsButtonHeld(MouseButton.Right))
                {
                    character.Pose = Input.IsKeyHeld(Key.LeftShift) ? CharacterPoses.FocusedAiming : CharacterPoses.Aiming;

                    if (Scene.TryGetComponentFrom<WeaponComponent>(equipped.Entity, out var wpn))
                        if (Input.IsButtonHeld(MouseButton.Left))
                        {
                            if (Input.IsButtonPressed(MouseButton.Left))
                                wpn.AutoTimer = float.MaxValue;
                            wpn.Weapon.Use(Scene, wpn, character, Time.DeltaTime);
                        }
                        else
                            wpn.AutoTimer = 0;
                }
                else
                    character.Pose = equipped.HoldPoints.Length == 1 ? CharacterPoses.CarrySingleHanded : CharacterPoses.CarryTwoHanded;
            }
            else
            {
                if (Input.IsButtonHeld(MouseButton.Right))
                {
                    character.Pose = CharacterPoses.MeleeStance;


                    if (Input.IsButtonPressed(MouseButton.Left))
                    {
                        character.MeleeFlipFlop = !character.MeleeFlipFlop;
                        RoutineScheduler.Start(MeleeUtils.MeleeSequence(character));
                    }
                }
            }
        }
    }

    public override void FixedUpdate()
    {
        foreach (var player in Scene.GetAllComponentsOfType<PlayerControllerComponent>())
        {
            var character = Scene.GetComponentFrom<CharacterComponent>(player.Entity);

            if (!character.IsAlive)
                return;

            character.AimDirection = Utilities.SmoothApproach(character.AimDirection,
                Vector2.Normalize(
                Input.WorldMousePosition - character.NeckPoint),
                25, Time.FixedInterval);

            character.Facing = character.AimDirection.X < 0 ? Facing.Left : Facing.Right;
        }
    }
}
