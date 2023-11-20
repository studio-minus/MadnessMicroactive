using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using Walgelijk;

namespace MadnessMicroactive;

public class AiControllerSystem : Walgelijk.System
{
    public override void Update()
    {
        var seed = 0f;
        foreach (var ai in Scene.GetAllComponentsOfType<AiControllerComponent>())
        {
            seed += 158.58f;
            var character = Scene.GetComponentFrom<CharacterComponent>(ai.Entity);
            character.WalkAcceleration = default;

            if (!character.IsAlive)
                continue;

            character.Pose = CharacterPoses.Idle;

            if (ai.PickUpTarget.TryGet(Scene, out var pickupTarget))
                // we have a pickup target so lets get it
                ProcessPickupTarget(ai, character, pickupTarget);
            else
                // no pick up target :) killing it 
                ProcessKillTarget(seed, ai, character);

            if (!ai.PickUpTarget.IsValid(Scene) && !ai.KillTarget.IsValid(Scene))
            {
                // no goal in life

                if (Noise.GetValue(seed, Time.SecondsSinceLoad, -72.634f) > 0.25f)
                    character.WalkAcceleration.X += Math.Sign(Noise.GetValue(seed, seed, Time.SecondsSinceLoad)) * Utilities.RandomFloat(20, 120) * ai.MovementSpeedMultiplier;

                character.Facing = Noise.GetValue(seed, Time.SecondsSinceLoad * 0.02f, 72.634f) > 0 ? Facing.Right : Facing.Left;
            }
        }
    }

    private bool IsEligibleForKillTarget(AiControllerComponent me, int tolerance)
    {
        if (tolerance == int.MaxValue)
            return true;

        foreach (var item in Scene.GetAllComponentsOfType<AiControllerComponent>())
        {
            if (!Scene.GetComponentFrom<CharacterComponent>(item.Entity).IsAlive)
                continue;

            if (item.KillTarget != me.KillTarget)
                continue;

            if (!item.KillTarget.IsValid(Scene))
                continue;

            if (item.Entity == me.Entity)
                return true;
            else
            {
                if (tolerance <= 0)
                    return false;
                tolerance--;
            }
        }

        return false;
    }

    private bool IsFirstInLineForItemTarget(AiControllerComponent me)
    {
        foreach (var item in Scene.GetAllComponentsOfType<AiControllerComponent>())
        {
            if (!Scene.GetComponentFrom<CharacterComponent>(item.Entity).IsAlive)
                continue;

            if (item.PickUpTarget != me.PickUpTarget)
                continue;

            if (!item.PickUpTarget.IsValid(Scene))
                continue;

            return item.Entity == me.Entity;
        }

        return false;
    }

    private void ProcessPickupTarget(AiControllerComponent ai, CharacterComponent character, EquippableComponent pickupTarget)
    {
        if (pickupTarget.IsEquipped(Scene) || !pickupTarget.Enabled || !IsFirstInLineForItemTarget(ai)) // nevermind fuck this
            ai.PickUpTarget = default;
        else
        {
            var pickupTransform = Scene.GetComponentFrom<TransformComponent>(pickupTarget.Entity);
            character.AimDirection = Vector2.Normalize(
                pickupTransform.Position - character.NeckPoint
                );

            var deltaToTarget = pickupTransform.Position.X - character.BottomCenter.X;
            if (Math.Abs(deltaToTarget) < 50)
            {
                // close enough to pick it up
                character.Pose = CharacterPoses.Aiming;
                character.Equip(Scene, pickupTarget);
                ai.PickUpTarget = default;
            }
            else
            {
                character.WalkAcceleration.X += Math.Clamp(deltaToTarget, -120, 120) * ai.MovementSpeedMultiplier;
                character.Facing = character.AimDirection.X > 0 ? Facing.Right : Facing.Left;
            }
        }
    }

