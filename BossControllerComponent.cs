using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class BossControllerComponent : Component
{
    public BossStage Stage = BossStage.Idle;

    public float HealthAtStartStage;
    public int KillsThisStage = 0;
    public float SpawnTimer = 0;
    public float VulnerableTimer = 0;
    public bool DidLightningAttack = false;

    public Vector2 LightningCursor;
}
