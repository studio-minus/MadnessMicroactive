using OpenTK.Graphics.OpenGL;
using System.Numerics;
using Walgelijk;

namespace MadnessMicroactive;

public static class IntroScene
{
    public static Scene Create(Game game)
    {
        var scene = new Scene(game);
        scene.ShouldBeDisposedOnSceneChange = true;

        var d = new QOIDecoder();
        var read = d.Decode(File.ReadAllBytes("resources/textures/intro.qoi"), true);


        for (int i = 0; i < Math.Min(128, read.Width * read.Height); i++)
        {
            if (read.Colors[i].A < 0.5f)
                continue;
            var e = scene.CreateEntity();
            scene.AttachComponent(e, new BatchedSpriteComponent(Texture.White) { });
        }

        scene.AddSystem(new IntroSystem(read));
        scene.AddSystem(new BatchRendererSystem());
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

        return scene;
    }

    public class IntroSystem : Walgelijk.System
    {
        private readonly DecodedImage read;
        private readonly Color accent = new Color("#FF0061");
        private Vector2 mousePos;
        private Vector2[] offsets;

        public IntroSystem(DecodedImage read)
        {
            this.read = read;
            offsets = new Vector2[read.Colors.Count(static r => r.R >= 0.5f)];
        }

        public override void Update()
        {
            if (Time.SecondsSinceLoad > 8)
            {
                Game.Scene = MenuScene.Create(Game);
                return;
            }

            var sprites = Scene.GetAllComponentsOfType<BatchedSpriteComponent>().GetEnumerator();
            int i = 0;

            mousePos = Utilities.SmoothApproach(mousePos, Input.WorldMousePosition, 15, Time.DeltaTime);

            int offsetIndex = 0;
            for (int y = 0; y < read.Height; y++)
                for (int x = 0; x < read.Width; x++)
                {
                    if (read.Colors[i++].R < 0.5f)
                        continue;

                    if (!sprites.MoveNext())
                        return;

                    float time = ((Time.SecondsSinceLoad - Utilities.Hash(y * .0543f + x * 0.0643f) - 0.5f + y * 0.05f) / 6);
                    var openTime = Utilities.Clamp(Utilities.MapRange(0, 0.2f, 0, 1, time));
                    var closeTime = Utilities.Clamp(Utilities.MapRange(0.9f, 1, 0, 1, time));
                    bool isOpening = closeTime < float.Epsilon;

                    const float scale = 8;

                    var sprite = sprites.Current;

                    var position = new Vector2(x * (scale + 2), (y) * (scale + 2)) - new Vector2(read.Width * (scale + 2) / 2, read.Height * (scale + 2) / 2);
                    var origin = new Vector2(
                        (Utilities.Hash(x * .0543f - y * 0.0643f) * 2 - 1),
                        (Utilities.Hash(x * .0153f + y * 0.091f) * 2 - 1))
                            * 1024;

                    position = Utilities.Lerp(origin, position, Easings.Cubic.Out(openTime));
                    var th = Utilities.LerpAngle((Utilities.Hash(x * .0153f + y * 0.091f) * 2 - 1) * 45, 0, Easings.Expo.Out(openTime));
                    var skew = Utilities.Lerp((Utilities.Hash(y * .0153f + x * 0.091f) * 2 - 1) * MathF.PI * 0.5f, 0, Easings.Quad.Out(openTime));

                    if (!isOpening)
                        position.Y -= Easings.Expo.In(closeTime) * 50;

                    var s = Utilities.Lerp(4, 1, Easings.Cubic.Out(openTime));

                    var toMouse = mousePos - position;
                    var toMouseLgth = toMouse.Length();
                    if (toMouseLgth < 200)
                    {
                        var influence = 1 - (toMouseLgth / 200);
                        offsets[offsetIndex] = Utilities.SmoothApproach(
                            offsets[offsetIndex],
                            -toMouse * 20f / Math.Max(20, toMouseLgth) * influence,
                            8, Time.DeltaTime);
                    }
                    else
                        offsets[offsetIndex] = Utilities.SmoothApproach(
                            offsets[offsetIndex],
                            default,
                            8, Time.DeltaTime);

                    position += offsets[offsetIndex++];

                    var acc = Utilities.Lerp(accent, Colors.Red, Easings.Cubic.InOut(Utilities.Clamp(time * 2)));

                    sprite.Color = isOpening ? Utilities.Lerp(Colors.White.WithAlpha(0), acc, openTime) : Utilities.Lerp(acc, Colors.Gray.WithAlpha(0), closeTime);
                    sprite.Transform =
                        Matrix3x2.CreateRotation(th) *
                        Matrix3x2.CreateSkew(skew, 0) *
                        Matrix3x2.CreateScale(scale * s) *
                        Matrix3x2.CreateTranslation(position);
                }
        }

        public static Vector2 Hash21(float p)
        {
            var inputVector = new Vector3(p);
            var fractVector = new Vector3(.1031f, .1030f, .0973f);

            var p3 = new Vector3(
                inputVector.X * fractVector.X,
                inputVector.Y * fractVector.Y,
                inputVector.Z * fractVector.Z);

            float dotProduct = Vector3.Dot(p3, new Vector3(p3.Y, p3.Z, p3.X) + new Vector3(33.33f));
            p3 += new Vector3(dotProduct);

            var fractPart = new Vector2(p3.X - (int)p3.X, p3.Y - (int)p3.Y);

            var result = new Vector2(
                fractPart.X + p3.Y * p3.Z - (int)(fractPart.X + p3.Y * p3.Z),
                fractPart.Y + p3.Z * p3.Y - (int)(fractPart.Y + p3.Z * p3.Y)
            );

            return result;
        }
    }
}
