using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
namespace ShGame.game;

public static class Programm {

	[STAThread]
	public static void Main() {
		//_=new ConsoleRedirector();
		ConsoleRedirector.WriteLine("test");
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Logging.DisableColors();
        Console.WriteLine("start");

		//return;

		//unsafe
		//{
		//	Console.WriteLine("test");
		//	TempData<PrimitiveVector3I> tempData;
		//	tempData = TempStorageAllocator<PrimitiveVector3I>.Get();
		//	tempData.data->X = 1;
		//	Console.WriteLine(tempData);
		//	Console.WriteLine(tempData.data->ToString());
		//	Console.WriteLine(tempData.data->X);
		//	//TempStorageAllocator<PrimitiveVector3I>.Recycle(ref tempData);
		//	Console.WriteLine(tempData);
		//	PrimitiveVector3I vec = *tempData.data;
		//          Console.WriteLine(tempData.data->X);
		//	TempStorageAllocator<PrimitiveVector3I>.Recycle(ref tempData);
		//	//return;
		//}

		//      unsafe
		//{
		//	Console.WriteLine("test");
		//	TempData<PrimitiveVector3I> tempData;
		//	tempData = TempStorageAllocator<PrimitiveVector3I>.Get();
		//	tempData.data->X = 1;
		//	Console.WriteLine(tempData.data->ToString());
		//	Console.WriteLine(tempData.data->X);
		//	//return;
		//}

		//unsafe {
		//	Console.WriteLine($"Size of MyStruct: {sizeof(PrimitiveVector3I)} bytes"); // This will output 12 bytes
		//}

		Application.Run(new InitialScreen());
        //new Logger(new LoggingLevel("programm")).Log("Main");
        //new GameServer(100);
		//while (true)
		//Thread.Sleep(100);
		//Control.CheckForIllegalCr'ossThreadCalls=true;
		//Application.Run(new Client.Client());
	}
}