using System.Numerics;
using Walgelijk;
using Walgelijk.Onion;
using Walgelijk.ParticleSystem;
using Walgelijk.Physics;

namespace MadnessMicroactive;

public static class LevelScene
{
    public static Scene Create(Game game, Levels.Level level)
    {
        var scene = new Scene(game);
        scene.ShouldBeDisposedOnSceneChange = false; //TODO should be true but there is an engine bug... somewhere

        scene.AddSystem(new BatchRendererSystem());
        scene.AddSystem(new AiControllerSystem());
        scene.AddSystem(new PlayerControllerSystem());
        scene.AddSystem(new WeaponSystem());
        scene.AddSystem(new BulletSystem());
        scene.AddSystem(new CharacterSystem());
        scene.AddSystem(new CharacterApparelSystem());
        scene.AddSystem(new PlayerUiSystem());
        scene.AddSystem(new EquippableSystem()); // after character system because equippables position themselves
        scene.AddSystem(new PhysicsSystem());
        //scene.AddSystem(new PhysicsDebugSystem());
        scene.AddSystem(new ParticleSystem());
        scene.AddSystem(new TransformSystem());
        scene.AddSystem(new MuzzleflashSystem());
        scene.AddSystem(new LevelBackgroundSystem());
        scene.AddSystem(new LevelProgressSystem());
        scene.AddSystem(new EnemySpawnerSystem());
        scene.AddSystem(new RoutineSystem());
        scene.AddSystem(new DeathSequenceSystem());
        scene.AddSystem(new OnionSystem());
        scene.AddSystem(new MusicSystem());
        scene.AddSystem(new BossSystem());
        scene.AddSystem(new CameraSystem() { ExecutionOrder = -1 });

        scene.AttachComponent(scene.CreateEntity(), new PhysicsWorldComponent { ChunkSize = 128 });
        scene.AttachComponent(scene.CreateEntity(), new LevelProgressComponent());
        scene.AttachComponent(scene.CreateEntity(), new BatchRendererStorageComponent());

        scene.AttachComponent(scene.CreateEntity(), level.LevelComponent);

        var camera = scene.CreateEntity();
        scene.AttachComponent(camera, new TransformComponent()
        {
            Position = new Vector2(0, game.Window.Height / 2)
        });
        scene.AttachComponent(camera, new CameraComponent()
        {
            ClearColour = new Color(76, 76, 76),
            OrthographicSize = 1,
            PixelsPerUnit = 1
        });

        Prefabs.CreatePlayer(scene);

        var casingMat = ParticleMaterialInitialiser.CreateDefaultMaterial();
        casingMat.SetUniform("mainTex", Resources.Load<Texture>("casing.png"));
        MandessUtils.CreateCasingEjectionParticleSystem(scene, casingMat);

        level.Init?.Invoke(scene);

        return scene;
    }

    [Command]
    public static CommandResult Revive(bool force = false)
    {
        var s = Game.Main.Scene;

        if (s.FindAnyComponent<PlayerControllerComponent>(out var player))
        {
            var character = s.GetComponentFrom<CharacterComponent>(player.Entity);
            character.Revive();
        }
        else if (s.HasSystem<PlayerControllerSystem>())
            Prefabs.CreatePlayer(s);
        else if (!force)
            return CommandResult.Error("No player controller system found. You probably shouldn't revive here. Pass true to force a revive anyway.");

        return "I will keep bringing them back";
    }
}