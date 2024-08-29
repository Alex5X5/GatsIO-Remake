using sh_game.game.client;
using sh_game.game.net;

using SimpleLogging.logging;

//using System.Threading;
using System.Windows.Forms;

namespace sh_game.game {
	internal static class Programm {

		//[STAThread]
		public static void Main() {
			Logging.DisableColors();


			//byte[] temp = new byte[8];
			//new Random().NextBytes(temp);
			//Player player = new Player(new Logic.Vector3d(100, 100, 0), 100, BitConverter.ToInt64(temp, 0)) {
			//	Dir=new Logic.Vector3d(10, 3, 0).Nor()
			//};
			//Console.WriteLine(player.ToString());
			//byte[] buffer = new byte[Player.PLAYER_BYTE_LENGTH];
			//int counter = 0;
			//Player.SerializePlayer(ref buffer, ref player, ref counter);

			//Player p2 = new Player(null, 100, 0);

			//int counter2 = 0;
			//Player.DeserializePlayer(ref buffer, ref p2, ref counter2);
			//Console.WriteLine(p2.ToString());

			//return;
			new Logger(new LoggingLevel("programm")).Log("Main");
			//new GameServer(100);
			//Thread.Sleep(1000);
			Control.CheckForIllegalCrossThreadCalls=true;
			Application.Run(new Client());
		}
	}
}