using System.Numerics;
using Walgelijk;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public class BossSystem : Walgelijk.System
{
    private readonly EnemyPreset[] presets =
    {
        new EnemyPreset(Outfits.Grunt, CharacterStats.Grunt),
        new EnemyPreset(Outfits.Agent, CharacterStats.Agent),
        new EnemyPreset(Outfits.Engineer, CharacterStats.Engineer),
        new EnemyPreset(Outfits.Soldat, CharacterStats.Soldat),
    };

    public override void FixedUpdate()
    {
        if (!Scene.FindAnyComponent<PlayerControllerComponent>(out var player) || !Scene.TryGetComponentFrom<CharacterComponent>(player.Entity, out var playerChar))
            return;

        const float introDuration = 8;
        var initialHoverTarget = new Vector2(400, 250);

        if (Scene.FindAnyComponent<BossControllerComponent>(out var boss))
        {
            var character = Scene.GetComponentFrom<CharacterComponent>(boss.Entity);

            character.WalkState.Flying = character.IsAlive;

            if (character.IsAlive)
            {
                character.Facing = playerChar.BottomCenter.X < character.BottomCenter.X ? Facing.Left : Facing.Right;
                if (Time.SecondsSinceSceneChange < introDuration)
                {
                    var p = Utilities.Clamp(Time.SecondsSinceSceneChange / introDuration);
                    character.BottomCenter = Utilities.Lerp(new Vector2(800, 500), initialHoverTarget, Easings.Cubic.Out(p));
                    boss.Stage = BossStage.Idle;
                    character.Health = character.Stats.MaxHealth;
                    boss.HealthAtStartStage = character.Health;
                    boss.DidLightningAttack = false;
                }
                else
                {
                    var p = Easings.Quad.In(Utilities.Clamp((Time.SecondsSinceSceneChange - introDuration) / 2));
                    var t = Time.SecondsSinceSceneChange * 0.04f;
                    var pos = Utilities.Lerp(initialHoverTarget, new Vector2(
                            Utilities.MapRange(-1, 1, -600, 600, Noise.GetSimplex(39.32f, t, 0)),
                            Utilities.MapRange(-1, 1, LevelComponent.FloorLevel + 100, 500, Noise.GetSimplex(39.32f, 0, t))
                        ), p);
                    character.BottomCenter = pos;
                    if (boss.Stage is BossStage.Idle)
                        boss.Stage = BossStage.Vulnerable;
                }
            }
            else
            {
                character.BottomCenter -= Vector2.UnitY * Time.FixedInterval * 1500;
                character.BottomCenter.Y = MathF.Max(character.BottomCenter.Y, LevelComponent.FloorLevel);
                boss.Stage = BossStage.Idle;
            }

            character.AimDirection = Vector2.Normalize(character.NeckPoint - playerChar.NeckPoint);

            switch (boss.Stage)
            {
                case BossStage.Vulnerable:
                    {
                        boss.VulnerableTimer += Time.FixedInterval;
                        var damage = (boss.HealthAtStartStage - character.Health);
                        if (damage > 500 || boss.VulnerableTimer > 10)
                            setStage(BossStage.HumanWave);
                    }
                    break;
                case BossStage.HumanWave:
                    {
                        if (boss.KillsThisStage > 10)
                        {
                            setStage(BossStage.ZedWave);

                            Audio.PlayOnce(SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("zombify.wav")));

                            // zombify
                            foreach (var ai in Scene.GetAllComponentsOfType<AiControllerComponent>())
                            {
                                var aiChar = Scene.GetComponentFrom<CharacterComponent>(ai.Entity);
                                aiChar.Revive();
                                aiChar.SkinColour = new Color(0, 190, 0);
                                aiChar.Stats.RegenSpeed = 100;
                                aiChar.Stats.MaxHealth = 300;
                                aiChar.Stats.MeleeDamage = 60;
                                aiChar.Refill();

                                Scene.AttachComponent(ai.Entity, new DespawnWhenDeadComponent());

                                ai.MovementSpeedMultiplier = Utilities.RandomFloat(0.5f, 0.8f);
                            }
                            break;
                        }

                        boss.SpawnTimer += Time.FixedInterval;

                        if (boss.SpawnTimer > 1 && Scene.GetAllComponentsOfType<AiControllerComponent>().Count() < 5)
                        {
                            boss.SpawnTimer = 0;
                            var e = EnemySpawnerSystem.SpawnEnemy(Scene, Faction.AAHW, null, Utilities.PickRandom(presets), Utilities.PickRandom(-600, 600));
                            Scene.DetachComponent<DespawnWhenDeadComponent>(e.Entity); // we gotta turn them into zombies next
                            e.OnDeath.AddListener(_ => boss.KillsThisStage++);
                        }
                    }
                    break;
                case BossStage.ZedWave:
                    {
                        if (!Scene.FindAnyComponent<AiControllerComponent>(out _))
                            setStage(BossStage.Vulnerable);
                    }
                    break;
            }

            void setStage(BossStage stage)
            {
                boss.HealthAtStartStage = character.Health;
                boss.KillsThisStage = 0;
                boss.VulnerableTimer = 0;
                boss.Stage = stage;
                boss.DidLightningAttack = false;
                boss.LightningCursor = character.NeckPoint;

                Prefabs.CreateWeapon(Scene, Weapons.Firearms.Mp5k, new Vector2(0, 400));
            }
        }
    }

    public override void Update()
    {
        if (!Scene.FindAnyComponent<PlayerControllerComponent>(out var player) || !Scene.TryGetComponentFrom<CharacterComponent>(player.Entity, out var playerChar))
            return;

        if (!Scene.FindAnyComponent<BossControllerComponent>(out var boss) || Scene.FindAnyComponent<DeathSequenceComponent>(out _))
            return;

        var character = Scene.GetComponentFrom<CharacterComponent>(boss.Entity);
        if (character.IsAlive)
        {
            var p = Utilities.Clamp(character.Health / character.Stats.MaxHealth);
            var t = Easings.Quad.Out(Utilities.Clamp((Time.SecondsSinceSceneChange - 4) / 4));

            var offset = Utilities.Lerp(new Vector2(0, -100), new Vector2(0, 0), t);

            Draw.Reset();
            Draw.ScreenSpace = true;
            Draw.Order = RenderOrder.UI;

            Draw.Colour = Colors.Black.WithAlpha(0.8f);
            Draw.Line(new Vector2(200, 50) + offset, new Vector2(Window.Width - 50, 50) + offset, 25);
            Draw.Colour = Colors.Red;
            Draw.Line(new Vector2(200, 50) + offset, new Vector2(Utilities.Lerp(200, Window.Width - 50, p), 50) + offset, 13);

            Draw.Colour = Colors.White;
            Draw.Font = Resources.Load<Font>("impact.wf");
            Draw.FontSize = 32;
            Draw.Text("FALSE GOD", new Vector2(Window.Width / 2 + 75, 50) + offset, Vector2.One, HorizontalTextAlign.Center, VerticalTextAlign.Middle);
        }

        if (boss.Stage is BossStage.Vulnerable)
        {
            if (!boss.DidLightningAttack)
            {
                if (boss.VulnerableTimer <= 3.5f)
                    boss.LightningCursor = Utilities.SmoothApproach(boss.LightningCursor, playerChar.NeckPoint, 1, Time.FixedInterval);
                {
                    var zapImg = Resources.Load<Texture>("zap_cursor.png");
                    Draw.Reset();
                    Draw.Order = RenderOrder.UI;
                    Draw.Colour = Colors.Red;
                    if (boss.VulnerableTimer > 3.5f && boss.VulnerableTimer % 0.2f > 0.1)
                        Draw.Colour = Colors.White;
                    Draw.Texture = zapImg;
                    Draw.Quad(new Rect(boss.LightningCursor, zapImg.Size));

                    if (boss.VulnerableTimer > 4)
                    {
                        boss.DidLightningAttack = true;
                        Audio.PlayOnce(SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("zeus.wav")));
                        var from = character.NeckPoint;
                        var to = boss.LightningCursor;
                        RoutineScheduler.Start(DrawLightning(from, to));

                        if (Vector2.Distance(boss.LightningCursor, playerChar.NeckPoint) < 50)
                        {
                            playerChar.Damage(Scene,1000000);
                        }

                        IEnumerator<IRoutineCommand> DrawLightning(Vector2 from, Vector2 to)
                        {
                            var t = 0f;
                            while (t < 0.05f)
                            {
                                Draw.Reset();
                                Draw.Order = new RenderOrder(200, 0);
                                Draw.Colour = Colors.White;
                                const int r = 8;
                                for (int i = 0; i < r; i++)
                                {
                                    var a = Utilities.Lerp(from, to, i / (float)r);
                                    var b = Utilities.Lerp(from, to, (i + 1) / (float)r);
                                    a.X += Utilities.MapRange(0, 1, -1, 1, Noise.GetValue(a.X, a.Y, 0)) * 80;
                                    a.Y += Utilities.MapRange(0, 1, -1, 1, Noise.GetValue(0, a.X, a.Y)) * 80;
                                    b.X += Utilities.MapRange(0, 1, -1, 1, Noise.GetValue(b.X, b.Y, 0)) * 80;
                                    b.Y += Utilities.MapRange(0, 1, -1, 1, Noise.GetValue(0, b.X, b.Y)) * 80;
                                    Draw.Line(a, b, 5);
                                }
                                yield return new RoutineFrameDelay();
                                t += Time.DeltaTime;
                            }
                        }
                    }
                }
            }
        }
    }
}
