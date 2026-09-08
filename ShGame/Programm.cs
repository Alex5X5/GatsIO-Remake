namespace ShGame.Start;

using ShGame.Client;
using ShGame.Net.Server;
using ShGame.Util;
using SimpleLogging.logging;
using System;
using System.Linq;
using System.Net;
using System.Threading;

/// <summary>
/// This class contains the main entry point for the programm.
/// </summary>
public static class Programm {

	private static Configuration ExtractStartupConfig(string[] args) {
		string address = "";
		int port = 1;

		if (args.Contains("-ip")) {
			address =  args[args.IndexOf("-ip")+1];
		} else {
			address = NetUtil.GetLocalIP().MapToIPv4().ToString();
		}

		try {
			port = args.Contains("-port") ? Convert.ToInt32(args[args.IndexOf("-port")+1]) : 5000;
		} catch {
			port = 5000;
		}

		return new Configuration() {
			Address = address,
			Port = port,
		};
	}

	[STAThread]
	public static void Main(string[] args) {
		Paths.ExtractFiles();
		Logging.DisableColors();
		//Logging.SetStartTime();

		System.Collections.Generic.List<string> args_ = args.ToList();
		bool noGui = args_.Contains("-nogui");

		//start a server if the --server argument is provided
		//otherwise start a client
		if (args_.Contains("--server")) {
			Configuration configuration = ExtractStartupConfig(args);
			var server = new GameServer(configuration);
			server.StartAcceptLoopAsync().Wait();
		} else {
			Configuration configuration = ExtractStartupConfig(args);
			_ = new Client(configuration);
		}
	}
}