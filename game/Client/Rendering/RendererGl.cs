namespace ShGame.Game.Client.Rendering;

using ShGame.Game.Net;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using ShGame.Game.Util;
using System.Collections.Generic;

public class RendererGl {

	//private readonly IWindow window;
	private static readonly Logger logger = new(new LoggingLevel("RendererGL"));

	public static readonly Line3d BORDER_TOP = Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(GameServer.MAP_WIDTH, 0, 0));
	public static readonly Line3d BORDER_BOTTOM = Line3d.FromPoints(new Vector3d(0, GameServer.MAP_HEIGHT, 0), new Vector3d(GameServer.MAP_WIDTH, GameServer.MAP_HEIGHT, 0));
	public static readonly Line3d BORDER_LEFT = Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(0, GameServer.MAP_HEIGHT, 0));
	public static readonly Line3d BORDER_RIGHT = Line3d.FromPoints(new Vector3d(GameServer.MAP_WIDTH, 0, 0), new Vector3d(GameServer.MAP_WIDTH, GameServer.MAP_HEIGHT, 0));

	private List<DebugDrawable> DebugDrawables = new();

	private double Time;
	private double LastFrame;

	private bool loaded = false;

	public static GL? Gl;

	private static uint staticShaderProgram;
	private static uint textureShaderProgram;

	private static uint shadowTexture;
	private Dictionary<string, uint> textures;

	private static readonly uint playerShaderProgram;
	private static readonly uint shadowShaderProgram;
	private static readonly uint obstackleShaderProgram;


	public RendererGl() {
	}

	public unsafe void OnLoad(IWindow window, Client client) {
		loaded = true;

		Gl = GL.GetApi(window);

		staticShaderProgram = CreateShaderProgram(
			window,
			ShaderSources.STATIC_VERTEXT_SHADER_SOURCE,
			ShaderSources.STATIC_FRAGMENT_SHADER_SOURCE
		);

		shadowTexture = TextureDrawable.CreateGlTexture(Gl, Paths.AssetsPath("shadow.png"));

		textureShaderProgram = CreateShaderProgram(
			window,
			ShaderSources.TEXTURE_VERTEX_SHADER_SOURCE,
			ShaderSources.TEXTURE_FRAGMENT_SHADER_SOURCE
		);

		window.FramebufferResize += (Vector2D<int> size) => {
			Gl.UseProgram(staticShaderProgram);
			int screenWidthLocation = Gl.GetUniformLocation(staticShaderProgram, "u_WindowWidth");
			int screenHeightLocation = Gl.GetUniformLocation(staticShaderProgram, "u_WindowHeight");
			int colorTypeLocation = Gl.GetUniformLocation(staticShaderProgram, "u_colorMode");
			Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
			Gl.Uniform1(screenWidthLocation, (float)size.X);
			Gl.Uniform1(screenWidthLocation, (float)size.Y);
		};

		window.FramebufferResize += (Vector2D<int> size) => {
			Gl.UseProgram(textureShaderProgram);
			int screenWidthLocation = Gl.GetUniformLocation(textureShaderProgram, "u_WindowWidth");
			int screenHeightLocation = Gl.GetUniformLocation(textureShaderProgram, "u_WindowHeight");
			Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
			// Also update uniforms for width and height if you're using them
			Gl.Uniform1(screenWidthLocation, (float)size.X);
			Gl.Uniform1(screenWidthLocation, (float)size.Y);
		};


		client.player.Setup(Gl);
		for (int i = 0; i<client.foreignPlayers.Length; i++)
			client.foreignPlayers[i].Setup(Gl);

		for (int i = 0; i<client.obstacles.Length; i++) {
			client.obstacles[i].Setup(Gl);
			client.obstacles[i]?.shadow?.Setup(Gl);
		}
		for (int i = 0; i<client.obstacles.Length; i++)
			client.bullets[i].Setup(Gl);
	}

	public unsafe void OnRender(double deltaTime, IWindow window, Client client) {
		//Time+=deltaTime;
		//if (Time-LastFrame>=1/GameServer.TARGET_TPS) {
		//	LastFrame = Time;

		//}
		if (!loaded)
			return;
		//logger.Log("on render");
		Gl.ClearColor(0.5f, 0.5f, 0.6f, 1f);
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		//if (textures.Keys.Contains(""))
		//	return;

		Gl.UseProgram(staticShaderProgram);
		int colorModeLocation = Gl.GetUniformLocation(staticShaderProgram, "colorMode");

		Gl.Uniform1(colorModeLocation, 1);
		client.player.Draw(Gl);
		for (int i = 0; i<client.foreignPlayers.Length; i++) {
			if (client.foreignPlayers[i]?.Health!=-1&&client.foreignPlayers[i]?.PlayerUUID!=client.player.PlayerUUID)
				client.foreignPlayers[i]?.Draw(Gl);
		}

		Gl.UseProgram(textureShaderProgram);
		Gl.BindTexture(TextureTarget.Texture2D, shadowTexture);
		//Gl.Uniform1(colorModeLocation, 0);
		for (int i = 0; i<client.obstacles.Length; i++) {
			if (client.obstacles[i]!=null && client.obstacles[i].shadow!=null) {
				client.obstacles[i].shadow.dirty = true;
				client.obstacles[i].shadow.Draw(Gl);
			}
		}
		Gl.BindTexture(TextureTarget.Texture2D, 0);
		Gl.UseProgram(staticShaderProgram);
		colorModeLocation = Gl.GetUniformLocation(staticShaderProgram, "colorMode");

		Gl.Uniform1(colorModeLocation, 2);
		for (int i = 0; i<client.obstacles.Length; i++) {
			client.obstacles[i].dirty=true;
			client.obstacles[i]?.Draw(Gl);
		}

		Gl.Uniform1(colorModeLocation, 3);
		for (int i = 0; i<client.bullets.Length; i++) {
			client.bullets[i].dirty = true;
			client.bullets[i].Draw(Gl);
		}

		Gl.UseProgram(staticShaderProgram);
		colorModeLocation = Gl.GetUniformLocation(staticShaderProgram, "colorMode");

		Gl.Uniform1(colorModeLocation, 1);
		foreach (DebugDrawable drawable in DebugDrawables) {
			drawable.dirty = true;
			drawable.Draw(Gl);
		}
	}

	private static uint CreateShaderProgram(IWindow window, string vertexShaderSource, string fragmentShaderSource) {
		if (Gl==null)
			return 32767;
		uint shaderProgram = Gl.CreateProgram();

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

		Gl.DeleteShader(vertexShader);
		Gl.DeleteShader(fragmentShader);

		//string s = Gl.GetActiveUniform(shaderProgram, windowWidthLocation<0 ? 0 : (uint)windowWidthLocation, out int size, out UniformType type);
		//s += " "+Gl.GetActiveUniform(shaderProgram, windowHeightLocation<0 ? 0 : (uint)windowWidthLocation, out int size2, out UniformType type2);
		//Console.WriteLine("activeUniforms:"+s);

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