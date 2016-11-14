using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;


namespace XieHeProject
{
    class LowerLimb
    {
        /// <summary>
        /// 左脚抬脚高度
        /// </summary>
        private static List<double> LeftFootHeight = new List<double>();

        /// <summary>
        /// 右脚抬脚高度
        /// </summary>
        private static List<double> RightFootHeight = new List<double>();


        /// <summary>
        /// 搜集抬脚高度
        /// </summary>
        /// <param name="body">当前身体</param>
        /// <param name="bf">当前骨骼帧</param>
        public static void CollectFootHeight(Body body,BodyFrame bf)
        { 
            //收集左脚与右脚点数
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            CameraSpacePoint LeftFoot = joints[JointType.AnkleLeft].Position;
            CameraSpacePoint RightFoot = joints[JointType.AnkleRight].Position;

            //计算离地高度添加到队列
            LeftFootHeight.Add(BuTai.distancetoFloor(LeftFoot,bf));
            RightFootHeight.Add(BuTai.distancetoFloor(RightFoot, bf));
        }

        /// <summary>
        /// 计算下肢灵活度结果
        /// </summary>
        public static void CalculateLowerLimbResult()
        {
            try
            {
                //计算左脚抬脚高度均值方差
                double mLeftHeight = 0;
                double dLeftHeight = 0;
                List<double> realLeftHight = new List<double>();
                
                for (int i = 0; i < LeftFootHeight.Count; i++)
                {
                    if (LeftFootHeight[i] != 0)
                    {
                        realLeftHight.Add(Math.Abs(LeftFootHeight[i]));
                    }
                }

               
                //存储几个最大点的值
                List<double> leftHeightMax = new List<double>();
                BuTai.FindMaxValuesOrPosition(realLeftHight.ToArray(), leftHeightMax,0);
                BuTai.CalCulateMD(leftHeightMax, ref mLeftHeight, ref dLeftHeight);


                //计算右脚抬脚高度均值方差
                double mRightHeight = 0;
                double dRightHeight = 0;
                List<double> realRigthHight = new List<double>();

                for (int i = 0; i < RightFootHeight.Count; i++)
                {
                    if (RightFootHeight[i] != 0)
                    {
                        realRigthHight.Add(Math.Abs(RightFootHeight[i]));
                    }
                }
               
                //存储几个最大点的值
                List<double> rightHeightMax = new List<double>();
                BuTai.FindMaxValuesOrPosition(realRigthHight.ToArray(),rightHeightMax,0);
                BuTai.CalCulateMD(rightHeightMax,ref mRightHeight,ref dRightHeight);

                //左右脚变异度
                BingRen.RightFootAberrance = Math.Round(dRightHeight / mRightHeight,4);
                BingRen.LeftFootAberrance = Math.Round(dLeftHeight / mLeftHeight,4);
            }
            catch
            {
                ClearLowerLimbData();
                System.Windows.MessageBox.Show("没有收集到骨骼数据!");
                return;
            }
            finally
            {
                Console.WriteLine(BingRen.LeftFootAberrance+" "+BingRen.RightFootAberrance+"\n");
                ClearLowerLimbData();
            }

        }


        /// <summary>
        /// 清除下肢灵活度数据
        /// </summary>
        private static void ClearLowerLimbData()
        {
            LeftFootHeight.Clear();
            RightFootHeight.Clear();
            BingRen.LeftFootAberrance = 0;
            BingRen.RightFootAberrance = 0;
        }
    }
}
