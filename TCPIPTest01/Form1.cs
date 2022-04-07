using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.IO;


namespace TCPIPTest01
{
    public partial class Form1 : Form
    {
        private TcpClient m_client;
        private Socket client;
        public static object sendLock = new object();
        private byte[] buf = new byte[1024];
        private static object SendLock = new object();
        private delegate void delUpdateUI(string sMessage);
        



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // Create Tcp client.
                int nPort = 2001;
                m_client = new TcpClient("127.0.0.1", nPort);

                
                //IPAddress ip = IPAddress.Parse("127.0.0.1");
                //IPEndPoint ipe = new IPEndPoint(ip, nPort);//把ip和埠轉化為IPEndPoint例項
                //Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//建立一個Socket
                //Console.WriteLine("Conneting...");
                //c.Connect(ipe);//連線到伺服器
                //string sendStr = "hello!This is a socket test";
                //byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                //Console.WriteLine("Send Message");
                //c.Send(bs, bs.Length, 0);//傳送測試資訊
                //string recvStr = "";
                //byte[] recvBytes = new byte[1024];
                //int bytes;
                //bytes = c.Receive(recvBytes, recvBytes.Length, 0);//從伺服器端接受返回資訊
                //recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                //Console.WriteLine("Client Get Message:{0}", recvStr);//顯示伺服器返回資訊
                


                //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //socket.Connect("127.0.0.1", 13000); // 1.設定 IP:Port 2.連線至伺服器
                //NetworkStream stream = new NetworkStream(socket);
                //StreamReader sr = new StreamReader(stream);
                //StreamWriter sw = new StreamWriter(stream);

                //sw.WriteLine("你好伺服器，我是客戶端。"); // 將資料寫入緩衝
                //sw.Flush(); // 刷新緩衝並將資料上傳到伺服器

                //Console.WriteLine("從伺服器接收的資料： " + sr.ReadLine());

                //Console.ReadLine();


            }
            catch (ArgumentNullException a)
            {
                Console.WriteLine("ArgumentNullException:{0}", a);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException:{0}", ex);
            }
        }

        //private void btnSend_Click(object sender, EventArgs e)
        //{
        //    byte[] btData = System.Text.Encoding.ASCII.GetBytes(txtData.Text + "\r\n"); //  Convert string to byte array. \r\n "\迴車\換行"     
            
        //    try
        //    {
        //        NetworkStream stream = m_client.GetStream();
        //        stream.Write(btData, 0, btData.Length); // Write data to server.
                
                
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Write Exception:{0}", ex);
        //    }
        //}

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            m_client.Close();

            
        }



        private void Receive(IAsyncResult ia)
        {
            try
            {
                client = ia.AsyncState as Socket;
                int count = client.EndReceive(ia);
                client.BeginReceive(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(Receive), client);
                string context = Encoding.ASCII.GetString(buf, 0, count);
                if (context.Length > 0)
                {
                    ProcessData(context);

                }
                else
                {

                }
            }
            catch
            {
            }
        }
        private void ProcessData(string recvdata)
        {
            recvdata = recvdata.Trim();
            string[] cmddata = recvdata.Split(new char[] { '#' });
            //string[] singlecmddata1 = cmddata[1].Split(new char[] { '\r\n' });
            string[] singlecmddata = cmddata[1].Split(new char[] { ',' });
            if (singlecmddata[0].Trim() != "RobotPos")
            {
                //RunLog.instance.SaveLog(1, "PC接收到Robot發來的去" + recvdata + "命令");
            }
            switch (singlecmddata[0].Trim())
            {
                case "RobotPos":
                    //相關處理
                    break;
                default:
                    break;
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string sendcmd = txtData.Text;
                //sendcmd = part1 + " " + part2 + " " + part3 + " " + part4 + " " + part5 + " " + part6 + " " + part7;
                SendCmdData(sendcmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Write Exception:{0}", ex);
            }
        }

        public void SendCmdData(string StrData)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(StrData);
            //RunLog.instance.SaveLog(1, "PC向Robot發送" + StrData + "命令");
            lock (SendLock)
            {
                try
                {
                    byte[] buf1;
                    buf1 = Encoding.ASCII.GetBytes(StrData + "\r\n");
                    NetworkStream stream = m_client.GetStream();
                    stream.Write(buf1, 0, buf1.Length); // Write data to server.
                    //client.Send(buf1, 0, buf1.Length, SocketFlags.None);

                    

                    data = new Byte[1024];
                    String responseData = String.Empty;
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    UdpateMessage("Received Data:" + responseData);
                    Console.WriteLine("Received: {0}", responseData);
                }
                catch
                {
                }
            }
        }

        private void UdpateMessage(string sReceiveData)
        {
            if (this.InvokeRequired)
            {
                delUpdateUI del = new delUpdateUI(UdpateMessage);
                this.Invoke(del, sReceiveData);
            }
            else
            {
                txtMessage.Text += sReceiveData + Environment.NewLine;
            }
        }


    }
}