    private void ProcessKillTarget(float seed, AiControllerComponent ai, CharacterComponent character)
    {
        // we need a weapon
        if (!character.Equipped.IsValid(Scene))
            ai.PickUpTarget = FindPickupTarget(character);

        if (!ai.KillTarget.TryGet(Scene, out var killTarget))
        {
            ai.KillTarget = FindKillTarget(character);
            if (character.Equipped.TryGet(Scene, out var equipped))
            {
                character.Pose = equipped.HoldPoints.Length == 1 ? CharacterPoses.CarrySingleHanded : CharacterPoses.CarryTwoHanded;
                if (Scene.TryGetComponentFrom<WeaponComponent>(equipped.Entity, out var w)
                    && w.Weapon is Firearm
                    && Noise.GetValue(Time.SecondsSinceLoad, seed, 23.03f) > 0.2f)
                {
                    if (w.RemainingRounds <= 0)
                        character.Drop(Scene);
                }
            }
        }
        else
        {
            if (!killTarget.IsAlive)
                ai.KillTarget = default;

            var aimOffset = 250 * new Vector2(
                    Noise.GetValue(Time.SecondsSinceLoad * 0.2f, seed, -seed),
                    Noise.GetValue(seed, -seed, Time.SecondsSinceLoad * 0.2f)
                );

            character.AimDirection = Vector2.Normalize((killTarget.NeckPoint + aimOffset) - character.NeckPoint);
            character.Facing = character.AimDirection.X > 0 ? Facing.Right : Facing.Left;

            var targetRange = 500f;

            if (!IsEligibleForKillTarget(ai, character.Stats.AiBusinessTolerance))
            {
                ai.AttackTimer = 0;
                targetRange = Utilities.RandomFloat(400, 500);
            }
            else if (character.Equipped.TryGet(Scene, out var equipped))
            {
                ai.AttackTimer += Time.DeltaTime;

                if (Noise.GetValue(seed, Time.SecondsSinceLoad / 2.5f, -110.44f) > -0.5f)
                {
                    character.Pose = Noise.GetValue(-64.42f, seed, Time.SecondsSinceLoad / 12.5f) > 0 ? CharacterPoses.FocusedAiming : CharacterPoses.Aiming;

                    if (ai.AttackTimer > (1 / (1 + character.Stats.AiBusinessTolerance) + 0.5f) && Scene.TryGetComponentFrom<WeaponComponent>(equipped.Entity, out var wpn))
                    {
                        var wasAttacking = ai.IsAttacking;
                        ai.IsAttacking = Noise.GetValue(seed, Time.SecondsSinceLoad * 0.5f, -310.23f) > 0.0f;

                        if (ai.IsAttacking)
                        {
                            if (!wasAttacking)
                                wpn.AutoTimer = float.MaxValue;

                            wpn.Weapon.Use(Scene, wpn, character, Time.DeltaTime);
                        }
                        else
                            wpn.AutoTimer = 0;
                    }
                }
                else
                    character.Pose = equipped.HoldPoints.Length == 1 ? CharacterPoses.CarrySingleHanded : CharacterPoses.CarryTwoHanded;

                if (Scene.TryGetComponentFrom<WeaponComponent>(equipped.Entity, out var w)
                    && w.Weapon is Firearm
                    && Noise.GetValue(Time.SecondsSinceLoad, seed, 23.03f) > 0.2f)
                {
                    if (w.RemainingRounds <= 0)
                        character.Drop(Scene);
                }
            }
            else
            {
                ai.AttackTimer = 0;

                if (Noise.GetValue(seed, Time.SecondsSinceLoad / 2.5f, -110.44f) > -0.5f)
                {
                    character.Pose = CharacterPoses.MeleeStance;
                    targetRange = 220;

                    var wasAttacking = ai.IsAttacking;
                    ai.IsAttacking = Noise.GetValue(seed, Time.SecondsSinceLoad * 0.5f, -310.23f) > 0.1f;

                    if (!wasAttacking && ai.IsAttacking)
                    {
                        character.MeleeFlipFlop = !character.MeleeFlipFlop;
                        RoutineScheduler.Start(MeleeUtils.MeleeSequence(character));
                    }
                }
            }

            var deltaToEnemy = killTarget.BottomCenter.X - character.BottomCenter.X;
            // TOO FAR!
            if (deltaToEnemy > targetRange)
                character.WalkAcceleration.X += MathF.Min(deltaToEnemy, 100) * ai.MovementSpeedMultiplier;
            else if (deltaToEnemy < -targetRange)
                character.WalkAcceleration.X += MathF.Max(deltaToEnemy, -100) * ai.MovementSpeedMultiplier;
            // TOO CLOSE!
            else if (Math.Abs(deltaToEnemy) < targetRange * 0.5f)
                character.WalkAcceleration.X -= Math.Sign(deltaToEnemy) * 90 * ai.MovementSpeedMultiplier;
            else if (Noise.GetValue(seed, Time.SecondsSinceLoad, 32.634f) > 0.2f)
                character.WalkAcceleration.X += Math.Sign(Noise.GetValue(seed, 32.634f, Time.SecondsSinceLoad)) * 30 * ai.MovementSpeedMultiplier;
        }
    }

    private ComponentRef<EquippableComponent> FindPickupTarget(CharacterComponent character)
    {
        foreach (var v in Scene.GetAllComponentsOfType<EquippableComponent>())
        {
            if (!v.Enabled || v.IsEquipped(Scene))
                continue;

            if (Scene.TryGetComponentFrom<WeaponComponent>(v.Entity, out var wc))
            {
                switch (wc.Weapon)
                {
                    case Firearm firearm:
                        {
                            if (wc.RemainingRounds > 0)
                                return v;
                        }
                        break;
                }
            }

        }
        return default;
    }

    private ComponentRef<CharacterComponent> FindKillTarget(CharacterComponent character)
    {
        foreach (var v in Scene.GetAllComponentsOfType<CharacterComponent>())
        {
            if (v == character || !v.IsAlive || !character.Faction.Enemies.Contains(v.Faction))
                continue;

            return v;
        }
        return default;
    }
}
