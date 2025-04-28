namespace ShGame.game.Client;

using ShGame.game.Net;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Net;
using ShGame.game.Client.Rendering;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;

#pragma warning disable CS8500 //insert spaces instead of tabs

public class Client2 : Form {

    private IWindow window;

    private GL Gl;

    private uint _shaderProgram;

    //privaet

    internal bool keyUp = false;
    internal bool keyDown = false;
    internal bool keyLeft = false;
    internal bool keyRight = false;

    private bool stop = false;

    private readonly Logger logger;

    //Client.Panel? panel;
    private readonly Renderer renderer;
    private readonly LoggingLevel mlvl = new("Client");
    private NetHandler? netHandler;

    internal Player player;

    unsafe private Player[] players;
    unsafe internal Obstacle[] obstacles = new Obstacle[GameServer.OBSTACLE_COUNT];
    private Thread renderThread = new(() => { });
    private Thread connectionThread = new(() => { });
    private Thread playerMoveThread = new(() => { });

    public Client2() : this(5000) { }

    public Client2(uint port) : this(GameServer.GetLocalIP(), port) { }

    public Client2(IPAddress address, uint port) : base() {
        logger=new Logger(mlvl);
        logger.Log(
            "address port constructor",
            new MessageParameter("address", address.ToString()),
            new MessageParameter("port", port)
        );


        var options = WindowOptions.Default;
        options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
        options.Title = "OpenGL Drawable Triangles";


        window = Window.Create(options);
        window.Load += OnLoad;
        window.Run();

        //SetVisible();
        //Thread.Sleep(500);
        //handler=new NetHandler();
        //byte[] temp = new byte[8];
        //new Random().NextBytes(temp);
        //player=new Player(new Vector3d(100, 100, 0), 100, BitConverter.ToInt64(temp, 0));
        //players=new Player[GameServer.MAX_PLAYER_COUNT];
        //for (int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++)
        //	players[i] = new Player(new Vector3d(0, 0, 0), -1, 1);
        //obstacles.Initialize();
        //renderer=new Renderer();
        //StartThreads(address, port);
    }

    private void OnLoad() {
        Gl = GL.GetApi(window);
        window.Render += (double deltaTime) => OnRender(deltaTime, Gl);

        // Create the shader program
        _shaderProgram = CreateShaderProgram();

        //foreach (Drawable drawable in dTriangles) {
        //    drawable.Setup(Gl);
        //}

    }

    private void OnRender() {

    }

    public unsafe void OnRender(double deltaTime, GL gl) {
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);

        gl.UseProgram(_shaderProgram);

        int screenWidthLocation = gl.GetUniformLocation(_shaderProgram, "u_WindowWidth");
        int screenHeightLocation = gl.GetUniformLocation(_shaderProgram, "u_WindowHeight");
        gl.Uniform1(screenWidthLocation, (float)window.Size.X);
        gl.Uniform1(screenHeightLocation, (float)window.Size.Y);

        //foreach (var triangle in dTriangles) {
        //    triangle.Draw(gl);
        //}

