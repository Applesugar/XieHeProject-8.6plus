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

namespace XieHeProject
{
    /// <summary>
    /// MainPageWindows.xaml 的交互逻辑
    /// </summary>
    public partial class MainPageWindows : Window
    {
        public MainPageWindows()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开视频录制窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoRecordButton_Click(object sender, RoutedEventArgs e)
        {
            GetWPFWindows.GetMainWindow();
        }

        /// <summary>
        /// 打开功能评分窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SorceButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"..\..\..\..\XieHePlayer\XieHePlayer\bin\Debug\XieHePlayer.exe");
        }
        /// <summary>
        /// 打开步态参数计算窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GitParamsButton_Click(object sender, RoutedEventArgs e)
        {
            GetWPFWindows.GetGaitWindow();
        }
    }
}
