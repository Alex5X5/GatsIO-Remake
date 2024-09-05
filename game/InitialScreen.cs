using ShGame.game.Net;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ShGame.game {
	public partial class InitialScreen : Form {
		public InitialScreen() {
			InitializeComponent();
		}

		private void Form_Load(object sender, EventArgs e) {
		}

		private void PortField_Clicked(object sender, EventArgs e) => portTextBox.Text = "";

		private void IpField_Clicked(object sender, EventArgs e) {
			ipTextBox.Text = "";
		}

		private void StartServer(object sender, EventArgs e) {
			int port = -1;
			IPAddress? address = null;
			try{
				address = IPAddress.Parse(ipTextBox.Text);
				port = Convert.ToInt32(portTextBox.Text);
				Console.WriteLine("read successfully");
				if(port!=-1&&address!=null){
					Console.WriteLine("Initial Screen: ip="+address+" port="+port);
					Console.WriteLine("starting non localhost server");
					_ = new Net.GameServer(address,port);
				}
				return;
			} catch {
				if(address==null|port==-1) {
					Console.WriteLine("Initial Screen: ip="+address+" port="+port);
					Console.WriteLine("starting localhost server");
					_ = new Net.GameServer();
				}
			}
						//Enabled = false;
						//while (!c.IsDisposed) {
							//Thread.Sleep(1000);
							//Dispose();
						//}
			//new Thread(
			//		() => {
			//			_ = new Net.GameServer();
			//		}
			//).Start();
			//Enabled = false;
			//Dispose();
		}

		private void StartClient(object sender, EventArgs e) {
			new Thread(
					() => {
						int port = -1;
						IPAddress? address = null;
						try{
							address = IPAddress.Parse(ipTextBox.Text);
							port = Convert.ToInt32(portTextBox.Text);
							Console.WriteLine("read successfully");
							if(port!=-1&&address!=null){
								Console.WriteLine("Initial Screen: ip="+address+" port="+port);
								Console.WriteLine("starting non localhost client");
								Client.Client c = new(address, port);
								c.ShowDialog();
							}
							return;
						} catch {
							if(address==null|port==-1) {
								Console.WriteLine("Initial Screen: ip=" + address + " port=" +port);
								Console.WriteLine("starting localhost client");
								Client.Client c = new(address, port);
								c.ShowDialog();
							}
						}
					}
			).Start();
		}
	}
}
