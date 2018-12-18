using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommandWatcherUtility
{
    static class CommandWatcher
    {
        public static string iPath;
        public static string oPath;

        private static Process process;
        private static StreamWriter stream;
        private static StringBuilder buffer;
        private static FileSystemWatcher watcher;
        private static Timer timer = new Timer(new TimerCallback(DoChange));

        public static void Start()
        {
            if (string.IsNullOrEmpty(iPath) || string.IsNullOrEmpty(oPath)) return;

            process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += new DataReceivedEventHandler(OnReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(OnReceived);
            process.Start();

            stream = process.StandardInput;
            buffer = new StringBuilder();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(iPath);
            watcher.Filter = Path.GetFileName(iPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            timer.Dispose();
            timer = new Timer(new TimerCallback(DoChange), null, 999, Timeout.Infinite);
        }

        private static void DoChange(object state)
        {
            var lines = File.ReadAllLines(iPath);
            stream.WriteLine(lines.Last());
        }

        private static void OnReceived(object sender, DataReceivedEventArgs e)
        {
            buffer.AppendLine(e.Data);
            timer.Dispose();
            timer = new Timer(new TimerCallback(DoReceived), null, 999, Timeout.Infinite);
        }

        private static void DoReceived(object state)
        {
            using (StreamWriter stream = new StreamWriter(oPath, true))
                stream.Write(buffer.ToString());
            buffer.Clear();
        }

        public static void Stop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            stream.Close();
            process.WaitForExit();
            process.Close();
        }
    }
}
