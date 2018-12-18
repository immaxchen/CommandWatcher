using System.Windows.Forms;
using CommandWatcherUtility;

namespace Example
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CommandWatcher.iPath = @"C:\Users\user\Desktop\TEST\input.txt";
            CommandWatcher.oPath = @"C:\Users\user\Desktop\TEST\output.txt";
            CommandWatcher.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CommandWatcher.Stop();
        }
    }
}