        //i++;
        //if (i>10) {
        //	_window.Close();
        //	_window.Render-=OnRender;
        //}
    }



    private uint CreateShaderProgram() {

        uint shaderProgram = Gl.CreateProgram();

        // Vertex Shader
        string vertexShaderSource = @"
			#version 330 core

			layout(location = 0) in vec3 aPos;
			uniform float u_WindowWidth;
			uniform float u_WindowHeight;
			void main()
			{
				gl_Position = vec4(aPos.x / u_WindowWidth -1 , aPos.y / u_WindowHeight -1, aPos.z, 1.0);
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
        Gl.UseProgram(shaderProgram);
        Gl.AttachShader(shaderProgram, vertexShader);
        Gl.AttachShader(shaderProgram, fragmentShader);
        Gl.LinkProgram(shaderProgram);


        int windowWidthLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowWidth");
        int windowHeightLocation = Gl.GetUniformLocation(shaderProgram, "u_WindowHeight");
        Gl.Uniform1(windowWidthLocation, window.Size.X);
        Gl.Uniform1(windowHeightLocation, window.Size.Y);

        //string s = gl.GetActiveUniform(shaderProgram, windowWidthLocation<0 ? 0 : (uint)windowWidthLocation, out int size, out UniformType type);
        //s += " "+gl.GetActiveUniform(shaderProgram, windowHeightLocation<0 ? 0 : (uint)windowWidthLocation, out int size2, out UniformType type2);
        //Console.WriteLine("activeUniforms:"+s);

        Gl.DeleteShader(vertexShader);
        Gl.DeleteShader(fragmentShader);

        return shaderProgram;
    }

	private uint CompileShader(ShaderType type, string source) {
		uint shader = Gl.CreateShader(type);
		Gl.ShaderSource(shader, source);
		Gl.CompileShader(shader);

		Gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
		if (status == 0) {
			string infoLog = Gl.GetShaderInfoLog(shader);
			Console.WriteLine($"Error compiling shader ({type}): {infoLog}");
		}

		return shader;
	}

    private void SetVisible() {
        logger.Log("setting vivible");

        SuspendLayout();

        AutoScaleMode=AutoScaleMode.None;
        ClientSize=new Size(Renderer.WIDTH, Renderer.HEIGHT);
        Name="Client";
        FormClosing+=Stop;
        KeyDown+=new KeyEventHandler(KeyDown_);
        KeyUp+=new KeyEventHandler(KeyUp_);
        Text="Client";
        //panel = new() {
        //    ClientSize=new Size(Renderer.WIDTH, Renderer.HEIGHT),
        //    Name="Panel"
        //};

        ResumeLayout(false);
        PerformLayout();
        logger.Log("performed layout");
    }

    private unsafe void StartThreads(IPAddress address, uint port) {
        logger.Log("start threads!");
        connectionThread=new Thread(
            () => {
                netHandler = new(address, port);
                if (NetHandlerConnected())
                    netHandler.GetMap(ref obstacles);
                Console.WriteLine(player);
                while (!stop && NetHandlerConnected()) {
                    logger.Log("asking for players");
                    netHandler.ExchangePlayers(player, ref players);
                    Thread.Sleep(100);
                }
                netHandler?.Dispose();
            }
        );
        connectionThread.Start();

        renderThread=new Thread(
                () => {
                    while (!CanRaiseEvents&&!stop)
                        Thread.Sleep(10);
                    while (!stop) {
                        Thread.Sleep(30);
                        Invalidate();
                    }
                }
        );
        renderThread.Start();
        logger.Log("started render thread");

        playerMoveThread=new Thread(
                () => {
                    while (!CanRaiseEvents&&!stop)
                        Thread.Sleep(10);
                    while (!stop) {
                        foreach (Player p in players) {
                            if (p!=null)
                                if (p.Health!=-1)
                                    p.Move();
                        }
                        if (player!=null)
                            if (player.Health!=-1)
                                player.Move();
                        Thread.Sleep(10);
                    }
                }
        );
        playerMoveThread.Start();
        logger.Log("started player move thread");
    }

    private void KeyUp_(object? sender, KeyEventArgs e) {
        switch (e.KeyCode) {
            case Keys.W:
                keyUp=false;
                break;
            case Keys.S:
                keyDown=false;
                break;
            case Keys.A:
                keyLeft=false;
                break;
            case Keys.D:
                keyRight=false;
                break;
        }
        //player.OnKeyEvent(c: this);
        Console.WriteLine("key up, p:"+player.ToString());
    }

    private void KeyDown_(object? sender, KeyEventArgs e) {
        switch (e.KeyCode) {
            case Keys.W:
                keyUp=true;
                break;
            case Keys.S:
                keyDown=true;
                break;
            case Keys.A:
                keyLeft=true;
                break;
            case Keys.D:
                keyRight=true;
                break;
            case Keys.Escape:
                Stop(this, null);
                break;
        }
        //player.OnKeyEvent(c: this);
        Console.WriteLine("key up, p:"+player.ToString());
    }

    protected override void OnPaint(PaintEventArgs e) {
        unsafe {
            fixed (Obstacle[]* ob = &obstacles)
                if (!stop)
                    e.Graphics.DrawImage(renderer.Render(ref players, ref player, ob), 0, 0);
        }
    }

    protected override void OnPaintBackground(PaintEventArgs e) {
        //Don't allow the background to paint
    }

    private bool NetHandlerConnected() {
        if (netHandler != null)
            if (netHandler.Connected)
                return true;
        return false;
    }

    private unsafe void Stop(object? sender, FormClosingEventArgs? e) {
        stop=true;
        if (sender==this) {
            logger.Log("stopping");
            renderer.Dispose();
            Dispose();
            Thread.Sleep(500);
        }
    }


    private class Panel : System.Windows.Forms.Panel {
        public Panel() : base() {
        }
        protected override void OnPaintBackground(PaintEventArgs e) {

        }

        protected override void OnPaint(PaintEventArgs e) {
            //if (!stop)
            //e.Graphics.DrawImage(renderer.Render(ref players, ref player, ref obstacles), 0, 0);
        }
    }
}