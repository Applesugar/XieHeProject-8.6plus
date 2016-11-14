using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MSWord = Microsoft.Office.Interop.Word;
using System.Reflection;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.Util;
using NPOI.SS.UserModel;
using System.Threading;
using NPOI.POIFS.FileSystem;
using System.Timers;
using System.Globalization;

namespace XieHePlayer
{
    public partial class Form1 : Form
    {

        //String Path = "";
        //String FileName = "";

        //默认保存位置
        String savePath = "../../../../总表.xls";

        //每个人的切分文件保存位置
        String QieFenFsavePath = "";

        //视频文件名
        String FileName = "";

        //自适应窗体类
        AutoSizeFormClass asc = new AutoSizeFormClass();

        //倒计时使用的计数
        int timeCount = 0;
        int timeCount1 = 0;

        //各项表总分
        int SARAzfI = 0;
        int SPPBzfI = 0;
        int TBTzfI = 0;
        int TPHzfI = 0;
        int UPDRSzfI = 0;

        //SPPB表计时用
        double StartTime = 0;
        double EndTime = 0;
        double blzlST=0;
        double blzlET = 0;
        double blzlUT = 0;

        double bclzlST = 0;
        double bclzlET = 0;
        double bclzlUT = 0;

        double clzlST = 0;
        double clzlET = 0;
        double clzlUT = 0;

        double xzsdST = 0;
        double xzsdET = 0;
        double xzsdUT = 0;

        double cfqlST = 0;
        double cfqlET = 0;
        double cfqlUT = 0;

        //切分记录用时
        double QFzzST = 0;
        double QFzzET = 0;
        double QFzzUT = 0;

        double QFszzzLST = 0;
        double QFszzzLET = 0;
        double QFszzzLUT = 0;

        double QFszzzRST = 0;
        double QFszzzRET = 0;
        double QFszzzRUT = 0;

        double QFzbsyLST = 0;
        double QFzbsyLET = 0;
        double QFzbsyLUT = 0;

        double QFzbsyRST = 0;
        double QFzbsyRET = 0;
        double QFzbsyRUT = 0;

        double QFksltLST = 0;
        double QFksltLET = 0;
        double QFksltLUT = 0;

        double QFksltRST = 0;
        double QFksltRET = 0;
        double QFksltRUT = 0;

        double QFsznhLST = 0;
        double QFsznhLET = 0;
        double QFsznhLUT = 0;

        double QFsznhRST = 0;
        double QFsznhRET = 0;
        double QFsznhRUT = 0;

        double QFsbydLST = 0;
        double QFsbydLET = 0;
        double QFsbydLUT = 0;

        double QFsbydRST = 0;
        double QFsbydRET = 0;
        double QFsbydRUT = 0;

        double QFssltLST = 0;
        double QFssltLET = 0;
        double QFssltLUT = 0;

        double QFssltRST = 0;
        double QFssltRET = 0;
        double QFssltRUT = 0;

        double QFxzlhLST = 0;
        double QFxzlhLET = 0;
        double QFxzlhLUT = 0;

        double QFxzlhRST = 0;
        double QFxzlhRET = 0;
        double QFxzlhRUT = 0;

        double QFzrqzST = 0;
        double QFzrqzET = 0;
        double QFzrqzUT = 0;

        double QFbxqqlST = 0;
        double QFbxqqlET = 0;
        double QFbxqqlUT = 0;

        double QFcfqlST = 0;
        double QFcfqlET = 0;
        double QFcfqlUT = 0;

        double QFzrzzST = 0;
        double QFzrzzET = 0;
        double QFzrzzUT = 0;

        double QFblzlST = 0;
        double QFblzlET = 0;
        double QFblzlUT = 0;

        double QFbmzlST = 0;
        double QFbmzlET = 0;
        double QFbmzlUT = 0;

        double QFbclzlST = 0;
        double QFbclzlET = 0;
        double QFbclzlUT = 0;

        double QFclzlST = 0;
        double QFclzlET = 0;
        double QFclzlUT = 0;

        double QFyzbxzST = 0;
        double QFyzbxzET = 0;
        double QFyzbxzUT = 0;

        double QFhlsyST = 0;
        double QFhlsyET = 0;
        double QFhlsyUT = 0;

        double QFzsST = 0;
        double QFzsET = 0;
        double QFzsUT = 0;

        double QFsmbxST = 0;
        double QFsmbxET = 0;
        double QFsmbxUT = 0;

        double QFsmbxfST = 0;
        double QFsmbxfET = 0;
        double QFsmbxfUT = 0;

        //四个表
        bool Biao1 = false;
        bool Biao2 = false;
        bool Biao31 = false;
        bool Biao32 = false;
        bool Biao4 = false;


        public Form1()
        {
            InitializeComponent();
        }

        //载入Avi文件
        private void SelectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Avi录像文件(*.avi)|*.avi";
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                //选择播放文件
                axWindowsMediaPlayer1.URL = openFile.FileName;
                axWindowsMediaPlayer1.Ctlcontrols.play();
                //Console.WriteLine(openFile.FileName);
              
                //对文件名径解析，得到文件夹路径
                String[] s = openFile.FileName.Split(new char[] { '\\' });
                String path = "";
                for (int i = 0; i < s.Length-1; i++)
                {
                    path += s[i]+"/";
                }
               
                FileName = s[s.Length - 1];

                
                try
                {
                    String[] s1 = FileName.Split(new char[] { '_' });
                    if (s1.Length > 1)
                        BRHtextBox.Text = s1[0];
                    else
                        BRHtextBox.Text = "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //单独切分文件保存路径
                QieFenFsavePath = path;
                
                //切换视频时清理界面和数据
                //clearUI();
                //clearData();
            }
        }

        //生成报告
        private void PdfButton_Click(object sender, EventArgs e)
        {
            //判断切分表是否填完
            bool flag1

             = (QFzzUT == 0 || QFszzzLUT == 0 || QFszzzRUT == 0 || QFksltLUT == 0 || QFksltRUT == 0 ||
                QFzbsyLUT == 0 || QFzbsyRUT == 0 || QFsznhLUT == 0 || QFsznhRUT == 0 || QFsbydLUT == 0 ||
                QFsbydRUT == 0 || QFssltLUT == 0 || QFssltRUT == 0 || QFxzlhLUT == 0 || QFxzlhRUT == 0 || QFzrqzUT == 0 ||
                QFbxqqlUT == 0 || QFcfqlUT == 0 || QFzrzzUT == 0 || QFblzlUT == 0 ||
                QFbmzlUT == 0 || QFbclzlUT == 0 || QFclzlUT == 0 || QFyzbxzUT == 0 ||
                QFhlsyUT == 0||QFzsUT==0||QFsmbxUT==0||QFsmbxfUT==0);

            if (flag1)
            {
                MessageBox.Show("切分未完成,请继续切分");
                return;
            }

            //判断病人号和评分医生是否填写
            bool flag2

             = (BRHtextBox.Text == "" || PFYStextBox.Text=="");
            if (flag2)
            {
                MessageBox.Show("病人号和评分医生未填写");
                return;
            }

            //判断四个表是否填完
            bool flag3
             = (!Biao1 || !Biao2 || !Biao31 || !Biao32 || !Biao4);
            if (flag3)
            {
                MessageBox.Show("四项评分表未填写");
                return;
            }

            
            try
            {
                //总的评分表
                if (!File.Exists(savePath))
                {
                    //不存在创建新的
                    createExcel();

                }
                //在存在的后面添加一行
                addExcel();

                //总的切分txt
                if (!File.Exists("../../../../切分时间.txt"))
                {
                    //不存在创建新的
                    CreateTxtWrite();
                }
                //在存在的后面添加一行
                AddTxtWrite();

                //分的切分txt，如果存在，直接覆盖
                AddPersonalTxtWrite(QieFenFsavePath + "/" + BRHtextBox.Text+".txt");

                MessageBox.Show("报告已生成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
           
        }

        private void SARAzf_Click(object sender, EventArgs e)
        {
            if (!(SARAbtcomboBox.SelectedIndex == -1 ||SARAzzcomboBox.SelectedIndex == -1||SARAzuzcomboBox.SelectedIndex == -1 ||
               SARAgyblcomboBox.SelectedIndex == -1 || SARAszzzcomboBoxL.SelectedIndex == -1 ||SARAszzzcomboBoxR.SelectedIndex == -1
                || SARAzbcomboBoxL.SelectedIndex == -1 || SARAzbcomboBoxR.SelectedIndex == -1 || SARAksltcomboBoxL.SelectedIndex == -1
                || SARAksltcomboBoxR.SelectedIndex == -1 || SARAgxjcomboBoxL.SelectedIndex == -1 || SARAgxjcomboBoxR.SelectedIndex == -1
                ))
            {
                int total = SARAbtcomboBox.SelectedIndex + SARAzzcomboBox.SelectedIndex + SARAzuzcomboBox.SelectedIndex
               + SARAgyblcomboBox.SelectedIndex + SARAszzzcomboBoxL.SelectedIndex + SARAszzzcomboBoxR.SelectedIndex
               + SARAzbcomboBoxL.SelectedIndex + SARAzbcomboBoxR.SelectedIndex + SARAksltcomboBoxL.SelectedIndex
               + SARAksltcomboBoxR.SelectedIndex + SARAgxjcomboBoxL.SelectedIndex + SARAgxjcomboBoxR.SelectedIndex;
                SARAzf.Text = "SARA总分: " + total;
                SARAzfI = total;
                Biao1 = true;
            }
            else
            {
                MessageBox.Show("SARA还有未评分的项目");
            }
           
        }

        //判断SPPB表是否填完
        private void SPPBzflabel_Click(object sender, EventArgs e)
        {
            if (!(SPPBblzlcomboBox.SelectedIndex == -1 || SPPBbblzlcomboBox.SelectedIndex == -1 || SPPBclzlcomboBox.SelectedIndex == -1
                || SPPBxzsdcomboBox.SelectedIndex == -1 || SPPBcfzwqlcomboBox.SelectedIndex == -1 || blzlUT == 0 || bclzlUT == 0 || clzlUT == 0 || xzsdUT == 0 || cfqlUT == 0))
            {
                int total = SPPBblzlcomboBox.SelectedIndex + SPPBbblzlcomboBox.SelectedIndex + SPPBclzlcomboBox.SelectedIndex +
                SPPBxzsdcomboBox.SelectedIndex + SPPBcfzwqlcomboBox.SelectedIndex;
                SPPBzflabel.Text = "SPPB总分: " + total;
                SPPBzfI = total;
                Biao2 = true;
            }
            else
            {
                MessageBox.Show("SPPB还有未评分的项目");
            }
            
        }


        //每项可以计时的项目3或4个分项(以下类似)
        //保存开始时间并显示
        private void SPPBblzlSlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            blzlST = Math.Round(StartTime, 2);
            SPPBblzlSlabel.Text = "开始时间:" + blzlST;
        }

        //保存结束时间和用时，并显示
        private void SPPBblzjElabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            blzlET = Math.Round(EndTime, 2);
            SPPBblzlElabel.Text = "结束时间:" + blzlET;


            blzlUT = Math.Round(blzlET - blzlST,2);
            SPPBblzlYlabel.Text = "用时:" + blzlUT;
        }

