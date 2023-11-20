namespace MadnessMicroactive;

public class WeaponSystem : Walgelijk.System
{
    public override void FixedUpdate()
    {
        foreach (var wc in Scene.GetAllComponentsOfType<WeaponComponent>())
        {
            var eq = Scene.GetComponentFrom<EquippableComponent>(wc.Entity);

            if (wc.Weapon is Firearm firearm)
            {
                if (wc.RemainingRounds <= 0)
                    eq.Enabled = false;
            }
        }
    }
}
