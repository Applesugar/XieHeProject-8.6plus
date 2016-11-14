using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XieHeProject
{
    class MyArray
    {
        /// <summary>
        /// 镜面翻转
        /// </summary>
        /// <param name="arry">彩色帧数组</param>
        /// <param name="index">每组索引头</param>
        /// <param name="length">每组长度</param>
        public static void Reverse(byte[] arry, int index, int length)
        {
            byte temp = 0;
            int n = -1;
            for (int i = index; i < index + length / 2; i++)
            {
                if (i % 4 == 0)
                {
                    n++;
                }
                temp = arry[i];
                arry[i] = arry[i + length - 4 - 8 * n];
                arry[i + length - 4 - 8 * n] = temp;
            }
        }
    }
}
