using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.IO;
using System.Windows;
using Microsoft.Kinect.Tools;
using System.Threading;

namespace XieHeProject
{


    class PhoneConnection
    {
        /// <summary>
        /// 抓窗体用
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
        private static extern void SetForegroundWindow(IntPtr hwnd);

         /// <summary>
        /// 服务器收到的指令
        /// </summary>
        private static int _command = 0;

        /// <summary>
        /// 从客服端收到信息
        /// </summary>
        private static string _data = "";

        /// <summary>
        /// 判断连接是否断开
        /// </summary>
        private static bool _isReceivingMsg;

        public static bool IsReceivingMsg
        {
            get { return PhoneConnection._isReceivingMsg; }
            set { PhoneConnection._isReceivingMsg = value; }
        }

        /// <summary>
        /// 接收消息的六个类型
        /// </summary>
        private enum messsageType
        {
            FIRST=101,
            SECOND=102,
            THIRD=103,
            START_RECORD=111,
            STOP_RECORD=112,
            PATIENT_ID=200
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        public static void InitConnection()
        {
            int HOST_PORT = 8003;
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(HOST_PORT);
                listener.Start();
            }
            catch 
            {
                listener.Stop();
                return;
            }
            ReceiveCommand(listener);
        }

        /// <summary>
        /// 得到窗体接收指令
        /// </summary>
        /// <param name="command">指令</param>
        private static void GetFormSendCommand(string command)
        {
            IntPtr hwndCalc = FindWindow(null, "步态采集");
            SetForegroundWindow(hwndCalc);
            System.Windows.Forms.SendKeys.SendWait(command);
        }

        /// <summary>
        /// 接收指令
        /// </summary>
        /// <param name="listener">建立的监听器</param>
        private static void ReceiveCommand(TcpListener listener)
        {
       
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Connection accepted.");
            
            NetworkStream ns = client.GetStream();
            _isReceivingMsg = true;

            while (true == _isReceivingMsg)
            {
               try
                {
                    byte[] bytes = new byte[1024];
                    int bytesRead = ns.Read(bytes, 0, bytes.Length);
                    String str = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                   
                     //解析字符串，翻译成信息
                    String[] s = str.Split(new char[] { ':' });
                    _command = int.Parse(s[0]);
                    _data = s[1].Substring(0, s[1].Length - 1);

                    //抓住窗口，发送键盘指令，然后进行相应的处理
                    GetFormSendCommand("^3");
                }
                catch 
                {
                    _isReceivingMsg = false;
                    MessageBox.Show("连接已断开!");
                    //MessageBox.Show(GetWPFWindows.Mwindow, "连接已断开!");
                }
            }
            ns.Close();
            client.Close();
            listener.Stop();
        }

        
        /// <summary>
        /// 处理指令
        /// </summary>
        public static void dealCommand()
        {
            switch (_command)
            {
                    //开始录像
                case (int)messsageType.START_RECORD:
                    {
                        string[] files = Directory.GetFiles(BingRen.PeopleDir);
                        GetFormSendCommand("^1");
                        AddPeopleLog(BingRen.CurrentProject+"开始");
                        //startRecord();
                    }
                    break;
                    //结束录像
                case (int)messsageType.STOP_RECORD:
                    {
                        GetFormSendCommand("^2");
                        AddPeopleLog(BingRen.CurrentProject + "结束");
                        //stopRecord();
                    }
                    break;
                    //第一项打分
                case (int)messsageType.FIRST:
                    {
                       AddPointLog("构音不良 "+_data);
                    }
                    break;
                    //第二项打分
                case (int)messsageType.SECOND:
                    {
                       AddPointLog("僵硬 " + _data);
                    }
                    break;
                    //第三项左打分
                case (int)messsageType.THIRD:
                    {
                        AddPointLog("跟膝胫实验 " + _data);
                    }

                    break;
              
                    //建立病人文件夹
                case (int)messsageType.PATIENT_ID:
                {
                   BingRen.PeopleId= _data;
                   CreatePeopleDir();
                }
               
                break;
            }
        }


        /// <summary>
        /// 设定存储根目录
        /// </summary>
        public static void SetStoreDir()
        {
            System.Windows.Forms.FolderBrowserDialog df = new System.Windows.Forms.FolderBrowserDialog();

            //设置文件浏览对话框上的描述内容   
            df.Description = "选择所有xef文件保存的根目录地址";

            //不显示对话框下方的创建新文件夹按钮   
            df.ShowNewFolderButton = true;

         
            //显示文件夹对话框，并返回对话框处理结果数值   
            System.Windows.Forms.DialogResult result = df.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) //另外一种判断办法 if (df.ShowDialog(this) == DialogResult.OK)   
            {
                //将中的数据库目录地址赋于类全局变量数据库根目录   
                string folderPath = df.SelectedPath;
                if (folderPath != "")
                {
                    BingRen.RootDir = folderPath;
                }
            }
        }

        /// <summary>
        /// 创立病人文件夹
        /// </summary>
        public static void CreatePeopleDir()
        {
            string dir = BingRen.RootDir;
            if (BingRen.PeopleId != "")
            {
                dir += "\\" + BingRen.PeopleId.ToString();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                BingRen.PeopleDir = dir;
                AddPeopleLog("建立目录：" + BingRen.PeopleDir);
            }
            else
            {   
                //MessageBox.Show(GetWPFWindows.Mwindow, "病人ID为空，请重新扫码！" + dir);
            }
        }

        /// <summary>
        /// 添加病人日志
        /// </summary>
        /// <param name="content">添加内容</param>
        private static void AddPeopleLog(string content)
        {
            if (Directory.Exists(BingRen.PeopleDir))
            {
                TxtWrite(content, BingRen.PeopleDir + "\\log.txt");
            }
            else
            {
                 //MessageBox.Show(GetWPFWindows.Mwindow, "病人目录为空,请在手机端重新输入病人号");
            }
        }

       /// <summary>
       /// 添加病人前三项评分
       /// </summary>
       /// <param name="content"></param>
        private static void AddPointLog(string content)
        {
            if (Directory.Exists(BingRen.PeopleDir))
            {
                TxtWrite(content, BingRen.PeopleDir + "\\point.txt");
            }
            else
            {
                //MessageBox.Show(GetWPFWindows.Mwindow, "病人目录为空,请在手机端重新输入病人号");
            }
        }

         /// <summary>
        /// 生成log文件
        /// </summary>
        /// <param name="strToWrite">文件内容</param>
        /// <param name="filename">文件路径</param>
        private static void TxtWrite(string strToWrite, string filename)
        {
            DateTime now = DateTime.Now;
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(now.ToString() + ":" + now.ToString("fff") + ": " + strToWrite);
                sw.Close();
            }
            catch 
            {
                //MessageBox.Show(GetWPFWindows.Mwindow, "文件异常");
            }
        }

      

        
    }
}
