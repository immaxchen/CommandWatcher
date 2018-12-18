using System.Windows.Forms;
using CommandWatcherUtility;

namespace Example
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CommandWatcher.iPath = @"D:\TEST\input.txt";
            CommandWatcher.oPath = @"D:\TEST\output.txt";
            CommandWatcher.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CommandWatcher.Stop();
        }
    }
}
