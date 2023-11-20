namespace MadnessMicroactive;

public class Outfit
{
    public readonly string Name;
    public List<Appareltem> Head = new();
    public List<Appareltem> Body = new();

    public Outfit(string name)
    {
        Name = name;
    }
}