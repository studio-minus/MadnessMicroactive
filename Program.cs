using System.Numerics;
using Walgelijk;
using Walgelijk.Onion;
using Walgelijk.OpenTK;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public class Program
{
    public static readonly Program Instance = new();
    public readonly Game Game;

    public static void Main(string[] args)
    {
        global::OpenTK.Audio.OpenAL.ALBase.RegisterOpenALResolver();

        _ = Instance;
    }

    public Program()
    {
        Game = new(
            new OpenTKWindow("Madness Microactive", new Vector2(-1), new Vector2(1280, 720)),
            new OpenALAudioRenderer()
        );

        Game.UpdateRate = 120;
        Game.FixedUpdateRate = 60;
        Game.Window.VSync = false;
        Game.Window.Resizable = false;

        Draw.CacheTextMeshes = -1;

        TextureLoader.Settings.FilterMode = FilterMode.Nearest;

        Resources.SetBasePathForType<FixedAudioData>("audio");
        Resources.SetBasePathForType<StreamAudioData>("audio");
        Resources.SetBasePathForType<Texture>("textures");
        Resources.SetBasePathForType<Font>("fonts");

        Onion.Theme.Base.Rounding = 0;
        Onion.Theme.Base.Foreground = new(Colors.Gray, new Color("#93B348"));
        Onion.Theme.Base.Text = Colors.Black;
        Onion.Theme.Base.FontSize = 20;
        Onion.Theme.Base.Font = Resources.Load<Font>("arialbd.wf");
        Onion.Theme.Base.OutlineWidth = 0;
        Onion.Theme.Base.ShowScrollbars = false;
        Onion.Theme.Base.Accent = new Color("#93B348");
        Onion.Theme.Base.Background = new(new Color("#93B348"));

        Onion.Animation.Default.Clear();
        Onion.Animation.DefaultDurationSeconds = 0;
        Onion.Configuration.ScrollSensitivity = 24;

        Onion.HoverSound = null; 
        Onion.ActiveSound = null; 

        Game.Scene = IntroScene.Create(Game);

        Game.AudioRenderer.Play(SoundCache.Instance.LoadMusicNonLoop(Resources.Load<StreamAudioData>("music/splitmek.ogg")));

        Game.DevelopmentMode = true;
        Game.Console.DrawConsoleNotification = false;
        Game.Window.SetIcon(Resources.Load<Texture>("icon.png"));
        Game.Profiling.DrawQuickProfiler = false;

        Game.Compositor.Enabled = false;

        Game.Start();
    }
}
