namespace MusicBeePlugin
{
  partial class SettingsWindow
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
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.textBoxDetails = new System.Windows.Forms.TextBox();
      this.textBoxState = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.button2 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.textBoxTrackNo = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.textBoxTrackCnt = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer1.IsSplitterFixed = true;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
      this.splitContainer1.Panel1.Controls.Add(this.label4);
      this.splitContainer1.Panel1.Controls.Add(this.textBoxTrackCnt);
      this.splitContainer1.Panel1.Controls.Add(this.label3);
      this.splitContainer1.Panel1.Controls.Add(this.textBoxTrackNo);
      this.splitContainer1.Panel1.Controls.Add(this.label2);
      this.splitContainer1.Panel1.Controls.Add(this.textBoxDetails);
      this.splitContainer1.Panel1.Controls.Add(this.textBoxState);
      this.splitContainer1.Panel1.Controls.Add(this.label1);
      this.splitContainer1.Panel1.Controls.Add(this.textBox1);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.button2);
      this.splitContainer1.Panel2.Controls.Add(this.button1);
      this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3);
      this.splitContainer1.Size = new System.Drawing.Size(507, 155);
      this.splitContainer1.SplitterDistance = 114;
      this.splitContainer1.TabIndex = 0;
      // 
      // textBoxDetails
      // 
      this.textBoxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxDetails.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxDetails.Location = new System.Drawing.Point(108, 65);
      this.textBoxDetails.Name = "textBoxDetails";
      this.textBoxDetails.Size = new System.Drawing.Size(169, 25);
      this.textBoxDetails.TabIndex = 3;
      // 
      // textBoxState
      // 
      this.textBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxState.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxState.Location = new System.Drawing.Point(108, 34);
      this.textBoxState.Name = "textBoxState";
      this.textBoxState.Size = new System.Drawing.Size(266, 25);
      this.textBoxState.TabIndex = 2;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(246)))));
      this.label1.Location = new System.Drawing.Point(108, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(84, 19);
      this.label1.TabIndex = 1;
      this.label1.Text = "MusicBee";
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(12, 12);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(90, 90);
      this.textBox1.TabIndex = 0;
      // 
      // button2
      // 
      this.button2.Dock = System.Windows.Forms.DockStyle.Left;
      this.button2.Location = new System.Drawing.Point(3, 3);
      this.button2.Margin = new System.Windows.Forms.Padding(5);
      this.button2.Name = "button2";
      this.button2.Padding = new System.Windows.Forms.Padding(5);
      this.button2.Size = new System.Drawing.Size(163, 31);
      this.button2.TabIndex = 1;
      this.button2.Text = "Placeholders";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.Dock = System.Windows.Forms.DockStyle.Right;
      this.button1.Location = new System.Drawing.Point(341, 3);
      this.button1.Margin = new System.Windows.Forms.Padding(5);
      this.button1.Name = "button1";
      this.button1.Padding = new System.Windows.Forms.Padding(5);
      this.button1.Size = new System.Drawing.Size(163, 31);
      this.button1.TabIndex = 0;
      this.button1.Text = "Save";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(246)))));
      this.label2.Location = new System.Drawing.Point(281, 68);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(13, 17);
      this.label2.TabIndex = 4;
      this.label2.Text = "(";
      // 
      // textBoxTrackNo
      // 
      this.textBoxTrackNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxTrackNo.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxTrackNo.Location = new System.Drawing.Point(300, 65);
      this.textBoxTrackNo.Name = "textBoxTrackNo";
      this.textBoxTrackNo.Size = new System.Drawing.Size(74, 25);
      this.textBoxTrackNo.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(246)))));
      this.label3.Location = new System.Drawing.Point(378, 68);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(20, 17);
      this.label3.TabIndex = 6;
      this.label3.Text = "of";
      // 
      // textBoxTrackCnt
      // 
      this.textBoxTrackCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxTrackCnt.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxTrackCnt.Location = new System.Drawing.Point(404, 65);
      this.textBoxTrackCnt.Name = "textBoxTrackCnt";
      this.textBoxTrackCnt.Size = new System.Drawing.Size(74, 25);
      this.textBoxTrackCnt.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(230)))), ((int)(((byte)(246)))));
      this.label4.Location = new System.Drawing.Point(482, 68);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(13, 17);
      this.label4.TabIndex = 8;
      this.label4.Text = ")";
      // 
      // SettingsWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(507, 155);
      this.Controls.Add(this.splitContainer1);
      this.MaximumSize = new System.Drawing.Size(99999999, 194);
      this.MinimumSize = new System.Drawing.Size(523, 194);
      this.Name = "SettingsWindow";
      this.ShowIcon = false;
      this.Text = "DiscordBee Settings";
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.TextBox textBoxDetails;
    private System.Windows.Forms.TextBox textBoxState;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox textBoxTrackCnt;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBoxTrackNo;
    private System.Windows.Forms.Label label2;
  }
}