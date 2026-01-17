namespace ShGame.Client.Rendering;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Collections.Generic;
using SimpleLogging.logging;
using ShGame.Drawing.Models;
using ShGame.Drawing.Services;

public class RenderService {

	private IWindow? window;
	private static readonly Logger logger = new(new LoggingLevel("RenderService"));

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


	public RenderService() {
		window.Render += (double deltaTime)=>OnRender(deltaTime, window);
		window.Load += OnLoad;
        textures = [];
	}

    protected virtual unsafe void OnLoad() {
		loaded = true;

		_Gl = GL.GetApi(window);

		staticShaderProgram = CreateShaderProgram(
			_Gl,
			window,
			ShaderSources.STATIC_VERTEXT_SHADER_SOURCE,
			ShaderSources.STATIC_FRAGMENT_SHADER_SOURCE
		);

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
	}

	protected unsafe void OnRender(double deltaTime, IWindow window) {
		
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

	public void SetVisible(WindowOptions options) {

		logger.Log("setting vivible");

		window = Window.Create(options);
		window.Load +=
			() => OnLoad(window);

		window.Run();

		return;
	}
}