using Walgelijk;

namespace MadnessMicroactive;

public class Faction
{
    public readonly string Name;
    public readonly Color Color;

    public readonly HashSet<Faction> Enemies = new();

    public Faction(string name, Color color)
    {
        Name = name;
        Color = color;
    }

    public static readonly Faction Player = new(nameof(Player), Colors.Cyan);
    public static readonly Faction AAHW = new(nameof(AAHW), Colors.Red);

    static Faction()
    {
        Player.Enemies.Add(AAHW);
        AAHW.Enemies.Add(Player);
    }
}
