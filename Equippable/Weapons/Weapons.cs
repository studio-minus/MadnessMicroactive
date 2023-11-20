using System.Numerics;
using System.Xml.Linq;
using Walgelijk;

namespace MadnessMicroactive;

public static class Weapons
{
    public static class Firearms
    {
        public static readonly Firearm M1911 = new(nameof(M1911))
        {
            Texture = Resources.Load<Texture>("weapons/m1911.png"),
            HoldPoints = new[] { new Vector2(-18, -2) },
            Sounds = new[] { SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("wep/m1911.wav")) },
            BarrelOut = new Vector2(28, 12),
            Damage = 60,
            Recoil = 12,
            AutoInterval = -1,
            MaxRounds = 8
        };

        public static readonly Firearm Tec9 = new(nameof(Tec9))
        {
            Texture = Resources.Load<Texture>("weapons/tec9.png"),
            HoldPoints = new[] { new Vector2(-27, 0), new Vector2(5, -10) },
            Sounds = new[] { SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("wep/tec9.wav")) },
            BarrelOut = new Vector2(40, 20),
            Damage = 40,
            Recoil = 8,
            AutoInterval = 0.1f,
            MaxRounds = 15,
            Dispersion = 5,
        };

        public static readonly Firearm Deagle = new(nameof(Deagle))
        {
            Texture = Resources.Load<Texture>("weapons/deagle.png"),
            HoldPoints = new[] { new Vector2(-24, -5) },
            Sounds = new[] { SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("wep/deagle.wav")) },
            BarrelOut = new Vector2(40, 16),
            Damage = 99,
            Recoil = 80,
            AutoInterval = -1,
            MaxRounds = 8,
            Dispersion = 5,
        };

        public static readonly Firearm FnFal = new(nameof(FnFal))
        {
            Texture = Resources.Load<Texture>("weapons/fal.png"),
            HoldPoints = new[] { new Vector2(-46, -6), new Vector2(41, 10) },
            Sounds = new[] { SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("wep/fal.wav")) },
            BarrelOut = new Vector2(125, 15),
            Damage = 70,
            Recoil = 10,
            AutoInterval = 0.09f,
            MaxRounds = 30,
            Dispersion = 5,
        };

        public static readonly Firearm Mp5k = new(nameof(Mp5k))
        {
            Texture = Resources.Load<Texture>("weapons/mp5k.png"),
            HoldPoints = new[] { new Vector2(-25, 0), new Vector2(30, 6) },
            Sounds = new[] { SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("wep/mp5k.wav")) },
            BarrelOut = new Vector2(46, 29),
            Damage = 40,
            Recoil = 2,
            AutoInterval = 0.08f,
            MaxRounds = 30,
            Dispersion = 3,
        };

        public static readonly Firearm Ksg = new(nameof(Ksg))
        {
            Texture = Resources.Load<Texture>("weapons/ksg.png"),
            HoldPoints = new[] { new Vector2(-11, 0), new Vector2(60, 6) },
            Sounds = new[] { SoundCache.Instance.LoadSoundEffect(Resources.Load<FixedAudioData>("wep/shotgun.wav")) },
            BarrelOut = new Vector2(87, 19),
            Damage = 20,
            Recoil = 16,
            AutoInterval = -1,
            MaxRounds = 8,
            Dispersion = 9,
            BulletsPerShot = 8
        };
    }

    public static readonly List<Firearm> AllFirearms = new();
    public static readonly List<IWeapon> AllWeapons = new();

    static Weapons()
    {
        var firearms = typeof(Firearms).GetFields();

        foreach (var field in firearms)
        {
            if (field.GetValue(null) is Firearm f)
                AllFirearms.Add(f);
        }

        AllWeapons.AddRange(AllFirearms);
    }

    [Command]
    public static CommandResult Give(string name)
    {
        var s = Game.Main.Scene;

        IWeapon? weapon = null;

        foreach (var f in AllFirearms)
            if (f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                weapon = f;
                break;
            }

        if (weapon == null)
            return CommandResult.Error($"Couldn't find \"{name}\"");

        if (s.FindAnyComponent<PlayerControllerComponent>(out var player) && s.TryGetComponentFrom<CharacterComponent>(player.Entity, out var character))
        {
            var wpn = Prefabs.CreateWeapon(s, weapon, character.BottomCenter);
            character.Equip(s, s.GetComponentFrom<EquippableComponent>(wpn.Entity));
            return CommandResult.Info($"Materialised a {weapon.Name}");
        }

        return CommandResult.Error("Player not found");
    }

    [Command]
    public static CommandResult Spawn(string name)
    {
        var s = Game.Main.Scene;

        IWeapon? weapon = null;

        foreach (var f in AllFirearms)
            if (f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                weapon = f;
                break;
            }

        if (weapon == null)
            return CommandResult.Error($"Couldn't find \"{name}\"");

        Prefabs.CreateWeapon(s, weapon, Game.Main.State.Input.WorldMousePosition);

        return CommandResult.Info($"{weapon.Name} spawned at mouse position");
    }
}
