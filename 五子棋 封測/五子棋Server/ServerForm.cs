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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Information;

namespace 五子棋Server
{
    public partial class ServerForm : Form
    {
        TcpListener Listener;
        List<TcpClient> Clients;
        List<NetworkStream> ClientStreams;
        Packet AllInfo;
        Thread _NewGameThread;

        public ServerForm()
        {
            InitializeComponent();
            AllInfo = new Packet();//建立遊戲資訊類別
            StartListening();

            Form.CheckForIllegalCrossThreadCalls = false;
        }

        private void StartListening()//接收Client的連接
        {
            Listener = new TcpListener(IPAddress.Any, 5200);
            Listener.Start();//開始偵測Client的連接

            Clients = new List<TcpClient>(2);//開兩個TcpClient的List準備接收兩個Client連接

            AcptRcvBckWrkr.RunWorkerAsync();
            
        }

        private void AcceptClients()//Accept兩個Clients，並送出各自的ID
        {
            ClientStreams = new List<NetworkStream>(2);
            for (int i = 0; i < 2; i++)
            {
                Clients.Add(Listener.AcceptTcpClient());
                ClientStreams.Add(Clients[i].GetStream());
            }
            for (int i = 0; i < 2; i++)
            {
                Byte[] SndBuf = new Byte[Clients[i].SendBufferSize];
                SndBuf = BitConverter.GetBytes(i+1);
                ClientStreams[i].Flush();//清除原本資料流的資料
                ClientStreams[i].Write(SndBuf, 0, SndBuf.Length);
                
            }
        }

        private void ReceivePersonalInfo()//接收兩個Clients的基本資訊
        {
            for (int i = 0; i < 2; i++)
            {
                Byte[] RcvBuf = new Byte[Clients[i].ReceiveBufferSize];
                
                ClientStreams[i].Read(RcvBuf, 0, RcvBuf.Length);
                AllInfo.BytesToPacket(RcvBuf);
                AllInfo.AllIP[i] = AllInfo.MyIPAddress;
                AllInfo.AllName[i] = AllInfo.MyName;
            }
        }

        private void ReceivePhoto()//接收兩個Client所選的頭像
        {
            for (int i = 0; i < 2; i++)
            {
                Byte[] RcvBuf = new Byte[Clients[i].ReceiveBufferSize];

                ClientStreams[i].Read(RcvBuf, 0, RcvBuf.Length);
                AllInfo.BytesToPacket(RcvBuf);
                AllInfo.AllChar[i] = AllInfo.MyChar;
            }
        }

        private void SendAllInfo()//寄送所有遊戲前資訊給兩位玩家
        {
            for (int i = 0; i < 2; i++)
            {
                Byte[] SndBuf = new Byte[Clients[i].SendBufferSize];
                SndBuf = AllInfo.PacketToBytes();
                ClientStreams[i].Write(SndBuf, 0, SndBuf.Length);
            }
        }

