using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CommandWatchManager
{
    public partial class Form1 : Form
    {
        Dictionary<string, Process> processes;
        FileSystemWatcher watcher;
        DataTable table;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            processes = new Dictionary<string, Process>();

            table = new DataTable();
            table.Columns.Add("Request Name");
            table.Columns.Add("Created At");
            table.Columns.Add("Input Path");
            table.Columns.Add("Output Path");
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].Width = 160;
            dataGridView1.Columns[1].Width = 160;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Width = 200;

            watcher = new FileSystemWatcher();
            watcher.Path = ConfigurationManager.AppSettings["REQUEST_FOLDER"];
            watcher.Filter = "*.txt";
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            var name = Path.GetFileNameWithoutExtension(path);
            if (!processes.ContainsKey(name))
            {
                Thread.Sleep(999);
                var args = File.ReadAllLines(path).Select(s => s.Trim()).ToArray();
                var process = new Process();
                process.StartInfo.FileName = "CommandWatchWorker.exe";
                process.StartInfo.Arguments = String.Format("\"{0}\" \"{1}\"", args[0], args[1]);
                process.Start();
                processes.Add(name, process);
                table.LoadDataRow(new object[] { name, DateTime.Now.ToString("yyyy/MM/dd HH:mm"), args[0], args[1] }, true);
            }
            this.Invoke((MethodInvoker)delegate { dataGridView1.Refresh(); });
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            var name = Path.GetFileNameWithoutExtension(path);
            if (processes.ContainsKey(name))
            {
                processes[name].Kill();
                processes[name].Close();
                processes.Remove(name);
                var rows = table.Select("[Request Name] = '" + name + "'");
                if (rows.Length > 0) table.Rows.Remove(rows.Single());
            }
            this.Invoke((MethodInvoker)delegate { dataGridView1.Refresh(); });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (table.Rows.Count == 0) return;
            var item = (DataRowView)dataGridView1.CurrentRow.DataBoundItem;
            var name = item.Row["Request Name"].ToString();
            File.Delete(Path.Combine(watcher.Path, name + ".txt"));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            watcher.EnableRaisingEvents = false;
            foreach (var process in processes.Values) process.Kill();
            foreach (var file in new DirectoryInfo(watcher.Path).GetFiles()) file.Delete();
            watcher.Dispose();
        }
    }
}
