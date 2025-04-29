namespace ShGame.game.Client.Rendering;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

class RendererGl {

    private readonly IWindow window;
    private GL gl;
    
	private static uint _shaderProgram;

	private static uint playerShaderProgram;
	private static uint shadowShaderProgram;
	private static uint obstackleShaderProgram;

    private static readonly Drawable[] dTriangles = [
		//new([0,0,0,10,0,0,0,10,0])
		//#new()
	];

	public RendererGl() {
		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
		options.Title = "OpenGL Drawable Triangles";

		window = Window.Create(options);
		window.Load += OnLoad;
		window.Run();
	}

	private void OnLoad() {
        // Initialize OpenGL context
        gl = GL.GetApi(window);
        window.Render += (double deltaTime) => OnRender(deltaTime, gl);

        // Create the shader program
        _shaderProgram = CreateShaderProgram();

        int screenWidthLocation = gl.GetUniformLocation(_shaderProgram, "u_WindowWidth");
        int screenHeightLocation = gl.GetUniformLocation(_shaderProgram, "u_WindowHeight");

        window.FramebufferResize += (Vector2D<int> size) => {
            gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
            // Also update uniforms for width and height if you're using them
            gl.Uniform1(screenWidthLocation, (float)size.X);
            gl.Uniform1(screenWidthLocation, (float)size.Y);
        };


        foreach (Drawable drawable in dTriangles) {
            drawable.Setup(gl);
        }
    }

	public void OnLoad(GL gl) {
		// Initialize OpenGL context
		gl = GL.GetApi(window);
		window.Render += (double deltaTime) => OnRender(deltaTime, gl);

		// Create the shader program
		_shaderProgram = CreateShaderProgram();

		foreach (Drawable drawable in dTriangles) {
			drawable.Setup(gl);
		}
	}

	public unsafe void OnRender(double deltaTime, GL gl) {
		gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		gl.UseProgram(_shaderProgram);

		int screenWidthLocation = gl.GetUniformLocation(_shaderProgram, "u_WindowWidth");
		int screenHeightLocation = gl.GetUniformLocation(_shaderProgram, "u_WindowHeight");
		gl.Uniform1(screenWidthLocation, (float)window.Size.X);
		gl.Uniform1(screenHeightLocation, (float)window.Size.Y);

		foreach (var triangle in dTriangles) {
			triangle.Draw(gl);
		}

		//i++;
		//if (i>10) {
		//	_window.Close();
		//	_window.Render-=OnRender;
		//}
	}

	private uint CreateShaderProgram() {
		
		uint shaderProgram = gl.CreateProgram();
		
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
        gl.UseProgram(shaderProgram);
		gl.AttachShader(shaderProgram, vertexShader);
		gl.AttachShader(shaderProgram, fragmentShader);
		gl.LinkProgram(shaderProgram);


        int windowWidthLocation = gl.GetUniformLocation(shaderProgram, "u_WindowWidth");
        int windowHeightLocation = gl.GetUniformLocation(shaderProgram, "u_WindowHeight");
        gl.Uniform1(windowWidthLocation, window.Size.X);
        gl.Uniform1(windowHeightLocation, window.Size.Y);

        //string s = gl.GetActiveUniform(shaderProgram, windowWidthLocation<0 ? 0 : (uint)windowWidthLocation, out int size, out UniformType type);
        //s += " "+gl.GetActiveUniform(shaderProgram, windowHeightLocation<0 ? 0 : (uint)windowWidthLocation, out int size2, out UniformType type2);
        //Console.WriteLine("activeUniforms:"+s);

        gl.DeleteShader(vertexShader);
        gl.DeleteShader(fragmentShader);

		return shaderProgram;
	}

	private uint CompileShader(ShaderType type, string source) {
		uint shader = gl.CreateShader(type);
		gl.ShaderSource(shader, source);
		gl.CompileShader(shader);

		gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
		if (status == 0) {
			string infoLog = gl.GetShaderInfoLog(shader);
			Console.WriteLine($"Error compiling shader ({type}): {infoLog}");
		}

		return shader;
	}
}