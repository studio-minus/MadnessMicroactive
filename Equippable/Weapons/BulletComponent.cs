using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class BulletComponent : Component
{
    public Vector2 Position;
    public Vector2 LastPosition;
    public Vector2 Velocity;
    public float Time;

    public float Damage;
}