        private void SPPBblzlYlabel_Click(object sender, EventArgs e)
        {
            
        }

        
        private void SPPBbblzlSlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            bclzlST = Math.Round(StartTime, 2);
            SPPBbblzlSlabel.Text = "开始时间:" + bclzlST;
            
        }

        private void SPPBbblzlElabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            bclzlET =  Math.Round(EndTime, 2);
            SPPBbblzlElabel.Text = "结束时间:" + bclzlET;


            bclzlUT = Math.Round(bclzlET - bclzlST,2);
            SPPBbblzlYlabel.Text = "用时:" + bclzlUT;
        }

        private void SPPBbblzlYlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void SPPBclzlSlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            clzlST =  Math.Round(StartTime, 2);
            SPPBclzlSlabel.Text = "开始时间:" + clzlST;
        }

        private void SPPBclzlElabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            clzlET =  Math.Round(EndTime, 2);
            SPPBclzlElabel.Text = "结束时间:" + clzlET;


            clzlUT = Math.Round(clzlET - clzlST,2);
            SPPBclzlYlabel.Text = "用时:" + clzlUT;
        }

        private void SPPBclzlYlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void SPPBxzsdSlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            xzsdST =  Math.Round(StartTime, 2);
            SPPBxzsdSlabel.Text = "开始时间:" + xzsdST;
        }

        private void SPPBxzsdElabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            xzsdET = Math.Round(EndTime, 2);
            SPPBxzsdElabel.Text = "结束时间:" + xzsdET;


            xzsdUT = Math.Round(xzsdET - xzsdST,2);
            SPPBxzsdYlabel.Text = "用时:" + xzsdUT;
        }

        private void SPPBxzsdYlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void SPPBcfzwqlSlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            cfqlST = Math.Round(StartTime, 2);
            SPPBcfzwqlSlabel.Text = "开始时间:" + cfqlST;
        }

        private void SPPBcfzwqlElabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            cfqlET = Math.Round(EndTime, 2);
            SPPBcfzwqlElabel.Text = "结束时间:" + cfqlET;


            cfqlUT = Math.Round(cfqlET - cfqlST,2);
            SPPBcfzwqlYlabel.Text = "用时:" + cfqlUT;
        }

        private void SPPBcfzwqlYlabel_Click(object sender, EventArgs e)
        {
           
        }

        //判断Tinetti表步态是否填完
        private void Tbtzflabel_Click(object sender, EventArgs e)
        {
            if (!(TqbcomboBox.SelectedIndex ==-1||TtjgdcomboBoxL.SelectedIndex==-1|| TtjgdcomboBoxR.SelectedIndex==-1||
               TbccomboBoxL.SelectedIndex ==-1|| TbccomboBoxR.SelectedIndex ==-1||  TbtdcxcomboBox.SelectedIndex==-1||
               TbflxxcomboBox.SelectedIndex ==-1|| TljcomboBox.SelectedIndex ==-1|| TqgwdcomboBox.SelectedIndex==-1||
               TbkcomboBox.SelectedIndex ==-1|| TxzzzscomboBox.SelectedIndex==-1))
            {
                int total = TqbcomboBox.SelectedIndex + TtjgdcomboBoxL.SelectedIndex + TtjgdcomboBoxR.SelectedIndex
               + TbccomboBoxL.SelectedIndex + TbccomboBoxR.SelectedIndex + TbtdcxcomboBox.SelectedIndex
               + TbflxxcomboBox.SelectedIndex + TljcomboBox.SelectedIndex + TqgwdcomboBox.SelectedIndex
               + TbkcomboBox.SelectedIndex + TxzzzscomboBox.SelectedIndex;
                Tbtzflabel.Text = "步态总分: " + total;
                TBTzfI = total;
                Biao31 = true;
            }
            else
            {
                MessageBox.Show("步态还有未评分的项目");
            }
            
        }

        //判断Tinetti表平衡是否填完
        private void Tphzflabel_Click(object sender, EventArgs e)
        {
            if (!(TzwphcomboBox.SelectedIndex==-1|| TzqcomboBox.SelectedIndex ==-1||TjkzlpxcomboBox.SelectedIndex==-1||
                TzlphcomboBox.SelectedIndex ==-1|| TbmzlcomboBox.SelectedIndex ==-1|| TqtsycomboBox.SelectedIndex==-1||
                TzscomboBox.SelectedIndex ==-1|| TzxcomboBox.SelectedIndex==-1))
            {
                int total = TzwphcomboBox.SelectedIndex + TzqcomboBox.SelectedIndex + TjkzlpxcomboBox.SelectedIndex
                + TzlphcomboBox.SelectedIndex + TbmzlcomboBox.SelectedIndex + TqtsycomboBox.SelectedIndex
                + TzscomboBox.SelectedIndex + TzxcomboBox.SelectedIndex;
                Tphzflabel.Text = "平衡总分: " + total;
                TPHzfI = total;
                Biao32 = true;
            }
            else
            {
                MessageBox.Show("平衡还有未评分的项目");
            }
        }

        //判断UPDRS表是否填完
        private void UPDRSzflabel_Click(object sender, EventArgs e)
        {
            if(!(UyycomboBox.SelectedIndex==-1|| UmbbqcomboBox.SelectedIndex ==-1|| UjzxzccomboBox.SelectedIndex
                ==-1|| UdzxzccomboBoxL.SelectedIndex ==-1|| UdzxzccomboBoxR.SelectedIndex ==-1|| UjycomboBoxL.SelectedIndex
                ==-1|| UjycomboBoxR.SelectedIndex ==-1|| UsznhcomboBoxL.SelectedIndex ==-1|| UsznhcomboBoxR.SelectedIndex
                ==-1 ||UsbydcomboBoxL.SelectedIndex ==-1|| UsbydcomboBoxR.SelectedIndex ==-1|| UssltcomboBoxL.SelectedIndex
                ==-1||UssltcomboBoxR.SelectedIndex ==-1||UxzlhdcomboBoxL.SelectedIndex ==-1||UxzlhdcomboBoxR.SelectedIndex
                ==-1|| UzyqlcomboBox.SelectedIndex ==-1||UzscomboBox.SelectedIndex ==-1|| UbtcomboBox.SelectedIndex
                == -1 || UztphcomboBox.SelectedIndex == -1 || UchhjscomboBox.SelectedIndex==-1))
            {
                int total = UyycomboBox.SelectedIndex + UmbbqcomboBox.SelectedIndex + UjzxzccomboBox.SelectedIndex
                + UdzxzccomboBoxL.SelectedIndex + UdzxzccomboBoxR.SelectedIndex + UjycomboBoxL.SelectedIndex
                + UjycomboBoxR.SelectedIndex + UsznhcomboBoxL.SelectedIndex + UsznhcomboBoxR.SelectedIndex
                + UsbydcomboBoxL.SelectedIndex + UsbydcomboBoxR.SelectedIndex + UssltcomboBoxL.SelectedIndex
                + UssltcomboBoxR.SelectedIndex + UxzlhdcomboBoxL.SelectedIndex + UxzlhdcomboBoxR.SelectedIndex
                + UzyqlcomboBox.SelectedIndex + UzscomboBox.SelectedIndex + UbtcomboBox.SelectedIndex
                + UztphcomboBox.SelectedIndex + UchhjscomboBox.SelectedIndex;
                UPDRSzflabel.Text = "UPDRS总分: " + total;
                UPDRSzfI = total;
                Biao4 = true;
            }
            else
            {
                MessageBox.Show("UPDRS还有未评分的项目");
            }
            
        }

        //创建总表的表头
        public void createExcel()
        {

            FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            HSSFWorkbook wk = new HSSFWorkbook();
            ISheet tb = wk.CreateSheet("Sheet01");
            IRow row = tb.CreateRow(0);
            row.CreateCell(0).SetCellValue("病人号");
            row.CreateCell(1).SetCellValue("SARA");
            row.CreateCell(2).SetCellValue("步态");
            row.CreateCell(3).SetCellValue("站姿");
            row.CreateCell(4).SetCellValue("坐姿");
            row.CreateCell(5).SetCellValue("构音不良");
            row.CreateCell(6).SetCellValue("手指追踪试验(左)");
            row.CreateCell(7).SetCellValue("手指追踪试验(右)");
            row.CreateCell(8).SetCellValue("指鼻试验(左)");
            row.CreateCell(9).SetCellValue("指鼻试验(右)");
            row.CreateCell(10).SetCellValue("快速轮替试验(左)");
            row.CreateCell(11).SetCellValue("快速轮替试验(右)");
            row.CreateCell(12).SetCellValue("跟膝胫试验(左)");
            row.CreateCell(13).SetCellValue("跟膝胫试验(右)");
            row.CreateCell(14).SetCellValue("SARA总分");
            row.CreateCell(15).SetCellValue("SPPB");
            row.CreateCell(16).SetCellValue("并联站立");
            row.CreateCell(17).SetCellValue("并联站立用时");
            row.CreateCell(18).SetCellValue("半串联站立");
            row.CreateCell(19).SetCellValue("半串联站立用时");
            row.CreateCell(20).SetCellValue("串联站立");
            row.CreateCell(21).SetCellValue("串联站立用时");
            row.CreateCell(22).SetCellValue("行走速度检查");
            row.CreateCell(23).SetCellValue("行走速度检查用时");
            row.CreateCell(24).SetCellValue("重复坐位起立");
            row.CreateCell(25).SetCellValue("重复坐位起立用时");
            row.CreateCell(26).SetCellValue("SPPB总分");
            row.CreateCell(27).SetCellValue("Tinetti");
            row.CreateCell(28).SetCellValue("起步");
            row.CreateCell(29).SetCellValue("抬脚高度(左)");
            row.CreateCell(30).SetCellValue("抬脚高度(右)");
            row.CreateCell(31).SetCellValue("步长(左)");
            row.CreateCell(32).SetCellValue("步长(右)");
            row.CreateCell(33).SetCellValue("步态对称性");
            row.CreateCell(34).SetCellValue("步伐连续性");
            row.CreateCell(35).SetCellValue("路径");
            row.CreateCell(36).SetCellValue("躯干稳定");
            row.CreateCell(37).SetCellValue("步宽");
            row.CreateCell(38).SetCellValue("行走中转身");
            row.CreateCell(39).SetCellValue("步态总分");
            row.CreateCell(40).SetCellValue("坐位平衡");
            row.CreateCell(41).SetCellValue("站起");
            row.CreateCell(42).SetCellValue("即刻站立平衡");
            row.CreateCell(43).SetCellValue("站立平衡");
            row.CreateCell(44).SetCellValue("闭目站立");
            row.CreateCell(45).SetCellValue("轻推试验");
            row.CreateCell(46).SetCellValue("转身360度");
            row.CreateCell(47).SetCellValue("坐下");
            row.CreateCell(48).SetCellValue("平衡总分");
            row.CreateCell(49).SetCellValue("UPDRS");
            row.CreateCell(50).SetCellValue("言语");
            row.CreateCell(51).SetCellValue("面部表情");
            row.CreateCell(52).SetCellValue("静止性震颤");
            row.CreateCell(53).SetCellValue("双手动作性或位置性震颤(左)");
            row.CreateCell(54).SetCellValue("双手动作性或位置性震颤(右)");
            row.CreateCell(55).SetCellValue("僵硬(左)");
            row.CreateCell(56).SetCellValue("僵硬(右)");
            row.CreateCell(57).SetCellValue("手指捏合(左)");
            row.CreateCell(58).SetCellValue("手指捏合(右)");
            row.CreateCell(59).SetCellValue("手部运动(左)");
            row.CreateCell(60).SetCellValue("手部运动(右)");
            row.CreateCell(61).SetCellValue("双手快速轮替动作(左)");
            row.CreateCell(62).SetCellValue("双手快速轮替动作(右)");
            row.CreateCell(63).SetCellValue("下肢灵活度(左)");
            row.CreateCell(64).SetCellValue("下肢灵活度(右)");
            row.CreateCell(65).SetCellValue("坐椅起立");
            row.CreateCell(66).SetCellValue("姿势");
            row.CreateCell(67).SetCellValue("步态");
            row.CreateCell(68).SetCellValue("姿势平衡");
            row.CreateCell(69).SetCellValue("身体运动迟缓和减少");
            row.CreateCell(70).SetCellValue("UPDRS总分");
            row.CreateCell(71).SetCellValue("评分医生");
            row.CreateCell(72).SetCellValue("录像文件");
            row.CreateCell(73).SetCellValue("评分提交时间");
            wk.Write(fs);
            wk.Close();
            fs.Close();
             
        }

        //填写总表的分项(追加式)
        public void addExcel()
        {
            FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            POIFSFileSystem ps = new POIFSFileSystem(fs);
            HSSFWorkbook wk = new HSSFWorkbook(ps);
            ISheet tb = wk.GetSheet("Sheet01");
            int number = tb.LastRowNum;
            FileStream fs1 = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            IRow row1 = tb.CreateRow(number+1);
            row1.CreateCell(0).SetCellValue(BRHtextBox.Text);
            row1.CreateCell(1).SetCellValue("");
            row1.CreateCell(2).SetCellValue(SARAbtcomboBox.SelectedIndex);
            row1.CreateCell(3).SetCellValue(SARAzzcomboBox.SelectedIndex);
            row1.CreateCell(4).SetCellValue(SARAzuzcomboBox.SelectedIndex);
            row1.CreateCell(5).SetCellValue(SARAgyblcomboBox.SelectedIndex);
            row1.CreateCell(6).SetCellValue(SARAszzzcomboBoxL.SelectedIndex);
            row1.CreateCell(7).SetCellValue(SARAszzzcomboBoxR.SelectedIndex);
            row1.CreateCell(8).SetCellValue(SARAzbcomboBoxL.SelectedIndex);
            row1.CreateCell(9).SetCellValue(SARAzbcomboBoxR.SelectedIndex);
            row1.CreateCell(10).SetCellValue(SARAksltcomboBoxL.SelectedIndex);
            row1.CreateCell(11).SetCellValue(SARAksltcomboBoxR.SelectedIndex);
            row1.CreateCell(12).SetCellValue(SARAgxjcomboBoxL.SelectedIndex);
            row1.CreateCell(13).SetCellValue(SARAgxjcomboBoxR.SelectedIndex);
            row1.CreateCell(14).SetCellValue(SARAzfI);
            row1.CreateCell(15).SetCellValue("");
            row1.CreateCell(16).SetCellValue(SPPBblzlcomboBox.SelectedIndex);
            row1.CreateCell(17).SetCellValue(blzlUT);
            row1.CreateCell(18).SetCellValue(SPPBbblzlcomboBox.SelectedIndex);
            row1.CreateCell(19).SetCellValue(bclzlUT);
            row1.CreateCell(20).SetCellValue(SPPBclzlcomboBox.SelectedIndex);
            row1.CreateCell(21).SetCellValue(clzlUT);
            row1.CreateCell(22).SetCellValue(SPPBxzsdcomboBox.SelectedIndex);
            row1.CreateCell(23).SetCellValue(xzsdUT);
            row1.CreateCell(24).SetCellValue(SPPBcfzwqlcomboBox.SelectedIndex);
            row1.CreateCell(25).SetCellValue(cfqlUT);
            row1.CreateCell(26).SetCellValue(SPPBzfI);
            row1.CreateCell(27).SetCellValue("");
            row1.CreateCell(28).SetCellValue(TqbcomboBox.SelectedIndex);
            row1.CreateCell(29).SetCellValue(TtjgdcomboBoxL.SelectedIndex);
            row1.CreateCell(30).SetCellValue(TtjgdcomboBoxR.SelectedIndex);
            row1.CreateCell(31).SetCellValue(TbccomboBoxL.SelectedIndex);
            row1.CreateCell(32).SetCellValue(TbccomboBoxR.SelectedIndex);
            row1.CreateCell(33).SetCellValue(TbtdcxcomboBox.SelectedIndex);
            row1.CreateCell(34).SetCellValue(TbflxxcomboBox.SelectedIndex);
            row1.CreateCell(35).SetCellValue(TljcomboBox.SelectedIndex);
            row1.CreateCell(36).SetCellValue(TqgwdcomboBox.SelectedIndex);
            row1.CreateCell(37).SetCellValue(TbkcomboBox.SelectedIndex);
            row1.CreateCell(38).SetCellValue(TxzzzscomboBox.SelectedIndex);
            row1.CreateCell(39).SetCellValue(TBTzfI);
            row1.CreateCell(40).SetCellValue(TzwphcomboBox.SelectedIndex);
            row1.CreateCell(41).SetCellValue(TzqcomboBox.SelectedIndex);
            row1.CreateCell(42).SetCellValue(TjkzlpxcomboBox.SelectedIndex);
            row1.CreateCell(43).SetCellValue(TzlphcomboBox.SelectedIndex);
            row1.CreateCell(44).SetCellValue(TbmzlcomboBox.SelectedIndex);
            row1.CreateCell(45).SetCellValue(TqtsycomboBox.SelectedIndex);
            row1.CreateCell(46).SetCellValue(TzscomboBox.SelectedIndex);
            row1.CreateCell(47).SetCellValue(TzxcomboBox.SelectedIndex);
            row1.CreateCell(48).SetCellValue(TPHzfI);
            row1.CreateCell(49).SetCellValue("");
            row1.CreateCell(50).SetCellValue(UyycomboBox.SelectedIndex);
            row1.CreateCell(51).SetCellValue(UmbbqcomboBox.SelectedIndex);
            row1.CreateCell(52).SetCellValue(UjzxzccomboBox.SelectedIndex);
            row1.CreateCell(53).SetCellValue(UdzxzccomboBoxL.SelectedIndex);
            row1.CreateCell(54).SetCellValue(UdzxzccomboBoxR.SelectedIndex);
            row1.CreateCell(55).SetCellValue(UjycomboBoxL.SelectedIndex);
            row1.CreateCell(56).SetCellValue(UjycomboBoxR.SelectedIndex);
            row1.CreateCell(57).SetCellValue(UsznhcomboBoxL.SelectedIndex);
            row1.CreateCell(58).SetCellValue(UsznhcomboBoxR.SelectedIndex);
            row1.CreateCell(59).SetCellValue(UsbydcomboBoxL.SelectedIndex);
            row1.CreateCell(60).SetCellValue(UsbydcomboBoxR.SelectedIndex);
            row1.CreateCell(61).SetCellValue(UssltcomboBoxL.SelectedIndex);
            row1.CreateCell(62).SetCellValue(UssltcomboBoxR.SelectedIndex);
            row1.CreateCell(63).SetCellValue(UxzlhdcomboBoxL.SelectedIndex);
            row1.CreateCell(64).SetCellValue(UxzlhdcomboBoxR.SelectedIndex);
            row1.CreateCell(65).SetCellValue(UzyqlcomboBox.SelectedIndex);
            row1.CreateCell(66).SetCellValue(UzscomboBox.SelectedIndex);
            row1.CreateCell(67).SetCellValue(UbtcomboBox.SelectedIndex);
            row1.CreateCell(68).SetCellValue(UztphcomboBox.SelectedIndex);
            row1.CreateCell(69).SetCellValue(UchhjscomboBox.SelectedIndex);
            row1.CreateCell(70).SetCellValue(UPDRSzfI);
            row1.CreateCell(71).SetCellValue(PFYStextBox.Text);
            row1.CreateCell(72).SetCellValue(FileName);
            row1.CreateCell(73).SetCellValue(DateTime.Now.ToString("yyyy_MM_dd_HH", DateTimeFormatInfo.InvariantInfo));
            fs1.Flush();
            wk.Write(fs1);
            wk.Close();
            fs1.Close();
        }

        //清理界面
        public void clearUI()
        { 
            //清理下拉框
            SARAbtcomboBox.SelectedIndex = -1;
            SARAzzcomboBox.SelectedIndex =-1;
            SARAzuzcomboBox.SelectedIndex = -1;
            SARAgyblcomboBox.SelectedIndex = -1;
            SARAszzzcomboBoxL.SelectedIndex =-1;
            SARAszzzcomboBoxR.SelectedIndex =-1;
            SARAzbcomboBoxL.SelectedIndex =-1; 
            SARAzbcomboBoxR.SelectedIndex =-1; 
            SARAksltcomboBoxL.SelectedIndex =-1;
            SARAksltcomboBoxR.SelectedIndex =-1;
            SARAgxjcomboBoxL.SelectedIndex = -1; 
            SARAgxjcomboBoxR.SelectedIndex =-1;
            SPPBblzlcomboBox.SelectedIndex =-1; 
            SPPBbblzlcomboBox.SelectedIndex =-1;
            SPPBclzlcomboBox.SelectedIndex =-1;
            SPPBxzsdcomboBox.SelectedIndex=-1;
            SPPBcfzwqlcomboBox.SelectedIndex=-1;
            TzwphcomboBox.SelectedIndex=-1;
            TzqcomboBox.SelectedIndex =-1;
            TjkzlpxcomboBox.SelectedIndex=-1;
            TzlphcomboBox.SelectedIndex =-1; 
            TbmzlcomboBox.SelectedIndex =-1;
            TqtsycomboBox.SelectedIndex=-1;
            TzscomboBox.SelectedIndex =-1;
            TzxcomboBox.SelectedIndex=-1;
            UyycomboBox.SelectedIndex=-1;
            UmbbqcomboBox.SelectedIndex =-1;
            UjzxzccomboBox.SelectedIndex=-1;
            UdzxzccomboBoxL.SelectedIndex =-1;
            UdzxzccomboBoxR.SelectedIndex =-1;
            UjycomboBoxL.SelectedIndex=-1;
            UjycomboBoxR.SelectedIndex =-1; 
            UsznhcomboBoxL.SelectedIndex =-1;
            UsznhcomboBoxR.SelectedIndex=-1;
            UsbydcomboBoxL.SelectedIndex =-1;
            UsbydcomboBoxR.SelectedIndex =-1;
            UssltcomboBoxL.SelectedIndex=-1;
            UssltcomboBoxR.SelectedIndex =-1;
            UxzlhdcomboBoxL.SelectedIndex =-1;
            UxzlhdcomboBoxR.SelectedIndex =-1;
            UzyqlcomboBox.SelectedIndex =-1;
            UzscomboBox.SelectedIndex =-1;
            UbtcomboBox.SelectedIndex= -1;
            UztphcomboBox.SelectedIndex =-1;
            UchhjscomboBox.SelectedIndex = -1;
            TqbcomboBox.SelectedIndex =-1;
            TtjgdcomboBoxL.SelectedIndex=-1;
            TtjgdcomboBoxR.SelectedIndex=-1;
            TbccomboBoxL.SelectedIndex =-1;
            TbccomboBoxR.SelectedIndex =-1;
            TbtdcxcomboBox.SelectedIndex=-1;
            TbflxxcomboBox.SelectedIndex =-1;
            TljcomboBox.SelectedIndex =-1;
            TqgwdcomboBox.SelectedIndex=-1;
            TbkcomboBox.SelectedIndex =-1;
            TxzzzscomboBox.SelectedIndex =-1;

            //清理SPPB的时间文字
            SPPBblzlSlabel.Text = "开始时间";
            SPPBbblzlSlabel.Text = "开始时间";
            SPPBclzlSlabel.Text = "开始时间";
            SPPBxzsdSlabel.Text = "开始时间";
            SPPBcfzwqlSlabel.Text = "开始时间";
            SPPBblzlElabel.Text = "结束时间";
            SPPBbblzlElabel.Text = "结束时间";
            SPPBclzlElabel.Text = "结束时间";
            SPPBxzsdElabel.Text = "结束时间";
            SPPBcfzwqlElabel.Text = "结束时间";
            SPPBblzlYlabel.Text = "用时";
            SPPBbblzlYlabel.Text = "用时";
            SPPBclzlYlabel.Text = "用时";
            SPPBxzsdYlabel.Text = "用时";
            SPPBcfzwqlYlabel.Text = "用时";

            //清理切分表时间文字
            QFzzSTlabel.Text = "开始时间";
            QFszzzLSTlabel.Text = "开始时间";
            QFszzzRSTlabel.Text = "开始时间";
            QFzbsyLSTlabel.Text = "开始时间";
            QFzbsyRSTlabel.Text = "开始时间";
            QFksltLlSTlabel.Text = "开始时间";
            QFksltRSTlabel.Text = "开始时间";
            QFsznhLSTlabel.Text = "开始时间";
            QFsznhRSTlabel.Text = "开始时间";
            QFsbydLSTlabel.Text = "开始时间";
            QFsbydRSTlabel.Text = "开始时间";
            QFssltSTLlabel.Text = "开始时间";
            QFssltRSTlabel.Text = "开始时间";
            QFxzlhLSTlabel.Text = "开始时间";
            QFxzlhSTRlabel.Text = "开始时间";
            QFzrqzSTlabel.Text = "开始时间";
            QFbxqqlSTlabel.Text = "开始时间";
            QFcfqlSTlabel.Text = "开始时间";
            QFzrzzSTlabel.Text = "开始时间";
            QFblzlSTlabel.Text = "开始时间";
            QFbmzlSTlabel.Text = "开始时间";
            QFbclzlSTlabel.Text = "开始时间";
            QFclzlSTlabel.Text = "开始时间";
            QFyzbxzSTlabel.Text = "开始时间";
            QFhlsySTlabel.Text = "开始时间";
            QFzsSTlabel.Text = "开始时间";
            QFsmbxSTlabel.Text = "开始时间";
            QFsmbxfSTlabel.Text = "开始时间";

            QFzzETlabel.Text = "结束时间";
            QFszzzLETlabel.Text = "结束时间";
            QFszzzRETlabel.Text = "结束时间";
            QFzbsyLETlabel.Text = "结束时间";
            QFzbsyETRlabel.Text = "结束时间";
            QFksltLETllabel.Text = "结束时间";
            QFksltRETlabel.Text = "结束时间";
            QFsznhLETlabel.Text = "结束时间";
            QFsznhRETlabel.Text = "结束时间";
            QFsbydLETlabel.Text = "结束时间";
            QFsbydETRlabel.Text = "结束时间";
            QFssltETLlabel.Text = "结束时间";
            QFssltRETlabel.Text = "结束时间";
            QFxzlhLETlabel.Text = "结束时间";
            QFxzlhRETlabel.Text = "结束时间";
            QFzrqzETlabel.Text = "结束时间";
            QFbxqqlETlabel.Text = "结束时间";
            QFcfqlETlabel.Text = "结束时间";
            QFzrzzETlabel.Text = "结束时间";
            QFblzlETlabel.Text = "结束时间";
            QFbmzlETlabel.Text = "结束时间";
            QFbclzlETlabel.Text = "结束时间";
            QFclzlETlabel.Text = "结束时间";
            QFyzbxzETlabel.Text = "结束时间";
            QFhlsyETlabel.Text = "结束时间";
            QFzsETlabel.Text = "结束时间";
            QFsmbxETlabel.Text = "结束时间";
            QFsmbxfETlabel.Text = "结束时间";

            QFzzUTlabel.Text = "用时";
            QFszzzLUTlabel.Text = "用时";
            QFszzzRUTlabel.Text = "用时";
            QFzbsyLUTlabel.Text = "用时";
            QFzbsyUTRlabel.Text = "用时";
            QFksltLlUTlabel.Text = "用时";
            QFksltRUTlabel.Text = "用时";
            QFsznhLUTlabel.Text = "用时";
            QFsznhRUTlabel.Text = "用时";
            QFsbydLUTlabel.Text = "用时";
            QFsbydRUTlabel.Text = "用时";
            QFssltUTLlabel.Text = "用时";
            QFssltRUTlabel.Text = "用时";
            QFxzlhLUTlabel.Text = "用时";
            QFxzlhRUTlabel.Text = "用时";
            QFzrqzUTlabel.Text = "用时";
            QFbxqqlUTlabel.Text = "用时";
            QFcfqlUTlabel.Text = "用时";
            QFzrzzUTlabel.Text = "用时";
            QFblzlUTlabel.Text = "用时";
            QFbmzlUTlabel.Text = "用时";
            QFbclzlUTlabel.Text = "用时";
            QFclzlUTlabel.Text = "用时";
            QFyzbxzUTlabel.Text = "用时";
            QFhlsyUTlabel.Text = "用时";
            QFzsUTlabel.Text = "用时";
            QFsmbxUTlabel.Text = "用时";
            QFsmbxfUTlabel.Text = "用时";

            //清理总分文字
            SARAzf.Text = "SARA总分";
            SPPBzflabel.Text = "SPPB总分";
            Tbtzflabel.Text = "步态总分";
            Tphzflabel.Text = "平衡总分";
            UPDRSzflabel.Text = "UPDRS总分";

            //清理病人号和医生名
            BRHtextBox.Text = "";
            PFYStextBox.Text = "";
        }


        //清理数据
        public void clearData()
        {
            //各项表总分
            SARAzfI = 0;
            SPPBzfI = 0;
            TBTzfI = 0;
            TPHzfI = 0;
            UPDRSzfI = 0;

            //SPPB表计时用
            StartTime = 0;
            EndTime = 0;
            blzlST = 0;
            blzlET = 0;
            blzlUT = 0;

            bclzlST = 0;
            bclzlET = 0;
            bclzlUT = 0;

            clzlST = 0;
            clzlET = 0;
            clzlUT = 0;

            xzsdST = 0;
            xzsdET = 0;
            xzsdUT = 0;

            cfqlST = 0;
            cfqlET = 0;
            cfqlUT = 0;

            //切分记录用时
            QFzzST = 0;
            QFzzET = 0;
            QFzzUT = 0;

            QFszzzLST = 0;
            QFszzzLET = 0;
            QFszzzLUT = 0;

            QFszzzRST = 0;
            QFszzzRET = 0;
            QFszzzRUT = 0;

            QFzbsyLST = 0;
            QFzbsyLET = 0;
            QFzbsyLUT = 0;

            QFzbsyRST = 0;
            QFzbsyRET = 0;
            QFzbsyRUT = 0;

            QFksltLST = 0;
            QFksltLET = 0;
            QFksltLUT = 0;

            QFksltRST = 0;
            QFksltRET = 0;
            QFksltRUT = 0;

            QFsznhLST = 0;
            QFsznhLET = 0;
            QFsznhLUT = 0;

            QFsznhRST = 0;
            QFsznhRET = 0;
            QFsznhRUT = 0;

            QFsbydLST = 0;
            QFsbydLET = 0;
            QFsbydLUT = 0;

            QFsbydRST = 0;
            QFsbydRET = 0;
            QFsbydRUT = 0;

            QFssltLST = 0;
            QFssltLET = 0;
            QFssltLUT = 0;

            QFssltRST = 0;
            QFssltRET = 0;
            QFssltRUT = 0;

            QFxzlhLST = 0;
            QFxzlhLET = 0;
            QFxzlhLUT = 0;

            QFxzlhRST = 0;
            QFxzlhRET = 0;
            QFxzlhRUT = 0;

            QFzrqzST = 0;
            QFzrqzET = 0;
            QFzrqzUT = 0;

            QFbxqqlST = 0;
            QFbxqqlET = 0;
            QFbxqqlUT = 0;

            QFcfqlST = 0;
            QFcfqlET = 0;
            QFcfqlUT = 0;

            QFzrzzST = 0;
            QFzrzzET = 0;
            QFzrzzUT = 0;

            QFblzlST = 0;
            QFblzlET = 0;
            QFblzlUT = 0;

            QFbmzlST = 0;
            QFbmzlET = 0;
            QFbmzlUT = 0;

            QFbclzlST = 0;
            QFbclzlET = 0;
            QFbclzlUT = 0;

            QFclzlST = 0;
            QFclzlET = 0;
            QFclzlUT = 0;

            QFyzbxzST = 0;
            QFyzbxzET = 0;
            QFyzbxzUT = 0;

            QFhlsyST = 0;
            QFhlsyET = 0;
            QFhlsyUT = 0;

            QFzsST = 0;
            QFzsET = 0;
            QFzsUT = 0;

            QFsmbxST = 0;
            QFsmbxET = 0;
            QFsmbxUT = 0;

            QFsmbxfST = 0;
            QFsmbxfET = 0;
            QFsmbxfUT = 0;

            //四个表
            Biao1 = false;
            Biao2 = false;
            Biao31 = false;
            Biao32 = false;
            Biao4 = false;

            //捏合次数
            LNHCounttextBox.Text = "";
            RNHCounttextBox.Text = "";
            
        }

        //清理按钮
        private void UIclearButton_Click(object sender, EventArgs e)
        {
            clearUI();
            clearData();
        }

        //选择总表的保存位置
        private void SelectDRbutton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel文件(*.xls)|*.xls";
            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                savePath = saveFile.FileName;
            }
        }

        private void QFzzSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzzST =  Math.Round(StartTime, 2);
            QFzzSTlabel.Text = "开始时间:" + QFzzST;
        }

        private void QFzzETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzzET = Math.Round(EndTime, 2);
            QFzzETlabel.Text = "结束时间:" + QFzzET;


            QFzzUT = Math.Round(QFzzET - QFzzST,2);
            QFzzUTlabel.Text = "用时:" + QFzzUT;
        }

        private void QFzzUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFzzbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzzST;
        }

        private void QFszzzSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFszzzLST = Math.Round(StartTime, 2);
            QFszzzLSTlabel.Text = "开始时间:" + QFszzzLST;
        }

        private void QFszzzETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFszzzLET = Math.Round(EndTime, 2);
            QFszzzLETlabel.Text = "结束时间:" + QFszzzLET;


            QFszzzLUT = Math.Round(QFszzzLET - QFszzzLST,2);
            QFszzzLUTlabel.Text = "用时:" + QFszzzLUT;
        }

        private void QFszzzUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFszzzbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFszzzLST;
        }

        private void QFzbsySTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzbsyLST = Math.Round(StartTime, 2);
            QFzbsyLSTlabel.Text = "开始时间:" + QFzbsyLST;
        }

        private void QFzbsyETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzbsyLET = Math.Round(EndTime, 2);
            QFzbsyLETlabel.Text = "结束时间:" + QFzbsyLET;


            QFzbsyLUT = Math.Round(QFzbsyLET - QFzbsyLST,2);
            QFzbsyLUTlabel.Text = "用时:" + QFzbsyLUT;
        }

        private void QFzbsyUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFzbsybutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzbsyLST;
        }

        private void QFksltLlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFksltLST = Math.Round(StartTime, 2);
            QFksltLlSTlabel.Text = "开始时间:" + QFksltLST;
        }

        private void QFksltLETllabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFksltLET = Math.Round(EndTime, 2);
            QFksltLETllabel.Text = "结束时间:" + QFksltLET;

            QFksltLUT = Math.Round(QFksltLET - QFksltLST,2);
            QFksltLlUTlabel.Text = "用时:" + QFksltLUT;
        }

        private void QFksltLlUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFksltLlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFksltLST;
        }

        private void QFksltRSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFksltRST = Math.Round(StartTime, 2);
            QFksltRSTlabel.Text = "开始时间:" + QFksltRST;
        }

        private void QFksltRETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFksltRET = Math.Round(EndTime, 2);
            QFksltRETlabel.Text = "结束时间:" + QFksltRET;

            QFksltRUT = Math.Round(QFksltRET - QFksltRST,2);
            QFksltRUTlabel.Text = "用时:" + QFksltRUT;
        }

        private void QFksltRUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFksltRbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFksltRST;
        }

        private void QFsznhLSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsznhLST = Math.Round(StartTime, 2);
            QFsznhLSTlabel.Text = "开始时间:" + QFsznhLST;
        }

        private void QFsznhLETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsznhLET = Math.Round(EndTime, 2);
            QFsznhLETlabel.Text = "结束时间:" + QFsznhLET;

            QFsznhLUT = Math.Round(QFsznhLET - QFsznhLST,2);
            QFsznhLUTlabel.Text = "用时:" + QFsznhLUT;
        }

        private void QFsznhLUTlabel_Click(object sender, EventArgs e)
        {
          
        }

        private void QFsznhLbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsznhLST;
        }

        private void QFsznhRSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsznhRST = Math.Round(StartTime, 2);
            QFsznhRSTlabel.Text = "开始时间:" + QFsznhRST;
        }

        private void QFsznhRETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsznhRET = Math.Round(EndTime, 2);
            QFsznhRETlabel.Text = "结束时间:" + QFsznhRET;

            QFsznhRUT = Math.Round(QFsznhRET - QFsznhRST,2);
            QFsznhRUTlabel.Text = "用时:" + QFsznhRUT;
        }

        private void QFsznhRUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFsznhRbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsznhRST;
        }

        private void QFsbydSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsbydLST = Math.Round(StartTime, 2);
            QFsbydLSTlabel.Text = "开始时间:" + QFsbydLST;
        }

        private void QFsbydETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsbydLET = Math.Round(EndTime, 2);
            QFsbydLETlabel.Text = "结束时间:" + QFsbydLET;

            QFsbydLUT = Math.Round(QFsbydLET - QFsbydLST,2);
            QFsbydLUTlabel.Text = "用时:" + QFsbydLUT;
        }

        private void QFsbydUTlabel_Click(object sender, EventArgs e)
        {
           
        }

        private void QFsbydbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsbydLST;
        }

        private void QFssltSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFssltLST = Math.Round(StartTime, 2);
            QFssltSTLlabel.Text = "开始时间:" + QFssltLST;
        }

        private void QFssltETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFssltLET = Math.Round(EndTime, 2);
            QFssltETLlabel.Text = "结束时间:" + QFssltLET;

            QFssltLUT = Math.Round(QFssltLET - QFssltLST,2);
            QFssltUTLlabel.Text = "用时:" + QFssltLUT;
        }

        private void QFssltUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFssltbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFssltLST;
        }

        private void QFxzlhdSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFxzlhLST = Math.Round(StartTime, 2);
            QFxzlhLSTlabel.Text = "开始时间:" + QFxzlhLST;
        }

        private void QFxzlhdETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFxzlhLET = Math.Round(EndTime, 2);
            QFxzlhLETlabel.Text = "结束时间:" + QFxzlhLET;

            QFxzlhLUT = Math.Round(QFxzlhLET - QFxzlhLST,2);
            QFxzlhLUTlabel.Text = "用时:" + QFxzlhLUT;
        }

        private void QFxzlhdUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFxzlhdbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFxzlhLST;
        }


        private void QFzrqlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzrqzST = Math.Round(StartTime, 2);
            QFzrqzSTlabel.Text = "开始时间:" + QFzrqzST;
        }

        private void QFzrqlETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzrqzET = Math.Round(EndTime, 2);
            QFzrqzETlabel.Text = "结束时间:" + QFzrqzET;


            QFzrqzUT = Math.Round(QFzrqzET - QFzrqzST, 2);
            QFzrqzUTlabel.Text = "用时:" + QFzrqzUT;
        }

        private void QFzrqlUTlabel_Click(object sender, EventArgs e)
        {
            
        
        }

        private void QFzrqlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzrqzST;
        }

        private void QFbxqqlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFbxqqlST = Math.Round(StartTime, 2);
            QFbxqqlSTlabel.Text = "开始时间:" + QFbxqqlST;
        }

        private void QFbxqqlETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFbxqqlET = Math.Round(EndTime, 2);
            QFbxqqlETlabel.Text = "结束时间:" + QFbxqqlET;

            QFbxqqlUT = Math.Round(QFbxqqlET - QFbxqqlST,2);
            QFbxqqlUTlabel.Text = "用时:" + QFbxqqlUT;
        }

        private void QFbxqqlUTlabel_Click(object sender, EventArgs e)
        {
           
        }

        private void QFbxqqlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFbxqqlST;
        }

        private void QFcfqlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFcfqlST = Math.Round(StartTime, 2);
            QFcfqlSTlabel.Text = "开始时间:" + QFcfqlST;

            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            cfqlST = Math.Round(StartTime, 2);
            SPPBcfzwqlSlabel.Text = "开始时间:" + cfqlST;
        }

        private void QFcfqlETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFcfqlET = Math.Round(EndTime, 2);
            QFcfqlETlabel.Text = "结束时间:" + QFcfqlET;


            QFcfqlUT = Math.Round(QFcfqlET - QFcfqlST,2);
            QFcfqlUTlabel.Text = "用时:" + QFcfqlUT;

            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            cfqlET = Math.Round(EndTime, 2);
            SPPBcfzwqlElabel.Text = "结束时间:" + cfqlET;


            cfqlUT = Math.Round(cfqlET - cfqlST, 2);
            SPPBcfzwqlYlabel.Text = "用时:" + cfqlUT;
        }

        private void QFcfqlUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFcfqlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFcfqlST;
        }

        private void QFzrzlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzrzzST = Math.Round(StartTime, 2);
            QFzrzzSTlabel.Text = "开始时间:" + QFzrzzST;
        }

        private void QFzrzlETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzrzzET = Math.Round(EndTime, 2);
            QFzrzzETlabel.Text = "结束时间:" + QFzrzzET;

            QFzrzzUT = Math.Round(QFzrzzET - QFzrzzST,2);
            QFzrzzUTlabel.Text = "用时:" + QFzrzzUT;
        }

        private void QFzrzlUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFzrzlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzrzzST;
        }

        //有对应关系的，较为特殊
        private void QFblzlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFblzlST = Math.Round(StartTime, 2);
            QFblzlSTlabel.Text = "开始时间:" + QFblzlST;

            blzlST = Math.Round(StartTime, 2);
            SPPBblzlSlabel.Text = "开始时间:" + blzlST;
        }

        //有对应关系的，较为特殊
        private void QFblzlETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFblzlET = Math.Round(EndTime, 2);
            QFblzlETlabel.Text = "结束时间:" + QFblzlET;

            QFblzlUT = Math.Round(QFblzlET - QFblzlST,2);
            QFblzlUTlabel.Text = "用时:" + QFblzlUT;

            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            blzlET = Math.Round(EndTime, 2);
            SPPBblzlElabel.Text = "结束时间:" + blzlET;


            blzlUT = Math.Round(blzlET - blzlST, 2);
            SPPBblzlYlabel.Text = "用时:" + blzlUT;
        }

        private void QFblzlUTlabel_Click(object sender, EventArgs e)
        {
           
        }

        private void QFblzlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFblzlST;
        }

        private void QFbmzlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFbmzlST = Math.Round(StartTime, 2);
            QFbmzlSTlabel.Text = "开始时间:" + QFbmzlST;
        }

        private void QFbmzlETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFbmzlET = Math.Round(EndTime, 2);
            QFbmzlETlabel.Text = "结束时间:" + QFbmzlET;

            QFbmzlUT = Math.Round(QFbmzlET - QFbmzlST,2);
            QFbmzlUTlabel.Text = "用时:" + QFbmzlUT;
        }

        private void QFbmzlUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFbmzlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFbmzlST;
        }

        private void QFbblzlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFbclzlST = Math.Round(StartTime, 2);
            QFbclzlSTlabel.Text = "开始时间:" + QFbclzlST;

            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            bclzlST = Math.Round(StartTime, 2);
            SPPBbblzlSlabel.Text = "开始时间:" + bclzlST;
        }

        private void QFbblzlETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFbclzlET = Math.Round(EndTime, 2);
            QFbclzlETlabel.Text = "结束时间:" + QFbclzlET;

            QFbclzlUT = Math.Round(QFbclzlET - QFbclzlST,2);
            QFbclzlUTlabel.Text = "用时:" + QFbclzlUT;

            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            bclzlET = Math.Round(EndTime, 2);
            SPPBbblzlElabel.Text = "结束时间:" + bclzlET;


            bclzlUT = Math.Round(bclzlET - bclzlST, 2);
            SPPBbblzlYlabel.Text = "用时:" + bclzlUT;
        }

        private void QFbblzlUTlabel_Click(object sender, EventArgs e)
        {
           
        }

        private void QFbblzlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFbclzlST;
        }

        private void QFclzlSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFclzlST = Math.Round(StartTime, 2);
            QFclzlSTlabel.Text = "开始时间:" + QFclzlST;

            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            clzlST = Math.Round(StartTime, 2);
            SPPBclzlSlabel.Text = "开始时间:" + clzlST;
        }

        private void QFclzlETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFclzlET = Math.Round(EndTime, 2);
            QFclzlETlabel.Text = "结束时间:" + QFclzlET;

            QFclzlUT = Math.Round(QFclzlET - QFclzlST,2);
            QFclzlUTlabel.Text = "用时:" + QFclzlUT;

            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            clzlET = Math.Round(EndTime, 2);
            SPPBclzlElabel.Text = "结束时间:" + clzlET;


            clzlUT = Math.Round(clzlET - clzlST, 2);
            SPPBclzlYlabel.Text = "用时:" + clzlUT;
        }

        private void QFclzlUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFclzlbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFclzlST;
        }

        private void QFyzbxzSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFyzbxzST = Math.Round(StartTime, 2);
            QFyzbxzSTlabel.Text = "开始时间:" + QFyzbxzST;
        }

        private void QFyzbxzETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFyzbxzET = Math.Round(EndTime, 2);
            QFyzbxzETlabel.Text = "结束时间:" + QFyzbxzET;

            QFyzbxzUT = Math.Round(QFyzbxzET - QFyzbxzST,2);
            QFyzbxzUTlabel.Text = "用时:" + QFyzbxzUT;
        }

        private void QFyzbxzUTlabel_Click(object sender, EventArgs e)
        {
           
        }

        private void QFyzbxzbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFyzbxzST;
        }

        private void QFhlsySTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFhlsyST = Math.Round(StartTime, 2);
            QFhlsySTlabel.Text = "开始时间:" + QFhlsyST;
        }

        private void QFhlsyETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFhlsyET = Math.Round(EndTime, 2);
            QFhlsyETlabel.Text = "结束时间:" + QFhlsyET;

            QFhlsyUT = Math.Round(QFhlsyET - QFhlsyST,2);
            QFhlsyUTlabel.Text = "用时:" + QFhlsyUT;
        }

        private void QFhlsyUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFhlsybutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFhlsyST;
        
        }

        private void QFzsSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzsST = Math.Round(StartTime, 2);
            QFzsSTlabel.Text = "开始时间:" + QFzsST;
        }

        private void QFzsETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzsET = Math.Round(EndTime, 2);
            QFzsETlabel.Text = "结束时间:" + QFzsET;

            QFzsUT = Math.Round(QFzsET - QFzsST,2);
            QFzsUTlabel.Text = "用时:" + QFzsUT;
        }

        private void QFzsUTlabel_Click(object sender, EventArgs e)
        {
            
        }

        private void QFzsbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzsST;
        
        }

        private void QFsmbxSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsmbxST = Math.Round(StartTime, 2);
            QFsmbxSTlabel.Text = "开始时间:" + QFsmbxST;

            xzsdST = Math.Round(StartTime, 2);
            SPPBxzsdSlabel.Text = "开始时间:" + xzsdST;
           
        }

        private void QFsmbxETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsmbxET = Math.Round(EndTime, 2);
            QFsmbxETlabel.Text = "结束时间:" + QFsmbxET;

            QFsmbxUT = Math.Round(QFsmbxET - QFsmbxST,2);
            QFsmbxUTlabel.Text = "用时:" + QFsmbxUT;


            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            xzsdET = Math.Round(EndTime, 2);
            SPPBxzsdElabel.Text = "结束时间:" + xzsdET;


            xzsdUT = Math.Round(QFsmbxET - QFsmbxST, 2);
            SPPBxzsdYlabel.Text = "用时:" + xzsdUT;
        }

        private void QFsmbxUTlabel_Click(object sender, EventArgs e)
        {
           
        }

        private void QFsmbxbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsmbxST;
        }

        private void QFsmbxfSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsmbxfST = Math.Round(StartTime, 2);
            QFsmbxfSTlabel.Text = "开始时间:" + QFsmbxfST;
        }

        private void QFsmbxfETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsmbxfET = Math.Round(EndTime, 2);
            QFsmbxfETlabel.Text = "结束时间:" + QFsmbxfET;

            QFsmbxfUT = Math.Round(QFsmbxfET - QFsmbxfST, 2);
            QFsmbxfUTlabel.Text = "用时:" + QFsmbxfUT;
        }

        private void QFsmbxfUTlabel_Click(object sender, EventArgs e)
        {
  
        }

        private void QFsmbxfbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsmbxfST;
        }

        //创立总切分文件表头
        private void CreateTxtWrite()
        {
            FileStream fs = new FileStream("../../../../切分时间.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            String str = "病人号 "+"坐姿开始时间 "+"坐姿结束时间 "+"坐姿用时 "
                + "手指追踪(左)开始时间 " + "手指追踪(左)结束时间 " + "手指追踪(左)用时 "
                + "手指追踪(右)开始时间 " + "手指追踪(右)结束时间 " + "手指追踪(右)用时 "
                + "指鼻试验(左)开始时间 " + "指鼻试验(左)结束时间 " + "指鼻试验(左)用时 "
                + "指鼻试验(右)开始时间 " + "指鼻试验(右)结束时间 " + "指鼻试验(右)用时 "
                + "快速轮替(左)开始时间 " + "快速轮替(左)结束时间 " + "快速轮替(左)用时 "
                + "快速轮替(右)开始时间 " + "快速轮替(右)结束时间 " + "快速轮替(右)用时 "
                + "手指捏合(左)开始时间 " + "手指捏合(左)结束时间 " + "手指捏合(左)用时 "
                + "手指捏合(右)开始时间 " + "手指捏合(右)结束时间 " + "手指捏合(右)用时 "
                + "手部运动(左)开始时间 " + "手部运动(左)结束时间 " + "手部运动(左)用时 "
                + "手部运动(右)开始时间 " + "手部运动(右)结束时间 " + "手部运动(右)用时 "
                + "双手快速轮替(左)开始时间 " + "双手快速轮替(左)结束时间 " + "双手快速轮替(左)用时 "
                + "双手快速轮替(右)开始时间 " + "双手快速轮替(右)结束时间 " + "双手快速轮替(右)用时 "
                + "下肢灵活度(左)开始时间 " + "下肢灵活度(左)结束时间 " + "下肢灵活度(左)用时 "
                + "下肢灵活度(右)开始时间 " + "下肢灵活度(右)结束时间 " + "下肢灵活度(右)用时 "
                + "自然状态座椅起立开始时间 " + "自然状态座椅起立结束时间 " + "自然状态座椅起立用时 "
                + "双手抱于胸前从座椅起立时间 " + "双手抱于胸前从座椅结束时间 " + "双手抱于胸前从座椅用时 "
                + "重复座位起立开始时间 " + "重复座位起立结束时间 " + "重复座位起立用时 "
                + "自然站姿开始时间 " + "自然站姿结束时间 " + "自然站姿用时 "
                + "并联站立开始时间 " + "并联站立结束时间 " + "并联站立用时 "
                + "闭目站立及轻推实验开始时间 " + "闭目站立及轻推实验结束时间 " + "闭目站立及轻推实验用时 "
                + "半串联站立开始时间 " + "半串联站立结束时间 " + "半串联站立用时 "
                + "串联站立开始时间 " + "串联站立结束时间 " + "串联站立用时 "
                + "一字步行走开始时间 " + "一字步行走结束时间 " + "一字步行走用时 "
                + "后拉实验开始时间 " + "后拉实验结束时间 " + "后拉实验用时 "
                + "转身360度开始时间 " + "转身360度结束时间 " + "转身360度用时 "
                + "3米步行开始时间 " + "3米步行结束时间 " + "3米步行用时 "
                + "3米步行返回开始时间 " + "3米步行返回结束时间 " + "3米步行返回用时 ";
            sw.WriteLine(str);
            sw.Close();
            fs.Close();
        }

        //添加总切分文件内容(追加式)
        private void AddTxtWrite()
        {
            FileStream fs = new FileStream("../../../../切分时间.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            String str = BRHtextBox.Text + " " + QFzzST + " " + QFzzET + " " + QFzzUT + " "
                + QFszzzLST + " " + QFszzzLET + " " + QFszzzLUT + " "
                + QFszzzRST + " " + QFszzzRET + " " + QFszzzRUT + " "
                + QFzbsyLST + " " + QFzbsyLET + " " + QFzbsyLUT + " "
                + QFzbsyRST + " " + QFzbsyRET + " " + QFzbsyRUT + " "
                + QFksltLST + " " + QFksltLET + " " + QFksltLUT + " "
                + QFksltRST + " " + QFksltRET + " " + QFksltRUT + " "
                + QFsznhLST + " " + QFsznhLET + " " + QFsznhLUT + " "
                + QFsznhRST + " " + QFsznhRET + " " + QFsznhRUT + " "
                + QFsbydLST + " " + QFsbydLET + " " + QFsbydLUT + " "
                + QFsbydRST + " " + QFsbydRET + " " + QFsbydRUT + " "
                + QFssltLST + " " + QFssltLET + " " + QFssltLUT + " "
                + QFssltRST + " " + QFssltRET + " " + QFssltRUT + " "
                + QFxzlhLST + " " + QFxzlhLET + " " + QFxzlhLUT + " "
                + QFxzlhRST + " " + QFxzlhRET + " " + QFxzlhRUT + " "
                + QFzrqzST + " " + QFzrqzET + " " + QFzrqzUT + " "
                + QFbxqqlST + " " + QFbxqqlET + " " + QFbxqqlUT + " "
                + QFcfqlST + " " + QFcfqlET + " " + QFcfqlUT + " "
                + QFzrzzST + " " + QFzrzzET + " " + QFzrzzUT + " "
                + QFblzlST + " " + QFblzlET + " " + QFblzlUT + " "
                + QFbmzlST + " " + QFbmzlET + " " + QFbmzlUT + " "
                + QFbclzlST + " " + QFbclzlET + " " + QFbclzlUT + " "
                + QFclzlST + " " + QFclzlET + " " + QFclzlUT + " "
                + QFyzbxzST + " " + QFyzbxzET + " " + QFyzbxzUT + " "
                + QFhlsyST + " " + QFhlsyET + " " + QFhlsyUT + " "
                + QFzsST + " " + QFzsET + " " + QFzsUT + " "
                + QFsmbxST + " " + QFsmbxET + " " + QFsmbxUT + " "
                + QFsmbxfST + " " + QFsmbxfET + " " + QFsmbxfUT + " ";
            sw.WriteLine(str);
            sw.Close();
            fs.Close();
        }

        //添加单独切分文件内容
        private void AddPersonalTxtWrite(String savePath)
        {
            FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            String str = BRHtextBox.Text + " " + QFzzST + " " + QFzzET + " " + QFzzUT + " "
                + QFszzzLST + " " + QFszzzLET + " " + QFszzzLUT + " "
                + QFszzzRST + " " + QFszzzRET + " " + QFszzzRUT + " "
                + QFzbsyLST + " " + QFzbsyLET + " " + QFzbsyLUT + " "
                + QFzbsyRST + " " + QFzbsyRET + " " + QFzbsyRUT + " "
                + QFksltLST + " " + QFksltLET + " " + QFksltLUT + " "
                + QFksltRST + " " + QFksltRET + " " + QFksltRUT + " "
                + QFsznhLST + " " + QFsznhLET + " " + QFsznhLUT + " "
                + QFsznhRST + " " + QFsznhRET + " " + QFsznhRUT + " "
                + QFsbydLST + " " + QFsbydLET + " " + QFsbydLUT + " "
                + QFsbydRST + " " + QFsbydRET + " " + QFsbydRUT + " "
                + QFssltLST + " " + QFssltLET + " " + QFssltLUT + " "
                + QFssltRST + " " + QFssltRET + " " + QFssltRUT + " "
                + QFxzlhLST + " " + QFxzlhLET + " " + QFxzlhLUT + " "
                + QFxzlhRST + " " + QFxzlhRET + " " + QFxzlhRUT + " "
                + QFzrqzST + " " + QFzrqzET + " " + QFzrqzUT + " "
                + QFbxqqlST + " " + QFbxqqlET + " " + QFbxqqlUT + " "
                + QFcfqlST + " " + QFcfqlET + " " + QFcfqlUT + " "
                + QFzrzzST + " " + QFzrzzET + " " + QFzrzzUT + " "
                + QFblzlST + " " + QFblzlET + " " + QFblzlUT + " "
                + QFbmzlST + " " + QFbmzlET + " " + QFbmzlUT + " "
                + QFbclzlST + " " + QFbclzlET + " " + QFbclzlUT + " "
                + QFclzlST + " " + QFclzlET + " " + QFclzlUT + " "
                + QFyzbxzST + " " + QFyzbxzET + " " + QFyzbxzUT + " "
                + QFhlsyST + " " + QFhlsyET + " " + QFhlsyUT + " "
                + QFzsST + " " + QFzsET + " " + QFzsUT + " "
                + QFsmbxST + " " + QFsmbxET + " " + QFsmbxUT + " "
                + QFsmbxfST + " " + QFsmbxfET + " " + QFsmbxfUT + " ";
            sw.WriteLine(str);
            sw.Close();
            fs.Close();
        }

        private void SARAbtlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsmbxST;
        }

        private void SARAzuzlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzzST;
        }

        private void SARAszzzlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFszzzLST;
        }

        private void SARAzblabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzbsyLST;
        }

        private void SARAksltlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFksltLST;
        }

        private void SPPBblzllabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFblzlST;
        }

        private void SPPBbblzjlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFbclzlST;
        }

        private void SPPBclzllabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFclzlST;
        }

        private void SPPBcfzwqllabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFcfqlST;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsmbxST;
        }

        private void Tbmzllabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFbmzlST;
        }

        private void Tzslabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzsST;
        }

        private void Ujylabel_Click(object sender, EventArgs e)
        {
            
        }

        private void Usznhlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsznhLST;
        }

        private void Usbydlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsbydLST;
        }

        private void Ussltlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFssltLST;
        }

        private void Txzlhdlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFxzlhLST;
        }

        private void Ubtlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsmbxST;
        }

        private void QFjylabel_Click(object sender, EventArgs e)
        {

        }

        private void QFszzzRSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFszzzRST = Math.Round(StartTime, 2);
            QFszzzRSTlabel.Text = "开始时间:" + QFszzzRST;
        }

        private void QFszzzRETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFszzzRET = Math.Round(EndTime, 2);
            QFszzzRETlabel.Text = "结束时间:" + QFszzzRET;


            QFszzzRUT = Math.Round(QFszzzRET - QFszzzRST, 2);
            QFszzzRUTlabel.Text = "用时:" + QFszzzRUT;
        }

        private void QFszzzRUTlabel_Click(object sender, EventArgs e)
        {

        }

        private void QFszzzRbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFszzzRST;
        }

        private void QFzbsyRSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzbsyRST = Math.Round(StartTime, 2);
            QFzbsyRSTlabel.Text = "开始时间:" + QFzbsyRST;
        }

        private void QFzbsyETRlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFzbsyRET = Math.Round(EndTime, 2);
            QFzbsyETRlabel.Text = "结束时间:" + QFzbsyRET;


            QFzbsyRUT = Math.Round(QFzbsyRET - QFzbsyRST, 2);
            QFzbsyUTRlabel.Text = "用时:" + QFzbsyRUT;
        }

        private void QFzbsyUTRlabel_Click(object sender, EventArgs e)
        {

        }

        private void QFzbsyRbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzbsyRST;
        }

        private void QFsbydRSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsbydRST = Math.Round(StartTime, 2);
            QFsbydRSTlabel.Text = "开始时间:" + QFsbydRST;
        }

        private void QFsbydETRlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsbydRET = Math.Round(EndTime, 2);
            QFsbydETRlabel.Text = "结束时间:" + QFsbydRET;

            QFsbydRUT = Math.Round(QFsbydRET - QFsbydRST, 2);
            QFsbydRUTlabel.Text = "用时:" + QFsbydRUT;
        }

        private void QFsbydRUTlabel_Click(object sender, EventArgs e)
        {

        }

        private void QFsbydRbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsbydRST;
        }

        private void QFssltRSTlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFssltRST = Math.Round(StartTime, 2);
            QFssltRSTlabel.Text = "开始时间:" + QFssltRST;
        }

        private void QFssltRETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFssltRET = Math.Round(EndTime, 2);
            QFssltRETlabel.Text = "结束时间:" + QFssltRET;

            QFssltRUT = Math.Round(QFssltRET - QFssltRST, 2);
            QFssltRUTlabel.Text = "用时:" + QFssltRUT;
        }

        private void QFssltRUTlabel_Click(object sender, EventArgs e)
        {

        }

        private void QFssltRbutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFssltRST;
        }

        private void QFxzlhSTRlabel_Click(object sender, EventArgs e)
        {
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFxzlhRST = Math.Round(StartTime, 2);
            QFxzlhSTRlabel.Text = "开始时间:" + QFxzlhRST;
        }

        private void QFxzlhRETlabel_Click(object sender, EventArgs e)
        {
            EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFxzlhRET = Math.Round(EndTime, 2);
            QFxzlhRETlabel.Text = "结束时间:" + QFxzlhRET;

            QFxzlhRUT = Math.Round(QFxzlhRET - QFxzlhRST, 2);
            QFxzlhRUTlabel.Text = "用时:" + QFxzlhRUT;
        }

        private void QFxzlhRUTlabel_Click(object sender, EventArgs e)
        {

        }

        private void QFxzlhRbutton_Click(object sender, EventArgs e)
        {
             this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFxzlhRST;
        
        }

        private void SARAzzlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzrzzST;
        }

        private void Tzlphlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzrzzST;
        }

        private void Uzyqllabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFzrqzST;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        //载入单独切分文件按钮
        private void loadTxtbutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "txt病人单独切分文件(*.txt)|*.txt";
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    loadQieFenData(txtRead(openFile.FileName));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("载入文件格式不正确或不是单独的切分文件");
                }
            }
        }

        //读单独切分文件
        private String txtRead(String path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line="";
            String result = "";
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line);
                result += line;
            }
            sr.Close();
            //Console.WriteLine(result);
            return result;
        }

        //加载切分文件
        private void loadQieFenData(String  str)
        {
            //分析字符串
            String[] s = str.Split(new char[] { ' ' });

            //加载数据
            BRHtextBox.Text = s[0];
            
            QFzzST = Double.Parse(s[1]);
            QFzzET = Double.Parse(s[2]);
            QFzzUT = Double.Parse(s[3]);

            QFszzzLST = Double.Parse(s[4]);
            QFszzzLET = Double.Parse(s[5]);
            QFszzzLUT = Double.Parse(s[6]);

            QFszzzRST = Double.Parse(s[7]);
            QFszzzRET = Double.Parse(s[8]);
            QFszzzRUT = Double.Parse(s[9]);

            QFzbsyLST = Double.Parse(s[10]);
            QFzbsyLET = Double.Parse(s[11]);
            QFzbsyLUT = Double.Parse(s[12]);

            QFzbsyRST = Double.Parse(s[13]);
            QFzbsyRET = Double.Parse(s[14]);
            QFzbsyRUT = Double.Parse(s[15]);

            QFksltLST = Double.Parse(s[16]);
            QFksltLET = Double.Parse(s[17]);
            QFksltLUT = Double.Parse(s[18]);

            QFksltRST = Double.Parse(s[19]);
            QFksltRET = Double.Parse(s[20]);
            QFksltRUT = Double.Parse(s[21]);

            QFsznhLST = Double.Parse(s[22]);
            QFsznhLET = Double.Parse(s[23]);
            QFsznhLUT = Double.Parse(s[24]);

            QFsznhRST = Double.Parse(s[25]);
            QFsznhRET = Double.Parse(s[26]);
            QFsznhRUT = Double.Parse(s[27]);

            QFsbydLST = Double.Parse(s[28]);
            QFsbydLET = Double.Parse(s[29]);
            QFsbydLUT = Double.Parse(s[30]);

            QFsbydRST = Double.Parse(s[31]);
            QFsbydRET = Double.Parse(s[32]);
            QFsbydRUT = Double.Parse(s[33]);

            QFssltLST = Double.Parse(s[34]);
            QFssltLET = Double.Parse(s[35]);
            QFssltLUT = Double.Parse(s[36]);

            QFssltRST = Double.Parse(s[37]);
            QFssltRET = Double.Parse(s[38]);
            QFssltRUT = Double.Parse(s[39]);

            QFxzlhLST = Double.Parse(s[40]);
            QFxzlhLET = Double.Parse(s[41]);
            QFxzlhLUT = Double.Parse(s[42]);

            QFxzlhRST = Double.Parse(s[43]);
            QFxzlhRET = Double.Parse(s[44]);
            QFxzlhRUT = Double.Parse(s[45]);

            QFzrqzST = Double.Parse(s[46]);
            QFzrqzET = Double.Parse(s[47]);
            QFzrqzUT = Double.Parse(s[48]);

            QFbxqqlST = Double.Parse(s[49]);
            QFbxqqlET = Double.Parse(s[50]);
            QFbxqqlUT = Double.Parse(s[51]);

            QFcfqlST = Double.Parse(s[52]);
            QFcfqlET = Double.Parse(s[53]);
            QFcfqlUT = Double.Parse(s[54]);

            QFzrzzST = Double.Parse(s[55]);
            QFzrzzET = Double.Parse(s[56]);
            QFzrzzUT = Double.Parse(s[57]);

            QFblzlST = Double.Parse(s[58]);
            QFblzlET = Double.Parse(s[59]);
            QFblzlUT = Double.Parse(s[60]);

            QFbmzlST = Double.Parse(s[61]);
            QFbmzlET = Double.Parse(s[62]);
            QFbmzlUT = Double.Parse(s[63]);

            QFbclzlST = Double.Parse(s[64]);
            QFbclzlET = Double.Parse(s[65]);
            QFbclzlUT = Double.Parse(s[66]);

            QFclzlST = Double.Parse(s[67]);
            QFclzlET = Double.Parse(s[68]);
            QFclzlUT = Double.Parse(s[69]);

            QFyzbxzST = Double.Parse(s[70]);
            QFyzbxzET = Double.Parse(s[71]);
            QFyzbxzUT = Double.Parse(s[72]);

            QFhlsyST = Double.Parse(s[73]);
            QFhlsyET = Double.Parse(s[74]);
            QFhlsyUT = Double.Parse(s[75]);

            QFzsST = Double.Parse(s[76]);
            QFzsET = Double.Parse(s[77]);
            QFzsUT = Double.Parse(s[78]);

            QFsmbxST = Double.Parse(s[79]);
            QFsmbxET = Double.Parse(s[80]);
            QFsmbxUT = Double.Parse(s[81]);

            QFsmbxfST = Double.Parse(s[82]);
            QFsmbxfET = Double.Parse(s[83]);
            QFsmbxfUT = Double.Parse(s[84]);

            blzlST = QFblzlST;
            blzlET = QFblzlET;
            blzlUT = QFblzlUT;

            bclzlST = QFbclzlST;
            bclzlET = QFbclzlET;
            bclzlUT = QFbclzlUT;

            clzlST = QFclzlST;
            clzlET = QFclzlET;
            clzlUT = QFclzlUT;

            cfqlST = QFcfqlST;
            cfqlET = QFcfqlET;
            cfqlUT = QFcfqlUT;

            //加载SPPB界面
            SPPBblzlSlabel.Text = "开始时间:"+blzlST;
            SPPBbblzlSlabel.Text = "开始时间:"+bclzlST;
            SPPBclzlSlabel.Text = "开始时间:"+clzlST;
            SPPBcfzwqlSlabel.Text = "开始时间:"+cfqlST;

            SPPBblzlElabel.Text = "结束时间:"+blzlET;
            SPPBbblzlElabel.Text = "结束时间:" + bclzlET;
            SPPBclzlElabel.Text = "结束时间:" + clzlET;
            SPPBcfzwqlElabel.Text = "结束时间:" + cfqlET;

            SPPBblzlYlabel.Text = "用时:"+blzlUT;
            SPPBbblzlYlabel.Text = "用时:"+bclzlUT;
            SPPBclzlYlabel.Text = "用时:" + clzlUT;
            SPPBcfzwqlYlabel.Text = "用时:" + cfqlUT;

            //加载切分表界面
            QFzzSTlabel.Text = "开始时间:"+QFzzST;
            QFszzzLSTlabel.Text = "开始时间:"+QFszzzLST;
            QFszzzRSTlabel.Text = "开始时间:" + QFszzzRST;
            QFzbsyLSTlabel.Text = "开始时间:"+QFzbsyLST;
            QFzbsyRSTlabel.Text = "开始时间:" + QFzbsyRST;
            QFksltLlSTlabel.Text = "开始时间:"+QFksltLST;
            QFksltRSTlabel.Text = "开始时间:" + QFksltRST;
            QFsznhLSTlabel.Text = "开始时间:"+QFsznhLST;
            QFsznhRSTlabel.Text = "开始时间:" + QFsznhRST;
            QFsbydLSTlabel.Text = "开始时间:"+QFsbydLST;
            QFsbydRSTlabel.Text = "开始时间:" + QFsbydRST;
            QFssltSTLlabel.Text = "开始时间:"+QFssltLST;
            QFssltRSTlabel.Text = "开始时间:" + QFssltRST;
            QFxzlhLSTlabel.Text = "开始时间:"+QFxzlhLST;
            QFxzlhSTRlabel.Text = "开始时间:"+QFxzlhRST;
            QFzrqzSTlabel.Text = "开始时间:"+QFzrqzST;
            QFbxqqlSTlabel.Text = "开始时间:"+QFbxqqlST;
            QFcfqlSTlabel.Text = "开始时间:"+QFcfqlST;
            QFzrzzSTlabel.Text = "开始时间:"+QFzrzzST;
            QFblzlSTlabel.Text = "开始时间:"+QFblzlST;
            QFbmzlSTlabel.Text = "开始时间:"+QFbmzlST;
            QFbclzlSTlabel.Text = "开始时间:"+QFbclzlST;
            QFclzlSTlabel.Text = "开始时间:"+QFclzlST;
            QFyzbxzSTlabel.Text = "开始时间:"+QFyzbxzST;
            QFhlsySTlabel.Text = "开始时间:"+QFhlsyST;
            QFzsSTlabel.Text = "开始时间:"+QFzsST;
            QFsmbxSTlabel.Text = "开始时间:"+QFsmbxST;
            QFsmbxfSTlabel.Text = "开始时间:"+QFsmbxfST;

            QFzzETlabel.Text = "结束时间:" + QFzzET;
            QFszzzLETlabel.Text = "结束时间:" + QFszzzLET;
            QFszzzRETlabel.Text = "结束时间:" + QFszzzRET;
            QFzbsyLETlabel.Text = "结束时间:" + QFzbsyLET;
            QFzbsyETRlabel.Text = "结束时间:" + QFzbsyRET;
            QFksltLETllabel.Text = "结束时间:" + QFksltLET;
            QFksltRETlabel.Text = "结束时间:" + QFksltRET;
            QFsznhLETlabel.Text = "结束时间:" + QFsznhLET;
            QFsznhRETlabel.Text = "结束时间:" + QFsznhRET;
            QFsbydLETlabel.Text = "结束时间:" + QFsbydLET;
            QFsbydETRlabel.Text = "结束时间:" + QFsbydRET;
            QFssltETLlabel.Text = "结束时间:" + QFssltLET;
            QFssltRETlabel.Text = "结束时间:" + QFssltRET;
            QFxzlhLETlabel.Text = "结束时间:" + QFxzlhLET;
            QFxzlhRETlabel.Text = "结束时间:" + QFxzlhRET;
            QFzrqzETlabel.Text = "结束时间:"+QFzrqzET;
            QFbxqqlETlabel.Text = "结束时间:" + QFbxqqlET;
            QFcfqlETlabel.Text = "结束时间:" + QFcfqlET;
            QFzrzzETlabel.Text = "结束时间:"+QFzrzzET;
            QFblzlETlabel.Text = "结束时间:" + QFblzlET;
            QFbmzlETlabel.Text = "结束时间:" + QFbmzlET;
            QFbclzlETlabel.Text = "结束时间:" + QFbclzlET;
            QFclzlETlabel.Text = "结束时间:" + QFclzlET;
            QFyzbxzETlabel.Text = "结束时间:"+QFyzbxzET;
            QFhlsyETlabel.Text = "结束时间:" + QFhlsyET;
            QFzsETlabel.Text = "结束时间:" + QFzsET;
            QFsmbxETlabel.Text = "结束时间:" + QFsmbxET;
            QFsmbxfETlabel.Text = "结束时间:" + QFsmbxfET;

            QFzzUTlabel.Text = "用时:"+QFzzUT;
            QFszzzLUTlabel.Text = "用时:" + QFszzzLUT;
            QFszzzRUTlabel.Text = "用时:" + QFszzzRUT;
            QFzbsyLUTlabel.Text = "用时:" + QFzbsyLUT;
            QFzbsyUTRlabel.Text = "用时:" + QFzbsyRUT;
            QFksltLlUTlabel.Text = "用时:" + QFksltLUT;
            QFksltRUTlabel.Text = "用时:" + QFksltRUT;
            QFsznhLUTlabel.Text = "用时:" + QFsznhLUT;
            QFsznhRUTlabel.Text = "用时:" + QFsznhRUT;
            QFsbydLUTlabel.Text = "用时:" + QFsbydLUT;
            QFsbydRUTlabel.Text = "用时:" + QFsbydRUT;
            QFssltUTLlabel.Text = "用时:" + QFssltLUT;
            QFssltRUTlabel.Text = "用时:" + QFssltRUT;
            QFxzlhLUTlabel.Text = "用时:" + QFxzlhLUT;
            QFxzlhRUTlabel.Text = "用时:" + QFxzlhRUT;
            QFzrqzUTlabel.Text = "用时:" + QFzrqzUT;
            QFbxqqlUTlabel.Text = "用时:" + QFbxqqlUT;
            QFcfqlUTlabel.Text = "用时:" + QFcfqlUT;
            QFzrzzUTlabel.Text = "用时:" + QFzrzzUT;
            QFblzlUTlabel.Text = "用时:" + QFblzlUT;
            QFbmzlUTlabel.Text = "用时:" + QFbmzlUT;
            QFbclzlUTlabel.Text = "用时:" + QFbclzlUT;
            QFclzlUTlabel.Text = "用时:" + QFclzlUT;
            QFyzbxzUTlabel.Text = "用时:" + QFyzbxzUT;
            QFhlsyUTlabel.Text = "用时:" + QFhlsyUT;
            QFzsUTlabel.Text = "用时:" + QFzsUT;
            QFsmbxUTlabel.Text = "用时:" + QFsmbxUT;
            QFsmbxfUTlabel.Text = "用时:" + QFsmbxfUT;
        }

        //载入窗体时
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Counttimer2.Start();
            //记录窗体大小
            //asc.controllInitializeSize(this);
            //asc.controlAutoSize(this);
        }

        //当窗口大小改变时
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //改变窗体大小
            //asc.controlAutoSize(this);
        }

        //开始倒计时
        private void countTimebutton_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.play();
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsznhLST = Math.Round(StartTime, 2);
            QFsznhLSTlabel.Text = "开始时间:" + QFsznhLST;
            this.Counttimer.Start();
            timeCount = 0;
            this.countTimebutton.Enabled = false;
           
        }

        //5秒倒计时接收器
        private void Counttimer_Tick(object sender, EventArgs e)
        {
            
            timeCount++;
            if (timeCount < 5)
            {
                
            }
            else
            {
             
                this.Counttimer.Stop();
                this.axWindowsMediaPlayer1.Ctlcontrols.pause();
                timeCount = 0;
                this.countTimebutton.Enabled = true;
                EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
                QFsznhLET = Math.Round(EndTime, 2);
                QFsznhLETlabel.Text = "结束时间:" + QFsznhLET;

                QFsznhLUT = Math.Round(QFsznhLET - QFsznhLST, 2);
                QFsznhLUTlabel.Text = "用时:" + QFsznhLUT;
            }

        }

        private void SPPBxzsdlabel_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition = QFsmbxST;
        }

        private void countTimebutton1_Click(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.play();
            StartTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            QFsznhRST = Math.Round(StartTime, 2);
            QFsznhRSTlabel.Text = "开始时间:" + QFsznhRST;
            this.Counttimer1.Start();
            timeCount1 = 0;
            this.countTimebutton1.Enabled = false;
        }

        private void Counttimer1_Tick(object sender, EventArgs e)
        {
            timeCount1++;
            if (timeCount1 < 5)
            {

            }
            else
            {

                this.Counttimer1.Stop();
                this.axWindowsMediaPlayer1.Ctlcontrols.pause();
                timeCount1 = 0;
                this.countTimebutton1.Enabled = true;
                EndTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
                QFsznhRET = Math.Round(EndTime, 2);
                QFsznhRETlabel.Text = "结束时间:" + QFsznhRET;

                QFsznhRUT = Math.Round(QFsznhRET - QFsznhRST, 2);
                QFsznhRUTlabel.Text = "用时:" + QFsznhRUT;
            }
        }

        private void savetxtbutton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "txt病人单独切分文件(*.txt)|*.txt";
            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    AddPersonalTxtWrite(saveFile.FileName);
                    //loadQieFenData(txtRead(saveFile.FileName));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                   // MessageBox.Show("载入文件格式不正确或不是单独的切分文件");
                }
            }
        }

        private void Counttimer2_Tick(object sender, EventArgs e)
        {
            if (!QieFenFsavePath.Equals(""))
            {
                if(!BRHtextBox.Text.ToString().Equals(""))
                    AddPersonalTxtWrite(QieFenFsavePath + "/" + BRHtextBox.Text + ".txt");
                else
                    AddPersonalTxtWrite(QieFenFsavePath + "/" + "oldVesionQF" + ".txt");
            }
            else
            {
            
            }
                
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Counttimer2.Stop();
        }

       
    }
}
