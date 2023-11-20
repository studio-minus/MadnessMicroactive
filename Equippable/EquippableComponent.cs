using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

[RequiresComponents(typeof(TransformComponent))]
public class EquippableComponent : Component
{
    public EquippableComponent(Vector2[] holdPoints)
    {
        HoldPoints = holdPoints;
    }

    public bool Enabled = true;

    public Vector2[] HoldPoints;
    public ComponentRef<CharacterComponent> Wielder;
    public Vector2 Velocity;

    public float DecayTimer;

    public bool IsEquipped(Scene scene) => Wielder.IsValid(scene);

    public Vector2 GetHoldPoint(int index, bool flipped)
    {
        if (index < 0 || index >= HoldPoints.Length)
            return default;
        var a = HoldPoints[index];
        if (flipped)
            a.X *= -1;
        return a;
    }
}
