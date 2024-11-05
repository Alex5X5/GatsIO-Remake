using ShGame.game.Net;

using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShGame.game {
	public unsafe partial class ServerConsole : Form {

		internal ServerConsole(GameServer gs) {
			InitializeComponent(gs);
			StartThreads();
		}

		private void Form_Load(object sender, EventArgs e) {
		}

		private void StartThreads() {
			new Thread(
					() => {
						//File f = new("out.txt");
						Console.SetOut(
								new ConsoleWriter {
									OnWrite =
										(s) => Write(s.ToString()),
									OnWriteLine =
										(s) => WriteLine(s.ToString())
								}
						);
					}
			).Start();
		}

        public void Write(string s) {
            if (InvokeRequired) {
                Invoke(WriteLine, s);
            } else {
                string[] lines = OutputArea.Text.Split("\n");
                string temp = "";
                for (int i = 1; i<lines.Length; i++)
                    temp+=lines[i]+"\n";
                temp+=s;
                OutputArea.Text=temp;
            }
        }

        public void WriteLine(string s) {
			if(Disposing&&!IsDisposed)
				if (InvokeRequired) {
					//if the thread that called this method isn't the thread that created the console, make the ServerConsole call this method
					Invoke(WriteLine, s);
				} else {
					string[] lines = OutputArea.Text.Split("\n");
					string temp = "";
					for (int i = 1; i<lines.Length; i++)
						temp+=lines[i]+"\n";
					temp+=s;
					OutputArea.Text=temp;
				}
	   }

		private delegate void MessageDelegate(string s);
	}

	public class ConsoleWriter : TextWriter {

		public required Action<string> OnWriteLine;
		public required Action<char> OnWrite;

		public override Encoding Encoding => Encoding.UTF8;

		public override void Write(char c) {
			OnWrite(c);
			using (Stream s = Console.OpenStandardOutput())
				using(StreamWriter w = new(s)){
					w.Write(c);
					Console.WriteLine(c.ToString());
				}
			Console.SetOut(this);
		}

		public override void WriteLine(string? st) {
			OnWriteLine(st??"");
            using (Stream s = Console.OpenStandardOutput())
				using (StreamWriter w = new(s))
					w.WriteLine(st);
			Console.SetOut(this);
        }

		public override void Close() {
			base.Close();
		}
	}
}
