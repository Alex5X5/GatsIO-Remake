namespace ShGame.game.Client.Rendering;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

class RendererGl {

	//private readonly IWindow window;
	private static readonly Logger logger = new(new LoggingLevel("RendererGL"));

    public const int WIDTH = 2500, HEIGHT = 1500;


    private bool loaded = false;

	public static GL? Gl;
	
	private static uint _shaderProgram;

	private static uint playerShaderProgram;
	private static uint shadowShaderProgram;
	private static uint obstackleShaderProgram;

	private static readonly Drawable[] dTriangles = [
		//new([0,0,0,10,0,0,0,10,0])
		//#new()
	];

	private Obstacle2 testObstacle;

	public RendererGl() {
	}

	public void OnLoad(IWindow window, Client2 client) {
		loaded = true;

		// Initialize OpenGL context
		Gl = GL.GetApi(window);
		//window.Render += (double deltaTime) => OnRender(deltaTime);

		// Create the shader program
		_shaderProgram = CreateShaderProgram(window);

		int screenWidthLocation = Gl.GetUniformLocation(_shaderProgram, "u_WindowWidth");
		int screenHeightLocation = Gl.GetUniformLocation(_shaderProgram, "u_WindowHeight");

		window.FramebufferResize += (Vector2D<int> size) => {
			Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
			// Also update uniforms for width and height if you're using them
			Gl.Uniform1(screenWidthLocation, (float)size.X);
			Gl.Uniform1(screenWidthLocation, (float)size.Y);
		};

        for (int i = 0; i<client.foreignPlayers.Length; i++) {
            client.foreignPlayers[i].Setup(Gl);
        }
        for(int i=0; i<client.obstacles.Length; i++) {
            client.obstacles[i].Setup(Gl);
        }
        client.player.Setup(Gl);
	}

	public unsafe void OnRender(double deltaTime, IWindow window, Client2 client) {
		if(!loaded) return;

		//logger.Log("on render");
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		Gl.UseProgram(_shaderProgram);

		int screenWidthLocation = Gl.GetUniformLocation(_shaderProgram, "u_WindowWidth");
		int screenHeightLocation = Gl.GetUniformLocation(_shaderProgram, "u_WindowHeight");
		Gl.Uniform1(screenWidthLocation, (float)window.Size.X);
		Gl.Uniform1(screenHeightLocation, (float)window.Size.Y);

        client.player.Draw(Gl);
        for (int i = 0; i<client.foreignPlayers.Length; i++) {
			if(client.foreignPlayers[i]?.Health!=-1&&client.foreignPlayers[i]?.PlayerUUID!=client.player.PlayerUUID)
				client.foreignPlayers[i]?.Draw(Gl);
        }
        for (int i = 0; i<client.obstacles.Length; i++) {
            client.obstacles[i]?.Draw(Gl);
        }
	}

	private uint CreateShaderProgram(IWindow window) {
		
		uint shaderProgram = Gl.CreateProgram();
		
		// Vertex Shader
		string vertexShaderSource = @"
			#version 330 core

			layout(location = 0) in vec3 aPos;
			uniform float u_WindowWidth;
			uniform float u_WindowHeight;
			void main()
			{
				vec2 ndc = vec2(
					aPos.x / (u_WindowWidth  / 4.0) - 1.0,
					aPos.y / (u_WindowHeight / 4.0) - 1.0
				);
				gl_Position = vec4(ndc, aPos.z, 1.0);

			}
		";
		
		string fragmentShaderSource = @"
			#version 330 core
			in vec3 vertexColor;
			out vec4 FragColor;
			void main()
			{
			   FragColor = vec4(0.5, 0.5, 0.5, 1.0f);
			}
		";
		
		uint vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
		uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);


		// Link shaders into a program
		Gl.UseProgram(shaderProgram);
		Gl.AttachShader(shaderProgram, vertexShader);
		Gl.AttachShader(shaderProgram, fragmentShader);
		Gl.LinkProgram(shaderProgram);


		int windowWidthLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowWidth");
		int windowHeightLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowHeight");
		Gl.Uniform1(windowWidthLocation, window.Size.X);
		Gl.Uniform1(windowHeightLocation, window.Size.Y);

		//string s = gl.GetActiveUniform(shaderProgram, windowWidthLocation<0 ? 0 : (uint)windowWidthLocation, out int size, out UniformType type);
		//s += " "+gl.GetActiveUniform(shaderProgram, windowHeightLocation<0 ? 0 : (uint)windowWidthLocation, out int size2, out UniformType type2);
		//Console.WriteLine("activeUniforms:"+s);

		Gl.DeleteShader(vertexShader);
		Gl.DeleteShader(fragmentShader);

		return shaderProgram;
	}

	private uint CompileShader(ShaderType type, string source) {
		uint shader = Gl.CreateShader(type);
		Gl.ShaderSource(shader, source);
		Gl.CompileShader(shader);

		Gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
		if (status == 0) {
			string infoLog = Gl.GetShaderInfoLog(shader);
			Console.WriteLine($"Error compiling shader ({type}): {infoLog}");
		}

		return shader;
	}
}