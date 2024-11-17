namespace ShGame.game.Client;

using ShGame.game.Net;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;

//#pragma warning disable CS8500 //a pointer is created to a variable of an unmanaged type

internal class RendererGl : IDisposable {

	#region fields

	private static readonly bool RESTRICTED_VIEW = true;

	public const int WIDTH = 1400, HEIGHT = 900;
	public const int gSclX = 1/WIDTH, gSclY = 1/HEIGHT;

	private readonly Brush GROUND_COLOR = new SolidBrush(Color.FromArgb(85, 85, 90));
	private readonly Brush SHADOW_COLOR = new SolidBrush(Color.FromArgb(0, 0, 0));
	private readonly Brush OBSTACLE_COLOR = new SolidBrush(Color.Gray);
	private readonly Brush PLAYER_RED_COLOR = new SolidBrush(Color.Red);

	public static readonly Line3d BORDER_TOP =
		Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(WIDTH, 0, 0));
	public static readonly Line3d BORDER_BOTTOM =
		Line3d.FromPoints(new Vector3d(0, HEIGHT, 0), new Vector3d(WIDTH, HEIGHT, 0));
	public static readonly Line3d BORDER_LEFT =
		Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(0, HEIGHT, 0));
	public static readonly Line3d BORDER_RIGHT =
		Line3d.FromPoints(new Vector3d(WIDTH, 0, 0), new Vector3d(WIDTH, HEIGHT, 0));

	private readonly Logger logger = new(new LoggingLevel("Renderer"));

	private static IWindow window;
	private static GL Gl;

	private static uint vertexBuffer;
	private static uint indexBuffer;
	private static uint VertexArray;
	private static uint Shader;

	//Vertex shaders are run on each vertex.
	private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec4 vPos;

        void main() {
            gl_Position = vec4(vPos.x-1, -vPos.y+1, vPos.z, 1.0);
        }";

	//Fragment shaders are run on each fragment/pixel of the geometry.
	private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main() {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }";

	//a rectangle is defined by four points which have three coordinates each.
	private const int FLOATS_PER_RECT = 12;
	//each shadow of an obstackle as well as the obstackle are drawn as a rectangle.
	private const int TOTAL_VERTEX_COUNT =
		2*FLOATS_PER_RECT*GameServer.OBSTACLE_COUNT;

	//Vertex data, uploaded to the verex buffer.
	private static readonly float[] Vertecies =
		new float[TOTAL_VERTEX_COUNT];

	//Index data, uploaded to the element buffer.
	private static readonly uint[] Indices =
		new uint[6*GameServer.OBSTACLE_COUNT*2];

	//the default vertecies for debugging.
	private static readonly float[] Vertecies_ = [
	  //X    Y      Z
		0f, 1f, 0.0f,
		0f, 0f, 0.0f,
		2f, 0f, 0.0f,
		1f, 1f, 0.0f
	];

	private static readonly float[] Vertecies2 = [
	  //X    Y      Z
		0f, 1f, 0.0f,
		0f, 0f, 0.0f,
		1f, 0f, 0.0f,
		1f, 1f, 0.0f
	];

	#endregion fields

	public unsafe RendererGl() {
		Vertecies.Initialize();
		for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++) {
			//the indices of two triangles that form a rectangle together
			Indices[i*6] = 1;
			Indices[i*6+1] = 2;
			Indices[i*6+2] = 3;
			Indices[i*6+3] = 1;
			Indices[i*6+4] = 4;
			Indices[i*6+5] = 3;
		}
		for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++)
			Vertecies_.CopyTo(Vertecies, i*FLOATS_PER_RECT);
		//float x = 0.5f;
		//float y = 0.2f;
		//float z = -0.6f;
		//UpdateShadow(&x, &z, &y, &x, &x, &z, &y, &x, 2);
		SetupWindow();
		//RenderableRect rect = new();
		//rect.BufferData();
		//rect.BindAndDraw();
	}

	private static void SetupWindow() {
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
		VertexArray = Gl.GenVertexArray();
		Gl.BindVertexArray(VertexArray);

		//Initializing a vertex buffer that holds the vertex data.
		vertexBuffer = Gl.GenBuffer(); //Creating the buffer.
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vertexBuffer); //Binding the buffer.
		fixed (void* v = &Vertecies[0]) {
			Gl.BufferData(
				BufferTargetARB.ArrayBuffer,
				TOTAL_VERTEX_COUNT * sizeof(uint),
				v,
				BufferUsageARB.StaticDraw
			); //Setting buffer data.
		}

		//Initializing a index buffer that holds the index data.
		indexBuffer = Gl.GenBuffer(); //Creating the buffer.
		Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, indexBuffer); //Binding the buffer.
		fixed (void* i = &Indices[0]) {
			Gl.BufferData(
				BufferTargetARB.ElementArrayBuffer,
				(nuint)(Indices.Length * sizeof(uint)),
				i,
				BufferUsageARB.StaticDraw
			); //Setting buffer data.
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

	private static unsafe void OnRender(double obj) { //Method needs to be unsafe due to draw elements.
		//Console.Write("vertecies\n");
		//for (int i = 0; i<Vertecies.Length; i++) {
		//	Console.Write("||"+Vertecies[i]);
		//	if ((i+1)%12==0&&i!=0)
		//		Console.WriteLine("");
		//}
		//Console.Write("\n\n");
		//Clear the color channel.
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		//Bind the geometry and shader.
		Gl.BindVertexArray(VertexArray);
		Gl.UseProgram(Shader);

		//Draw the geometry.
		Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
	}

	private static unsafe uint GetVertecies() {
		uint buffer = Gl.GenBuffer();
		return 0;
	} 

	private static unsafe void UpdateShadow(float* x1, float* y1, float* x2, float* y2, float* x3, float* y3, float* x4, float* y4, int pos) {
		Console.WriteLine("update Shadow: x1:"+*x1+" y1:"+*y1+" x2:"+*x2+" y2:"+*y2+" x3:"+*x3+" y3:"+*y3);
		new float[] {
            //X    Y      Z
			*x1, *y1, 0.0f,
			*x2, *y2, 0.0f,
			*x3, *y3, 0.0f,
			*x4, *y4, 0.0f,
		}.CopyTo(Vertecies, FLOATS_PER_RECT*2*pos);

		//Vbo = Gl.GenBuffer();
		//Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo); //Binding the buffer.
		//fixed (void* v = &Vertecies[0]) {
		//	Gl.BufferData(
		//		BufferTargetARB.ArrayBuffer,
		//		TOTAL_VERTEX_COUNT * sizeof(uint),
		//		v,
		//		BufferUsageARB.StaticDraw
		//	); //Setting buffer data.
		//}
		//Gl.BindVertexArray(Vao);
		//Gl.UseProgram(Shader);
		return;

		vaoHandle = Gl.GenVertexArray();
		Gl.BindVertexArray(vaoHandle);


		// Create a VertexBuffer
		vboHandle = Gl.GenBuffer();
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);


		// Set up attribs
		SetupVAO();


		// Clean up
		UnbindVAO();
		UnbindVBO();
		uint vbo_ = 0;

		uint ebo_ = Gl.GenBuffer(); //Creating the buffer.
		Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, indexBuffer); //Binding the buffer.
		fixed (void* i = &Indices[0]) {
			Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
		}

		//BindVAO();
		////Gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)size);
		//UnbindVAO();
	}

	private static unsafe void SetupVAO() {
		Gl.EnableVertexAttribArray(0);
		Gl.EnableVertexAttribArray(1);
		Gl.EnableVertexAttribArray(2);
		Gl.EnableVertexAttribArray(3);

		Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)3, null);
		//Gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)12);
		//Gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, vertexSize, (void*)24);
		//Gl.VertexAttribIPointer(3, 1, VertexAttribIType.Int, vertexSize, (void*)32);
	}

	#region window events
	private static void OnUpdate(double obj) {

	}

	private static void OnFramebufferResize(Vector2D<int> newSize) {
		Gl.Viewport(newSize);
	}

	private static void OnClose() {
		//Remember to delete the buffers.
		Gl.DeleteBuffer(vertexBuffer);
		Gl.DeleteBuffer(indexBuffer);
		Gl.DeleteVertexArray(VertexArray);
		Gl.DeleteProgram(Shader);
	}

	private static void KeyDown(IKeyboard arg1, Key arg2, int arg3) {
		if (arg2 == Key.Escape) {
			window.Close();
		}
	}

	#endregion window events

	// Shortcut functions

	static uint vaoHandle;
	static uint vboHandle;
	public static void BindVAO() => Gl.BindVertexArray(vaoHandle);
	public static void BindVBO() => Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);

	public static void UnbindVAO() => Gl.BindVertexArray(0);
	public static void UnbindVBO() => Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

	public void Dispose() {
		logger.Log("stoppping");
		PLAYER_RED_COLOR.Dispose();
		SHADOW_COLOR.Dispose();
		OBSTACLE_COLOR.Dispose();
		GROUND_COLOR.Dispose();
	}

	private class RenderableRect {
		private static float[] vertecies = [
		//  X  Y  Z
			0f,1f,0.0f,
			0f,0f,0.0f,
			2f,0f,0.0f,
			1f,1f,0.0f
		];

		private uint vao; //a pointer to the vertecies
		private uint vbo; //a pointer to the indices

		public RenderableRect() {
			vao = Gl.GenVertexArray();
			BindVAO();
			vbo = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
			UnbindVAO();
			UnbindVBO();
		}

		public unsafe void BufferData() {
			BindVBO();
			fixed (void* i = &vertecies[0])
				Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertecies.Length * sizeof(float)), i, BufferUsageARB.StaticDraw);
			UnbindVBO();
		}

		public unsafe void BindAndDraw() {
			BindVAO();
			Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
			UnbindVAO();
		}

		public void BindVAO() => Gl.BindVertexArray(vao);
		public void BindVBO() => Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		public static void UnbindVAO() => Gl.BindVertexArray(0);
		public static void UnbindVBO() => Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
	}

	public struct Triangle {
        private const string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec4 vPos;

        void main() {
            gl_Position = vec4(vPos.x, -vPos.y, vPos.z, 1.0);
        }";

        public const int VERTECIES_LENGHT = 9 * sizeof(float);
		public const int INDICES_LENGTH = 6 * sizeof(int);

		private readonly int[] indices = [1, 2, 3, 1, 4, 3];
		private float[] vertecies = new float[9];

        public Triangle() {
			vertecies.Initialize();
		}

		public unsafe void Update(float* x1, float* y1, float* x2, float* y2, float* x3, float* y3) {
			vertecies[0] = *x1;
			vertecies[1] = *y1;
			vertecies[2] = *x2;
			vertecies[2] = *y2;
			vertecies[4] = *x3;
			vertecies[5] = *y3;
		}

		public void OverrideArary(ref float[] array, int offset) {
			array[offset] = vertecies[0];
			array[offset + 1] = vertecies[1];
			array[offset + 2] = vertecies[2];
			array[offset + 3] = vertecies[3];
			array[offset + 4] = vertecies[4];
			array[offset + 5] = vertecies[5];
		}
	}
}