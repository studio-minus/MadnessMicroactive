namespace MadnessMicroactive;

public static class Outfits
{
    public static readonly Outfit Player = new(nameof(Player))
    {
        Head = new List<Appareltem> { Apparel.BaseballCap },
        Body = new List<Appareltem> { },
    };

    public static readonly Outfit Grunt = new(nameof(Grunt))
    {
        Head = new List<Appareltem> {  },
        Body = new List<Appareltem> {  },
    };

    public static readonly Outfit Agent = new(nameof(Agent))
    {
        Head = new List<Appareltem> { Apparel.AgentGlasses },
        Body = new List<Appareltem> { Apparel.AgentBody },
    };

    public static readonly Outfit Engineer = new(nameof(Engineer))
    {
        Head = new List<Appareltem> { Apparel.Goatee, Apparel.GeekyGlasses },
        Body = new List<Appareltem> { Apparel.BusinessSuit },
    };

    public static readonly Outfit Soldat = new(nameof(Soldat))
    {
        Head = new List<Appareltem> { Apparel.SamFisher },
        Body = new List<Appareltem> { Apparel.BusinessSuitWhite },
    };

    public static readonly Outfit Boss = new(nameof(Soldat))
    {
        Head = new List<Appareltem> { Apparel.SantyClause, Apparel.Halo },
        Body = new List<Appareltem> { },
    };
}