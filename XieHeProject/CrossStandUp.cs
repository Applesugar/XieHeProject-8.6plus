using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;

namespace XieHeProject
{
    class CrossStandUp
    {
        /// <summary>
        /// the height of the target person when doing Cross Stand Up
        /// the time of current frame
        /// </summary>
        private static List<double> CSUheight = new List<double>();
        private static List<DateTime> CSUtime = new List<DateTime>();

        //previous collection time
        private static DateTime d1 = new DateTime();

        //judge whether it's the first time to collect data
//        private static bool timeflag = true;

        ///<summary>
        ///collect the height of CSU
        ///<param name = "body"/>current body</param>
        ///<param neme = "bf"/>current skeleton</param>
        ///</summary>
        public static void CollectCSUheight(Body body, BodyFrame bf)
        {
            //whether it is the first time
            d1 = DateTime.Now;

            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            CameraSpacePoint Neck = joints[JointType.Neck].Position;

            //calculate the height of Neck and add it to the queue
            CSUheight.Add(BuTai.distancetoFloor(Neck, bf));
            CSUtime.Add(d1);
        }

        ///<summary>
        ///calculate the variance in a double[]
        ///</summary>
        public static double CalculateVariance(double[] a)
        {
            try
            {
                double average = a.Average();
                double variance = 0.0;
                double sum = 0.0;
                for(int i = 0;i < a.Length;++i)
                {
                    sum += Math.Pow((a[i] - average), 2);
                }
                variance = sum / a.Length;
                return variance;
            }
            catch
            {
                return double.MinValue;
            }
        }

        ///<summary>
        ///calculate the velocity and acceleration
        ///</summary>

