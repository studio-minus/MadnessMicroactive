using Microsoft.Win32;
using Walgelijk;
using Walgelijk.Onion;

namespace MadnessMicroactive;

public class LevelProgressSystem : Walgelijk.System
{
    public override void Update()
    {
        if (Scene.FindAnyComponent<BossControllerComponent>(out _))
            return;

        if (Scene.FindAnyComponent<LevelProgressComponent>(out var progress))
        {
            if (Scene.FindAnyComponent<LevelComponent>(out var level))
            {
                if (progress.Kills >= level.TotalEnemies && !Scene.FindAnyComponent<WonComponent>(out _))
                    Scene.AttachComponent(Scene.CreateEntity(), new WonComponent());
            }

            if (Scene.FindAnyComponent<WonComponent>(out var won) && Scene.FindAnyComponent<PlayerControllerComponent>(out var player))
            {
                var character = Scene.GetComponentFrom<CharacterComponent>(player.Entity);
                Ui.Layout.Size(300, 150).Center();
                Ui.Theme.OutlineColour(Colors.Black).OutlineWidth(8).Rounding(19).Once();
                Ui.StartGroup();
                {
                    Ui.Layout.PreferredSize().FitWidth().StickLeft().StickTop().Move(0, 5);
                    Ui.TextRect("Level Complete!", HorizontalTextAlign.Center, VerticalTextAlign.Top);

                    Ui.Layout.Move(10, 35);
                    Ui.Label($"Kills: " + progress.Kills, HorizontalTextAlign.Left);
                    Ui.Layout.Move(10, 65);
                    Ui.Label($"Time: " + progress.Time.Seconds + "s", HorizontalTextAlign.Left);

                    Ui.Layout.Size(190, 48).StickBottom().CenterHorizontal().Move(0, -5);
                    if (Ui.Button("PROCEED"))
                    {
                        PersistentData.Instance.Health = Math.Max(character.Health, character.Stats.MaxHealth / 2);
                        PersistentData.Instance.LevelIndex++;
                        if (character.Equipped.TryGet(Scene, out var eq) && Scene.TryGetComponentFrom<WeaponComponent>(eq.Entity, out var wpn))
                        {
                            PersistentData.Instance.Ammo = wpn.RemainingRounds;
                            PersistentData.Instance.Weapon = wpn.Weapon;
                        }
                        else
                            PersistentData.Instance.Weapon = null;
                        Game.Scene = LevelScene.Create(Game, Levels.Campaign[PersistentData.Instance.LevelIndex]);
                        return;
                    }
                }
                Ui.End();
            }
            else
                progress.Time += TimeSpan.FromSeconds(Time.DeltaTime);
        }
    }
}

[SingleInstance]
public class WonComponent : Component
{

}
