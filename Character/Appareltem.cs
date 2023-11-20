using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class Appareltem
{
    public string Name;
    public IReadableTexture Texture;
    public Vector2 Offset;

    public Appareltem(string name, IReadableTexture texture, Vector2 offset)
    {
        Name = name;
        Texture = texture;
        Offset = offset;
    }

    public override string? ToString() => Name;
}
