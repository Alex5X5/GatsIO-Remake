namespace ShGame.game.Client.Rendering;
//using Silk.NET.OpenGL
using Silk.NET.OpenGL;

using System.Runtime.InteropServices;
using System.Threading;

public abstract class Drawable {

	uint vaoHandle = 0;
	uint vboHandle = 0;

	protected uint FLOAT_COUNT;

	public float[] vertices;

	private Vector3d p1 = new(30,1000,0);
	private Vector3d p2 = new(500,0,0);
	private Vector3d p3 = new(0,100,0);

	private bool dirty = true;

	private static readonly Logger logger = new(new LoggingLevel("Drawable"));


	public Drawable(float[] vertices_) : this() {
		Console.WriteLine("[Drawable]:gl constructor");
		vertices = vertices_;
	}

	public Drawable() {
		Console.WriteLine("[Drawable]:empty constructor");
	}

	public unsafe void Setup(GL gl) {
		Console.WriteLine("[Drawable]:setup");
		vaoHandle = gl.GenVertexArray();
		BindVAO();
        vboHandle = gl.GenBuffer();
		BindVBO();
        gl.EnableVertexAttribArray(0);
        //gl.EnableVertexAttribArray(1);
        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12, null);
		gl.BufferData(BufferTargetARB.ArrayBuffer, 9 * sizeof(float), in IntPtr.Zero, BufferUsageARB.StaticDraw);
        //gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 12, (void*)12);
        //gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 12, (void*)24);
        //gl.DrawArrays(PrimitiveType.Triangles, 0, 1);
        UnbindVBO();
		UnbindVAO();
	}

	public unsafe void Update(GL gl) {
		logger.Log("updating");
		float* temporaryMemory = (float*)NativeMemory.Alloc(36);
		float* ptr = temporaryMemory;
		BindVBO();
        *ptr = (float)p1.x;
        ptr += 1;
        *ptr = (float)p1.y;
        ptr += 1;
        *ptr = 0;
        ptr += 1;
        *ptr = (float)p2.x;
        ptr += 1;
        *ptr = (float)p2.y;
        ptr += 1;
        *ptr = 0;
        ptr += 1;
        *ptr = (float)p3.x;
        ptr += 1;
        *ptr = (float)p3.y;
        ptr += 1;
        *ptr = 0;
        ptr += 1;
        ptr = temporaryMemory;
		Console.WriteLine("data:");
		for(int i=0; i<9; i++) {
			Console.WriteLine(*ptr);
            ptr++;
		}
        gl.BufferSubData(GLEnum.ArrayBuffer, 0, 36, temporaryMemory);
        UnbindVBO();
		BindVAO();
		gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
		UnbindVAO();
        NativeMemory.Free(temporaryMemory);
		logger.Log("finished update");
	}

	public unsafe void Draw(GL gl) {
		Console.WriteLine("[Drawable]:Draw");
		BindVAO();
		gl.DrawArrays(PrimitiveType.Triangles, 0, 9);
		UnbindVAO();
        logger.Log("finished Draw");
    }

	public static unsafe void BufferTriangles(float* triangles, uint count, GL? gl) {
		logger.Log("buffering triangles");
		float* buffer = (float*)NativeMemory.Alloc((nuint)count*sizeof(float));
		float* ptr = buffer;
		for(int i=0; i<count/3; i++) {
			logger.Log(Convert.ToString(*triangles));
            *ptr=*triangles;
            ptr++;
			triangles++;
			logger.Log(Convert.ToString(*triangles));
            *ptr =*triangles;
            ptr++;
			triangles++;
			logger.Log(Convert.ToString(*triangles));
            *ptr = 0;
			triangles++;
            ptr++;
        }
		gl?.BufferSubData(GLEnum.ArrayBuffer, 0, count, buffer);
		NativeMemory.Free(buffer);
    }

    public void BindVBO() => RendererGl.Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);
	public static void UnbindVBO() => RendererGl.Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

	public void BindVAO() => RendererGl.Gl?.BindVertexArray(vaoHandle);
	public static void UnbindVAO() => RendererGl.Gl?.BindVertexArray(0);
}
