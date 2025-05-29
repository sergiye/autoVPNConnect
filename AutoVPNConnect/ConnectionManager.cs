using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoVPNConnect {
  class ConnectionManager {

    private const int RAS_MaxEntryName = 256;
    private const int UNLEN = 256;
    private const int PWLEN = 256;
    private const int DNLEN = 15;
    private const int RAS_MaxPhoneNumber = 128;
    private const int RAS_MaxCallbackNumber = RAS_MaxPhoneNumber;

    private IntPtr hRasConn = IntPtr.Zero;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct RASDIALPARAMS {
      public int dwSize;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
      public string szEntryName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxPhoneNumber + 1)]
      public string szPhoneNumber;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxCallbackNumber + 1)]
      public string szCallbackNumber;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UNLEN + 1)]
      public string szUserName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PWLEN + 1)]
      public string szPassword;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DNLEN + 1)]
      public string szDomain;
    }

    [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
    private static extern uint RasDial(
        IntPtr lpRasDialExtensions,
        string lpszPhonebook,
        ref RASDIALPARAMS lpRasDialParams,
        int dwNotifierType,
        IntPtr lpvNotifier,
        out IntPtr lphRasConn);

    [DllImport("rasapi32.dll", SetLastError = true)]
    private static extern uint RasHangUp(IntPtr hRasConn);

    [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
    private static extern uint RasGetErrorString(uint errorCode, StringBuilder lpszErrorString, int cBufSize);

    [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
    private static extern uint RasGetEntryDialParams(string lpszPhonebook, ref RASDIALPARAMS lpRasDialParams, ref bool lpfPassword);

    readonly SettingsManager mSettingsManager;
    private readonly Timer vpnConnectionCheckTimer = new Timer();

    public ConnectionManager(ref SettingsManager rSettingsManager) {
      mSettingsManager = rSettingsManager;

      //Init timer
      vpnConnectionCheckTimer.Interval = 10000;
      vpnConnectionCheckTimer.Enabled = true;
      vpnConnectionCheckTimer.Tick += VPNConnectionCheckTimer_Tick;
    }

    public bool IsBusy { get; private set; }

    public static IEnumerable<NetworkInterface> GetActiveVpnConnections(string connectionName = null) {
      if (!NetworkInterface.GetIsNetworkAvailable())
        yield break;

      var interfaces = NetworkInterface.GetAllNetworkInterfaces();
      foreach (var ni in interfaces) {
        if (ni.NetworkInterfaceType == NetworkInterfaceType.Ppp &&
            ni.NetworkInterfaceType != NetworkInterfaceType.Loopback && 
            ni.OperationalStatus == OperationalStatus.Up) {
          yield return ni;
          if (!string.IsNullOrEmpty(connectionName) && ni.Name == connectionName)
            yield break;
        }
      }
    }

    public bool VpnIsConnected() {
      var vpnConnectionName = mSettingsManager.GetConnectionName();
      return GetActiveVpnConnections(vpnConnectionName).Any();
      //todo: hRasConn = IntPtr.Zero;
    }

    public Task ToggleConnection() {
      return Task.Run(() => {
        if (VpnIsConnected()) {
          DisconnectFromVpn();
        }
        else {
          ConnectToVpn();
        }
      });
    }

    private string GetRasError(uint errorCode) {
      var sb = new StringBuilder(512);
      RasGetErrorString(errorCode, sb, sb.Capacity);
      return $"Ошибка {errorCode}: {sb}";
    }

    private string DisconnectFromVpn() {
      if (IsBusy) {
        return "Busy";
      }
      IsBusy = true;
      try {
        if (hRasConn != IntPtr.Zero) {
          uint ret = RasHangUp(hRasConn);
          hRasConn = IntPtr.Zero;
          if (ret == 0) {
            return null;
          }
          //else {
          //  return GetRasError(ret);
          //}
        }

        var vpnName = mSettingsManager.GetConnectionName();
        var process = Process.Start(new ProcessStartInfo("rasdial.exe", $" \u0022{vpnName}\u0022 /disconnect") {
          RedirectStandardOutput = true,
          UseShellExecute = false,
          CreateNoWindow = true,
          WindowStyle = ProcessWindowStyle.Hidden
        });
        if (!process.WaitForExit(60000))
          process.Kill();
        return process.ExitCode == 0 ? null : "Error";
      }
      catch (Exception ex) {
        return ex.Message;
      }
      finally {
        IsBusy = false;
      }
    }

    private string ConnectToVpn() {
      if (IsBusy) return "Busy";
      try {
        IsBusy = true;
        var vpnName = mSettingsManager.GetConnectionName();
        var userName = mSettingsManager.GetUserName();
        var password = mSettingsManager.GetPassword();

        var dialParams = new RASDIALPARAMS();
        dialParams.dwSize = Marshal.SizeOf(typeof(RASDIALPARAMS));
        dialParams.szEntryName = vpnName;
        dialParams.szUserName = userName;
        dialParams.szPassword = password;
        dialParams.szDomain = "";

        bool hasPassword = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password);
        if (!hasPassword) {
          var ret = RasGetEntryDialParams(null, ref dialParams, ref hasPassword);
          if (ret != 0)
            return GetRasError(ret);
        }
        if (hasPassword && !string.IsNullOrEmpty(dialParams.szUserName)) {
          var ret = RasDial(IntPtr.Zero, null, ref dialParams, 0, IntPtr.Zero, out IntPtr conn);
          if (ret != 0)
            return GetRasError(ret);
          hRasConn = conn;
          return null;
        }
        else {
          ProcessStartInfo procStartInfo;
          if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password)) {
            var rasdialCommand = " " + '\u0022' + vpnName + '\u0022';
            rasdialCommand += " " + userName;
            rasdialCommand += " " + password;
            procStartInfo = new ProcessStartInfo("rasdial.exe", rasdialCommand);
          }
          else {
            var rasphoneCommand = " -d " + '\u0022' + vpnName + '\u0022';
            procStartInfo = new ProcessStartInfo("rasphone", rasphoneCommand);
          }

          procStartInfo.RedirectStandardOutput = true;
          procStartInfo.UseShellExecute = false;
          procStartInfo.CreateNoWindow = true;
          procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
          var process = Process.Start(procStartInfo);
          if (!process.WaitForExit(60000))
            process.Kill();
          return process.ExitCode == 0 ? null : "Error";
        }
      }
      catch (Exception ex) {
        return ex.Message;
      }
      finally {
        IsBusy = false;
      }
    }

    private void VPNConnectionCheckTimer_Tick(object sender, EventArgs e) {
      if (!mSettingsManager.GetApplicationEnabledSetting()) return;
      if (!VpnIsConnected() && SettingsManager.ValidSettingsFound()) {
        ConnectToVpn();
      }
    }
  }
}
