namespace ShGame.Drawing.Models;

using Silk.NET.OpenGL;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.IO;
using StbImageSharp;

public unsafe abstract class TextureDrawable<ModelT> : Drawable<ModelT> {

	Image<Rgba32> image;
	private byte[] pixelData;
	private uint texture;

	private uint width;
	private uint height;

	public unsafe TextureDrawable(string path, ModelT viewModel, uint verticesCount) :base(viewModel, verticesCount) {
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

	public void BindTexture(GL? gl) => gl?.BindTexture(TextureTarget.Texture2D, texture);
	public static void UnbindTexture(GL? gl) => gl?.BindTexture(TextureTarget.Texture2D, 0);
}

