using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using Information;
using 五子棋Client.Properties;

namespace 五子棋Client
{
    public partial class ClientForm : Form
    {
        TcpClient Client;//宣告TcpClient連線類別
        Packet MyInfo;//宣告遊戲資訊類別
        NetworkStream MyStream;//網路資料流
        Thread _ReceiveIDThread;//接收ID的Thread
        Button ConfirmButton;//確認下棋按鈕
        Button NewGameBtn;//重新開局按鈕
        PictureBox[,] Pixels;//棋盤上的225個圖片方格
        PictureBox[] SelectPhoto;//選擇角色頭像
        PictureBox LoadingPctrBox;//LoadIng圖案
        PictureBox LeftPhoto;//玩家1頭像
        PictureBox RightPhoto;//玩家2頭像
        PictureBox WinPctrBox;//獲勝玩家圖像
        Label ChooseCharacterLabel;//選擇角色標籤
        Label Vursus;//V.S.標籤
        Label GameTimeLbl;//遊戲計時標籤
        Label CountDownLbl;//遊戲倒數標籤
        Label WinLbl;//獲勝玩家名稱標籤
        Label LeftName;
        Label RightName;
        int ID;
        int GameTime;//遊戲時間
        int CountDown;//回合倒數

        public ClientForm()
        {
            InitializeComponent();
            Client = new TcpClient();//建立TcpClient連線類別
            MyInfo = new Packet();//建立遊戲資訊類別
        }

        private void LeaveBtn_Click(object sender, EventArgs e)//按下離開鍵則關閉視窗
        {
            this.Close();
            this.Dispose();
        }

        private void StartBtn_Click(object sender, EventArgs e)//按下登入鍵則繼續遊戲
        {
            //建立連線並連接到server端
            try//成功連接server
            {
                Client.Connect(new IPEndPoint(IPAddress.Parse(IPTxtBox.Text), 5200));//connect至server端
                MyStream = Client.GetStream();
            }
            catch (Exception ex)//連接server失敗
            {
                MessageBox.Show(ex.ToString() + "\n\n可能的原因:\n1.IP輸入有誤\n2.server端未開啟連接\n3.沒有網路連接\n\n請嘗試重新連接");//顯示錯誤
                return;
            }
            //將個人基本資訊存入遊戲資訊
            GetPersonalInfo();
            //接收ID順序並寄送個人資訊到server端
            LoadingBeforeSelect.RunWorkerAsync();

            Loading1();
        }

