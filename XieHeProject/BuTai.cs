using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.Util;
using NPOI.SS.UserModel;
using System.Threading;
using NPOI.POIFS.FileSystem;

namespace XieHeProject
{
    class BuTai
    {   
        //使用的数据结构
        //6个点的原始数据
        private Queue<Object> BuTaiElement;

        //实时边处理边收集的信息
        private Queue<Object> ResultElement;

        //由于实时显示时出了队列，所以又要重新收集一次
        private Queue<Object> ReturnBuTaiElement;

        //所有6个关节的数据
        private List<Queue<Object>> AllBuTaiElement=new List<Queue<Object>>();

        //所有收集的实时数据
        private List<Queue<Object>> AllResultElement = new List<Queue<Object>>();

        //上次收集的时间
        private DateTime d1 = new DateTime();

        //当前收集的时间
        private DateTime d2 = new DateTime();

        //判断是否为第一次收集
        private bool timeflag =true;

        /// <summary>
        /// 步距绝对值
        /// </summary>
        private static List<double> StepDistance = new List<double>();

        /// <summary>
        /// Z绝对值
        /// </summary>
        private static List<double> ValueZ = new List<double>();

        /// <summary>
        /// 左脚离地高度
        /// </summary>
        private static List<double> LeftFootY = new List<double>();

        /// <summary>
        /// 左脚Z值
        /// </summary>
        private static List<double> LeftFootZ = new List<double>();

        /// <summary>
        /// 右脚离地高度
        /// </summary>
        private static List<double> RightFootY = new List<double>();

        /// <summary>
        /// 右脚Z值
        /// </summary>
        private static List<double> RightFootZ = new List<double>();

        /// <summary>
        /// 左右脚Z值之差
        /// </summary>
        private static List<double> LeftSubRightFootZ = new List<double>();

        /// <summary>
        /// 标识是左脚在前还是右脚在前
        /// </summary>
        private static List<bool> FootFlag = new List<bool>();

        /// <summary>
        /// 时间对应
        /// </summary>
        private static List<DateTime> FootTime=new List<DateTime>();
             
        //文件保存路径
        private String savePath = "../../../../步态总表.xls";


        /// <summary>
        /// 收集步距
        /// </summary>
        /// <param name="body"></param>
        public static void CollectStepDistance(Body body)
        {
            //收集左脚与右脚点数
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            CameraSpacePoint LeftFoot = joints[JointType.FootLeft].Position;
            CameraSpacePoint RightFoot = joints[JointType.FootRight].Position;
            StepDistance.Add(Math.Round(Math.Abs(LeftFoot.Z-RightFoot.Z),4));
        }

        /// <summary>
        /// 收集Z
        /// </summary>
        /// <param name="body"></param>
        public static void CollectValueZ(Body body,BodyFrame bf)
        {
            //收集左脚与右脚点数
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            CameraSpacePoint Base = joints[JointType.SpineBase].Position;
            CameraSpacePoint Shoulder = joints[JointType.SpineShoulder].Position;

            double x = Shoulder.X - Base.X;
            double y = Shoulder.Y - Base.Y;
            double z = Shoulder.Z - Base.Z;
            double angle = x * bf.FloorClipPlane.X + y * bf.FloorClipPlane.Y + z * bf.FloorClipPlane.Z
                / (Math.Sqrt(x * x + y * y + z * z)*Math.Sqrt(bf.FloorClipPlane.X*bf.FloorClipPlane.X
                + bf.FloorClipPlane.Y * bf.FloorClipPlane.Y + bf.FloorClipPlane.Z * bf.FloorClipPlane.Z));
            ValueZ.Add(Math.Round(angle, 4));
        }


        /// <summary>
        /// 收集
        /// </summary>
        /// <param name="body"></param>
        public static void CollectFootValues(Body body,BodyFrame bf)
        {
            //收集左脚与右脚点数
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            CameraSpacePoint LeftFoot = joints[JointType.FootLeft].Position;
            CameraSpacePoint RightFoot = joints[JointType.FootRight].Position;

            //LeftFootY.Add(-BuTai.distancetoFloor(LeftFoot, bf));
            LeftFootZ.Add(LeftFoot.Z);
            //RightFootY.Add(-BuTai.distancetoFloor(RightFoot, bf));
            RightFootZ.Add(RightFoot.Z);
            FootTime.Add(DateTime.Now);
            
            //右脚在前
            if (LeftFoot.Z - RightFoot.Z > 0)
            {
                FootFlag.Add(true);
            }
            else //左脚在前
            {
                FootFlag.Add(false);
            }
            
            LeftSubRightFootZ.Add(Math.Abs(LeftFoot.Z - RightFoot.Z));
            
        }

       
        /// <summary>
        /// 实时收集数据数据时得到数据
        /// </summary>
        /// <param name="body">当前骨骼帧</param>
        /// <returns></returns>
        public Queue<Object> getBuTaiData(Body body)
        {
            //判断是否是第一次
            if (timeflag)
            {
                d1 = DateTime.Now;
                d2 = DateTime.Now;
                timeflag = false;
            }
            else
            {
                d1 = d2;
                d2 = DateTime.Now;
            }

            //6个关节点和2个时间进入队列
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            BuTaiElement = new Queue<Object>();
            BuTaiElement.Enqueue(joints[JointType.AnkleLeft].Position);
            BuTaiElement.Enqueue(joints[JointType.AnkleRight].Position);
            BuTaiElement.Enqueue(joints[JointType.FootLeft].Position);
            BuTaiElement.Enqueue(joints[JointType.FootRight].Position);
            BuTaiElement.Enqueue(joints[JointType.KneeLeft].Position);
            BuTaiElement.Enqueue(joints[JointType.KneeRight].Position);
            BuTaiElement.Enqueue(d2);
            BuTaiElement.Enqueue(d2.Subtract(d1));
            //AllBuTaiElement.Add(BuTaiElement);
            return BuTaiElement;
        }

