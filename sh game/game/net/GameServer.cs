using sh_game.game.client;
using sh_game.game.Logic;
using sh_game.game.net.protocoll;
using sh_game.game.server;

using SimpleLogging.logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace sh_game.game.net{
	internal class GameServer:Socket {

		private bool stop = false;
		private readonly Logger logger;
		//private Socket serverSocket;

		public const int MAP_WIDTH = 1000, MAP_HEIGHT = 1000, MAX_PLAYER_COUNT = 10;

		//private TcpClient
		private readonly ServerConnection[] clients = new ServerConnection[MAX_PLAYER_COUNT];
		private readonly Player[] players = new Player[MAX_PLAYER_COUNT];
		private readonly Obstacle[] obstacles = new Obstacle[20];

		internal GameServer(int port) : base(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], port).AddressFamily, SocketType.Stream, ProtocolType.Tcp) {
			logger = new Logger(new LoggingLevel("GameServer"));
			logger.Log("constructor");
			SpreadObstacles();
			//Console.WriteLine("[Server]:constructor");
			logger.Log("binding", new MessageParameter("server",this.ToString()));
			Bind(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], port));
			new Thread(
					() => Run()
			).Start();
		}

		private void OnAccept(Socket s) {
			Console.WriteLine("[Server]:OnAccept("+s.ToString()+")");
			for(int i = 0; i<clients.Length; i++) {
				if(clients[i]==null) {
					clients[i]=new ServerConnection(s.DuplicateAndClose(Process.GetCurrentProcess().Id), this);
					break;
				}
			}
		}

		private async void Run() {
			logger.Log("run");
			while(!stop) {
				Listen(1);
				while(true) {
					try {
						Socket handler = await Task.Factory.FromAsync(this.BeginAccept, this.EndAccept, null);
						_=Task.Run(() => OnAccept(handler));
					} catch(Exception e) {
						Console.WriteLine(e.ToString());
						break;
					}
				}
			}
		}

		internal MapProtocoll OnMapRequest() {
			logger.Log("OnMapRequest");
			return new MapProtocoll(false,obstacles);
		}

		internal PingProtocoll OnPingRequest(Protocoll p) {
			if(p.answer) {
				logger.Log("answering ping");
				return new PingProtocoll(false);
			} else
				logger.Log("not answering ping");
			return null;
		}

		internal PlayerProtocoll OnPlayerRequest(Protocoll p) {
			List<ParsablePlayer> list = new List<ParsablePlayer>();
			foreach(Player pl in players)
				list.Add(new ParsablePlayer(pl));
			return null;
		}

		public void Stop() {
			stop = true;
			Thread.Sleep(2000);
			foreach(ServerConnection c in clients)
				c.Stop();
		}

		private void SpreadObstacles() {
			logger.Log("generating Obstacles");
			int c = 0;
			for(int x = 0; x<5; x++)
				for(int y = 0; y<4; y++) {
					PlaceObstacles(x, y, c);
					c++;
				}
		}

		private void PlaceObstacles(int x, int y, int c) {
			x=MAP_WIDTH/5*x;
			y=MAP_HEIGHT/4*y;
			var r = new Random();
			var t = 5;
			while(t==5)
				t = 1+(int)Math.Floor(r.NextDouble()*4);
			obstacles[c]=new Obstacle(
				new Vector3d(
					Math.Floor(r.NextDouble()*MAP_WIDTH/5)+x,
					Math.Floor(r.NextDouble()*MAP_WIDTH/4)+y, 
					0
				), 
				t
			);
		}
	}
}

