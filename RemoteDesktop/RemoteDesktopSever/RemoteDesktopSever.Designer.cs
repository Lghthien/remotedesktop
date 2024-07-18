namespace remotedesktopsever
{
    partial class RemoteDesktopSever
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
            this.startButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pinLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(63, 49);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(140, 35);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start Server";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(72, 103);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(116, 16);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "Server is stopped.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "PIN";
            // 
            // pinLabel
            // 
            this.pinLabel.AutoSize = true;
            this.pinLabel.Location = new System.Drawing.Point(110, 9);
            this.pinLabel.Name = "pinLabel";
            this.pinLabel.Size = new System.Drawing.Size(0, 16);
            this.pinLabel.TabIndex = 3;
            // 
            // RemoteDesktopSever
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 128);
            this.Controls.Add(this.pinLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.startButton);
            this.Name = "RemoteDesktopSever";
            this.Text = "Sever";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label pinLabel;
    }
}

