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
            //pictureBox.SizeMode = PictureBoxSizeMode.StretchImage; // Stretch image to fit PictureBox
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
                        int totalBytesRead = 0;
                        while (totalBytesRead < length)
                        {
                            int bytesRead = stream.Read(buffer, totalBytesRead, length - totalBytesRead);
                            if (bytesRead == 0)
                            {
                                throw new IOException("Connection closed unexpectedly.");
                            }
                            totalBytesRead += bytesRead;
                        }

                        using (MemoryStream ms = new MemoryStream(buffer))
                        {
                            try
                            {
                                // Assign the new Bitmap to PictureBox.Image, disposing the old one
                                pictureBox.Invoke((MethodInvoker)(() =>
                                {
                                    if (pictureBox.Image != null)
                                    {
                                        pictureBox.Image.Dispose(); // Dispose the old image to release resources
                                    }
                                    pictureBox.Image = Image.FromStream(ms); // Set PictureBox image from memory stream
                                }));
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
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                NetworkStream stream = client.GetStream();
                int messageLength = messageBytes.Length;

                byte[] lengthBytes = BitConverter.GetBytes(messageLength);
                stream.Write(lengthBytes, 0, 4); // Send the length of the message

                stream.Write(messageBytes, 0, messageLength); // Send the message itself
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Exception in SendMessage: {ex.Message}");
                });
            }
        }
    }
}
