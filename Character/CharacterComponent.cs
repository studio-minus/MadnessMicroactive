using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class CharacterComponent : Component
{
    public ComponentRef<TransformComponent> Head;
    public ComponentRef<TransformComponent> Body;

    public ComponentRef<TransformComponent> Foot1;
    public ComponentRef<TransformComponent> Foot2;

    public ComponentRef<TransformComponent> Hand1;
    public ComponentRef<TransformComponent> Hand2;

    public Outfit Outfit { get; private set; } = Outfits.Grunt;
    public ComponentRef<BatchedSpriteComponent>[]? OutfitSprites;

    public int RenderLayer;
    public Color SkinColour = Colors.White;
    public bool IncrementsKillsOnDeath;

    public Faction Faction = Faction.AAHW;
    public CharacterStats Stats = new()
    {
        MaxHealth = 50,
        MeleeDamage = 15,
        RegenSpeed = 20
    };
    public float RegenCooldownTimer = 0;

    public Vector2 BottomCenter;
    public Vector2 AimDirection;
    public Facing Facing;
    public bool Flipped => Facing == Facing.Left;
    public float Recoil;
    public float DamageJump = 0;
    public float Health = 100;

    public bool IsAlive => Health > 0;
    public float DeadTime = 0;
    public bool HasDied = false;

    public readonly Hook<Scene> OnDeath = new();

    public ComponentRef<EquippableComponent> Equipped;

    public Vector2 WalkAcceleration;
    public CharacterWalkState WalkState = new()
    {
        HopDuration = 0.3f,
        HopTimer = float.MaxValue
    };

    public bool MeleeFlipFlop;
    public CharacterPose Pose = CharacterPoses.Idle;
    public readonly Dictionary<Limb, VelocityPos> LimbPositions = new();

    public Vector2 NeckPoint => new(BottomCenter.X, BottomCenter.Y + 100);

    public void Equip(Scene scene, EquippableComponent component)
    {
        if (Equipped.IsValid(scene))
            Drop(scene);
        component.Wielder = this;
        Equipped = component;
    }

    public void Drop(Scene scene)
    {
        if (Equipped.TryGet(scene, out var equipped))
            equipped.Wielder = default;
        Equipped = default;
    }

    public void Damage(Scene scene, float damage)
    {
        RegenCooldownTimer = 0;
        Health -= damage;
        if (Health <= 0)
            Kill(scene);
    }

    public void Kill(Scene scene)
    {
        if (HasDied)
            return;

        HasDied = true;

        OnDeath.Dispatch(scene);

        Drop(scene);
        WalkAcceleration = default;
        Pose = CharacterPoses.Dead;
        Health = 0;

        if (IncrementsKillsOnDeath &&  scene.FindAnyComponent<LevelProgressComponent>(out var progress))
        {
            PersistentData.Instance.Kills++;
            progress.Kills++;
        }
    }

    public void Revive()
    {
        HasDied = false;
        Health = Stats.MaxHealth;
        RegenCooldownTimer = 0;
    }

    public void RemoveFromScene(Scene scene)
    {
        if (OutfitSprites != null)
            foreach (var s in OutfitSprites)
                scene.RemoveEntity(s.Entity);

        scene.RemoveEntity(Head.Entity);
        scene.RemoveEntity(Body.Entity);

        scene.RemoveEntity(Hand1.Entity);
        scene.RemoveEntity(Hand2.Entity);

        scene.RemoveEntity(Foot1.Entity);
        scene.RemoveEntity(Foot2.Entity);

        scene.RemoveEntity(Entity);
    }

    public void SetOutfit(Scene scene, Outfit outfit)
    {
        Outfit = outfit;

        if (OutfitSprites != null)
            foreach (var s in OutfitSprites)
                scene.RemoveEntity(s.Entity);

        OutfitSprites = new ComponentRef<BatchedSpriteComponent>[outfit.Head.Count + outfit.Body.Count];

        int i = 0;

        foreach (var headApparel in outfit.Head)
        {
            var e = scene.CreateEntity();
            var transform = scene.AttachComponent(e, new TransformComponent
            {
                Scale = headApparel.Texture.Size,
                InterpolationFlags = InterpolationFlags.Rotation | InterpolationFlags.Position
            });
            var sprite = scene.AttachComponent(e, new BatchedSpriteComponent(headApparel.Texture) { SyncWithTransform = true });
            OutfitSprites[i++] = sprite;
            transform.Position = transform.Position = Head.Get(scene).Position;
        }

        foreach (var bodyApparel in outfit.Body)
        {
            var e = scene.CreateEntity();
            var transform = scene.AttachComponent(e, new TransformComponent
            {
                Scale = bodyApparel.Texture.Size,
                InterpolationFlags = InterpolationFlags.Rotation | InterpolationFlags.Position
            });
            var sprite = scene.AttachComponent(e, new BatchedSpriteComponent(bodyApparel.Texture) { SyncWithTransform = true });
            OutfitSprites[i++] = sprite;
            transform.Position = transform.Position = Body.Get(scene).Position;
        }
    }

    public void Refill()
    {
        Health = Stats.MaxHealth;
        RegenCooldownTimer = float.MaxValue;
    }
}
