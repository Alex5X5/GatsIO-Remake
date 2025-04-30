using ShGame.game.Client.Rendering;
using ShGame.game.Net;
using ShGame.game.Util;

using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
namespace ShGame.game;

public static class Programm {

	[STAThread]
    public static void Main(string[] args) {
        Logging.DisableColors();
        //Programm3.Main_();
        //return;
        RendererGl rd = new();
        //Client2 c = new();
        return;

        System.Collections.Generic.List<string> args_ = args.ToList<string>();
        bool noGui = args_.Contains("-nogui");

        if (args_.Contains("--server")) {
            new Thread(
                () => {
                    //Console.WriteLine("Initial Screen: ip="+address+" port="+port);
                    IPAddress? address = null;
                    int port = 1;
                    try {
                        address = IPAddress.Parse(args_.Contains("-ip") ? args_[args_.IndexOf("-ip")+1] : "");
                    } catch {
                        address = GameServer.GetLocalIP().MapToIPv4();
                    }
                    try {
                        port = args_.Contains("-port") ? Convert.ToInt32(args_[args_.IndexOf("-port")+1]): -1;
                    } catch {
                        port = 5000;
                    }
                    
                    _ = new Net.GameServer(address, (uint)port);

                }
            ).Start();
            return;
        }

        if (args_.Contains("--client")) {
            new Thread(
                () => {
                    //Console.WriteLine("Initial Screen: ip="+address+" port="+port);
                    IPAddress? address = null;
                    int port = 1;
                    try {
                        address = IPAddress.Parse(args_.Contains("-ip") ? args_[args_.IndexOf("-ip")+1] : "");
                    } catch {
                        address = GameServer.GetLocalIP().MapToIPv4();
                    }
                    try {
                        port = args_.Contains("-port") ? Convert.ToInt32(args_[args_.IndexOf("-port")+1]) : 5000;
                    } catch {
                        port = 5000;
                    }

                    Client.Client c = new(
                        address, (uint)port
                    );
                    Console.WriteLine("Initial Screen: ip="+address+" port="+port);
                    c.ShowDialog();

                }
            ).Start();
            return;
        }

        Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
        Console.WriteLine("start");
		Application.Run(new InitialScreen());
	}
}