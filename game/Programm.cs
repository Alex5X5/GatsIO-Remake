using ShGame.game.Client.Rendering;
using ShGame.game.Net;
using ShGame.game.Util;

using System.Linq;
using System.Net;
using System.Threading;
namespace ShGame.game;

/// <summary>
/// This class contains the main entry point for the programm.
/// </summary>
public static class Programm {

	[STAThread]
	public static void Main(string[] args) {
		
		Logging.DisableColors();

		System.Collections.Generic.List<string> args_ = args.ToList<string>();
		bool noGui = args_.Contains("-nogui");

		if (args_.Contains("--server")) {
			new Thread(
				() => {
					//Console.WriteLine("Initial Screen: ip="+address+" port="+port);
					IPAddress? address = null;
					int port = 1;
					try {
						address = IPAddress.Parse(args_.Contains("-ip") ? args_[args_.IndexOf("-ip")+1] : "");
					} catch {
						address = GameServer.GetLocalIP().MapToIPv4();
					}
					try {
						port = args_.Contains("-port") ? Convert.ToInt32(args_[args_.IndexOf("-port")+1]) : 5000;
					} catch {
						port = 5000;
					}

					GameServer server = new Net.GameServer(address, port);

				}
			).Start();
			return;
		}

		if (args_.Contains("--client")) {
			new Thread(
				() => {
					IPAddress? address = null;
					int port = 1;
					try {
						address = IPAddress.Parse(args_.Contains("-ip") ? args_[args_.IndexOf("-ip")+1] : "");
					} catch {
						address = GameServer.GetLocalIP().MapToIPv4();
					}
					try {
						port = args_.Contains("-port") ? Convert.ToInt32(args_[args_.IndexOf("-port")+1]) : 5000;
					} catch {
						port = 5000;
					}

					Client.Client c = new(
						address, port
					);
					Console.WriteLine("Initial Screen: ip="+address+" port="+port);
				}
			).Start();
			return;
		}
	}
}