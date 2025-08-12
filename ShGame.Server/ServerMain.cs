namespace ShGame.Net.Server;

using ShGame.Util;
using System.Net;

public class ServerMain {
    public static void Main(string[] args) {
        Paths.ExtractFiles();
        Logging.DisableColors();
        IPAddress? address = NetUtil.GetLocalIP().MapToIPv4();
        int port = 5000;
        _ = new GameServer(address, (uint)port);
    }
}