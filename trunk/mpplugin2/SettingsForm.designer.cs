namespace NrkBrowser
{
  partial class SettingsForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.saveButton = new System.Windows.Forms.Button();
        this.speedLabel = new System.Windows.Forms.Label();
        this.speedUpDown = new System.Windows.Forms.NumericUpDown();
        this.unitLabel = new System.Windows.Forms.Label();
        this.cancelButton = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.liveStreamQualityCombo = new System.Windows.Forms.ComboBox();
        this.header = new System.Windows.Forms.Label();
        this.nameLabel = new System.Windows.Forms.Label();
        this.nameTextbox = new System.Windows.Forms.TextBox();
        this.labelVersion = new System.Windows.Forms.Label();
        this.labelVersionVerdi = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.speedUpDown)).BeginInit();
        this.SuspendLayout();
        // 
        // saveButton
        // 
        this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.saveButton.Location = new System.Drawing.Point(384, 185);
        this.saveButton.Margin = new System.Windows.Forms.Padding(2);
        this.saveButton.Name = "saveButton";
        this.saveButton.Size = new System.Drawing.Size(56, 19);
        this.saveButton.TabIndex = 0;
        this.saveButton.Text = "Save";
        this.saveButton.UseVisualStyleBackColor = true;
        // 
        // speedLabel
        // 
        this.speedLabel.AutoSize = true;
        this.speedLabel.Location = new System.Drawing.Point(11, 62);
        this.speedLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        this.speedLabel.Name = "speedLabel";
        this.speedLabel.Size = new System.Drawing.Size(93, 13);
        this.speedLabel.TabIndex = 1;
        this.speedLabel.Text = "Connection speed";
        // 
        // speedUpDown
        // 
        this.speedUpDown.Location = new System.Drawing.Point(120, 62);
        this.speedUpDown.Margin = new System.Windows.Forms.Padding(2);
        this.speedUpDown.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
        this.speedUpDown.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            0});
        this.speedUpDown.Name = "speedUpDown";
        this.speedUpDown.Size = new System.Drawing.Size(54, 20);
        this.speedUpDown.TabIndex = 2;
        this.speedUpDown.Value = new decimal(new int[] {
            2048,
            0,
            0,
            0});
        // 
        // unitLabel
        // 
        this.unitLabel.AutoSize = true;
        this.unitLabel.Location = new System.Drawing.Point(178, 64);
        this.unitLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        this.unitLabel.Name = "unitLabel";
        this.unitLabel.Size = new System.Drawing.Size(29, 13);
        this.unitLabel.TabIndex = 3;
        this.unitLabel.Text = "kb/s";
        // 
        // cancelButton
        // 
        this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.cancelButton.Location = new System.Drawing.Point(323, 185);
        this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
        this.cancelButton.Name = "cancelButton";
        this.cancelButton.Size = new System.Drawing.Size(56, 19);
        this.cancelButton.TabIndex = 4;
        this.cancelButton.Text = "Cancel";
        this.cancelButton.UseVisualStyleBackColor = true;
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(10, 91);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(94, 13);
        this.label1.TabIndex = 7;
        this.label1.Text = "Live stream quality";
        // 
        // liveStreamQualityCombo
        // 
        this.liveStreamQualityCombo.FormattingEnabled = true;
        this.liveStreamQualityCombo.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
        this.liveStreamQualityCombo.Location = new System.Drawing.Point(120, 88);
        this.liveStreamQualityCombo.Name = "liveStreamQualityCombo";
        this.liveStreamQualityCombo.Size = new System.Drawing.Size(121, 21);
        this.liveStreamQualityCombo.TabIndex = 8;
        // 
        // header
        // 
        this.header.AutoSize = true;
        this.header.Location = new System.Drawing.Point(17, 13);
        this.header.Name = "header";
        this.header.Size = new System.Drawing.Size(71, 13);
        this.header.TabIndex = 9;
        this.header.Text = "NRK Browser";
        // 
        // nameLabel
        // 
        this.nameLabel.AutoSize = true;
        this.nameLabel.Location = new System.Drawing.Point(10, 116);
        this.nameLabel.Name = "nameLabel";
        this.nameLabel.Size = new System.Drawing.Size(65, 13);
        this.nameLabel.TabIndex = 10;
        this.nameLabel.Text = "Plugin name";
        // 
        // nameTextbox
        // 
        this.nameTextbox.Location = new System.Drawing.Point(120, 116);
        this.nameTextbox.Name = "nameTextbox";
        this.nameTextbox.Size = new System.Drawing.Size(121, 20);
        this.nameTextbox.TabIndex = 11;
        // 
        // labelVersion
        // 
        this.labelVersion.AutoSize = true;
        this.labelVersion.Location = new System.Drawing.Point(136, 13);
        this.labelVersion.Name = "labelVersion";
        this.labelVersion.Size = new System.Drawing.Size(45, 13);
        this.labelVersion.TabIndex = 12;
        this.labelVersion.Text = "Version:";
        // 
        // labelVersionVerdi
        // 
        this.labelVersionVerdi.AutoSize = true;
        this.labelVersionVerdi.Location = new System.Drawing.Point(187, 13);
        this.labelVersionVerdi.Name = "labelVersionVerdi";
        this.labelVersionVerdi.Size = new System.Drawing.Size(78, 13);
        this.labelVersionVerdi.TabIndex = 13;
        this.labelVersionVerdi.Text = "versjonnummer";
        // 
        // SettingsForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(451, 214);
        this.Controls.Add(this.labelVersionVerdi);
        this.Controls.Add(this.labelVersion);
        this.Controls.Add(this.nameTextbox);
        this.Controls.Add(this.nameLabel);
        this.Controls.Add(this.header);
        this.Controls.Add(this.liveStreamQualityCombo);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.cancelButton);
        this.Controls.Add(this.saveButton);
        this.Controls.Add(this.speedLabel);
        this.Controls.Add(this.unitLabel);
        this.Controls.Add(this.speedUpDown);
        this.Margin = new System.Windows.Forms.Padding(2);
        this.Name = "SettingsForm";
        this.Text = "NRK Browser settings";
        ((System.ComponentModel.ISupportInitialize)(this.speedUpDown)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.Label speedLabel;
    private System.Windows.Forms.Label unitLabel;
    public System.Windows.Forms.NumericUpDown speedUpDown;
      private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Label label1;
    public System.Windows.Forms.ComboBox liveStreamQualityCombo;
      private System.Windows.Forms.Label header;
      private System.Windows.Forms.Label nameLabel;
      private System.Windows.Forms.TextBox nameTextbox;
      private System.Windows.Forms.Label labelVersion;
      private System.Windows.Forms.Label labelVersionVerdi;
  }
}