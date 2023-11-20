using Walgelijk;

namespace MadnessMicroactive;

public class DeathSequenceComponent : Component
{
    public ComponentRef<CharacterComponent> Character;
    public float Time;

    public DeathSequenceComponent(ComponentRef<CharacterComponent> character)
    {
        Character = character;
    }
}