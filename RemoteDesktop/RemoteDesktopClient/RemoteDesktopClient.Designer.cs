﻿namespace reomtedesktopclient
{
    partial class RemoteDesktopClient
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.pinTextBox = new System.Windows.Forms.TextBox();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.PIN = new System.Windows.Forms.Label();
            this.IP = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 87);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1102, 462);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(29, 12);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(142, 23);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Conncet to sever";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // pinTextBox
            // 
            this.pinTextBox.Location = new System.Drawing.Point(332, 13);
            this.pinTextBox.Name = "pinTextBox";
            this.pinTextBox.Size = new System.Drawing.Size(100, 22);
            this.pinTextBox.TabIndex = 3;
            // 
            // ipTextBox
            // 
            this.ipTextBox.Location = new System.Drawing.Point(332, 59);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(100, 22);
            this.ipTextBox.TabIndex = 4;
            // 
            // PIN
            // 
            this.PIN.AutoSize = true;
            this.PIN.Location = new System.Drawing.Point(255, 17);
            this.PIN.Name = "PIN";
            this.PIN.Size = new System.Drawing.Size(29, 16);
            this.PIN.TabIndex = 5;
            this.PIN.Text = "PIN";
            // 
            // IP
            // 
            this.IP.AutoSize = true;
            this.IP.Location = new System.Drawing.Point(263, 61);
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(19, 16);
            this.IP.TabIndex = 6;
            this.IP.Text = "IP";
            // 
            // RemoteDesktopClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1126, 561);
            this.Controls.Add(this.IP);
            this.Controls.Add(this.PIN);
            this.Controls.Add(this.ipTextBox);
            this.Controls.Add(this.pinTextBox);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.pictureBox);
            this.Name = "RemoteDesktopClient";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox pinTextBox;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Label PIN;
        private System.Windows.Forms.Label IP;
    }
}

