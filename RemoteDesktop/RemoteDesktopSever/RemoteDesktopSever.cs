using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace remotedesktopsever
{
    public partial class RemoteDesktopSever : Form
    {
        private TcpListener listener;
        private Thread serverThread;
        private string pin = "1234"; // Mã PIN mặc định
        private List<TcpClient> clients = new List<TcpClient>();

        public RemoteDesktopSever()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (serverThread == null || !serverThread.IsAlive)
            {
                serverThread = new Thread(StartServer);
                serverThread.Start();
                Invoke((MethodInvoker)delegate
                {
                    statusLabel.Text = "Server is running...";
                });
            }
        }

        private void StartServer()
        {
            listener = new TcpListener(IPAddress.Any, 9000);
            listener.Start();
            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    clients.Add(client);
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
                catch (SocketException ex)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"SocketException: {ex.Message}");
                    });
                    break;
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] pinBuffer = new byte[4];
            stream.Read(pinBuffer, 0, 4);
            string receivedPin = Encoding.UTF8.GetString(pinBuffer);

            if (receivedPin == pin)
            {
                byte[] responseBuffer = BitConverter.GetBytes(1); // Mã PIN đúng
                stream.Write(responseBuffer, 0, responseBuffer.Length);

                Thread captureThread = new Thread(() => CaptureScreenLoop(client));
                captureThread.Start();

                while (true)
                {
                    try
                    {
                        byte[] lengthBuffer = new byte[4];
                        stream.Read(lengthBuffer, 0, 4);
                        int length = BitConverter.ToInt32(lengthBuffer, 0);
                        byte[] buffer = new byte[length];
                        stream.Read(buffer, 0, length);

                        string command = Encoding.UTF8.GetString(buffer);
                        ExecuteCommand(command);
                    }
                    catch (IOException)
                    {
                        break;
                    }
                }
            }
            else
            {
                byte[] responseBuffer = BitConverter.GetBytes(0); // Mã PIN sai
                stream.Write(responseBuffer, 0, responseBuffer.Length);
                client.Close();
            }
        }

        private void CaptureScreenLoop(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            while (true)
            {
                try
                {
                    Bitmap screenshot = CaptureScreen();
                    MemoryStream ms = new MemoryStream();
                    screenshot.Save(ms, ImageFormat.Jpeg);
                    byte[] buffer = ms.ToArray();
                    stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                    stream.Write(buffer, 0, buffer.Length);
                    Thread.Sleep(100); // Giảm tải CPU
                }
                catch (IOException)
                {
                    break;
                }
            }
        }

        private Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return screenshot;
        }

        private void ExecuteCommand(string command)
        {
            string[] parts = command.Split('|');
            string cmdType = parts[0];

            if (cmdType == "MOUSE_MOVE")
            {
                int x = int.Parse(parts[1]);
                int y = int.Parse(parts[2]);
                Cursor.Position = new Point(x, y);
            }
            else if (cmdType == "MOUSE_CLICK")
            {
                int x = int.Parse(parts[1]);
                int y = int.Parse(parts[2]);
                MouseClick(x, y);
            }
            else if (cmdType == "KEY_PRESS")
            {
                Keys key = (Keys)Enum.Parse(typeof(Keys), parts[1]);
                KeyPress(key);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        private void MouseClick(int x, int y)
        {
            Cursor.Position = new Point(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        private const int KEYEVENTF_KEYDOWN = 0x00;
        private const int KEYEVENTF_KEYUP = 0x02;

        private void KeyPress(Keys key)
        {
            byte vk = (byte)key;
            keybd_event(vk, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(vk, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
