using Walgelijk;

namespace MadnessMicroactive;

public interface IWeapon
{
    public string Name { get; }
    public void Use(Scene scene, WeaponComponent weapon, CharacterComponent wielder, float dt);
}
