namespace ShGame.Drawing;

using ShGame.Drawing.Helpers;
using ShGame.Math;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using System;

public abstract class Window : IDisposable {

	private Thread thread;

	private IWindow? window;
	IInputContext input;

	private double Time;
	private double LastFrame;
	private bool Dirty = true;

	private bool loaded = false;

	private ColorF backgroundColor = new(Color.BLACK);

	private GL Gl;
	private DrawingContext drawingContext;

	public Rect Size => new(0, 0, window != null ? (double)window.Size.X : 0.0, window != null ? (double)window.Size.Y : 0.0);


	public Window(double width, double height, string title) : this(CreateWindowoptions(width, height, title)) { }

	public Window(WindowOptions options) {
		window = Silk.NET.Windowing.Window.Create(options);
		Initialize();
		drawingContext = new(window);
	}

	private static WindowOptions CreateWindowoptions(double width, double height, string title) {
		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>((int)Math.Floor(width), (int)Math.Floor(height));
		options.Title = title;
		return options;
	}

	private void Initialize() {
		ThrowHelper.ThrowIfNull(window, "Can not initialize window while window is null");
		window.Load += OnLoad;
		window.Render += OnRender;
		window.FramebufferResize += OnFrameBufferResize;
	}

	public void Dispose() {

	}

	private void OnLoad() {
		loaded = true;
		Gl = GL.GetApi(window);
		drawingContext.Load(Gl);
		input = window!.CreateInput();
		foreach (var keyboard in input.Keyboards) {
			keyboard.KeyUp += KeyUpBase;
			keyboard.KeyDown += KeyDownBase;
		}
		foreach (var mouse in input.Mice) {
			mouse.MouseUp += MouseUpBase;
			mouse.MouseDown += MouseDownBase;
			mouse.MouseMove += MouseMovedBase;
		}
		window!.Closing += OnClosing;
	}

	public void InvalidateVisual() {
		Dirty = true;
	}

	private void OnRender(double deltaTime) {
		if (Dirty) {
			ReDraw(deltaTime);
			Dirty = false;
		}
		OnImmediateRender(deltaTime);
	}

	private void OnFrameBufferResize(Vector2D<int> size) {
		drawingContext.OnFrameBufferSizeChanged(size);
		Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
	}

	private void ReDraw(double deltaTime) {
		drawingContext.ClearOps();
		Draw(deltaTime, drawingContext);
		drawingContext.BufferTrianglesToGpu(Gl);
	}

	private void OnImmediateRender(double deltaTime) {
		ThrowHelper.ThrowIfNull(Gl, "Can not render while gl is null");
		Gl.ClearColor(backgroundColor.Red, backgroundColor.Green, backgroundColor.Blue, 1f);
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);
		Gl.UseProgram(drawingContext.fillShaderProgramm.ProgramHandle);
		drawingContext.DrawTriangleBuffer();
	}

	protected abstract void Draw(double deltaTime, DrawingContext context);

	private void MouseDownBase(IMouse mouse, MouseButton button) {
		Vector3d pos = new(mouse.Position.X, mouse.Position.Y, 0.0);
		MousePressed(pos);
	}

	private void MouseUpBase(IMouse mouse, MouseButton button) {
		Vector3d pos = new(mouse.Position.X, mouse.Position.Y, 0.0);
		MouseReleased(pos);
	}

	private void MouseMovedBase(IMouse mouse, System.Numerics.Vector2 pos) {
		Vector3d pos_ = new(pos.X, pos.Y, 0.0);
		MouseMoved(pos_);
	}

	private void KeyDownBase(IKeyboard keyboard, Silk.NET.Input.Key key, int arg3) {
		KeyPressed((Key)key);
	}

	private void KeyUpBase(IKeyboard keyboard, Silk.NET.Input.Key key, int arg3) {
		KeyReleased((Key)key);
	}

	private void OnClosingBase() {
		OnClosing();
	}

	protected virtual void KeyPressed(Key key) { }
	protected virtual void KeyReleased(Key key) { }

	protected virtual void MouseMoved(Vector3d pos) { }
	protected virtual void MousePressed(Vector3d pos) { }
	protected virtual void MouseReleased(Vector3d pos) { }
	protected virtual void MouseClicked(Vector3d pos) { }

	protected virtual void OnClosing() { }

	public void SetBackgroundColor(Color color) {
		backgroundColor = new ColorF(color);
	}

	public void Show() {
		ThrowHelper.ThrowIfNull(window, $"cannot show when window is {window?.ToString() ?? "null"}");
		thread = new(window!.Run);
		thread.Start();
	}
}
