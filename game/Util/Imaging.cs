using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using static System.Windows.Forms.DataFormats;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ShGame.game.Util;

class Imaging {

	public static unsafe void CreateImage(int width, int height) {

		//IntPtr dataP = (IntPtr)data.Scan0();
		Bitmap map = new(1, 1);
		var bufferAndStride = map.ToBufferAndStride();

		Console.WriteLine();
		int pixelLength = System.Drawing.Image.GetPixelFormatSize(new Bitmap(1, 1).PixelFormat);
		Console.WriteLine(pixelLength.ToString());
		int byteCount = width * height * pixelLength;
		Console.WriteLine(byteCount.ToString());
		byte* ptr = (byte*)NativeMemory.AllocZeroed((nuint)byteCount);

		Random r = new();
		for (int i = 0; i < byteCount; i++) {
			*ptr = (byte)r.Next(255);
			//ptr+=pixelLength;
		}
		System.Drawing.Image.FromHbitmap((nint)ptr).Save("image.png",ImageFormat.Png);
	}

	public static unsafe void CreateImage2() {
		const int width = 1000;
		const int height = 1000;
		const int bytesPerPixel = 4;  // 32-bit pixel format (e.g., ARGB)

		// Calculate the total memory required for the bitmap (width * height * bytes per pixel)
		const long totalBytes = width * height * bytesPerPixel;

		// Allocate unmanaged memory using NativeMemory.Alloc
		IntPtr unmanagedMemory = (IntPtr)NativeMemory.Alloc((nuint)totalBytes);

		try {
			// Initialize the memory with some pixel data if needed (this part is optional)
			Random random = new();
			unsafe {
				byte* ptr = (byte*)unmanagedMemory.ToPointer();
				for (int i = 0; i < totalBytes; i++) {
					//ptr[i] = (byte)ToArgb(255,255,100,0);
					ptr[i] = (byte)random.Next(255); // Example: Fill all bytes with 255 (white pixels)
				}
			}

			// Create a Bitmap object from the unmanaged memory
			Bitmap bitmap = new(
				width,
				height,
				width * bytesPerPixel, // Stride (bytes per row)
				PixelFormat.Format32bppArgb, // 32-bit pixel format (ARGB)
				unmanagedMemory // Pointer to the unmanaged memory
			);

			// Save or display the bitmap
			bitmap.Save("output.png");

			// Dispose of the bitmap when done
			bitmap.Dispose();
		} finally {
			// Free unmanaged memory when done
			NativeMemory.Free((void*)unmanagedMemory);
		}
	}

	private static uint ToArgb(byte alpha, byte red, byte green, byte blue) {
		//Alpha(255 or 0xFF): Shifted 24 bits to the left → 0xFF000000
		//Red(255 or 0xFF): Shifted 16 bits to the left → 0x00FF0000
		//Green(0 or 0x00): Shifted 8 bits to the left → 0x00000000
		//Blue(0 or 0x00): Stays in place → 0x00000000
		return (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
	}

	public static unsafe void Iterate() {
	}

	public static unsafe void Iterate2() {
		//int[,] array = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
		int[,] array = new int[1000,100];
		for (int i = 0; i < array.GetLength(0); i++)
			for (int j = 0; j < array.GetLength(1); j++)
				array[i,j] = i*j;

		Stopwatch s = Stopwatch.StartNew();
		s.Start();

		fixed (int* p = array) {
			int rows = array.GetLength(0);
			int cols = array.GetLength(1);

			for (int i = 0; i < rows; i++)
				for (int j = 0; j < cols; j++) {
					int* ptr = p + (i * cols + j);
					Console.WriteLine(*ptr);
				}
		}

		s.Stop();

		Stopwatch s2 = Stopwatch.StartNew();
		s2.Start();

		for (int i = 0; i < array.GetLength(0); i++)
			for (int j = 0; j < array.GetLength(1); j++)
				Console.WriteLine(array[i,j]);
		s2.Stop();

		Console.WriteLine("ms1:"+s.ElapsedMilliseconds);
		Console.WriteLine("ms2:"+s2.ElapsedMilliseconds);
	}

	public void ProcessImageArea() {

	}
}

public class BitmapContainer {
	public PixelFormat Format { get; }

	public int Width { get; }

	public int Height { get; }

	public IntPtr Buffer { get; }

	public int Stride { get; set; }

	public BitmapContainer(Bitmap bitmap) {
		ArgumentNullException.ThrowIfNull(bitmap);
		Format = bitmap.PixelFormat;
		Width = bitmap.Width;
		Height = bitmap.Height;

		var bufferAndStride = Helper.ToBufferAndStride(bitmap);
		Buffer = bufferAndStride.Item1;
		Stride = bufferAndStride.Item2;
	}

	public Bitmap ToBitmap() {
		return new Bitmap(Width, Height, Stride, Format, Buffer);
	}

	private static unsafe void DrawTriangle(int* x, int* y, int* z) {
		
	}
}

static class Helper {

