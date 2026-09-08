namespace ShGame.Drawing;

using ShGame.Math;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

public class DrawingContext : IDisposable {
	
	private GL? Gl;

	private TriangleBuffer buffer;
	private TextureBuffer textures;

	private uint vaoHandle, vboHandle;
	internal ShaderProgram fillShaderProgramm;

	public DrawingContext() {
		buffer = new();
		textures = new();
	}

	internal void Load(GL gl) {
		Gl = gl;
		fillShaderProgramm = new(Gl, ShaderSources.COLOR_VERTEXT_SHADER_SOURCE, ShaderSources.COLOR_FRGMENT_SHADER_SOURCE);
		vaoHandle = Gl.GenVertexArray();
		vboHandle = Gl.GenBuffer();
		BindVAO();
		BindVBO();
		unsafe {
			Gl.EnableVertexAttribArray(0);
			Gl.EnableVertexAttribArray(1);
			Gl.EnableVertexAttribArray(2);
			Gl.EnableVertexAttribArray(3);
			const uint stride = ColoredPointF.SizeInBytes;
			//position data
			Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, (void*)0);
			//color data
			Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, stride, (void*)PointF.SizeInBytes);
			// texture position
			Gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, (void*)(PointF.SizeInBytes + ColorF.SizeInBytes));
			// texture index
			Gl.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, stride, (void*)(PointF.SizeInBytes + ColorF.SizeInBytes + PointF.SizeInBytes));
		}
	}

	private void BindVBO() => Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);
	private void UnbindVBO() => Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

	private void BindVAO() => Gl?.BindVertexArray(vaoHandle);
	private void UnbindVAO() => Gl?.BindVertexArray(0);

	internal void OnFrameBufferSizeChanged(Vector2D<int> size) {
		fillShaderProgramm.OnFrameBufferSizeChanged(size);
	}

	internal void BufferTrianglesToGpu() {
		//Span<TriangleShape> span = CollectionsMarshal.AsSpan(bufferedTriangles);
		BindVAO();
		BindVBO();
		unsafe {
			Gl.BufferData(
				BufferTargetARB.ArrayBuffer,
				buffer.Count*TriangleShape.SizeInBytes,
				buffer.Data,
				BufferUsageARB.DynamicDraw);
		}
		UnbindVAO();
		UnbindVBO();
	}

	internal void BufferTexturesToGpu() {
		foreach(var image in textures.buffer) {
			
		}
	}

	internal void DrawTriangleBuffer() {
		Gl.UseProgram(fillShaderProgramm.ProgramHandle);
		BindVAO();
		BindVBO();
		Gl.DrawArrays(PrimitiveType.Triangles, 0, buffer.Count * 3);
		UnbindVAO();
		UnbindVBO();
	}

	internal void ClearOps() {
		buffer.Clear();
	}

	internal DrawingContext CreateChildContext() {
		return null;
	}

	public void DrawLine(double x1, double y1, double x2, double y2, Color color, double stroke = 1) {
		DrawLine(new Line(x1, y1, x2, y2), color, stroke);
	}

	public void DrawLine(Vector3d p1, Vector3d p2, Color color, double stroke = 1) {
		DrawLine(new Line(p1, p2), color, stroke);
	}

	public void DrawLine(Line line, Color color, double stroke = 1) {
		Vector3d dir = line.Direction;
		Vector3d perp = new Vector3d(dir.Y * -1.0, dir.X, 0.0).Nor().Scl(stroke * 0.5);
		Vector3d p1 = line.A.Add(perp);
		Vector3d p2 = line.A.Add(perp.Scl(-1.0));
		Vector3d distance = line.B.Sub(line.A);
		Vector3d p3 = p1.Add(distance);
		Vector3d p4 = p2.Add(distance);
		DrawQuad(new Quad(p1, p2, p3, p4), color);
	}

	public void DrawTriangle(double x1, double y1, double x2, double y2, double x3, double y3, Color color) {
		DrawTriangle(new Triangle(x1, y1, x2, y2, x3, y3), color);
	}

	public void DrawTriangle(Vector3d p1, Vector3d p2, Vector3d p3, Color color) {
		DrawTriangle(new Triangle(p1, p2, p3), color);
	}

	public void DrawTriangle(Triangle triangle, Color color) {
		TriangleShape shape = new TriangleShape(triangle, color);
		unsafe {
			buffer.Buffer(&shape, 1);
		}
	}

	public void DrawQuad(Vector3d p1, Vector3d p2, Vector3d p3, Vector3d p4, Color color) {
		DrawQuad(new Quad(p1, p2, p3, p4), color);
	}

	public void DrawQuad(Quad quad, Color color) {
		QuadShape shape = new QuadShape(quad, color);
		unsafe {
			buffer.Buffer((TriangleShape*)&shape, 2);
		}
	}

	public void DrawRectangle(double x, double y, double width, double height, Color color) {
		Rect rect = new(x, y, width, height);
		DrawRectangle(rect, color);
	}

	public void DrawRectangle(Rect rect, Color color) {
		RectShape shape = new(rect, color);
		unsafe {
			buffer.Buffer((TriangleShape*)&shape, 2);
		}
	}

	public void DrawCircle(Circle circle, Color color) {
		CircleShape shape = new(circle, color);
		unsafe {
			fixed(TriangleShape* ptr = &shape.triangles[0])
				buffer.Buffer(ptr, (uint)shape.triangles.Length);
		}
	}

	public void DrawLineRectangle() {

	}

	public void DrawBuffer() {
		
	}

	public unsafe void DrawImage(Vector3d pos, Image image) {
		RectShape shape = new(new Rect(pos.X, pos.Y, image.Width, image.Height), Color.WHITE);
		ColoredPointF* ptr = (ColoredPointF*)&shape;
		//for(int i=0; i<6; i++) {
		//	ptr->color = new ColorF(Color.WHITE);
		//	ptr->texturePos = new PointF(0, 0);
		//	ptr->textureIndex = 0;
		//	ptr++;
		//}
		unsafe {
			buffer.Buffer((TriangleShape*)&shape, 2);
		}	
	}

	public void Dispose() {
		buffer.Dispose();
	}
}
