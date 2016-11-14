using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Tools;
using System.Windows;

namespace XieHeProject
{
    class KinectVideo
    {
        /// <summary>
        /// 判断是否在录制视频的标识
        /// </summary>
        private static int _flag= 0;

        /// <summary>
        /// 控制出错后是否改变_flag
        /// </summary>
        private static int _flag1 = 0;

        /// <summary>
        /// 是否回放xef文件
        /// </summary>
        private static int _flag2 = 0;

        public static int Flag2
        {
            get { return KinectVideo._flag2; }
            set { KinectVideo._flag2 = value; }
        }

        /// <summary>
        /// 开始录制xef文件
        /// </summary>
        /// <param name="filePath">xef文件路径</param>
        public void StartRecordClip(object filePath)
        {
            try
            {
                using (KStudioClient client = KStudio.CreateClient())
                {
                    client.ConnectToService();
                    KStudioEventStreamSelectorCollection streamCollection = new KStudioEventStreamSelectorCollection();
                    //streamCollection.Add(KStudioEventStreamDataTypeIds.Ir);
                    streamCollection.Add(KStudioEventStreamDataTypeIds.Depth);
                    streamCollection.Add(KStudioEventStreamDataTypeIds.Body);
                    streamCollection.Add(KStudioEventStreamDataTypeIds.BodyIndex);
                    //streamCollection.Add(KStudioEventStreamDataTypeIds.UncompressedColor);
                    using (KStudioRecording recording = client.CreateRecording((string)filePath, streamCollection))
                    {

                        recording.Start();
                        SharpAvi.IsCreateRecord = true;
                        SharpAvi.IsRecording = true;
                        while (recording.State == KStudioRecordingState.Recording)
                        {
                            //Flag为1时调出循环，结束录像
                            if (_flag == 1)
                                break;
                        }
                        SharpAvi.IsStopRecord = true;
                        recording.Stop();
                        recording.Dispose();

                    }
                    client.DisconnectFromService();
                    client.Dispose();
                    _flag = 0;
                }
            }
            catch
            {
                _flag = 0;
                _flag1 = 1;
                SharpAvi.IsStopRecord = true;
                MessageBox.Show("视频录制出现异常");
            }
        }

        /// <summary>
        /// 结束xef文件录制
        /// </summary>
        public void StopRecordClip()
        {
            //没有异常
            if (_flag1 != 1)
            {
                //_flag为1调出循环，停止录像
                _flag = 1;
            }
            else//有异常
            {
                _flag1 = 0;
            }
        }

        /// <summary>
        /// 播放xef视频
        /// </summary>
        /// <param name="filePath">xef视频路径</param>
         public void PlaybackClip(object filePath)
        {


             ///以前的方法，不能加骨骼上去
            #region
            using (KStudioClient PlayClient = KStudio.CreateClient())
            {
                PlayClient.ConnectToService();
                using (KStudioPlayback playback = PlayClient.CreatePlayback((string)filePath))
                {

                    playback.LoopCount = 0;
                    playback.Start();
                    Flag2 = 0;
                    while (playback.State == KStudioPlaybackState.Playing)
                    {

                    }
                    if (playback.State == KStudioPlaybackState.Error)
                    {
                        Flag2 = 1;
                        throw new InvalidOperationException("Error: Playback failed!");
                    }
                    Flag2 = 1;
                    playback.Stop();
                    playback.Dispose();
                }
                PlayClient.DisconnectFromService();
                System.Windows.Forms.MessageBox.Show("回放结束");
            }
            #endregion
        }
    }
}