        /// <summary>
        /// 实时收集数据时得到步态参数数据,并返回到链表中
        /// </summary>
        /// <param name="stepWidth">步宽</param>
        /// <param name="leftstepLength">左步长</param>
        /// <param name="rightstepLength">右步长</param>
        /// <param name="leftstepHeight">左步高</param>
        /// <param name="rightstepHeight">右步高</param>
        /// <param name="time"></param>
        public void setResultData(double stepWidth,double leftstepLength,double rightstepLength,double leftstepHeight,double rightstepHeight,DateTime time)
        {
            ResultElement = new Queue<Object>();
            ResultElement.Enqueue(stepWidth);
            ResultElement.Enqueue(leftstepLength);
            ResultElement.Enqueue(rightstepLength);
            ResultElement.Enqueue(leftstepHeight);
            ResultElement.Enqueue(rightstepHeight);
            ResultElement.Enqueue(time);
            AllResultElement.Add(ResultElement);
        }


        /// <summary>
        /// 实时收集返回6个骨骼点数据
        /// </summary>
        /// <param name="p1">左脚踝</param>
        /// <param name="p2">右脚踝</param>
        /// <param name="p3">左脚尖</param>
        /// <param name="p4">右脚尖</param>
        /// <param name="p5">左膝盖</param>
        /// <param name="p6">右膝盖</param>
        public void setElementData(CameraSpacePoint p1, CameraSpacePoint p2, CameraSpacePoint p3, CameraSpacePoint p4, CameraSpacePoint p5, CameraSpacePoint p6)
        {
            ReturnBuTaiElement = new Queue<Object>();
            ReturnBuTaiElement.Enqueue(p1);
            ReturnBuTaiElement.Enqueue(p2);
            ReturnBuTaiElement.Enqueue(p3);
            ReturnBuTaiElement.Enqueue(p4);
            ReturnBuTaiElement.Enqueue(p5);
            ReturnBuTaiElement.Enqueue(p6);
            AllBuTaiElement.Add(ReturnBuTaiElement);
        }

