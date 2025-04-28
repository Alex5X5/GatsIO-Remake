using ShGame.game.Client.Rendering;
using ShGame.game.Util;
using System.Threading;
using System.Windows.Forms;
namespace ShGame.game;

public static class Programm {

	[STAThread]
	public static void Main() {
		//Programm3.Main_();
		//return;
		Logging.DisableColors();
		//RendererGl rd = new();
		Client2 c = new();
		return;
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		//Logging.DisableLog();
		Console.WriteLine("start");
		Application.Run(new InitialScreen());
	}
}