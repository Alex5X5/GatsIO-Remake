namespace ShGame.Start;

using Microsoft.Extensions.DependencyInjection;

using ShGame.Client;
using ShGame.Client.Rendering;
using ShGame.Game.Services;
using ShGame.Net.Server;
using ShGame.Util;
using ShGame.Util.Services;

using SimpleLogging.logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

/// <summary>
/// This class contains the main entry point for the programm.
/// </summary>
public static class Programm {

	[STAThread]
	public static void Main(string[] args) {

		Paths.ExtractFiles();
		Logging.DisableColors();


		IServiceCollection serviceCollection = new ServiceCollection();

		ConfigurationService configurationService = new(args);
		serviceCollection.AddSingleton(configurationService);
		serviceCollection.AddTransient<GameService, GameService>();

		if (configurationService.StartServer) {
			serviceCollection.AddTransient<GameServer, GameServer>();
		}
		if (configurationService.StartClient) {
			serviceCollection.AddTransient<RendererGl, RendererGl>();
		}
	}
}