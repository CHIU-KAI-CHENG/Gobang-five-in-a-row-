using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


public enum CharPhoto
{
    進藤光,
    佐為,
    塔矢亮
}
namespace Information
{
    [Serializable]
    public class Packet
    {
        public string[] AllIP;//存放所有人的IP
        public string[] AllName;//存放所有人的名稱
        public CharPhoto[] AllChar;//存放所有人的人物頭像
        public string MyIPAddress;//存放自己的IP
        public string MyName;//存放自己的登入名稱
        public CharPhoto MyChar;//存放自己的人物頭像
        public int PosX;//下棋的X座標位置
        public int PosY;//下棋的Y座標位置
        public int WhoWin;//代表勝負
        public string GameTime;//遊戲時間
        public int[] Win;//兩人分別的總勝場
        public int[,] Board;//棋盤上的資訊
        public bool NewGame;//是否重新開局
        public bool Leave1;//玩家1是否離開
        public bool Leave2;//玩家2是否離開
        public bool[] AllNewGame;

        public Packet()
        {
            PosX = -1;
            PosY = -1;
            NewGame = false;
            AllName = new String[2];
            AllIP = new String[2];
            AllChar = new CharPhoto[2];
            Win = new int[2];
            Board = new int[15,15];
            AllNewGame = new bool[2];
        }

        public Byte[] PacketToBytes()//將Packet轉為Byte[]型態以便傳送
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this);//將本類別序列化至ms
            byte[] Bytes = ms.ToArray();
            ms.Close();
            return Bytes;
        }
        
        public Packet BytesToPacket(Byte[] BytesOfPacket)//將接收到的Byte[]轉為Packet型態以便存取
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(BytesOfPacket);//將BytesOfPacket初始化

            Packet TmpPkt = (Packet)bf.Deserialize(ms);//將ms解序列化至TmpPkt
            ms.Close();
            this.MyIPAddress = TmpPkt.MyIPAddress;
            this.MyName = TmpPkt.MyName;
            this.MyChar = TmpPkt.MyChar;
            this.PosX = TmpPkt.PosX;
            this.PosY = TmpPkt.PosY;
            this.WhoWin = TmpPkt.WhoWin;
            this.GameTime = TmpPkt.GameTime;
            this.NewGame = TmpPkt.NewGame;
            Array.Copy(TmpPkt.Win, this.Win, TmpPkt.Win.Length);
            Array.Copy(TmpPkt.Board, this.Board, TmpPkt.Board.Length);
            return TmpPkt;
        }
    }
}
