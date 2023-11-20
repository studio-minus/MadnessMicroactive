using Walgelijk;

namespace MadnessMicroactive;

public class AiControllerComponent : Component
{
    public ComponentRef<CharacterComponent> KillTarget;
    public ComponentRef<EquippableComponent> PickUpTarget;

    public bool IsAttacking;
    public float AttackTimer;
    public float MovementSpeedMultiplier = 1;
}
