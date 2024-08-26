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


			byte[] data = new byte[6];
			Array.Copy(BitConverter.GetBytes((int)-1), 0, data, 0, 4);
			
			Console.WriteLine(Convert.ToString(BitConverter.ToInt32(data,0)));
			Logging.DisableColors();
			Logging.SetStartTime();
			new Logger(new LoggingLevel("programm")).Log("Main");
			new Logger(new LoggingLevel("abc")).Log("bla");
			new GameServer(100);
			Thread.Sleep(1000);
			//Control.CheckForIllegalCrossThreadCalls=false;
			Application.Run(new Client());
		}
	}
}