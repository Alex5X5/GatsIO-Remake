using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
namespace ShGame.game;

//public static class Programm {

//	[STAThread]
//	public static void Main() {
//		//_=new ConsoleRedirector();
//		ConsoleRedirector.WriteLine("test");
//        Application.EnableVisualStyles();
//        Application.SetCompatibleTextRenderingDefault(false);
//        Logging.DisableColors();
//        Console.WriteLine("start");

//		//return;

//		//unsafe
//		//{
//		//	Console.WriteLine("test");
//		//	TempData<PrimitiveVector3I> tempData;
//		//	tempData = TempStorageAllocator<PrimitiveVector3I>.Get();
//		//	tempData.data->X = 1;
//		//	Console.WriteLine(tempData);
//		//	Console.WriteLine(tempData.data->ToString());
//		//	Console.WriteLine(tempData.data->X);
//		//	//TempStorageAllocator<PrimitiveVector3I>.Recycle(ref tempData);
//		//	Console.WriteLine(tempData);
//		//	PrimitiveVector3I vec = *tempData.data;
//		//          Console.WriteLine(tempData.data->X);
//		//	TempStorageAllocator<PrimitiveVector3I>.Recycle(ref tempData);
//		//	//return;
//		//}

//		//      unsafe
//		//{
//		//	Console.WriteLine("test");
//		//	TempData<PrimitiveVector3I> tempData;
//		//	tempData = TempStorageAllocator<PrimitiveVector3I>.Get();
//		//	tempData.data->X = 1;
//		//	Console.WriteLine(tempData.data->ToString());
//		//	Console.WriteLine(tempData.data->X);
//		//	//return;
//		//}

//		//unsafe {
//		//	Console.WriteLine($"Size of MyStruct: {sizeof(PrimitiveVector3I)} bytes"); // This will output 12 bytes
//		//}

//		Application.Run(new InitialScreen());
//        //new Logger(new LoggingLevel("programm")).Log("Main");
//        //new GameServer(100);
//		//while (true)
//		//Thread.Sleep(100);
//		//Control.CheckForIllegalCr'ossThreadCalls=true;
//		//Application.Run(new Client.Client());
//	}
//}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Programm
{
	static void Main(string[] args)
	{
		// Define the IP address and port to bind the socket to.
		string ipAddress = "127.0.0.1"; // Replace with your custom IP address
		int port = 8080; // Replace with your desired port

		// Create an endpoint from the provided IP address and port.
		IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

		// Create a new socket.
		Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		try
		{
			// Bind the socket to the specified endpoint.
			listener.Bind(localEndPoint);
			// Start listening with a backlog of 10 connections.
			listener.Listen(10);

			Console.WriteLine($"Socket bound to {ipAddress}:{port}");
			Console.WriteLine("Waiting for a connection...");

			// Accept incoming connections.
			Socket handler = listener.Accept();

			Console.WriteLine("Client connected!");

			// Receive data from the client.
			byte[] buffer = new byte[1024];
			int bytesReceived = handler.Receive(buffer);
			string data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
			Console.WriteLine($"Received data: {data}");

			// Send a response back to the client.
			byte[] response = Encoding.ASCII.GetBytes("Hello from the server!");
			handler.Send(response);

			// Close the handler and listener sockets.
			handler.Shutdown(SocketShutdown.Both);
			handler.Close();
			listener.Close();
		}
		catch (Exception e)
		{
			Console.WriteLine($"An error occurred: {e.Message}");
		}
	}
}