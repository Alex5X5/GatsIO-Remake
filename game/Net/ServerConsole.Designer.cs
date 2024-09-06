using ShGame.game.Net;

using System.Net;
using System.Windows.Forms;

namespace ShGame.game
{

	partial class ServerConsole {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected void Dispose() {
			if ((components != null)) {
				components.Dispose();
			}
			base.Dispose(true);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent(GameServer gs) {
			OutputArea = new System.Windows.Forms.Label();
			SuspendLayout();
			// 
			// OutputArea
			// 
			OutputArea.BackColor = System.Drawing.SystemColors.ControlLight;
			OutputArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			OutputArea.Font = new System.Drawing.Font("Segoe UI", 12F);
			OutputArea.Location = new System.Drawing.Point(15, 15);
			OutputArea.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			OutputArea.Name = "OutputArea";
			OutputArea.Size = new System.Drawing.Size(970, 880);
			OutputArea.TabIndex = 2;
			OutputArea.Text = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\na";
			// 
			// ServerConsole
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1000, 1000);
			Controls.Add(OutputArea);
			Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			Name = "ServerConsole";
			Text = "ServerConsole";
			Load += Form_Load;
			//
			//Others
			//
			initializeCallbacks(gs);
			ResumeLayout(false);
		}

		#endregion

		private void initializeCallbacks(GameServer gs) {
			FormClosed+= 
					(object sender, FormClosedEventArgs e) => {
						gs.Stop();
					};
		}

		private System.Windows.Forms.Label OutputArea;
	}
}