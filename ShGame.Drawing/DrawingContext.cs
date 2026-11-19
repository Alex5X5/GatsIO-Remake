namespace ShGame.Drawing;

using ShGame.Drawing.Helpers;
using ShGame.Math;

using Silk.NET.Maths;
using Silk.NET.OpenGL;

using System.Collections.Generic;

public class DrawingContext : IDisposable {
	
	private GL? Gl;

	private TriangleBuffer buffer;

	private uint vaoHandle, vboHandle;
	private uint currentTexture;

	internal ShaderProgram? fillShaderProgramm;
	internal ShaderProgram? textureShaderProgram;

	private readonly List<DrawBatch> batches = new();
	private int batchStart;


	internal DrawingContext() {
		buffer = new();
	}

	internal void Load(GL gl) {
		Gl = gl;
		
		fillShaderProgramm = new(Gl, ShaderSources.COLOR_VERTEX_SHADER_SOURCE, ShaderSources.COLOR_FRAGMENT_SHADER_SOURCE);
		textureShaderProgram = new(Gl, ShaderSources.TEXTURE_VERTEX_SHADER_SOURCE, ShaderSources.TEXTURE_FRAGMENT_SHADER_SOURCE);
		
		vaoHandle = Gl.GenVertexArray();
		vboHandle = Gl.GenBuffer();
		Gl?.BindVertexArray(vaoHandle);
		Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);
		unsafe {
			Gl.EnableVertexAttribArray(0);
			Gl.EnableVertexAttribArray(1);
			Gl.EnableVertexAttribArray(2);
			const uint stride = ColoredPointF.SizeInBytes;
			//position data
			Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, (void*)0);
			//color data
			Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, stride, (void*)PointF.SizeInBytes);
			// texture position
			Gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, (void*)(PointF.SizeInBytes + ColorF.SizeInBytes));
		}
	}

	private void BindVBO() => Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, vboHandle);
	private void UnbindVBO() => Gl?.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

	private void BindVAO() => Gl?.BindVertexArray(vaoHandle);
	private void UnbindVAO() => Gl?.BindVertexArray(0);

	internal void OnFrameBufferSizeChanged(Vector2D<int> size) {
		fillShaderProgramm?.OnFrameBufferSizeChanged(size);
		textureShaderProgram?.OnFrameBufferSizeChanged(size);
	}

	internal void BufferTrianglesToGpu() {
		FlushBatch();
		BindVAO();
		BindVBO();
		unsafe {
			Gl!.BufferData(
				BufferTargetARB.ArrayBuffer,
				buffer.Count * TriangleShape.SizeInBytes,
				buffer.Data,
				BufferUsageARB.DynamicDraw);
		}
	}

	internal void DrawTriangleBuffer() {
		if (fillShaderProgramm is null)
			throw new InvalidOperationException("can not draw when fill shader programm is null");
		if (textureShaderProgram is null)
			throw new InvalidOperationException("can not draw when texture shader programm is null");
		BindVAO();
		BindVBO();
		foreach (var b in batches) {
			if (b.Texture == 0) {
				Gl!.UseProgram(fillShaderProgramm.ProgramHandle);
			} else {
				Gl!.UseProgram(textureShaderProgram.ProgramHandle);
				Gl.BindTexture(TextureTarget.Texture2D, b.Texture);
				textureShaderProgram.PrintErrors();
			}
			Gl.DrawArrays(PrimitiveType.Triangles, b.FirstVertex, (uint)b.VertexCount);
		}
	}

	internal void ClearOps() {
		buffer.Clear();
		batches.Clear();
		batchStart = 0;
	}

	private void FlushBatch() {
		int end = (int)buffer.Count * 3;
		if (end > batchStart)
			batches.Add(new DrawBatch(batchStart, end - batchStart, currentTexture));
		batchStart = end;
	}

	private void SwitchTexture(uint texture) {
		if (texture != currentTexture) {
			FlushBatch();
			currentTexture = texture;
		}
	}

	private void CloseBatch(uint texture) {
		int end = (int)buffer.Count * 3;
		if (end > batchStart)
			batches.Add(new DrawBatch(batchStart, end - batchStart, texture));
		batchStart = end;
	}

	private void AppendToBatch(Image image, int count) {
		if(batches.Count == 0)
			CloseBatch(0);
		if(batches.Count == 0){
			batches.Add(new DrawBatch(0, 0, image.texture));
			batchStart = 0;
		}
		var lastBatch = batches[batches.Count - 1];
		if (lastBatch.Texture == image.texture) {
			batches[batches.Count - 1] = new(lastBatch.FirstVertex, lastBatch.VertexCount + count, lastBatch.Texture);
		} else {
			CloseBatch(image.texture);
		}
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
		SwitchTexture(0);
		unsafe {
			buffer.Buffer(&shape, 1);
		}
	}

	public void DrawQuad(Vector3d p1, Vector3d p2, Vector3d p3, Vector3d p4, Color color) {
		DrawQuad(new Quad(p1, p2, p3, p4), color);
	}

	public void DrawQuad(Quad quad, Color color) {
		QuadShape shape = new QuadShape(quad, color);
		SwitchTexture(0);
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
		SwitchTexture(0);
		unsafe {
			buffer.Buffer((TriangleShape*)&shape, 2);
		}
	}

	public void DrawCircle(Circle circle, Color color) {
		CircleShape shape = new(circle, color);
		SwitchTexture(0);
		unsafe {
			fixed(TriangleShape* ptr = &shape.triangles[0])
				buffer.Buffer(ptr, (uint)shape.triangles.Length);
		}
	}

	public void DrawImage(Vector3d pos, Image image) {
		Rect rect = new(pos.X, pos.Y, image.Width, image.Height);
		DrawImageRectangle(rect, image);
	}

	public void DrawImageRectangle(Rect rect, Image image) {
		RectShape shape = new(rect, Color.BLACK);
		SwitchTexture(image.texture);
		unsafe {
			buffer.Buffer((TriangleShape*)&shape, 2);
		}
	}

	public void DrawImageTriangle(Triangle triangle, Image image) {
		TriangleShape shape = new(triangle, Color.BLACK);
		SwitchTexture(image.texture);
		unsafe {
			buffer.Buffer(&shape, 1);
		}
	}

	public void DrawImageQuad(Quad quad, Image image) {
		QuadShape shape = new(quad, Color.BLACK);
		SwitchTexture(image.texture);
		unsafe {
			buffer.Buffer((TriangleShape*)&shape, 2);
		}
	}

	public void Dispose() {
		buffer.Dispose();
	}

	private readonly record struct DrawBatch(int FirstVertex, int VertexCount, uint Texture);
}
