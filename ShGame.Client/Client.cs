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

	private RenderService renderService;
	private NetworkService netService;
	private	GameService gameService;
	private Logger logger;

	private Player ControlledPlayer;

	private Vector3d MousePos;

	private Image fogImage;
	
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
			Thread.Sleep(20);
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

	protected override void MousePressed(Vector3d pos, MouseKey key) {
		Console.WriteLine("Mouse Down! "+pos);
		if (key==MouseKey.Left) {
			//mouseLeftDown=true;
			if (ControlledPlayer!=null)
				ControlledPlayer.IsShooting = 0x1;
		}
		//if (key==MouseKey.Right)
		//	mouseRightDown=true;
	}

	protected override void MouseReleased(Vector3d pos, MouseKey key) {
		Console.WriteLine("Mouse Up! "+pos);
		if (key==MouseKey.Left) {
			//mouseLeftDown=false;
			if (ControlledPlayer!=null)
				ControlledPlayer.IsShooting = 0x0;
		}
		//if (button==MouseButton.Right)
			//mouseRightDown=false;
	}

	protected override void MouseMoved(Vector3d pos) {
		Console.WriteLine(pos);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++)
			if (gameService.Players[i].PlayerUUID == ControlledPlayer.PlayerUUID) {
				var player = gameService.Players[i];
				var playerPos = player.Pos;
				var relative = pos.Sub(playerPos).Nor();
				gameService.Players[i].LookDir = relative;
			}
	}

	private bool NetHandlerConnected() {
		if (netService != null)
			if (netService.Connected)
				return true;
		return false;
	}

	protected override void Load(LoadingContext context) {
		fogImage = context.LoadImage(Paths.AssetsPath("shadow.png"));
	}

	protected override void Draw(double deltaTime, DrawingContext context) {
		renderService.OnDraw(context);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++)
			if (gameService.Players[i].PlayerUUID == ControlledPlayer.PlayerUUID) {
				var player = gameService.Players[i];
				var lp = new Vector3d(100, 100, 0).Add(player.LookDir.Scl(30));
				context.DrawLine(new Vector3d(100,100,0), lp, new Color(49, 245, 39));
			}
		context.DrawLine(new Vector3d(0, 0, 0), MousePos, Color.ORANGE, 3);
	}
}