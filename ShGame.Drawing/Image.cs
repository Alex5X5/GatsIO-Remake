namespace ShGame.Drawing;

using System.Runtime.InteropServices;

using Silk.NET.OpenGL;

using StbImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using SharpImage = SixLabors.ImageSharp.Image;
using System.IO;

public unsafe struct Image : IDisposable {

	private SixLabors.ImageSharp.Image<Rgba32> image;
	private readonly byte* pixelData;
	private readonly uint dataSize;
	private uint texture;

	public readonly uint Width, Height;

	public bool IsLoaded { private set; get; } = false;

	public Image(string path) {
		image = SharpImage.Load<Rgba32>(path);
		Width = (uint)image.Width;
		Height = (uint)image.Height;
		dataSize = Width * Height * 4;
		pixelData = (byte*)NativeMemory.Alloc(dataSize);
		image.CopyPixelDataTo(new Span<byte>(pixelData, (int)dataSize));
		image.Dispose();
	}

	internal void Load(GL gl) {
		if (IsLoaded)
			return;
		texture = gl.GenTexture();
		gl.ActiveTexture(TextureUnit.Texture0);
		gl.BindTexture(TextureTarget.Texture2D, texture);
		gl.TexImage2D(
			TextureTarget.Texture2D,
			0,
			InternalFormat.Rgba,
			Width,
			Height,
			0,
			PixelFormat.Rgba,
			PixelType.UnsignedByte,
			pixelData);

		gl.GenerateMipmap(TextureTarget.Texture2D);

		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

		gl.BindTexture(TextureTarget.Texture2D, 0);
		IsLoaded = true;
	}

	public void Dispose() {
		NativeMemory.Free(pixelData);
	}

}
