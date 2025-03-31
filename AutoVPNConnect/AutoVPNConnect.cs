using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace AutoVPNConnect {
  public partial class AutoVPNConnect : Form {
    private readonly SettingsManager mSettingsManager;
    private readonly ConnectionManager mConnectionManager;
    private readonly Timer mUpdateUITimer = new Timer();

    public AutoVPNConnect() {
      InitializeComponent();
      mSettingsManager = new SettingsManager();
      mConnectionManager = new ConnectionManager(ref mSettingsManager);

      //Check if there is already a running instance
      if (Process.GetProcessesByName
      (System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1) {
        MessageBox.Show("AutoVPNConnect is already running.\nIt is recommended to close this instance.", "Warning");
      }

      //Init timer
      mUpdateUITimer.Interval = 3000;
      mUpdateUITimer.Enabled = true;
      mUpdateUITimer.Tick += new EventHandler(UpdateUITimer_Tick);

      InitUI();
    }

    private void InitUI() {
      //Go to Settings tab when no settings found
      if (mSettingsManager.validSettingsFound() == false) {
        tabControl.SelectedTab = tabPage2;
      }
      else {
        lblConnectionName.Text = mSettingsManager.getConnectionName();
        lblAppEnabled.Text = "Application enabled: " + mSettingsManager.getApplicationEnabledSetting().ToString();
      }

      lblConnectionStatus.Text = mConnectionManager.VPNisConnected() ? "Connection status: Connected" : "Connection status: Disconnected";

      checkBoxStartWithSystem.Checked = mSettingsManager.getApplicationStartWithSystem();
      checkBoxShowMessages.Checked = mSettingsManager.getShowMessagesSetting();
      checkBoxApplicationEnabled.Checked = mSettingsManager.getApplicationEnabledSetting();
      checkBoxStartApplicationMinimized.Checked = mSettingsManager.getStartApplicationMinimized();
    }

    private void comboBoxActiveVPNConnections_DropDown(object sender, EventArgs e) {
      comboBoxActiveVPNConnections.Items.Clear();
      List<NetworkInterface> VPNConnections = mConnectionManager.getActiveVPNConnections();

      if (VPNConnections.Count == 0) {
        MessageBox.Show("Connect to a VPN first");
        try {
          ProcessStartInfo startInfo = new ProcessStartInfo("NCPA.cpl");
          startInfo.UseShellExecute = true;
          Process.Start(startInfo);
        }
        catch {
          ;
        }
      }

      foreach (NetworkInterface Interface in VPNConnections) {
        comboBoxActiveVPNConnections.Items.Add(Interface.Name);
      }
    }

    private void btnSaveSettings_Click(object sender, EventArgs e) {
      string vpnConnectionName = comboBoxActiveVPNConnections.Text;
      string username = textBoxUsername.Text;
      string password = textBoxPassword.Text;

      if (vpnConnectionName == "") {
        MessageBox.Show("Invalid input");
      }
      else {
        mSettingsManager.setConnectionName(vpnConnectionName);
        mSettingsManager.setUserName(username);
        mSettingsManager.setPassword(password);

        MessageBox.Show("Settings successfully saved.\n" +
        "AutoVPNConnect will automatically connect to VPN connection: " +
        vpnConnectionName + "\nWhen this is not working, enter your username and password again.");
      }
    }

    private void checkBoxStartWithSystem_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.setApplicationStartWithSystem(checkBoxStartWithSystem.Checked);
    }

    private void checkBoxShowMessages_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.setShowMessages(checkBoxShowMessages.Checked);
    }

    private void checkBoxApplicationEnabled_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.setApplicationEnabledSetting(checkBoxApplicationEnabled.Checked);
    }

    private void checkBoxStartApplicationMinimized_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.setStartApplicationMinimized(checkBoxStartApplicationMinimized.Checked);
    }

    void UpdateUITimer_Tick(object sender, EventArgs e) {
      UpdateUI();
    }

    private void UpdateUI() {
      lblConnectionName.Text = mSettingsManager.getConnectionName();
      lblAppEnabled.Text = "Application enabled: " + mSettingsManager.getApplicationEnabledSetting().ToString();

      if (mConnectionManager.VPNisConnected()) {
        lblConnectionStatus.Text = "Connection status: Connected";
      }
      else {
        lblConnectionStatus.Text = "Connection status: Disconnected";
      }
    }

    private void AutoVPNConnect_Resize(object sender, EventArgs e) {
      if (WindowState == FormWindowState.Minimized) {
        mNotifyIcon.Visible = true;
        ShowInTaskbar = false;
        mUpdateUITimer.Enabled = false;

        if (mSettingsManager.getShowMessagesSetting() == true) {
          mNotifyIcon.Text = "AutoVPNConnect";
          mNotifyIcon.BalloonTipTitle = "AutoVPNConnect";
          mNotifyIcon.BalloonTipText = "AutoVPNConnect runs in background";
          mNotifyIcon.ShowBalloonTip(1000);
        }
      }
      else {
        ShowInTaskbar = true;
        mNotifyIcon.Visible = false;
        mUpdateUITimer.Enabled = true;
        UpdateUI();
      }
    }

    private void AutoVPNConnect_Load(object sender, EventArgs e) {
      if (mSettingsManager.getStartApplicationMinimized() == true) {
        WindowState = FormWindowState.Minimized;
      }
    }

    private void mNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
      WindowState = FormWindowState.Normal;
      ShowInTaskbar = true;
      mNotifyIcon.Visible = false;
    }

    private void contextMenuStrip_Click(object sender, EventArgs e) {
      Close();
    }

    private void AutoVPNConnect_FormClosing(object sender, FormClosingEventArgs e) {
      mNotifyIcon.Visible = false;
    }
  }
}
