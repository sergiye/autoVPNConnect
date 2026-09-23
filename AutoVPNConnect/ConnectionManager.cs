using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoVPNConnect {

  class ConnectionManager {

    private const int RAS_MaxEntryName = 256;
    private const int UNLEN = 256;
    private const int PWLEN = 256;
    private const int DNLEN = 15;
    private const int RAS_MaxPhoneNumber = 128;
    private const int RAS_MaxCallbackNumber = RAS_MaxPhoneNumber;
    private const int RAS_MaxDeviceType = 16;
    private const int RAS_MaxDeviceName = 128;
    private const int MAX_PATH = 260;

    private const uint ERROR_BUFFER_TOO_SMALL = 603;
    private const uint ERROR_PORT_NOT_AVAILABLE = 633;

    private static readonly TimeSpan RasManRestartInterval = TimeSpan.FromMinutes(10);
    private static DateTime lastRasManRestart = DateTime.MinValue;

    private IntPtr hRasConn = IntPtr.Zero;
    private bool isBusy;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    struct RASCONN {
      public int dwSize;
      public IntPtr hrasconn;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
      public string szEntryName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceType + 1)]
      public string szDeviceType;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceName + 1)]
      public string szDeviceName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
      public string szPhonebook;
      public int dwSubEntry;
      public Guid guidEntry;
      public int dwFlags;
      public long luid;
      public Guid guidCorrelationId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct RASCONNSTATUS {
      public int dwSize;
      public int rasconnstate;
      public int dwError;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceType + 1)]
      public string szDeviceType;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceName + 1)]
      public string szDeviceName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxPhoneNumber + 1)]
      public string szPhoneNumber;
    }

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
    private static extern uint RasDial(IntPtr lpRasDialExtensions, string lpszPhonebook, ref RASDIALPARAMS lpRasDialParams, int dwNotifierType, IntPtr lpvNotifier, out IntPtr lphRasConn);

    [DllImport("rasapi32.dll", SetLastError = true)]
    private static extern uint RasHangUp(IntPtr hRasConn);

    [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
    private static extern uint RasGetErrorString(uint errorCode, StringBuilder lpszErrorString, int cBufSize);

    [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
    private static extern uint RasEnumConnections([In, Out] RASCONN[] lprasconn, ref int lpcb, ref int lpcConnections);

    [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
    private static extern uint RasGetConnectStatus(IntPtr hRasConn, ref RASCONNSTATUS lpRasConnStatus);

    [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
    private static extern uint RasGetEntryDialParams(string lpszPhonebook, ref RASDIALPARAMS lpRasDialParams, ref bool lpfPassword);

    readonly SettingsManager mSettingsManager;

    public ConnectionManager(ref SettingsManager rSettingsManager) {
      mSettingsManager = rSettingsManager;
      NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
    }

    public event Action OnStatusChanged;

    private void NetworkAddressChanged(object sender, EventArgs e) {
      if (mSettingsManager.Reconnect) {
        RestoreConnection();
      }
      OnStatusChanged?.Invoke();
    }

    public bool IsBusy {
      get => isBusy;
      private set {
        isBusy = value;
        OnStatusChanged?.Invoke();
      }
    }

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
      var vpnConnectionName = mSettingsManager.VpnConnectionName;
      return GetActiveVpnConnections(vpnConnectionName).Any();
      //todo: hRasConn = IntPtr.Zero;
    }

    public void ToggleConnection() {
      Task.Run(() => {
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
      return $"Error {errorCode}: {sb}";
    }

    //RasHangUp is asynchronous: the port is released only after the connection state machine terminates
    private static uint HangUp(IntPtr conn) {
      var ret = RasHangUp(conn);
      var status = new RASCONNSTATUS { dwSize = Marshal.SizeOf(typeof(RASCONNSTATUS)) };
      var deadline = DateTime.UtcNow.AddSeconds(5);
      while (DateTime.UtcNow < deadline && RasGetConnectStatus(conn, ref status) == 0) {
        Thread.Sleep(100);
      }
      return ret;
    }

    private static List<RASCONN> EnumConnections() {
      var size = Marshal.SizeOf(typeof(RASCONN));
      var conns = new RASCONN[1];
      conns[0].dwSize = size;
      var cb = size;
      var count = 0;
      var ret = RasEnumConnections(conns, ref cb, ref count);
      if (ret == ERROR_BUFFER_TOO_SMALL) {
        conns = new RASCONN[cb / size + 1];
        for (var i = 0; i < conns.Length; i++)
          conns[i].dwSize = size;
        cb = conns.Length * size;
        ret = RasEnumConnections(conns, ref cb, ref count);
      }
      return ret == 0 ? conns.Take(count).ToList() : new List<RASCONN>();
    }

    //hang up leftovers of previous dials which keep the WAN Miniport port busy
    private void HangUpStaleConnections(string vpnName) {
      if (hRasConn != IntPtr.Zero) {
        HangUp(hRasConn);
        hRasConn = IntPtr.Zero;
      }
      foreach (var conn in EnumConnections()) {
        if (string.Equals(conn.szEntryName, vpnName, StringComparison.OrdinalIgnoreCase))
          HangUp(conn.hrasconn);
      }
    }

    private static bool IsElevated() {
      using (var identity = WindowsIdentity.GetCurrent())
        return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
    }

    //last resort: Windows keeps the port marked as in use until Remote Access Connection Manager is restarted
    private static bool RestartRasMan() {
      if (DateTime.UtcNow - lastRasManRestart < RasManRestartInterval)
        return false;
      lastRasManRestart = DateTime.UtcNow;
      try {
        var elevated = IsElevated();
        var startInfo = new ProcessStartInfo("cmd.exe", "/c net stop RasMan /y & net start RasMan") {
          UseShellExecute = !elevated,
          CreateNoWindow = true,
          WindowStyle = ProcessWindowStyle.Hidden
        };
        if (!elevated)
          startInfo.Verb = "runas";
        using (var process = Process.Start(startInfo)) {
          if (process == null)
            return false;
          if (!process.WaitForExit(60000)) {
            process.Kill();
            return false;
          }
          return process.ExitCode == 0;
        }
      }
      catch (Exception) {
        //UAC prompt was declined or service control is not allowed
        return false;
      }
    }

    private uint Dial(ref RASDIALPARAMS dialParams) {
      var ret = RasDial(IntPtr.Zero, null, ref dialParams, 0, IntPtr.Zero, out IntPtr conn);
      if (ret == 0) {
        hRasConn = conn;
      }
      else if (conn != IntPtr.Zero) {
        //the handle must be released even if dialing failed, otherwise the port stays in use (error 633)
        HangUp(conn);
      }
      return ret;
    }

    private uint DialWithRecovery(ref RASDIALPARAMS dialParams) {
      var ret = Dial(ref dialParams);
      if (ret != ERROR_PORT_NOT_AVAILABLE)
        return ret;

      HangUpStaleConnections(dialParams.szEntryName);
      ret = Dial(ref dialParams);
      if (ret != ERROR_PORT_NOT_AVAILABLE || !RestartRasMan())
        return ret;
      return Dial(ref dialParams);
    }

    private static int RunDialProcess(ProcessStartInfo startInfo) {
      using (var process = Process.Start(startInfo)) {
        if (!process.WaitForExit(60000)) {
          process.Kill();
          return -1;
        }
        return process.ExitCode;
      }
    }

    private string DisconnectFromVpn() {
      if (IsBusy) {
        return "Busy";
      }
      IsBusy = true;
      try {
        if (hRasConn != IntPtr.Zero) {
          uint ret = HangUp(hRasConn);
          hRasConn = IntPtr.Zero;
          if (ret == 0) {
            return null;
          }
          //else {
          //  return GetRasError(ret);
          //}
        }

        var vpnName = mSettingsManager.VpnConnectionName;
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
        var vpnName = mSettingsManager.VpnConnectionName;
        var userName = mSettingsManager.UserName;
        var password = mSettingsManager.Password;

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
          mSettingsManager.UserName = dialParams.szUserName;
          mSettingsManager.Password = dialParams.szPassword;
          var ret = DialWithRecovery(ref dialParams);
          return ret == 0 ? null : GetRasError(ret);
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
          var exitCode = RunDialProcess(procStartInfo);
          //rasdial.exe returns RAS error code as exit code
          if (exitCode == ERROR_PORT_NOT_AVAILABLE) {
            HangUpStaleConnections(vpnName);
            exitCode = RunDialProcess(procStartInfo);
            if (exitCode == ERROR_PORT_NOT_AVAILABLE && RestartRasMan())
              exitCode = RunDialProcess(procStartInfo);
          }
          return exitCode == 0 ? null : "Error";
        }
      }
      catch (Exception ex) {
        return ex.Message;
      }
      finally {
        IsBusy = false;
      }
    }

    public void RestoreConnection() {
      if (!VpnIsConnected() && mSettingsManager.IsConnectionConfigured) {
        ConnectToVpn();
      }
    }
  }
}
