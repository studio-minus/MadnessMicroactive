using System.Numerics;
using Walgelijk;
using Walgelijk.SimpleDrawing;

namespace MadnessMicroactive;

public class PlayerUiSystem : Walgelijk.System
{
    private readonly string[] deathTexts =
    {
        "YOU FAILED",
        "ONE MORE SHOT",
        "ALMOST THERE",
        "OFF TO HELL",
        "CATASTROPHE",
        "CRITICAL DAMAGE",
    };

    public override void Update()
    {
        Draw.Reset();
        Draw.ScreenSpace = true;
        Draw.Colour = Colors.White;
        Draw.Font = Resources.Load<Font>("impact.wf");
        Draw.Order = RenderOrder.UI;

        foreach (var player in Scene.GetAllComponentsOfType<PlayerControllerComponent>())
        {
            var character = Scene.GetComponentFrom<CharacterComponent>(player.Entity);
            DrawUi(player, character);
        }

        if (Input.IsKeyReleased(Key.Escape))
        {
            Game.Scene = MenuScene.Create(Game);
            return;
        }
    }

    private void DrawUi(PlayerControllerComponent player, CharacterComponent character)
    {
        // damage effects
        float h = (character.Stats.MaxHealth - character.Health) / character.Stats.MaxHealth * 64;
        Draw.Colour = Colors.Red;
        Draw.BlendMode = BlendMode.Multiply;
        Draw.Quad(new Rect(0, 0, Window.Width, h));
        Draw.Quad(new Rect(0, Window.Height - h, Window.Width, Window.Height));
        Draw.Quad(new Rect(0, 0, h, Window.Height));
        Draw.Quad(new Rect(Window.Width - h, 0, Window.Width, Window.Height));
        if (1 - character.RegenCooldownTimer * 2 > float.Epsilon)
        {
            var v = Utilities.Clamp(character.RegenCooldownTimer * 2);
            Draw.Colour = new Color(1, v, v);
            Draw.Quad(new Rect(0, 0, Window.Width, Window.Height));
        }
        Draw.BlendMode = default;

        if (!character.IsAlive)
        {
            var progress = Easings.Quad.Out(Utilities.Clamp((character.DeadTime - 2) / 3));

            Draw.Colour = Colors.White.WithAlpha(progress);
            Draw.FontSize = 120;

            Draw.DrawBounds = new DrawBounds(new Rect(0, 0, Window.Width, Window.Height / 2));
            Draw.Text("GAME OVER", new Vector2(Window.Width / 2, Window.Height / 2 - progress * 100), Vector2.One, HorizontalTextAlign.Center, VerticalTextAlign.Top);

            Draw.FontSize = 50;
            Draw.DrawBounds = new DrawBounds(new Rect(0, Window.Height / 2, Window.Width, Window.Height));
            Draw.Text("Press <color=#ff0000>[ENTER]</color> to retry", new Vector2(Window.Width / 2, Window.Height / 2 + progress * 60 - 5), new Vector2(1.2f, 1), HorizontalTextAlign.Center, VerticalTextAlign.Bottom);

            Draw.ResetDrawBounds();

            Draw.FontSize = 32;
            Draw.Colour = Colors.Red.WithAlpha(0.3f);
            for (int i = 0; i < 5; i++)
            {
                var t = Utilities.MapRange(0, 1, -100, Window.Width + 100, ((character.DeadTime + i) / 5) % 1);
                Draw.Text(getDeathText(i), new Vector2(t, 10), new Vector2(1, progress), HorizontalTextAlign.Center, VerticalTextAlign.Top);
            }

            for (int i = 0; i < 5; i++)
            {
                var t = Utilities.MapRange(0, 1, Window.Width + 100, -100, ((character.DeadTime + i) / 5) % 1);
                Draw.Text(getDeathText(i), new Vector2(t, Window.Height - 10), new Vector2(1, progress), HorizontalTextAlign.Center, VerticalTextAlign.Bottom);
            }

            string getDeathText(int seed)
            {
                var v = Noise.GetValue(seed * 13.523f, 94.23f, -character.DeadTime * 0.002f);
                return deathTexts[(int)Utilities.MapRange(-1, 1, 0, deathTexts.Length * 20, v) % deathTexts.Length];
            }
        }
        else
        {
            // weapon name & ammo counter
            if (character.Equipped.TryGet(Scene, out var eq))
            {
                if (Scene.TryGetComponentFrom<WeaponComponent>(eq.Entity, out var wpn))
                {
                    Draw.Colour = Colors.White;
                    Draw.FontSize = 48;
                    Draw.Text(wpn.Weapon.Name, new Vector2(20), Vector2.One, HorizontalTextAlign.Left, VerticalTextAlign.Top);
                    Draw.FontSize = 32;
                    if (wpn.Weapon is Firearm firearm)
                    {
                        Draw.Colour = wpn.RemainingRounds > 0 ? Colors.Black : Colors.Red;
                        Draw.Text($"{wpn.RemainingRounds}/{firearm.MaxRounds}", new Vector2(20, 20 + 48), Vector2.One, HorizontalTextAlign.Left, VerticalTextAlign.Top);
                    }
                }
            }
        }
    }
}
