namespace MadnessMicroactive;

public struct CharacterPose
{
    public (bool, bool) AimAdjust;

    public LimbPosition Head;
    public LimbPosition Body;
    public LimbPosition Hand1, Hand2;
    public LimbPosition Foot1, Foot2;
}
