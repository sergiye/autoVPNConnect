using Microsoft.Win32;
using sergiye.Common;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AutoVPNConnect {
  class SettingsManager {
    private static string RegistryPath = $@"Software\{Updater.ApplicationCompany}\{Updater.ApplicationName}";

    private string vpnConnectionName = "No settings found";
    private string username = "";
    private string password = "";
    private string theme = "";
    private bool reconnect;
    private bool runInBackground;
    private bool alwaysOnTop;

    private static readonly string encryptionKey = "123456789012345678901234";

    public SettingsManager() {
      ReadSettings();
    }

    internal bool IsConnectionConfigured => !string.IsNullOrEmpty(VpnConnectionName) && VpnConnectionName != "No settings found";

    private void ReadSettings() {
      var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
      if (key == null) {
        return;
      }
      try {
        vpnConnectionName = key.GetValue("VPNConnectionName")?.ToString();
        username = key.GetValue("Username")?.ToString() ?? "";
        var encryptedPassword = key.GetValue("Password")?.ToString();
        password = Decrypt(encryptedPassword) ?? "";
        
        theme = key.GetValue("Theme")?.ToString();

        reconnect = key.GetValue("ApplicationEnabled")?.ToString() == "True";
        runInBackground = key.GetValue("StartApplicationMinimized")?.ToString() == "True";
        alwaysOnTop = key.GetValue("AlwaysOnTop")?.ToString() == "True";

        key.Close();
      }
      catch {
        key.Close();
      }
    }

    private void WriteSettings() {

      try {
        Registry.CurrentUser.DeleteSubKey(RegistryPath, false);
        using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath, true)) {
          if (key != null) {
            if (!string.IsNullOrEmpty(vpnConnectionName))
              key.SetValue("VPNConnectionName", vpnConnectionName, RegistryValueKind.String);
            if (!string.IsNullOrEmpty(username))
              key.SetValue("Username", username, RegistryValueKind.String);
            var encryptedPassword = Encrypt(password);
            if (!string.IsNullOrEmpty(encryptedPassword))
              key.SetValue("Password", encryptedPassword, RegistryValueKind.String);
            if (!string.IsNullOrEmpty(theme))
              key.SetValue("Theme", theme, RegistryValueKind.String);
            key.SetValue("ApplicationEnabled", reconnect, RegistryValueKind.String);
            key.SetValue("StartApplicationMinimized", runInBackground, RegistryValueKind.String);
            key.SetValue("AlwaysOnTop", alwaysOnTop, RegistryValueKind.String);
          }
          else {
            throw new Exception("Registry unavailable");
          }
        }
      }
      catch(Exception ex) {
        MessageBox.Show("Error while writing settings: " + ex.Message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private static string Encrypt(string value) {
      if (string.IsNullOrEmpty(value)) {
        return null;
      }

      var inputArray = Encoding.UTF8.GetBytes(value);
      var tripleDes = new TripleDESCryptoServiceProvider();
      tripleDes.Key = Encoding.UTF8.GetBytes(encryptionKey);
      tripleDes.Mode = CipherMode.ECB;
      tripleDes.Padding = PaddingMode.PKCS7;
      var cTransform = tripleDes.CreateEncryptor();
      var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
      tripleDes.Clear();
      return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private static string Decrypt(string encryptedPassword) {
      if (string.IsNullOrEmpty(encryptedPassword))
        return encryptedPassword;

      var inputArray = Convert.FromBase64String(encryptedPassword);
      var tripleDes = new TripleDESCryptoServiceProvider();
      tripleDes.Key = Encoding.UTF8.GetBytes(encryptionKey);
      tripleDes.Mode = CipherMode.ECB;
      tripleDes.Padding = PaddingMode.PKCS7;
      var cTransform = tripleDes.CreateDecryptor();
      var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
      tripleDes.Clear();
      return Encoding.UTF8.GetString(resultArray);
    }

    private bool GetApplicationStartWithSystem() {
      try {
        using (var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run")) {
          var regValue = registryKey.GetValue("AutoVPNConnect")?.ToString();
          return regValue == Updater.CurrentFileLocation;
        }
      }
      catch {
        return false;
      }
    }

    private void SetApplicationStartWithSystem(bool enabled) {
      try {
        using (var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)) {
          if (enabled)
            registryKey.SetValue("AutoVPNConnect", Application.ExecutablePath);
          else
            registryKey.DeleteValue("AutoVPNConnect", false);
        }
      }
      catch (Exception ex) {
        MessageBox.Show("Error while changing application auto start: " + ex.Message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    public string VpnConnectionName {
      get => vpnConnectionName;
      set {
        if (vpnConnectionName == value) return;
        vpnConnectionName = value;
        WriteSettings();
      }
    }

    public string UserName {
      get => username;
      set {
        if (username == value) return;
        username = value;
        WriteSettings();
      }
    }

    public string Password {
      get => password;
      set {
        if (password == value) return;
        password = value;
        WriteSettings();
      }
    }

    public string Theme {
      get => theme;
      set {
        if (theme == value) return;
        theme = value;
        WriteSettings();
      }
    }

    public bool AutoStartApp {
      get => GetApplicationStartWithSystem();
      set => SetApplicationStartWithSystem(value);
    }

    public bool Reconnect {
      get => reconnect;
      set {
        if (reconnect == value) return;
        reconnect = value;
        WriteSettings();
      }
    }

    public bool RunInBackground {
      get => runInBackground;
      set {
        if (runInBackground == value) return;
        runInBackground = value;
        WriteSettings();
      }
    }

    public bool AlwaysOnTop {
      get => alwaysOnTop;
      set {
        if (alwaysOnTop == value) return;
        alwaysOnTop = value;
        WriteSettings();
      }
    }
  }
}
