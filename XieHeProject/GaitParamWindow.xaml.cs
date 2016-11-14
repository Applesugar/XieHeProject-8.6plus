using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
using System.Windows.Threading;

namespace XieHeProject
{
    /// <summary>
    /// GaitParamWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GaitParamWindow : Window
    {
        /// <summary>
        /// 抓窗体
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

        //彩色图图像
        private WriteableBitmap colorBitmap = null;

        //Kinect传感器
        private KinectSensor kinectSensor = null;

        //坐标转换
        private CoordinateMapper coordinateMapper = null;


        //身体Reader
        private BodyFrameReader bodyFrameReader = null;

        //彩色图Reader
        private ColorFrameReader colorFrameReader = null;

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

        //获取彩色图像
        public ImageSource ColorImageSource
        {
            get
            {
                return this.colorBitmap;
            }

        }

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


        private KinectVideo _kVideo = new KinectVideo();

        /// <summary>
        /// 步态测试类
        /// </summary>
        private BuTai  butai = new BuTai();

        /// <summary>
        /// 主函数
        /// </summary>
        public GaitParamWindow()
        {
            //取得传感器
            this.kinectSensor = KinectSensor.GetDefault();

            //取得坐标转换器
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            //获取彩色图像信息
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            //打开身体Reader
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            //打开彩色图Reader
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

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
            this.imageSource = new DrawingImage(this.drawingGroup);
            this.DataContext = this;

            //创建位图
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
            InitializeComponent();
        }

        /// <summary>
        /// 窗体载入函数
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

          
            //目前隐藏2个按钮
            this.MoveToBuTaiTimeButton.IsEnabled = false;
            this.LoadBuTaiButton.IsEnabled = false;


            //步态按钮状态
            this.BuTaiStartButton.IsEnabled = true;
            this.BuTaiStopButton.IsEnabled = false;

            this.BingRenText.Focus();

        }

