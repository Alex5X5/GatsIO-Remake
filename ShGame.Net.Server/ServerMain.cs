namespace ShGame.Net.Server;

using ShGame.Util;
using System.Net;

public class ServerMain {
    public static void Main(string[] args) {
        Paths.ExtractFiles();
        Logging.DisableColors();
        Configuration config = new Configuration() {
            Address = NetUtil.GetLocalIP().MapToIPv4().ToString(),
            Port = 5000
        };
        var server = new GameServer(config);
        server.StartAcceptLoopAsync().Wait();
    }
}