	public static Tuple<IntPtr, int> ToBufferAndStride(this Bitmap bitmap) {
		BitmapData? bitmapData = null;

		try {
			bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, bitmap.PixelFormat);

			return new Tuple<IntPtr, int>(bitmapData.Scan0, bitmapData.Stride);
		} finally {
			if (bitmapData != null)
				bitmap.UnlockBits(bitmapData);
		}
	}
}
static class BareboneTriangle {
	private static IWindow window;
	private static GL Gl;

	private static uint Vbo;
	private static uint Ebo;
	private static uint Vao;
	private static uint Shader;

	//Vertex shaders are run on each vertex.
	private static readonly string VertexShaderSource = @"
		#version 330 core //Using version GLSL version 3.3
		layout (location = 0) in vec4 vPos;

		void main() {
			gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
		}
	";

	//Fragment shaders are run on each fragment/pixel of the geometry.
	private static readonly string FragmentShaderSource = @"
		#version 330 core
		out vec4 FragColor;

		void main() {
			FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
		}
	";

	//Vertex data, uploaded to the VBO.
	private static readonly float[] Vertices = {
        //X    Y      Z
        0.5f,  0.5f, 0.0f,
		0.5f, -0.5f, 0.0f,
		-0.5f, -0.5f, 0.0f,
		-0.5f,  0.5f, 0.5f
		-0.5f,  1.0f, 0.1f
	};

	//Index data, uploaded to the EBO.
	private static readonly uint[] Indices = {
		0, 1, 3,
		1, 2, 3,
		2, 3, 4
	};


	public static void Main_() {
		var options = WindowOptions.Default;
		options.Size = new Vector2D<int>(800, 600);
		options.Title = "LearnOpenGL with Silk.NET";
		window = Window.Create(options);

		window.Load += OnLoad;
		window.Render += OnRender;
		window.Update += OnUpdate;
		window.FramebufferResize += OnFramebufferResize;
		window.Closing += OnClose;

		window.Run();

		window.Dispose();
	}

	private static unsafe void OnLoad() {
		IInputContext input = window.CreateInput();
		for (int i = 0; i < input.Keyboards.Count; i++) {
			input.Keyboards[i].KeyDown += KeyDown;
		}

		//Getting the opengl api for drawing to the screen.
		Gl = GL.GetApi(window);

		//Creating a vertex array.
		Vao = Gl.GenVertexArray();
		Gl.BindVertexArray(Vao);

		//Initializing a vertex buffer that holds the vertex data.
		Vbo = Gl.GenBuffer(); //Creating the buffer.
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo); //Binding the buffer.
		fixed (void* v = &Vertices[0]) {
			Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
		}

		//Initializing a element buffer that holds the index data.
		Ebo = Gl.GenBuffer(); //Creating the buffer.
		Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo); //Binding the buffer.
		fixed (void* i = &Indices[0]) {
			Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
		}

		//Creating a vertex shader.
		uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
		Gl.ShaderSource(vertexShader, VertexShaderSource);
		Gl.CompileShader(vertexShader);

		//Checking the shader for compilation errors.
		string infoLog = Gl.GetShaderInfoLog(vertexShader);
		if (!string.IsNullOrWhiteSpace(infoLog)) {
			Console.WriteLine($"Error compiling vertex shader {infoLog}");
		}

		//Creating a fragment shader.
		uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
		Gl.ShaderSource(fragmentShader, FragmentShaderSource);
		Gl.CompileShader(fragmentShader);

		//Checking the shader for compilation errors.
		infoLog = Gl.GetShaderInfoLog(fragmentShader);
		if (!string.IsNullOrWhiteSpace(infoLog)) {
			Console.WriteLine($"Error compiling fragment shader {infoLog}");
		}

		//Combining the shaders under one shader program.
		Shader = Gl.CreateProgram();
		Gl.AttachShader(Shader, vertexShader);
		Gl.AttachShader(Shader, fragmentShader);
		Gl.LinkProgram(Shader);

		//Checking the linking for errors.
		Gl.GetProgram(Shader, GLEnum.LinkStatus, out var status);
		if (status == 0) {
			Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(Shader)}");
		}

		//Delete the no longer useful individual shaders;
		Gl.DetachShader(Shader, vertexShader);
		Gl.DetachShader(Shader, fragmentShader);
		Gl.DeleteShader(vertexShader);
		Gl.DeleteShader(fragmentShader);

		//Tell opengl how to give the data to the shaders.
		Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
		Gl.EnableVertexAttribArray(0);
	}

	private static unsafe void OnRender(double obj) {//Method needs to be unsafe due to draw elements.
		//Clear the color channel.
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		//Bind the geometry and shader.
		Gl.BindVertexArray(Vao);
		Gl.UseProgram(Shader);

		//Draw the geometry.
		Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
	}

	private static void OnUpdate(double obj) {

	}

	private static void OnFramebufferResize(Vector2D<int> newSize) {
		Gl.Viewport(newSize);
	}

	private static void OnClose() {
		//Remember to delete the buffers.
		Gl.DeleteBuffer(Vbo);
		Gl.DeleteBuffer(Ebo);
		Gl.DeleteVertexArray(Vao);
		Gl.DeleteProgram(Shader);
	}

	private static void KeyDown(IKeyboard arg1, Key arg2, int arg3) {
		if (arg2 == Key.Escape) {
			window.Close();
		}
	}
}
