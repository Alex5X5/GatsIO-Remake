using System.Threading;
using System.Windows.Forms;

namespace ShGame.game {
    public partial class InitialScreen : Form {
        public InitialScreen() {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e) {
            startLabel.Focus();
            Console.WriteLine(ActiveControl);
		}

        private void PortField_Clicked(object sender, EventArgs e) => portTextBox.Text = "";

        private void IpField_Clicked(object sender, EventArgs e) {
            ipTextBox.Text = "";
		}

        private void StartServer(object sender, EventArgs e) {
            new Thread(
                    () => {
                        _ = new Net.GameServer();
                    }
            ).Start();
            //Enabled = false;
            //Dispose();
        }

        private void StartClient(object sender, EventArgs e) {
            new Thread(
                    () => {
                        Client.Client c = new();
                        c.ShowDialog();
                        //Enabled = false;
                        //while (!c.IsDisposed) {
                            //Thread.Sleep(1000);
                            //Dispose();
                        //}
                    }
            ).Start();
        }
    }
}
