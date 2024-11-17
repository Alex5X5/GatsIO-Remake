using Silk.NET.OpenGL;
using Silk.NET.Windowing;

class Programm3 {
	private static IWindow _window;
	private static GL _gl;
	private static uint _vertexArray;
	private static uint _vertexBuffer;
	private static uint _shaderProgram;

	// Array of DrawableTriangle objects
	private static DrawableTriangle[] dTriangles = new DrawableTriangle[] {
		new DrawableTriangle(
			[
				0.0f, 0.5f, 0.0f,   // Top vertex of triangle 1
				-0.5f, -0.5f, 0.0f, // Bottom-left vertex of triangle 1
				0.5f, -0.5f, 0.0f   // Bottom-right vertex of triangle 1
			]
		),
		new DrawableTriangle(
			[
				-0.8f, 0.0f, 0.0f,  // Top vertex of triangle 2
				-0.9f, -0.8f, 0.0f, // Bottom-left vertex of triangle 2
				-0.6f, -0.8f, 0.0f  // Bottom-right vertex of triangle 2
			]
		)
	};



	public static void Main_() {
		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
		options.Title = "OpenGL Drawable Triangles";

		_window = Window.Create(options);
		_window.Load += OnLoad;
		_window.Render += OnRender;
		_window.Run();
	}

	private static void OnLoad() {
		// Initialize OpenGL context
		_gl = GL.GetApi(_window);

		// Create the shader program
		_shaderProgram = CreateShaderProgram();

		// Generate the Vertex Array Object and Buffer Object
		_vertexArray = _gl.GenVertexArray();
		_gl.BindVertexArray(_vertexArray);

		_vertexBuffer = _gl.GenBuffer();
		_gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);

		// Allocate buffer for the largest possible triangle (9 floats per triangle)
		_gl.BufferData(BufferTargetARB.ArrayBuffer, 9 * sizeof(float), in IntPtr.Zero, BufferUsageARB.DynamicDraw);

		// Specify the layout of the vertex data
		_gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
		_gl.EnableVertexAttribArray(0);

		// Unbind the VAO
		_gl.BindVertexArray(0);
	}

	private static unsafe void OnRender(double deltaTime) {
		// Clear the screen with black
		_gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		// Use the shader program
		_gl.UseProgram(_shaderProgram);

		// Bind the Vertex Array Object
		_gl.BindVertexArray(_vertexArray);

		// Loop through the array of DrawableTriangles and draw each one
		foreach (var triangle in dTriangles) {
			// Upload the vertices from the current DrawableTriangle to the buffer
			_gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
			fixed(void* ptr = triangle.Vertices)
				_gl.BufferSubData((Silk.NET.OpenGL.GLEnum)BufferTargetARB.ArrayBuffer, 0, 9*sizeof(float), ptr);

			// Draw the triangle
			_gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
		}

		// Unbind the VAO
		_gl.BindVertexArray(0);
	}

	private static uint CreateShaderProgram() {
		// Vertex Shader
		string vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
            }
        ";
		uint vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);

		// Fragment Shader
		string fragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;
            void main()
            {
                FragColor = vec4(0.5, 0.5, 0.5, 1.0);  // Gray color
            }
        ";
		uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

		// Link shaders into a program
		uint shaderProgram = _gl.CreateProgram();
		_gl.AttachShader(shaderProgram, vertexShader);
		_gl.AttachShader(shaderProgram, fragmentShader);
		_gl.LinkProgram(shaderProgram);

		// Clean up the shaders (they are no longer needed after linking)
		_gl.DeleteShader(vertexShader);
		_gl.DeleteShader(fragmentShader);

		return shaderProgram;
	}

	private static uint CompileShader(ShaderType type, string source) {
		uint shader = _gl.CreateShader(type);
		_gl.ShaderSource(shader, source);
		_gl.CompileShader(shader);

		// Check for compilation errors
		_gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
		if (status == 0) {
			string infoLog = _gl.GetShaderInfoLog(shader);
			Console.WriteLine($"Error compiling shader ({type}): {infoLog}");
		}

		return shader;
	}
    public struct DrawableTriangle {
        public float[] Vertices { get; set; }

        public DrawableTriangle(float[] vertices) {
            if (vertices.Length != 9) {
                throw new ArgumentException("A triangle must have exactly 9 vertex values.");
            }
            Vertices = vertices;
        }

        public unsafe void Draw(float*[] updatedVertecies) {
			if (updatedVertecies!=null) {
				
			} else {

			}
        }


    }
    public struct DrawableCircle {
        public float[] vertices { get; set; }

        public DrawableCircle(float[] _vertices) {
            if (_vertices.Length != 9) {
                throw new ArgumentException("A triangle must have exactly 9 vertex values.");
            }
            vertices = _vertices;
        }

		public unsafe void Draw(float*[] unpdatedVertecies) {
			
		}
    }
}