using System.Numerics;
using Walgelijk;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public static class FinalWinScene
{
    private static Sound music = SoundCache.Instance.LoadMusic(Resources.Load<StreamAudioData>("music/end.ogg"));

    public static Scene Create(Game game)
    {
        var scene = new Scene(game);
        scene.ShouldBeDisposedOnSceneChange = true;

        scene.AddSystem(new FinalWinSystem());
        scene.AddSystem(new TransformSystem());
        scene.AddSystem(new CameraSystem());
        scene.AttachComponent(scene.CreateEntity(), new BatchRendererStorageComponent());

        var camera = scene.CreateEntity();
        scene.AttachComponent(camera, new TransformComponent()
        {
            Position = default
        });
        scene.AttachComponent(camera, new CameraComponent()
        {
            ClearColour = Colors.Black,
            OrthographicSize = 1,
            PixelsPerUnit = 1
        });

        game.AudioRenderer.StopAll();
        game.AudioRenderer.Play(music);

        return scene;
    }

    public class FinalWinSystem : Walgelijk.System
    {
        public override void Update()
        {
            if (Input.IsKeyReleased(Key.Escape))
            {
                Audio.StopAll();
                Game.Scene = MenuScene.Create(Game);
                return;
            }

            Draw.Reset();
            Draw.ScreenSpace = true;
            Draw.Colour = Colors.Red;

            Draw.Font = Resources.Load<Font>("impact.wf");
            Draw.FontSize = 120;
            Draw.Text("GOD IS DEAD", new Vector2(Window.Width / 2, 200), Vector2.One, HorizontalTextAlign.Center, VerticalTextAlign.Middle);

            Draw.Colour = Colors.White;
            Draw.FontSize = 32;
            Draw.Text("Thank you for playing. I don't know how to end this game, it was probably quite abrupt. This is not the actual demo for MIR, but I figured it would be funny if I released a demo that was just a completely different tiny little game. I was away for a few days, and I spent that time making this. I hope it was at least kind of fun. If you press F12 and type 'list', you'll get to see some commands and cheats.\n\nIt's 4 AM, goodnight :)", Window.Size / 2 + new Vector2(0, 40), Vector2.One, HorizontalTextAlign.Center, VerticalTextAlign.Middle, Window.Width - 20);
        }
    }
}
