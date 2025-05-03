using ShGame.game.Net;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ShGame.game; 
public partial class InitialScreen : Form {

	private bool portInitialClick = true;
	private bool ipInitialClick = true;
	private bool ipV4ButtonPressed_ = false;
    private bool ipV6ButtonPressed_ = false;
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
		//make sure that only inV4Button was clicked
		if (sender!=ipV4Button)
            return;
        //uncheck the other button 
        ipV6Button.Checked = false;
		//if the button was clicked before, uncheck it
		if (ipV4ButtonPressed_) {
			ipVersion = IpVersion.DeviceDefault;
            ipTextBox.Text = "enter IP (localhost is "+GameServer.GetLocalIP()+")";
            //reset the button
            ipV4Button.Checked = false;
			ipV4ButtonPressed_ = false;
			ipV6ButtonPressed_ = false;
		} else {
			//set the ip protocoll version to be used to ip v4
			ipTextBox.Text = "enter IP (localhost is "+GameServer.GetLocalIPv4()+")";
            ipVersion = IpVersion.V4;
			ipV4ButtonPressed_ = true;
		}
    }

	private void UseIpV6(object sender, EventArgs e) {
        //make sure that only inV6Button was clicked
        if (sender!=ipV6Button)
			return;
		//uncheck the other button
        ipV4Button.Checked = false;
        //if the button was clicked before, uncheck it
        if (ipV6ButtonPressed_) {
            ipVersion = IpVersion.DeviceDefault;
            ipTextBox.Text = "enter IP (localhost is "+GameServer.GetLocalIP()+")";
            //reset the button
            ipV6Button.Checked = false;
            ipV6ButtonPressed_ = false;
			ipV4ButtonPressed_ = false;
        } else {
            //set the ip protocoll version to be used to ip v6
            ipTextBox.Text = "enter IP (localhost is "+GameServer.GetLocalIPv6()+")";
            ipVersion = IpVersion.V6;
            ipV6ButtonPressed_ = true;
        }
    }

	private void StartServer(object sender, EventArgs e) {
		new Thread(
				() => {
					GetStartValues(out IPAddress address, out int port);
					Console.WriteLine("Initial Screen: ip="+address+" port="+port);
					_ = new Net.GameServer(address, (int)Math.Abs(port));
				}
		).Start();
	}

	private void StartClient(object sender, EventArgs e) {
		new Thread(
				() => {
					GetStartValues(out IPAddress address, out int port);
					Client.Client2 c = new(
						address, port
					);
					Console.WriteLine("Initial Screen: ip="+address+" port="+port);
					//c.ShowDialog();
				}
		).Start();
	}

	private void GetStartValues(out IPAddress address, out int port) {
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
            _ => GameServer.GetLocalIP()
        };
        port=port_>=0 ? (int)Math.Abs(port_) : 5000;
    }

    public enum IpVersion {
        V4, V6, DeviceDefault
    }
}
