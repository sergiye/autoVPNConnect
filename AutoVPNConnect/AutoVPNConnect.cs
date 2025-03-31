﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AutoVPNConnect {

  public partial class AutoVpnConnect : Form {

    private readonly SettingsManager mSettingsManager;
    private readonly ConnectionManager mConnectionManager;
    private readonly Timer mUpdateUiTimer = new Timer();
    private readonly Icon redIcon;
    private bool canClose = false;
    private bool showApp = false;

    public AutoVpnConnect() {
      InitializeComponent();
      
      Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
      mSettingsManager = new SettingsManager();
      mConnectionManager = new ConnectionManager(ref mSettingsManager);

      var assembly = Assembly.GetExecutingAssembly();
      using (var st = assembly.GetManifestResourceStream("AutoVPNConnect.Red.ico")) {
        redIcon = new Icon(st);
      }

      //Check if there is already a running instance
      if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(assembly.Location)).Length > 1) {
        MessageBox.Show("AutoVPNConnect is already running.\nIt is recommended to close this instance.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      //Init timer
      mUpdateUiTimer.Interval = 1000;
      mUpdateUiTimer.Enabled = true;
      mUpdateUiTimer.Tick += UpdateUITimer_Tick;

      //Go to Settings tab when no settings found
      if (SettingsManager.ValidSettingsFound() == false) {
        tabControl.SelectedTab = tabPage2;
        lblConnectionStatus.Text = "Connection status: Disconnected";
      }

      checkBoxStartWithSystem.Checked = mSettingsManager.GetApplicationStartWithSystem();
      checkBoxApplicationEnabled.Checked = mSettingsManager.GetApplicationEnabledSetting();
      checkBoxStartApplicationMinimized.Checked = mSettingsManager.GetStartApplicationMinimized();
      showApp = !checkBoxStartApplicationMinimized.Checked;
    }

    protected override void SetVisibleCore(bool value) {
      base.SetVisibleCore(showApp ? value : showApp);
    }
    
    private void comboBoxActiveVPNConnections_DropDown(object sender, EventArgs e) {
      comboBoxActiveVPNConnections.Items.Clear();
      var vpnConnections = ConnectionManager.GetActiveVpnConnections();

      if (vpnConnections.Count == 0) {
        MessageBox.Show("Connect to a VPN first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        try {
          var startInfo = new ProcessStartInfo("NCPA.cpl");
          startInfo.UseShellExecute = true;
          Process.Start(startInfo);
        }
        catch {
          //ignore
        }
      }

      foreach (var @interface in vpnConnections) {
        comboBoxActiveVPNConnections.Items.Add(@interface.Name);
      }
    }

    private void btnSaveSettings_Click(object sender, EventArgs e) {
      var vpnConnectionName = comboBoxActiveVPNConnections.Text;
      var username = textBoxUsername.Text;
      var password = textBoxPassword.Text;

      if (vpnConnectionName == "") {
        MessageBox.Show("Invalid input", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      else {
        mSettingsManager.SetConnectionName(vpnConnectionName);
        mSettingsManager.SetUserName(username);
        mSettingsManager.SetPassword(password);

        MessageBox.Show("Settings successfully saved.\n" +
        "AutoVPNConnect will automatically connect to VPN connection: " +
        vpnConnectionName + "\nWhen this is not working, enter your username and password again.", 
          "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void checkBoxStartWithSystem_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.SetApplicationStartWithSystem(checkBoxStartWithSystem.Checked);
    }

    private void checkBoxApplicationEnabled_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.SetApplicationEnabledSetting(checkBoxApplicationEnabled.Checked);
    }

    private void checkBoxStartApplicationMinimized_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.SetStartApplicationMinimized(checkBoxStartApplicationMinimized.Checked);
    }

    void UpdateUITimer_Tick(object sender, EventArgs e) {
      lblConnectionName.Text = mSettingsManager?.GetConnectionName();
      var isConnected = mConnectionManager?.VpNisConnected() ?? false;
      mNotifyIcon.Icon = isConnected ? Icon : redIcon;
      lblConnectionStatus.Text = isConnected ? "Connection status: Connected" : "Connection status: Disconnected";
      lblConnectionStatus.ForeColor = isConnected ? Color.DarkGreen : Color.Red;
    }

    private void mNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
      showApp = !showApp;
      Visible = !Visible;
      if (Visible)
        Activate();
    }

    private void toolStripMenuItemExit_Click(object sender, EventArgs e) {
      canClose = true;
      Close();
    }

    private void AutoVPNConnect_FormClosing(object sender, FormClosingEventArgs e) {
      showApp = false;
      Visible = false;
      e.Cancel = !canClose;
    }
  }
}
