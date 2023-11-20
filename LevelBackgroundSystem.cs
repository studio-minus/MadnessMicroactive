using Walgelijk;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public class LevelBackgroundSystem : Walgelijk.System
{
    public override void Update()
    {
        if (!Scene.FindAnyComponent<LevelComponent>(out var level))
            return;

        Draw.Reset();
        Draw.Order = new RenderOrder(-500);

        var r = new Rect(0, 0, level.Background.Width, level.Background.Height);
        r = r.Translate(r.Width / -2, r.Height);

        Draw.Image(level.Background, r, ImageContainmentMode.Contain);
    }
}
