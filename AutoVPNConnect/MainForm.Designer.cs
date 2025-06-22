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
      this.lblConnectsTo = new System.Windows.Forms.Label();
      this.cbxRunInBackground = new System.Windows.Forms.CheckBox();
      this.cbxReconnect = new System.Windows.Forms.CheckBox();
      this.cbxAutoStart = new System.Windows.Forms.CheckBox();
      this.btnSaveSettings = new System.Windows.Forms.Button();
      this.btnToggle = new System.Windows.Forms.Button();
      this.lblPassword = new System.Windows.Forms.Label();
      this.textBoxPassword = new System.Windows.Forms.TextBox();
      this.lblConnectionStatus = new System.Windows.Forms.Label();
      this.lblUsername = new System.Windows.Forms.Label();
      this.textBoxUsername = new System.Windows.Forms.TextBox();
      this.cmbConnections = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // lblConnectsTo
      // 
      this.lblConnectsTo.AutoSize = true;
      this.lblConnectsTo.Location = new System.Drawing.Point(11, 58);
      this.lblConnectsTo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblConnectsTo.Name = "lblConnectsTo";
      this.lblConnectsTo.Size = new System.Drawing.Size(61, 13);
      this.lblConnectsTo.TabIndex = 2;
      this.lblConnectsTo.Text = "Connection";
      // 
      // cbxRunInBackground
      // 
      this.cbxRunInBackground.AutoSize = true;
      this.cbxRunInBackground.Location = new System.Drawing.Point(14, 184);
      this.cbxRunInBackground.Margin = new System.Windows.Forms.Padding(2);
      this.cbxRunInBackground.Name = "cbxRunInBackground";
      this.cbxRunInBackground.Size = new System.Drawing.Size(117, 17);
      this.cbxRunInBackground.TabIndex = 11;
      this.cbxRunInBackground.Text = "Run in background";
      this.cbxRunInBackground.UseVisualStyleBackColor = true;
      this.cbxRunInBackground.CheckedChanged += new System.EventHandler(this.cbxRunInBackground_CheckedChanged);
      // 
      // cbxReconnect
      // 
      this.cbxReconnect.AutoSize = true;
      this.cbxReconnect.Location = new System.Drawing.Point(14, 163);
      this.cbxReconnect.Margin = new System.Windows.Forms.Padding(2);
      this.cbxReconnect.Name = "cbxReconnect";
      this.cbxReconnect.Size = new System.Drawing.Size(119, 17);
      this.cbxReconnect.TabIndex = 10;
      this.cbxReconnect.Text = "Restore connection";
      this.cbxReconnect.UseVisualStyleBackColor = true;
      this.cbxReconnect.CheckedChanged += new System.EventHandler(this.cbxReconnect_CheckedChanged);
      // 
      // cbxAutoStart
      // 
      this.cbxAutoStart.AutoSize = true;
      this.cbxAutoStart.Location = new System.Drawing.Point(14, 142);
      this.cbxAutoStart.Margin = new System.Windows.Forms.Padding(2);
      this.cbxAutoStart.Name = "cbxAutoStart";
      this.cbxAutoStart.Size = new System.Drawing.Size(159, 17);
      this.cbxAutoStart.TabIndex = 9;
      this.cbxAutoStart.Text = "Start application with system";
      this.cbxAutoStart.UseVisualStyleBackColor = true;
      this.cbxAutoStart.CheckedChanged += new System.EventHandler(this.cbxAutoStart_CheckedChanged);
      // 
      // btnSaveSettings
      // 
      this.btnSaveSettings.Location = new System.Drawing.Point(227, 80);
      this.btnSaveSettings.Margin = new System.Windows.Forms.Padding(2);
      this.btnSaveSettings.Name = "btnSaveSettings";
      this.btnSaveSettings.Size = new System.Drawing.Size(70, 41);
      this.btnSaveSettings.TabIndex = 8;
      this.btnSaveSettings.Text = "Save";
      this.btnSaveSettings.UseVisualStyleBackColor = true;
      this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
      // 
      // btnToggle
      // 
      this.btnToggle.Location = new System.Drawing.Point(210, 11);
      this.btnToggle.Margin = new System.Windows.Forms.Padding(2);
      this.btnToggle.Name = "btnToggle";
      this.btnToggle.Size = new System.Drawing.Size(87, 30);
      this.btnToggle.TabIndex = 1;
      this.btnToggle.Text = "Toggle";
      this.btnToggle.UseVisualStyleBackColor = true;
      // 
      // lblPassword
      // 
      this.lblPassword.AutoSize = true;
      this.lblPassword.Location = new System.Drawing.Point(11, 107);
      this.lblPassword.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblPassword.Name = "lblPassword";
      this.lblPassword.Size = new System.Drawing.Size(53, 13);
      this.lblPassword.TabIndex = 6;
      this.lblPassword.Text = "Password";
      // 
      // textBoxPassword
      // 
      this.textBoxPassword.Location = new System.Drawing.Point(76, 104);
      this.textBoxPassword.Margin = new System.Windows.Forms.Padding(2);
      this.textBoxPassword.Name = "textBoxPassword";
      this.textBoxPassword.PasswordChar = '*';
      this.textBoxPassword.Size = new System.Drawing.Size(145, 20);
      this.textBoxPassword.TabIndex = 7;
      // 
      // lblConnectionStatus
      // 
      this.lblConnectionStatus.AutoSize = true;
      this.lblConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblConnectionStatus.Location = new System.Drawing.Point(11, 20);
      this.lblConnectionStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblConnectionStatus.Name = "lblConnectionStatus";
      this.lblConnectionStatus.Size = new System.Drawing.Size(195, 13);
      this.lblConnectionStatus.TabIndex = 0;
      this.lblConnectionStatus.Text = "Connection status: Disconnected";
      // 
      // lblUsername
      // 
      this.lblUsername.AutoSize = true;
      this.lblUsername.Location = new System.Drawing.Point(11, 83);
      this.lblUsername.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblUsername.Name = "lblUsername";
      this.lblUsername.Size = new System.Drawing.Size(55, 13);
      this.lblUsername.TabIndex = 4;
      this.lblUsername.Text = "Username";
      // 
      // textBoxUsername
      // 
      this.textBoxUsername.Location = new System.Drawing.Point(76, 80);
      this.textBoxUsername.Margin = new System.Windows.Forms.Padding(2);
      this.textBoxUsername.Name = "textBoxUsername";
      this.textBoxUsername.Size = new System.Drawing.Size(145, 20);
      this.textBoxUsername.TabIndex = 5;
      // 
      // cmbConnections
      // 
      this.cmbConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbConnections.FormattingEnabled = true;
      this.cmbConnections.Location = new System.Drawing.Point(76, 55);
      this.cmbConnections.Margin = new System.Windows.Forms.Padding(2);
      this.cmbConnections.Name = "cmbConnections";
      this.cmbConnections.Size = new System.Drawing.Size(221, 21);
      this.cmbConnections.TabIndex = 3;
      this.cmbConnections.DropDown += new System.EventHandler(this.comboBoxActiveVPNConnections_DropDown);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(308, 212);
      this.Controls.Add(this.cbxRunInBackground);
      this.Controls.Add(this.btnSaveSettings);
      this.Controls.Add(this.lblConnectionStatus);
      this.Controls.Add(this.cbxReconnect);
      this.Controls.Add(this.cmbConnections);
      this.Controls.Add(this.btnToggle);
      this.Controls.Add(this.textBoxUsername);
      this.Controls.Add(this.cbxAutoStart);
      this.Controls.Add(this.lblUsername);
      this.Controls.Add(this.lblPassword);
      this.Controls.Add(this.lblConnectsTo);
      this.Controls.Add(this.textBoxPassword);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(2);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Auto VPN Connect";
      this.TransparencyKey = System.Drawing.SystemColors.ActiveBorder;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoVPNConnect_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label lblConnectsTo;
    private System.Windows.Forms.Label lblConnectionStatus;
    private System.Windows.Forms.ComboBox cmbConnections;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.TextBox textBoxPassword;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TextBox textBoxUsername;
    private System.Windows.Forms.CheckBox cbxAutoStart;
    private System.Windows.Forms.CheckBox cbxReconnect;
    private System.Windows.Forms.Button btnSaveSettings;
    private System.Windows.Forms.CheckBox cbxRunInBackground;
    private System.Windows.Forms.Button btnToggle;
  }
}

