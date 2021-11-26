using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Text;
using System.Threading;

namespace GameWindows
{
    public partial class Form1 : Form
    {
        private NetworkStream stream;
        private TcpClient tcpClient = new TcpClient();
        private String[] pictures={ "\\1.jpg", "\\2.jpg","\\3.jpg","\\4.jpg","5.jpg","6.jpg"};
        private int postion = 0;
        public Form1()
        {
            InitializeComponent();  
            CheckForIllegalCrossThreadCalls = false;
            quitGame.Enabled = false;
            axWindowsMediaPlayer1.Hide();
            pictureBox1.Image = Image.FromFile("G:\\VisualStudioProject\\GameWindows\\resources\\"+pictures[postion]);
        }

        private void start_Click(object sender, EventArgs e)
        {
            //新建线程打开音频文件
            new Thread(new ThreadStart(beginPlay)).Start();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            //暂定播放
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        private void beginGame_Click(object sender, EventArgs e)
        {

            Thread thread = new Thread(new ThreadStart(connection));
            thread.Start();
            beginGame.Enabled = false;
            quitGame.Enabled = true;

        }

 

        private void quitGame_Click(object sender, EventArgs e)
        {
            tcpClient.Close();
            beginGame.Enabled = true;
            quitGame.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile("G:\\VisualStudioProject\\GameWindows\\resources\\"+pictures[(++postion)%pictures.Length]);
        }



        private void sendButton_Click(object sender, EventArgs e)
        {
            sendCommand();
        }


        public void connection()
        {
            try
            {
                //向指定的IP地址的服务器发出连接请求

                tcpClient.Connect("10.1.230.74", 3900);
                messages.Items.Add("连接成功！");
                stream = tcpClient.GetStream();
                byte[] data = new byte[1024];
                //判断网络流是否可读            
                if (stream.CanRead)
                {
                    int len = stream.Read(data, 0, data.Length);
                    string msg = Encoding.Default.GetString(data, 0, data.Length);
                    string str = "\r\n";
                    char[] str1 = str.ToCharArray();
                    string[] msg1 = msg.Split(str1);
                    for (int j = 0; j < msg1.Length; j++)
                    {
                        messages.Items.Add(msg1[j]);
                    }
                }

            }
            catch (System.Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }

        public void beginPlay()
        {
            string s = @"G:\VisualStudioProject\GameWindows\resources\music.mp3";
            axWindowsMediaPlayer1.URL = s;
        }

        public void sendCommand()
        {
            if (tcpClient.Connected)
            {
                //向服务器发送数据
                string msg = command.Text;
                Byte[] outbytes = System.Text.Encoding.Default.GetBytes(msg + "\n");
                stream.Write(outbytes, 0, outbytes.Length);
                byte[] data = new byte[1024];
                //接收服务器回复数据
                if (stream.CanRead)
                {
                    int len = stream.Read(data, 0, data.Length);
                    string msg1 = Encoding.Default.GetString(data, 0, data.Length);
                    string str = "\r\n";
                    char[] str1 = str.ToCharArray();
                    string[] msg2 = msg1.Split(str1);
                    for (int j = 0; j < msg2.Length; j++)
                    {
                        messages.Items.Add(msg2[j]);
                    }
                }
            }
            else
            {
                messages.Items.Add("连接已断开");
            }

        }
    }
}
