namespace ShGame.Drawing;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

internal class ShaderProgram {

	private readonly IWindow window;
	private readonly GL Gl;

	public readonly uint vaoHandle, vboHandle;
	private readonly uint programm;

	public uint ProgramHandle {
		get => programm;
	}

	public ShaderProgram(GL gl, IWindow window, string vertexShaderSource, string fragmentShaderSource) {
		this.window = window;
		Gl = gl;

		programm = Gl.CreateProgram();

		uint vertexShader = CompileShader(Gl, ShaderType.VertexShader, vertexShaderSource);
		uint fragmentShader = CompileShader(Gl, ShaderType.FragmentShader, fragmentShaderSource);

		Gl.AttachShader(programm, vertexShader);
		Gl.AttachShader(programm, fragmentShader);
		Gl.LinkProgram(programm);

		PrintLinkingErrors(Gl);

		Gl.UseProgram(programm);

		vaoHandle = Gl.GenVertexArray();
		vboHandle = Gl.GenBuffer();

		PrintUnifom1Info("u_WindowWidth");
		PrintUnifom1Info("u_WindowHeight");

		OnFrameBufferSizeChanged(window.FramebufferSize);

		Gl.DeleteShader(vertexShader);
		Gl.DeleteShader(fragmentShader);
	}

	private static uint CompileShader(GL gl, ShaderType type, string source) {
		uint shader = gl.CreateShader(type);
		gl.ShaderSource(shader, source);
		gl.CompileShader(shader);

		gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
		if (status == 0) {
			string infoLog = gl.GetShaderInfoLog(shader);
			Console.Error.WriteLine($"Error compiling shader ({type}): {infoLog}");
		}

		return shader;
	}

	private void PrintLinkingErrors(GL Gl) {
		Gl.GetProgram(programm, ProgramPropertyARB.LinkStatus, out int linkStatus);
		if (linkStatus == 0) {
			string infoLog = Gl.GetProgramInfoLog(programm);
			Console.Error.WriteLine($"Error linking program: {infoLog}");
		}
	}

	private void PrintUnifom1Info(string uniform) {
		int location = Gl.GetUniformLocation(programm, uniform);
		if (location < 0) {
			Console.WriteLine($"{uniform}:null");
		} else {
			var val = Gl.GetUniform(programm, location);
			Console.WriteLine($"{uniform}:{val}");
		}
	}

	private void SetFloatUniform(string uniform, float value) {
		int location = Gl.GetUniformLocation(programm, uniform);
		if (location < 0)
			throw new InvalidOperationException($"""Unable to set uniform "{uniform}": not found""");
		Gl.Uniform1(location, value);
	}

	private void SetIntUniform(string uniform, int value) {
		int location = Gl.GetUniformLocation(programm, uniform);
		if (location < 0)
			throw new InvalidOperationException($"""Unable to set uniform "{uniform}": not found""");
		Gl.Uniform1(location, value);
	}

	public void UseFor(GL Gl) {
		Gl.UseProgram(programm);
	}

	public void OnFrameBufferSizeChanged(Vector2D<int> size) {
		Gl.UseProgram(programm);
		SetFloatUniform("u_WindowWidth", size.X);
		SetFloatUniform("u_WindowHeight", size.Y);
	}
}
