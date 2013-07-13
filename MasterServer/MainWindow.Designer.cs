namespace MasterServer
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblConnectedPlayersCount = new System.Windows.Forms.Label();
            this.mainWindowBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.lblRunningGamesTotal = new System.Windows.Forms.Label();
            this.lbOut = new System.Windows.Forms.ListBox();
            this.lblRunningQuickGameCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblRunningTournamentsCount = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTotalTournaments = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblTotalQuickGames = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTotalGames = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblTotalPlayers = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mainWindowBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Vector2(283, 383);
            this.btnExit.Name = "btnExit";
            this.btnExit.Rectangle = new System.Drawing.Rectangle(75, 23);
            this.btnExit.TabIndex = 0;
            this.btnExit.Text = "Ukončit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Vector2(40, 36);
            this.label1.Name = "label1";
            this.label1.Rectangle = new System.Drawing.Rectangle(123, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Právě připojených hráčů";
            // 
            // lblConnectedPlayersCount
            // 
            this.lblConnectedPlayersCount.AutoSize = true;
            this.lblConnectedPlayersCount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "Players", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblConnectedPlayersCount.Location = new System.Drawing.Vector2(192, 36);
            this.lblConnectedPlayersCount.Name = "lblConnectedPlayersCount";
            this.lblConnectedPlayersCount.Rectangle = new System.Drawing.Rectangle(13, 13);
            this.lblConnectedPlayersCount.TabIndex = 2;
            this.lblConnectedPlayersCount.Text = "0";

            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Vector2(40, 82);
            this.label3.Name = "label3";
            this.label3.Rectangle = new System.Drawing.Rectangle(139, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Celkový počet běžících her";
            // 
            // lblRunningGamesTotal
            // 
            this.lblRunningGamesTotal.AutoSize = true;
            this.lblRunningGamesTotal.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "RunningGames", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblRunningGamesTotal.Location = new System.Drawing.Vector2(192, 82);
            this.lblRunningGamesTotal.Name = "lblRunningGamesTotal";
            this.lblRunningGamesTotal.Rectangle = new System.Drawing.Rectangle(13, 13);
            this.lblRunningGamesTotal.TabIndex = 4;
            this.lblRunningGamesTotal.Text = "0";
            // 
            // lbOut
            // 
            this.lbOut.FormattingEnabled = true;
            this.lbOut.Location = new System.Drawing.Vector2(12, 106);
            this.lbOut.Name = "lbOut";
            this.lbOut.Rectangle = new System.Drawing.Rectangle(616, 238);
            this.lbOut.TabIndex = 5;
            // 
            // lblRunningQuickGameCount
            // 
            this.lblRunningQuickGameCount.AutoSize = true;
            this.lblRunningQuickGameCount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "QuickGames", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblRunningQuickGameCount.Location = new System.Drawing.Vector2(192, 57);
            this.lblRunningQuickGameCount.Name = "lblRunningQuickGameCount";
            this.lblRunningQuickGameCount.Rectangle = new System.Drawing.Rectangle(13, 13);
            this.lblRunningQuickGameCount.TabIndex = 7;
            this.lblRunningQuickGameCount.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Vector2(40, 57);
            this.label4.Name = "label4";
            this.label4.Rectangle = new System.Drawing.Rectangle(141, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Počet běžících rychlých her";
            // 
            // lblRunningTournamentsCount
            // 
            this.lblRunningTournamentsCount.AutoSize = true;
            this.lblRunningTournamentsCount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "Tournaments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblRunningTournamentsCount.Location = new System.Drawing.Vector2(192, 70);
            this.lblRunningTournamentsCount.Name = "lblRunningTournamentsCount";
            this.lblRunningTournamentsCount.Rectangle = new System.Drawing.Rectangle(13, 13);
            this.lblRunningTournamentsCount.TabIndex = 9;
            this.lblRunningTournamentsCount.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Vector2(40, 70);
            this.label6.Name = "label6";
            this.label6.Rectangle = new System.Drawing.Rectangle(116, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Počet běžících turnajů";
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Location = new System.Drawing.Vector2(192, 14);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Rectangle = new System.Drawing.Rectangle(58, 13);
            this.lblStartTime.TabIndex = 11;
            this.lblStartTime.Text = "0:00:00:00";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Vector2(40, 14);
            this.label5.Name = "label5";
            this.label5.Rectangle = new System.Drawing.Rectangle(92, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Čas startu serveru";
            // 
            // lblTotalTournaments
            // 
            this.lblTotalTournaments.AutoSize = true;
            this.lblTotalTournaments.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "TotalTournaments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblTotalTournaments.Location = new System.Drawing.Vector2(408, 70);
            this.lblTotalTournaments.Name = "lblTotalTournaments";
            this.lblTotalTournaments.Rectangle = new System.Drawing.Rectangle(13, 13);
            this.lblTotalTournaments.TabIndex = 19;
            this.lblTotalTournaments.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Vector2(256, 70);
            this.label7.Name = "label7";
            this.label7.Rectangle = new System.Drawing.Rectangle(129, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Počet odehraných turnajů";
            // 
            // lblTotalQuickGames
            // 
            this.lblTotalQuickGames.AutoSize = true;
            this.lblTotalQuickGames.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "TotalQuickGames", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblTotalQuickGames.Location = new System.Drawing.Vector2(408, 57);
            this.lblTotalQuickGames.Name = "lblTotalQuickGames";
            this.lblTotalQuickGames.Rectangle = new System.Drawing.Rectangle(13, 13);
            this.lblTotalQuickGames.TabIndex = 17;
            this.lblTotalQuickGames.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Vector2(256, 57);
            this.label9.Name = "label9";
            this.label9.Rectangle = new System.Drawing.Rectangle(154, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Počet odehraných rychlých her";
            // 
            // lblTotalGames
            // 
            this.lblTotalGames.AutoSize = true;
            this.lblTotalGames.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "PlayedGames", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblTotalGames.Location = new System.Drawing.Vector2(408, 82);
            this.lblTotalGames.Name = "lblTotalGames";
            this.lblTotalGames.Rectangle = new System.Drawing.Rectangle(13, 13);
            this.lblTotalGames.TabIndex = 15;
            this.lblTotalGames.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Vector2(256, 82);
            this.label11.Name = "label11";
            this.label11.Rectangle = new System.Drawing.Rectangle(152, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Celkový počet odehraných her";
            // 
            // lblTotalPlayers
            // 
            this.lblTotalPlayers.AutoSize = true;
            this.lblTotalPlayers.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "TotalPlayers", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblTotalPlayers.Location = new System.Drawing.Vector2(408, 36);
            this.lblTotalPlayers.Name = "lblTotalPlayers";
            this.lblTotalPlayers.Rectangle = new System.Drawing.Rectangle(13, 13);
            this.lblTotalPlayers.TabIndex = 13;
            this.lblTotalPlayers.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Vector2(256, 36);
            this.label13.Name = "label13";
            this.label13.Rectangle = new System.Drawing.Rectangle(105, 13);
            this.label13.TabIndex = 12;
            this.label13.Text = "Celkový počet hráčů";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Rectangle(640, 428);
            this.Controls.Add(this.lblTotalTournaments);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblTotalQuickGames);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblTotalGames);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblTotalPlayers);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblRunningTournamentsCount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblRunningQuickGameCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbOut);
            this.Controls.Add(this.lblRunningGamesTotal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblConnectedPlayersCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExit);
            this.Name = "MainWindow";
            this.Text = "Orbital MasterServer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mainWindowBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblConnectedPlayersCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblRunningGamesTotal;
        private System.Windows.Forms.ListBox lbOut;
        private System.Windows.Forms.Label lblRunningQuickGameCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblRunningTournamentsCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblTotalTournaments;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTotalQuickGames;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTotalGames;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblTotalPlayers;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.BindingSource mainWindowBindingSource;
    }
}

