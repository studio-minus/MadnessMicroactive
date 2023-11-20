using Walgelijk;

namespace MadnessMicroactive;

public class CharacterApparelSystem : Walgelijk.System
{
    public override void FixedUpdate()
    {
        foreach (var character in Scene.GetAllComponentsOfType<CharacterComponent>())
        {
            if (character.OutfitSprites != null)
            {
                int index = 0;
                var headTransform = character.Head.Get(Scene);
                var bodyTransform = character.Body.Get(Scene);

                var headSprite = Scene.GetComponentFrom<BatchedSpriteComponent>(headTransform.Entity);
                var bodySprite = Scene.GetComponentFrom<BatchedSpriteComponent>(bodyTransform.Entity);

                foreach (var headApparel in character.Outfit.Head)
                {
                    if (character.OutfitSprites[index].TryGet(Scene, out var sprite))
                    {
                        var transform = Scene.GetComponentFrom<TransformComponent>(sprite.Entity);
                        var offset = headApparel.Offset;
                        if (character.Flipped)
                            offset.X *= -1;

                        transform.Position = headTransform.Position + Utilities.RotatePoint(offset, headTransform.Rotation);
                        transform.Rotation = headTransform.Rotation;
                        sprite.VerticalFlip = character.Flipped;
                        sprite.RenderOrder = headSprite.RenderOrder.OffsetOrder(1);
                    }
                    index++;
                }

                foreach (var bodyApparel in character.Outfit.Body)
                {
                    if (character.OutfitSprites[index].TryGet(Scene, out var sprite))
                    {
                        var transform = Scene.GetComponentFrom<TransformComponent>(sprite.Entity);
                        var offset = bodyApparel.Offset;
                        if (character.Flipped)
                            offset.X *= -1;

                        transform.Position = bodyTransform.Position + Utilities.RotatePoint(offset, bodyTransform.Rotation);
                        transform.Rotation = bodyTransform.Rotation;
                        sprite.VerticalFlip = character.Flipped;
                        sprite.RenderOrder = bodySprite.RenderOrder.OffsetOrder(1);
                    }
                    index++;
                }
            }
        }
    }
}
