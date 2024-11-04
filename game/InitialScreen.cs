using ShGame.game.Net;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ShGame.game; 
public partial class InitialScreen : Form {

	private bool portInitialClick = true;
	private bool ipInitialClick = true;
	private IpVersion ipVersion = IpVersion.DeviceDefault;

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

	private void UseIpV4(object sender, EventArgs e) {
		ipV6Button.Checked = false;
        ipV4Button.Checked = !ipV4Button.Checked;
		ipVersion = ipV4Button.Checked ? IpVersion.V4 : IpVersion.DeviceDefault;
    }

	private void UseIpV6(object sender, EventArgs e) {
		ipV4Button.Checked = false;
		ipV6Button.Checked = !ipV6Button.Checked;
        ipVersion = ipV6Button.Checked ? IpVersion.V6 : IpVersion.DeviceDefault;
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
        address = ipVersion switch {
            IpVersion.V4 => address_!=null ? address_.MapToIPv4() : GameServer.GetLocalIP().MapToIPv4(),
            IpVersion.V6 => address_!=null ? address_.MapToIPv6() : GameServer.GetLocalIP().MapToIPv6(),
            _ => GameServer.GetLocalIP().MapToIPv4()
        };
        port=port_>=0 ? (uint)Math.Abs(port_) : 4000;
    }

    public enum IpVersion {
        V4, V6, DeviceDefault
    }
}
