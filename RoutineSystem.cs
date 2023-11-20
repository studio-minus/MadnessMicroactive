using Walgelijk;

namespace MadnessMicroactive;

public class RoutineSystem : Walgelijk.System
{
    public override void Update()
    {
        RoutineScheduler.StepRoutines(Time.DeltaTime);
    }
}