        /// <summary>
        /// 计算结果并出报告
        /// </summary>
        public void calResult()
        {
            try
            {
                //实时返回的数据
                List<double> stepWidthList = new List<double>();
                List<double> leftstepLengthList = new List<double>();
                List<double> rightstepLengthList = new List<double>();
                List<double> leftstepHeightList = new List<double>();
                List<double> rightstepHeightList = new List<double>();
                List<DateTime> timeList = new List<DateTime>();

                //实时返回的6个骨骼坐标
                List<CameraSpacePoint> AnkleLeftList = new List<CameraSpacePoint>();
                List<CameraSpacePoint> AnkleRightList = new List<CameraSpacePoint>();
                List<CameraSpacePoint> FootLeftList = new List<CameraSpacePoint>();
                List<CameraSpacePoint> FootRightList = new List<CameraSpacePoint>();
                List<CameraSpacePoint> KneeLeftList = new List<CameraSpacePoint>();
                List<CameraSpacePoint> KneeRightList = new List<CameraSpacePoint>();

                //实时返回的数据
                for (int i = 0; i < AllResultElement.Count; i++)
                {
                    stepWidthList.Add((double)AllResultElement[i].Dequeue());
                    leftstepLengthList.Add((double)AllResultElement[i].Dequeue());
                    rightstepLengthList.Add((double)AllResultElement[i].Dequeue());
                    leftstepHeightList.Add((double)AllResultElement[i].Dequeue());
                    rightstepHeightList.Add((double)AllResultElement[i].Dequeue());
                    timeList.Add((DateTime)AllResultElement[i].Dequeue());
                }

                //实时返回的6个骨骼坐标
                for (int i = 0; i < AllBuTaiElement.Count; i++)
                {
                    AnkleLeftList.Add((CameraSpacePoint)AllBuTaiElement[i].Dequeue());
                    AnkleRightList.Add((CameraSpacePoint)AllBuTaiElement[i].Dequeue());
                    FootLeftList.Add((CameraSpacePoint)AllBuTaiElement[i].Dequeue());
                    FootRightList.Add((CameraSpacePoint)AllBuTaiElement[i].Dequeue());
                    KneeLeftList.Add((CameraSpacePoint)AllBuTaiElement[i].Dequeue());
                    KneeRightList.Add((CameraSpacePoint)AllBuTaiElement[i].Dequeue());
                }

                //计算走了多少步
                //膝盖左右的距离
                //double[] xP = new double[FootLeftList.Count];
                double[] yP = new double[FootLeftList.Count];
                for (int i = 0; i < FootLeftList.Count; i++)
                {
                    //xP[i] = AnkleLeftList[i].Z;
                    yP[i] = Math.Sqrt(Math.Pow(AnkleLeftList[i].X - AnkleRightList[i].X, 2) +
                        Math.Pow(AnkleLeftList[i].Y - AnkleRightList[i].Y, 2) +
                        Math.Pow(AnkleLeftList[i].Z - AnkleRightList[i].Z, 2));
                }
                //高斯滤波
                double[] GuassyP = Guass.GuassFitter(yP);

                //SMT滤波
                double[] SMTyP = SMT.SMTFilter(GuassyP);

                //找范围确定步数
                List<int> pointSMTyp = SMT.pointSMT(SMTyP);
                BingRen.StepCount = (pointSMTyp.Count + 1) / 2;

                //找每一步的起始点
                List<double[]> startendPoint = new List<double[]>();
                for (int i = 0; i < pointSMTyp.Count; i = i + 2)
                {
                    if (i == pointSMTyp.Count - 1)
                    {
                        double[] m = SMT.findMax(GuassyP, pointSMTyp[i], yP.Length - 1);
                        startendPoint.Add(m);
                    }
                    else
                    {
                        double[] m = SMT.findMax(GuassyP, pointSMTyp[i], pointSMTyp[i + 1]);
                        startendPoint.Add(m);
                    }

                }


                //计算步速
                DateTime startTime = timeList[0];
                DateTime endTime = timeList[timeList.Count - 1];
                TimeSpan useTime = endTime.Subtract(startTime);
                double ut = useTime.TotalSeconds;//TotalSeconds 属性表示整数和小数秒，而 Seconds 属性表示整秒数。
                double distance = Math.Abs(AnkleLeftList[AnkleLeftList.Count - 1].Z - AnkleLeftList[0].Z);
                BingRen.StepSpeed = Math.Round(distance / ut,4);//距离除以时间求步速


                //计算左步速,右歩速,周期歩速
                double mLeftSpeed = 0;
                double dLeftSpeed = 0;
                double mRightSpeed = 0;
                double dRightSpeed = 0;
                double mCycleSpeed = 0;
                double dCycleSpeed = 0;

                ///记录落地点 
                List<double> leftFootLowPoints = new List<double>();//左脚抬起到落地
                List<double> rightFootLowPoints = new List<double>();//右脚抬起到落地
                List<double> cycleFootLowPoints = new List<double>();//右脚抬起落地到左脚抬起落地


                List<double> AbsMaxPoints =new List<double>();
                //步长绝对值最大点位置
                BuTai.FindMaxValuesOrPosition(LeftSubRightFootZ.ToArray(), AbsMaxPoints, 1);
                for (int i = 0; i < AbsMaxPoints.Count; i++)
                {
                    if (i == 0)
                    {
                        if (FootFlag[(int)AbsMaxPoints[i]])
                        {
                            leftFootLowPoints.Add((int)AbsMaxPoints[i]);
                        }
                        else
                        {
                            rightFootLowPoints.Add((int)AbsMaxPoints[i]);     
                        }
                    }
                    else if (i==AbsMaxPoints.Count)
                    {
                        if (FootFlag[(int)AbsMaxPoints[i]])
                        {
                            rightFootLowPoints.Add((int)AbsMaxPoints[i]);
                        }
                        else
                        {
                            leftFootLowPoints.Add((int)AbsMaxPoints[i]);
                        }
                    }
                    else {
                        leftFootLowPoints.Add((int)AbsMaxPoints[i]);
                        rightFootLowPoints.Add((int)AbsMaxPoints[i]);
                    }
                }

                for (int i = 0; i < AbsMaxPoints.Count; i++)
                {
                    if (!FootFlag[(int)AbsMaxPoints[i]])
                    {
                        cycleFootLowPoints.Add((int)AbsMaxPoints[i]);
                    }
                }

                List<double> LeftSpeed = new List<double>();
                for (int i = 1; i < leftFootLowPoints.Count; i=i+2)
                {
                    TimeSpan t = FootTime[(int)leftFootLowPoints[i]].Subtract(FootTime[(int)leftFootLowPoints[i - 1]]);

                    LeftSpeed.Add(Math.Abs(LeftFootZ[(int)leftFootLowPoints[i]] - LeftFootZ[(int)leftFootLowPoints[i - 1]])
                        / t.TotalSeconds);
                }
                BuTai.CalCulateMD(LeftSpeed, ref mLeftSpeed, ref dLeftSpeed);

                List<double> RightSpeed = new List<double>();
                for (int i = 1; i < rightFootLowPoints.Count; i=i+2)
                {
                    TimeSpan t = FootTime[(int)rightFootLowPoints[i]].Subtract(FootTime[(int)rightFootLowPoints[i - 1]]);

                    RightSpeed.Add(Math.Abs(RightFootZ[(int)rightFootLowPoints[i]] - RightFootZ[(int)rightFootLowPoints[i - 1]])
                        / t.TotalSeconds);
                }
                BuTai.CalCulateMD(RightSpeed, ref mRightSpeed, ref dRightSpeed);


                List<double> CycleSpeed = new List<double>();
                for (int i = 1; i < cycleFootLowPoints.Count; i++)
                {
                    TimeSpan t = FootTime[(int)cycleFootLowPoints[i]].Subtract(FootTime[(int)cycleFootLowPoints[i - 1]]);

                    CycleSpeed.Add(Math.Abs(LeftFootZ[(int)cycleFootLowPoints[i]] - RightFootZ[(int)cycleFootLowPoints[i - 1]])
                        / t.TotalSeconds);
                }
                BuTai.CalCulateMD(CycleSpeed, ref mCycleSpeed, ref dCycleSpeed);

                    /***
                    List<double> leftFootLowPoints = new List<double>();
                    List<double> rightFootLowPoints = new List<double>();
                    List<double> cycleFootLowPoints = new List<double>();
                    for (int i = 1; i < LeftSubRightFootZ.Count; i++)
                    {
                        if (i == 1)
                        {
                            if (LeftSubRightFootZ[i] >= 0)
                            {
                                rightFootLowPoints.Add(i);
                                cycleFootLowPoints.Add(i);
                            }
                            else
                            {
                                leftFootLowPoints.Add(i);
                            }
                        }
                        else
                        {
                            if (LeftSubRightFootZ[i] >= 0 && LeftSubRightFootZ[i - 1] <= 0)
                            {
                                rightFootLowPoints.Add(i);
                                cycleFootLowPoints.Add(i);
                            }
                            if (LeftSubRightFootZ[i] <= 0 && LeftSubRightFootZ[i - 1] >= 0)
                            {
                                leftFootLowPoints.Add(i);
                                cycleFootLowPoints.Add(i);
                            }
                        }
                    }

                    List<double> LeftSpeed = new List<double>();
                    for (int i = 1; i < leftFootLowPoints.Count; i++)
                    {
                        TimeSpan t = FootTime[(int)leftFootLowPoints[i]].Subtract(FootTime[(int)leftFootLowPoints[i - 1]]);

                        LeftSpeed.Add(Math.Abs(LeftFootZ[(int)leftFootLowPoints[i]] - LeftFootZ[(int)leftFootLowPoints[i - 1]])
                            / t.TotalSeconds);
                    }
                    BuTai.CalCulateMD(LeftSpeed, ref mLeftSpeed, ref dLeftSpeed);

                    List<double> RightSpeed = new List<double>();
                    for (int i = 1; i < rightFootLowPoints.Count; i++)
                    {
                        TimeSpan t = FootTime[(int)rightFootLowPoints[i]].Subtract(FootTime[(int)rightFootLowPoints[i - 1]]);

                        RightSpeed.Add(Math.Abs(RightFootZ[(int)rightFootLowPoints[i]] - RightFootZ[(int)rightFootLowPoints[i - 1]])
                            / t.TotalSeconds);
                    }
                    BuTai.CalCulateMD(RightSpeed, ref mRightSpeed, ref dRightSpeed);


                    List<double> CycleSpeed = new List<double>();
                    for (int i = 2; i < cycleFootLowPoints.Count; i++)
                    {
                        TimeSpan t = FootTime[(int)cycleFootLowPoints[i]].Subtract(FootTime[(int)cycleFootLowPoints[i - 2]]);

                        CycleSpeed.Add(Math.Abs(RightFootZ[(int)cycleFootLowPoints[i]] - RightFootZ[(int)cycleFootLowPoints[i - 2]])
                            / t.TotalSeconds);
                    }
                    BuTai.CalCulateMD(CycleSpeed, ref mCycleSpeed, ref dCycleSpeed);
                    **/

                //赋值
                BingRen.LeftSpeedM = Math.Round(mLeftSpeed, 4);
                BingRen.LeftSpeedD = Math.Round(dLeftSpeed,4);
                BingRen.RightSpeedM = Math.Round(mRightSpeed,4);
                BingRen.RightSpeedD = Math.Round(dRightSpeed,4);
                BingRen.CyleSpeedM = Math.Round(mCycleSpeed,4);
                BingRen.CyleSpeedD = Math.Round(dCycleSpeed,4);

                ///计算左右步长均值,左右步长方差,左右步长协调性
                //计算左步长
                double mLeftStepLength = 0;
                double dLeftStepLength = 0;
                List<double> realLeftLength = new List<double>();
                for (int i = 0; i < leftstepLengthList.Count; i++)
                {
                    if (leftstepLengthList[i] != 0)
                    {
                        realLeftLength.Add(Math.Abs(leftstepLengthList[i]));
                    }
                }
                //存储几个最大点的值
                List<double> leftStepLengthMax = new List<double>();
                BuTai.FindMaxValuesOrPosition(realLeftLength.ToArray(), leftStepLengthMax, 0);
                BuTai.CalCulateMD(leftStepLengthMax,ref mLeftStepLength,ref dLeftStepLength);


                //计算右步长
                double mRightStepLength = 0;
                double dRightStepLength = 0;
                List<double> realRightength = new List<double>();
                for (int i = 0; i < rightstepLengthList.Count; i++)
                {
                    if (rightstepLengthList[i] != 0)
                        realRightength.Add(rightstepLengthList[i]);
                }
                //存储几个最大点的值
                List<double> rightStepLengthMax = new List<double>();
                BuTai.FindMaxValuesOrPosition(realRightength.ToArray(), rightStepLengthMax,0);
                BuTai.CalCulateMD(rightStepLengthMax, ref mRightStepLength, ref dRightStepLength);

                //赋值和求左右步长的协调性
                BingRen.RightStepLengthM =Math.Round(mRightStepLength,4);
                BingRen.RightStepLengthD = Math.Round(dRightStepLength,4);
                BingRen.LeftStepLengthM = Math.Round(mLeftStepLength,4);
                BingRen.LeftStepLengthD = Math.Round(dLeftStepLength,4);
                BingRen.StepLengthBalance = Math.Round(Math.Abs(mRightStepLength - mLeftStepLength),4);


                //求左右高均值,左右步高方差,左右步高的协调性
                //计算左步高均值和方差
                double mLeftStepHeight = 0;
                double dLeftStepHeight = 0;
                List<double> realeftHeight = new List<double>();
                for (int i = 0; i < leftstepHeightList.Count; i++)
                {
                    if (leftstepHeightList[i] != 0)
                        realeftHeight.Add(leftstepHeightList[i]);
                }
                //存储几个最大点
                List<double> leftMaxHeight = new List<double>();
                BuTai.FindMaxValuesOrPosition(realeftHeight.ToArray(),leftMaxHeight,0);
                BuTai.CalCulateMD(leftMaxHeight, ref mLeftStepHeight, ref dLeftStepHeight);

                //计算右步高均值和方差
                double mRightStepHeight = 0;
                double dRightStepHeight = 0;
                List<double> realrightHeight = new List<double>();
                for (int i = 0; i < rightstepHeightList.Count; i++)
                {
                    if (rightstepHeightList[i] != 0)
                        realrightHeight.Add(rightstepHeightList[i]);
                }
                //存储几个最大点
                List<double> rightMaxHeight = new List<double>();
                BuTai.FindMaxValuesOrPosition(realrightHeight.ToArray(), rightMaxHeight, 0);
                BuTai.CalCulateMD(rightMaxHeight, ref mRightStepHeight, ref dRightStepHeight);

                //赋值和计算左右步高协调性
                BingRen.RightStepHeightM = Math.Round(mRightStepHeight,4);
                BingRen.RightStepHeightD = Math.Round(dRightStepHeight,4);
                BingRen.LeftStepHeightM = Math.Round(mLeftStepHeight,4);
                BingRen.LeftStepHeightD = Math.Round(dLeftStepHeight,4);
                BingRen.StepHeightBalance = Math.Round(Math.Abs(mRightStepHeight-mLeftStepHeight),4);

                ///计算步宽平均方差,步距平均方差
                //计算步宽(平均)
                double mstepWidth = 0;
                double dstepWidth = 0;
                BuTai.CalCulateMD(stepWidthList,ref mstepWidth,ref dstepWidth);

                //赋值
                BingRen.StepWidthM = Math.Round(mstepWidth,4);
                BingRen.StepWidthD = Math.Round(dstepWidth,4);

                //计算步距
                double mStepDistance = 0;
                double dStepDistance = 0;
                //存储几个最大点的值
                List<double> stepdistanceMax = new List<double>();
                BuTai.FindMaxValuesOrPosition(StepDistance.ToArray(), stepdistanceMax,0);
                BuTai.CalCulateMD(stepdistanceMax,ref mStepDistance,ref dStepDistance);

                //赋值
                BingRen.StepDistanceM = Math.Round(mStepDistance,4);
                BingRen.StepDistanceD = Math.Round(dStepDistance,4);


                //算z值
                double mZ = 0;
                double dZ = 0;
                BuTai.CalCulateMD(ValueZ, ref mZ, ref dZ);

                BingRen.ZM = Math.Round(mZ,4);
                BingRen.ZD = Math.Round(dZ,4);

                    Console.WriteLine(BingRen.StepSpeed + " " + BingRen.RightSpeedM + " " + BingRen.RightSpeedD + " " + BingRen.LeftSpeedM + " " + BingRen.LeftSpeedD + "\n"
                     + BingRen.CyleSpeedM + " " + BingRen.CyleSpeedD + " " + BingRen.RightStepLengthM + " " + BingRen.RightStepLengthD + " " + BingRen.LeftStepLengthM + " " + BingRen.LeftStepLengthD + "\n"
                      + BingRen.StepLengthBalance + " " + BingRen.RightStepHeightD + " " + BingRen.RightStepHeightM + " " + BingRen.LeftStepHeightD + " " + BingRen.LeftStepHeightM + "\n"
                      + BingRen.StepHeightBalance + " " + BingRen.StepWidthM + " " + BingRen.StepWidthD + " " + BingRen.StepDistanceM + " " + BingRen.StepDistanceD + "\n"
                      + BingRen.ZM + " " + BingRen.ZD);
            }
            catch
            {
                clearData();
                System.Windows.MessageBox.Show("没有收集到骨骼数据!");
                return;
            }


            //暂时不用
            
            //生成报告
            try
            {
                //如果没有文件生成表头
                if (!File.Exists(savePath))
                {
                    createExcel();
                }
                //添加内容
                addExcel();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            //清理步态数据
            clearData();
            
        }

       

        /// <summary>
        /// 点到地面的距离
        /// </summary>
        /// <param name="oldpoint">点</param>
        /// <param name="fe">当前骨骼帧</param>
        /// <returns></returns>
        public static double distancetoFloor(CameraSpacePoint oldpoint, BodyFrame fe)
        {
            double distance = Math.Abs(fe.FloorClipPlane.X*oldpoint.X+fe.FloorClipPlane.Y*oldpoint.Y+fe.FloorClipPlane.Z*oldpoint.Z+fe.FloorClipPlane.W)
                / Math.Sqrt(Math.Pow(fe.FloorClipPlane.X * fe.FloorClipPlane.X + fe.FloorClipPlane.Y * fe.FloorClipPlane.Y + fe.FloorClipPlane.Z * fe.FloorClipPlane.Z, 2));

            return distance;
        }

        /// <summary>
        /// 计算均值与方差
        /// </summary>
        /// <param name="list">链表数组</param>
        /// <param name="m">均值</param>
        /// <param name="d">方差</param>
        public static void CalCulateMD(List<double> list,ref double m,ref double d)
        {
            for (int i = 0; i < list.Count; i++)
                m += list[i];
            m = m / list.Count;
            for (int i = 0; i < list.Count; i++)
                d += (m-list[i])*(m-list[i]);
            d = d / list.Count;

        }

        /// <summary>
        /// 找到最大点的值或者位置
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="valuesorpositions">最大点的值或者位置</param>
        /// <param name="vorp">添加值或者位置(0为值,1为位置)</param>
        public static void FindMaxValuesOrPosition(double[] data, List<double> valuesorpositions,int vorp)
        {
            //高斯滤波平滑曲线
            double[] leftLGuass = Guass.GuassFitter(data.ToArray());

            //SMT得到0，1折线
            double[] leftLSMT = SMT.SMTFilter(leftLGuass);

            //确定0，1折线的开始结束点
            List<int> pointleftLSMT = SMT.pointSMT(leftLSMT);

            //找到几个步长最大点
            for (int i = 0; i < pointleftLSMT.Count; i = i + 2)
            {
                if (i == pointleftLSMT.Count - 1)
                {
                    if (vorp == 0)
                    {
                        double[] m = SMT.findMax(data, pointleftLSMT[i], data.Length - 1);
                        valuesorpositions.Add(m[vorp]);
                    }
                }
                else
                {
                    double[] m = SMT.findMax(data.ToArray(), pointleftLSMT[i], pointleftLSMT[i + 1]);
                    valuesorpositions.Add(m[vorp]);
                }

            }
        }



        /// <summary>
        /// 生成步态execl表格
        /// </summary>
        private void createExcel()
        {
            FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            HSSFWorkbook wk = new HSSFWorkbook();
            ISheet tb = wk.CreateSheet("Sheet01");
            IRow row = tb.CreateRow(0);
            row.CreateCell(0).SetCellValue("歩速");
            row.CreateCell(1).SetCellValue("右歩速均值");
            row.CreateCell(2).SetCellValue("右歩速方差");
            row.CreateCell(3).SetCellValue("左歩速均值");
            row.CreateCell(4).SetCellValue("左歩速方差");
            row.CreateCell(5).SetCellValue("周期歩速均值");
            row.CreateCell(6).SetCellValue("周期歩速方差");
            row.CreateCell(7).SetCellValue("右步长均值");
            row.CreateCell(8).SetCellValue("右步长方差");
            row.CreateCell(9).SetCellValue("左步长均值");
            row.CreateCell(10).SetCellValue("左步长方差");
            row.CreateCell(11).SetCellValue("左右步长协调性");
            row.CreateCell(12).SetCellValue("右步高均值");
            row.CreateCell(13).SetCellValue("右步高方差");
            row.CreateCell(14).SetCellValue("左步高均值");
            row.CreateCell(15).SetCellValue("左步高方差");
            row.CreateCell(16).SetCellValue("左右步高协调性");
            row.CreateCell(17).SetCellValue("步宽均值");
            row.CreateCell(18).SetCellValue("步宽方差");
            row.CreateCell(19).SetCellValue("步距均值");
            row.CreateCell(20).SetCellValue("步距方差");
            row.CreateCell(21).SetCellValue("z角均值");
            row.CreateCell(22).SetCellValue("z角方差");
            row.CreateCell(23).SetCellValue("右脚变异度");
            row.CreateCell(24).SetCellValue("左脚变异度");
            row.CreateCell(25).SetCellValue("自然状态起立速度");
            row.CreateCell(26).SetCellValue("自然状态坐下速度");
            row.CreateCell(27).SetCellValue("自然状态起立加速度");
            row.CreateCell(28).SetCellValue("自然状态坐下加速度");
            row.CreateCell(29).SetCellValue("交叉起立速度均值");
            row.CreateCell(30).SetCellValue("交叉起立速度方差");
            row.CreateCell(31).SetCellValue("交叉起立加速度均值");
            row.CreateCell(32).SetCellValue("交叉起立加速度方差");
            row.CreateCell(33).SetCellValue("交叉坐下速度均值");
            row.CreateCell(34).SetCellValue("交叉坐下速度方差");
            row.CreateCell(35).SetCellValue("交叉坐下加速度均值");
            row.CreateCell(36).SetCellValue("交叉坐下加速度方差");
            row.CreateCell(37).SetCellValue("交叉坐周期速度均值");
            row.CreateCell(38).SetCellValue("交叉坐周期速度方差");
            row.CreateCell(38).SetCellValue("交叉坐周期加速度均值");
            row.CreateCell(40).SetCellValue("交叉坐周期加速度方差");
            row.CreateCell(41).SetCellValue("交叉坐整体速度均值");
            row.CreateCell(42).SetCellValue("交叉坐整体加速度均值");
//            row.CreateCell(43).SetCellValue("");
//            row.CreateCell(44).SetCellValue("");
//            row.CreateCell(45).SetCellValue("");
//            row.CreateCell(46).SetCellValue("");



            wk.Write(fs);
            wk.Close();
            fs.Close();

        }

        /// <summary>
        /// Excel添加一行
        /// </summary>
        private void addExcel()
        {
            FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            POIFSFileSystem ps = new POIFSFileSystem(fs);
            HSSFWorkbook wk = new HSSFWorkbook(ps);
            ISheet tb = wk.GetSheet("Sheet01");
            int number = tb.LastRowNum;
            FileStream fs1 = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            IRow row1 = tb.CreateRow(number + 1);
            row1.CreateCell(0).SetCellValue(BingRen.StepSpeed);
            row1.CreateCell(1).SetCellValue(BingRen.RightSpeedM);
            row1.CreateCell(2).SetCellValue(BingRen.RightSpeedD);
            row1.CreateCell(3).SetCellValue(BingRen.LeftSpeedM);
            row1.CreateCell(4).SetCellValue(BingRen.LeftSpeedD);
            row1.CreateCell(5).SetCellValue(BingRen.CyleSpeedM);
            row1.CreateCell(6).SetCellValue(BingRen.CyleSpeedD);
            row1.CreateCell(7).SetCellValue(BingRen.RightStepLengthM);
            row1.CreateCell(8).SetCellValue(BingRen.RightStepLengthD);
            row1.CreateCell(9).SetCellValue(BingRen.LeftStepLengthM);
            row1.CreateCell(10).SetCellValue(BingRen.LeftStepLengthD);
            row1.CreateCell(11).SetCellValue(BingRen.StepLengthBalance);
            row1.CreateCell(12).SetCellValue(BingRen.RightStepHeightM);
            row1.CreateCell(13).SetCellValue(BingRen.RightStepHeightD);
            row1.CreateCell(14).SetCellValue(BingRen.LeftStepHeightM);
            row1.CreateCell(15).SetCellValue(BingRen.LeftStepHeightD);
            row1.CreateCell(16).SetCellValue(BingRen.StepHeightBalance);
            row1.CreateCell(17).SetCellValue(BingRen.StepWidthM);
            row1.CreateCell(18).SetCellValue(BingRen.StepWidthD);
            row1.CreateCell(19).SetCellValue(BingRen.StepDistanceM);
            row1.CreateCell(20).SetCellValue(BingRen.StepDistanceD);
            row1.CreateCell(21).SetCellValue(BingRen.ZM);
            row1.CreateCell(22).SetCellValue(BingRen.ZD);
            row1.CreateCell(23).SetCellValue(BingRen.LeftFootAberrance);
            row1.CreateCell(24).SetCellValue(BingRen.RightFootAberrance);
            row1.CreateCell(25).SetCellValue(BingRen.NSUVelocityUp);
            row1.CreateCell(26).SetCellValue(BingRen.NSUVelocityDown);
            row1.CreateCell(27).SetCellValue(BingRen.NSUAccelerationUp);
            row1.CreateCell(28).SetCellValue(BingRen.NSUAccelerationDown);
            row1.CreateCell(29).SetCellValue(BingRen.CSUVelocityUp);
            row1.CreateCell(30).SetCellValue(BingRen.CSUVelocityUp_variance);
            row1.CreateCell(31).SetCellValue(BingRen.CSUAccelerationUp);
            row1.CreateCell(32).SetCellValue(BingRen.CSUAccelerationUp_variance);
            row1.CreateCell(33).SetCellValue(BingRen.CSUVelocityDown);
            row1.CreateCell(34).SetCellValue(BingRen.CSUVelocityDown_variance);
            row1.CreateCell(35).SetCellValue(BingRen.CSUAccelerationDown);
            row1.CreateCell(36).SetCellValue(BingRen.CSUAccelerationDown_variance);
            row1.CreateCell(37).SetCellValue(BingRen.CSUVelocityOneCircle);
            row1.CreateCell(38).SetCellValue(BingRen.CSUVelocityOneCircle_variance);
            row1.CreateCell(39).SetCellValue(BingRen.CSUAccelerationOneCircle);
            row1.CreateCell(40).SetCellValue(BingRen.CSUAccelerationOneCircle_variance);
            row1.CreateCell(41).SetCellValue(BingRen.CSUVelocityWholeAction);
            row1.CreateCell(42).SetCellValue(BingRen.CSUAccelerationWholeAction);


            fs1.Flush();
            wk.Write(fs1);
            wk.Close();
            fs1.Close();
        }

        /// <summary>
        /// 清除步态数据
        /// </summary>
        private void clearData()
        {
            AllBuTaiElement.Clear();
            AllResultElement.Clear();
            StepDistance.Clear();
            ValueZ.Clear();
            LeftFootY.Clear();
            LeftFootZ.Clear();
            RightFootY.Clear();
            RightFootZ.Clear();
            FootTime.Clear();
            LeftSubRightFootZ.Clear();
            FootFlag.Clear();

            BingRen.StepSpeed = 0;
            BingRen.StepWidth = 0;
            BingRen.LeftstepHeight = 0;
            BingRen.RightstepHeight = 0;
            BingRen.LeftstepLength = 0;
            BingRen.RightstepLength = 0;
            BingRen.StepCount = 0;

            BingRen.StepSpeed = 0;
            BingRen.RightSpeedM = 0;
            BingRen.RightSpeedD = 0;
            BingRen.LeftSpeedM = 0;
            BingRen.LeftSpeedD = 0;
            BingRen.CyleSpeedM = 0;
            BingRen.CyleSpeedD = 0;
            BingRen.RightStepLengthM = 0;
            BingRen.RightStepLengthD = 0;
            BingRen.LeftStepLengthM = 0;
            BingRen.LeftStepLengthD = 0;
            BingRen.StepLengthBalance = 0;
            BingRen.RightStepHeightD = 0;
            BingRen.RightStepHeightM = 0;
            BingRen.LeftStepHeightD = 0;
            BingRen.LeftStepHeightM = 0;
            BingRen.StepHeightBalance = 0;
            BingRen.StepWidthM = 0;
            BingRen.StepWidthD = 0;
            BingRen.StepDistanceM = 0;
            BingRen.StepDistanceD = 0;
            BingRen.ZM = 0;
            BingRen.ZD = 0;
            BingRen.RightFootAberrance = 0;
            BingRen.LeftFootAberrance = 0;

        }


    }
}
