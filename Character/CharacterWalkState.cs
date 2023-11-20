using System.Numerics;

namespace MadnessMicroactive;

public struct CharacterWalkState
{
    public float MainTimer;
    public float HopTimer;
    public float HopDuration;
    public bool InHop;
    public Vector2 Origin, Destination;
    public bool Flying;
}
