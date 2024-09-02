using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using ShGame.game.Net;
using System.DirectoryServices;
using System;

namespace ShGame.game;

public static class Programm {

	[STAThread]
	public static void Main() {
		Logging.DisableColors();
		Logging.DisableDebug();
		Logging.DisableLog();
		Console.WriteLine("start");
		Debug.WriteLine("test");
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

		//unsafe {
		//	TempData<PrimitiveVector3I> tempData;
		//	tempData = TempStorageAllocator<PrimitiveVector3I>.Get();
		//	tempData.data->X=1;
		//	Console.WriteLine(tempData.data->ToString());
		//	Console.WriteLine(tempData.data->X);
		//	//return;
		//}

		//unsafe {
		//	Console.WriteLine($"Size of MyStruct: {sizeof(PrimitiveVector3I)} bytes"); // This will output 12 bytes
		//}

		//new Logger(new LoggingLevel("programm")).Log("Main");
		//new GameServer(100);
		//while (true)
		//Thread.Sleep(100);
		//Control.CheckForIllegalCr'ossThreadCalls=true;
		Application.Run(new Client.Client());
	}
}