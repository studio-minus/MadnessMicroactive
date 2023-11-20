using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public static class Apparel
{
    public static readonly Appareltem AgentGlasses =
        new("Agent glasses", Resources.Load<Texture>("character/agent_glasses.png"), new Vector2(26, 0));

    public static readonly Appareltem SherrifHat =
        new("Sherrif hat", Resources.Load<Texture>("character/sherrif_hat.png"), new Vector2(0, 42));

    public static readonly Appareltem BaseballCap =
        new("Baseball cap", Resources.Load<Texture>("character/baseball_cap.png"), new Vector2(11, 28));

    public static readonly Appareltem Bandana =
        new("Bandana", Resources.Load<Texture>("character/bandana.png"), new Vector2(-7, 0));

    public static readonly Appareltem TopHat =
        new("Top hat", Resources.Load<Texture>("character/hoed.png"), new Vector2(3, 136));

    public static readonly Appareltem ClownWig =
        new("Clown wig", Resources.Load<Texture>("character/clown_wig.png"), new Vector2(-11, 20));

    public static readonly Appareltem PirateHat =
        new("Pirate hat", Resources.Load<Texture>("character/pirate_hat.png"), new Vector2(1, 39));

    public static readonly Appareltem GeekyGlasses =
        new("Geeky glasses", Resources.Load<Texture>("character/geeky.png"), new Vector2(10, 0));

    public static readonly Appareltem SamFisher =
        new("Sam Fisher", Resources.Load<Texture>("character/sam_fisher.png"), new Vector2(23, 7));

    public static readonly Appareltem EyePatch =
        new("Eye patch", Resources.Load<Texture>("character/eyepatch.png"), new Vector2(0, -1));

    public static readonly Appareltem Goatee =
        new("Goatee", Resources.Load<Texture>("character/goatee.png"), new Vector2(20, -30));

    public static readonly Appareltem SantyClause =
        new("Santy Clause", Resources.Load<Texture>("character/santa.png"), new Vector2(2, -47));
    
    public static readonly Appareltem Bonzo =
        new("Bonzo", Resources.Load<Texture>("character/bonzo.png"), new Vector2(19, -22));
        
    public static readonly Appareltem Ninja =
        new("Ninja", Resources.Load<Texture>("character/ninja.png"), new Vector2(-9, -1));        

    public static readonly Appareltem Goggles =
        new("Goggles", Resources.Load<Texture>("character/goggles.png"), new Vector2(1, -3));

    public static readonly Appareltem Halo =
        new("Halo", Resources.Load<Texture>("character/halo.png"), new Vector2(0, 45));

    public static readonly Appareltem BusinessSuit =
        new("Business suit", Resources.Load<Texture>("character/businessman.png"), default);
    
    public static readonly Appareltem BusinessSuitWhite =
        new("Business suit (white)", Resources.Load<Texture>("character/whitebusinessman.png"), default);

    public static readonly Appareltem NinjaSuit =
        new("Ninja suit", Resources.Load<Texture>("character/ninja_suit.png"), default);

    public static readonly Appareltem AgentBody =
        new("Agent suit", Resources.Load<Texture>("character/agent_body.png"), default);

    //--------------------------------------------------

    public static readonly Appareltem[] HeadClothes =
    {
        SherrifHat,
        BaseballCap,
        Bandana,
        TopHat,
        ClownWig,
        PirateHat,
        Halo,

        GeekyGlasses,
        SamFisher,
        EyePatch,
        AgentGlasses,
        Goggles,

        Goatee,
        SantyClause,
        Bonzo,
        Ninja,
    };

    public static readonly Appareltem[] BodyClothes =
    {
        BusinessSuit,
        BusinessSuitWhite,
        NinjaSuit,
        AgentBody,
    };
}
