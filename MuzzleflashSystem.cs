using System.Numerics;
using Walgelijk;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public class MuzzleflashSystem : Walgelijk.System
{
    public override void Update()
    {
        Draw.Reset();
        Draw.Colour = Colors.White;
        foreach (var m in Scene.GetAllComponentsOfType<MuzzleflashComponent>())
        {
            m.Time += Time.DeltaTime;
            if (m.Time > m.MaxTime)
                Scene.RemoveEntity(m.Entity);

            var p = m.Time / m.MaxTime;

            var radius = new Vector2(
                m.Size * 64 * Easings.Quad.Out(p),
                64 * Easings.Expo.In(1 - p)
            );
            var th = -Utilities.VectorToAngle(m.Direction);
            var pos = m.Position + m.Direction * radius.X;

            Draw.Circle(pos, radius, th);
        }
    }
}
