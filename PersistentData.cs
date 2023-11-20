namespace MadnessMicroactive;

public class PersistentData
{
    public static PersistentData Instance { get; } = new();

    public IWeapon? Weapon;
    public int Ammo;
    public float Health = 500;
    public int Kills;
    public int LevelIndex;
}
