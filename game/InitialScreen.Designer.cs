using Microsoft.VisualBasic.ApplicationServices;

using ShGame.game.Net;

namespace ShGame.game
{

	partial class InitialScreen {
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
        private void InitializeComponent() {
            portTextBox = new System.Windows.Forms.TextBox();
            ipTextBox = new System.Windows.Forms.TextBox();
            serverButton = new System.Windows.Forms.Button();
            clientButton = new System.Windows.Forms.Button();
            startLabel = new System.Windows.Forms.Label();
            ipV4Button = new System.Windows.Forms.RadioButton();
            ipV6Button = new System.Windows.Forms.RadioButton();
            // 
            // ipTextBox
            // 
            ipTextBox.Font = new System.Drawing.Font("Segoe UI", 15F);
            ipTextBox.Location = new System.Drawing.Point(30, 15);
            ipTextBox.Size = new System.Drawing.Size(480, 50);
            ipTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            ipTextBox.Name = "ipTextBox";
            ipTextBox.ShortcutsEnabled = false;
            ipTextBox.TabIndex = 0;
            ipTextBox.TabStop = false;
            ipTextBox.Text = "enter IP (192.168.100.100 by default)";
            ipTextBox.Click += IpField_Clicked;
            SuspendLayout();
            // 
            // portTextBox
            // 
            portTextBox.Font = new System.Drawing.Font("Segoe UI", 15F);
            portTextBox.Location = new System.Drawing.Point(30, 80);
            portTextBox.Size = new System.Drawing.Size(380, 50);
            portTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            portTextBox.Name = "portTextBox";
            portTextBox.TabIndex = 0;
            portTextBox.Text = "enter port (4000 by default)";
            portTextBox.Click += PortField_Clicked;
            // 
            // serverButton
            // 
            serverButton.Font = new System.Drawing.Font("Segoe UI", 15F);
            serverButton.Location = new System.Drawing.Point(430, 80);
            serverButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            serverButton.Name = "serverButton";
            serverButton.Size = new System.Drawing.Size(270, 70);
            serverButton.TabIndex = 1;
            serverButton.Text = "Server";
            serverButton.UseVisualStyleBackColor = true;
            serverButton.Click += StartServer;
            // 
            // clientButton
            // 
            clientButton.Font = new System.Drawing.Font("Segoe UI", 15F);
            clientButton.Location = new System.Drawing.Point(710, 80);
            clientButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            clientButton.Name = "clientButton";
            clientButton.Size = new System.Drawing.Size(276, 72);
            clientButton.TabIndex = 1;
            clientButton.Text = "Client";
            clientButton.UseVisualStyleBackColor = true;
            clientButton.Click += StartClient;
            // 
            // startLabel
            // 
            startLabel.AutoSize = true;
            startLabel.Font = new System.Drawing.Font("Segoe UI", 15F);
            startLabel.Location = new System.Drawing.Point(610, 20);
            startLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            startLabel.Name = "startLabel";
            startLabel.Size = new System.Drawing.Size(184, 41);
            startLabel.TabIndex = 2;
            startLabel.Text = "what to start";
            // 
            // radioButton1
            // 
            ipV4Button.Name = "ipV4Button";
            ipV4Button.TabStop = true;
            ipV4Button.TabIndex = 3;
            ipV4Button.AutoSize = true;
            ipV4Button.UseVisualStyleBackColor = true;
            ipV4Button.Location = new System.Drawing.Point(30, 140);
            ipV4Button.Size = new System.Drawing.Size(140, 30);
            ipV4Button.Font = new System.Drawing.Font("Segoe UI", 12F);
            ipV4Button.Text = "use strict IPV4";
            ipV4Button.Click += UseIpV4;
            // 
            // radioButton2
            //
            ipV6Button.Name = "ipV6Button";
            ipV6Button.TabIndex = 3;
            ipV6Button.TabStop = true;
            ipV6Button.AutoSize = true;
            ipV6Button.Location = new System.Drawing.Point(220, 140);
            ipV6Button.Size = new System.Drawing.Size(140, 30);
            ipV6Button.Font = new System.Drawing.Font("Segoe UI", 12F);
            ipV6Button.Text = "use strict IPV6";
            ipV6Button.UseVisualStyleBackColor = true;
            ipV6Button.Click += UseIpV6;
            // 
            // InitialScreen
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1071, 197);
            Controls.Add(ipV6Button);
            Controls.Add(ipV4Button);
            Controls.Add(startLabel);
            Controls.Add(clientButton);
            Controls.Add(serverButton);
            Controls.Add(ipTextBox);
            Controls.Add(portTextBox);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "InitialScreen";
            Text = "Form1";
            Load += Form_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox portTextBox;
		private System.Windows.Forms.TextBox ipTextBox;
		private System.Windows.Forms.Button serverButton;
		private System.Windows.Forms.Button clientButton;
		private System.Windows.Forms.Label startLabel;
        private System.Windows.Forms.RadioButton ipV4Button;
        private System.Windows.Forms.RadioButton ipV6Button;
    }
}