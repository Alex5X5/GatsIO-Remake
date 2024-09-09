using System.Linq;
using System.Net;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ShGame.game;

public static class Programm {

	[STAThread]
	public static void Main() {



		//foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
		//{
		//	if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
		//	{
		//		//Console.WriteLine(ni.Name);
		//		Console.WriteLine(ni.GetIPProperties().UnicastAddresses[ni.GetIPProperties().UnicastAddresses.Count-1]);
		//		//foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
		//		//{
		//		//	if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
		//		//	{
		//		//		Console.WriteLine(ip.Address.ToString());
		//		//	}
		//		//}
		//	}
		//}
		NetworkInterface ni = NetworkInterface.GetAllNetworkInterfaces()[1];
		Console.WriteLine(ni.GetIPProperties().UnicastAddresses[ni.GetIPProperties().UnicastAddresses.Count-1].Address);

		Console.WriteLine(Dns.GetHostEntry("srhk.srh.de").AddressList[1]);

		//_=new ConsoleRedirector();
		ConsoleRedirector.WriteLine("test");
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Logging.DisableColors();
		Console.WriteLine("start");
		Application.Run(new InitialScreen());

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
		//		  Console.WriteLine(tempData.data->X);
		//	TempStorageAllocator<PrimitiveVector3I>.Recycle(ref tempData);
		//	//return;
		//}
	}
}