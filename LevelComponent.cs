using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

[SingleInstance]
public class LevelComponent : Component
{
    public const float FloorLevel = 50;

    public int MaxEnemies = 2;
    public int TotalEnemies = 5;
    public float EnemySpawnRate = 1;
    public IWeapon[] Weapons = Array.Empty<IWeapon>();
    public EnemyPreset[] Enemies = Array.Empty<EnemyPreset>();
    public IReadableTexture Background;

    public Vector2 WalkingRange = new(-600, 600);

    public LevelComponent(IReadableTexture background)
    {
        Background = background;
    }

    [Command]
    public static void SetLevel(int level)
    {
        PersistentData.Instance.LevelIndex = level;
        Game.Main.Scene = LevelScene.Create(Game.Main, Levels.Campaign[level]);
    }
}
