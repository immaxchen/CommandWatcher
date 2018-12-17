using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CommandWatcher
{
    public partial class Form1 : Form
    {
        private static StringBuilder cmdOutput;
        Process cmdProcess;
        StreamWriter cmdStreamWriter;

        public Form1()
        {
            InitializeComponent();

            cmdOutput = new StringBuilder();
            cmdProcess = new Process();

            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.CreateNoWindow = true;
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.RedirectStandardInput = true;
            cmdProcess.StartInfo.RedirectStandardOutput = true;
            cmdProcess.OutputDataReceived += new DataReceivedEventHandler(StdoutHandler);
            cmdProcess.Start();

            cmdStreamWriter = cmdProcess.StandardInput;
            cmdProcess.BeginOutputReadLine();

            Thread.Sleep(500);
            cmdOutput.AppendLine(string.Empty);
            cmdStreamWriter.WriteLine(string.Empty);

            Thread.Sleep(500);
            textBox2.AppendText(cmdOutput.ToString());
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
            {
                int lastNewLine = cmdOutput.ToString().LastIndexOf(Environment.NewLine);
                cmdOutput.Remove(lastNewLine, cmdOutput.Length - lastNewLine);
                cmdStreamWriter.WriteLine(textBox1.Text);

                Thread.Sleep(500);
                cmdOutput.AppendLine(string.Empty);
                cmdStreamWriter.WriteLine(string.Empty);

                Thread.Sleep(500);
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
                textBox2.AppendText(cmdOutput.ToString());
            }
        }

        private static void StdoutHandler(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                cmdOutput.Append(Environment.NewLine + e.Data);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cmdStreamWriter.Close();
            cmdProcess.WaitForExit();
            cmdProcess.Close();
        }
    }
}
