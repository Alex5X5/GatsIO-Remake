using ShGame.game.Client.Rendering;
using ShGame.game.Net;
using ShGame.game.Util;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using static ShGame.game.InitialScreen;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace ShGame.game;

public static class Programm {

	[STAThread]
	public static void Main(string[] args) {
		//List<string> args_ = args.ToList<string>();
		//bool noGui = args_.Contains("-nogui");
		//if (args_.Contains("--server")) {
		//		new Thread(
		//			() => {
  //                      //Console.WriteLine("Initial Screen: ip="+address+" port="+port);
  //                      IPAddress? address = null;
  //                      try {
  //                          address = IPAddress.Parse(args_.Contains("-nogui") ? args_[args_.IndexOf("-ip")+1]);
  //                      } catch { }
  //                      try {
  //                          port_ = Convert.ToInt32(portTextBox.Text);
  //                      } catch { }
  //                      address = ipVersion switch {
  //                          IpVersion.V4 => address_!=null ? address_.MapToIPv4() : GameServer.GetLocalIP().MapToIPv4(),
  //                          IpVersion.V6 => address_!=null ? address_.MapToIPv6() : GameServer.GetLocalIP().MapToIPv6(),
  //                          _ => GameServer.GetLocalIP()
  //                      };
  //                      port=port_>=0 ? (uint)Math.Abs(port_) : 5000;
  //                      _ = new Net.GameServer(address, (uint)Math.Abs(port));

		//			}
		//		).Start();
  //          if (args_.Contains("-ip")) {
				
		//	}

		//}

        //Programm3.Main_();
        //return;
        Logging.DisableColors();
		//RendererGl rd = new();
		//Client2 c = new();
		//return;
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		//Logging.DisableLog();
		Console.WriteLine("start");
		Application.Run(new InitialScreen());
	}
}