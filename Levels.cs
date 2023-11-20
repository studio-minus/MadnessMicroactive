using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public static class Levels
{
    public struct Level
    {
        public LevelComponent LevelComponent;
        public Action<Scene>? Init;

        public Level(LevelComponent levelComponent, Action<Scene>? init)
        {
            LevelComponent = levelComponent;
            Init = init;
        }

        public static implicit operator Level(LevelComponent lvl)
        {
            return new Level(lvl, null);
        }
    }

    public static Level[] Campaign =
    {
        new LevelComponent(Resources.Load<Texture>("levels/lvl1.qoi"))
        {
            EnemySpawnRate = 1,
            MaxEnemies = 1,
            TotalEnemies = 10,
            Weapons = new[]
            {
                Weapons.Firearms.M1911
            },
            Enemies = new[]
            {
                new EnemyPreset(Outfits.Grunt, CharacterStats.Grunt)
            }
        },
        new LevelComponent(Resources.Load<Texture>("levels/lvl2.qoi"))
        {
            EnemySpawnRate = 0.5f,
            MaxEnemies = 2,
            TotalEnemies = 10,
            Weapons = new[]
            {
                Weapons.Firearms.M1911,
                Weapons.Firearms.Tec9
            },
            Enemies = new[]
            {
                new EnemyPreset(Outfits.Grunt, CharacterStats.Grunt),
                new EnemyPreset(Outfits.Grunt, CharacterStats.Grunt),
                new EnemyPreset(Outfits.Agent, CharacterStats.Agent),
            }
        },
        new LevelComponent(Resources.Load<Texture>("levels/lvl3.qoi"))
        {
            EnemySpawnRate = 1.5f,
            MaxEnemies = 3,
            TotalEnemies = 15,
            Weapons = new[]
            {
                Weapons.Firearms.M1911,
                Weapons.Firearms.Tec9,
                Weapons.Firearms.Ksg,
            },
            Enemies = new[]
            {
                new EnemyPreset(Outfits.Grunt, CharacterStats.Grunt),
                new EnemyPreset(Outfits.Agent, CharacterStats.Agent),
                new EnemyPreset(Outfits.Agent, CharacterStats.Agent),
                new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
            }
        },
        new LevelComponent(Resources.Load<Texture>("levels/lvl4.qoi"))
        {
            EnemySpawnRate = 2,
            MaxEnemies = 4,
            TotalEnemies = 20,
            WalkingRange = new Vector2(-600,340),
            Weapons = new[]
            {
                Weapons.Firearms.M1911,
                Weapons.Firearms.Tec9,
                Weapons.Firearms.Ksg,
                Weapons.Firearms.Deagle,
            },
            Enemies = new[]
            {
                new EnemyPreset(Outfits.Agent, CharacterStats.Agent),
                new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
                new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
            }
        },
        new LevelComponent(Resources.Load<Texture>("levels/lvl5.qoi"))
        {
            EnemySpawnRate = 2,
            MaxEnemies = 4,
            TotalEnemies = 15,
            Weapons = new[]
            {
                Weapons.Firearms.M1911,
                Weapons.Firearms.Tec9,
                Weapons.Firearms.Ksg,
                Weapons.Firearms.Deagle,
                Weapons.Firearms.Mp5k,
            },
            Enemies = new[]
            {
                new EnemyPreset(Outfits.Engineer, CharacterStats.Agent),
                new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
                new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
                new EnemyPreset(Outfits.Soldat, CharacterStats.Soldat),
            }
        },
        new LevelComponent(Resources.Load<Texture>("levels/lvl6.qoi"))
        {
            EnemySpawnRate = 3,
            MaxEnemies = 4,
            TotalEnemies = 20,
            Weapons = new[]
            {
                Weapons.Firearms.M1911,
                Weapons.Firearms.Tec9,
                Weapons.Firearms.Ksg,
                Weapons.Firearms.Deagle,
                Weapons.Firearms.Mp5k,
                Weapons.Firearms.FnFal,
            },
            Enemies = new[]
            {
                new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
                new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
                new EnemyPreset(Outfits.Soldat, CharacterStats.Soldat),
            }
        },
        new LevelComponent(Resources.Load<Texture>("levels/lvl7.qoi"))
        {
            EnemySpawnRate = 3,
            MaxEnemies = 5,
            TotalEnemies = 20,
            Weapons = new[]
            {
                Weapons.Firearms.M1911,
                Weapons.Firearms.Tec9,
                Weapons.Firearms.Ksg,
                Weapons.Firearms.Deagle,
                Weapons.Firearms.Mp5k,
                Weapons.Firearms.FnFal,
            },
            Enemies = new[]
            {
                new EnemyPreset(Outfits.Soldat, CharacterStats.Soldat),
                new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
            }
        },
        new LevelComponent(Resources.Load<Texture>("levels/lvl8.qoi"))
        {
            EnemySpawnRate = 3,
            MaxEnemies = 6,
            TotalEnemies = 20,
            Weapons = Weapons.AllWeapons.ToArray(),
            Enemies = new[]
            {
               new EnemyPreset(Outfits.Soldat, CharacterStats.Soldat),
            }
        },
        new (new LevelComponent(Resources.Load<Texture>("levels/lvl9.qoi"))
        {
            EnemySpawnRate = 0,
            MaxEnemies = 0,
            TotalEnemies = 1,
            Weapons = Weapons.AllWeapons.ToArray(),
        }, s =>
            {
                Prefabs.CreateBoss(s);
                RoutineScheduler.Start(giveWeapon());
                IEnumerator<IRoutineCommand> giveWeapon()
                {
                    yield return new RoutineDelay(7);
                    Weapons.Give("deagle"); // TODO this is fucked up but who caresa lmao
                }
            }),
    };
}
