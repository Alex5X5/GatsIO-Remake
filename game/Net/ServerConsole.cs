using ShGame.game.Net;

using System.Threading;
using System.Windows.Forms;

namespace ShGame.game {
	public partial class ServerConsole : Form {

		internal ServerConsole(GameServer gs) {
			InitializeComponent(gs);
		}

		private void Form_Load(object sender, EventArgs e) {
		}

		public void Writeline(string s) {
			string[] lines = OutputArea.Text.Split(['\n']);
			string temp = "";
			for (int i = 1; i<lines.Length; i++)
				temp+=lines[i]+"\n";
			temp+=s;
			Text=temp;
		}
	}
}
