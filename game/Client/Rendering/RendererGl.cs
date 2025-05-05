namespace ShGame.game.Client.Rendering;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;


class RendererGl {

	//private readonly IWindow window;
	private static readonly Logger logger = new(new LoggingLevel("RendererGL"));

	public const int WIDTH = 2500, HEIGHT = 1500;

    public static readonly Line3d BORDER_TOP = Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(WIDTH, 0, 0));
    public static readonly Line3d BORDER_BOTTOM = Line3d.FromPoints(new Vector3d(0, HEIGHT, 0), new Vector3d(WIDTH, HEIGHT, 0));
    public static readonly Line3d BORDER_LEFT = Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(0, HEIGHT, 0));
    public static readonly Line3d BORDER_RIGHT = Line3d.FromPoints(new Vector3d(WIDTH, 0, 0), new Vector3d(WIDTH, HEIGHT, 0));


    private bool loaded = false;

	public static GL? Gl;
	
	private static uint shaderProgram;

	private static readonly uint playerShaderProgram;
	private static readonly uint shadowShaderProgram;
	private static readonly uint obstackleShaderProgram;

	public RendererGl() {
	}

	public void OnLoad(IWindow window, Client client) {
		loaded = true;

		Gl = GL.GetApi(window);

		shaderProgram = CreateShaderProgram(window);

		int screenWidthLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowWidth");
		int screenHeightLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowHeight");
		int colorTypeLocation = Gl.GetUniformLocation(shaderProgram, "colorMode");

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
        for (int i = 0; i<client.obstacles.Length; i++) {
            if (client.obstacles[i]!=null && client.obstacles[i].shadow!=null) {
                client.obstacles[i].shadow.Setup(Gl);
            }
        }
    }

    public unsafe void OnRender(double _, IWindow window, Client client) {
		if(!loaded) return;
		//logger.Log("on render");
		Gl.ClearColor(0.5f, 0.5f, 0.6f, 1f);
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		Gl.UseProgram(shaderProgram);

		int screenWidthLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowWidth");
		int screenHeightLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowHeight");
		int colorModeLocation = Gl.GetUniformLocation(shaderProgram, "colorMode");
		Gl.Uniform1(screenWidthLocation, (float)window.Size.X);
		Gl.Uniform1(screenHeightLocation, (float)window.Size.Y);
		
		Gl.Uniform1(colorModeLocation, 1);
		client.player.Draw(Gl);
		for (int i = 0; i<client.foreignPlayers.Length; i++) {
			if(client.foreignPlayers[i]?.Health!=-1&&client.foreignPlayers[i]?.PlayerUUID!=client.player.PlayerUUID)
				client.foreignPlayers[i]?.Draw(Gl);
		}

        Gl.Uniform1(colorModeLocation, 0);
        for (int i = 0; i<client.obstacles.Length; i++) {
			if (client.obstacles[i]!=null && client.obstacles[i].shadow!=null) {
				client.obstacles[i].shadow.dirty = true;
				client.obstacles[i].shadow.Draw(Gl);
			}
        }

		Gl.Uniform1(colorModeLocation, 2);
		for (int i = 0; i<client.obstacles.Length; i++) {
			client.obstacles[i]?.Draw(Gl);
		}

    }

	private static uint CreateShaderProgram(IWindow window) {
		
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
out vec4 FragColor;

uniform int colorMode;

void main()
{
	vec3 color;

	if (colorMode == 0)
		color = vec3(0.4, 0.4, 0.5); // Green
	else if (colorMode == 1)
		color = vec3(1.0, 0.0, 0.0); // Red
	else if (colorMode == 2)
		color = vec3(0.0, 0.0, 1.0); // Blue
	else
		color = vec3(1.0); // Default white

	FragColor = vec4(color, 1.0);
}
		";
		
		uint vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
		uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);


		Gl.UseProgram(shaderProgram);
		Gl.AttachShader(shaderProgram, vertexShader);
		Gl.AttachShader(shaderProgram, fragmentShader);
		Gl.LinkProgram(shaderProgram);


		int windowWidthLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowWidth");
		int windowHeightLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowHeight");
		int colorTypeLocation = Gl.GetUniformLocation(shaderProgram, "u_ColorType");
		Gl.Uniform1(windowWidthLocation, window.Size.X);
		Gl.Uniform1(windowHeightLocation, window.Size.Y);
		Gl.Uniform1(colorTypeLocation, 1);

		//string s = gl.GetActiveUniform(shaderProgram, windowWidthLocation<0 ? 0 : (uint)windowWidthLocation, out int size, out UniformType type);
		//s += " "+gl.GetActiveUniform(shaderProgram, windowHeightLocation<0 ? 0 : (uint)windowWidthLocation, out int size2, out UniformType type2);
		//Console.WriteLine("activeUniforms:"+s);

		Gl.DeleteShader(vertexShader);
		Gl.DeleteShader(fragmentShader);

		return shaderProgram;
	}

	private static uint CompileShader(ShaderType type, string source) {
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