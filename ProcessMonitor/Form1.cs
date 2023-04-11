using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Management;
using static System.Formats.Asn1.AsnWriter;

namespace ProcessMonitor
{
    public partial class Form1 : Form
    {
        private static readonly string FixedProcessName = "notepad.exe";
        private static readonly string FixedWindowTitle = "Untitled - Notepad";

        private static bool IsProcessRun = false;

        private readonly Thread monitorThread;
        public Form1()
        {
            InitializeComponent();
            monitorThread = new Thread(MonitorProcess);
            monitorThread.Start();
        }

        private void MonitorProcess()
        {
            while (true)
            {
                var process = Process.GetProcesses().FirstOrDefault(process => process.MainWindowTitle.Contains(FixedWindowTitle));

                if (process == null && IsProcessRun)
                {
                    IsProcessRun = false;
                    listBox1.Invoke((MethodInvoker)delegate
                    {
                        listBox1.Items.Add($"{DateTime.Now}: Process '{FixedWindowTitle}' has closed.");
                    });
                }
                if (process != null)
                {
                    if (!IsProcessRun)
                    {
                        listBox1.Invoke((MethodInvoker)delegate
                        {
                            listBox1.Items.Add($"{DateTime.Now}: Process '{process.MainWindowTitle}' has started.");
                        });
                        IsProcessRun = true;
                    }
                }
                Thread.Sleep(100);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                monitorThread.Abort();
            }
            catch { }

            base.OnFormClosing(e);
        }
    }
}