using System.Net;

namespace ShGame.game
{

    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
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
            SuspendLayout();
            // 
            // textBox1
            // 
            portTextBox.Font = new System.Drawing.Font("Segoe UI", 15F);
            portTextBox.Location = new System.Drawing.Point(19, 61);
            portTextBox.Name = "textBox1";
            portTextBox.Size = new System.Drawing.Size(271, 34);
            portTextBox.TabIndex = 0;
            portTextBox.Text = "enter port (100 by default)";
            portTextBox.TextChanged += PortField_Changed;
			portTextBox.Click+=PortField_Clicked;
            // 
            // textBox2
            //
            ipTextBox.Font = new System.Drawing.Font("Segoe UI", 15F);
            ipTextBox.Location = new System.Drawing.Point(19, 12);
            ipTextBox.Name = "textBox2";
            ipTextBox.ShortcutsEnabled = false;
            ipTextBox.Size = new System.Drawing.Size(395, 34);
            ipTextBox.TabIndex = 0;
            ipTextBox.TabStop = false;
            ipTextBox.Text = "enter IP (leave empty to use "+Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString()+")";
			ipTextBox.TextChanged += IpField_Changed;
            ipTextBox.Click += IpField_Clicked;
			// 
			// button1
			// 
			serverButton.Font = new System.Drawing.Font("Segoe UI", 15F);
            serverButton.Location = new System.Drawing.Point(296, 52);
            serverButton.Name = "button1";
            serverButton.Size = new System.Drawing.Size(187, 43);
            serverButton.TabIndex = 1;
            serverButton.Text = "Server";
            serverButton.UseVisualStyleBackColor = true;
            serverButton.Click += StartServer;
            // 
            // button2
            // 
            clientButton.Font = new System.Drawing.Font("Segoe UI", 15F);
            clientButton.Location = new System.Drawing.Point(499, 52);
            //clientButton.Name = "button2";
            clientButton.Size = new System.Drawing.Size(193, 43);
            clientButton.TabIndex = 1;
            clientButton.Text = "Client";
            clientButton.UseVisualStyleBackColor = true;
            clientButton.Click += StartClient;
            // 
            // label1
            // 
            startLabel.AutoSize = true;
            startLabel.Font = new System.Drawing.Font("Segoe UI", 15F);
            startLabel.Location = new System.Drawing.Point(429, 15);
            startLabel.Name = "label1";
            startLabel.Size = new System.Drawing.Size(122, 28);
            startLabel.TabIndex = 2;
            startLabel.Text = "what to start";
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(750, 118);
            Load += Form_Load;
            Controls.Add(startLabel);
            Controls.Add(clientButton);
            Controls.Add(serverButton);
            Controls.Add(ipTextBox);
            Controls.Add(portTextBox);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

		#endregion

		private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Button serverButton;
        private System.Windows.Forms.Button clientButton;
        private System.Windows.Forms.Label startLabel;
    }
}