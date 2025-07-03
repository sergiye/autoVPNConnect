using sergiye.Common;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AutoVPNConnect {
  class SettingsManager {
    private readonly PersistentSettings settings;
    private static readonly string encryptionKey = "123456789012345678901234";

    public SettingsManager(PersistentSettings settings) {
      this.settings = settings;
    }

    internal bool IsConnectionConfigured => !string.IsNullOrEmpty(VpnConnectionName) && VpnConnectionName != "No settings found";

    private static string Encrypt(string value) {
      if (string.IsNullOrEmpty(value))
        return null;

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
        using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run")) {
          if (key != null) {
            var regValue = key.GetValue(Updater.ApplicationName)?.ToString();
            return Updater.CurrentFileLocation.Equals(regValue, StringComparison.OrdinalIgnoreCase);
          }
        }
      }
      catch {
        //ignore
      }
      return false;
    }

    private void SetApplicationStartWithSystem(bool enabled) {
      try {
        using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)) {
          if (key != null) {
            if (enabled)
              key.SetValue(Updater.ApplicationName, Application.ExecutablePath);
            else
              key.DeleteValue(Updater.ApplicationName, false);
          }
        }
      }
      catch (Exception ex) {
        MessageBox.Show("Error while changing application auto start: " + ex.Message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    public string VpnConnectionName {
      get => settings.GetValue("VPNConnectionName", "No settings found");
      set => settings.SetValue("VPNConnectionName", value);
    }

    public string UserName {
      get => settings.GetValue("Username", "");
      set => settings.SetValue("Username", value ?? "");
    }

    public string Password {
      get => Decrypt(settings.GetValue("Password", ""));
      set => settings.SetValue("Password", Encrypt(value) ?? "");
    }

    public bool AutoStartApp {
      get => GetApplicationStartWithSystem();
      set => SetApplicationStartWithSystem(value);
    }

    public bool Reconnect {
      get => settings.GetValue("ApplicationEnabled", false);
      set => settings.SetValue("ApplicationEnabled", value);
    }
  }
}
