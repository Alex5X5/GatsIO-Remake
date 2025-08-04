using ShGame.Util;

using System.Net;

namespace ShGame.Client;

class ClientMain {
	public static void Main() {
		Logging.DisableColors();
		Paths.ExtractFiles();
		IPAddress? address = IPAddress.Parse("192.168.56.1");
		int port = 5000;
		_ = new Client(
			address, port
		);
	}
}
