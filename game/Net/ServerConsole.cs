using ShGame.game.Net;

using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace ShGame.game {
    public unsafe partial class ServerConsole : Form {

        private readonly Label* Outputarea;

        //private String[] Lines;
        private bool stop = false;

        internal ServerConsole(GameServer gs) {
            InitializeComponent(gs);
            //Lines = new String[10];
            //Console.WriteLine("text:"+OutputArea.Text);
            StartThreads();
        }

        private void Form_Load(object sender, EventArgs e) {
        }

        private void StartThreads() {
            new Thread(
                    () => {
                        //File f = new("out.txt");
                        Console.SetOut(new ConsoleStream(Outputarea));
                        StreamWriter writer = new StreamWriter("out.txt");
                        Console.SetOut(writer);
                        //while(writer.)                        
                        //Console.SetOut(reader);
                        //Console.Out;

                        while (!stop==!Disposing) {
                            break;
                        }
                    }
            ).Start();
        }

        public void WriteLine(string s) {
            if (IsHandleCreated)
                Invoke(new MessageDelegate(DoSendMessage), [s]);
        }

        private delegate void MessageDelegate(string s);

        private void DoSendMessage(string s) {
            string[] lines = OutputArea.Text.Split("\n");
            string temp = "";
            for (int i = 1; i<lines.Length; i++)
                temp+=lines[i]+"\n";
            temp+=s;
            OutputArea.Text=temp;
            //richTextBox2.Text = lines[i]+"\n"+lines[2]+"\n"+s;
        }
        private unsafe class ConsoleStream : TextWriter {

            private Label* label;
            public override Encoding Encoding => Encoding.UTF8;

            public ConsoleStream(Label* label_) {
                label = label_;
            }

            public override void Write(char value) {
                if (label->InvokeRequired) {
                    label->BeginInvoke(new Action<char>(Write), value);
                } else {
                    label->Text+=value;
                }
            }

            public override void WriteLine(string? s) {
                if (label->InvokeRequired) {
                    label->BeginInvoke(new Action<char>(Write), s);
                } else {
                    label->Text+=s+Environment.NewLine;
                }
            }
        }
    }
}
