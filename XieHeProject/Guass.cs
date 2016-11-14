using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XieHeProject
{
    //高斯滤波类
    class Guass
    {
        //高斯滤波，滤波过程，返回的参数是滤波后的数组
        public static double[] GuassFitter(double[] data)
        {
            double[] result = new double[data.Length];
            for (int i = 0; i< data.Length; i++)
            {
                //数组前2个和最后2个赋予原来的值
                if (i < 2 || i > data.Length - 1 - 2)
                {
                    result[i] = data[i];
                }
                //其他数使用权重平衡
                else
                {
                    result[i] = (data[i - 2] * 1 + data[i - 1] * 2 + data[i] * 4 + data[i + 1] * 2 + data[i + 2] * 1) / 10;
                }
            }
            return result;
        }
    }
}
