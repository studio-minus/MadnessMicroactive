using Walgelijk;

namespace MadnessMicroactive;

public class LimbComponent : Component
{
    public ComponentRef<CharacterComponent> Character;

    public LimbComponent(ComponentRef<CharacterComponent> character)
    {
        Character = character;
    }
}