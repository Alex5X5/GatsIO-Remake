using ShGame.game.Util;
using System.Threading;
using System.Windows.Forms;
namespace ShGame.game;

public static class Programm {

	[STAThread]
	public static void Main() {
		//Programm3.Main_();
		//return;
		//RendererGl rd = new();
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Logging.DisableColors();
		//Logging.DisableLog();
		Console.WriteLine("start");
		Application.Run(new InitialScreen());
	}
}