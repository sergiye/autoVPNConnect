using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AutoVPNConnect {
  class SettingsManager {
    private const string REGISTRY_PATH = @"Software\sergiye\AutoVPNConnect";
    private string VPNConnectionName = "No settings found";
    private string Username = "";
    private string Password = "";

    private bool StartApplicationWithSystem = true;
    private bool ShowMessages = true;
    private bool ApplicationEnabled = true;
    private bool StartApplicationMinimized = false;

    private readonly string EncryptionKey = "123456789012345678901234";

    public SettingsManager() {
      if (validSettingsFound()) {
        readSettings();
      }
    }

    public bool settingsRegistryFound() {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH);
      if (key != null) {
        return true;
      }
      else {
        return false;
      }
    }

    public bool validSettingsFound() {
      if (settingsRegistryFound() == false) {
        return false;
      }

      var key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH);

      if (key == null) {
        return false;
      }

      string ConnectionName;
      string UserName;
      string Password;
      try {
        ConnectionName = key.GetValue("VPNConnectionName").ToString();
        UserName = key.GetValue("Username").ToString();
        Password = key.GetValue("Password").ToString();
        key.Close();
      }
      catch {
        key.Close();
        return false;
      }

      return (ConnectionName != "No settings found");
    }

    private void readSettings() {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH);
      if (key == null) {
        return;
      }
      try {
        VPNConnectionName = key.GetValue("VPNConnectionName").ToString();
        Username = key.GetValue("Username").ToString();
        string EncryptedPassword = key.GetValue("Password").ToString();
        Password = decryptPassword(EncryptedPassword);

        string applicationStartWithSystem = key.GetValue("StartApplicationWithSystem").ToString();
        StartApplicationWithSystem = applicationStartWithSystem == "True";

        string showMessages = key.GetValue("ShowMessages").ToString();
        ShowMessages = showMessages == "True";

        string applicationEnabled = key.GetValue("ApplicationEnabled").ToString();
        ApplicationEnabled = applicationEnabled == "True";

        string startApplicationMinimized = key.GetValue("StartApplicationMinimized").ToString();
        StartApplicationMinimized = startApplicationMinimized == "True";

        key.Close();
      }
      catch {
        key.Close();
      }
    }

    private void writeSettings() {
      if (settingsRegistryFound()) {
        Registry.CurrentUser.DeleteSubKey(REGISTRY_PATH);
      }
      Registry.CurrentUser.CreateSubKey(REGISTRY_PATH);

      RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true);
      if (key == null) {
        MessageBox.Show("Error while writing settings", "Error");
        return;
      }

      try {
        string EncryptedPassword = encryptPassword(Password);
        key.SetValue("VPNConnectionName", VPNConnectionName, RegistryValueKind.String);
        key.SetValue("Username", Username, RegistryValueKind.String);
        key.SetValue("Password", EncryptedPassword, RegistryValueKind.String);
        key.SetValue("StartApplicationWithSystem", StartApplicationWithSystem, RegistryValueKind.String);
        key.SetValue("ShowMessages", ShowMessages, RegistryValueKind.String);
        key.SetValue("ApplicationEnabled", ApplicationEnabled, RegistryValueKind.String);
        key.SetValue("StartApplicationMinimized", StartApplicationMinimized, RegistryValueKind.String);
      }
      catch {
        MessageBox.Show("Error while writing settings");
      }

      key.Close();
    }

    private string encryptPassword(string password) {
      if (password == "") {
        return "";
      }

      byte[] inputArray = Encoding.UTF8.GetBytes(password);
      TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
      tripleDES.Key = Encoding.UTF8.GetBytes(EncryptionKey);
      tripleDES.Mode = CipherMode.ECB;
      tripleDES.Padding = PaddingMode.PKCS7;
      ICryptoTransform cTransform = tripleDES.CreateEncryptor();
      byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
      tripleDES.Clear();
      return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private string decryptPassword(string encryptedPassword) {
      if (encryptedPassword == "") {
        return "";
      }

      byte[] inputArray = Convert.FromBase64String(encryptedPassword);
      TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
      tripleDES.Key = Encoding.UTF8.GetBytes(EncryptionKey);
      tripleDES.Mode = CipherMode.ECB;
      tripleDES.Padding = PaddingMode.PKCS7;
      ICryptoTransform cTransform = tripleDES.CreateDecryptor();
      byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
      tripleDES.Clear();
      return Encoding.UTF8.GetString(resultArray);
    }

    public void setConnectionName(string connectionName) {
      VPNConnectionName = connectionName;
      writeSettings();
    }

    public string getConnectionName() {
      return VPNConnectionName;
    }

    public void setUserName(string userName) {
      Username = userName;
      writeSettings();
    }

    public string getUserName() {
      return Username;
    }

    public void setPassword(string password) {
      Password = password;
      writeSettings();
    }

    public string getPassword() {
      return Password;
    }

    public void setApplicationStartWithSystem(bool enabled) {
      RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
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
        MessageBox.Show("Error while writing settings", "Error");
      }

      StartApplicationWithSystem = enabled;
      writeSettings();
    }

    public bool getApplicationStartWithSystem() {
      return StartApplicationWithSystem;
    }

    public void setShowMessages(bool enabled) {
      ShowMessages = enabled;
      writeSettings();
    }

    public bool getShowMessagesSetting() {
      return ShowMessages;
    }

    public void setApplicationEnabledSetting(bool enabled) {
      ApplicationEnabled = enabled;
      writeSettings();
    }

    public bool getApplicationEnabledSetting() {
      return ApplicationEnabled;
    }

    public void setStartApplicationMinimized(bool enabled) {
      StartApplicationMinimized = enabled;
      writeSettings();
    }

    public bool getStartApplicationMinimized() {
      return StartApplicationMinimized;
    }
  }
}