        public static void CalculateCSUResult()
        {
            try
            {
                double[] currentHeight = new double[CSUheight.Count];
                for (int i = 0; i < CSUheight.Count; ++i)
                {
                    currentHeight[i] = CSUheight[i];
                }
                //Guass Filter
                double[] GuassianHeight = Guass.GuassFitter(currentHeight);
                //SMT Filter
                double[] SMTHeight = SMT.SMTFilter(GuassianHeight);
                //find the startpoint and endpoint
                List<int> pointSMTHeight = SMT.pointSMT(SMTHeight);
                List<double[]> startPoint = new List<double[]>();
                List<double[]> endPoint = new List<double[]>();
                for (int i = 0; i < pointSMTHeight.Count; i = i + 2)
                {
                    if (i == pointSMTHeight.Count - 1)
                    {
                        double[] max = SMT.findMax(GuassianHeight, pointSMTHeight[i], currentHeight.Length - 1);
                        double[] min = SMT.findMin(GuassianHeight, pointSMTHeight[i], currentHeight.Length - 1);
                        startPoint.Add(max);
                        endPoint.Add(min);
                    }
                    else
                    {
                        double[] max = SMT.findMax(GuassianHeight, pointSMTHeight[i], pointSMTHeight[i + 1]);
                        double[] min = SMT.findMin(GuassianHeight, pointSMTHeight[i], pointSMTHeight[i + 1]);
                        startPoint.Add(max);
                        endPoint.Add(min);
                    }
                }
                double[] deltaHeightUp = new double[startPoint.Count - 1];
                double[] deltaTimeUp = new double[startPoint.Count - 1];
                double[] CSUVelocityUp = new double[startPoint.Count - 1];
                double[] CSUAccelerationUp = new double[startPoint.Count - 1];
                double[] deltaHeightDown = new double[startPoint.Count];
                double[] deltaTimeDown = new double[startPoint.Count];
                double[] CSUVelocityDown = new double[startPoint.Count];
                double[] CSUAccelerationDown = new double[startPoint.Count];
                for (int i = 0; i < startPoint.Count; ++i)
                {
                    deltaHeightDown[i] = startPoint[i][0] - endPoint[i][0];  //should be positive
                    Debug.WriteLine("deltaHeightDown = " + deltaHeightDown[i]);
                    deltaTimeDown[i] = -(CSUtime[Convert.ToInt32(startPoint[i][1])] - CSUtime[Convert.ToInt32(endPoint[i][1])]).TotalSeconds;  //should be positive
                    CSUVelocityDown[i] = deltaHeightDown[i] / deltaTimeDown[i];
                    CSUAccelerationDown[i] = deltaHeightDown[i] / Math.Pow(deltaTimeDown[i], 2);
                }
                BingRen.CSUVelocityDown = CSUVelocityDown.Average();
                BingRen.CSUAccelerationDown = CSUAccelerationDown.Average();
                BingRen.CSUVelocityDown_variance = CalculateVariance(CSUVelocityDown);
                BingRen.CSUAccelerationDown_variance = CalculateVariance(CSUAccelerationDown);
                for (int i = 0; i < startPoint.Count - 1; ++i)
                {
                    deltaHeightUp[i] = startPoint[i + 1][0] - endPoint[i][0];  //should be positive
                    Debug.WriteLine("deltaHeightUp = " + deltaHeightUp[i]);
                    deltaTimeUp[i] = (CSUtime[Convert.ToInt32(startPoint[i + 1][1])] - CSUtime[Convert.ToInt32(endPoint[i][1])]).TotalSeconds;  //should be positive
                    CSUVelocityUp[i] = deltaHeightUp[i] / deltaTimeUp[i];
                    CSUAccelerationUp[i] = deltaHeightUp[i] / Math.Pow(deltaTimeUp[i], 2);
                }
                BingRen.CSUVelocityUp = CSUVelocityUp.Average();
                BingRen.CSUAccelerationUp = CSUAccelerationUp.Average();
                BingRen.CSUVelocityUp_variance = CalculateVariance(CSUVelocityUp);
                BingRen.CSUAccelerationUp_variance = CalculateVariance(CSUAccelerationUp);

                double[] deltaHeightOneCircle = new double[startPoint.Count - 1];
                double[] deltaTimeOneCircle = new double[startPoint.Count - 1];
                double[] CSUVelocityOneCircle = new double[startPoint.Count - 1];
                double[] CSUAccelerationOneCircle = new double[startPoint.Count - 1];
                for(int i = 0;i < startPoint.Count - 1;++i)
                {
                    deltaHeightOneCircle[i] = deltaHeightUp[i] + deltaHeightDown[i + 1];
                    deltaTimeOneCircle[i] = deltaTimeUp[i] + deltaTimeDown[i + 1];
                    CSUVelocityOneCircle[i] = deltaHeightOneCircle[i] / deltaTimeOneCircle[i];
                    CSUAccelerationOneCircle[i] = deltaHeightOneCircle[i] / Math.Pow(deltaTimeOneCircle[i], 2);
                }
                BingRen.CSUVelocityOneCircle = CSUVelocityOneCircle.Average();
                BingRen.CSUAccelerationOneCircle = CSUAccelerationOneCircle.Average();
                BingRen.CSUVelocityOneCircle_variance = CalculateVariance(CSUVelocityOneCircle);
                BingRen.CSUAccelerationOneCircle_variance = CalculateVariance(CSUAccelerationOneCircle);

                double deltaHeightWholeAction = 0;
                double deltaTimeWholeAction = 0;
                for(int i = 0;i < startPoint.Count - 1;++i)
                {
                    deltaHeightWholeAction += deltaHeightUp[i] + deltaHeightDown[i];
                    deltaTimeWholeAction += deltaTimeUp[i] + deltaTimeDown[i];
                }
                deltaHeightWholeAction += deltaHeightDown[startPoint.Count - 1];
                deltaTimeWholeAction += deltaTimeDown[startPoint.Count - 1];
                BingRen.CSUVelocityWholeAction = deltaHeightWholeAction / deltaTimeWholeAction;
                BingRen.CSUAccelerationWholeAction = deltaHeightWholeAction / Math.Pow(deltaTimeWholeAction, 2);
            }
            catch
            {
                ClearCSUData();
                System.Windows.MessageBox.Show("No Skeleton Data!");
                return;
            }
            finally
            {
                Debug.WriteLine(BingRen.CSUVelocityUp + " " + BingRen.CSUVelocityDown + " " + BingRen.CSUAccelerationUp + " " + BingRen.CSUAccelerationDown);
                ClearCSUData();
            }
        }

        ///<summary>
        ///clear the CrossStandUp data
        ///</summary>

        private static void ClearCSUData()
        {
            CSUheight.Clear();
            //also need to clear BingRen.CSUVelocity and BingRen.CSUAcceleration
            BingRen.CSUVelocityUp = 0;
            BingRen.CSUVelocityDown = 0;
            BingRen.CSUAccelerationUp = 0;
            BingRen.CSUAccelerationDown = 0;
            BingRen.CSUVelocityUp_variance = 0;
            BingRen.CSUVelocityDown_variance = 0;
            BingRen.CSUAccelerationUp_variance = 0;
            BingRen.CSUAccelerationDown_variance = 0;
            BingRen.CSUAccelerationOneCircle = 0;
            BingRen.CSUAccelerationOneCircle_variance = 0;
            BingRen.CSUVelocityOneCircle = 0;
            BingRen.CSUVelocityOneCircle_variance = 0;
            BingRen.CSUAccelerationWholeAction = 0;
            BingRen.CSUAccelerationWholeAction_variance = 0;
            BingRen.CSUVelocityWholeAction = 0;
            BingRen.CSUVelocityWholeAction_variance = 0;
        }
    }
}
