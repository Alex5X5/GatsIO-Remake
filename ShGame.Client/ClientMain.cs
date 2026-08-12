using ShGame.Util;

using System.Net;

namespace ShGame.Client;

class ClientMain {
	public static void Main() {
		Logging.DisableColors();
		Paths.ExtractFiles();

		Configuration config = new Configuration() {
			Address = NetUtil.GetLocalIP().MapToIPv4().ToString(),
			Port = 5000
		};

		_ = new Client(config);
	}
}
