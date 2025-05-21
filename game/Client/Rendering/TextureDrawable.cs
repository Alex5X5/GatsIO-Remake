namespace ShGame.Game.Client.Rendering;

using Silk.NET.OpenGL;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

public unsafe abstract class TextureDrawable:Drawable {

    Image<Rgba32> image;
    private byte[] pixelData;
    private uint texture;

    private uint width;
    private uint height;

    public TextureDrawable(string path, uint verticesCount):base(verticesCount) {
        image = Image.Load<Rgba32>(path);
        width = (uint)image.Width;
        height = (uint)image.Height;
        pixelData = new byte[width * height * 4];
        image.CopyPixelDataTo(pixelData);
    }

    public unsafe void Setup(GL gl) {
        base.Setup(gl);
        gl.GenTextures(1, out texture);
        BindTexture();
        fixed(byte* ptr = &pixelData[0])
            gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

        // Texture settings
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        UnbindTexture();
    }

    public unsafe void Draw(GL gl) {
        if (dirty) {
            UpdateVertices();
            dirty = false;
        }
        BindVAO();
        BindVBO();
        BindTexture();
        float* ptr = VertexDataPtr; 
        for (nuint i = 0; i<VERTICES_COUNT; i+=9) {
            gl.BufferSubData(GLEnum.ArrayBuffer, (nint)i*sizeof(float), (uint)9*sizeof(float), ptr);
            ptr+=(uint)9;
        }
        for (nuint i = 0; i<VERTICES_COUNT; i+=9) {
            gl.BufferSubData(GLEnum.ArrayBuffer, (nint)i*sizeof(float), (uint)9*sizeof(float), ptr);
            ptr+=(uint)9;
        }
        gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)VERTICES_COUNT);
        UnbindVAO();
        UnbindVBO();
        UnbindTexture();
    }

    public void BindTexture() => RendererGl.Gl?.BindTexture(TextureTarget.Texture2D, texture);
    public static void UnbindTexture() => RendererGl.Gl?.BindTexture(TextureTarget.Texture2D, 0);
}

