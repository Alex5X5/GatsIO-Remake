namespace ShGame.Client;

using ShGame.Drawing;
using ShGame.Game;
using ShGame.Math;
using ShGame.Net.Client.Services;
using ShGame.Rendering.Services;
using ShGame.Types;
using ShGame.Util;

using System.Diagnostics;
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
	private ShadowService shadowService;
	private NetworkService netService;
	private	GameService gameService;
	private Logger logger;

	private Player ControlledPlayer;

	private Vector3d MousePos;
	
	public Client(Configuration config) : base(1200, 800, "ShGame") {

		logger=new Logger(new LoggingLevel("Client"));

		netService = new NetworkService(config);

		ControlledPlayer=new(new(100, 100, 0), 100, 1);
		
		gameService = new GameService();
		shadowService = new ShadowService(gameService);
		renderService = new RenderService(gameService, shadowService, ControlledPlayer);

		StartNetworkThread();
		gameService.StartAllLoops();

		SetBackgroundColor(new Color(90, 90, 110));
		Thread.Sleep(1000);

		new Thread(RefreshLoop).Start();
		Show();
	}

	private void RefreshLoop() {
		while (!Closing && !stop) {
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
					gameService.Map = await netService.GetMapAsync();
					ControlledPlayer = await netService.RegisterToServerAsync();
					gameService.Players = await netService.GetPlayersAsync();
					shadowService = new ShadowService(gameService);
					renderService = new(gameService, shadowService, ControlledPlayer);
				} else {
					gameService.Map = new Map();
					gameService.SpreadObstacles();
					gameService.Players[0] = ControlledPlayer;
					gameService.Players[1] = new(new(100, 300, 0), 100, 2);
					shadowService = new ShadowService(gameService);
					renderService = new(gameService, shadowService, ControlledPlayer);
				}
				var sw = new Stopwatch();
				while (!stop && NetHandlerConnected()) {
					sw.Start();
					await netService.UpdatePlayerAsync(gameService.GetPlayerById(ControlledPlayer.PlayerUUID));
					gameService.Players = await netService.GetPlayersAsync();
					Bullet[] bullets = await netService.GetBulletsAsync();
					sw.Stop();
					gameService.MergeBulletSnapshot(bullets, sw.ElapsedTicks / 2.0);
					sw.Reset();
					await Task.Delay(50);
				}
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
				stop = true;
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
		gameService
			.GetPlayerById(ControlledPlayer.PlayerUUID)
			.UpdateDir(keyUp, keyDown, keyLeft, keyRight);
	}

	protected override void MousePressed(Vector3d pos, MouseKey key) {
		Console.WriteLine("Mouse Down! "+pos);
		if (key==MouseKey.Left) {
			gameService.GetPlayerById(ControlledPlayer.PlayerUUID).IsShooting = 0x1;
		}
		Console.WriteLine(gameService.GetPlayerById(ControlledPlayer.PlayerUUID).IsShooting);
	}

	protected override void MouseReleased(Vector3d pos, MouseKey key) {
		Console.WriteLine("Mouse Up! "+pos);
		if (key==MouseKey.Left) {
			gameService.GetPlayerById(ControlledPlayer.PlayerUUID).IsShooting = 0x0;
		}
		Console.WriteLine(gameService.GetPlayerById(ControlledPlayer.PlayerUUID).IsShooting);
	}

	protected override void MouseMoved(Vector3d pos) {
		MousePos = pos;
		var playerPos = gameService.GetPlayerById(ControlledPlayer.PlayerUUID).Pos;
		var relative = pos.Sub(playerPos).Nor();
		gameService.GetPlayerById(ControlledPlayer.PlayerUUID).LookDir = relative;
	}

	private bool NetHandlerConnected() {
		if (netService != null)
			if (netService.Connected)
				return true;
		return false;
	}

	protected override void Load(LoadingContext context) {
		renderService.OnLoad(context);
	}

	protected override void Draw(double deltaTime, DrawingContext context) {
		renderService.OnDraw(context);
		if(Constants.DEBUG_DRAW) {
			var player = gameService.GetPlayerById(ControlledPlayer.PlayerUUID);
			var lp = new Vector3d(100, 100, 0).Add(player.LookDir.Scl(30));
			context.DrawLine(new Vector3d(100,100,0), lp, new Color(49, 245, 39));
		}
		//context.DrawLine(new Vector3d(0, 0, 0), MousePos, Color.ORANGE, 3);
	}
}