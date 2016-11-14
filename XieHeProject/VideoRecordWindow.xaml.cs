using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Media;
using System.Windows.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.POIFS.FileSystem;







namespace XieHeProject
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        //彩色图图像
        private WriteableBitmap colorBitmap = null;
        
        //Kinect传感器
        private KinectSensor kinectSensor = null;

        
        //彩色图Reader
        private ColorFrameReader colorFrameReader = null;

        //获取彩色图像
        public ImageSource ColorImageSource
        {
            get
            {
                return this.colorBitmap;
            }
            
        }

        /// <summary>
        /// 图像的宽度
        /// </summary>
        private int _displayWidth;

        /// <summary>
        /// 图像的高度
        /// </summary>
        private int _displayHeight;

        /// <summary>
        /// Avi录制类
        /// </summary>
        private static SharpAvi _aviWriter = new SharpAvi();

        private static KinectVideo _kVideo = new KinectVideo();

        /// <summary>
        /// 彩色帧数据
        /// </summary>
        byte[] _frameData = new byte[1920 * 1080 * 4];



        /// <summary>
        /// 绘制骨头或者彩色图有关数据
        /// </summary>
        #region
        /// //手的尺寸
        private const double HandSize = 80;

        //骨骼的厚度
        private const double JointThickness = 20;

        //摄像机边缘的厚度，用于提示是否碰到了摄像机边缘
        private const double ClipBoundsThickness = 10;

        //摄像空间轴
        private const float InferredZPositionClamp = 0.1f;


        //表示手状态的刷子
        private readonly System.Windows.Media.Brush handClosedBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(128, 255, 0, 0));
        private readonly System.Windows.Media.Brush handOpenBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(128, 0, 255, 0));
        private readonly System.Windows.Media.Brush handLassoBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(128, 0, 0, 255));

        //画骨骼的刷子
        private readonly System.Windows.Media.Brush trackedJointBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 68, 192, 68));
        private readonly System.Windows.Media.Brush inferredJointBrush = System.Windows.Media.Brushes.Yellow;

        //画骨骼的笔
        private readonly System.Windows.Media.Pen inferredBonePen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Gray, 1);

        //单个绘图进行运算的绘图集合
        private DrawingGroup drawingGroup;

        //绘画出来的图像
        private DrawingImage imageSource = null;

        

        //坐标转换
        private CoordinateMapper coordinateMapper = null;


        //身体Reader
        private BodyFrameReader bodyFrameReader = null;


        //一共6人的身体
        private Body[] bodies = null;

        //人体的骨额
        private List<Tuple<JointType, JointType>> bones;

        //图像的宽度
        private int displayWidth;

        //图像的高度
        private int displayHeight;

        //画身体的笔
        private List<System.Windows.Media.Pen> bodyColors;

  
        //获取骨骼图像
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
            set
            {
                this.imageSource = (DrawingImage)value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                }
            }
        }

        //事件绑定
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// 步态测试类
        /// </summary>
        private BuTai butai = new BuTai();


        /// <summary>
        /// 基本信息表保存位置
        /// </summary>
        String savePath = "../../../../基本信息表.xls";

        /// <summary>
        /// 计时器
        /// </summary>
        DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// 是否写入
        /// </summary>
        int isInWrite = 0;

        /// <summary>
        /// 构造器
        /// </summary>
        public MainWindow()
        {

            ///彩色图相关
            //取得传感器
            this.kinectSensor = KinectSensor.GetDefault();

            //获取彩色图像信息
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            //打开彩色图Reader
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            
            //彩色图的空间坐标
            this._displayWidth = colorFrameDescription.Width;
            this._displayHeight = colorFrameDescription.Height;

            this.DataContext = this;

            //创建位图
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);


            //取得坐标转换器
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;


            ///骨骼相关
            //打开身体Reader
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            //获得骨骼空间坐标
            this.displayWidth = colorFrameDescription.Width;
            this.displayHeight = colorFrameDescription.Height;

            //元组链表作为骨骼
            this.bones = new List<Tuple<JointType, JointType>>();

            // 躯干
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // 右臂
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // 左臂
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // 右腿
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // 左腿
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));



            // 每个人身体的颜色
            this.bodyColors = new List<System.Windows.Media.Pen>();

            this.bodyColors.Add(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Red, 15));
            this.bodyColors.Add(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Orange, 15));
            this.bodyColors.Add(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Green, 15));
            this.bodyColors.Add(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Blue, 15));
            this.bodyColors.Add(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Indigo, 15));
            this.bodyColors.Add(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Violet, 15));

            //打开传感器
            this.kinectSensor.Open();

            //创建绘图和绑定窗体
            this.drawingGroup = new DrawingGroup();
            //this.imageSource = new DrawingImage(this.drawingGroup);
            this.DataContext = this;
            //this.ImageSource = null;

            this.timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += timer_Tick;
            timer.Start();
            InitializeComponent();
            
          }

        /// <summary>
        /// 窗体载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //添加身体Reader事件
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += bodyFrameReader_FrameArrived;
            }
           
            //添加彩色图Reader事件
            if (this.colorFrameReader != null)
            {
                this.colorFrameReader.FrameArrived += colorFrameReader_FrameArrived;
            }
            //步态按钮状态
            this.BuTaiStartButton.IsEnabled = true;
            this.BuTaiStopButton.IsEnabled = false;

            //下肢灵活度状态
            this.LowerLimbStartButton.IsEnabled = true;
            this.LowerLimbEndButton.IsEnabled = false;

            this.BingRenIdTextBox.Focus();
        }

        DateTime d1=DateTime.Now;
        DateTime d2=DateTime.Now;

        //掉帧数和总帧数
        static double totalframe = 1;
        static double wrongframe = 0;

        /// <summary>
        /// 彩色图回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void colorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            //isInWrite = 0;
            //timer.Start();
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                d1 = DateTime.Now;
                TimeSpan span1 = (TimeSpan)(d1 - d2);
                this.TimerLabel2.Content = "进函数:" + span1.Milliseconds;
                if (colorFrame != null)
                {
                    //获取彩色坐标信息
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    //绘制彩色图
                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        //分析数据将新的彩色图数据写入位图
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {

                            //判断是初始化avi文件
                            if (SharpAvi.IsCreateRecord)
                            {
                                _aviWriter.InitRecord(BingRen.PeopleDir + "\\" + BingRen.PeopleId + DateTime.Now.ToString("_yyMMdd_HHmmss", DateTimeFormatInfo.InvariantInfo) + ".avi");
                            }
                            //判断是否进行avi文件录制
                            if (SharpAvi.IsRecording)
                            {
                               
                                colorFrame.CopyConvertedFrameDataToArray(_frameData, ColorImageFormat.Bgra);
                                //镜面翻转
                                for (int i = 0; i < _frameData.Length; i = i + 1920 * 4)
                                    MyArray.Reverse(_frameData, i, 1920 * 4);
                                _aviWriter.Recording(_frameData);

                            }
                            //判断是否结束avi文件录制
                            if (SharpAvi.IsStopRecord)
                            {
                               _aviWriter.Stoping();
                               this.DiaoZenLabel.Content = "掉帧提示";
                            }

                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }

                        this.colorBitmap.Unlock();
                       
                        d2 = DateTime.Now;
                        TimeSpan span2 = (TimeSpan)(d2 - d1);
                       
                        this.TimerLabel.Content ="函数内:"+span2.Milliseconds;

                        if ((span1.Milliseconds+span2.Milliseconds)>50)
                        {
                            wrongframe++;
                        }
                        totalframe++;
                    }
                }
            }
        }

        /// <summary>
        /// 计时器函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            if ((wrongframe / totalframe) <= 0.15)
            {
                this.DiaoZenLabel.Content = "不掉帧";
            }
            else
            {
                this.DiaoZenLabel.Content = "掉帧";
            }
            totalframe = 1;
            wrongframe = 0;
        }

         /// <summary>
         /// 窗体关闭事件
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //关闭Reader和传感器
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }
            if (this.colorFrameReader != null)
            {
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
            if (this.timer != null)
            {
                timer.Tick -= timer_Tick;
                this.timer.Stop();
                this.timer = null;
            }
        }

        /// <summary>
        /// 开始视频录制
        /// </summary>
        private void StartRecord()
        {
            //xef
            Thread mTcpServerThread = new Thread(_kVideo.StartRecordClip);
            mTcpServerThread.IsBackground = true;
            mTcpServerThread.Start(BingRen.PeopleDir + "\\" + BingRen.PeopleId + DateTime.Now.ToString("_yyMMdd_HHmmss", DateTimeFormatInfo.InvariantInfo) + ".xef");

            PlaySound();
        }

        /// <summary>
        /// 结束视频录制
        /// </summary>
        private void StopRecord()
        {
            //xef
            Thread mTcpServerThread = new Thread(_kVideo.StopRecordClip);
            mTcpServerThread.IsBackground = true;
            mTcpServerThread.Start();

            PlaySound();
            Thread.Sleep(1000);
            PlaySound();
        }

        /// <summary>
        /// 键盘相应事件处理各种消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
             //处理开始录制事件(avi和xef)
            if (Keyboard.IsKeyDown(Key.D1) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                StartRecord();
            }
            //处理结束录制事件(avi和xef)
            else if (Keyboard.IsKeyDown(Key.D2) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                StopRecord();
            }
            //启动处理命令线程
            else if (Keyboard.IsKeyDown(Key.D3) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Thread mTcpServerThread = new Thread(PhoneConnection.dealCommand);
                mTcpServerThread.IsBackground = true;
                mTcpServerThread.Start();
            }
            else if (Keyboard.IsKeyDown(Key.F1))
            {
                if (BingRen.PeopleDir.Equals("") || BingRen.PeopleId.Equals(""))
                {
                    System.Windows.MessageBox.Show("请输入病人病号或请选择存储目录");
                    return;
                }
                if (this.VideoPlayButton.Tag.Equals("1"))
                {
                    StartRecord();
                    this.RecordStateLabel.Content = "录制中...";
                    this.VideoPlayButton.Content = "结束录制";
                    this.VideoPlayButton.Tag = "2";
                }
                
            }
            else if (Keyboard.IsKeyDown(Key.F2))
            {
                if (this.VideoPlayButton.Tag.Equals("2"))
                {
                    StopRecord();
                    this.RecordStateLabel.Content = "未开始录制";
                    this.VideoPlayButton.Content = "开始录制";
                    this.VideoPlayButton.Tag = "1";
                }
            }
            else if (Keyboard.IsKeyDown(Key.Enter))
            {
                if (BingRen.RootDir.Equals(""))
                {
                    this.BingRenIdTextBox.Focus();
                    System.Windows.MessageBox.Show("请选择存储目录");
                    return;
                }
                this.BingRenIdTextBox.Focus();
                BingRen.PeopleId = this.BingRenIdTextBox.Text.ToString();
                PhoneConnection.CreatePeopleDir();
                this.BingRenIdTextBox.Text= "";
                this.BingRenIdLabel.Content = "病号:" + BingRen.PeopleId;
            }
            else if (Keyboard.IsKeyDown(Key.F3))
            {
                if (this.BuTaiStartButton.IsEnabled == true)
                    this.BuTaiStartButton_Click(sender, e);
            }
            else if (Keyboard.IsKeyDown(Key.F4))
            {
                if (this.BuTaiStopButton.IsEnabled == true)
                    this.BuTaiStopButton_Click(sender, e);
            }
        }

       ///手机端创建服务器(已弃用)
       #region
         /// <summary>
        /// 创建服务器(医院说不方便，已弃用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (BingRen.RootDir == "")
            {
                System.Windows.MessageBox.Show("请选择存储目录");
                this.BingRenIdTextBox.Focus();
                return;
            }

            //已弃用

            ////最开始改变接收服务按钮的状态方否则按钮改变状态失效
            //PhoneConnection.IsReceivingMsg = true;
            ////连接服务器，接收指令
            //Thread mTcpServerThread = new Thread(PhoneConnection.InitConnection);
            //mTcpServerThread.IsBackground = true;
            //mTcpServerThread.Start();
            //ChangeChangeButtonState(false);

            ////控制按钮是否可用
            //Thread mTcpServerThread1 = new Thread(ReturnCreatServiceButtonStatic);
            //mTcpServerThread1.IsBackground = true;
            //mTcpServerThread1.Start();

            //this.BingRenIdTextBox.Focus();
        }


        /// <summary>
        /// 还原创建按钮状态(已弃用)
        /// </summary>
        private void ReturnCreatServiceButtonStatic()
        {
            //while (PhoneConnection.IsReceivingMsg==true)
            //{

            //}
            //ChangeChangeButtonState(true);
        }

        
        /// <summary>
        /// 改变创建按钮状态(已弃用)
        /// </summary>
        /// <param name="state">状态布尔值</param>
        private void ChangeChangeButtonState(bool state)
        {
           // this.Dispatcher.BeginInvoke(new Action(() => this.CreatServerButton.IsEnabled = state));
        }
        #endregion

        /// <summary>
        /// 选择保存目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetDirButton_Click(object sender, RoutedEventArgs e)
        {
            PhoneConnection.SetStoreDir();
            this.BingRenIdTextBox.Focus();
        }

        /// <summary>
        /// 播放提示音
        /// </summary>
        private void PlaySound()
        {
            SoundPlayer soundPlayer = new SoundPlayer(@"..\..\..\Resources\Audio\sound.wav");
            soundPlayer.Play();
        }

        /// <summary>
        /// 播放提示音二
        /// </summary>
        private void PlaySoundTwo()
        {
            SoundPlayer soundPlayer = new SoundPlayer(@"..\..\..\Resources\Audio\sound2.wav");
            soundPlayer.Play();
        }

        /// <summary>
        /// 播放提示音二
        /// </summary>
        private void PlaySoundThree()
        {
            SoundPlayer soundPlayer = new SoundPlayer(@"..\..\..\Resources\Audio\sound3.wav");
            soundPlayer.Play();
        }

        /// <summary>
        /// 身体回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            //判断是否检测到身体数据
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    //判断是否有增加的人数
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }

                if (dataReceived)
                {
                    using (DrawingContext dc = this.drawingGroup.Open())
                    {
                        // Draw a transparent background to set the render size
                        dc.DrawRectangle(System.Windows.Media.Brushes.Transparent, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                        int penIndex = 0;
                        foreach (Body body in this.bodies)
                        {
                            System.Windows.Media.Pen drawPen = this.bodyColors[penIndex];

                            if (body.IsTracked)
                            {
                                this.DrawClippedEdges(body, dc);

                                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                                // convert the joint points to depth (display) space
                                Dictionary<JointType, System.Windows.Point> jointPoints = new Dictionary<JointType, System.Windows.Point>();


                                //坐标转换，骨骼转彩色
                                foreach (JointType jointType in joints.Keys)
                                {
                                    // sometimes the depth(Z) of an inferred joint may show as negative
                                    // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                    CameraSpacePoint position = joints[jointType].Position;
                                    if (position.Z < 0)
                                    {
                                        position.Z = InferredZPositionClamp;
                                    }

                                    ColorSpacePoint colorSpacePoint = this.coordinateMapper.MapCameraPointToColorSpace(position);
                                    jointPoints[jointType] = new System.Windows.Point(colorSpacePoint.X, colorSpacePoint.Y);
                                    //DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                    //jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                                }

                                if (this.HandsState(body.HandLeftState, body.HandRightState))
                                {
                                    this.DrawLabelText(jointPoints[JointType.Head], dc, "测试个体");

                                    //判断是否开始收集步态数据
                                    if (!this.BuTaiStartButton.IsEnabled)
                                    {
                                        changBuTaiLabels(body, bodyFrame);
                                        BuTai.CollectStepDistance(body);
                                        BuTai.CollectValueZ(body, bodyFrame);
                                        BuTai.CollectFootValues(body, bodyFrame);
                                    }


                                    //判断是否开始收集下肢灵活数据
                                    if (!this.LowerLimbStartButton.IsEnabled)
                                        LowerLimb.CollectFootHeight(body, bodyFrame);
                                    
                                }
                                //whether to start to collect Normal Stand Up data
                                if (!this.NSUStartButton.IsEnabled)
                                    NormalStandUp.CollectNSUheight(body, bodyFrame);
                                //whether to start to collect Cross Stand Up data
                                if (!this.CSUStartButton.IsEnabled)
                                    CrossStandUp.CollectCSUheight(body, bodyFrame);

                                //画身体
                                this.DrawBody(joints, jointPoints, dc, drawPen);
                                //画左手
                                this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                                //画右手
                                this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                                //画标号
                                //this.DrawLabelText(jointPoints[JointType.Head], dc,penIndex++);
                                //patientIndex++;
                            }

                        }

                        if (this.PatientIndexComBox.SelectedIndex == -1)
                        {
                            /**
                            //判断是否开始收集步态数据
                            if (!this.BuTaiStartButton.IsEnabled)
                                changBuTaiLabels(this.bodies[0], bodyFrame);

                            //判断是否开始收集下肢灵活数据
                            if (!this.LowerLimbStartButton.IsEnabled)
                                LowerLimb.CollectFootHeight(this.bodies[0], bodyFrame);
                             * **/
                        }
                        else
                        {
                            /*
                            //判断是否开始收集步态数据
                            if (!this.BuTaiStartButton.IsEnabled)
                                changBuTaiLabels(this.bodies[this.PatientIndexComBox.SelectedIndex], bodyFrame);
                            //Console.WriteLine(this.PatientIndexComBox.SelectedIndex);

                            //判断是否开始收集下肢灵活数据
                            if (!this.LowerLimbStartButton.IsEnabled)
                                LowerLimb.CollectFootHeight(this.bodies[this.PatientIndexComBox.SelectedIndex], bodyFrame);
                             * */
                        }

                        // prevent drawing outside of our render area
                        this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                    }
                }
            }


        }

       /// <summary>
       /// 画病人标号
       /// </summary>
       /// <param name="position">绘制位置</param>
       /// <param name="drawingContext">绘制环境</param>
       /// <param name="number">序号</param>
       private void DrawLabelText(System.Windows.Point position, DrawingContext drawingContext,String number)
       {
           drawingContext.DrawText(new FormattedText(number.ToString(),CultureInfo.GetCultureInfo("en-us"),
                FlowDirection,new Typeface("Verdana"),39, System.Windows.Media.Brushes.Red),
          position);
       }

        /// <summary>
        /// 绘制身体
        /// </summary>
        /// <param name="joints"></param>
        /// <param name="jointPoints"></param>
        /// <param name="drawingContext"></param>
        /// <param name="drawingPen"></param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, System.Windows.Point> jointPoints, DrawingContext drawingContext, System.Windows.Media.Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                System.Windows.Media.Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }


        /// <summary>
        /// 绘制骨头
        /// </summary>
        /// <param name="joints"></param>
        /// <param name="jointPoints"></param>
        /// <param name="jointType0"></param>
        /// <param name="jointType1"></param>
        /// <param name="drawingContext"></param>
        /// <param name="drawingPen"></param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, System.Windows.Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, System.Windows.Media.Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            System.Windows.Media.Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        
        /// <summary>
        /// 绘制手的状态
        /// </summary>
        /// <param name="handState"></param>
        /// <param name="handPosition"></param>
        /// <param name="drawingContext"></param>
        private void DrawHand(HandState handState, System.Windows.Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

        /// <summary>
        /// 绘制摄像头边框
        /// </summary>
        /// <param name="body"></param>
        /// <param name="drawingContext"></param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    System.Windows.Media.Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    System.Windows.Media.Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    System.Windows.Media.Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    System.Windows.Media.Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }


        /// <summary>
        /// 返回结果状态
        /// </summary>
        /// <param name="leftHandState">左手状态</param>
        /// <param name="rightHandState">右手状态</param>
        /// <returns></returns>
        private bool HandsState(HandState leftHandState, HandState rightHandState)
        {
            return leftHandState.Equals(HandState.Open) && rightHandState.Equals(HandState.Open);
        }

        /// <summary>
        /// 实时显示步态参数
        /// </summary>
        /// <param name="body">当前人体</param>
        /// <param name="fe">当前骨骼帧</param>
        public void changBuTaiLabels(Body body, BodyFrame fe)
        {
            //取得6个骨骼点坐标和时间
            Queue<Object> e = butai.getBuTaiData(body);
            CameraSpacePoint bonePoint1 = (CameraSpacePoint)e.Dequeue();
            CameraSpacePoint bonePoint2 = (CameraSpacePoint)e.Dequeue();
            CameraSpacePoint bonePoint3 = (CameraSpacePoint)e.Dequeue();
            CameraSpacePoint bonePoint4 = (CameraSpacePoint)e.Dequeue();
            CameraSpacePoint bonePoint5 = (CameraSpacePoint)e.Dequeue();
            CameraSpacePoint bonePoint6 = (CameraSpacePoint)e.Dequeue();
            DateTime dt = (DateTime)e.Dequeue();
            TimeSpan tp = (TimeSpan)e.Dequeue();

            //显示实时左步长和右步长
            double StepLength = bonePoint1.Z - bonePoint2.Z;
            double leftstepLength = 0;
            double rightstepLength = 0;

            if (StepLength > 0)
            {
                this.RightStepLengthLabel.Content = "右步长:" + Math.Round(Math.Abs(StepLength), 4);
                rightstepLength = StepLength;
                leftstepLength = 0;
            }
            else
            {
                this.LeftStepLengthLabel.Content = "左步长:" + Math.Round(Math.Abs(StepLength), 4);
                leftstepLength = StepLength;
                rightstepLength = 0;
            }

            //实时显示左步高和右步高
            double leftstepHeight = BuTai.distancetoFloor(bonePoint3, fe);
            double rightstepHeight = BuTai.distancetoFloor(bonePoint4, fe);

            this.LeftStepHeightLabel.Content = "左步高:" + Math.Round(leftstepHeight, 4);
            this.RightStepHeightLabel.Content = "右步高:" + Math.Round(rightstepHeight, 4);

            //实时显示步宽
            //double stepwidth = bonePoint5.Y;
            double stepwidth = Math.Sqrt(Math.Abs(bonePoint1.X - bonePoint2.X));
            this.StepWidthLabel.Content = "步宽:" + Math.Round(stepwidth, 4);

            //将结果储存起来
            butai.setResultData(stepwidth, leftstepLength, rightstepLength, leftstepHeight, rightstepHeight, dt);
            butai.setElementData(bonePoint1, bonePoint2, bonePoint3, bonePoint4, bonePoint5, bonePoint6);

        }

        /// <summary>
        /// 步态参数开始计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuTaiStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (BingRen.PeopleId.Equals(""))
            {
                System.Windows.MessageBox.Show("输入病人号");
            }
            else
            {
                //改变两个按钮的状态
                this.BuTaiStartButton.IsEnabled = !this.BuTaiStartButton.IsEnabled;
                this.BuTaiStopButton.IsEnabled = !this.BuTaiStopButton.IsEnabled;
                this.BingRenIdTextBox.Focus();
                PlaySoundTwo();
            }
        }

        /// <summary>
        /// 步态参加结束计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuTaiStopButton_Click(object sender, RoutedEventArgs e)
        {
            //改变两个按钮的状态
            this.BuTaiStartButton.IsEnabled = !this.BuTaiStartButton.IsEnabled;
            this.BuTaiStopButton.IsEnabled = !this.BuTaiStopButton.IsEnabled;
            this.BingRenIdTextBox.Focus();

            //计算步态
            try
            {
                Thread t1 = new Thread(butai.calResult);
                t1.Start();
                //System.Windows.MessageBox.Show("步态报告生成完成");
            }
            catch
            {
                //System.Windows.MessageBox.Show("没有收集到骨骼数据或骨骼数据不完整");
            }

            PlaySoundThree();
            //Thread.Sleep(1000);
            //PlaySoundTwo();
        }

        /// <summary>
        /// 控制录像按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoPlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.BingRenIdTextBox.Focus();

            if (VideoPlayButton.Tag .ToString()== "1")
            {
              
                System.Windows.Forms.SendKeys.SendWait("{F1}");
                //this.RecordStateLabel.Content = "录制中......";
            }
            else if (VideoPlayButton.Tag.ToString() == "2")
            {
               
                //this.RecordStateLabel.Content = "未开始录制";
                System.Windows.Forms.SendKeys.SendWait("{F2}");
            }
        }


        /// <summary>
        /// 控制有无骨骼按钮(有勾)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkeletonShow_Checked(object sender, RoutedEventArgs e)
        {
            this.BingRenIdTextBox.Focus();
            ImageSource = new DrawingImage(this.drawingGroup);
            //Console.WriteLine("有图");
        }

        /// <summary>
        /// 控制有无骨骼按钮(无勾)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkeletonShow_Unchecked(object sender, RoutedEventArgs e)
        {
            this.BingRenIdTextBox.Focus();
            ImageSource = null;
            //Console.WriteLine("没有图");
        }

        /// <summary>
        /// 弃用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 输写前三项报告
        /// </summary>
        /// <param name="savePath">文件保存路径</param>
        private void AddPointText(String savePath)
        {
            //FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs);
            //String s1 = "构音不良"+"  "+this.GouYin.SelectedIndex;
            //String s2 = "僵硬" + "  " + this.JiangYing_One.SelectedIndex + "  " + this.JiangYing_Two.SelectedIndex;
            //String s3 = "跟膝胫实验" + "  " + this.GenXiJin_One.SelectedIndex + "  " + this.GenXiJin_Two.SelectedIndex;
            
            //sw.WriteLine(s1.ToString());
            //sw.WriteLine(s2.ToString());
            //sw.WriteLine(s3.ToString());

            //sw.Close();
            //fs.Close();
        }

        /// <summary>
        /// 清除前三项数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_FirstThree_Click(object sender, RoutedEventArgs e)
        {
            this.GouYin.SelectedIndex = -1;
            this.JiangYing_One.SelectedIndex = -1;
            this.JiangYing_Two.SelectedIndex = -1;
            this.GenXiJin_One.SelectedIndex = -1;
            this.GenXiJin_Two.SelectedIndex = -1;

            this.Height.Text = "";
            this.Weight.Text = "";

            this.LS01L.SelectedIndex = -1;
            this.LS01R.SelectedIndex = -1;

            this.LS02L.SelectedIndex = -1;
            this.LS02R.SelectedIndex = -1;

            this.LS03L.SelectedIndex = -1;
            this.LS03R.SelectedIndex = -1;

            this.LS04L.SelectedIndex = -1;
            this.LS04R.SelectedIndex = -1;

            this.LS05L.SelectedIndex = -1;
            this.LS05R.SelectedIndex = -1;

            this.LS06L.SelectedIndex = -1;
            this.LS06R.SelectedIndex = -1;

            this.LS07L.SelectedIndex = -1;
            this.LS07R.SelectedIndex = -1;

            this.LS08L.SelectedIndex = -1;
            this.LS08R.SelectedIndex = -1;

            this.LS09L.SelectedIndex = -1;
            this.LS09R.SelectedIndex = -1;

            this.LS10L.SelectedIndex = -1;
            this.LS10R.SelectedIndex = -1;
        }

        /// <summary>
        /// 生成前三项报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create_FirstThree_Click(object sender, RoutedEventArgs e)
        {
            if (BingRen.PeopleId.Equals(""))
            {
                System.Windows.MessageBox.Show("请输入病人病号");
                return;
            }
            else
            {
                if (!FirstThreeIsVaild())
                {
                    System.Windows.MessageBox.Show("前三项未填完");
                    return;
                }
                if (!LSisFill())
                {
                    System.Windows.MessageBox.Show("利手检测未填完");
                    return;
                }
                if (!LSisVail())
                {
                    System.Windows.MessageBox.Show("利手检测填写未填写规范");
                    return;
                }

                String result = ResultOfLS();

                try
                {
                    if (!File.Exists(savePath))
                    {
                        createExcel();
                    }
                    addExcel(result);
                    System.Windows.MessageBox.Show("报告已生成");
                }
                catch
                {
                    System.Windows.MessageBox.Show("生成报告出错");
                }

            }
        }

        /// <summary>
        /// 检查利手表是否填好
        /// </summary>
        /// <returns>是否填好</returns>
        bool LSisFill()
        {
            if (this.Height.Text.Equals("") || this.Weight.Text.Equals(""))
                return false;
            if (this.LS01L.SelectedIndex == -1 || this.LS01R.SelectedIndex == -1 ||
                this.LS02L.SelectedIndex == -1 || this.LS02R.SelectedIndex == -1||
                this.LS03L.SelectedIndex==-1||this.LS03R.SelectedIndex==-1||
                this.LS04L.SelectedIndex==-1||this.LS04R.SelectedIndex==-1||
                this.LS05L.SelectedIndex==-1||this.LS05R.SelectedIndex==-1||
                this.LS06L.SelectedIndex==-1||this.LS06R.SelectedIndex==-1||
                this.LS07L.SelectedIndex==-1||this.LS07R.SelectedIndex==-1||
                this.LS08L.SelectedIndex==-1||this.LS08R.SelectedIndex==-1||
                this.LS09L.SelectedIndex==-1||this.LS09R.SelectedIndex==-1||
                this.LS10L.SelectedIndex==-1||this.LS10R.SelectedIndex==-1)
                    return false;
            return true;
        }

        /// <summary>
        /// 判断利手表是否符合要求
        /// </summary>
        /// <returns>是否符合要求</returns>
        bool LSisVail()
        {
            if (!(Math.Abs(LS01L.SelectedIndex - LS01R.SelectedIndex) == 2 || (LS01L.SelectedIndex == 1 && LS01R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS02L.SelectedIndex - LS02R.SelectedIndex) == 2 || (LS02L.SelectedIndex == 1 && LS02R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS03L.SelectedIndex - LS03R.SelectedIndex) == 2 || (LS03L.SelectedIndex == 1 && LS03R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS04L.SelectedIndex - LS04R.SelectedIndex) == 2 || (LS04L.SelectedIndex == 1 && LS04R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS05L.SelectedIndex - LS05R.SelectedIndex) == 2 || (LS05L.SelectedIndex == 1 && LS05R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS06L.SelectedIndex - LS06R.SelectedIndex) == 2 || (LS06L.SelectedIndex == 1 && LS06R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS07L.SelectedIndex - LS07R.SelectedIndex) == 2 || (LS07L.SelectedIndex == 1 && LS07R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS08L.SelectedIndex - LS08R.SelectedIndex) == 2 || (LS08L.SelectedIndex == 1 && LS08R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS09L.SelectedIndex - LS09R.SelectedIndex) == 2 || (LS09L.SelectedIndex == 1 && LS09R.SelectedIndex == 1)))
                return false;
            if (!(Math.Abs(LS10L.SelectedIndex - LS10R.SelectedIndex) == 2 || (LS10L.SelectedIndex == 1 && LS10R.SelectedIndex == 1)))
                return false;
            return true;
        }

        /// <summary>
        /// 前三项表是否填好
        /// </summary>
        /// <returns>是否填好</returns>
        bool FirstThreeIsVaild()
        {
            return (this.GouYin.SelectedIndex != -1 && this.JiangYing_One.SelectedIndex != -1 && this.JiangYing_Two.SelectedIndex != -1
                && this.GenXiJin_One.SelectedIndex != -1 && this.GenXiJin_Two.SelectedIndex != -1);
        }

        /// <summary>
        /// 计算利手表结果
        /// </summary>
        /// <returns>利手表结果</returns>
        private String ResultOfLS()
        {
            String result = "";
            int LH = this.LS01L.SelectedIndex + this.LS02L.SelectedIndex +
            this.LS03L.SelectedIndex + this.LS04L.SelectedIndex +
            this.LS05L.SelectedIndex + this.LS06L.SelectedIndex +
            this.LS07L.SelectedIndex + this.LS08L.SelectedIndex +
            this.LS09L.SelectedIndex + this.LS10L.SelectedIndex;

            int  RH = this.LS01R.SelectedIndex + this.LS02R.SelectedIndex +
            this.LS03R.SelectedIndex + this.LS04R.SelectedIndex +
            this.LS05R.SelectedIndex + this.LS06R.SelectedIndex +
            this.LS07R.SelectedIndex + this.LS08R.SelectedIndex +
            this.LS09R.SelectedIndex + this.LS10R.SelectedIndex;

            float CT = LH + RH;
            float D = RH - LH;
            float R1 = D / CT;
            float R = R1 * 100;
            if (R < -40)
            {
                result = "左手";            
            }
            else if (R>=-40&&R <= 40)
            {
                result = "两手";
            }
            else
            {
                result = "右手";
            }

            return result;
        }

        /// <summary>
        /// 创建表头
        /// </summary>
        private void createExcel()
        {
            FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            HSSFWorkbook wk = new HSSFWorkbook();
            ISheet tb = wk.CreateSheet("Sheet01");
            IRow row = tb.CreateRow(0);
            row.CreateCell(0).SetCellValue("病人ID");
            row.CreateCell(1).SetCellValue("构音不良");
            row.CreateCell(2).SetCellValue("僵硬(左)");
            row.CreateCell(3).SetCellValue("僵硬(右)");
            row.CreateCell(4).SetCellValue("跟膝胫实验(左)");
            row.CreateCell(5).SetCellValue("跟膝胫实验(右)");
            row.CreateCell(6).SetCellValue("身高(cm)");
            row.CreateCell(7).SetCellValue("体重(kg)");
            row.CreateCell(8).SetCellValue("利手量表");
            row.CreateCell(9).SetCellValue("日期");
            wk.Write(fs);
            wk.Close();
            fs.Close();
        }


        

        /// <summary>
        /// 写excel表结果
        /// </summary>
        /// <param name="LS">利手表结果</param>
        private void addExcel(String LS)
        {
            FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            POIFSFileSystem ps = new POIFSFileSystem(fs);
            HSSFWorkbook wk = new HSSFWorkbook(ps);
            ISheet tb = wk.GetSheet("Sheet01");
            int number = tb.LastRowNum;
            FileStream fs1 = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            IRow row1 = tb.CreateRow(number + 1);
            row1.CreateCell(0).SetCellValue(BingRen.PeopleId);
            row1.CreateCell(1).SetCellValue(this.GouYin.SelectedIndex);
            row1.CreateCell(2).SetCellValue(this.JiangYing_One.SelectedIndex);
            row1.CreateCell(3).SetCellValue(this.JiangYing_Two.SelectedIndex);
            row1.CreateCell(4).SetCellValue(this.GenXiJin_One.SelectedIndex);
            row1.CreateCell(5).SetCellValue(this.GenXiJin_Two.SelectedIndex);
            row1.CreateCell(6).SetCellValue(this.Height.Text);
            row1.CreateCell(7).SetCellValue(this.Weight.Text);
            row1.CreateCell(8).SetCellValue(LS);
            row1.CreateCell(9).SetCellValue(DateTime.Now.ToString("yyyy_MM_dd_HH", DateTimeFormatInfo.InvariantInfo));
            fs1.Flush();
            wk.Write(fs1);
            wk.Close();
            fs1.Close();
        }

        private void LowerLimbEndButton_Click(object sender, RoutedEventArgs e)
        {
            //改变两个按钮的状态
            this.LowerLimbEndButton.IsEnabled = !this.LowerLimbEndButton.IsEnabled;
            this.LowerLimbStartButton.IsEnabled = !this.LowerLimbStartButton.IsEnabled;
            this.BingRenIdTextBox.Focus();

            //计算参数
            try
            {
                Thread t1 = new Thread(LowerLimb.CalculateLowerLimbResult);
                t1.Start();
                //System.Windows.MessageBox.Show("步态报告生成完成");
            }
            catch
            {
                //System.Windows.MessageBox.Show("没有收集到骨骼数据或骨骼数据不完整");
            }

            PlaySoundThree();
        }

        private void LowerLimbStartButton_Click(object sender, RoutedEventArgs e)
        {

            if (BingRen.PeopleId.Equals(""))
            {
                System.Windows.MessageBox.Show("输入病人号");
            }
            else
            {
                //改变两个按钮的状态
                this.LowerLimbEndButton.IsEnabled = !this.LowerLimbEndButton.IsEnabled;
                this.LowerLimbStartButton.IsEnabled = !this.LowerLimbStartButton.IsEnabled;
                this.BingRenIdTextBox.Focus();
                PlaySoundTwo();
            }

           
        }

        private void NSUStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (BingRen.PeopleId.Equals(""))
            {
                System.Windows.MessageBox.Show("输入病人号");
            }
            else
            {
                //改变两个按钮的状态
                this.NSUEndButton.IsEnabled = !this.NSUEndButton.IsEnabled;
                this.NSUStartButton.IsEnabled = !this.NSUStartButton.IsEnabled;
                this.BingRenIdTextBox.Focus();
                PlaySoundTwo();
            }
        }

        private void NSUEndButton_Click(object sender, RoutedEventArgs e)
        {
            //改变两个按钮的状态
            this.NSUEndButton.IsEnabled = !this.NSUEndButton.IsEnabled;
            this.NSUStartButton.IsEnabled = !this.NSUStartButton.IsEnabled;
            this.BingRenIdTextBox.Focus();

            //计算参数
            try
            {
                Thread t1 = new Thread(NormalStandUp.CalculateNSUResult);
                t1.Start();
                //System.Windows.MessageBox.Show("步态报告生成完成");
            }
            catch
            {
                //System.Windows.MessageBox.Show("没有收集到骨骼数据或骨骼数据不完整");
            }

            PlaySoundThree();
        }

        private void CSUStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (BingRen.PeopleId.Equals(""))
            {
                System.Windows.MessageBox.Show("输入病人号");
            }
            else
            {
                //改变两个按钮的状态
                this.CSUEndButton.IsEnabled = !this.CSUEndButton.IsEnabled;
                this.CSUStartButton.IsEnabled = !this.CSUStartButton.IsEnabled;
                this.BingRenIdTextBox.Focus();
                PlaySoundTwo();
            }

        }

        private void CSUEndButton_Click(object sender, RoutedEventArgs e)
        {
            //改变两个按钮的状态
            this.CSUEndButton.IsEnabled = !this.CSUEndButton.IsEnabled;
            this.CSUStartButton.IsEnabled = !this.CSUStartButton.IsEnabled;
            this.BingRenIdTextBox.Focus();

            //计算参数
            try
            {
                Thread t1 = new Thread(CrossStandUp.CalculateCSUResult);
                t1.Start();
                //System.Windows.MessageBox.Show("步态报告生成完成");
            }
            catch
            {
                //System.Windows.MessageBox.Show("没有收集到骨骼数据或骨骼数据不完整");
            }

            PlaySoundThree();
        }
    }
}
