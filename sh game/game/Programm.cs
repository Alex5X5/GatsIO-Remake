using sh_game.game.client;
using sh_game.game.net;

using SimpleLogging.logging;


//using sh_game.game.client;

using System;
using System.Net;
using System.Threading;

//using System.Threading;
using System.Windows.Forms;

namespace sh_game.game {
	internal static class Programm {

		//[STAThread]
		public static void Main() {
			Logging.DisableColors();
			Logging.SetStartTime();
			new Logger(new LoggingLevel("programm")).Log("Main");
			new GameServer(100);
			Thread.Sleep(1000);
			//Control.CheckForIllegalCrossThreadCalls=false;
			Application.Run(new Client());
		}
	}
}