        /// <summary>
        /// 计时器回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            this.BingRenText.Focus();
            //Console.WriteLine("HiHiHi......");
        }  

        /// <summary>
        /// 彩色图回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void colorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
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

                           
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }

                        this.colorBitmap.Unlock();
                    }
                }
            }
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
                            System.Windows.Media.Pen drawPen = this.bodyColors[penIndex++];

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

                                //判断是否开始收集步态数据
                                if (!this.BuTaiStartButton.IsEnabled)
                                    changBuTaiLabels(body, bodyFrame);

                                //画身体
                                this.DrawBody(joints, jointPoints, dc, drawPen);
                                //画左手
                                this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                                //画右手
                                this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                            }
                        }

                        // prevent drawing outside of our render area
                        this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                    }
                }
            }


        }

        /// <summary>
        /// 窗体关闭函数
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
        /// 是否显示骨骼,但是已经弃用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowsSkeleton(object sender, RoutedEventArgs e)
        {

            //if ((bool)this.CheckBoxShow.IsChecked)
            //{
            //    //this.ImageSource = new DrawingImage(this.drawingGroup);
            //    if (this.bodyFrameReader != null)
            //    {
            //        this.bodyFrameReader.FrameArrived += bodyFrameReader_FrameArrived;
                    
            //    }
            //}
            //else
            //{
            //    //this.ImageSource = null;
            //    if (this.bodyFrameReader != null)
            //    {
            //        this.bodyFrameReader.FrameArrived -= bodyFrameReader_FrameArrived;
            //    }
            //}
        }

      

        /// <summary>
        /// 键盘响应事件,现在弃用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                this.BingRenText.Focus();
                BingRen.PeopleId = this.BingRenText.Text.ToString();
                this.BingRenText.Text= "";
                this.BingRenLable.Content = "病号:" + BingRen.PeopleId;
            }
        }

        /// <summary>
        /// 回放xef文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Kinect录像文件(*.xef)|*.xef";
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                //_kVideo.PlaybackClip(openFile.FileName);

                BingRen.PeopleId = BingRen.GetBingRenVideoFileId(System.IO.Path.GetFileName(openFile.FileName));
                Console.WriteLine(BingRen.PeopleId);
                //开启其它线程播放文件
                Thread t = new Thread(_kVideo.PlaybackClip);
                t.IsBackground = true;
                t.Start(openFile.FileName);

                ChangeLoadButtonState(false);
                KinectVideo.Flag2 = 0;

                Thread t1 = new Thread(ChangeLoadButton);
                t1.IsBackground = true;
                t1.Start();
            }
            else
            {
                BingRen.PeopleId = "";
            }
            BingRenLable.Content = "病人Id号:" + BingRen.PeopleId;
            this.BingRenText.Focus();
        }

        /// <summary>
        /// 改变Load按钮状态
        /// </summary>
        private void ChangeLoadButton()
        {
            while (KinectVideo.Flag2 == 0)
            {
            }
            ChangeLoadButtonState(true);
        }

        /// <summary>
        /// 设定另一个线程中Load按钮可用状态
        /// </summary>
        /// <param name="state">状态布尔值</param>
        private void ChangeLoadButtonState(bool state)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.LoadButton.IsEnabled = state));
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
            double stepwidth = Math.Sqrt(Math.Abs(bonePoint1.X - bonePoint2.X));
            this.StepWidthLabel.Content = "步宽:" + Math.Round(stepwidth, 4);

            //将结果储存起来
            butai.setResultData(stepwidth, leftstepLength, rightstepLength, leftstepHeight, rightstepHeight, dt);
            butai.setElementData(bonePoint1, bonePoint2, bonePoint3, bonePoint4, bonePoint5, bonePoint6);

        }

        /// <summary>
        /// 步态开始按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuTaiStartButton_Click(object sender, RoutedEventArgs e)
        {
            //改变两个按钮的状态
            this.BuTaiStartButton.IsEnabled = !this.BuTaiStartButton.IsEnabled;
            this.BuTaiStopButton.IsEnabled = !this.BuTaiStopButton.IsEnabled;
            this.BingRenText.Focus();
        }

        /// <summary>
        /// 步态结束按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuTaiStopButton_Click(object sender, RoutedEventArgs e)
        {
            //改变两个按钮的状态
            this.BuTaiStartButton.IsEnabled = !this.BuTaiStartButton.IsEnabled;
            this.BuTaiStopButton.IsEnabled = !this.BuTaiStopButton.IsEnabled;
            this.BingRenText.Focus();

            //计算步态
            Thread t1 = new Thread(butai.calResult);
            t1.Start();
            System.Windows.MessageBox.Show("步态报告生成完成");
        }


        //载入单独病人切分文件，得到三米步态的开始时间
        private void LoadButton_Click_1(object sender, RoutedEventArgs e)
        {
            //openFile = new OpenFileDialog();
            //openFile.Filter = "txt病人单独切分文件(*.txt)|*.txt";
            //if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    try
            //    {
            //        String butaiStarttime = pConnection.txtRead(openFile.FileName);
            //        this.SmBuTaiLabel.Content = "三米步态切分开始时间:" + butaiStarttime;
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //        System.Windows.MessageBox.Show("载入文件格式不正确或不是单独的切分文件");
            //    }
            //}
        }

        //将xef视频跳转到三米步态的开始时间
        private void MoveToBuTaiTimeButton_Click(object sender, RoutedEventArgs e)
        {
            //String[] s = this.SmBuTaiLabel.Content.ToString().Split(new char[] { ':' });
            //if (s.Length >= 2)
            //{
            //    Int64 d = (Int64)Double.Parse(s[1]);
            //    d = d * 10000000;
            //    TimeSpan t = new TimeSpan(d);
            //    //Console.WriteLine(t.ToString());


            //    Int64 d1 = 3;
            //    d1 = d1 * 10000000;
            //    TimeSpan t1 = new TimeSpan(d1);


            //    pConnection.PlaybackPauseClip();
            //    pConnection.PlaybackMoveToClick(t);
            //    pConnection.PlaybackResumeClip();


            //    /*
            //    Thread thread = new Thread(pConnection.PlaybackMoveToClick);
            //    thread.Start(t1);
            //    **/
            //}
        }

       
    }
}
