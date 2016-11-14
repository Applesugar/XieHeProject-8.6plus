using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XieHeProject
{

    //SMT触发器
    class SMT
    {
        //使用SMT触发器对曲线滤波
        public static double[] SMTFilter(double[] data)
        { 
            //求平均数
            double total=0;
            for (int i = 0; i < data.Length; i++)
                total += data[i];
            double avg = total/data.Length;

            //得到上限H1下限H2
            double H1 = avg * 1.025;
            double H0 = avg * 0.975;

            //滤波过程
            double[] result=new double[data.Length];
            for (int i = 0; i < result.Length; i++)
            {
                if (i == 0)
                {
                    result[i] = 0;
                }
                else
                {
                    if (result[i - 1] == 0)
                    {
                        if (data[i] > H1)
                            result[i] = 1;
                        else
                            result[i] = 0;
                    }
                    else
                    {
                        if (data[i] <H0)
                            result[i] = 0;
                        else
                            result[i] = 1;
                    }
                }
            }
                return result;
        }


        //得到SMT滤波的0,1 的开始结束区间
        public static List<int> pointSMT(double[] data)
        {
            List<int> point=new List<int>();
            for (int i = 0; i < data.Length-1; i++)
            {
                //使用值的变化判断开始结束区间
                if (data[i] != data[i + 1])
                    point.Add(i);
            }
            return point;
        }

        //找到 0,1 的开始结束区间的最大值及其坐标
        public static double[] findMax(double[] data, int start, int end)
        {
            double max = -1;
            double point = 0;
            //找最大值
            for (int i = start; i <= end; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                    point = i;
                }
                    
            }
            double[] result = { max,point};
            return result;
        }
        //find the minimum and its coordinates
        public static double[] findMin(double[] data, int start,int end)
        {
            double min = double.MaxValue;
            double point = 0;
            for(int i = start; i <= end; ++i)
            {
                if(data[i] < min)
                {
                    min = data[i];
                    point = i;
                }
            }
            double[] result = { min, point};
            return result;
        }
    }
}
