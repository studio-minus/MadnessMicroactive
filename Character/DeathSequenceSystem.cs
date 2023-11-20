using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public class DeathSequenceSystem : Walgelijk.System
{
    public override void Update()
    {
        if (!Scene.FindAnyComponent<CameraComponent>(out var camera))
            return;

        var center = new Vector2(0, Window.Height / 2);
        var camTransform = Scene.GetComponentFrom<TransformComponent>(camera.Entity);

        camTransform.Position = center;
        camera.OrthographicSize = 1;

        foreach (var item in Scene.GetAllComponentsOfType<DeathSequenceComponent>())
        {
            if (!item.Character.TryGet(Scene, out var character))
                continue;

            if (character.IsAlive)
            {
                Scene.DetachComponent<DeathSequenceComponent>(camera.Entity);
                continue;
            }

            // THIS IS FUCKED
            if (Input.IsKeyPressed(Key.Enter))
            {
                RoutineScheduler.Start(retry());
                IEnumerator<IRoutineCommand> retry()
                {
                    yield return new RoutineFrameDelay();
                    if (Game.Scene.HasSystem<ExperimentScene.ExperimentModeSystem>())
                        LevelScene.Revive();
                    else
                        Game.Scene = LevelScene.Create(Game, Levels.Campaign[PersistentData.Instance.LevelIndex]);
                }
                return;
            }

            var moveDuration = 3;
            var t = Utilities.Clamp(item.Time / moveDuration);
            var zoom = Utilities.Lerp(1, 4, 1 / Easings.Cubic.InOut(1 / t));
            var pos = Utilities.Lerp(center, character.Body.Get(Scene).Position, Easings.Cubic.InOut(t));

            camTransform.Position = pos;
            camera.OrthographicSize = 1 / zoom;

            item.Time += Time.DeltaTime;
        }
    }
}
