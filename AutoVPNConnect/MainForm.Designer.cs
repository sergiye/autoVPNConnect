namespace AutoVPNConnect {
  partial class MainForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.lblConnectionName = new System.Windows.Forms.Label();
      this.lblConnectsTo = new System.Windows.Forms.Label();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.groupBoxGeneralSettings = new System.Windows.Forms.GroupBox();
      this.checkBoxStartApplicationMinimized = new System.Windows.Forms.CheckBox();
      this.lblStartMinimized = new System.Windows.Forms.Label();
      this.checkBoxApplicationEnabled = new System.Windows.Forms.CheckBox();
      this.lblApplicationEnabled = new System.Windows.Forms.Label();
      this.checkBoxStartWithSystem = new System.Windows.Forms.CheckBox();
      this.lblStartWithSystem = new System.Windows.Forms.Label();
      this.groupBoxStatus = new System.Windows.Forms.GroupBox();
      this.btnToggle = new System.Windows.Forms.Button();
      this.lblConnectionStatus = new System.Windows.Forms.Label();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.groupBoxVPNSettings = new System.Windows.Forms.GroupBox();
      this.btnSaveSettings = new System.Windows.Forms.Button();
      this.lblPassword = new System.Windows.Forms.Label();
      this.textBoxPassword = new System.Windows.Forms.TextBox();
      this.lblUsername = new System.Windows.Forms.Label();
      this.textBoxUsername = new System.Windows.Forms.TextBox();
      this.comboBoxActiveVPNConnections = new System.Windows.Forms.ComboBox();
      this.lblSelectVPNConnection = new System.Windows.Forms.Label();
      this.tabControl.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.groupBoxGeneralSettings.SuspendLayout();
      this.groupBoxStatus.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.groupBoxVPNSettings.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblConnectionName
      // 
      this.lblConnectionName.AutoSize = true;
      this.lblConnectionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblConnectionName.Location = new System.Drawing.Point(164, 24);
      this.lblConnectionName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblConnectionName.Name = "lblConnectionName";
      this.lblConnectionName.Size = new System.Drawing.Size(151, 20);
      this.lblConnectionName.TabIndex = 2;
      this.lblConnectionName.Text = "No settings found";
      // 
      // lblConnectsTo
      // 
      this.lblConnectsTo.AutoSize = true;
      this.lblConnectsTo.Location = new System.Drawing.Point(8, 24);
      this.lblConnectsTo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblConnectsTo.Name = "lblConnectsTo";
      this.lblConnectsTo.Size = new System.Drawing.Size(126, 20);
      this.lblConnectsTo.TabIndex = 1;
      this.lblConnectsTo.Text = "Auto connect to:";
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.tabPage1);
      this.tabControl.Controls.Add(this.tabPage2);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(444, 279);
      this.tabControl.TabIndex = 2;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.groupBoxGeneralSettings);
      this.tabPage1.Controls.Add(this.groupBoxStatus);
      this.tabPage1.Location = new System.Drawing.Point(4, 29);
      this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage1.Size = new System.Drawing.Size(436, 246);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "General";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // groupBoxGeneralSettings
      // 
      this.groupBoxGeneralSettings.Controls.Add(this.checkBoxStartApplicationMinimized);
      this.groupBoxGeneralSettings.Controls.Add(this.lblStartMinimized);
      this.groupBoxGeneralSettings.Controls.Add(this.checkBoxApplicationEnabled);
      this.groupBoxGeneralSettings.Controls.Add(this.lblApplicationEnabled);
      this.groupBoxGeneralSettings.Controls.Add(this.checkBoxStartWithSystem);
      this.groupBoxGeneralSettings.Controls.Add(this.lblStartWithSystem);
      this.groupBoxGeneralSettings.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBoxGeneralSettings.Location = new System.Drawing.Point(4, 109);
      this.groupBoxGeneralSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBoxGeneralSettings.Name = "groupBoxGeneralSettings";
      this.groupBoxGeneralSettings.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBoxGeneralSettings.Size = new System.Drawing.Size(428, 132);
      this.groupBoxGeneralSettings.TabIndex = 1;
      this.groupBoxGeneralSettings.TabStop = false;
      this.groupBoxGeneralSettings.Text = "General settings";
      // 
      // checkBoxStartApplicationMinimized
      // 
      this.checkBoxStartApplicationMinimized.AutoSize = true;
      this.checkBoxStartApplicationMinimized.Location = new System.Drawing.Point(390, 96);
      this.checkBoxStartApplicationMinimized.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.checkBoxStartApplicationMinimized.Name = "checkBoxStartApplicationMinimized";
      this.checkBoxStartApplicationMinimized.Size = new System.Drawing.Size(22, 21);
      this.checkBoxStartApplicationMinimized.TabIndex = 5;
      this.checkBoxStartApplicationMinimized.UseVisualStyleBackColor = true;
      this.checkBoxStartApplicationMinimized.CheckedChanged += new System.EventHandler(this.checkBoxStartApplicationMinimized_CheckedChanged);
      // 
      // lblStartMinimized
      // 
      this.lblStartMinimized.AutoSize = true;
      this.lblStartMinimized.Location = new System.Drawing.Point(14, 96);
      this.lblStartMinimized.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblStartMinimized.Name = "lblStartMinimized";
      this.lblStartMinimized.Size = new System.Drawing.Size(118, 20);
      this.lblStartMinimized.TabIndex = 4;
      this.lblStartMinimized.Text = "Start minimized";
      // 
      // checkBoxApplicationEnabled
      // 
      this.checkBoxApplicationEnabled.AutoSize = true;
      this.checkBoxApplicationEnabled.Location = new System.Drawing.Point(390, 63);
      this.checkBoxApplicationEnabled.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.checkBoxApplicationEnabled.Name = "checkBoxApplicationEnabled";
      this.checkBoxApplicationEnabled.Size = new System.Drawing.Size(22, 21);
      this.checkBoxApplicationEnabled.TabIndex = 3;
      this.checkBoxApplicationEnabled.UseVisualStyleBackColor = true;
      this.checkBoxApplicationEnabled.CheckedChanged += new System.EventHandler(this.checkBoxApplicationEnabled_CheckedChanged);
      // 
      // lblApplicationEnabled
      // 
      this.lblApplicationEnabled.AutoSize = true;
      this.lblApplicationEnabled.Location = new System.Drawing.Point(14, 65);
      this.lblApplicationEnabled.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblApplicationEnabled.Name = "lblApplicationEnabled";
      this.lblApplicationEnabled.Size = new System.Drawing.Size(148, 20);
      this.lblApplicationEnabled.TabIndex = 2;
      this.lblApplicationEnabled.Text = "Restore connection";
      // 
      // checkBoxStartWithSystem
      // 
      this.checkBoxStartWithSystem.AutoSize = true;
      this.checkBoxStartWithSystem.Location = new System.Drawing.Point(390, 29);
      this.checkBoxStartWithSystem.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.checkBoxStartWithSystem.Name = "checkBoxStartWithSystem";
      this.checkBoxStartWithSystem.Size = new System.Drawing.Size(22, 21);
      this.checkBoxStartWithSystem.TabIndex = 1;
      this.checkBoxStartWithSystem.UseVisualStyleBackColor = true;
      this.checkBoxStartWithSystem.CheckedChanged += new System.EventHandler(this.checkBoxStartWithSystem_CheckedChanged);
      // 
      // lblStartWithSystem
      // 
      this.lblStartWithSystem.AutoSize = true;
      this.lblStartWithSystem.Location = new System.Drawing.Point(14, 31);
      this.lblStartWithSystem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblStartWithSystem.Name = "lblStartWithSystem";
      this.lblStartWithSystem.Size = new System.Drawing.Size(210, 20);
      this.lblStartWithSystem.TabIndex = 0;
      this.lblStartWithSystem.Text = "Start application with system";
      // 
      // groupBoxStatus
      // 
      this.groupBoxStatus.Controls.Add(this.btnToggle);
      this.groupBoxStatus.Controls.Add(this.lblConnectionName);
      this.groupBoxStatus.Controls.Add(this.lblConnectsTo);
      this.groupBoxStatus.Controls.Add(this.lblConnectionStatus);
      this.groupBoxStatus.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBoxStatus.Location = new System.Drawing.Point(4, 5);
      this.groupBoxStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBoxStatus.Name = "groupBoxStatus";
      this.groupBoxStatus.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBoxStatus.Size = new System.Drawing.Size(428, 104);
      this.groupBoxStatus.TabIndex = 0;
      this.groupBoxStatus.TabStop = false;
      this.groupBoxStatus.Text = "Status";
      // 
      // btnToggle
      // 
      this.btnToggle.Location = new System.Drawing.Point(316, 52);
      this.btnToggle.Name = "btnToggle";
      this.btnToggle.Size = new System.Drawing.Size(105, 41);
      this.btnToggle.TabIndex = 4;
      this.btnToggle.Text = "Toggle";
      this.btnToggle.UseVisualStyleBackColor = true;
      // 
      // lblConnectionStatus
      // 
      this.lblConnectionStatus.AutoSize = true;
      this.lblConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblConnectionStatus.Location = new System.Drawing.Point(8, 62);
      this.lblConnectionStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblConnectionStatus.Name = "lblConnectionStatus";
      this.lblConnectionStatus.Size = new System.Drawing.Size(275, 20);
      this.lblConnectionStatus.TabIndex = 3;
      this.lblConnectionStatus.Text = "Connection status: Disconnected";
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.groupBoxVPNSettings);
      this.tabPage2.Location = new System.Drawing.Point(4, 29);
      this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage2.Size = new System.Drawing.Size(436, 246);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Setup";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // groupBoxVPNSettings
      // 
      this.groupBoxVPNSettings.Controls.Add(this.btnSaveSettings);
      this.groupBoxVPNSettings.Controls.Add(this.lblPassword);
      this.groupBoxVPNSettings.Controls.Add(this.textBoxPassword);
      this.groupBoxVPNSettings.Controls.Add(this.lblUsername);
      this.groupBoxVPNSettings.Controls.Add(this.textBoxUsername);
      this.groupBoxVPNSettings.Controls.Add(this.comboBoxActiveVPNConnections);
      this.groupBoxVPNSettings.Controls.Add(this.lblSelectVPNConnection);
      this.groupBoxVPNSettings.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBoxVPNSettings.Location = new System.Drawing.Point(4, 5);
      this.groupBoxVPNSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBoxVPNSettings.Name = "groupBoxVPNSettings";
      this.groupBoxVPNSettings.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBoxVPNSettings.Size = new System.Drawing.Size(428, 202);
      this.groupBoxVPNSettings.TabIndex = 0;
      this.groupBoxVPNSettings.TabStop = false;
      this.groupBoxVPNSettings.Text = "VPN Settings";
      // 
      // btnSaveSettings
      // 
      this.btnSaveSettings.Location = new System.Drawing.Point(14, 158);
      this.btnSaveSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnSaveSettings.Name = "btnSaveSettings";
      this.btnSaveSettings.Size = new System.Drawing.Size(112, 35);
      this.btnSaveSettings.TabIndex = 4;
      this.btnSaveSettings.Text = "Apply";
      this.btnSaveSettings.UseVisualStyleBackColor = true;
      this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
      // 
      // lblPassword
      // 
      this.lblPassword.AutoSize = true;
      this.lblPassword.Location = new System.Drawing.Point(218, 94);
      this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblPassword.Name = "lblPassword";
      this.lblPassword.Size = new System.Drawing.Size(78, 20);
      this.lblPassword.TabIndex = 4;
      this.lblPassword.Text = "Password";
      // 
      // textBoxPassword
      // 
      this.textBoxPassword.Location = new System.Drawing.Point(222, 118);
      this.textBoxPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.textBoxPassword.Name = "textBoxPassword";
      this.textBoxPassword.PasswordChar = '*';
      this.textBoxPassword.Size = new System.Drawing.Size(190, 26);
      this.textBoxPassword.TabIndex = 3;
      // 
      // lblUsername
      // 
      this.lblUsername.AutoSize = true;
      this.lblUsername.Location = new System.Drawing.Point(9, 94);
      this.lblUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblUsername.Name = "lblUsername";
      this.lblUsername.Size = new System.Drawing.Size(83, 20);
      this.lblUsername.TabIndex = 2;
      this.lblUsername.Text = "Username";
      // 
      // textBoxUsername
      // 
      this.textBoxUsername.Location = new System.Drawing.Point(15, 118);
      this.textBoxUsername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.textBoxUsername.Name = "textBoxUsername";
      this.textBoxUsername.Size = new System.Drawing.Size(196, 26);
      this.textBoxUsername.TabIndex = 2;
      // 
      // comboBoxActiveVPNConnections
      // 
      this.comboBoxActiveVPNConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxActiveVPNConnections.FormattingEnabled = true;
      this.comboBoxActiveVPNConnections.Location = new System.Drawing.Point(15, 57);
      this.comboBoxActiveVPNConnections.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.comboBoxActiveVPNConnections.Name = "comboBoxActiveVPNConnections";
      this.comboBoxActiveVPNConnections.Size = new System.Drawing.Size(397, 28);
      this.comboBoxActiveVPNConnections.TabIndex = 1;
      this.comboBoxActiveVPNConnections.DropDown += new System.EventHandler(this.comboBoxActiveVPNConnections_DropDown);
      // 
      // lblSelectVPNConnection
      // 
      this.lblSelectVPNConnection.AutoSize = true;
      this.lblSelectVPNConnection.Location = new System.Drawing.Point(10, 31);
      this.lblSelectVPNConnection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblSelectVPNConnection.Name = "lblSelectVPNConnection";
      this.lblSelectVPNConnection.Size = new System.Drawing.Size(123, 20);
      this.lblSelectVPNConnection.TabIndex = 0;
      this.lblSelectVPNConnection.Text = "VPN connection";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(444, 279);
      this.Controls.Add(this.tabControl);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Auto VPN Connect";
      this.TransparencyKey = System.Drawing.SystemColors.ActiveBorder;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoVPNConnect_FormClosing);
      this.tabControl.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.groupBoxGeneralSettings.ResumeLayout(false);
      this.groupBoxGeneralSettings.PerformLayout();
      this.groupBoxStatus.ResumeLayout(false);
      this.groupBoxStatus.PerformLayout();
      this.tabPage2.ResumeLayout(false);
      this.groupBoxVPNSettings.ResumeLayout(false);
      this.groupBoxVPNSettings.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.Label lblConnectionName;
    private System.Windows.Forms.Label lblConnectsTo;
    private System.Windows.Forms.GroupBox groupBoxStatus;
    private System.Windows.Forms.Label lblConnectionStatus;
    private System.Windows.Forms.GroupBox groupBoxVPNSettings;
    private System.Windows.Forms.ComboBox comboBoxActiveVPNConnections;
    private System.Windows.Forms.Label lblSelectVPNConnection;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.TextBox textBoxPassword;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TextBox textBoxUsername;
    private System.Windows.Forms.GroupBox groupBoxGeneralSettings;
    private System.Windows.Forms.CheckBox checkBoxStartWithSystem;
    private System.Windows.Forms.Label lblStartWithSystem;
    private System.Windows.Forms.CheckBox checkBoxApplicationEnabled;
    private System.Windows.Forms.Label lblApplicationEnabled;
    private System.Windows.Forms.Button btnSaveSettings;
    private System.Windows.Forms.CheckBox checkBoxStartApplicationMinimized;
    private System.Windows.Forms.Label lblStartMinimized;
    private System.Windows.Forms.Button btnToggle;
  }
}

