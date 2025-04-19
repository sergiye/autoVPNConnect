using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AutoVPNConnect {

  public partial class AutoVpnConnect : Form {

    private readonly SettingsManager mSettingsManager;
    private readonly ConnectionManager mConnectionManager;
    private readonly NotifyIcon mNotifyIcon;
    private readonly MenuItem menuItemReconnect;
    private readonly MenuItem menuItemConnect;
    private readonly Timer mUpdateUiTimer = new Timer();
    private readonly Icon redIcon;
    private readonly Icon yellowIcon;
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

      menuItemConnect = new MenuItem("Connect", async (s, e) => {
        menuItemConnect.Enabled = false;
        await mConnectionManager.ToggleConnection();
        menuItemConnect.Enabled = !mConnectionManager.IsBusy;
      });
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
        Visible = true
      };
      mNotifyIcon.MouseDoubleClick += menuItemShowHide_Click;

      checkBoxStartWithSystem.Checked = mSettingsManager.GetApplicationStartWithSystem();
      checkBoxApplicationEnabled.Checked = mSettingsManager.GetApplicationEnabledSetting();
      checkBoxStartApplicationMinimized.Checked = mSettingsManager.GetStartApplicationMinimized();
      showApp = !checkBoxStartApplicationMinimized.Checked;

      //Init timer
      mUpdateUiTimer.Interval = 1000;
      mUpdateUiTimer.Enabled = true;
      mUpdateUiTimer.Tick += UpdateUITimer_Tick;
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
      menuItemReconnect.Checked = checkBoxApplicationEnabled.Checked;
    }

    private void checkBoxStartApplicationMinimized_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.SetStartApplicationMinimized(checkBoxStartApplicationMinimized.Checked);
    }

    void UpdateUITimer_Tick(object sender, EventArgs e) {

      var connectionName = mSettingsManager?.GetConnectionName();
      var isConnecting = mConnectionManager?.IsBusy ?? false;
      var isConnected = mConnectionManager?.VpNisConnected() ?? false;
      var isConnectedText = isConnecting ? "Busy" : isConnected ? "Connected" : "Disconnected";
      menuItemConnect.Text = isConnected ? "Disconnect" : "Connect";
      menuItemConnect.Enabled = !string.IsNullOrEmpty(connectionName) && !mConnectionManager.IsBusy;

      lblConnectionName.Text = connectionName;
      lblConnectionStatus.Text = "Connection status: " + isConnectedText;
      lblConnectionStatus.ForeColor = isConnected ? Color.DarkGreen : Color.Red;

      mNotifyIcon.Icon = isConnecting ? yellowIcon : isConnected ? Icon : redIcon;
      mNotifyIcon.Text = "AutoVPNConnect";
      if (!string.IsNullOrEmpty(connectionName)) {
        mNotifyIcon.Text += $"\n{connectionName} - {isConnectedText}";
      }
      //mNotifyIcon.BalloonTipTitle = "AutoVPNConnect";
      //mNotifyIcon.BalloonTipText = "AutoVPNConnect runs in background";
      //mNotifyIcon.ShowBalloonTip(1000);
    }

    private void menuItemShowHide_Click(object sender, EventArgs e) {
      showApp = !showApp;
      Visible = !Visible;
      if (Visible)
        Activate();
    }

    private void toolStripMenuItemExit_Click(object sender, EventArgs e) {
      mNotifyIcon.Visible = false;
      Environment.Exit(0);
    }

    private void AutoVPNConnect_FormClosing(object sender, FormClosingEventArgs e) {
      showApp = false;
      Visible = false;
      e.Cancel = true;
    }
  }
}
