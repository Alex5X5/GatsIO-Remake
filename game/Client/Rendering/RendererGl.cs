namespace ShGame.game.Client.Rendering;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;


public class RendererGl {

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
		int colorTypeLocation = Gl.GetUniformLocation(shaderProgram, "u_colorMode");

		window.FramebufferResize += (Vector2D<int> size) => {
			Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
			// Also update uniforms for width and height if you're using them
			Gl.Uniform1(screenWidthLocation, (float)size.X);
			Gl.Uniform1(screenWidthLocation, (float)size.Y);
		};

		client.player.Setup(Gl);
		for(int i = 0; i<client.foreignPlayers.Length; i++)
			client.foreignPlayers[i].Setup(Gl);

		for(int i=0; i<client.obstacles.Length; i++) {
			client.obstacles[i].Setup(Gl);
			client.obstacles[i]?.shadow?.Setup(Gl);
		}
		for(int i = 0; i<client.obstacles.Length; i++)
			client.bullets[i].Setup(Gl);
	}

    public unsafe void OnRender(double _, IWindow window, Client client) {
        if (!loaded) return;
        //logger.Log("on render");
        Gl.ClearColor(0.5f, 0.5f, 0.6f, 1f);
        Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

        Gl.UseProgram(shaderProgram);

        int screenWidthLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowWidth");
        int screenHeightLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowHeight");
        int colorModeLocation = Gl.GetUniformLocation(shaderProgram, "colorMode");
        //Gl.Uniform1(screenWidthLocation, (float)window.Size.X);
        //Gl.Uniform1(screenHeightLocation, (float)window.Size.Y);
        Gl.Uniform1(screenWidthLocation, (float)Net.GameServer.MAP_WIDTH);
        Gl.Uniform1(screenHeightLocation, (float)Net.GameServer.MAP_HEIGHT);

        Gl.Uniform1(colorModeLocation, 1);
        client.player.Draw(Gl);
        for (int i = 0; i<client.foreignPlayers.Length; i++) {
            if (client.foreignPlayers[i]?.Health!=-1&&client.foreignPlayers[i]?.PlayerUUID!=client.player.PlayerUUID)
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
        for (int i = 0; i<client.obstacles.Length; i++)
            client.obstacles[i]?.Draw(Gl);

        Gl.Uniform1(colorModeLocation, 3);
        for (int i = 0; i<client.bullets.Length; i++) {
            client.bullets[i].dirty = true;
            client.bullets[i].Draw(Gl);
        }
    }

    public unsafe void OnClosing(IWindow window, Client client) {
		for (int i = 0; i<client.obstacles.Length; i++)
			client.obstacles[i].Dispose();
		for (int i = 0; i<client.foreignPlayers.Length; i++)
			client.foreignPlayers[i].Dispose();
		for (int i = 0; i<client.bullets.Length; i++)
			client.bullets[i].Dispose();
		client.player.Dispose();
	}

    private static uint CreateShaderProgram(IWindow window) {
        if (Gl==null)
            return 32767;
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
		aPos.x / (2000  / 4.0) - 1.0,
		aPos.y / (1200 / 4.0) - 1.0
	);
	gl_Position = vec4(ndc, aPos.z, 1.0);

}
";

        string fragmentShaderSource = @"
#version 330 core
#ifdef GL_ES
precision mediump float;
#endif

out vec4 FragColor;
uniform int colorMode;
uniform vec2 u_mouse;
uniform float u_time;
uniform float u_colorMode;
uniform float u_WindowWidth;
uniform float u_WindowHeight;

float function(vec2 point) {
    return ((sin(point.y*1.804-2.228)+sin(((point.x*7.168-1.476)/(0.948)))+((sin(8.920*point.x+8.160*point.y-
		9.224)+1.2*pow(sin(point.x*7.864+-1.512)+
		((sin(point.x*8.988+point.y*7.0-(sin(u_time/20.0)/2.0+0.5)*1.1))/(2.0)),2.0)+
		sin(point.x*3.104+5.124-(sin(u_time/20.0)/2.0+0.5)*14.220*point.y))/(2.400))+3.504)/(7.0));
}

void main()
{
	vec2 u_resolution = vec2(u_WindowWidth,u_WindowHeight);
    vec2 st = gl_FragCoord.xy/u_resolution.xy;
    st.x *= u_resolution.x/u_resolution.y;

	vec3 color;

	if (colorMode == 0)
		color = vec3(
			function(st*0.5)*0.28+0.3,
			function(st*0.5)*0.36+0.3,
			function(st*0.5)*0.48+0.3
		);
	else if (colorMode == 1)
		color = vec3(1.0, 0.0, 0.0); // Red
	else if (colorMode == 2)
		color = vec3(0.0, 0.0, 1.0); // Blue
	else if (colorMode == 3)
		color = vec3(0.0, 0.0, 0.0); // BLACK
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
			logger.Log($"Error compiling shader ({type}): {infoLog}");
		}

		return shader;
	}
}