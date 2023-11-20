using System.Numerics;

namespace MadnessMicroactive;

public struct LimbPosition
{
    public Vector2 Translation;
    public float Rotation;
    public int Order;

    public LimbPosition(Vector2 translation, float rotation, int order)
    {
        Translation = translation;
        Rotation = rotation;
        Order = order;
    }

    public readonly LimbPosition Flipped => new(
        new Vector2(-Translation.X, Translation.Y),
        -Rotation,
        Order
    );
}
