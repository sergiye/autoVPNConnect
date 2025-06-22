using sergiye.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoVPNConnect {

  public partial class MainForm : Form {

    private readonly SettingsManager mSettingsManager;
    private readonly ConnectionManager mConnectionManager;
    private readonly NotifyIcon mNotifyIcon;
    private readonly ToolStripMenuItem menuItemAutoStart;
    private readonly ToolStripMenuItem menuItemReconnect;
    private readonly ToolStripMenuItem menuItemConnect;
    private readonly MenuItem themeMenuItem;
    private readonly Icon greenIcon;
    private readonly Icon redIcon;
    private readonly Icon yellowIcon;
    private bool showApp = false;

    #region form decoration

    private const int SysMenuAboutId = 0x1;
    private const int SysMenuTopMost = 0x2;
    private const int SysMenuAppSite = 0x3;
    private const int SysMenuCheckUpdates = 0x4;

    protected override void OnHandleCreated(EventArgs e) {

      base.OnHandleCreated(e);

      var hSysMenu = WinApiHelper.GetSystemMenu(Handle, false);

      WinApiHelper.DeleteMenu(hSysMenu, WinApiHelper.SC_SIZE, WinApiHelper.MF_BY_COMMAND);
      WinApiHelper.DeleteMenu(hSysMenu, WinApiHelper.SC_MINIMIZE, WinApiHelper.MF_BY_COMMAND);
      WinApiHelper.DeleteMenu(hSysMenu, WinApiHelper.SC_MAXIMIZE, WinApiHelper.MF_BY_COMMAND);

      uint menuIndex = 2;
      WinApiHelper.InsertMenu(hSysMenu, ++menuIndex, WinApiHelper.MF_BY_POSITION | WinApiHelper.MF_SEPARATOR, 0, string.Empty);
      var checkedAttr = TopMost ? WinApiHelper.MF_CHECKED : WinApiHelper.MF_UNCHECKED;
      WinApiHelper.InsertMenu(hSysMenu, ++menuIndex, WinApiHelper.MF_BY_POSITION | checkedAttr, SysMenuTopMost, "Always on top");
      WinApiHelper.InsertMenu(hSysMenu, ++menuIndex, WinApiHelper.MF_BY_POSITION | WinApiHelper.MF_POPUP, (int)themeMenuItem.Handle, themeMenuItem.Text);
      themeMenuItem.Tag = menuIndex;
      WinApiHelper.InsertMenu(hSysMenu, ++menuIndex, WinApiHelper.MF_BY_POSITION, SysMenuAppSite, "Site");
      WinApiHelper.InsertMenu(hSysMenu, ++menuIndex, WinApiHelper.MF_BY_POSITION, SysMenuCheckUpdates, "Check for updates");
      WinApiHelper.InsertMenu(hSysMenu, ++menuIndex, WinApiHelper.MF_BY_POSITION, SysMenuAboutId, "About…");
    }

    protected override void WndProc(ref Message m) {

      base.WndProc(ref m);
      if (m.Msg == WinApiHelper.WM_SYS_COMMAND) {
        switch ((int)m.WParam) {
          case SysMenuAboutId:
            Updater.ShowAbout();
            break;
          case SysMenuCheckUpdates:
            Updater.CheckForUpdates(false);
            break;
          case SysMenuAppSite:
            Updater.VisitAppSite();
            break;
          case SysMenuTopMost:
            ToggleTopMost(null, EventArgs.Empty);
            break;
        }
      }
      else if (m.Msg == WinApiHelper.WM_SHOWME) {
        if (WindowState == FormWindowState.Minimized)
          WindowState = FormWindowState.Normal;
        Activate();
        //bool top = TopMost;
        //TopMost = true;
        //TopMost = top;
      }
    }

    private void ToggleTopMost(object sender, EventArgs e) {
      mSettingsManager.AlwaysOnTop = TopMost = !TopMost;
      var hSysMenu = WinApiHelper.GetSystemMenu(Handle, false);
      WinApiHelper.CheckMenuItem(hSysMenu, SysMenuTopMost, TopMost ? WinApiHelper.MF_CHECKED : WinApiHelper.MF_UNCHECKED);
    }

    private static bool DoSnap(int pos, int edge) {
      return Math.Abs(pos - edge) <= 50;
    }

    private void MainForm_ResizeEnd(object sender, EventArgs e) {
      var scn = Screen.FromPoint(Location);
      if (DoSnap(Left, scn.WorkingArea.Left)) Left = scn.WorkingArea.Left;
      if (DoSnap(Top, scn.WorkingArea.Top)) Top = scn.WorkingArea.Top;
      if (DoSnap(scn.WorkingArea.Right, Right)) Left = scn.WorkingArea.Right - Width;
      if (DoSnap(scn.WorkingArea.Bottom, Bottom)) Top = scn.WorkingArea.Bottom - Height;
    }

    #endregion form decoration

    public MainForm() {
      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
      greenIcon = this.Icon;
      Text = Updater.ApplicationTitle;
      themeMenuItem = new MenuItem("&Themes");

      mSettingsManager = new SettingsManager();
      mConnectionManager = new ConnectionManager(ref mSettingsManager);
      mConnectionManager.OnStatusChanged += UpdateUI;

      var assembly = Assembly.GetExecutingAssembly();
      using (var st = assembly.GetManifestResourceStream("AutoVPNConnect.Resources.Red.ico")) {
        redIcon = new Icon(st);
      }
      using (var st = assembly.GetManifestResourceStream("AutoVPNConnect.Resources.Yellow.ico")) {
        yellowIcon = new Icon(st);
      }

      TopMost = mSettingsManager.AlwaysOnTop;
      if (mSettingsManager.IsConnectionConfigured == false) {
        lblConnectionStatus.Text = "Connection status: Disconnected";
      }

      menuItemAutoStart = new ToolStripMenuItem(cbxAutoStart.Text, null, (s, e) => { cbxAutoStart.Checked = !cbxAutoStart.Checked; });
      menuItemReconnect = new ToolStripMenuItem("Restore connection", null, menuReconnect_Click);
      menuItemConnect = new ToolStripMenuItem("Connect", null, btnToggle_Click);
      btnToggle.Click += btnToggle_Click;
      mNotifyIcon = new NotifyIcon(components) {
        ContextMenuStrip = new ContextMenuStrip(),
        Icon = yellowIcon ?? Icon,
      };
      mNotifyIcon.ContextMenuStrip.Items.AddRange(
        new ToolStripItem[] {
          new ToolStripMenuItem("Show/Hide", null, menuItemShowHide_Click),
          menuItemConnect,
          new ToolStripSeparator(),
          menuItemAutoStart,
          menuItemReconnect,
          new ToolStripSeparator(),
          new ToolStripMenuItem("Site", null, (s, e) => Updater.VisitAppSite()),
          new ToolStripMenuItem("Check for updates", null, (s, e) => Updater.CheckForUpdates(false)),
          new ToolStripMenuItem("About…", null, (s, e) => Updater.ShowAbout()),
          new ToolStripSeparator(),
          new ToolStripMenuItem("Exit", null, toolStripMenuItemExit_Click)
        }
      );

      mNotifyIcon.MouseDoubleClick += menuItemShowHide_Click;
      cbxAutoStart.Checked = mSettingsManager.AutoStartApp;
      cbxReconnect.Checked = mSettingsManager.Reconnect;
      cbxRunInBackground.Checked = mSettingsManager.RunInBackground;
      showApp = !cbxRunInBackground.Checked;

      Updater.Subscribe(
        (message, isError) => { MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information); },
        (message) => { return MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK; },
        Application.Exit
      );
      var timer = new Timer();
      timer.Tick += async (_, _) => {
        timer.Enabled = false;
        timer.Enabled = !await Updater.CheckForUpdatesAsync(true);
      };
      timer.Interval = 3000;
      timer.Enabled = true;

      InitializeTheme();
    }

    #region themes

    private void OnThemeCurrentChanged() {
      UpdateUI();
    }

    private void InitializeTheme() {

      mNotifyIcon.ContextMenuStrip.Renderer = new ThemedToolStripRenderer();

      Theme.OnCurrentChanged -= OnThemeCurrentChanged;

      var menuHandle = WinApiHelper.GetSystemMenu(Handle, false);
      WinApiHelper.RemoveMenu(menuHandle, (uint)themeMenuItem.Tag, WinApiHelper.MF_BY_POSITION);
      themeMenuItem.MenuItems.Clear();

      if (Theme.SupportsAutoThemeSwitching()) {
        themeMenuItem.MenuItems.Add(new RadioButtonMenuItem("Auto", (o, e) => {
          (o as RadioButtonMenuItem).Checked = true;
          Theme.SetAutoTheme();
          mSettingsManager.Theme = "auto";
        }));
      }

      var settingsTheme = mSettingsManager.Theme;
      var allThemes = CustomTheme.GetAllThemes("themes", "AutoVPNConnect.themes").OrderBy(x => x.DisplayName).ToList();
      var setTheme = allThemes.FirstOrDefault(theme => settingsTheme == theme.Id);
      if (setTheme != null) {
        Theme.Current = setTheme;
      }

      AddThemeMenuItems(allThemes.Where(t => t is not CustomTheme));
      var customThemes = allThemes.Where(t => t is CustomTheme).ToList();
      if (customThemes.Count > 0) {
        themeMenuItem.MenuItems.Add("-");
        AddThemeMenuItems(customThemes);
      }

      if (setTheme == null && themeMenuItem.MenuItems.Count > 0)
        themeMenuItem.MenuItems[0].PerformClick();
      WinApiHelper.InsertMenu(menuHandle, (uint)themeMenuItem.Tag, WinApiHelper.MF_BY_POSITION | WinApiHelper.MF_POPUP, (int)themeMenuItem.Handle, themeMenuItem.Text);

      Theme.Current.Apply(this);
      OnThemeCurrentChanged();
      Theme.OnCurrentChanged += OnThemeCurrentChanged;
    }

    private void AddThemeMenuItems(IEnumerable<Theme> themes) {
      foreach (var theme in themes) {
        var item = new RadioButtonMenuItem(theme.DisplayName, OnThemeMenuItemClick);
        themeMenuItem.MenuItems.Add(item);
        item.Tag = theme;
        if (Theme.Current != null && Theme.Current.Id == theme.Id) {
          item.Checked = true;
        }
      }
    }

    private void OnThemeMenuItemClick(object sender, EventArgs e) {
      if (sender is not RadioButtonMenuItem item || item.Tag is not Theme theme)
        return;
      item.Checked = true;
      Theme.Current = theme;
      mSettingsManager.Theme = theme.Id;
    }

    #endregion

    private void btnToggle_Click(object sender, EventArgs e) {
      btnToggle.Enabled = menuItemConnect.Enabled = false;
      mConnectionManager.ToggleConnection();
    }

    protected override void SetVisibleCore(bool value) {
      base.SetVisibleCore(showApp ? value : showApp);
    }
    
    private void comboBoxActiveVPNConnections_DropDown(object sender, EventArgs e) {
      cmbConnections.Items.Clear();
      var vpnConnections = ConnectionManager.GetActiveVpnConnections();
      foreach (var @interface in vpnConnections) {
        cmbConnections.Items.Add(@interface.Name);
      }
      if (cmbConnections.Items.Count == 0) {
        if (MessageBox.Show("Connect to a VPN first?", Updater.ApplicationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
          Process.Start(new ProcessStartInfo("NCPA.cpl") { UseShellExecute = true });
        }
        else {
          var connectionName = mSettingsManager.VpnConnectionName;
          if (!string.IsNullOrEmpty(connectionName))
            cmbConnections.Items.Add(connectionName);
        }
      }
    }

    private void btnSaveSettings_Click(object sender, EventArgs e) {
      var vpnConnectionName = cmbConnections.Text;
      var userName = textBoxUsername.Text;
      var password = textBoxPassword.Text;

      if (string.IsNullOrEmpty(vpnConnectionName)) {
        MessageBox.Show("Connection is not configured", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      else {
        mSettingsManager.VpnConnectionName = vpnConnectionName;
        mSettingsManager.UserName = userName;
        mSettingsManager.Password = password;

        MessageBox.Show("Settings successfully saved.\n" +
        $"{Updater.ApplicationTitle} will automatically connect to: " +
        vpnConnectionName + "\nIf this does not work, enter your username and password again.",
          Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void cbxAutoStart_CheckedChanged(object sender, EventArgs e) {
      menuItemAutoStart.Checked = mSettingsManager.AutoStartApp = cbxAutoStart.Checked;
    }

    private void cbxReconnect_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.Reconnect = cbxReconnect.Checked;
      menuItemReconnect.Checked = cbxReconnect.Checked;
      if (cbxReconnect.Checked) {
        Task.Run(() => {
          mConnectionManager?.RestoreConnection();
        });
      }
    }

    private void cbxRunInBackground_CheckedChanged(object sender, EventArgs e) {
      mSettingsManager.RunInBackground = cbxRunInBackground.Checked;
      mNotifyIcon.Visible = cbxRunInBackground.Checked;
    }

    private void UpdateUI() {

      if (this.InvokeRequired) {
        if (InvokeRequired) {
          Invoke(new Action(UpdateUI));
          return;
        }
      }

      var connectionName = mSettingsManager?.VpnConnectionName;
      var isConnecting = mConnectionManager?.IsBusy ?? false;
      var isConnected = mConnectionManager?.VpnIsConnected() ?? false;
      var isConnectedText = isConnecting ? "Busy" : isConnected ? "Connected" : "Disconnected";
      btnToggle.Text = menuItemConnect.Text = isConnected ? "Disconnect" : "Connect";
      btnToggle.Enabled = menuItemConnect.Enabled = !string.IsNullOrEmpty(connectionName) && !mConnectionManager.IsBusy;

      if (!string.IsNullOrEmpty(connectionName)) {
        cmbConnections.Items.Add(connectionName);
        cmbConnections.SelectedIndex = 0;
      }

      lblConnectionStatus.Text = "Connection status: " + isConnectedText;
      lblConnectionStatus.ForeColor = isConnecting ? Theme.Current.InfoColor : isConnected ? Theme.Current.MessageColor : Theme.Current.WarnColor;

      this.Icon = mNotifyIcon.Icon = isConnecting ? yellowIcon : isConnected ? greenIcon : redIcon;
      mNotifyIcon.Text = Updater.ApplicationTitle;
      if (!string.IsNullOrEmpty(connectionName)) {
        mNotifyIcon.Text += $"\n{connectionName} - {isConnectedText}";
      }
      //mNotifyIcon.BalloonTipTitle = Updater.ApplicationTitle;
      //mNotifyIcon.BalloonTipText = $"{Updater.ApplicationTitle} runs in background";
      //mNotifyIcon.ShowBalloonTip(1000);

      if (!string.IsNullOrEmpty(mSettingsManager.UserName))
        textBoxUsername.Text = mSettingsManager.UserName;
      //if (!string.IsNullOrEmpty(mSettingsManager.Password))
      //  textBoxPassword.Text = mSettingsManager.Password;
    }

    private void menuReconnect_Click(object sender, EventArgs e) {
      cbxReconnect.Checked = !cbxReconnect.Checked;
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
      if (cbxRunInBackground.Checked) {
        showApp = false;
        Visible = false;
        e.Cancel = true;
      }
    }
  }
}
