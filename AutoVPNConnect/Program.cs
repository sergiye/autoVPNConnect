using System;
using System.Windows.Forms;

namespace AutoVPNConnect {
  static class Program {
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new AutoVpnConnect());
    }
  }
}
