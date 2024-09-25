using System.Threading;
using System.Windows.Forms;
namespace ShGame.game;

public static class Programm {

	//[STAThread]
	public static void Main() {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Logging.DisableColors();
		Console.WriteLine("start");
		Application.Run(new InitialScreen());
	}
}