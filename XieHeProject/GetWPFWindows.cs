using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XieHeProject
{
    class GetWPFWindows
    {
        /// <summary>
        /// 视频录制窗口单例
        /// </summary>
        private static MainWindow _mwindow;

        public static MainWindow Mwindow
        {
            get { return GetWPFWindows._mwindow; }
            set { GetWPFWindows._mwindow = value; }
        }

        /// <summary>
        /// 步态参数计算窗口单例
        /// </summary>
        private static GaitParamWindow _gwindow;

        public static GaitParamWindow Gwindow
        {
            get { return GetWPFWindows._gwindow; }
            set { GetWPFWindows._gwindow = value; }
        }

        /// <summary>
        /// 私有化构造参数
        /// </summary>
        private GetWPFWindows()
        {
        
        }

        /// <summary>
        /// 单例打开视频录制窗口
        /// </summary>
        public static void GetMainWindow()
        {
            if (_mwindow == null)
            {
                _mwindow = new MainWindow();
                _mwindow.Show();
            }
            else
            {
                if (_mwindow.IsLoaded)
                {
                    //Console.WriteLine("You are Right");
                }
                else
                {
                    _mwindow =new MainWindow();
                    _mwindow.Show();
                }
            }
        }

        /// <summary>
        /// 单例打开步态参数
        /// </summary>
        public static void GetGaitWindow()
        {
            if (_gwindow == null)
            {
                _gwindow = new GaitParamWindow();
                _gwindow.Show();
            }
            else
            {
                if (_gwindow.IsLoaded)
                {
                    //Console.WriteLine("You are Right");
                }
                else
                {
                    _gwindow = new GaitParamWindow();
                    _gwindow.Show();
                }
            }
        }

        
    }
}
