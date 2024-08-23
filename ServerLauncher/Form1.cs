using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace ServerLauncher
{
    public partial class Form1 : Form
    {
        public static Form1 Instance { get; private set; }
        public Process AccountCacher;
        public Process LauncherServer;
        public Process LobbyServer;
        public Process WorldServer;
        public ProcessWatchdog ACWatch, LauSWatch, LSWatch, WSWatch;

        public Form1()
        {
            InitializeComponent();
            Instance = this;
            if (Program.AutoStart)
            {
                Start();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { ACWatch.Stop(); } catch (Exception) { }
            try { LauSWatch.Stop(); } catch (Exception) { }
            try { LSWatch.Stop(); } catch (Exception) { }
            try { WSWatch.Stop(); } catch (Exception) { }
        }

        private void B_start_Click(object sender, EventArgs e)
        {
            B_start.Enabled = false;

            Start();
            B_start.Enabled = true;
        }

        private void Start()
        {
            if (StartAccountCheckBox.Checked)
            {
                AccountCacher = new Process();
                AccountCacher.StartInfo.FileName = "AccountCacher.exe";
                AccountCacher.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                AccountCacher.StartInfo.Arguments = acStartArgument.Text;
                AccountCacher.Start();
                if (ACWatch == null && acWatchChk.Checked)
                {
                    ACWatch = new ProcessWatchdog(AccountCacher, "AccountCacher.exe", acStartArgument.Text);
                }
                Thread.Sleep(500);
            }

            if (StartLauncherCheckBox.Checked)
            {
                LauncherServer = new Process();
                LauncherServer.StartInfo.FileName = "LauncherServer.exe";
                LauncherServer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                LauncherServer.StartInfo.Arguments = lsStartArguments.Text;
                LauncherServer.Start();
                if (LauSWatch == null && lausWatchChk.Checked)
                {
                    LauSWatch = new ProcessWatchdog(LauncherServer, "LauncherServer.exe", lsStartArguments.Text);
                }
                Thread.Sleep(500);
            }

            if (StartLobbyCheckBox.Checked)
            {
                LobbyServer = new Process();
                LobbyServer.StartInfo.FileName = "LobbyServer.exe";
                LobbyServer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                LobbyServer.StartInfo.Arguments = lobsStartArgument.Text;
                LobbyServer.Start();
                if (LSWatch == null && lsWatchChk.Checked)
                {
                    LSWatch = new ProcessWatchdog(LobbyServer, "LobbyServer.exe", lobsStartArgument.Text);
                }
                Thread.Sleep(500);
            }

            if (StartWorldCheckBox.Checked)
            {
                WorldServer = new Process();
                WorldServer.StartInfo.FileName = "WorldServer.exe";
                WorldServer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                WorldServer.StartInfo.Arguments = wrldsStartArguments.Text;
                WorldServer.Start();
                if (WSWatch == null && wsWatchChk.Checked)
                {
                    WSWatch = new ProcessWatchdog(WorldServer, "WorldServer.exe", wrldsStartArguments.Text);
                }
            }
        }

        private void B_stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (WorldServer != null) { WorldServer.Kill(); WSWatch.Stop(); WSWatch = null; }
            }
            catch (Exception) { }

            try
            {
                if (LobbyServer != null) { LobbyServer.Kill(); LSWatch.Stop(); LSWatch = null; }
            }
            catch (Exception) { }

            try
            {
                if (LauncherServer != null) { LauncherServer.Kill(); LauSWatch.Stop(); LauSWatch = null; }
            }
            catch (Exception) { }

            try
            {
                if (AccountCacher != null) { AccountCacher.Kill(); ACWatch.Stop(); ACWatch = null; }
            }
            catch (Exception) { }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}