using Silk.NET.OpenGL;

using StbImageSharp;

namespace ShGame.Drawing.Services;

public class TextureService {

	public static unsafe uint CreateGlTexture(GL? Gl, string path) {
		uint tex = Gl.GenTexture();
		Gl.ActiveTexture(TextureUnit.Texture0);
		Gl.BindTexture(TextureTarget.Texture2D, tex);


		ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);

		fixed (byte* ptr = result.Data)
			Gl.TexImage2D(
				TextureTarget.Texture2D,
				0,
				InternalFormat.Rgba,
				(uint)result.Width,
				(uint)result.Height,
				0,
				PixelFormat.Rgba,
				PixelType.UnsignedByte,
				ptr
			);

		Gl.GenerateMipmap(TextureTarget.Texture2D);

		Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

		Gl.BindTexture(TextureTarget.Texture2D, 0);

		return tex;
	}
}
