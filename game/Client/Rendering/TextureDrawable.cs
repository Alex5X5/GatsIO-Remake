namespace ShGame.Game.Client.Rendering;

using Silk.NET.OpenGL;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using ShGame.Game.Util;
using System.Runtime.InteropServices;
using System.IO;
using StbImageSharp;

public unsafe abstract class TextureDrawable:Drawable {

	Image<Rgba32> image;
	private byte[] pixelData;
	private uint texture;

	private uint width;
	private uint height;

	public unsafe TextureDrawable(string path, uint verticesCount):base(verticesCount) {
		//byte* ptr = (byte*)NativeMemory.AllocZeroed(3);
		//new Span<byte>(ptr, (int)fileSize)
		image = Image.Load<Rgba32>(path);
		width = (uint)image.Width;
		height = (uint)image.Height;
		pixelData = new byte[width * height * 4];
		image.CopyPixelDataTo(pixelData);
	}

	public unsafe void Setup(GL gl) {
		base.Setup(gl);

        gl.EnableVertexAttribArray(1);
	}

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

	public void BindTexture(GL? gl) => gl?.BindTexture(TextureTarget.Texture2D, texture);
	public static void UnbindTexture(GL? gl) => gl?.BindTexture(TextureTarget.Texture2D, 0);
}

