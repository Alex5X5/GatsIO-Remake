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
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new System.Windows.Forms.TextBox();
            textBox2 = new System.Windows.Forms.TextBox();
            textBox3 = new System.Windows.Forms.TextBox();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Font = new System.Drawing.Font("Segoe UI", 15F);
            textBox1.Location = new System.Drawing.Point(19, 61);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(271, 34);
            textBox1.TabIndex = 0;
            textBox1.Text = "enter port (100 by default)";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // textBox2
            // 
            textBox2.Font = new System.Drawing.Font("Segoe UI", 15F);
            textBox2.Location = new System.Drawing.Point(19, 12);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.ShortcutsEnabled = false;
            textBox2.Size = new System.Drawing.Size(271, 34);
            textBox2.TabIndex = 0;
            textBox2.TabStop = false;
            textBox2.Text = "enter IP (localhost by default)";
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // textBox3
            // 
            textBox3.Font = new System.Drawing.Font("Segoe UI", 15F);
            textBox3.Location = new System.Drawing.Point(296, 12);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.ShortcutsEnabled = false;
            textBox3.Size = new System.Drawing.Size(396, 34);
            textBox3.TabIndex = 0;
            textBox3.TabStop = false;
            textBox3.Text = "what to start";
            textBox3.TextChanged += textBox2_TextChanged;
            // 
            // button1
            // 
            button1.Font = new System.Drawing.Font("Segoe UI", 15F);
            button1.Location = new System.Drawing.Point(296, 52);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(187, 43);
            button1.TabIndex = 1;
            button1.Text = "Server";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Font = new System.Drawing.Font("Segoe UI", 15F);
            button2.Location = new System.Drawing.Point(499, 52);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(193, 43);
            button2.TabIndex = 1;
            button2.Text = "Client";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(750, 118);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}