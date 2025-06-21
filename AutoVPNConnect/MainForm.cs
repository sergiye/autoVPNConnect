using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AutoVPNConnect {

  public partial class MainForm : Form {

    private readonly SettingsManager mSettingsManager;
    private readonly ConnectionManager mConnectionManager;
    private readonly NotifyIcon mNotifyIcon;
    private readonly MenuItem menuItemReconnect;
    private readonly MenuItem menuItemConnect;
    private readonly Timer mUpdateUiTimer = new Timer();
    private readonly Icon redIcon;
    private readonly Icon yellowIcon;
    private bool showApp = false;

    public MainForm() {
      InitializeComponent();
      
      Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
      mSettingsManager = new SettingsManager();
      mConnectionManager = new ConnectionManager(ref mSettingsManager);

      var assembly = Assembly.GetExecutingAssembly();
      using (var st = assembly.GetManifestResourceStream("AutoVPNConnect.Red.ico")) {
        redIcon = new Icon(st);
      }
      using (var st = assembly.GetManifestResourceStream("AutoVPNConnect.Yellow.ico")) {
        yellowIcon = new Icon(st);
      }

      //Check if there is already a running instance
      if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(assembly.Location)).Length > 1) {
        MessageBox.Show("AutoVPNConnect is already running.\nIt is recommended to close this instance.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      //Go to Settings tab when no settings found
      if (SettingsManager.ValidSettingsFound() == false) {
        tabControl.SelectedTab = tabPage2;
        lblConnectionStatus.Text = "Connection status: Disconnected";
      }

      menuItemReconnect = new MenuItem("Auto connect", (s, e) => {
        checkBoxApplicationEnabled.Checked = !checkBoxApplicationEnabled.Checked;
      }) {
        Checked = mSettingsManager.GetApplicationEnabledSetting(),
      };

      btnToggle.Click += btnToggle_Click;
      menuItemConnect = new MenuItem("Connect", btnToggle_Click);
      mNotifyIcon = new NotifyIcon(components) {
        ContextMenu = new ContextMenu(new MenuItem[] {
            new MenuItem("Show/Hide", menuItemShowHide_Click) {
              DefaultItem = true
            },
            menuItemReconnect,
            menuItemConnect,
            new MenuItem("-"),
            new MenuItem("Exit", toolStripMenuItemExit_Click)}),
        Icon = yellowIcon ?? Icon,
      };
      mNotifyIcon.MouseDoubleClick += menuItemShowHide_Click;

      checkBoxStartWithSystem.Checked = mSettingsManager.GetApplicationStartWithSystem();
      checkBoxApplicationEnabled.Checked = mSettingsManager.GetApplicationEnabledSetting();
      checkBoxStartApplicationMinimized.Checked = mSettingsManager.GetStartApplicationMinimized();
      showApp = !checkBoxStartApplicationMinimized.Checked;

      //Init timer
      mUpdateUiTimer.Interval = 1500;
      mUpdateUiTimer.Tick += UpdateUITimer_Tick;
      UpdateUITimer_Tick(null, EventArgs.Empty);
    }

    private async void btnToggle_Click(object sender, EventArgs e) {
      btnToggle.Enabled = menuItemConnect.Enabled = false;
      await mConnectionManager.ToggleConnection();
    }

    protected override void SetVisibleCore(bool value) {
      base.SetVisibleCore(showApp ? value : showApp);
    }
    
    private void comboBoxActiveVPNConnections_DropDown(object sender, EventArgs e) {
      comboBoxActiveVPNConnections.Items.Clear();
      var vpnConnections = ConnectionManager.GetActiveVpnConnections();
      foreach (var @interface in vpnConnections) {
        comboBoxActiveVPNConnections.Items.Add(@interface.Name);
      }
      if (comboBoxActiveVPNConnections.Items.Count == 0) {
        MessageBox.Show("Connect to a VPN first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        try {
          Process.Start(new ProcessStartInfo("NCPA.cpl") { UseShellExecute = true });
        }
        catch {
          //ignore
        }
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
      menuItemReconnect.Checked = checkBoxApplicationEnabled.Checked;
    }

    private void checkBoxStartApplicationMinimized_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.SetStartApplicationMinimized(checkBoxStartApplicationMinimized.Checked);
      mNotifyIcon.Visible = checkBoxStartApplicationMinimized.Checked;
    }

    void UpdateUITimer_Tick(object sender, EventArgs e) {

      mUpdateUiTimer.Enabled = false;
      var connectionName = mSettingsManager?.GetConnectionName();
      var isConnecting = mConnectionManager?.IsBusy ?? false;
      var isConnected = mConnectionManager?.VpnIsConnected() ?? false;
      var isConnectedText = isConnecting ? "Busy" : isConnected ? "Connected" : "Disconnected";
      btnToggle.Text = menuItemConnect.Text = isConnected ? "Disconnect" : "Connect";
      btnToggle.Enabled = menuItemConnect.Enabled = !string.IsNullOrEmpty(connectionName) && !mConnectionManager.IsBusy;

      lblConnectionName.Text = connectionName;
      lblConnectionStatus.Text = "Connection status: " + isConnectedText;
      lblConnectionStatus.ForeColor = isConnecting ? Color.DarkGoldenrod : isConnected ? Color.DarkGreen : Color.Red;

      mNotifyIcon.Icon = isConnecting ? yellowIcon : isConnected ? Icon : redIcon;
      mNotifyIcon.Text = "AutoVPNConnect";
      if (!string.IsNullOrEmpty(connectionName)) {
        mNotifyIcon.Text += $"\n{connectionName} - {isConnectedText}";
      }
      //mNotifyIcon.BalloonTipTitle = "AutoVPNConnect";
      //mNotifyIcon.BalloonTipText = "AutoVPNConnect runs in background";
      //mNotifyIcon.ShowBalloonTip(1000);
      
      mUpdateUiTimer.Enabled = true;
    }

    private void menuItemShowHide_Click(object sender, EventArgs e) {
      showApp = !showApp;
      Visible = !Visible;
      if (Visible)
        Activate();
    }

    private void toolStripMenuItemExit_Click(object sender, EventArgs e) {
      mNotifyIcon.Visible = false;
      Close();
    }

    private void AutoVPNConnect_FormClosing(object sender, FormClosingEventArgs e) {
      if (mNotifyIcon.Visible) {
        showApp = false;
        Visible = false;
        e.Cancel = true;
      }
    }
  }
}
