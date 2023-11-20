namespace MadnessMicroactive;

public struct EnemyPreset
{
    public Outfit Outfit;
    public CharacterStats Stats;

    public EnemyPreset(Outfit outfit, CharacterStats stats)
    {
        Outfit = outfit;
        Stats = stats;
    }
}