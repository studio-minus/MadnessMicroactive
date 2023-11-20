using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class EnemySpawnerSystem : Walgelijk.System
{
    private readonly FixedIntervalDistributor distributor = new(1);

    public override void FixedUpdate()
    {
        var enemyFaction = Faction.AAHW;
        var currentCount = Scene.GetAllComponentsOfType<CharacterComponent>().Count(c => c.IsAlive && c.Faction == enemyFaction);
        var enemiesRemaining = 1;
        var maxSimultaneousEnemies = 1;
        var spawnXs = new Vector2(-600, 600);

        if (Scene.FindAnyComponent<LevelComponent>(out var level))
        {
            enemiesRemaining = level.TotalEnemies;
            maxSimultaneousEnemies = level.MaxEnemies;
            spawnXs = level.WalkingRange;

            if (Scene.FindAnyComponent<LevelProgressComponent>(out var progress))
                enemiesRemaining = level.TotalEnemies - progress.Kills;
        }

        for (int i = 0; i < distributor.CalculateCycleCount(Time.FixedInterval); i++)
            if (currentCount < maxSimultaneousEnemies && enemiesRemaining > 0)
            {
                var preset = new EnemyPreset(Outfits.Grunt, CharacterStats.Grunt);
                var x = Utilities.PickRandom(spawnXs.X, spawnXs.Y);

                if (Scene.FindAnyComponent(out level) && level.Enemies != null && level.Enemies.Length > 0)
                    preset = Utilities.PickRandom(level.Enemies);

                SpawnEnemy(Scene, enemyFaction, level, preset, x);
                currentCount++;
                enemiesRemaining--;
            }
    }

    public static CharacterComponent SpawnEnemy(Scene scene, Faction enemyFaction, LevelComponent? level, EnemyPreset preset, float x)
    {
        var e = Prefabs.CreateCharacter(
            scene,
            Utilities.RandomInt(0, 100),
            new Vector2(x, LevelComponent.FloorLevel));

        e.Faction = enemyFaction;
        e.Stats = preset.Stats;
        e.IncrementsKillsOnDeath = true;
        e.SetOutfit(scene, preset.Outfit);

        if (Utilities.RandomFloat() > 0.5f)
        {
            IWeapon weaponToSpawnWith = Utilities.PickRandom(Weapons.AllWeapons);
            if (level != null)
                weaponToSpawnWith = Utilities.PickRandom(level.Weapons);

            if (weaponToSpawnWith is Firearm f)
            {
                var wpn = Prefabs.CreateWeapon(scene, f, e.BottomCenter);
                e.Equip(scene, scene.GetComponentFrom<EquippableComponent>(wpn.Entity));
            }//else TODO
        }

        scene.AttachComponent(e.Entity, new AiControllerComponent());
        scene.AttachComponent(e.Entity, new DespawnWhenDeadComponent());
        return e;
    }
}
