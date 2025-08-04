namespace ShGame.Client.Rendering;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Collections.Generic;
using ShGame.Client;
using SimpleLogging.logging;
using ShGame.Net;
using ShGame.Math;
using ShGame.Drawing;
using ShGame.Util;

public class RendererGl {

	//private readonly IWindow window;
	private static readonly Logger logger = new(new LoggingLevel("RendererGL"));

	private List<DebugDrawable> DebugDrawables = [];

	private double Time;
	private double LastFrame;

	private bool loaded = false;

	private  GL? _Gl;
	public GL? Gl {
		get => _Gl;
	}

	private static uint staticShaderProgram;
	private static uint textureShaderProgram;

	private static uint shadowTexture;
	private Dictionary<string, uint> textures;

	private static readonly uint playerShaderProgram;
	private static readonly uint shadowShaderProgram;
	private static readonly uint obstackleShaderProgram;


	public RendererGl() {
		textures = [];
	}

	public unsafe void OnLoad(IWindow window, Client client) {
		loaded = true;

		_Gl = GL.GetApi(window);

		staticShaderProgram = CreateShaderProgram(
			_Gl,
			window,
			ShaderSources.STATIC_VERTEXT_SHADER_SOURCE,
			ShaderSources.STATIC_FRAGMENT_SHADER_SOURCE
		);

		shadowTexture = TextureDrawable.CreateGlTexture(_Gl, Paths.AssetsPath("shadow.png"));

		textureShaderProgram = CreateShaderProgram(
			_Gl,
			window,
			ShaderSources.TEXTURE_VERTEX_SHADER_SOURCE,
			ShaderSources.TEXTURE_FRAGMENT_SHADER_SOURCE
		);

		window.FramebufferResize += (Vector2D<int> size) => {
			_Gl.UseProgram(staticShaderProgram);
			int screenWidthLocation = _Gl.GetUniformLocation(staticShaderProgram, "u_WindowWidth");
			int screenHeightLocation = _Gl.GetUniformLocation(staticShaderProgram, "u_WindowHeight");
			int colorTypeLocation = _Gl.GetUniformLocation(staticShaderProgram, "u_colorMode");
			_Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
			_Gl.Uniform1(screenWidthLocation, (float)size.X);
			_Gl.Uniform1(screenWidthLocation, (float)size.Y);
		};

		window.FramebufferResize += (Vector2D<int> size) => {
			_Gl.UseProgram(textureShaderProgram);
			int screenWidthLocation = _Gl.GetUniformLocation(textureShaderProgram, "u_WindowWidth");
			int screenHeightLocation = _Gl.GetUniformLocation(textureShaderProgram, "u_WindowHeight");
			_Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
			_Gl.Uniform1(screenWidthLocation, (float)size.X);
			_Gl.Uniform1(screenWidthLocation, (float)size.Y);
		};


		//client.ControlledPlayer.Setup(_Gl);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++)
			client.Game.Players[i].Setup(_Gl);

		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			client.Game.Obstacles[i].Setup(_Gl);
			client.Game.Obstacles[i]?.shadow?.Setup(_Gl);
		}
		for (int i = 0; i<Constants.BULLET_COUNT; i++)
			client.Game.Bullets[i].Setup(_Gl);
	}

	public unsafe void OnRender(double deltaTime, IWindow window, Client client) {
		//Time+=deltaTime;
		//if (Time-LastFrame>=1/GameServer.TARGET_TPS) {
		//	LastFrame = Time;

		//}
		if (!loaded)
			return;
		//logger.Log("on render");
		_Gl.ClearColor(0.5f, 0.5f, 0.6f, 1f);
		_Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		//if (textures.Keys.Contains(""))
		//	return;

		_Gl.UseProgram(staticShaderProgram);
		int colorModeLocation = _Gl.GetUniformLocation(staticShaderProgram, "colorMode");

		_Gl.Uniform1(colorModeLocation, 1);
		client.ControlledPlayer?.Draw(_Gl);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			if (client.ControlledPlayer!=null) {
				if (client.Game.Players[i].Health!=-1&&client.Game.Players[i].PlayerUUID!=client.ControlledPlayer.PlayerUUID)
					client.Game.Players[i].Draw(_Gl);
			} else {
			
			}
				client.Game.Players[i]?.Draw(_Gl);
		}
		_Gl.Uniform1(colorModeLocation, 1);

		_Gl.UseProgram(textureShaderProgram);
		_Gl.BindTexture(TextureTarget.Texture2D, shadowTexture);
		//Gl.Uniform1(colorModeLocation, 0);
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			if (client.Game.Obstacles[i]!=null && client.Game.Obstacles[i].shadow!=null) {
				client.Game.Obstacles[i].shadow.dirty = true;
				client.Game.Obstacles[i].shadow.Draw(_Gl);
			}
		}
		_Gl.BindTexture(TextureTarget.Texture2D, 0);
		_Gl.UseProgram(staticShaderProgram);
		colorModeLocation = _Gl.GetUniformLocation(staticShaderProgram, "colorMode");

		_Gl.Uniform1(colorModeLocation, 2);
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			//client.Game.Obstacles[i].dirty=true;
			client.Game.Obstacles[i]?.Draw(_Gl);
		}

		_Gl.Uniform1(colorModeLocation, 3);
		if (client.ControlledPlayer!=null) {
			client.ControlledPlayer.dirty=true;
			client.ControlledPlayer.Draw(_Gl);
		}
		for (int i = 0; i<Constants.BULLET_COUNT; i++) {
			client.Game.Bullets[i].dirty = true;
			client.Game.Bullets[i].Draw(_Gl);
		}

		_Gl.UseProgram(staticShaderProgram);
		colorModeLocation = _Gl.GetUniformLocation(staticShaderProgram, "colorMode");

		_Gl.Uniform1(colorModeLocation, 1);
		foreach (DebugDrawable drawable in DebugDrawables) {
			drawable.dirty = true;
			drawable.Draw(_Gl);
		}
	}

	private static uint CreateShaderProgram(GL gl, IWindow window, string vertexShaderSource, string fragmentShaderSource) {
		if (gl ==null)
			return 32767;
		uint shaderProgram = gl.CreateProgram();

		uint vertexShader = CompileShader(gl, ShaderType.VertexShader, vertexShaderSource);
		uint fragmentShader = CompileShader(gl, ShaderType.FragmentShader, fragmentShaderSource);


		gl.UseProgram(shaderProgram);
		gl.AttachShader(shaderProgram, vertexShader);
		gl.AttachShader(shaderProgram, fragmentShader);
		gl.LinkProgram(shaderProgram);


		int windowWidthLocation = gl.GetUniformLocation(shaderProgram, "u_WindowWidth");
		int windowHeightLocation = gl.GetUniformLocation(shaderProgram, "u_WindowHeight");
		int colorTypeLocation = gl.GetUniformLocation(shaderProgram, "u_ColorType");
		gl.Uniform1(windowWidthLocation, window.Size.X);
		gl.Uniform1(windowHeightLocation, window.Size.Y);
		gl.Uniform1(colorTypeLocation, 1);

		gl.DeleteShader(vertexShader);
		gl.DeleteShader(fragmentShader);

		//string s = Gl.GetActiveUniform(shaderProgram, windowWidthLocation<0 ? 0 : (uint)windowWidthLocation, out int size, out UniformType type);
		//s += " "+Gl.GetActiveUniform(shaderProgram, windowHeightLocation<0 ? 0 : (uint)windowWidthLocation, out int size2, out UniformType type2);
		//Console.WriteLine("activeUniforms:"+s);

		return shaderProgram;
	}

	private static uint CompileShader(GL gl, ShaderType type, string source) {
		uint shader = gl.CreateShader(type);
		gl.ShaderSource(shader, source);
		gl.CompileShader(shader);

		gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
		if (status == 0) {
			string infoLog = gl.GetShaderInfoLog(shader);
			logger.Log($"Error compiling shader ({type}): {infoLog}");
		}

		return shader;
	}
}