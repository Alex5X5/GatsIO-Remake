using ShGame.game.Util;
using System.Threading;
using System.Windows.Forms;
namespace ShGame.game;

public static class Programm {

	//[STAThread]
	public static void Main() {
		//BareboneTriangle.Main_();
		_=new RendererGl();
		return;
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Logging.DisableColors();
		Console.WriteLine("start");
		Application.Run(new InitialScreen());
	}
}