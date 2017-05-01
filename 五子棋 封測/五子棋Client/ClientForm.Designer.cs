namespace 五子棋Client
{
    partial class ClientForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.IPTxtBox = new System.Windows.Forms.TextBox();
            this.NameTxtBox = new System.Windows.Forms.TextBox();
            this.IPlabel = new System.Windows.Forms.Label();
            this.NameLbl = new System.Windows.Forms.Label();
            this.StartBtn = new System.Windows.Forms.Button();
            this.LeaveBtn = new System.Windows.Forms.Button();
            this.HomepagePicBox = new System.Windows.Forms.PictureBox();
            this.LoadingBeforeSelect = new System.ComponentModel.BackgroundWorker();
            this.RcvAllInfoBeforeGame = new System.ComponentModel.BackgroundWorker();
            this.GameTmr = new System.Windows.Forms.Timer(this.components);
            this.RcvInfoInGame = new System.ComponentModel.BackgroundWorker();
            this.NewGameBckWrkr = new System.ComponentModel.BackgroundWorker();
            this.CountDownTmr = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.HomepagePicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.TitleLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.TitleLabel.Font = new System.Drawing.Font("標楷體", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TitleLabel.Location = new System.Drawing.Point(316, 44);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(331, 96);
            this.TitleLabel.TabIndex = 4;
            this.TitleLabel.Text = "五子棋";
            // 
            // IPTxtBox
            // 
            this.IPTxtBox.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.IPTxtBox.Location = new System.Drawing.Point(680, 427);
            this.IPTxtBox.Name = "IPTxtBox";
            this.IPTxtBox.Size = new System.Drawing.Size(218, 43);
            this.IPTxtBox.TabIndex = 5;
            this.IPTxtBox.Text = "127.0.0.1";
            // 
            // NameTxtBox
            // 
            this.NameTxtBox.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.NameTxtBox.Location = new System.Drawing.Point(680, 496);
            this.NameTxtBox.Name = "NameTxtBox";
            this.NameTxtBox.Size = new System.Drawing.Size(218, 43);
            this.NameTxtBox.TabIndex = 5;
            this.NameTxtBox.Text = "玩家";
            // 
            // IPlabel
            // 
            this.IPlabel.AutoSize = true;
            this.IPlabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.IPlabel.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.IPlabel.ForeColor = System.Drawing.Color.White;
            this.IPlabel.Location = new System.Drawing.Point(483, 430);
            this.IPlabel.Name = "IPlabel";
            this.IPlabel.Size = new System.Drawing.Size(181, 40);
            this.IPlabel.TabIndex = 6;
            this.IPlabel.Text = "server端IP:";
            // 
            // NameLbl
            // 
            this.NameLbl.AutoSize = true;
            this.NameLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NameLbl.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.NameLbl.ForeColor = System.Drawing.Color.White;
            this.NameLbl.Location = new System.Drawing.Point(483, 499);
            this.NameLbl.Name = "NameLbl";
            this.NameLbl.Size = new System.Drawing.Size(153, 40);
            this.NameLbl.TabIndex = 6;
            this.NameLbl.Text = "你的名字:";
            // 
            // StartBtn
            // 
            this.StartBtn.BackColor = System.Drawing.Color.Moccasin;
            this.StartBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.StartBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.StartBtn.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.StartBtn.ForeColor = System.Drawing.Color.Maroon;
            this.StartBtn.Location = new System.Drawing.Point(680, 564);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(97, 58);
            this.StartBtn.TabIndex = 7;
            this.StartBtn.Text = "登入";
            this.StartBtn.UseVisualStyleBackColor = false;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // LeaveBtn
            // 
            this.LeaveBtn.BackColor = System.Drawing.Color.Moccasin;
            this.LeaveBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LeaveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.LeaveBtn.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.LeaveBtn.ForeColor = System.Drawing.Color.Maroon;
            this.LeaveBtn.Location = new System.Drawing.Point(801, 564);
            this.LeaveBtn.Name = "LeaveBtn";
            this.LeaveBtn.Size = new System.Drawing.Size(97, 58);
            this.LeaveBtn.TabIndex = 7;
            this.LeaveBtn.Text = "離開";
            this.LeaveBtn.UseVisualStyleBackColor = false;
            this.LeaveBtn.Click += new System.EventHandler(this.LeaveBtn_Click);
            // 
            // HomepagePicBox
            // 
            this.HomepagePicBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HomepagePicBox.Image = global::五子棋Client.Properties.Resources.封面;
            this.HomepagePicBox.Location = new System.Drawing.Point(0, 0);
            this.HomepagePicBox.Name = "HomepagePicBox";
            this.HomepagePicBox.Size = new System.Drawing.Size(945, 649);
            this.HomepagePicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.HomepagePicBox.TabIndex = 0;
            this.HomepagePicBox.TabStop = false;
            // 
            // LoadingBeforeSelect
            // 
            this.LoadingBeforeSelect.DoWork += new System.ComponentModel.DoWorkEventHandler(this.LoadingBeforeSelect_DoWork);
            this.LoadingBeforeSelect.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.LoadingBeforeSelect_RunWorkerCompleted);
            // 
            // RcvAllInfoBeforeGame
            // 
            this.RcvAllInfoBeforeGame.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RcvAllInfoBeforeGame_DoWork);
            this.RcvAllInfoBeforeGame.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.RcvAllInfoBeforeGame_RunWorkerCompleted);
            // 
            // GameTmr
            // 
            this.GameTmr.Interval = 1000;
            this.GameTmr.Tick += new System.EventHandler(this.GameTmr_Tick);
            // 
            // RcvInfoInGame
            // 
            this.RcvInfoInGame.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RcvInfoInGame_DoWork);
            this.RcvInfoInGame.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.RcvInfoInGame_RunWorkerCompleted);
            // 
            // NewGameBckWrkr
            // 
            this.NewGameBckWrkr.DoWork += new System.ComponentModel.DoWorkEventHandler(this.NewGameBckWrkr_DoWork);
            this.NewGameBckWrkr.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.NewGameBckWrkr_RunWorkerCompleted);
            // 
            // CountDownTmr
            // 
            this.CountDownTmr.Interval = 94;
            this.CountDownTmr.Tick += new System.EventHandler(this.CountDownTmr_Tick);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::五子棋Client.Properties.Resources.封面;
            this.ClientSize = new System.Drawing.Size(945, 649);
            this.Controls.Add(this.LeaveBtn);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.NameLbl);
            this.Controls.Add(this.IPlabel);
            this.Controls.Add(this.NameTxtBox);
            this.Controls.Add(this.IPTxtBox);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.HomepagePicBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ClientForm";
            this.Text = "五子棋";
            ((System.ComponentModel.ISupportInitialize)(this.HomepagePicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox HomepagePicBox;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.TextBox IPTxtBox;
        private System.Windows.Forms.TextBox NameTxtBox;
        private System.Windows.Forms.Label IPlabel;
        private System.Windows.Forms.Label NameLbl;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Button LeaveBtn;
        private System.ComponentModel.BackgroundWorker LoadingBeforeSelect;
        private System.ComponentModel.BackgroundWorker RcvAllInfoBeforeGame;
        private System.Windows.Forms.Timer GameTmr;
        private System.ComponentModel.BackgroundWorker RcvInfoInGame;
        private System.ComponentModel.BackgroundWorker NewGameBckWrkr;
        private System.Windows.Forms.Timer CountDownTmr;
    }
}

