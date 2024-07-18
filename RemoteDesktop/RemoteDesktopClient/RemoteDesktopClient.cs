using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace reomtedesktopclient
{
    public partial class RemoteDesktopClient : Form
    {
        private TcpClient client;
        private Thread clientThread;
        private Bitmap screenshot;

        public RemoteDesktopClient()
        {
            InitializeComponent();
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage; // Stretch image to fit PictureBox
            this.Resize += new EventHandler(RemoteDesktopClient_Resize); // Register Resize event
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (clientThread == null || !clientThread.IsAlive)
            {
                clientThread = new Thread(ConnectToServer);
                clientThread.Start();
            }
        }

        private void ConnectToServer()
        {
            try
            {
                string serverIp = ipTextBox.Text;
                client = new TcpClient(serverIp, 9000);
                NetworkStream stream = client.GetStream();

                byte[] pinBytes = Encoding.UTF8.GetBytes(pinTextBox.Text);
                stream.Write(pinBytes, 0, pinBytes.Length);

                byte[] responseBuffer = new byte[4];
                stream.Read(responseBuffer, 0, 4);
                int response = BitConverter.ToInt32(responseBuffer, 0);

                if (response == 1)
                {
                    connectButton.Visible = false;
                    PIN.Visible = false;
                    IP.Visible = false;
                    pinTextBox.Visible = false;
                    ipTextBox.Visible = false;
                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Connected to server!");
                    });

                    while (true)
                    {
                        byte[] lengthBuffer = new byte[4];
                        stream.Read(lengthBuffer, 0, 4);
                        int length = BitConverter.ToInt32(lengthBuffer, 0);

                        // Validate the length to avoid overflow
                        if (length <= 0 || length > 10_000_000)
                        {
                            throw new InvalidOperationException("Invalid data length received.");
                        }

                        byte[] buffer = new byte[length];
                        stream.Read(buffer, 0, length);

                        using (MemoryStream ms = new MemoryStream(buffer))
                        {
                            try
                            {
                                screenshot = new Bitmap(ms);
                                pictureBox.Invoke(new Action(() => pictureBox.Image = screenshot)); // Update PictureBox
                            }
                            catch (ArgumentException ex)
                            {
                                Invoke((MethodInvoker)delegate
                                {
                                    MessageBox.Show($"Invalid image data: {ex.Message}");
                                });
                            }
                        }
                    }
                }
                else
                {
                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Invalid PIN!");
                    });
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Exception: {ex.Message}");
                });
            }
        }

        private void RemoteDesktopClient_Resize(object sender, EventArgs e)
        {
            AdjustPictureBoxSize();
        }

        private void AdjustPictureBoxSize()
        {
            pictureBox.Width = this.ClientSize.Width;
            pictureBox.Height = this.ClientSize.Height;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            SendMouseEvent("MOUSE_MOVE", e.X, e.Y);
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            SendMouseEvent("MOUSE_CLICK", e.X, e.Y);
        }

        private void pictureBox_KeyDown(object sender, KeyEventArgs e)
        {
            SendKeyEvent("KEY_PRESS", e.KeyCode);
        }

        private void SendMouseEvent(string type, int x, int y)
        {
            if (client != null && client.Connected)
            {
                NetworkStream stream = client.GetStream();
                string message = $"{type}|{x}|{y}";
                SendMessage(message);
            }
        }

        private void SendKeyEvent(string type, Keys keyCode)
        {
            if (client != null && client.Connected)
            {
                NetworkStream stream = client.GetStream();
                string message = $"{type}|{keyCode}";
                SendMessage(message);
            }
        }

        private void SendMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
    }
}
