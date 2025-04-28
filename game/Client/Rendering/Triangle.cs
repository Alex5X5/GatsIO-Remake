using Silk.NET.OpenGL;

using System.Runtime.InteropServices;

namespace ShGame.game.Client.Rendering;
internal class Triangle {

    Vector3d point1;
    Vector3d point2;
    Vector3d point3;


    uint vao;
    uint vbo;

    public unsafe void Setup(GL gl) {
        //create a vertex array object vao
        vao = gl.GenVertexArray();
        gl.BindVertexArray(vao);

        //create a vertex bufer object that i associated to the vurrent vao
        vbo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

        //set up the attributes of the current vao to accept Vector3d data
        gl.EnableVertexAttribArray(0);
        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3d.Size, null);

        //unbind the vao and the vbo
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    public unsafe void Update(GL gl, Vector3d* v1, Vector3d* v2, Vector3d* v3) {
        Console.WriteLine(v1->x);
        Console.WriteLine(v1->y);
        Console.WriteLine(v1->z);
        Console.WriteLine(v2->x);
        Console.WriteLine(v2->y);
        Console.WriteLine(v2->z);
        Console.WriteLine(v3->x);
        Console.WriteLine(v3->y);
        Console.WriteLine(v3->z);
        point1 = *v1;
        point2 = *v2;
        point3 = *v3;
        BufferData(gl);
    }

    private unsafe void BufferData(GL gl) {
        Vector3d* data = (Vector3d*)NativeMemory.Alloc(3*Vector3d.Size);
        data->x = point1.x;
        data->y = point1.y;
        data->z = point1.z;
        Console.WriteLine(point1.ToString());
        Console.WriteLine(data->x);
        Console.WriteLine(data->y);
        Console.WriteLine(data->z);
        data+=Vector3d.Size;
        *data = point2;
        Console.WriteLine(data->x);
        Console.WriteLine(data->y);
        Console.WriteLine(data->z);
        data+=Vector3d.Size;
        *data = point3;
        Console.WriteLine(data->x);
        Console.WriteLine(data->y);
        Console.WriteLine(data->z);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
        gl.BufferData(BufferTargetARB.ArrayBuffer, 3*Vertex.Size, data, BufferUsageARB.StaticDraw);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        NativeMemory.Free(data);
    }

    public void Draw(GL gl) {
        gl.BindVertexArray(vao);
        gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
        gl.BindVertexArray(0);

    }
}

public unsafe class Vertex {

    public const uint Size = Vector3d.Size;

    Vector3d Pos;

    public Vertex() { }

    public void Update(float x1, float y1, float z1) {
        Pos.x = x1; Pos.y = y1; Pos.z = z1;
    }

    public unsafe void Update(Vector3d* p1) {
        Pos.Set(*p1);
    }

    public static void SetupVAO(GL gl) {
        gl.EnableVertexAttribArray(0);
        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Size, null);
    }

}