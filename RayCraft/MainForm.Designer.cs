namespace RayCraft
{
    partial class MainForm
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
            this.LoginPane = new System.Windows.Forms.Panel();
            this.PlayButton = new System.Windows.Forms.Button();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.RenderBtn = new System.Windows.Forms.Button();
            this.StatsLabel = new System.Windows.Forms.Label();
            this.ServerLabel = new System.Windows.Forms.Label();
            this.ServerBox = new System.Windows.Forms.TextBox();
            this.LoginPane.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoginPane
            // 
            this.LoginPane.Controls.Add(this.ServerLabel);
            this.LoginPane.Controls.Add(this.ServerBox);
            this.LoginPane.Controls.Add(this.PlayButton);
            this.LoginPane.Controls.Add(this.UsernameLabel);
            this.LoginPane.Controls.Add(this.UsernameBox);
            this.LoginPane.Controls.Add(this.TitleLabel);
            this.LoginPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoginPane.Location = new System.Drawing.Point(0, 0);
            this.LoginPane.Name = "LoginPane";
            this.LoginPane.Size = new System.Drawing.Size(391, 332);
            this.LoginPane.TabIndex = 0;
            // 
            // PlayButton
            // 
            this.PlayButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PlayButton.Location = new System.Drawing.Point(158, 241);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(75, 23);
            this.PlayButton.TabIndex = 3;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(168, 119);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(55, 13);
            this.UsernameLabel.TabIndex = 2;
            this.UsernameLabel.Text = "Username";
            // 
            // UsernameBox
            // 
            this.UsernameBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.UsernameBox.Location = new System.Drawing.Point(96, 138);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(199, 20);
            this.UsernameBox.TabIndex = 1;
            this.UsernameBox.Text = "DevClient";
            this.UsernameBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TitleLabel
            // 
            this.TitleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(95, 68);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(201, 30);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "WinForms Minecraft";
            // 
            // RenderBtn
            // 
            this.RenderBtn.Location = new System.Drawing.Point(12, 12);
            this.RenderBtn.Name = "RenderBtn";
            this.RenderBtn.Size = new System.Drawing.Size(98, 23);
            this.RenderBtn.TabIndex = 1;
            this.RenderBtn.Text = "Start Render";
            this.RenderBtn.UseVisualStyleBackColor = true;
            this.RenderBtn.Click += new System.EventHandler(this.RenderBtn_Click);
            // 
            // StatsLabel
            // 
            this.StatsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatsLabel.AutoSize = true;
            this.StatsLabel.Location = new System.Drawing.Point(-1, 319);
            this.StatsLabel.Name = "StatsLabel";
            this.StatsLabel.Size = new System.Drawing.Size(27, 13);
            this.StatsLabel.TabIndex = 2;
            this.StatsLabel.Text = "FPS";
            // 
            // ServerLabel
            // 
            this.ServerLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ServerLabel.AutoSize = true;
            this.ServerLabel.Location = new System.Drawing.Point(176, 173);
            this.ServerLabel.Name = "ServerLabel";
            this.ServerLabel.Size = new System.Drawing.Size(38, 13);
            this.ServerLabel.TabIndex = 5;
            this.ServerLabel.Text = "Server";
            // 
            // ServerBox
            // 
            this.ServerBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ServerBox.Location = new System.Drawing.Point(96, 192);
            this.ServerBox.Name = "ServerBox";
            this.ServerBox.Size = new System.Drawing.Size(199, 20);
            this.ServerBox.TabIndex = 4;
            this.ServerBox.Text = "localhost";
            this.ServerBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(391, 332);
            this.Controls.Add(this.StatsLabel);
            this.Controls.Add(this.LoginPane);
            this.Controls.Add(this.RenderBtn);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "WinformsMC";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.LoginPane.ResumeLayout(false);
            this.LoginPane.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel LoginPane;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button RenderBtn;
        private System.Windows.Forms.Label StatsLabel;
        private System.Windows.Forms.Label ServerLabel;
        private System.Windows.Forms.TextBox ServerBox;
    }
}

