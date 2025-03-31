using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AutoVPNConnect {
  public partial class AutoVpnConnect : Form {
    private readonly SettingsManager mSettingsManager;
    private readonly ConnectionManager mConnectionManager;
    private readonly Timer mUpdateUiTimer = new Timer();

    public AutoVpnConnect() {
      InitializeComponent();
      mSettingsManager = new SettingsManager();
      mConnectionManager = new ConnectionManager(ref mSettingsManager);

      //Check if there is already a running instance
      if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 1) {
        MessageBox.Show("AutoVPNConnect is already running.\nIt is recommended to close this instance.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      //Init timer
      mUpdateUiTimer.Interval = 3000;
      mUpdateUiTimer.Enabled = true;
      mUpdateUiTimer.Tick += UpdateUITimer_Tick;

      InitUi();
    }

    private void InitUi() {
      //Go to Settings tab when no settings found
      if (SettingsManager.ValidSettingsFound() == false) {
        tabControl.SelectedTab = tabPage2;
      }
      else {
        lblConnectionName.Text = mSettingsManager.GetConnectionName();
        lblAppEnabled.Text = "Application enabled: " + mSettingsManager.GetApplicationEnabledSetting().ToString();
      }

      lblConnectionStatus.Text = mConnectionManager.VpNisConnected() ? "Connection status: Connected" : "Connection status: Disconnected";

      checkBoxStartWithSystem.Checked = mSettingsManager.GetApplicationStartWithSystem();
      checkBoxShowMessages.Checked = mSettingsManager.GetShowMessagesSetting();
      checkBoxApplicationEnabled.Checked = mSettingsManager.GetApplicationEnabledSetting();
      checkBoxStartApplicationMinimized.Checked = mSettingsManager.GetStartApplicationMinimized();
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

    private void checkBoxShowMessages_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.SetShowMessages(checkBoxShowMessages.Checked);
    }

    private void checkBoxApplicationEnabled_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.SetApplicationEnabledSetting(checkBoxApplicationEnabled.Checked);
    }

    private void checkBoxStartApplicationMinimized_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.SetStartApplicationMinimized(checkBoxStartApplicationMinimized.Checked);
    }

    void UpdateUITimer_Tick(object sender, EventArgs e) {
      UpdateUi();
    }

    private void UpdateUi() {
      lblConnectionName.Text = mSettingsManager.GetConnectionName();
      lblAppEnabled.Text = "Application enabled: " + mSettingsManager.GetApplicationEnabledSetting().ToString();

      if (mConnectionManager.VpNisConnected()) {
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
        mUpdateUiTimer.Enabled = false;

        if (mSettingsManager.GetShowMessagesSetting()) {
          mNotifyIcon.Text = "AutoVPNConnect";
          mNotifyIcon.BalloonTipTitle = "AutoVPNConnect";
          mNotifyIcon.BalloonTipText = "AutoVPNConnect runs in background";
          mNotifyIcon.ShowBalloonTip(1000);
        }
      }
      else {
        ShowInTaskbar = true;
        mNotifyIcon.Visible = false;
        mUpdateUiTimer.Enabled = true;
        UpdateUi();
      }
    }

    private void AutoVPNConnect_Load(object sender, EventArgs e) {
      if (mSettingsManager.GetStartApplicationMinimized()) {
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
