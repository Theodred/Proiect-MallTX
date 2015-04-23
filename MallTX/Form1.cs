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
    public partial class MallTX_Welcome : Form
    {
        public MallTX_Welcome()
        {
            InitializeComponent();
        }

        Socket socket;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        private void MallTX_Welcome_Load(object sender, EventArgs e)
        {
            tb_YourIP.ReadOnly = false;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            tb_YourIP.Text = GetIP();

            this.BackColor = Color.FromArgb(0, 0, 0);

            //label1.ForeColor = Color.DarkRed;
            //label2.ForeColor = Color.DarkRed;
            //label3.ForeColor = Color.DarkRed;
            //label4.ForeColor = Color.DarkRed;
            //label5.ForeColor = Color.DarkRed;

            tb_YourIP.BorderStyle = BorderStyle.None;
            tb_YourPort.BorderStyle = BorderStyle.None;
            tb_FriendIP.BorderStyle = BorderStyle.None;
            tb_FriendPort.BorderStyle = BorderStyle.None;

            tb_YourPort.Focus();

            //tb_YourIP.BackColor = Color.FromArgb(7, 0, 0);
            //tb_YourIP.ForeColor = Color.DarkRed;
            //tb_YourPort.BackColor = Color.FromArgb(7, 0, 0);
            //tb_YourPort.ForeColor = Color.DarkRed;
            //tb_FriendIP.BackColor = Color.FromArgb(7, 0, 0);
            //tb_FriendIP.ForeColor = Color.DarkRed;
            //tb_FriendPort.BackColor = Color.FromArgb(7, 0, 0);
            //tb_FriendPort.ForeColor = Color.DarkRed;

            //Connect.BackColor = Color.FromArgb(7, 0, 0);
            //Connect.ForeColor = Color.DarkRed;            
        }

        private string GetIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            return "127.0.0.1";
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            if (tb_YourIP.Text != "" && tb_YourPort.Text != "" && tb_FriendIP.Text != "" && tb_FriendPort.Text != "" && Convert.ToInt32(tb_YourPort.Text) < 2014 && Convert.ToInt32(tb_FriendPort.Text) < 2014 && tb_YourPort.Text != tb_FriendPort.Text && tb_FriendIP.Text != tb_YourIP.Text)
            {
                Client.port = Convert.ToInt32(tb_YourPort.Text) + 1000;
                Server.port = Convert.ToInt32(tb_FriendPort.Text) + 1000;

                Conversation conv = new Conversation(tb_YourIP.Text, tb_YourPort.Text, tb_FriendIP.Text, tb_FriendPort.Text);

                conv.Show();

                epLocal = new IPEndPoint(IPAddress.Parse(tb_YourIP.Text), Convert.ToInt32(tb_YourPort.Text));
                socket.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(tb_FriendIP.Text), Convert.ToInt32(tb_FriendPort.Text));
                socket.Connect(epRemote);

                buffer = new byte[1500];
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            else
            {
                MessageBox.Show("Please, check the data entered.", "Error!");
            }
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] receivedData = new byte[1500];
                receivedData = (byte[])aResult.AsyncState;

                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivedMessage = aEncoding.GetString(receivedData);

                //lb_Conversation.Items.Add("\rFriend: " + receivedMessage);

                buffer = new byte[1500];
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(" Introduceti adresele de IP impreuna cu porturile alese (diferite intre ele) si apasati pe butonul 'Connect' .");
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Autori : Corbu Teodora si Iordache Andrei");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MallTX usureaza comunicarea, facilitand conexiunea fara ajutorul internetului, doar prin intermediul adresei IP.");
        }
    }
}
