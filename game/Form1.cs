using ShGame.game.Net;
using System.Drawing.Printing;
using System.Threading;
using System.Windows.Forms;

namespace ShGame.game
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e) {
            startLabel.Focus();
            Console.WriteLine(ActiveControl);
		}

        private void PortField_Changed(object sender, EventArgs e) {
            
        }

        private void IpField_Changed(object sender, EventArgs e) {
            
        }

        private void PortField_Clicked(object sender, EventArgs e) {
            portTextBox.Text = "";
        }

        private void PortField_FocusLost(object sender, EventArgs e) {
            
        }

        private void IpField_Clicked(object sender, EventArgs e) {
            ipTextBox.Text = "";
		}

        private void StartServer(object sender, EventArgs e) {
            _ = new Net.GameServer();
            Parent?.Dispose();
        }

        private void StartClient(object sender, EventArgs e) {
			Parent?.Dispose();
            Client.Client c = new Client.Client();
            c.ShowDialog();
            //_ = new Client.Client();
		}
    }
}
