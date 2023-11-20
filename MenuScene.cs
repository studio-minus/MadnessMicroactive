using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Walgelijk;
using Walgelijk.Onion;
using Walgelijk.Onion.Controls;
using Walgelijk.Onion.Decorators;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public static class MenuScene
{
    private static readonly string VersionText = 
        $"Walgelijk v{typeof(Component).Assembly!.GetName()!.Version} | " +
        $"Madness Microactive v{Assembly.GetEntryAssembly()!.GetName()!.Version}";

    public enum Screen
    {
        None,
        Character,
        Credits
    }

    public static Scene Create(Game game)
    {
        var scene = new Scene(game);
        scene.ShouldBeDisposedOnSceneChange = true;

        scene.AddSystem(new OnionSystem());
        scene.AddSystem(new MainMenuSystem());
        scene.AddSystem(new MusicSystem());

        return scene;
    }

    public class MainMenuSystem : Walgelijk.System
    {
        public static Screen CurrentScreen;

        public override void Update()
        {
            Draw.Reset();
            Draw.ScreenSpace = true;
            Draw.Colour = new Color(76, 76, 76);
            Draw.Quad(new Rect(0, 0, Window.Width, Window.Height));

            Ui.Theme.Padding(32).OutlineColour(Colors.Black).OutlineWidth(8).Rounding(16).Once();
            Ui.Layout.Width(300).FitHeight().StickTop().StickLeft();
            Ui.Decorate(new HeaderDecorator("Main Menu"));
            Ui.StartGroup(true);
            {
                Ui.Theme.Padding(20).Once();
                Ui.Layout.FitContainer().StickTop().StickLeft().VerticalLayout();
                Ui.StartScrollView();
                {
                    Ui.Layout.FitWidth().Scale(-20, 0).Height(78).StickLeft();
                    if (MainMenuButton.Start(PersistentData.Instance.LevelIndex == 0 ? "New Game" : "Continue", Resources.Load<Texture>("ui_newgame.png")))
                    {
                        Game.Scene = LevelScene.Create(Game, Levels.Campaign[PersistentData.Instance.LevelIndex]);
                        return;
                    }

                    Ui.Layout.FitWidth().Scale(-20, 0).Height(78).StickLeft();
                    if (MainMenuButton.Start("Character", Resources.Load<Texture>("ui_character.png")))
                        CurrentScreen = CurrentScreen == Screen.Character ? Screen.None : Screen.Character;

                    Ui.Layout.FitWidth().Scale(-20, 0).Height(78).StickLeft();
                    if (MainMenuButton.Start("Experiment", Resources.Load<Texture>("ui_experiment.png")))
                    {
                        Game.Scene = ExperimentScene.Create(Game);
                        return;
                    }
                    Ui.Layout.FitWidth().Scale(-20, 0).Height(78).StickLeft();
                    if (MainMenuButton.Start("Credits", Resources.Load<Texture>("ui_credits.png")))
                        CurrentScreen = CurrentScreen == Screen.Credits ? Screen.None : Screen.Credits;

                    Ui.Layout.FitWidth().Scale(-20, 0).Height(78).StickLeft();
                    if (MainMenuButton.Start("Quit", Resources.Load<Texture>("ui_close.png")))
                    {
                        Window.IsVisible = false;
                        Game.Stop();
                    }
                }
                Ui.End();
            }
            Ui.End();

            if (CurrentScreen is not Screen.None)
            {
                Ui.Theme.Padding(32).OutlineColour(Colors.Black).OutlineWidth(8).Rounding(16).Once();
                Ui.Layout.FitContainer().Scale(-332, 0).StickTop().StickRight();
                Ui.Decorate(new HeaderDecorator(CurrentScreen.ToString()));
                Ui.StartGroup(true);
                {
                    Ui.Theme.Padding(20).Once();
                    Ui.Layout.FitContainer().StickTop().StickLeft().VerticalLayout();
                    Ui.StartScrollView();
                    {
                        switch (CurrentScreen)
                        {
                            case Screen.Character:
                                CharacterEditor();
                                break;
                            case Screen.Credits:
                                Ui.Layout.FitContent().FitWidth().Scale(-20, 0).StickLeft().StickTop();
                                Ui.StartGroup(false);
                                {
                                    Ui.Layout.FitContainer(0.5f, null, false).PreferredSize().StickLeft(false).StickTop(false);
                                    Ui.TextRect("<color=#af0000>Krinkels\nMax Abernethy\n\nMModule\nCheshyre\n\nzooi", HorizontalTextAlign.Left, VerticalTextAlign.Top);
                                    Ui.Layout.FitContainer(0.5f, null, false).PreferredSize().StickRight(false).StickTop(false);
                                    Ui.TextRect("Original concept\n\n\nMusic\n\n\nProgramming, Art", HorizontalTextAlign.Left, VerticalTextAlign.Top);
                                }
                                Ui.End();
                                break;
                        }
                    }
                    Ui.End();
                }
                Ui.End();
            }
            else
            {
                Ui.Layout.Size(Window.Width - 332, Window.Height).Move(332, 0);
                Ui.Theme.Font(Resources.Load<Font>("impact.wf")).FontSize(60).Text(Colors.White).Once();
                Ui.TextRect("Madness Microactive", HorizontalTextAlign.Right, VerticalTextAlign.Middle);

                Ui.Layout.FitWidth().Height(32).StickRight().StickBottom();
                Ui.TextRect(VersionText, HorizontalTextAlign.Right, VerticalTextAlign.Bottom);
            }
        }

        private static void CharacterEditor()
        {
            var t = Game.Main.State.Time.SecondsSinceSceneChange * 0.06f;
            Ui.Theme.Padding(8);
            {
                var player = Outfits.Player;

                Ui.Layout.Size(150, 210).CenterHorizontal().StickTop();
                Ui.Theme.Foreground(new(new Color(76, 76, 76))).Rounding(16).Once();
                Ui.StartGroup(); // draw avatar with UI elements lmfao
                {
                    var pose = CharacterPoses.Idle;

                    pose.Head.Translation += 4 * Noise2D(t, 1, 0);
                    pose.Body.Translation += 2 * Noise2D(t, 2, 0);
                    pose.Hand1.Translation += 3 * Noise2D(t, 3, 0);
                    pose.Hand2.Translation += 3 * Noise2D(t, 4, 0);

                    var head = Resources.Load<Texture>("character/head.png");
                    var body = Resources.Load<Texture>("character/body.png");
                    var hand = Resources.Load<Texture>("character/hand.png");
                    var foot = Resources.Load<Texture>("character/foot.png");

                    int v = 1;
                    foreach (var item in player.Head)
                    {
                        img(item.Texture, pose.Head.Translation + item.Offset, v, item.GetHashCode());
                        v++;
                    }
                    img(head, pose.Head.Translation, 0);
                    img(hand, pose.Hand1.Translation, 0);
                    img(body, pose.Body.Translation, -20);
                    v = -19;
                    foreach (var item in player.Body)
                    {
                        img(item.Texture, pose.Body.Translation + item.Offset, v, item.GetHashCode());
                        v++;
                    }
                    img(hand, pose.Hand2.Translation, -21);
                    img(foot, pose.Foot1.Translation - Vector2.UnitX * 5, -21);
                    img(foot, pose.Foot2.Translation - Vector2.UnitX * 5, -23);

                    void img(IReadableTexture tex, Vector2 offset, int order, [CallerLineNumber] int site = 0)
                    {
                        Ui.Layout.Size(tex.Width, tex.Height).Center().MoveAbs(0, 85).MoveAbs(offset.X, -offset.Y).Order(order);
                        Ui.Image(tex, ImageContainmentMode.Center, site);
                    }
                }
                Ui.End();

                Ui.Label("Head");
                Ui.Layout.FitWidth().Scale(-15, 0).Height(100).StickLeft().HorizontalLayout();
                Ui.Theme.Foreground(new(new Color(110, 110, 110))).Once();
                Ui.StartScrollView(true);
                {
                    foreach (var item in Apparel.HeadClothes)
                    {
                        var equipped = player.Head.Contains(item);

                        Ui.Layout.FitHeight().Width(90).StickTop();
                        Ui.Theme.Foreground(new(new Color(110, 110, 110)));
                        Ui.Decorators.Tooltip(item.Name);
                        if (equipped)
                            Ui.Decorate(new SelectedApparelDecorator());
                        if (Ui.ImageButton(item.Texture, ImageContainmentMode.Contain, identity: item.GetHashCode()))
                        {
                            if (equipped)
                                player.Head.Remove(item);
                            else
                                player.Head.Add(item);
                        }
                    }
                }
                Ui.End();

                Ui.Spacer(15);

                Ui.Label("Body");
                Ui.Layout.FitWidth().Scale(-15, 0).Height(100).StickLeft().HorizontalLayout();
                Ui.Theme.Foreground(new(new Color(110, 110, 110))).Once();
                Ui.StartScrollView(true);
                {
                    foreach (var item in Apparel.BodyClothes)
                    {
                        var equipped = player.Body.Contains(item);

                        Ui.Layout.FitHeight().Width(90).StickTop();
                        Ui.Theme.Foreground(new(new Color(110, 110, 110)));
                        Ui.Decorators.Tooltip(item.Name);
                        if (equipped)
                            Ui.Decorate(new SelectedApparelDecorator());
                        if (Ui.ImageButton(item.Texture, ImageContainmentMode.Contain, identity: item.GetHashCode()))
                        {
                            if (equipped)
                                player.Body.Remove(item);
                            else
                                player.Body.Add(item);
                        }
                    }
                }
                Ui.End();
            }
            Ui.Theme.Pop();
        }
    }

    public static Vector2 Noise2D(float x, float y, float z)
    {
        return new Vector2(
            Noise.GetValue(x, y, z),
            Noise.GetValue(-x, -z, -y)
            );
    }

    public readonly struct SelectedApparelDecorator : IDecorator
    {
        public void RenderBefore(in ControlParams p) { }

        public void RenderAfter(in ControlParams p)
        {
            Draw.Colour = default;
            Draw.OutlineWidth = 8;
            Draw.OutlineColour = new Color(147, 179, 72).WithAlpha(0.9f);
            Draw.ResetTexture();
            //Draw.ResetDrawBounds();
            Draw.Quad(p.Instance.Rects.Rendered.Expand(Utilities.MapRange(-1, 1, 0, 2, MathF.Sin(p.GameState.Time.SecondsSinceLoad * 4))));
        }
    }

    public readonly struct HeaderDecorator : IDecorator
    {
        public readonly string Text;

        public HeaderDecorator(string text)
        {
            Text = text;
        }

        public void RenderBefore(in ControlParams p) { }

        public void RenderAfter(in ControlParams p)
        {
            Draw.Font = p.Theme.Font;
            Draw.FontSize = p.Theme.FontSize.Default;
            var r = new Rect(0, 0, Draw.CalculateTextWidth(Text ?? string.Empty) + 30, 40);
            r = r.Translate(p.Instance.Rects.ComputedGlobal.BottomLeft);
            r = r.Translate(32, r.Height / -2);
            Draw.Colour = new Color("#93B348");
            Draw.OutlineColour = Colors.Black;
            Draw.OutlineWidth = 8;
            Draw.ResetTexture();
            Draw.ResetDrawBounds();
            Draw.Quad(r, 0);
            Draw.Colour = p.Theme.Text.Default;
            if (!string.IsNullOrEmpty(Text))
                Draw.Text(Text, r.GetCenter(), Vector2.One, HorizontalTextAlign.Center, VerticalTextAlign.Middle);
        }
    }
}
