﻿namespace ShGame.Game;

using ShGame.Game.Net;

using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;

/// <summary>
/// This class contains the main entry point for the programm.
/// </summary>
public static class Programm {

	[STAThread]
	public static void Main(string[] args) {
		int i = 0;
		Console.WriteLine(i+=1);


		Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
		Util.Paths.ExtractFiles();
        Logging.DisableColors();

		System.Collections.Generic.List<string> args_ = args.ToList<string>();
		bool noGui = args_.Contains("-nogui");

		//start a server if the --server argument is provided
		//otherwise start a client
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

					_ = new Net.GameServer(address, port);

				}
			).Start();
			return;
		} else {
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

		//Application.EnableVisualStyles();
		//Application.SetCompatibleTextRenderingDefault(false);
		//Console.WriteLine("start");
		//Application.Run(new InitialScreen());
	}
}