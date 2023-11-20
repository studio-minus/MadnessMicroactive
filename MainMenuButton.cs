using System.Numerics;
using System.Runtime.CompilerServices;
using Walgelijk;
using Walgelijk.Onion.Controls;
using Walgelijk.Onion;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public readonly struct MainMenuButton : IControl
{
    private readonly IReadableTexture img;

    private static readonly OptionalControlState<(bool, bool)> optionalControlState = new((false,false));

    public MainMenuButton(IReadableTexture img)
    {
        this.img = img;
    }

    public static bool Hold(string label, IReadableTexture img, int identity = 0, [CallerLineNumber] int site = 0)
        => Start(label, img, identity, site).Held;

    public static bool Click(string label, IReadableTexture img, int identity = 0, [CallerLineNumber] int site = 0)
        => Start(label, img, identity, site).Up;

    public static InteractionReport Start(string label, IReadableTexture img, int identity = 0, [CallerLineNumber] int site = 0)
    {
        var (instance, node) = Onion.Tree.Start(IdGen.Create(nameof(MainMenuButton).GetHashCode(), identity, site), new MainMenuButton(img));
        instance.RenderFocusBox = false;
        instance.Name = label;
        Onion.Tree.End();
        return new InteractionReport(instance, node, InteractionReport.CastingBehaviour.Up);
    }

    public void OnAdd(in ControlParams p) { }

    public void OnRemove(in ControlParams p) { }

    public void OnStart(in ControlParams p) { }
    public void OnProcess(in ControlParams p)
    {
        ControlUtils.ProcessButtonLike(p);
        var n = optionalControlState.GetValue(p.Identity);

        if (!n.Item1 && p.Instance.IsActive)
            Game.Main.AudioRenderer.PlayOnce(SoundCache.Instance.LoadUISoundEffect(Resources.Load<FixedAudioData>("ui_confirm.wav")));
        else if (!n.Item2 && p.Instance.IsHover)
            Game.Main.AudioRenderer.PlayOnce(SoundCache.Instance.LoadUISoundEffect(Resources.Load<FixedAudioData>("ui_hover.wav")), 0.5f);

        optionalControlState.SetValue(p.Identity, (p.Instance.IsActive, p.Instance.IsHover));
    }

    public void OnRender(in ControlParams p)
    {
        (ControlTree tree, Walgelijk.Onion.Layout.LayoutQueue layout, Input input, GameState state, Node node, ControlInstance instance) = p;

        var t = node.GetAnimationTime();
        var anim = instance.Animations;
        var renderedRect = instance.Rects.Rendered;

        var fg = p.Theme.Foreground[instance.State];
        Draw.Colour = fg.Color;
        Draw.Texture = fg.Texture;
        Draw.OutlineColour = p.Theme.OutlineColour[instance.State];
        Draw.OutlineWidth = p.Theme.OutlineWidth[instance.State];

        anim.AnimateRect(ref instance.Rects.Rendered, t);

        anim.AnimateColour(ref Draw.Colour, t);
        Draw.Quad(instance.Rects.Rendered, 0, p.Theme.Rounding);
        Draw.ResetTexture();

        Draw.Colour = Colors.White;
        anim.AnimateColour(ref Draw.Colour, t);
        var imgRect = new Rect(renderedRect.MinX, renderedRect.MinY, renderedRect.MinX + renderedRect.Height, renderedRect.MinY + renderedRect.Height);
        imgRect = imgRect.Expand(-instance.Theme.Padding);
        Draw.Image(img, imgRect, ImageContainmentMode.Stretch);

        Draw.Font = p.Theme.Font;
        Draw.Colour = p.Theme.Text[instance.State];
        anim.AnimateColour(ref Draw.Colour, t);
        if (anim.ShouldRenderText(t))
        {
            var ratio = renderedRect.Area / instance.Rects.ComputedGlobal.Area;
            Draw.Text(instance.Name,
                new Vector2(renderedRect.MinX + renderedRect.Height + instance.Theme.Padding, (renderedRect.MaxY + renderedRect.MinY) / 2),
                new Vector2(ratio),
                HorizontalTextAlign.Left, VerticalTextAlign.Middle, renderedRect.Width);
        }
    }

    public void OnEnd(in ControlParams p)
    {
    }
}