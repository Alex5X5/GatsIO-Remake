﻿using ShGame.game.Net;
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
			new Thread(
					() => {
						int port = -1;
						try {
							port = Convert.ToInt32(portTextBox.Text);
							Console.WriteLine("read successfully");
							//if (port!=-1) {
								Console.WriteLine("Initial Screen: port="+port);
								_ = new Net.GameServer(port);
							//}
						} catch {
								Console.WriteLine("Initial Screen: port="+port);
								_ = new Net.GameServer();
						}
					}
			).Start();
		}

		private void StartClient(object sender, EventArgs e) {
			new Thread(
					() => {
						int port = -1;
						IPAddress? address = null;
						try {
							address = IPAddress.Parse(ipTextBox.Text);
						} catch {}
                        try {
                            port = Convert.ToInt32(portTextBox.Text);
                        } catch {}
                        Client.Client c = new(
							address!=null ? address : GameServer.GetLocalhost(),
							port>=0 ? port : 100
						);
                        Console.WriteLine("Initial Screen: ip="+address+" port="+port);
                        c.ShowDialog();
					}
			).Start();
		}
	}
}