        private void GetPersonalInfo()//取得個人基本資訊
        {
            //取得本Client端IP位址
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    MyInfo.MyIPAddress = address.ToString();//將本機IP位址存入遊戲資訊
                }
            }
            //取得輸入名稱
            MyInfo.MyName = NameTxtBox.Text;
        }

        private void ReceiveIDThread()//接收自己的ID順序並Send個人資訊
        {
            Byte[] RcvBuf=new Byte[Client.ReceiveBufferSize];
            MyStream.Read(RcvBuf,0,RcvBuf.Length);
            ID = BitConverter.ToInt32(RcvBuf, 0);

            SendPersonalInfo();
        }

        private void ReceiveAllInfoBeforeGameThread()//在遊戲開始前接收所有資訊
        {
            Packet TmpPkt;
            Byte[] RcvBuf = new Byte[Client.ReceiveBufferSize];
            MyStream.Read(RcvBuf, 0, RcvBuf.Length);
            TmpPkt = MyInfo.BytesToPacket(RcvBuf);
            Array.Copy(TmpPkt.AllChar, MyInfo.AllChar, TmpPkt.AllChar.Length);
            Array.Copy(TmpPkt.AllIP, MyInfo.AllIP, TmpPkt.AllIP.Length);
            Array.Copy(TmpPkt.AllName, MyInfo.AllName, TmpPkt.AllName.Length);
        }

        private void SendPersonalInfo()//傳送個人資訊給Server
        {
            Byte[] SendBuf = new Byte[Client.SendBufferSize];//宣告且建立傳送資料緩衝區
            SendBuf = MyInfo.PacketToBytes();//將MyInfo轉成Byte[]型態
            MyStream.Write(SendBuf, 0, SendBuf.Length);//傳送MyInfo
        }

        private void ReceiveDataInGame()//接收遊戲過程資訊
        {
            Packet TmpPkt;
            Byte[] RcvBuf = new Byte[Client.SendBufferSize];
            MyStream.Read(RcvBuf, 0, RcvBuf.Length);
            TmpPkt = MyInfo.BytesToPacket(RcvBuf);
            Array.Copy(TmpPkt.AllNewGame, MyInfo.AllNewGame, TmpPkt.AllNewGame.Length);
            if (ID == 1)
            {
                MyInfo.Leave2 = TmpPkt.Leave2;
            }
            else
            {
                MyInfo.Leave1 = TmpPkt.Leave1;
            }
        }

        private void SelectMode()//進入選擇角色畫面
        {
            LoadingPctrBox.Visible = false;
            SelectPhoto = new PictureBox[3];
            //選擇大頭貼標籤
            ChooseCharacterLabel = new Label();
            ChooseCharacterLabel.AutoSize = true;
            ChooseCharacterLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            ChooseCharacterLabel.Font = new System.Drawing.Font("微軟正黑體", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            ChooseCharacterLabel.Location = new System.Drawing.Point(358, 74);
            ChooseCharacterLabel.Name = "ChooseCharacterLabel";
            ChooseCharacterLabel.Size = new System.Drawing.Size(265, 45);
            ChooseCharacterLabel.TabIndex = 4;
            ChooseCharacterLabel.Text = "請選擇一位角色";
            this.Controls.Add(ChooseCharacterLabel);
            ChooseCharacterLabel.BringToFront();
            //生成頭像選項
            for (int i = 0; i < 3; i++)
            {
                SelectPhoto[i] = new PictureBox();
                SelectPhoto[i].Cursor = System.Windows.Forms.Cursors.Hand;
                switch (i)
                {
                    case 0:
                        SelectPhoto[i].Image = Resources.進藤光;
                        SelectPhoto[i].Click += SelectPhoto1_Click;
                        break;
                    case 1:
                        SelectPhoto[i].Image = Resources.左為;
                        SelectPhoto[i].Click += SelectPhoto2_Click;
                        break;
                    case 2:
                        SelectPhoto[i].Image = Resources.塔矢亮;
                        SelectPhoto[i].Click += SelectPhoto3_Click;
                        break;
                }
                SelectPhoto[i].Location = new System.Drawing.Point(80 + i * 300, 195);
                SelectPhoto[i].Name = "ChoosePhoto1";
                SelectPhoto[i].Size = new System.Drawing.Size(183, 237);
                SelectPhoto[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                SelectPhoto[i].TabStop = false;
                Controls.Add(SelectPhoto[i]);
                SelectPhoto[i].BringToFront();
            }
        }

        private void SelectPhoto1_Click(object sender, EventArgs e)//選擇進藤光頭像所觸發之事件
        {
            Loading2();
            MyInfo.MyChar = CharPhoto.進藤光;
            SendPersonalInfo();
            RcvAllInfoBeforeGame.RunWorkerAsync();
        }

        private void SelectPhoto2_Click(object sender, EventArgs e)//選擇佐為頭像所觸發之事件
        {
            Loading2();
            MyInfo.MyChar = CharPhoto.佐為;
            SendPersonalInfo();
            RcvAllInfoBeforeGame.RunWorkerAsync();
        }

        private void SelectPhoto3_Click(object sender, EventArgs e)//選擇塔矢亮頭像所觸發之事件
        {
            Loading2();
            MyInfo.MyChar = CharPhoto.塔矢亮;
            SendPersonalInfo();
            RcvAllInfoBeforeGame.RunWorkerAsync();
        }

        private void StartGame()//遊戲開始畫面
        {
            
            //更新畫面
            HomepagePicBox.Dispose();
            LoadingPctrBox.Visible = false;
            this.BackgroundImage = Resources.棋盤;
            //倒數器標籤
            CountDownLbl = new Label();
            CountDownLbl.AutoSize = true;
            CountDownLbl.BackColor = System.Drawing.Color.White;
            CountDownLbl.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            CountDownLbl.Location = new System.Drawing.Point(20, 610);
            CountDownLbl.Name = "CountDownLbl";
            CountDownLbl.TabStop = false;
            CountDownLbl.Text = "剩餘時間: 30.00";
            Controls.Add(CountDownLbl);
            //計時器標籤
            GameTimeLbl = new Label();
            GameTimeLbl.AutoSize = true;
            GameTimeLbl.BackColor = System.Drawing.Color.White;
            GameTimeLbl.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            GameTimeLbl.Location = new System.Drawing.Point(688, 610);
            GameTimeLbl.Name = "GameTimeLbl";
            GameTimeLbl.Size = new System.Drawing.Size(245, 40);
            GameTimeLbl.TabStop = false;
            GameTimeLbl.Text = "遊戲時間: 00:00";
            Controls.Add(GameTimeLbl);
            //對戰標籤
            Vursus = new Label();
            Vursus.AutoSize = true;
            Vursus.BackColor = Color.White;
            Vursus.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            Vursus.Location = new Point(360, 5);
            Vursus.Name = "Vursus";
            Vursus.TabStop = false;
            Vursus.Text = "●     " + " V.S. " + "     ○";
            Controls.Add(Vursus);
            //玩家1姓名標籤
            LeftName = new Label();
            LeftName.AutoSize = true;
            LeftName.Location = new Point(20, 5);
            LeftName.Font = new Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            LeftName.Name = "LeftName";
            LeftName.TabStop = false;
            LeftName.Text = MyInfo.AllName[0];
            Controls.Add(LeftName);
            //玩家2姓名標籤
            RightName = new Label();
            RightName.AutoSize = true;
            RightName.Font = new Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            RightName.Location = new Point(800, 5);
            RightName.Name = "LeftName";
            RightName.TabStop = false;
            RightName.Text = MyInfo.AllName[1];
            Controls.Add(RightName);
            //動態配置pixel
            Pixels = new PictureBox[15, 15];
            int row = 45;
            for (int i = 0; i < 15; i++)
            {
                int column = 149;
                for (int j = 0; j < 15; j++)
                {
                    Pixels[i, j] = new PictureBox();
                    Pixels[i, j].BackColor = System.Drawing.Color.Transparent;
                    Pixels[i, j].Cursor = System.Windows.Forms.Cursors.Hand;
                    Pixels[i, j].Location = new System.Drawing.Point(column, row);
                    Pixels[i, j].Size = new System.Drawing.Size(44, 38);
                    Pixels[i, j].SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
                    Pixels[i, j].TabStop = false;
                    Pixels[i, j].Click += pixel_Click;
                    this.Controls.Add(Pixels[i, j]);
                    Pixels[i, j].BringToFront();
                    column += 43;
                }
                row += 38;
            }
            //生成玩家1頭像
            LeftPhoto = new PictureBox();
            switch (MyInfo.AllChar[0])
            {
                case CharPhoto.進藤光:
                    LeftPhoto.Image = Resources.進藤光;
                    break;
                case CharPhoto.佐為:
                    LeftPhoto.Image = Resources.左為;
                    break;
                case CharPhoto.塔矢亮:
                    LeftPhoto.Image = Resources.塔矢亮;
                    break;
            }
            LeftPhoto.Location = new Point(0, 220);
            LeftPhoto.Size = new Size(158, 189);
            LeftPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
            LeftPhoto.TabStop = false;
            Controls.Add(LeftPhoto);
            LeftPhoto.BringToFront();
            //生成玩家2頭像
            RightPhoto = new PictureBox();
            switch (MyInfo.AllChar[1])
            {
                case CharPhoto.進藤光:
                    RightPhoto.Image = Resources.進藤光;
                    break;
                case CharPhoto.佐為:
                    RightPhoto.Image = Resources.左為;
                    break;
                case CharPhoto.塔矢亮:
                    RightPhoto.Image = Resources.塔矢亮;
                    break;
            }
            RightPhoto.Location = new Point(800, 220);
            RightPhoto.Size = new Size(158, 189);
            RightPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
            RightPhoto.TabStop = false;
            Controls.Add(RightPhoto);
            RightPhoto.BringToFront();
            //生成確認送出按鈕
            ConfirmButton = new Button();
            ConfirmButton.BackColor = System.Drawing.Color.DarkSlateGray;
            ConfirmButton.Cursor = System.Windows.Forms.Cursors.Hand;
            ConfirmButton.Enabled = false;
            ConfirmButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            ConfirmButton.Font = new System.Drawing.Font("微軟正黑體", 20.00F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            ConfirmButton.Location = new System.Drawing.Point(427, 610);
            ConfirmButton.Name = "ConfirmButton";
            ConfirmButton.Size = new System.Drawing.Size(95, 35);
            ConfirmButton.TabIndex = 5;
            ConfirmButton.Text = "確認";
            ConfirmButton.UseVisualStyleBackColor = false;
            ConfirmButton.Click += ConfirmButton_Click;
            Controls.Add(ConfirmButton);
            ConfirmButton.BringToFront();
            //開始遊戲計時
            GameTime = 0;
            GameTmr.Start();
            //開始倒數
            CountDown = 300;
            if (ID == 1)
            {
                CountDownTmr.Start();
            }
            //決定先後順序
            if (ID == 2)
            {
                ChangeTurn(false);
                RcvInfoInGame.RunWorkerAsync();
            }
        }

        private void pixel_Click(object sender, EventArgs e)//下棋後的事件
        {
            if (MyInfo.PosX != -1)
            {
                Pixels[MyInfo.PosX, MyInfo.PosY].Image = null;
                Pixels[MyInfo.PosX, MyInfo.PosY].Click += pixel_Click;
                Pixels[MyInfo.PosX, MyInfo.PosY].Cursor = System.Windows.Forms.Cursors.Hand;
            }
            ConfirmButton.Enabled = true;
            PictureBox Box = sender as PictureBox;
            //取得該Pixel的座標值
            MyInfo.PosY = GetColumnIndex(Box.Location.X);
            MyInfo.PosX = GetRowIndex(Box.Location.Y);
            switch (ID)
            {
                case 1:
                    Box.Image = Resources.黑棋;
                    break;
                case 2:
                    Box.Image = Resources.白棋;
                    break;
            }
            //點過的Pixel不能再點
            Box.Cursor = System.Windows.Forms.Cursors.Default;
            Box.Click -= pixel_Click;
            //判斷自己是否贏
            if (isLine(ID))
            {
                //停止計時
                GameTmr.Stop();
                CountDownTmr.Stop();

                char[] Delimiter = { ' ' };
                string[] gametime = GameTimeLbl.Text.Split(Delimiter);
                MyInfo.GameTime = gametime[1];
                MyInfo.WhoWin = ID;
                MyInfo.Win[ID - 1]++;
                SendPersonalInfo();

                MessageBox.Show(MyInfo.AllName[MyInfo.WhoWin - 1] + "勝利");

                WinView();
            }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)//按下確認送出下棋按鈕
        {
            MyInfo.Board[MyInfo.PosX, MyInfo.PosY] = ID;
            ConfirmButton.Enabled = false;
            SendPersonalInfo();
            //停止倒數
            CountDownTmr.Stop();
            //換對方動作
            ChangeTurn(false);
            //準備接受對方下棋資訊
            RcvInfoInGame.RunWorkerAsync();
        }

        private void ChangeTurn(bool Myturn)//決定是否換自己
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Pixels[i, j].Enabled = Myturn;
                }
            }
        }

        private void WinView()//獲勝畫面
        {
            //隱藏玩家名稱
            LeftName.Dispose();
            RightName.Dispose();
            //隱藏倒數標籤
            CountDownLbl.Dispose();
            //勝利玩家圖像
            WinPctrBox = new PictureBox();
            WinPctrBox.Location = new System.Drawing.Point(355, 136);
            switch (MyInfo.AllChar[MyInfo.WhoWin-1])
            {
                case CharPhoto.進藤光:
                    WinPctrBox.Image = Resources.進藤光;
                    break;
                case CharPhoto.佐為:
                    WinPctrBox.Image = Resources.左為;
                    break;
                case CharPhoto.塔矢亮:
                    WinPctrBox.Image = Resources.塔矢亮;
                    break;
            }
            WinPctrBox.Name = "pictureBox1";
            WinPctrBox.Size = new System.Drawing.Size(261, 334);
            WinPctrBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            WinPctrBox.TabStop = false;
            Controls.Add(WinPctrBox);
            WinPctrBox.BringToFront();
            //勝利玩家名字
            WinLbl = new Label();
            WinLbl.AutoSize = true;
            WinLbl.Font = new System.Drawing.Font("微軟正黑體", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            WinLbl.Location = new System.Drawing.Point(373, 483);
            WinLbl.Name = "label1";
            WinLbl.Size = new System.Drawing.Size(231, 61);
            WinLbl.TabStop = false;
            WinLbl.Text = "獲勝玩家:\n"+MyInfo.AllName[MyInfo.WhoWin-1];
            Controls.Add(WinLbl);
            WinLbl.BringToFront();
            //改變背景
            this.BackgroundImage = null;
            //隱藏玩家頭像
            LeftPhoto.Dispose();
            RightPhoto.Dispose();
            //隱藏Vursus
            Vursus.Dispose();
            //隱藏時間標籤
            GameTimeLbl.Dispose();
            //隱藏所有棋子
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Pixels[i, j].Dispose();
                }
            }
            //隱藏確認按鈕
            ConfirmButton.Dispose();
            //顯示離開按鈕
            LeaveBtn.Visible = true;
            //生成重新開局按鈕
            NewGameBtn = new Button();
            NewGameBtn.BackColor = Color.Moccasin;
            NewGameBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            NewGameBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            NewGameBtn.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            NewGameBtn.ForeColor = Color.Maroon;
            NewGameBtn.Location = new Point(680, 564);
            NewGameBtn.Name = "NewGameBtn";
            NewGameBtn.Size = new System.Drawing.Size(97, 58);
            NewGameBtn.TabIndex = 7;
            NewGameBtn.Text = "再來一局";
            NewGameBtn.UseVisualStyleBackColor = false;
            NewGameBtn.Click += NewGameBtn_Click;
            Controls.Add(NewGameBtn);
            //等待接收對方是否重新開局
            NewGameBckWrkr.RunWorkerAsync();
        }

        private void NewGameBtn_Click(object sender, EventArgs e)
        {
            Loading3();
            MyInfo.NewGame = true;
            SendPersonalInfo();
        }

        private void Loading1()//等待兩人連線至Server
        {
            IPlabel.Dispose();
            NameLbl.Dispose();
            IPTxtBox.Dispose();
            NameTxtBox.Dispose();
            StartBtn.Dispose();
            TitleLabel.Dispose();
            LeaveBtn.Visible = false;
            //生成Loading圖案
            LoadingPctrBox = new PictureBox();
            LoadingPctrBox.BackColor = System.Drawing.Color.Transparent;
            LoadingPctrBox.Image = global::五子棋Client.Properties.Resources.Loading;
            LoadingPctrBox.Location = new System.Drawing.Point(431, 308);
            LoadingPctrBox.Name = "LoadingPctrBox";
            LoadingPctrBox.Size = new System.Drawing.Size(91, 93);
            LoadingPctrBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            LoadingPctrBox.TabStop = false;
            Controls.Add(LoadingPctrBox);
            LoadingPctrBox.BringToFront();
        }

        private void Loading2()//等待兩人選好角色
        {
            foreach (PictureBox Photo in SelectPhoto)
            {
                Photo.Dispose();
            }
            ChooseCharacterLabel.Dispose();
            LoadingPctrBox.Visible = true;
            LeaveBtn.Visible = false;
        }

        private void Loading3()//等待對方重新開始
        {
            WinPctrBox.Dispose();
            WinLbl.Dispose();
            NewGameBtn.Dispose();
            LeaveBtn.Visible = false;
            LoadingPctrBox.Visible = true;
        }

        private int GetRowIndex(int row)//取得X座標
        {
            int Index;
            Index = (row - 45) / 38;
            return Index;
        }

        private int GetColumnIndex(int column)//取得Y座標
        {
            int Index;
            Index = (column - 149) / 43;
            return Index;
        }

        private bool isLine(int chess)//判斷是否分出勝負
        {
            int line = 1;
            //左上右下
            for (int i = 1; i < 5; i++)
            {
                if (MyInfo.PosX - i >= 0 && MyInfo.PosY - i >= 0 && MyInfo.Board[MyInfo.PosX - i, MyInfo.PosY - i] == chess)
                {
                    line++;
                    if (line == 5)
                        return true;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i < 5; i++)
            {
                if (MyInfo.PosX + i <= 14 && MyInfo.PosY + i <= 14 && MyInfo.Board[MyInfo.PosX + i, MyInfo.PosY + i] == chess)
                {
                    line++;
                    if (line == 5)
                        return true;
                }
                else
                {
                    line = 1;
                    break;
                }
            }
            //上下
            for (int i = 1; i < 5; i++)
            {
                if (MyInfo.PosX - i >= 0 && MyInfo.Board[MyInfo.PosX - i, MyInfo.PosY] == chess)
                {
                    line++;
                    if (line == 5)
                        return true;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i < 5; i++)
            {
                if (MyInfo.PosX + i <= 14 && MyInfo.Board[MyInfo.PosX + i, MyInfo.PosY] == chess)
                {
                    line++;
                    if (line == 5)
                        return true;
                }
                else
                {
                    line = 1;
                    break;
                }
            }
            //右上左下
            for (int i = 1; i < 5; i++)
            {
                if (MyInfo.PosX - i >= 0 && MyInfo.PosY + 1 <= 14 && MyInfo.Board[MyInfo.PosX - i, MyInfo.PosY + i] == chess)
                {
                    line++;
                    if (line == 5)
                        return true;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i < 5; i++)
            {
                if (MyInfo.PosX + i <= 14 && MyInfo.PosY - i >= 0 && MyInfo.Board[MyInfo.PosX + i, MyInfo.PosY - i] == chess)
                {
                    line++;
                    if (line == 5)
                        return true;
                }
                else
                {
                    line = 1;
                    break;
                }
            }
            //左右
            for (int i = 1; i < 5; i++)
            {
                if (MyInfo.PosY - i >= 0 && MyInfo.Board[MyInfo.PosX, MyInfo.PosY - i] == chess)
                {
                    line++;
                    if (line == 5)
                        return true;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i < 5; i++)
            {
                if (MyInfo.PosY + i <= 14 && MyInfo.Board[MyInfo.PosX, MyInfo.PosY + i] == chess)
                {
                    line++;
                    if (line == 5)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private void LoadingBeforeSelect_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceiveIDThread();
        }

        private void LoadingBeforeSelect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SelectMode();
        }

        private void RcvAllInfoBeforeGame_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceiveAllInfoBeforeGameThread();
        }

        private void RcvAllInfoBeforeGame_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StartGame();
        }

        private void RcvInfoInGame_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceiveDataInGame();
        }

        private void RcvInfoInGame_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ID == 1)
            {
                if (MyInfo.Leave2)
                {
                    MessageBox.Show("對方玩家中離");
                    this.Dispose();
                }
            }
            else
            {
                if (MyInfo.Leave1)
                {
                    MessageBox.Show("對方玩家中離");
                    this.Dispose();
                }
            }
            //顯示對方所下的棋
            switch(ID)
            {
                case 1:
                    Pixels[MyInfo.PosX, MyInfo.PosY].Image = Resources.白棋;
                    break;
                case 2:
                    Pixels[MyInfo.PosX, MyInfo.PosY].Image = Resources.黑棋;
                    break;
            }
            //判斷對手是否贏
            if (ID == 1)
            {
                if (isLine(2))
                {
                    CountDownTmr.Stop();
                    GameTmr.Stop();
                    MessageBox.Show(MyInfo.AllName[1] + "勝利");
                    WinView();
                    return;
                }
            }
            else
            {
                if (isLine(1))
                {
                    CountDownTmr.Stop();
                    GameTmr.Stop();
                    MessageBox.Show(MyInfo.AllName[0] + "勝利");
                    WinView();
                    return;
                }
            }
            Pixels[MyInfo.PosX, MyInfo.PosY].Cursor = Cursors.Default;
            Pixels[MyInfo.PosX, MyInfo.PosY].Click -= pixel_Click;
            //換自己
            ChangeTurn(true);
            MyInfo.PosX = -1;
            MyInfo.PosY = -1;
            //重新倒數
            CountDown = 300;
            CountDownTmr.Start();
            CountDownLbl.ForeColor = Color.Black;
        }

        private void NewGameBckWrkr_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceiveDataInGame();
        }

        private void NewGameBckWrkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (MyInfo.AllNewGame[0] && MyInfo.AllNewGame[1])
            {
                MyInfo.NewGame = false;
                MyInfo.WhoWin = 0;
                MyInfo.Board = new int[15, 15];
                MyInfo.PosX = -1;
                MyInfo.PosY = -1;
                MyInfo.AllNewGame[0] = false;
                MyInfo.AllNewGame[1] = false;
                StartGame();
            }
            else
            {
                MessageBox.Show("對方玩家已離開遊戲");
                this.Dispose();
                
            }
        }

        private void GameTmr_Tick(object sender, EventArgs e)
        {
            string GameTimeString = "";   
            if (GameTime < 10)
            {
                GameTimeString += "00:0" + GameTime;
            }
            else if (GameTime < 60 && GameTime >= 10)
            {
                GameTimeString += "00:" + GameTime;
            }
            else if (GameTime < 600 && GameTime >= 60)
            {
                if (GameTime % 60 < 10)
                {
                    GameTimeString += "0" + GameTime / 60 + ":0" + GameTime % 60;
                }
                else
                {
                    GameTimeString += "0" + GameTime / 60 + ":" + GameTime % 60;
                }
            }
            else
            {
                if (GameTime % 60 < 10)
                {
                    GameTimeString += GameTime / 60 + ":0" + GameTime % 60;
                }
                else
                {
                    GameTimeString += GameTime / 60 + ":" + GameTime % 60;
                }
            }
            //顯示時間於標籤上
            GameTimeLbl.Text = "遊戲時間: " + GameTimeString;
            //時間加一秒
            GameTime++;
            //時間將到
            if (CountDown < 100 && CountDown != -1)
            {
                if (CountDownLbl.ForeColor == Color.Red)
                {
                    CountDownLbl.ForeColor = Color.Black;
                }
                else
                {
                    CountDownLbl.ForeColor = Color.Red;
                }
            }
            if (CountDown == -1)
            {
                CountDownLbl.ForeColor = Color.Red;
            }
            
        }

        private void CountDownTmr_Tick(object sender, EventArgs e)
        {
            string CountDownString = "";
            CountDownString += CountDown / 10 + "." + CountDown % 10;
            //顯示剩餘秒數於標籤上
            CountDownLbl.Text = "剩餘時間: " + CountDownString;
            //隨機產生一顆棋在棋盤上
            Random Rnd = new Random();
            if (CountDown == 0)
            {
                if (MyInfo.PosX != -1)
                {
                    Pixels[MyInfo.PosX, MyInfo.PosY].Image = null;
                }
                while (true)
                {
                    MyInfo.PosX = Rnd.Next(15);
                    MyInfo.PosY = Rnd.Next(15);
                    if (MyInfo.Board[MyInfo.PosX, MyInfo.PosY] == 0)
                    {
                        break;
                    }
                }
                MyInfo.Board[MyInfo.PosX, MyInfo.PosY] = ID;
                //在棋盤上顯示隨機的棋子
                switch (ID)
                {
                    case 1:
                        Pixels[MyInfo.PosX,MyInfo.PosY].Image = Resources.黑棋;
                        break;
                    case 2:
                        Pixels[MyInfo.PosX, MyInfo.PosY].Image = Resources.白棋;
                        break;
                }
                //點過的Pixel不能再點
                Pixels[MyInfo.PosX, MyInfo.PosY].Cursor = System.Windows.Forms.Cursors.Default;
                Pixels[MyInfo.PosX, MyInfo.PosY].Click -= pixel_Click;
                //判斷自己是否贏
                if (isLine(ID))
                {
                    //停止計時
                    GameTmr.Stop();

                    char[] Delimiter = { ' ' };
                    string[] gametime = GameTimeLbl.Text.Split(Delimiter);
                    MyInfo.GameTime = gametime[1];
                    MyInfo.WhoWin = ID;
                    MyInfo.Win[ID - 1]++;
                    SendPersonalInfo();

                    MessageBox.Show(MyInfo.AllName[MyInfo.WhoWin - 1] + "勝利");
                    //顯示勝利畫面
                    WinView();
                    return;
                }
                //換對方動作
                ChangeTurn(false);
                ConfirmButton.Enabled = false;
                //寄送遊戲資訊
                SendPersonalInfo();
                //停止倒數
                CountDownTmr.Stop();
                //準備接受對方下棋資訊
                RcvInfoInGame.RunWorkerAsync();
            }

            //倒數一單位
            CountDown--;
            
        }

    }
}
