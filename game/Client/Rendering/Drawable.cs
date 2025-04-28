//using Silk.NET.OpenGL//namespace ShGame.game.Client.Rendering;
using Silk.NET.OpenGL;

using System.Runtime.InteropServices;

internal class Drawable {

	uint vaoHandle = 0;
	uint vboHandle = 0;

	private readonly uint FLOAT_COUNT = 6;

	//public float[] Vertices { get; }

	private Vector3d p1 = new(30,1000,0);
	private Vector3d p2 = new(500,0,0);
	private Vector3d p3 = new(0,100,0);

	private bool dirty = true;

	private static readonly Logger logger = new(new LoggingLevel("Drawable"));


	public Drawable(GL gl, float[] vertices) : this() {
		Console.WriteLine("[Drawable]:gl constructor");
		Setup(gl);
	}

	public Drawable() {
		//Vertices = vertices;
		Console.WriteLine("[Drawable]:vertices constructor");
	}

	public unsafe void Setup(GL gl) {
		Console.WriteLine("[Drawable]:setup");
		vaoHandle = gl.GenVertexArray();
		BindVAO(gl);
        vboHandle = gl.GenBuffer();
		BindVBO(gl);
        gl.EnableVertexAttribArray(0);
        //gl.EnableVertexAttribArray(1);
        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12, null);
		gl.BufferData(BufferTargetARB.ArrayBuffer, 9 * sizeof(float), in IntPtr.Zero, BufferUsageARB.StaticDraw);
        //gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 12, (void*)12);
        //gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 12, (void*)24);
        //gl.DrawArrays(PrimitiveType.Triangles, 0, 1);
        UnbindVBO(gl);
		UnbindVAO(gl);
	}


	public unsafe void Update(GL gl) {
		logger.Log("updating");
		float* temporaryMemory = (float*)NativeMemory.Alloc(36);
		float* ptr = temporaryMemory;
		BindVBO(gl);
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
        UnbindVBO(gl);
		BindVAO(gl);
		gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
		UnbindVAO(gl);
        NativeMemory.Free(temporaryMemory);
		logger.Log("finished update");
	}

	public unsafe void Draw(GL gl) {
		Console.WriteLine("[Drawable]:Draw");
		BindVAO(gl);
		Update(gl);
		gl.DrawArrays(PrimitiveType.Triangles, 0, 9);
		UnbindVAO(gl);
        logger.Log("finished Draw");
    }

    public void BindVBO(GL gl) => gl.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);
	public void UnbindVBO(GL gl) => gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

	public void BindVAO(GL gl) => gl.BindVertexArray(vaoHandle);
	public void UnbindVAO(GL gl) => gl.BindVertexArray(0);
}
