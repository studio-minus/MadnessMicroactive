using System;
using System.Numerics;
using Walgelijk;
using Walgelijk.Onion;
using Walgelijk.ParticleSystem;
using Walgelijk.Physics;
using static MadnessMicroactive.Levels;
using static MadnessMicroactive.MenuScene;

namespace MadnessMicroactive;

public static class ExperimentScene
{
    public static Scene Create(Game game)
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
        scene.AddSystem(new ParticleSystem());
        scene.AddSystem(new ExperimentModeSystem());
        scene.AddSystem(new TransformSystem());
        scene.AddSystem(new MuzzleflashSystem());
        scene.AddSystem(new LevelBackgroundSystem());
        scene.AddSystem(new RoutineSystem());
        scene.AddSystem(new DeathSequenceSystem());
        scene.AddSystem(new OnionSystem());
        scene.AddSystem(new MusicSystem());
        scene.AddSystem(new CameraSystem() { ExecutionOrder = -1 });

        scene.AttachComponent(scene.CreateEntity(), new PhysicsWorldComponent { ChunkSize = 128 });
        scene.AttachComponent(scene.CreateEntity(), new BatchRendererStorageComponent());

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

        return scene;
    }

    public class ExperimentModeSystem : Walgelijk.System
    {
        private bool popupOpen = true;

        private readonly EnemyPreset[] presets =
        {
            new(Outfits.Grunt, CharacterStats.Grunt),
            new(Outfits.Agent, CharacterStats.Agent),
            new(Outfits.Engineer, CharacterStats.Engineer),
            new(Outfits.Soldat, CharacterStats.Soldat),
        };

        public override void Update()
        {
            if (popupOpen)
            {
                Ui.Layout.Size(320, 350).Center();
                Ui.Theme.OutlineColour(Colors.Black).OutlineWidth(8).Rounding(19).Once();
                Ui.Decorate(new HeaderDecorator("Experiment"));
                Ui.StartGroup();
                {
                    Ui.Layout.FitContainer().Scale(-10, -80).Center().Move(0, -20);
                    Ui.TextRect(
@"Right now, you're playing in experiment mode. This mode lets you mess around with the game and practice at your own pace.

Press [SPACE] to create enemies. 
Press [TAB] to create allies. 
Press the number keys to create weapons.",
                        HorizontalTextAlign.Left, VerticalTextAlign.Top);

                    Ui.Layout.Size(190, 48).StickBottom().CenterHorizontal().Move(0, -5);
                    if (Ui.Button("Understood"))
                    {
                        popupOpen = false;
                        return;
                    }
                }
                Ui.End();
            }
            else
            {
                if (Input.IsKeyPressed(Key.Space))
                    EnemySpawnerSystem.SpawnEnemy(Scene, Faction.AAHW, null, Utilities.PickRandom(presets), Utilities.PickRandom(-600, 600));

                if (Input.IsKeyPressed(Key.Tab))
                    EnemySpawnerSystem.SpawnEnemy(Scene, Faction.Player, null, Utilities.PickRandom(presets) with { Outfit = Outfits.Grunt }, Utilities.PickRandom(-600, 600));

                if (Input.KeysDown != null && Input.KeysDown.Any(d => d >= Key.D0 && d <= Key.D9))
                {
                    var key = Input.KeysDown.FirstOrDefault(d => d >= Key.D0 && d <= Key.D9);
                    Prefabs.CreateWeapon(Scene, Weapons.AllWeapons[(key - Key.D0) % Weapons.AllWeapons.Count], new Vector2(Utilities.RandomFloat(-600, 600), 200));
                }
            }
        }
    }
}
