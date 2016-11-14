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
                double[] deltaHeightUp = new double[startPoint.Count];
                double[] deltaTimeUp = new double[startPoint.Count];
                double[] CSUVelocityUp = new double[startPoint.Count];
                double[] CSUAccelerationUp = new double[startPoint.Count];
                double[] deltaHeightDown = new double[startPoint.Count];
                double[] deltaTimeDown = new double[startPoint.Count];
                double[] CSUVelocityDown = new double[startPoint.Count];
                double[] CSUAccelerationDown = new double[startPoint.Count];
                for (int i = 0; i < startPoint.Count; ++i)
                {
                    deltaHeightUp[i] = startPoint[i][0] - endPoint[i][0];
                    deltaTimeUp[i] = (CSUtime[Convert.ToInt32(startPoint[i][1])] - CSUtime[Convert.ToInt32(endPoint[i][1])]).TotalSeconds;
                    CSUVelocityUp[i] = deltaHeightUp[i] / deltaTimeUp[i];
                    CSUAccelerationUp[i] = deltaHeightUp[i] / Math.Pow(deltaTimeUp[i], 2);
                }
                BingRen.CSUVelocityUp = CSUVelocityUp.Average();
                BingRen.CSUAccelerationUp = CSUAccelerationUp.Average();

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
