using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAvi;
using SharpAvi.Output;
using SharpAvi.Codecs;

namespace XieHeProject
{
    class SharpAvi
    {
        /// <summary>
        /// Avi生成类
        /// </summary>
        private static AviWriter _writer = null;

        /// <summary>
        /// Avi视频流
        /// </summary>
        private static IAviVideoStream _stream = null;

        /// <summary>
        /// 是否创建文件
        /// </summary>
        private static bool _isCreateRecord = false;

        public static bool IsCreateRecord
        {
            get { return _isCreateRecord; }
            set { _isCreateRecord = value; }
        }

        /// <summary>
        /// 是否正在录像
        /// </summary>
        private static bool _isRecording = false;

        public static bool IsRecording
        {
            get { return _isRecording; }
            set { _isRecording = value; }
        }

        /// <summary>
        /// 是否已经结束录像
        /// </summary>
        private static bool _isStopRecord = false;

        public static bool IsStopRecord
        {
            get { return SharpAvi._isStopRecord; }
            set { SharpAvi._isStopRecord = value; }
        }

        /// <summary>
        /// 初始化Avi文件
        /// </summary>
        /// <param name="filePath">Avi文件路径</param>
        public void InitRecord(String filePath)
        {
            try
            {
                if (_writer == null)
                {
                    //在文件路径下创建AVI文件
                    _writer = new AviWriter(filePath)
                    {
                        FramesPerSecond = 30,
                        // Emitting AVI v1 index in addition to OpenDML index (AVI v2)
                        // improves compatibility with some software, including 
                        // standard Windows programs like Media Player and File Explorer
                        EmitIndex1 = true
                    };
                    //确定编码格式
                    _stream = _writer.AddMpeg4VideoStream(1920, 1080, 30,
                    quality: 70, codec: KnownFourCCs.Codecs.X264, forceSingleThreadedAccess: true);
                }
                
                //创建AVI文件完成，判断标记赋值
                IsCreateRecord = false;
            }
            catch 
            {
                Stoping();
                //System.Windows.MessageBox.Show(ex.Message);
                //Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// 录制Avi文件
        /// </summary>
        /// <param name="frameData">彩色帧数据</param>
        public void Recording(byte[] frameData)
        {
            try
            {
                //将2进制数据写入流中
                _stream.WriteFrame(true, // is key frame? (many codecs use concept of key frames, for others - all frames are keys)
                frameData, // array with frame data
                0, // starting index in the array
                frameData.Length // length of the data
                );
            }
            catch 
            {
                Stoping();
            }
       }

        /// <summary>
        /// 结束Avi录制
        /// </summary>
        public void Stoping()
        {
            if (_writer != null)
            {
                //关闭Writer，生成完整的，并且有时间轴的AVI文件
                _writer.Close();
                _writer = null;
                _stream = null;
            }
                //录制完毕，赋值标志位
            IsCreateRecord = false;
            IsRecording = false;
            IsStopRecord = false;
        }
    }
}
