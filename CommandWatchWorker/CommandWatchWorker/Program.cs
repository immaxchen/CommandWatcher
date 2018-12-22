using CommandWatcherUtility;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace CommandWatchWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            args = args.Select(s => s.Trim()).ToArray();
            try
            {
                File.WriteAllText(args[0], string.Empty);
                File.WriteAllText(args[1], string.Empty);
            }
            catch
            {
                return;
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            CommandWatcher.iPath = args[0];
            CommandWatcher.oPath = args[1];
            CommandWatcher.Start();
            Thread.Sleep(Timeout.Infinite);
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CommandWatcher.Stop();
        }
    }
}
