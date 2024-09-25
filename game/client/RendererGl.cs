namespace ShGame.game.Client;

using ShGame.game.Net;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;
using System.Reflection;

//#pragma warning disable CS8500 //a pointer is created to a variable of an unmanaged type

internal class RendererGl:IDisposable {

	#region fields

	private static readonly bool RESTRICTED_VIEW = true;

	public const int WIDTH = 1400, HEIGHT = 900;

	private readonly Brush GROUND_COLOR = new SolidBrush(Color.FromArgb(85, 85, 90));
	private readonly Brush SHADOW_COLOR = new SolidBrush(Color.FromArgb(0, 0, 0));
	private readonly Brush OBSTACLE_COLOR = new SolidBrush(Color.Gray);
	private readonly Brush PLAYER_RED_COLOR = new SolidBrush(Color.Red);

	public static readonly Line3d BORDER_TOP = Line3d.FromPoints(new Vector3d(0,0,0),new Vector3d(WIDTH,0,0));
	public static readonly Line3d BORDER_BOTTOM = Line3d.FromPoints(new Vector3d(0, HEIGHT, 0), new Vector3d(WIDTH, HEIGHT, 0));
	public static readonly Line3d BORDER_LEFT = Line3d.FromPoints(new Vector3d(0,0,0), new Vector3d(0, HEIGHT,0));
	public static readonly Line3d BORDER_RIGHT = Line3d.FromPoints(new Vector3d(WIDTH, 0,0), new Vector3d(WIDTH, HEIGHT,0));

	private readonly Logger logger = new(new LoggingLevel("Renderer"));

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
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

	//Fragment shaders are run on each fragment/pixel of the geometry.
	private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";

	//Index data, uploaded to the EBO.
	private static readonly uint[] Indices = new uint[6*GameServer.OBSTACLE_COUNT];

	//Vertex data, uploaded to the VBO.
	private static readonly float[] Vertecies = new float[12*GameServer.OBSTACLE_COUNT];

	private static readonly float[] Vertecies_ = [
		//X    Y      Z
		0.5f,  0.5f, 0.0f,
		0.5f, -0.5f, 0.0f,
		-0.5f, -0.5f, 0.0f,
		-0.5f,  0.5f, 0.0f
	];

	#endregion fields

	public unsafe RendererGl() {
		Vertecies.Initialize();
		for(int i = 0; i<GameServer.OBSTACLE_COUNT; i++){
			Indices[i*6+1] = 1;
			Indices[i*6+2] = 3;
			Indices[i*6+3] = 1;
			Indices[i*6+4] = 2;
			Indices[i*6+5] = 3;
		}
		for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++)
			Vertecies_.CopyTo(Vertecies, i*12);
		float x = 0f;
		float y = 1f;
		float z = -1f;
		UpdateShadow(&x, &z, &y, &x, &x, &z, &y, &x, 1);
		SetupWindow();
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
		Vao = Gl.GenVertexArray();
		Gl.BindVertexArray(Vao);

		//Initializing a vertex buffer that holds the vertex data.
		Vbo = Gl.GenBuffer(); //Creating the buffer.
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo); //Binding the buffer.
		fixed (void* v = &Vertecies[0]) {
			Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertecies.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
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
		if (!string.IsNullOrWhiteSpace(infoLog))
		{
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
		Console.Write("vertecies");
		for (int i=0; i<Vertecies.Length; i++)
			Console.Write(" "+Vertecies[i]);
		Console.Write("\n\n");
		//Clear the color channel.
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		//Bind the geometry and shader.
		Gl.BindVertexArray(Vao);
		Gl.UseProgram(Shader);

		//float x1 = 1, y1 = 0, x2 = 1, y2 = 1, x3 = 0, y3 = -1;
		//DrawTriangle(&x1, &y1, &x2, & y2, &x3, &y3);
		//Draw the geometry.
		Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
	}

	private static unsafe void UpdateShadow(float* x1, float* y1, float* x2, float* y2, float* x3, float* y3, float* x4, float* y4, int pos) {
		Console.WriteLine("uodate Shadow: x1:"+*x1+" y1:"+*y1+" x2:"+*x2+" y2:"+*y2+" x3:"+*x3+" y3:"+*y3);
		new float[] {
            //X    Y      Z
			*x1, *y1, 0.0f,
			*x2, *y2, 0.0f,
			*x3, *y3, 0.0f,
			*x4, *y4, 0.0f,
		}.CopyTo(Vertecies, 12*pos);

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

		uint ebo_= Gl.GenBuffer(); //Creating the buffer.
		Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo); //Binding the buffer.
		fixed (void* i = &Indices[0]) {
			Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
		}

		BindVAO();
		//Gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)size);
		UnbindVAO();
	}

	private static unsafe void SetupVAO(){
		Gl.EnableVertexAttribArray(0);
		Gl.EnableVertexAttribArray(1);
		Gl.EnableVertexAttribArray(2);
		Gl.EnableVertexAttribArray(3);

		Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)3, null);
		//Gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)12);
		//Gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, vertexSize, (void*)24);
		//Gl.VertexAttribIPointer(3, 1, VertexAttribIType.Int, vertexSize, (void*)32);
	}

	private static void OnUpdate(double obj) {

	}

	private static void OnFramebufferResize(Vector2D<int> newSize)
	{
		Gl.Viewport(newSize);
	}

	private static void OnClose()
	{
		//Remember to delete the buffers.
		Gl.DeleteBuffer(Vbo);
		Gl.DeleteBuffer(Ebo);
		Gl.DeleteVertexArray(Vao);
		Gl.DeleteProgram(Shader);
	}

	private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
	{
		if (arg2 == Key.Escape)
		{
			window.Close();
		}
	}

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
}
