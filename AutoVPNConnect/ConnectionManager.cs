using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace AutoVPNConnect {
  class ConnectionManager {
    readonly SettingsManager mSettingsManager;
    readonly Timer VPNConnectionCheckTimer = new Timer();

    public ConnectionManager(ref SettingsManager rSettingsManager) {
      mSettingsManager = rSettingsManager;

      //Init timer
      VPNConnectionCheckTimer.Interval = 15000;
      VPNConnectionCheckTimer.Enabled = true;
      VPNConnectionCheckTimer.Tick += new EventHandler(VPNConnectionCheckTimer_Tick);
    }

    public List<NetworkInterface> getActiveVPNConnections() {
      List<NetworkInterface> VPNConnections = new List<NetworkInterface>();

      if (!NetworkInterface.GetIsNetworkAvailable()) {
        return VPNConnections;
      }

      NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

      foreach (NetworkInterface Interface in interfaces) {
        //Get VPN connections
        if ((Interface.NetworkInterfaceType == NetworkInterfaceType.Ppp) &&
        (Interface.NetworkInterfaceType != NetworkInterfaceType.Loopback)) {
          VPNConnections.Add(Interface);
        }
      }
      return VPNConnections;
    }

    public bool VPNisConnected() {
      string VPNConnectionName = mSettingsManager.getConnectionName();

      List<NetworkInterface> VPNConnections = getActiveVPNConnections();

      foreach (NetworkInterface Interface in VPNConnections) {
        if (Interface.Name == VPNConnectionName) {
          return Interface.OperationalStatus == OperationalStatus.Up;
        }
      }
      return false;
    }

    private void ConnectToVPN() {

      var vpnName = mSettingsManager.getConnectionName();
      var userName = mSettingsManager.getUserName();
      var password = mSettingsManager.getPassword();

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
      Process process = new Process();
      process.StartInfo = procStartInfo;
      process.Start();
    }

    public void CheckConnectionStatus() {
      if (mSettingsManager.getApplicationEnabledSetting()) {
        if (VPNisConnected() == false && mSettingsManager.validSettingsFound()) {
          ConnectToVPN();
        }
      }
    }

    void VPNConnectionCheckTimer_Tick(object sender, EventArgs e) {
      CheckConnectionStatus();
    }
  }
}
