using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerLauncher
{
    public class ProcessWatchdog
    {
        private Process Process;
        private Thread WatchThread;
        private string Exe, Args;
        private bool ProcessHooked = false;

        public ProcessWatchdog(Process proc, string exe, string args)
        {
            Process = proc;
            Exe = exe;
            Args = args;
            ProcessHooked = true;
            WatchThread = new Thread(CheckProcess);
            WatchThread.Start();
        }

        public void Stop()
        {
            WatchThread.Abort();
        }

        private void CheckProcess()
        {
            while (true)
            {
                if (!ProcessHooked)
                {
                    Process[] processes = Process.GetProcessesByName(Exe.Replace(".exe", ""));
                    foreach (Process proc in processes)
                    {
                        if (!proc.HasExited)
                        {
                            Process = proc;
                            ProcessHooked = true;
                            if (Exe.Contains("WorldServer"))
                            {
                                Form1.Instance.WorldServer = Process;
                            }
                            else if (Exe.Contains("AccountCacher"))
                            {
                                Form1.Instance.AccountCacher = Process;
                            }
                            else if (Exe.Contains("LauncherServer"))
                            {
                                Form1.Instance.LauncherServer = Process;
                            }
                            else if (Exe.Contains("LobbyServer"))
                            {
                                Form1.Instance.LobbyServer = Process;
                            }
                            break;
                        }
                    }
                    if (ProcessHooked)
                        continue;
                }
                else
                {
                    try
                    {
                        if (!Directory.Exists($"{AppContext.BaseDirectory}\\CrashDumps"))
                        {
                            Directory.CreateDirectory($"{AppContext.BaseDirectory}\\CrashDumps");
                        }
                        else
                        {
                            int i = 0;
                            foreach (string file in Directory.EnumerateFiles($"{AppContext.BaseDirectory}\\CrashDumps"))
                            {
                                if (!file.Contains(Process.ProcessName))
                                    continue;

                                if (i != 0)
                                {
                                    File.Delete(file);
                                }
                                i++;
                            }
                        }

                        Process crashDump = new Process();

                        crashDump.StartInfo = new ProcessStartInfo("procdump.exe", $"{Exe.Replace(".exe", "")} -accepteula -e 1 -f C00000FD.STACK_OVERFLOW -g -ma .\\CrashDumps");
                        crashDump.StartInfo.UseShellExecute = true;
                        crashDump.StartInfo.CreateNoWindow = true;
                        crashDump.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        crashDump.Start();
                        crashDump.WaitForExit();
                        if (!Process.HasExited || !Process.Responding) //Critical exceptions like stakoverflow dont exit the app, but exited procdump indicates them well
                        {
                            //kill process and restart
                            Process newProcess = new Process();
                            newProcess.StartInfo.FileName = Exe;
                            newProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                            newProcess.StartInfo.Arguments = Args;
                            newProcess.Start();
                            Process.Kill();
                            Process = newProcess;
                            Thread.Sleep(5000);
                        }
                        else //??
                        {
                            ProcessHooked = false;
                        }
                    }
                    catch (Exception)
                    {
                        ProcessHooked = false;
                        //Stop();
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}