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
    class NormalStandUp
    {
        /// <summary>
        /// the height of the target person when doing Normal Stand Up
        /// the time of current frame
        /// </summary>
        private static List<double> NSUheight = new List<double>();
        private static List<DateTime> NSUtime = new List<DateTime>();

        //previous collection time
        private static DateTime d1 = new DateTime();

        //judge whether it's the first time to collect data
        private static bool timeflag = true;

        ///<summary>
        ///collect the height of NSU
        ///<param name = "body"/>current body</param>
        ///<param neme = "bf"/>current skeleton</param>
        ///</summary>
        public static void CollectNSUheight(Body body, BodyFrame bf)
        {
            //whether it is the first time
            d1 = DateTime.Now;

            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            CameraSpacePoint Neck = joints[JointType.Neck].Position;

            //calculate the height of Neck and add it to the queue
            NSUheight.Add(BuTai.distancetoFloor(Neck, bf));
            NSUtime.Add(d1);
        }

        ///<summary>
        ///calculate the velocity and acceleration
        ///</summary>
        
        public static void CalculateNSUResult()
        {
            try
            {
                double[] currentHeight = new double[NSUheight.Count];
                for(int i = 0;i < NSUheight.Count;++i)
                {
                    currentHeight[i] = NSUheight[i];
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
                double[] NSUVelocityUp = new double[startPoint.Count];
                double[] NSUAccelerationUp = new double[startPoint.Count];
                double[] deltaHeightDown = new double[startPoint.Count];
                double[] deltaTimeDown = new double[startPoint.Count];
                double[] NSUVelocityDown = new double[startPoint.Count];
                double[] NSUAccelerationDown = new double[startPoint.Count];
                for(int i = 0;i < startPoint.Count;++i)
                {
                    deltaHeightUp[i] = startPoint[i][0] - endPoint[i][0];
                    deltaTimeUp[i] = (NSUtime[Convert.ToInt32(startPoint[i][1])] - NSUtime[Convert.ToInt32(endPoint[i][1])]).TotalSeconds;
                    NSUVelocityUp[i] = deltaHeightUp[i] / deltaTimeUp[i];
                    NSUAccelerationUp[i] = deltaHeightUp[i] / Math.Pow(deltaTimeUp[i],2);
                }
                BingRen.NSUVelocityUp = NSUVelocityUp.Average();
                BingRen.NSUAccelerationUp = NSUAccelerationUp.Average();

                }
            catch
            {
                ClearNSUData();
                System.Windows.MessageBox.Show("No Skeleton Data!");
                return;
            }
            finally
            {
                Debug.WriteLine(BingRen.NSUVelocityUp + " " + BingRen.NSUVelocityDown + " " + BingRen.NSUAccelerationUp + " " + BingRen.NSUAccelerationDown);
                ClearNSUData();
            }
        }

        ///<summary>
        ///clear the NormalStandUp data
        ///</summary>
        
        private static void ClearNSUData()
        {
            NSUheight.Clear();
            //also need to clear BingRen.NSUVelocity and BingRen.NSUAcceleration
            BingRen.NSUVelocityUp = 0;
            BingRen.NSUVelocityDown = 0;
            BingRen.NSUAccelerationUp = 0;
            BingRen.NSUAccelerationDown = 0;
        }
    }
}
