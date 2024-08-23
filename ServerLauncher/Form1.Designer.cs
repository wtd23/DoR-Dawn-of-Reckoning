namespace ServerLauncher
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.B_start = new System.Windows.Forms.Button();
            this.B_stop = new System.Windows.Forms.Button();
            this.StartAccountCheckBox = new System.Windows.Forms.CheckBox();
            this.StartLauncherCheckBox = new System.Windows.Forms.CheckBox();
            this.StartLobbyCheckBox = new System.Windows.Forms.CheckBox();
            this.StartWorldCheckBox = new System.Windows.Forms.CheckBox();
            this.wrldsStartArguments = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lobsStartArgument = new System.Windows.Forms.TextBox();
            this.lsStartArguments = new System.Windows.Forms.TextBox();
            this.acStartArgument = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.acWatchChk = new System.Windows.Forms.CheckBox();
            this.lausWatchChk = new System.Windows.Forms.CheckBox();
            this.lsWatchChk = new System.Windows.Forms.CheckBox();
            this.wsWatchChk = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // B_start
            // 
            this.B_start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_start.Location = new System.Drawing.Point(10, 147);
            this.B_start.Name = "B_start";
            this.B_start.Size = new System.Drawing.Size(91, 23);
            this.B_start.TabIndex = 1;
            this.B_start.Text = "Start Selected";
            this.B_start.UseVisualStyleBackColor = true;
            this.B_start.Click += new System.EventHandler(this.B_start_Click);
            // 
            // B_stop
            // 
            this.B_stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_stop.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.B_stop.Location = new System.Drawing.Point(286, 147);
            this.B_stop.Name = "B_stop";
            this.B_stop.Size = new System.Drawing.Size(91, 23);
            this.B_stop.TabIndex = 2;
            this.B_stop.Text = "Stop All";
            this.B_stop.UseVisualStyleBackColor = false;
            this.B_stop.Click += new System.EventHandler(this.B_stop_Click);
            // 
            // StartAccountCheckBox
            // 
            this.StartAccountCheckBox.AutoSize = true;
            this.StartAccountCheckBox.Checked = true;
            this.StartAccountCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartAccountCheckBox.Location = new System.Drawing.Point(71, 39);
            this.StartAccountCheckBox.Name = "StartAccountCheckBox";
            this.StartAccountCheckBox.Size = new System.Drawing.Size(125, 17);
            this.StartAccountCheckBox.TabIndex = 3;
            this.StartAccountCheckBox.Text = "Start AccountCacher";
            this.StartAccountCheckBox.UseVisualStyleBackColor = true;
            // 
            // StartLauncherCheckBox
            // 
            this.StartLauncherCheckBox.AutoSize = true;
            this.StartLauncherCheckBox.Checked = true;
            this.StartLauncherCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartLauncherCheckBox.Location = new System.Drawing.Point(71, 63);
            this.StartLauncherCheckBox.Name = "StartLauncherCheckBox";
            this.StartLauncherCheckBox.Size = new System.Drawing.Size(127, 17);
            this.StartLauncherCheckBox.TabIndex = 4;
            this.StartLauncherCheckBox.Text = "Start LauncherServer";
            this.StartLauncherCheckBox.UseVisualStyleBackColor = true;
            // 
            // StartLobbyCheckBox
            // 
            this.StartLobbyCheckBox.AutoSize = true;
            this.StartLobbyCheckBox.Checked = true;
            this.StartLobbyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartLobbyCheckBox.Location = new System.Drawing.Point(71, 87);
            this.StartLobbyCheckBox.Name = "StartLobbyCheckBox";
            this.StartLobbyCheckBox.Size = new System.Drawing.Size(111, 17);
            this.StartLobbyCheckBox.TabIndex = 5;
            this.StartLobbyCheckBox.Text = "Start LobbyServer";
            this.StartLobbyCheckBox.UseVisualStyleBackColor = true;
            // 
            // StartWorldCheckBox
            // 
            this.StartWorldCheckBox.AutoSize = true;
            this.StartWorldCheckBox.Checked = true;
            this.StartWorldCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartWorldCheckBox.Location = new System.Drawing.Point(71, 111);
            this.StartWorldCheckBox.Name = "StartWorldCheckBox";
            this.StartWorldCheckBox.Size = new System.Drawing.Size(110, 17);
            this.StartWorldCheckBox.TabIndex = 6;
            this.StartWorldCheckBox.Text = "Start WorldServer";
            this.StartWorldCheckBox.UseVisualStyleBackColor = true;
            // 
            // wrldsStartArguments
            // 
            this.wrldsStartArguments.Location = new System.Drawing.Point(219, 108);
            this.wrldsStartArguments.Name = "wrldsStartArguments";
            this.wrldsStartArguments.Size = new System.Drawing.Size(159, 20);
            this.wrldsStartArguments.TabIndex = 7;
            this.wrldsStartArguments.Text = "-priority -debug -physics";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Server";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(216, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Arguments";
            // 
            // lobsStartArgument
            // 
            this.lobsStartArgument.Location = new System.Drawing.Point(219, 84);
            this.lobsStartArgument.Name = "lobsStartArgument";
            this.lobsStartArgument.Size = new System.Drawing.Size(159, 20);
            this.lobsStartArgument.TabIndex = 10;
            // 
            // lsStartArguments
            // 
            this.lsStartArguments.Location = new System.Drawing.Point(219, 60);
            this.lsStartArguments.Name = "lsStartArguments";
            this.lsStartArguments.Size = new System.Drawing.Size(159, 20);
            this.lsStartArguments.TabIndex = 11;
            // 
            // acStartArgument
            // 
            this.acStartArgument.Location = new System.Drawing.Point(219, 36);
            this.acStartArgument.Name = "acStartArgument";
            this.acStartArgument.Size = new System.Drawing.Size(159, 20);
            this.acStartArgument.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Watch";
            // 
            // acWatchChk
            // 
            this.acWatchChk.AutoSize = true;
            this.acWatchChk.Checked = true;
            this.acWatchChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.acWatchChk.Location = new System.Drawing.Point(24, 40);
            this.acWatchChk.Name = "acWatchChk";
            this.acWatchChk.Size = new System.Drawing.Size(15, 14);
            this.acWatchChk.TabIndex = 14;
            this.acWatchChk.UseVisualStyleBackColor = true;
            // 
            // lausWatchChk
            // 
            this.lausWatchChk.AutoSize = true;
            this.lausWatchChk.Checked = true;
            this.lausWatchChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lausWatchChk.Location = new System.Drawing.Point(24, 64);
            this.lausWatchChk.Name = "lausWatchChk";
            this.lausWatchChk.Size = new System.Drawing.Size(15, 14);
            this.lausWatchChk.TabIndex = 15;
            this.lausWatchChk.UseVisualStyleBackColor = true;
            // 
            // lsWatchChk
            // 
            this.lsWatchChk.AutoSize = true;
            this.lsWatchChk.Checked = true;
            this.lsWatchChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lsWatchChk.Location = new System.Drawing.Point(24, 88);
            this.lsWatchChk.Name = "lsWatchChk";
            this.lsWatchChk.Size = new System.Drawing.Size(15, 14);
            this.lsWatchChk.TabIndex = 16;
            this.lsWatchChk.UseVisualStyleBackColor = true;
            // 
            // wsWatchChk
            // 
            this.wsWatchChk.AutoSize = true;
            this.wsWatchChk.Checked = true;
            this.wsWatchChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wsWatchChk.Location = new System.Drawing.Point(24, 112);
            this.wsWatchChk.Name = "wsWatchChk";
            this.wsWatchChk.Size = new System.Drawing.Size(15, 14);
            this.wsWatchChk.TabIndex = 17;
            this.wsWatchChk.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(389, 182);
            this.Controls.Add(this.wsWatchChk);
            this.Controls.Add(this.lsWatchChk);
            this.Controls.Add(this.lausWatchChk);
            this.Controls.Add(this.acWatchChk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.acStartArgument);
            this.Controls.Add(this.lsStartArguments);
            this.Controls.Add(this.lobsStartArgument);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.wrldsStartArguments);
            this.Controls.Add(this.StartWorldCheckBox);
            this.Controls.Add(this.StartLobbyCheckBox);
            this.Controls.Add(this.StartLauncherCheckBox);
            this.Controls.Add(this.StartAccountCheckBox);
            this.Controls.Add(this.B_stop);
            this.Controls.Add(this.B_start);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ServerLauncher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_start;
        private System.Windows.Forms.Button B_stop;
        private System.Windows.Forms.CheckBox StartAccountCheckBox;
        private System.Windows.Forms.CheckBox StartLauncherCheckBox;
        private System.Windows.Forms.CheckBox StartLobbyCheckBox;
        private System.Windows.Forms.CheckBox StartWorldCheckBox;
        private System.Windows.Forms.TextBox wrldsStartArguments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox lobsStartArgument;
        private System.Windows.Forms.TextBox lsStartArguments;
        private System.Windows.Forms.TextBox acStartArgument;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox acWatchChk;
        private System.Windows.Forms.CheckBox lausWatchChk;
        private System.Windows.Forms.CheckBox lsWatchChk;
        private System.Windows.Forms.CheckBox wsWatchChk;
    }
}

