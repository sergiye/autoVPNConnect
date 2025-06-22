using sergiye.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AutoVPNConnect {
  static class Program {
    [STAThread]
    static void Main() {

      Crasher.Listen();

      if (!OperatingSystemHelper.IsCompatible(true, out var errorMessage, out var fixAction)) {
        if (fixAction != null) {
          if (MessageBox.Show(errorMessage, Updater.ApplicationTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
            fixAction?.Invoke();
          }
        }
        else {
          MessageBox.Show(errorMessage, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        Environment.Exit(0);
      }

      var currentProcessId = Process.GetCurrentProcess().Id;
      var processName = Path.GetFileNameWithoutExtension(Updater.CurrentFileLocation);
      var currentSessionID = Process.GetCurrentProcess().SessionId;
      var startedInstances = Process.GetProcessesByName(processName)
        .Where(p => p.SessionId == currentSessionID && p.Id != currentProcessId).ToArray();
      foreach (var process in startedInstances) {
        process.Refresh();
        var hwnd = process.MainWindowHandle;
        if (hwnd == IntPtr.Zero)
          hwnd = WinApiHelper.FindWindow(null, Updater.ApplicationTitle);
        WinApiHelper.SendMessage(hwnd, WinApiHelper.WM_SHOWME, 0, IntPtr.Zero);
      }
      if (startedInstances.Length > 0) {
#if DEBUG
        MessageBox.Show($"{Updater.ApplicationTitle} is already running.\nIt is recommended to close this instance.", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
        Environment.Exit(0);
#endif
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }
  }
}
