using System.Numerics;

namespace MadnessMicroactive;

public static class CharacterPoses
{
    public static readonly CharacterPose Idle = new()
    {
        Head = new(new Vector2(3, 132), rotation: 0, order: 10),
        Body = new(new Vector2(-6, 59), rotation: 9, order: 0),

        Hand1 = new(new Vector2(-20, 33), rotation: -90, order: 20),
        Hand2 = new(new Vector2(32, 35), rotation: -90, order: -10),

        Foot1 = new(new Vector2(-10, -3), rotation: 0, order: -20),
        Foot2 = new(new Vector2(15, 0), rotation: 0, order: -21),
    };

    public static readonly CharacterPose CarrySingleHanded = new()
    {
        Head = new(new Vector2(3, 132), rotation: 0, order: 10),
        Body = new(new Vector2(-6, 59), rotation: 9, order: 0),

        Hand1 = new(new Vector2(-30, 98), rotation: 90, order: 20),
        Hand2 = new(new Vector2(32, 35), rotation: -90, order: -10),

        Foot1 = new(new Vector2(-10, -3), rotation: 0, order: -20),
        Foot2 = new(new Vector2(15, 0), rotation: 0, order: -21),
    };   
    
    public static readonly CharacterPose CarryTwoHanded = new()
    {
        Head = new(new Vector2(3, 132), rotation: 0, order: 10),
        Body = new(new Vector2(-6, 59), rotation: 9, order: 0),

        Hand1 = new(new Vector2(0, 59), rotation: -35, order: 20),
        Hand2 = new(new Vector2(32, 35), rotation: -90, order: -10),

        Foot1 = new(new Vector2(-10, -3), rotation: 0, order: -20),
        Foot2 = new(new Vector2(15, 0), rotation: 0, order: -21),
    };

    public static readonly CharacterPose MeleeStance = new()
    {
        Head = Idle.Head with { Translation = new Vector2(6, 130) },
        Body = Idle.Body,

        Hand1 = new(new Vector2(14, 104), rotation: 0, order: 20),
        Hand2 = new(new Vector2(44, 118), rotation: 0, order: -10),

        Foot1 = Idle.Foot1,
        Foot2 = Idle.Foot2,
    };

    public static readonly CharacterPose Aiming = new()
    {
        AimAdjust = (true, false),

        Head = Idle.Head,
        Body = Idle.Body,

        Hand1 = new(new Vector2(98, 100), rotation: 0, order: 20),
        Hand2 = Idle.Hand2,

        Foot1 = Idle.Foot1,
        Foot2 = Idle.Foot2,
    };

    public static readonly CharacterPose FocusedAiming = new()
    {
        AimAdjust = (true, true),

        Head = Idle.Head with { Translation = new Vector2(9, 128) },
        Body = Idle.Body,

        Hand1 = new(new Vector2(98, 100), rotation: 0, order: 20),
        Hand2 = new(new Vector2(95, 95), rotation: 0, order: -10),

        Foot1 = Idle.Foot1,
        Foot2 = Idle.Foot2,
    };

    public static readonly CharacterPose Dead = new()
    {
        Head = new(new Vector2(132, 30), rotation: -85, order: 10),
        Body = new(new Vector2(59, 25), rotation: -95, order: 0),

        Hand1 = new(new Vector2(33, 8), rotation: -180, order: 20),
        Hand2 = new(new Vector2(35, 8), rotation: -180, order: -10),

        Foot1 = new(new Vector2(-3, 30), rotation: -70, order: -20),
        Foot2 = new(new Vector2(0, 5), rotation: -70, order: -21),
    };
}
