using Walgelijk;

namespace MadnessMicroactive;

public class SpriteMaterialCache : Cache<IReadableTexture, Material>
{
    public static readonly SpriteMaterialCache Instance = new();

    protected override Material CreateNew(IReadableTexture texture)
    {
        Material mat = new(Shader.Default);
        mat.SetUniform(ShaderDefaults.MainTextureUniform, texture);
        return mat;
    }

    protected override void DisposeOf(Material loaded)
    {
        Game.Main.Window.Graphics.Delete(loaded);
    }

    public Material Load(string resourcePath) => Load(Resources.Load<IReadableTexture>(resourcePath));
}