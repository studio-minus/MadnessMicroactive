using System.Numerics;
using Walgelijk;
using Walgelijk.Physics;

namespace MadnessMicroactive;

public static class Prefabs
{
    public static Entity CreateMuzzleflash(Scene scene, Vector2 point, Vector2 dir, float size)
    {
        var e = scene.CreateEntity();

        scene.AttachComponent(e, new MuzzleflashComponent
        {
            Position = point,
            Direction = dir,
            Size = size + Utilities.RandomFloat(-0.1f, 0.1f),
            MaxTime = Utilities.RandomFloat(0.05f, 0.15f)
        });

        return e;
    }

    public static void CreatePlayer(Scene scene)
    {
        var player = CreateCharacter(scene, 102, new Vector2(0, LevelComponent.FloorLevel));
        player.Faction = Faction.Player;
        player.Stats = CharacterStats.Player;
        player.Refill();
        player.OnDeath.AddListener(static scene =>
        {
            RoutineScheduler.Start(routine());

            IEnumerator<IRoutineCommand> routine()
            {
                yield return new RoutineFrameDelay();
                scene.Game.AudioRenderer.Play(SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("death.wav")), 10);
                if (scene.FindAnyComponent<CameraComponent>(out var camera) && scene.FindAnyComponent<PlayerControllerComponent>(out var player))
                {
                    scene.AttachComponent(camera.Entity, new DeathSequenceComponent(new ComponentRef<CharacterComponent>(player.Entity)));
                }
            }
        });
        player.SetOutfit(scene, Outfits.Player);
        scene.AttachComponent(player.Entity, new PlayerControllerComponent());

        player.Health = PersistentData.Instance.Health;
        if (PersistentData.Instance.Weapon != null)
        {
            var e = CreateWeapon(scene, PersistentData.Instance.Weapon, player.BottomCenter);
            player.Equip(scene, scene.GetComponentFrom<EquippableComponent>(e.Entity));
            e.RemainingRounds = PersistentData.Instance.Ammo;
        }
    }

    public static void CreateBoss(Scene scene)
    {
        var boss = CreateCharacter(scene, 101, new Vector2(1000));
        boss.Faction = Faction.AAHW;
        boss.Stats = CharacterStats.Boss;
        boss.Refill();
        boss.OnDeath.AddListener(static scene =>
        {
            RoutineScheduler.Start(routine());

            IEnumerator<IRoutineCommand> routine()
            {
                yield return new RoutineDelay(5);
                Game.Main.Scene = FinalWinScene.Create(Game.Main);
            }
        });
        boss.SetOutfit(scene, Outfits.Boss);
        scene.AttachComponent(boss.Entity, new BossControllerComponent());
    }

    public static WeaponComponent CreateWeapon(Scene scene, IWeapon wpn, Vector2 position)
    {
        var e = scene.CreateEntity();
        switch (wpn)
        {
            case Firearm firearm:
                {
                    scene.AttachComponent(e, new TransformComponent
                    {
                        Position = position,
                        Scale = firearm.Texture.Size,
                        InterpolationFlags = InterpolationFlags.Position | InterpolationFlags.Rotation
                    });
                    scene.AttachComponent(e, new BatchedSpriteComponent(firearm.Texture)
                    {
                        SyncWithTransform = true
                    });
                    scene.AttachComponent(e, new EquippableComponent(firearm.HoldPoints));
                    return scene.AttachComponent(e, new WeaponComponent(wpn)
                    {
                        RemainingRounds = firearm.MaxRounds
                    });
                }
            default:
                throw new NotImplementedException($"{wpn.GetType()} is unsupported");
        }
    }

    public static CharacterComponent CreateCharacter(Scene scene, int layer, Vector2 bottomCenter)
    {
        var main = scene.CreateEntity();
        var head = scene.CreateEntity();
        var body = scene.CreateEntity();
        var foot1 = scene.CreateEntity();
        var foot2 = scene.CreateEntity();
        var hand1 = scene.CreateEntity();
        var hand2 = scene.CreateEntity();

        var character = scene.AttachComponent(main, new CharacterComponent
        {
            BottomCenter = bottomCenter,
            RenderLayer = layer
        });

        character.Head = createLimb(head, CharacterPoses.Idle.Head, Resources.Load<Texture>("character/head.png"), Limb.Head);
        character.Body = createLimb(body, CharacterPoses.Idle.Body, Resources.Load<Texture>("character/body.png"), Limb.Body);

        character.Foot1 = createLimb(foot1, CharacterPoses.Idle.Foot1, Resources.Load<Texture>("character/foot.png"), Limb.Foot1);
        character.Foot2 = createLimb(foot2, CharacterPoses.Idle.Foot2, Resources.Load<Texture>("character/foot.png"), Limb.Foot2);

        character.Hand1 = createLimb(hand1, CharacterPoses.Idle.Hand1, Resources.Load<Texture>("character/hand.png"), Limb.Hand1);
        character.Hand2 = createLimb(hand2, CharacterPoses.Idle.Hand2, Resources.Load<Texture>("character/hand.png"), Limb.Hand2);

        scene.AttachComponent(head, new PhysicsBodyComponent
        {
            BodyType = BodyType.Dynamic,
            Collider = new RectangleCollider(character.Head.Get(scene), new Vector2(0.8f))
        });

        scene.AttachComponent(body, new PhysicsBodyComponent
        {
            BodyType = BodyType.Dynamic,
            Collider = new RectangleCollider(character.Body.Get(scene), new Vector2(0.8f))
        });

        TransformComponent createLimb(Entity e, LimbPosition p, IReadableTexture tex, Limb limb)
        {
            scene.AttachComponent(e, new BatchedSpriteComponent(tex)
            {
                SyncWithTransform = true,
                RenderOrder = new RenderOrder(layer, p.Order),
            });

            scene.AttachComponent(e, new LimbComponent(character));

            character.LimbPositions[limb] = new VelocityPos(bottomCenter + p.Translation, default);

            var t = scene.AttachComponent(e, new TransformComponent
            {
                Position = bottomCenter + p.Translation,
                Rotation = p.Rotation,
                Scale = tex.Size,
                InterpolationFlags = InterpolationFlags.Position | InterpolationFlags.Rotation
            });

            // becasue the interpolation is broken lol
            t.Position = t.Position;
            t.Rotation = t.Rotation;

            return t;
        }

        return character;
    }
}
