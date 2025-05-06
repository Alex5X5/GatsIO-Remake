namespace ShGame.game.Client.Rendering;
//using Silk.NET.OpenGL
using Silk.NET.OpenGL;

using System.Runtime.InteropServices;

	/// <summary>
	/// This is a Base class for objects that have to be drawn.
	/// </summary>
public unsafe abstract class Drawable:IDisposable {


	protected uint vaoHandle = 0;
	protected uint vboHandle = 0;

	private nuint VERTICES_COUNT;

	//public float[] vertices;
	public float* VertexDataPtr;


	public bool dirty = true;

	private static readonly Logger logger = new(new LoggingLevel("Drawable"));

	public Drawable(int verticesCount) {
		VertexDataPtr = (float*)NativeMemory.AllocZeroed((uint)verticesCount*3*sizeof(float));
		VERTICES_COUNT = (uint)verticesCount;
		//vertices = new float[VERTICES_COUNT];
		//vertices.Initialize();
		//Console.WriteLine("[Drawable]:empty constructor");
	}

	public virtual void Dispose() {
		GC.SuppressFinalize(this);
		NativeMemory.Free(VertexDataPtr);
	}

	public virtual void UpdateVertices() { }

	public unsafe void Setup(GL gl) {
		//Console.WriteLine("[Drawable]:setup");
		vaoHandle = gl.GenVertexArray();
		BindVAO();
		vboHandle = gl.GenBuffer();
		BindVBO();
		gl.EnableVertexAttribArray(0);
		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12, null);
		gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)VERTICES_COUNT * sizeof(float), in IntPtr.Zero, BufferUsageARB.StaticDraw);
		UnbindVBO();
		UnbindVAO();
	}

	public unsafe void Draw(GL gl) {
		if (dirty) {
			UpdateVertices();
			dirty = false;
		}
		BindVAO();
		BindVBO();
		float* ptr = VertexDataPtr;
		for (nuint i = 0; i<VERTICES_COUNT; i+=9) {
			gl.BufferSubData(GLEnum.ArrayBuffer, (nint)i*sizeof(float), (uint)9*sizeof(float), ptr);
			ptr+=(uint)9;
        }
		gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)VERTICES_COUNT);
		UnbindVAO();
		UnbindVBO();
	}

	public static unsafe float* BufferTriangleValues(float* vertices) {
		float* buffer = (float*)NativeMemory.Alloc(9*sizeof(float));
		float* ptr = buffer;
		//Console.Write("buffering:");
		for (int i = 0; i<3; i++) {
			//Console.Write("(");
			*ptr=*vertices;
			//Console.Write(*vertices+", ");
			ptr++;
			vertices++;
			*ptr =*vertices;
			//Console.Write(*vertices+", ");
			ptr++;
			vertices++;
			*ptr = 0;
			//Console.Write(0+", ");
			vertices++;
			ptr++;
			//Console.Write(")");
		}
		//Console.WriteLine();
		return buffer;
	}

	public void BindVBO() => RendererGl.Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);
	public static void UnbindVBO() => RendererGl.Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

	public void BindVAO() => RendererGl.Gl?.BindVertexArray(vaoHandle);
	public static void UnbindVAO() => RendererGl.Gl?.BindVertexArray(0);
}
