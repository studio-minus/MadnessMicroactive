using Walgelijk;

namespace MadnessMicroactive;

[RequiresComponents(typeof(EquippableComponent))]
public class WeaponComponent : Component
{
    public readonly IWeapon Weapon;

    public float AutoTimer;
    public int RemainingRounds;
    public FixedIntervalDistributor AutoDistributor = new();

    public WeaponComponent(IWeapon weapon)
    {
        Weapon = weapon;
    }
}
