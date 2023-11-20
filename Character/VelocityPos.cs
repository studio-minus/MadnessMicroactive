using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class VelocityPos
{
    public Vector2 PositionTarget;
    public Vector2 PositionVelocity;

    public float RotationTarget;
    public float RotationVelocity;

    public VelocityPos(Vector2 positionTarget, float rotationTarget)
    {
        PositionTarget = positionTarget;
        RotationTarget = rotationTarget;

        PositionVelocity = default;
        RotationVelocity = default;
    }

    public VelocityPos Update(Vector2 currentPos, float currentRot, float factor = 0.2f)
    {
        PositionVelocity += (PositionTarget - currentPos) * factor;
        RotationVelocity += Utilities.DeltaAngle(currentRot, RotationTarget) * factor;
        return this;
    }

    public VelocityPos SetTarget(Vector2 p, float r)
    {
        PositionTarget = p;
        RotationTarget = r;
        return this;
    }
}
