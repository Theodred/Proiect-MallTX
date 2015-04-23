using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace MallTX
{
    public partial class Conversation : Form
    {
        Socket socket;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        string yourIP, yourPort;
        string friendIP, friendPort;

        Server server = new Server();

        public Conversation(string yIP, string yPort, string fIP, string fPort)
        {
            InitializeComponent();

            yourIP = yIP;
            yourPort = yPort;
            friendIP = fIP;
            friendPort = fPort;

            
        }

        private void Conversation_Load(object sender, EventArgs e)
        {
            Client.ip2 = friendIP;
            Server.path = Application.StartupPath;

            if (Server.path.Length > 0)
                backgroundWorker1.RunWorkerAsync();

            //this.BackColor = Color.FromArgb(0, 0, 0);
            //menuStrip1.BackColor = Color.FromArgb(0, 0, 0);
            //menuStrip1.ForeColor = Color.DarkRed;
            //rtb_Conversation.BackColor = Color.FromArgb(0, 0, 0);
            //rtb_Conversation.ForeColor = Color.DarkRed;
            //rtb_Conversation.BorderStyle = BorderStyle.None;


            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            epLocal = new IPEndPoint(IPAddress.Parse(yourIP), Convert.ToInt32(yourPort));
            socket.Bind(epLocal);

            epRemote = new IPEndPoint(IPAddress.Parse(friendIP), Convert.ToInt32(friendPort));
            socket.Connect(epRemote);

            buffer = new byte[1500];
            socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
        }
        
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] receivedData = new byte[1500];
                receivedData = (byte[])aResult.AsyncState;

                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivedMessage = aEncoding.GetString(receivedData);

                rtb_Conversation.Text+="\rFriend: " + receivedMessage;

                buffer = new byte[1500];
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Send_Click(object sender, EventArgs e)
        {
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(rtb_Message.Text);

            socket.Send(sendingMessage);

            rtb_Conversation.Text+="\rMe: " + rtb_Message.Text;
            rtb_Message.Text = "";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            server.StartServer();
        }

        private void sendFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();

            if(fd.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {

                ASCIIEncoding aEncoding = new ASCIIEncoding();
                byte[] sendingMessage = new byte[1500];

                rtb_Message.Text = "System: File Tranfer.";
                sendingMessage = aEncoding.GetBytes(rtb_Message.Text);

                socket.Send(sendingMessage);
                rtb_Conversation.Text += "System: File Transfer.";

                Client.SendFile(fd.FileName);

                rtb_Message.Text = "";
            }
        }

        private void rtb_Message_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)Keys.Enter)
            {
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                byte[] sendingMessage = new byte[1500];
                sendingMessage = aEncoding.GetBytes(rtb_Message.Text);

                socket.Send(sendingMessage);

                rtb_Conversation.Text += "\rMe: " + rtb_Message.Text;
                rtb_Message.Text = "";
            }
        }
    }
}
