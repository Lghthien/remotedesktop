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
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // Đảm bảo hình ảnh giữ tỷ lệ
            this.Resize += new EventHandler(RemoteDesktopClient_Resize); // Đăng ký sự kiện Resize
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
                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Connected to server!");
                    });

                    while (true)
                    {
                        byte[] lengthBuffer = new byte[4];
                        stream.Read(lengthBuffer, 0, 4);
                        int length = BitConverter.ToInt32(lengthBuffer, 0);
                        byte[] buffer = new byte[length];
                        stream.Read(buffer, 0, length);
                        using (MemoryStream ms = new MemoryStream(buffer))
                        {
                            screenshot = new Bitmap(ms);
                            pictureBox.Invoke((MethodInvoker)delegate
                            {
                                pictureBox.Image = screenshot;

                                // Điều chỉnh kích thước của PictureBox theo kích thước của màn hình máy chủ
                                AdjustPictureBoxSize();
                            });
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

        private void AdjustPictureBoxSize()
        {
            if (screenshot != null)
            {
                float aspectRatio = (float)screenshot.Width / screenshot.Height;
                int formWidth = this.ClientSize.Width;
                int formHeight = this.ClientSize.Height;

                if (formWidth / (float)formHeight > aspectRatio)
                {
                    pictureBox.Width = (int)(formHeight * aspectRatio);
                    pictureBox.Height = formHeight;
                }
                else
                {
                    pictureBox.Width = formWidth;
                    pictureBox.Height = (int)(formWidth / aspectRatio);
                }
                pictureBox.Left = (this.ClientSize.Width - pictureBox.Width) / 2;
                pictureBox.Top = (this.ClientSize.Height - pictureBox.Height) / 2;
            }
        }

        private void RemoteDesktopClient_Resize(object sender, EventArgs e)
        {
            AdjustPictureBoxSize();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (client != null && client.Connected)
            {
                NetworkStream stream = client.GetStream();
                string message = $"MOUSE_MOVE|{e.X}|{e.Y}";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (client != null && client.Connected)
            {
                NetworkStream stream = client.GetStream();
                string message = $"MOUSE_CLICK|{e.X}|{e.Y}";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }

        private void pictureBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (client != null && client.Connected)
            {
                NetworkStream stream = client.GetStream();
                string message = $"KEY_PRESS|{e.KeyCode}";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }
    }
}
