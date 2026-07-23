namespace ShGame.Drawing;

using ShGame.Drawing.Shapes;
using ShGame.Math;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using System.Runtime.InteropServices;

public class DrawingContext {

	private IWindow window;
	private GL? Gl;

	private List<TriangleShape> bufferedTriangles;
	private uint triangleBufferSize;

	private List<LineShape> triangles;
	private uint linebufferSize;

	private uint vaoHandle, vboHandle;
	internal ShaderProgram fillShaderProgramm;
	internal ShaderProgram lineShaderProgramm;

	public DrawingContext(IWindow window) {
		this.window = window;
		bufferedTriangles = [];
		triangleBufferSize = 0;
	}

	internal void Load(GL gl) {
		Gl = gl;
		fillShaderProgramm = new(Gl, window, ShaderSources.COLOR_VERTEXT_SHADER_SOURCE, ShaderSources.COLOR_FRGMENT_SHADER_SOURCE);
		//lineShaderProgramm = new(Gl, window, ShaderSources.LINE_VERTEX_SHADER_SOURCE, ShaderSources.LINE_FRAGMENT_SOURCE);
		vaoHandle = Gl.GenVertexArray();
		vboHandle = Gl.GenBuffer();
		BindVAO();
		BindVBO();
		unsafe {
			Gl.EnableVertexAttribArray(0);
			Gl.EnableVertexAttribArray(1);
			const uint stride = ColoredPointF.SizeInBytes;
			//position data
			Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, (void*)0);
			//color data
			Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, stride, (void*)PointF.SizeInBytes);
		}
	}

	private void BindVBO() => Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);
	private void UnbindVBO() => Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

	private void BindVAO() => Gl?.BindVertexArray(vaoHandle);
	private void UnbindVAO() => Gl?.BindVertexArray(0);

	internal void OnFrameBufferSizeChanged(Vector2D<int> size) {
		fillShaderProgramm.OnFrameBufferSizeChanged(size);
		//lineShaderProgramm.OnFrameBufferSizeChanged(size);
	}

	internal uint GetRequiredBufferSize() {
		return triangleBufferSize;
	}

	internal void BufferTrianglesToGpu(GL Gl) {
		Span<TriangleShape> span = CollectionsMarshal.AsSpan(bufferedTriangles);
		BindVAO();
		BindVBO();
		unsafe {
			fixed (TriangleShape* ptr = span) {
				Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)triangleBufferSize, ptr, BufferUsageARB.DynamicDraw);
			}
		}
		UnbindVAO();
		UnbindVBO();
	}

	internal void DrawTriangleBuffer() {
		Gl.UseProgram(fillShaderProgramm.ProgramHandle);
		BindVAO();
		BindVBO();
		uint triangleCount = triangleBufferSize / TriangleShape.SizeInBytes;
		Gl.DrawArrays(PrimitiveType.Triangles, 0, triangleCount * 3);
		UnbindVAO();
		UnbindVBO();
	}

	internal void ClearOps() {
		bufferedTriangles = [];
		triangleBufferSize = 0;
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
		Vector3d perp = new Vector3d(dir.y * -1.0, dir.x, 0.0).Nor().Scl(stroke * 0.5);
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
		bufferedTriangles.Add(shape);
		triangleBufferSize += TriangleShape.SizeInBytes;
	}

	public void DrawQuad(Vector3d p1, Vector3d p2, Vector3d p3, Vector3d p4, Color color) {
		DrawQuad(new Quad(p1, p2, p3, p4), color);
	}

	public void DrawQuad(Quad quad, Color color) {
		QuadShape shape = new QuadShape(quad, color);
		bufferedTriangles.Add(shape.triangle1);
		bufferedTriangles.Add(shape.triangle2);
		triangleBufferSize += 2 * TriangleShape.SizeInBytes;
	}

	public void DrawRectangle(double x, double y, double width, double height, Color color) {
		Rect rect = new(x, y, width, height);
		DrawRectangle(rect, color);
	}

	public void DrawRectangle(Rect rect, Color color) {
		RectShape shape = new(rect, color);
		bufferedTriangles.Add(shape.triangle1);
		bufferedTriangles.Add(shape.triangle2);
		triangleBufferSize += 2 * TriangleShape.SizeInBytes;
	}

	public void DrawLineRectangle() {

	}
}
