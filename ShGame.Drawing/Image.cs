namespace ShGame.Drawing;

using Silk.NET.OpenGL;

using SixLabors.ImageSharp.PixelFormats;

using StbImageSharp;

using System.IO;
using System.Runtime.InteropServices;

using static System.Net.Mime.MediaTypeNames;

using SharpImage = SixLabors.ImageSharp.Image;

public unsafe struct Image : IDisposable {

	public readonly uint Width, Height;

	private readonly byte* pixelData;
	private readonly uint dataSize;

	internal uint texture;

	private bool IsLoaded { set; get; } = false;

	internal Image(string path) {
		var image = SharpImage.Load<Rgba32>(path);
		Width = (uint)image.Width;;
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

		IsLoaded = true;
	}

	internal void Bind(GL gl) => gl.BindTexture(TextureTarget.Texture2D, texture);

	public void Dispose() {
		NativeMemory.Free(pixelData);
	}

}