        private void RcvSndInfoInGame()//接收並寄送遊戲資訊
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    Byte[] RcvBuf = new Byte[Clients[i].ReceiveBufferSize];
                    ClientStreams[i].Read(RcvBuf, 0, RcvBuf.Length);
                    AllInfo.BytesToPacket(RcvBuf);
                }
                catch
                {
                    if (i == 0)
                    {
                        AllInfo.Leave1 = true;
                    }
                    else
                    {
                        AllInfo.Leave2 = true;
                    }
                }
                if (i == 0)
                {
                    try
                    {
                        Byte[] SndBuf = new Byte[Clients[1].SendBufferSize];
                        SndBuf = AllInfo.PacketToBytes();
                        ClientStreams[1].Write(SndBuf, 0, SndBuf.Length);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        Byte[] SndBuf = new Byte[Clients[0].SendBufferSize];
                        SndBuf = AllInfo.PacketToBytes();
                        ClientStreams[0].Write(SndBuf, 0, SndBuf.Length);
                    }
                    catch
                    {
                    }                    
                    i = -1;
                }
                //若勝利則跳出傳送與接收
                if (AllInfo.WhoWin != 0)
                {
                    break;
                }
              
                
            }     
        }

        private void NewGameThread()
        {
            for (int i = 0; i < 2; i++)
            {
                //接收重新開局資訊
                Packet TmpPkt;
                try
                {
                    Byte[] RcvBuf = new Byte[Clients[i].ReceiveBufferSize];
                    ClientStreams[i].Read(RcvBuf, 0, RcvBuf.Length);
                    TmpPkt = AllInfo.BytesToPacket(RcvBuf);
                    AllInfo.AllNewGame[i] = AllInfo.NewGame;
                }
                catch
                {
                    MessageBox.Show("玩家:" + AllInfo.AllName[i] + "已離開遊戲(IP:" + AllInfo.AllIP[i] + ")");
                    continue;
                }
                
            }
            for(int i = 0 ; i < 2; i++)
            {
                //寄送重新開局資訊
                if (i == 0)
                {
                    try
                    {
                        Byte[] SndBuf = new Byte[Clients[1].SendBufferSize];
                        SndBuf = AllInfo.PacketToBytes();
                        ClientStreams[1].Write(SndBuf, 0, SndBuf.Length);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else if(i==1)
                {
                    try{
                        Byte[] SndBuf = new Byte[Clients[0].SendBufferSize];
                        SndBuf = AllInfo.PacketToBytes();
                        ClientStreams[0].Write(SndBuf, 0, SndBuf.Length);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            if (AllInfo.AllNewGame[0] && AllInfo.AllNewGame[1])
            {
                RcvSndInfoInGameBckWrkr.RunWorkerAsync();
            }
        }

        private void CreateDataGrid()//將初始玩家資料寫入dataGridView
        {         
            int row = dataGridView1.Rows.Add();
               
            dataGridView1.Rows[row].Cells[0].Value = AllInfo.AllName[0];
            dataGridView1.Rows[row].Cells[1].Value = AllInfo.AllIP[0];
            dataGridView1.Rows[row].Cells[3].Value = AllInfo.Win[0];

            row = dataGridView1.Rows.Add();

            dataGridView1.Rows[row].Cells[0].Value = AllInfo.AllName[1];
            dataGridView1.Rows[row].Cells[1].Value = AllInfo.AllIP[1];
            dataGridView1.Rows[row].Cells[3].Value = AllInfo.Win[1];
        }

        private void FillDataGrid()//將玩家資料寫入dataGridView
        {           
            dataGridView1.Rows[0].Cells[2].Value = AllInfo.AllChar[0];
            dataGridView1.Rows[0].Cells[3].Value = AllInfo.Win[0];

            dataGridView1.Rows[1].Cells[2].Value = AllInfo.AllChar[1];
            dataGridView1.Rows[1].Cells[3].Value = AllInfo.Win[1];
        }

        private void AddResult()//將對戰結果寫入dataGridView
        {
            int row = dataGridView2.Rows.Add();

            dataGridView2.Rows[row].Cells[0].Value = row + 1;
            dataGridView2.Rows[row].Cells[1].Value = AllInfo.GameTime;
            dataGridView2.Rows[row].Cells[2].Value = AllInfo.AllName[AllInfo.WhoWin - 1];
        }

        private void AcptRcvBckWrkr_DoWork(object sender, DoWorkEventArgs e)//接收Client連線並接收個人初始資訊
        {
            AcceptClients();
            ReceivePersonalInfo();
        }

        private void AcptRcvBckWrkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CreateDataGrid();
            RcvPhotoBckWrkr.RunWorkerAsync();//準備接收頭像資訊
        }

        private void RcvPhotoBckWrkr_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceivePhoto();
        }

        private void RcvPhotoBckWrkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FillDataGrid();
            SendAllInfo();
            RcvSndInfoInGameBckWrkr.RunWorkerAsync();
        }

        private void RcvSndInfoInGameBckWrkr_DoWork(object sender, DoWorkEventArgs e)
        {
            RcvSndInfoInGame();
        }

        private void RcvSndInfoInGameBckWrkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FillDataGrid();
            AddResult();
            _NewGameThread = new Thread(NewGameThread);
            _NewGameThread.Start();
        }
    }
}
