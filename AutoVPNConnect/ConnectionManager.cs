using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoVPNConnect {
  class ConnectionManager {
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

    public static List<NetworkInterface> GetActiveVpnConnections() {
      var vpnConnections = new List<NetworkInterface>();

      if (!NetworkInterface.GetIsNetworkAvailable()) {
        return vpnConnections;
      }

      var interfaces = NetworkInterface.GetAllNetworkInterfaces();

      foreach (var @interface in interfaces) {
        //Get VPN connections
        if ((@interface.NetworkInterfaceType == NetworkInterfaceType.Ppp) &&
        (@interface.NetworkInterfaceType != NetworkInterfaceType.Loopback)) {
          vpnConnections.Add(@interface);
        }
      }
      return vpnConnections;
    }

    public bool VpNisConnected() {
      var vpnConnectionName = mSettingsManager.GetConnectionName();

      var vpnConnections = GetActiveVpnConnections();

      foreach (var @interface in vpnConnections) {
        if (@interface.Name == vpnConnectionName) {
          return @interface.OperationalStatus == OperationalStatus.Up;
        }
      }
      return false;
    }

    public Task ToggleConnection() {
      return Task.Run(() => {
        if (VpNisConnected()) {
          DisconnectFromVpn();
        }
        else {
          ConnectToVpn();
        }
      });
    }

    private void DisconnectFromVpn() {
      if (IsBusy) return;
      IsBusy = true;
      var vpnName = mSettingsManager.GetConnectionName();
      var process = Process.Start(new ProcessStartInfo("rasdial.exe", $" \u0022{vpnName}\u0022 /disconnect") {
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden
      });
      if (!process.WaitForExit(60000))
        process.Kill();
      IsBusy = false;
    }

    private void ConnectToVpn() {
      if (IsBusy) return;
      IsBusy = true;
      var vpnName = mSettingsManager.GetConnectionName();
      var userName = mSettingsManager.GetUserName();
      var password = mSettingsManager.GetPassword();

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

      // Do not create the black window.
      procStartInfo.CreateNoWindow = true;
      procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      //Start the process
      var process = Process.Start(procStartInfo);
      if (!process.WaitForExit(60000))
        process.Kill();
      IsBusy = false;
    }

    void VPNConnectionCheckTimer_Tick(object sender, EventArgs e) {
      if (!mSettingsManager.GetApplicationEnabledSetting()) return;
      if (!VpNisConnected() && SettingsManager.ValidSettingsFound()) {
        ConnectToVpn();
      }
    }
  }
}
