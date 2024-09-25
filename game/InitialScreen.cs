using ShGame.game.Net;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ShGame.game; 
public partial class InitialScreen : Form {

	private bool portInitialClick=true;
	private bool ipInitialClick=true;

	public InitialScreen() {
		InitializeComponent();
	}

	private void Form_Load(object sender, EventArgs e) {
	}

	private void PortField_Clicked(object sender, EventArgs e) {
			if(portInitialClick){
			portTextBox.Text = "";
			portInitialClick = false;
		}
	}

	private void IpField_Clicked(object sender, EventArgs e) {
		if(ipInitialClick) {
			ipTextBox.Text="";
			ipInitialClick=false;
		}
	}

	private void StartServer(object sender, EventArgs e) {
		new Thread(
				() => {
					GetStartValues(out IPAddress address, out uint port);
					Console.WriteLine("Initial Screen: ip="+address+" port="+port);
					_ = new Net.GameServer(address, (uint)Math.Abs(port));
				}
		).Start();
	}

	private void StartClient(object sender, EventArgs e) {
		new Thread(
				() => {
					GetStartValues(out IPAddress address, out uint port);
					Client.Client c = new(
					   address, port
					);
						Console.WriteLine("Initial Screen: ip="+address+" port="+port);
						c.ShowDialog();
				}
		).Start();
	}

	private void GetStartValues(out IPAddress address, out uint port) {
		int port_ = -1;
		IPAddress? address_ = null;
		try {
			address_ = IPAddress.Parse(ipTextBox.Text);
		} catch {}
		try {
			port_ = Convert.ToInt32(portTextBox.Text);
		} catch {}
		address=address_??GameServer.GetLocalhost();
		port=port_>=0 ? (uint)Math.Abs(port_) : 100;
	}
}
