namespace ShGame.Client;

using ShGame.Drawing;
using ShGame.Game;
using ShGame.Math;
using ShGame.Net.Client.Services;
using ShGame.Rendering.Services;
using ShGame.Types;
using ShGame.Util;

using System.Threading;
using System.Threading.Tasks;

//#pragma warning disable CS8500 //insert spaces instead of tabs

public class Client : Window {

	private bool stop;

	public static readonly int SCREEN_PIXEL_WIDTH = Silk.NET.Windowing.Monitor.GetMainMonitor(null).Bounds.Size.Y;
	public static readonly int SCREEN_PIXEL_HEIGHT = Silk.NET.Windowing.Monitor.GetMainMonitor(null).Bounds.Size.X;

	private RenderService renderService;
	private NetworkService netService;
	private	GameService gameService;
	private Logger logger;

	private Player ControlledPlayer;

	private Thread NetworkThread;

	private Vector3d MousePos;
	
	public Client(Configuration config) : base(1200, 800, "ShGame") {
		
		logger=new Logger(new LoggingLevel("Client"));

		netService = new NetworkService(config);

		ControlledPlayer=new(new(100, 100, 0), 100, 1);
		gameService = new(ControlledPlayer);

		renderService = new RenderService(gameService);

		StartNetworkThread();
		gameService.StartAllLoops();

		SetBackgroundColor(new Color(90, 90, 110));
		Show();
	}


	protected override void OnClosing() {
		stop = true;
		gameService.Stop();
		netService.Dispose();
	}

	private void StartNetworkThread() {
		logger.Log("start threads!");
		NetworkThread = new Thread(
			async () => {
				if (NetHandlerConnected()) {
					gameService.Map = await netService.GetMapAsync();
					ControlledPlayer = await netService.RegisterToServerAsync();
				}
				while (!stop && NetHandlerConnected()) {
					logger.Log("asking for players");
					await netService.UpdatePlayerAsync(ControlledPlayer);
					gameService.Players = await netService.GetPlayersAsync();
					await Task.Delay(50);
				}
				netService?.Dispose();
			}
		);
		NetworkThread.Start();
		logger.Log("started connection thread:"+NetworkThread.IsAlive);
	}

	//private void KeyUp_(IKeyboard keyboard, Key key, int keyCode) {
	//	//logger.Log("key "+key+" up");
	//	switch (key) {
	//		case Key.W:
	//			keyUp=false;
	//			break;
	//		case Key.S:
	//			keyDown=false;
	//			break;
	//		case Key.A:
	//			keyLeft=false;
	//			break;
	//		case Key.D:
	//			keyRight=false;
	//			break;
	//	}
	//	if (ControlledPlayer!=null)
	//		ControlledPlayer.OnKeyEvent(this);
	//	//Console.WriteLine("key up, p:"+player.ToString());
	//}

	//private void KeyDown_(IKeyboard keyboard, Key key, int keyCode) {
	//	//logger.Log("key "+key+" down");
	//	switch (key) {
	//		case Key.W:
	//			keyUp=true;
	//			break;
	//		case Key.S:
	//			keyDown=true;
	//			break;
	//		case Key.A:
	//			keyLeft=true;
	//			break;
	//		case Key.D:
	//			keyRight=true;
	//			break;
	//		case Key.Escape:
	//			Stop();
	//			break;
	//	}
	//	if(ControlledPlayer!=null)
	//		ControlledPlayer.OnKeyEvent(this);
	//	//Console.WriteLine("key up, p:"+player.ToString());
	//}

	//private void OnMouseDown(IMouse cursor, MouseButton button) {
	//	Console.WriteLine("Mouse Down! "+mousePos);
	//	if (button==MouseButton.Left) {
	//		mouseLeftDown=true;
	//		if(ControlledPlayer!=null)
	//			ControlledPlayer.IsShooting = 0x1;
	//	}
	//	if (button==MouseButton.Right)
	//		mouseRightDown=true;
	//}

	//private void OnMouseUp(IMouse cursor, MouseButton button) {
	//	Console.WriteLine("Mouse Up! "+mousePos);
	//	if (button==MouseButton.Left) {
	//		mouseLeftDown=false;
	//		if(ControlledPlayer!=null)
	//			ControlledPlayer.IsShooting = 0x0;
	//	}
	//	if (button==MouseButton.Right) {
	//		mouseRightDown=false;
	//	}
	//}

	protected override void MouseMoved(Vector3d pos) {
		double x = pos.X * ( Constants.MAP_GRID_WIDTH / this.Size.Width );
		double y = pos.X * ( Constants.MAP_GRID_WIDTH / this.Size.Width );
		MousePos = new(x, y, 0.0);
		//Console.WriteLine("I Moved! "+mousePos);
	}

	private bool NetHandlerConnected() {
		if (netService != null)
			if (netService.Connected)
				return true;
		return false;
	}

	protected override void Draw(double deltaTime, DrawingContext context) {
		renderService.OnDraw(context);
	}
}