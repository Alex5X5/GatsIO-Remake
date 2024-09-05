using System.Net;
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
            _ = new Thread(
                    () => {
                        _=new Net.GameServer();
                        //return;
                        //try {
                        //    IPAddress address = IPAddress.Parse(ipTextBox.Text);
                        //    int port = Convert.ToInt32(portTextBox.Text);
                        //    _ = new Net.GameServer(address, port);
                        //} catch {
                        //    _ = new Net.GameServer();
                        //}
                        //Enabled = false;
                        //while (!c.IsDisposed) {
                        //Thread.Sleep(1000);
                        //Dispose();
                        //}
                    }
            );
            //.Start();
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
                        Client.Client cl = new Client.Client(null,-1);
                        cl.ShowDialog();
                        return;
                    try{
                        IPAddress address = IPAddress.Parse(ipTextBox.Text);
                        int port = Convert.ToInt32(portTextBox.Text);
                        Client.Client c = new(address, port);
                        c.ShowDialog();
                    } catch { 
                        Client.Client c = new(IPAddress.None,-1);
                        c.ShowDialog();
                    }
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
