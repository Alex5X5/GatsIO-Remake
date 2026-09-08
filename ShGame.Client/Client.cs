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

public class Client : Window {

	private bool stop;
	public bool
		keyUp = false,
		keyDown = false,
		keyLeft = false,
		keyRight = false;

	public static readonly int SCREEN_PIXEL_WIDTH = Silk.NET.Windowing.Monitor.GetMainMonitor(null).Bounds.Size.Y;
	public static readonly int SCREEN_PIXEL_HEIGHT = Silk.NET.Windowing.Monitor.GetMainMonitor(null).Bounds.Size.X;

	private RenderService renderService;
	private NetworkService netService;
	private	GameService gameService;
	private Logger logger;

	private Player ControlledPlayer;

	private Vector3d MousePos;
	
	public Client(Configuration config) : base(1200, 800, "ShGame") {
		
		logger=new Logger(new LoggingLevel("Client"));

		netService = new NetworkService(config);

		ControlledPlayer=new(new(100, 100, 0), 100, 1);
		gameService = new(ControlledPlayer);

		renderService = new RenderService(gameService, ControlledPlayer);

		StartNetworkThread();
		gameService.StartAllLoops();

		SetBackgroundColor(new Color(90, 90, 110));
		Thread.Sleep(1000);

		new Thread(RefreshLoop).Start();
		Show();
	}

	private void RefreshLoop() {
		while (!Closing) {
			InvalidateVisual();
			Thread.Sleep(50);
		}
	}

	protected override void OnClosing() {
		stop = true;
		gameService.Stop();
		netService.Dispose();
	}

	private void StartNetworkThread() {
		logger.Log("start threads!");
		var NetworkThread = new Thread(
			async () => {
				if (NetHandlerConnected()) {
					var map = await netService.GetMapAsync();
					if (map != null)
						gameService.Map = map;
					else
						gameService.Map = new Map();
					ControlledPlayer = await netService.RegisterToServerAsync();
				}
				while (!stop && NetHandlerConnected()) {
					//logger.Log("asking for players");

					for (int i = 0; i<Constants.PLAYER_COUNT; i++)
						if (gameService.Players[i].PlayerUUID == ControlledPlayer.PlayerUUID)
							await netService.UpdatePlayerAsync(gameService.Players[i]);
					gameService.Players = await netService.GetPlayersAsync();
					await Task.Delay(50);
				}
				netService?.Dispose();
			}
		);
		NetworkThread.Start();
		logger.Log("started connection thread:"+NetworkThread.IsAlive);
	}

	protected override void KeyPressed(Key key) {
		//logger.Log("key "+key+" up");
		switch (key) {
			case Key.W:
				keyUp=true;
				break;
			case Key.S:
				keyDown=true;
				break;
			case Key.A:
				keyLeft=true;
				break;
			case Key.D:
				keyRight=true;
				break;
			case Key.Escape:
				Close();
				Dispose();
				break;
		}
		UpdateControlledDir();
	}

	protected override void KeyReleased(Key key) {
		//logger.Log("key "+key+" down");
		switch (key) {
			case Key.W:
				keyUp=false;
				break;
			case Key.S:
				keyDown=false;
				break;
			case Key.A:
				keyLeft=false;
				break;
			case Key.D:
				keyRight=false;
				break;
		}
		UpdateControlledDir();
		//Console.WriteLine("key up, p:"+player.ToString());
	}

	private void UpdateControlledDir() {
		for (int i = 0; i<Constants.PLAYER_COUNT; i++)
			if (gameService.Players[i].PlayerUUID == ControlledPlayer.PlayerUUID)
				gameService.Players[i].UpdateDir(keyUp, keyDown, keyLeft, keyRight);
	}

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
		MousePos = pos;
		InvalidateVisual();
	}

	private bool NetHandlerConnected() {
		if (netService != null)
			if (netService.Connected)
				return true;
		return false;
	}

	protected override void Draw(double deltaTime, DrawingContext context) {
		renderService.OnDraw(context);
		context.DrawLine(new Vector3d(0, 0, 0), MousePos, Color.ORANGE, 3);
	}
}