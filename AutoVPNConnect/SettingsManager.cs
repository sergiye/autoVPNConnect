using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AutoVPNConnect {
  class SettingsManager {
    private const string RegistryPath = @"Software\sergiye\AutoVPNConnect";
    private string vpnConnectionName = "No settings found";
    private string username = "";
    private string password = "";

    private bool startApplicationWithSystem = true;
    private bool showMessages = true;
    private bool applicationEnabled = true;
    private bool startApplicationMinimized;

    private readonly string encryptionKey = "123456789012345678901234";

    public SettingsManager() {
      if (ValidSettingsFound()) {
        ReadSettings();
      }
    }

    private static bool SettingsRegistryFound() {
      var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
      return key != null;
    }

    internal static bool ValidSettingsFound() {
      if (SettingsRegistryFound() == false) {
        return false;
      }

      var key = Registry.CurrentUser.OpenSubKey(RegistryPath);

      if (key == null) {
        return false;
      }

      string connectionName;
      // string userName;
      // string password;
      try {
        connectionName = key.GetValue("VPNConnectionName").ToString();
        // userName = key.GetValue("Username").ToString();
        // password = key.GetValue("Password").ToString();
        key.Close();
      }
      catch {
        key.Close();
        return false;
      }

      return connectionName != "No settings found";
    }

    private void ReadSettings() {
      var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
      if (key == null) {
        return;
      }
      try {
        vpnConnectionName = key.GetValue("VPNConnectionName").ToString();
        username = key.GetValue("Username").ToString();
        var encryptedPassword = key.GetValue("Password").ToString();
        password = DecryptPassword(encryptedPassword);

        var applicationStartWithSystem = key.GetValue("StartApplicationWithSystem").ToString();
        startApplicationWithSystem = applicationStartWithSystem == "True";
        showMessages = key.GetValue("ShowMessages").ToString() == "True";
        applicationEnabled = key.GetValue("ApplicationEnabled").ToString() == "True";
        startApplicationMinimized = key.GetValue("StartApplicationMinimized").ToString() == "True";

        key.Close();
      }
      catch {
        key.Close();
      }
    }

    private void WriteSettings() {
      if (SettingsRegistryFound()) {
        Registry.CurrentUser.DeleteSubKey(RegistryPath);
      }
      Registry.CurrentUser.CreateSubKey(RegistryPath);

      var key = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
      if (key == null) {
        MessageBox.Show("Error while writing settings", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      try {
        var encryptedPassword = EncryptPassword();
        key.SetValue("VPNConnectionName", vpnConnectionName, RegistryValueKind.String);
        key.SetValue("Username", username, RegistryValueKind.String);
        key.SetValue("Password", encryptedPassword, RegistryValueKind.String);
        key.SetValue("StartApplicationWithSystem", startApplicationWithSystem, RegistryValueKind.String);
        key.SetValue("ShowMessages", showMessages, RegistryValueKind.String);
        key.SetValue("ApplicationEnabled", applicationEnabled, RegistryValueKind.String);
        key.SetValue("StartApplicationMinimized", startApplicationMinimized, RegistryValueKind.String);
      }
      catch {
        MessageBox.Show("Error while writing settings", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      key.Close();
    }

    private string EncryptPassword() {
      if (string.IsNullOrEmpty(password)) {
        return "";
      }

      var inputArray = Encoding.UTF8.GetBytes(password);
      var tripleDes = new TripleDESCryptoServiceProvider();
      tripleDes.Key = Encoding.UTF8.GetBytes(encryptionKey);
      tripleDes.Mode = CipherMode.ECB;
      tripleDes.Padding = PaddingMode.PKCS7;
      var cTransform = tripleDes.CreateEncryptor();
      var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
      tripleDes.Clear();
      return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private string DecryptPassword(string encryptedPassword) {
      if (encryptedPassword == "") {
        return "";
      }

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

    public void SetConnectionName(string connectionName) {
      vpnConnectionName = connectionName;
      WriteSettings();
    }

    public string GetConnectionName() {
      return vpnConnectionName;
    }

    public void SetUserName(string userName) {
      username = userName;
      WriteSettings();
    }

    public string GetUserName() {
      return username;
    }

    public void SetPassword(string value) {
      password = value;
      WriteSettings();
    }

    public string GetPassword() {
      return password;
    }

    public void SetApplicationStartWithSystem(bool enabled) {
      var registryKey = Registry.CurrentUser.OpenSubKey
      ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

      try {
        if (enabled) {
          registryKey.SetValue("AutoVPNConnect", Application.ExecutablePath);
        }
        else {
          registryKey.DeleteValue("AutoVPNConnect");
        }
      }
      catch {
        MessageBox.Show("Error while writing settings", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      startApplicationWithSystem = enabled;
      WriteSettings();
    }

    public bool GetApplicationStartWithSystem() {
      return startApplicationWithSystem;
    }

    public void SetShowMessages(bool enabled) {
      showMessages = enabled;
      WriteSettings();
    }

    public bool GetShowMessagesSetting() {
      return showMessages;
    }

    public void SetApplicationEnabledSetting(bool enabled) {
      applicationEnabled = enabled;
      WriteSettings();
    }

    public bool GetApplicationEnabledSetting() {
      return applicationEnabled;
    }

    public void SetStartApplicationMinimized(bool enabled) {
      startApplicationMinimized = enabled;
      WriteSettings();
    }

    public bool GetStartApplicationMinimized() {
      return startApplicationMinimized;
    }
  }
}
