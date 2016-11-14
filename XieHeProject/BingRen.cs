using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XieHeProject
{
    class BingRen
    {
        /// <summary>
        /// 视频文件根目录
        /// </summary>
        private static string _rootDir = "";

        public static string RootDir
        {
            get { return BingRen._rootDir; }
            set { BingRen._rootDir = value; }
        }

        /// <summary>
        /// 病人的ID号
        /// </summary>
        private static string _peopleId = "";

        public static string PeopleId
        {
            get { return BingRen._peopleId; }
            set { BingRen._peopleId = value; }
        }

        /// <summary>
        /// 病人的文件夹
        /// </summary>
        private static string _peopleDir = "";

        public static string PeopleDir
        {
            get { return BingRen._peopleDir; }
            set { BingRen._peopleDir = value; }
        }

        /// <summary>
        /// 病人当前测试项目
        /// </summary>
        private static string _currentProject = "";

        public static string CurrentProject
        {
            get { return BingRen._currentProject; }
            set { BingRen._currentProject = value; }
        }

        /// <summary>
        /// 平均速度
        /// </summary>
        private static double _stepSpeed = 0;

        public static double StepSpeed
        {
            get { return _stepSpeed; }
            set { _stepSpeed = value; }
        }

        /// <summary>
        /// 平均步宽
        /// </summary>
        private static double _stepWidth = 0;

        public static double StepWidth
        {
            get { return _stepWidth; }
            set { _stepWidth = value; }
        }

        /// <summary>
        /// 平均左步高
        /// </summary>
        private static double _leftstepHeight = 0;

        public static double LeftstepHeight
        {
            get { return _leftstepHeight; }
            set { _leftstepHeight = value; }
        }

        /// <summary>
        /// 平均右步高
        /// </summary>
        private static double _rightstepHeight = 0;

        public static double RightstepHeight
        {
            get { return _rightstepHeight; }
            set { _rightstepHeight = value; }
        }

        /// <summary>
        /// 平均左步长
        /// </summary>
        private static double _leftstepLength = 0;

        public static double LeftstepLength
        {
            get { return _leftstepLength; }
            set { _leftstepLength = value; }
        }

        /// <summary>
        /// 平均右步长
        /// </summary>
        private static double _rightstepLength = 0;

        public static double RightstepLength
        {
            get { return _rightstepLength; }
            set { _rightstepLength = value; }
        }

        /// <summary>
        /// 走了多少歩
        /// </summary>
        private static double _stepCount = 0;

        public static double StepCount
        {
            get { return _stepCount; }
            set { _stepCount = value; }
        }

        /// <summary>
        /// 右步速均值
        /// </summary>
        private static double _rightSpeedM = 0;

        public static double RightSpeedM
        {
            get { return BingRen._rightSpeedM; }
            set { BingRen._rightSpeedM = value; }
        }

        /// <summary>
        /// 右步速方差
        /// </summary>
        private static double _rightSpeedD = 0;

        public static double RightSpeedD
        {
            get { return BingRen._rightSpeedD; }
            set { BingRen._rightSpeedD = value; }
        }


        /// <summary>
        /// 左步速均值
        /// </summary>
        private static double _leftSpeedM = 0;

        public static double LeftSpeedM
        {
            get { return BingRen._leftSpeedM; }
            set { BingRen._leftSpeedM = value; }
        }

        /// <summary>
        /// 左步速方差
        /// </summary>
        private static double _leftSpeedD = 0;

        public static double LeftSpeedD
        {
            get { return BingRen._leftSpeedD; }
            set { BingRen._leftSpeedD = value; }
        }

        /// <summary>
        /// 步行周期速度均值
        /// </summary>
        private static double _cyleSpeedM = 0;

        public static double CyleSpeedM
        {
            get { return BingRen._cyleSpeedM; }
            set { BingRen._cyleSpeedM = value; }
        }

        /// <summary>
        /// 步行周期速度方差
        /// </summary>
        private static double _cyleSpeedD = 0;

        public static double CyleSpeedD
        {
            get { return BingRen._cyleSpeedD; }
            set { BingRen._cyleSpeedD = value; }
        }

        /// <summary>
        /// 右步长的均值
        /// </summary>
        private static double _rightStepLengthM = 0;

        public static double RightStepLengthM
        {
            get { return BingRen._rightStepLengthM; }
            set { BingRen._rightStepLengthM = value; }
        }

        /// <summary>
        /// 右步长的方差
        /// </summary>
        private static double _rightStepLengthD = 0;

        public static double RightStepLengthD
        {
            get { return BingRen._rightStepLengthD; }
            set { BingRen._rightStepLengthD = value; }
        }

        /// <summary>
        /// 左步长的均值
        /// </summary>
        private static double _leftStepLengthM = 0;

        public static double LeftStepLengthM
        {
            get { return BingRen._leftStepLengthM; }
            set { BingRen._leftStepLengthM = value; }
        }

        /// <summary>
        /// 左步长的方差
        /// </summary>
        private static double _leftStepLengthD = 0;

        public static double LeftStepLengthD
        {
            get { return BingRen._leftStepLengthD; }
            set { BingRen._leftStepLengthD = value; }
        }

        /// <summary>
        /// 左右步长的协调性
        /// </summary>
        private static double _stepLengthBalance = 0;

        public static double StepLengthBalance
        {
            get { return BingRen._stepLengthBalance; }
            set { BingRen._stepLengthBalance = value; }
        }

        /// <summary>
        /// 右步高均值
        /// </summary>
        private static double _rightStepHeightM = 0;

        public static double RightStepHeightM
        {
            get { return BingRen._rightStepHeightM; }
            set { BingRen._rightStepHeightM = value; }
        }

        /// <summary>
        /// 右步高方差
        /// </summary>
        private static double _rightStepHeightD = 0;

        public static double RightStepHeightD
        {
            get { return BingRen._rightStepHeightD; }
            set { BingRen._rightStepHeightD = value; }
        }

        /// <summary>
        /// 左步高均值
        /// </summary>
        private static double _leftStepHeightM = 0;

        public static double LeftStepHeightM
        {
            get { return BingRen._leftStepHeightM; }
            set { BingRen._leftStepHeightM = value; }
        }

        /// <summary>
        /// 左步高方差
        /// </summary>
        private static double _leftStepHeightD = 0;

        public static double LeftStepHeightD
        {
            get { return BingRen._leftStepHeightD; }
            set { BingRen._leftStepHeightD = value; }
        }

        /// <summary>
        /// 左右步高的协调性
        /// </summary>
        private static double _stepHeightBalance = 0;

        public static double StepHeightBalance
        {
            get { return BingRen._stepHeightBalance; }
            set { BingRen._stepHeightBalance = value; }
        }

        /// <summary>
        /// 步宽均值
        /// </summary>
        private static double _stepWidthM = 0;

        public static double StepWidthM
        {
            get { return BingRen._stepWidthM; }
            set { BingRen._stepWidthM = value; }
        }

        /// <summary>
        /// 步宽方差
        /// </summary>
        private static double _stepWidthD = 0;

        public static double StepWidthD
        {
            get { return BingRen._stepWidthD; }
            set { BingRen._stepWidthD = value; }
        }


        /// <summary>
        /// 步距均值
        /// </summary>
        private static double _stepDistanceM = 0;

        public static double StepDistanceM
        {
            get { return BingRen._stepDistanceM; }
            set { BingRen._stepDistanceM = value; }
        }

        /// <summary>
        /// 步距方差
        /// </summary>
        private static double _stepDistanceD = 0;

        public static double StepDistanceD
        {
            get { return BingRen._stepDistanceD; }
            set { BingRen._stepDistanceD = value; }
        }

        /// <summary>
        /// 身体上部的z角度均值
        /// </summary>
        private static double _zM = 0;

        public static double ZM
        {
            get { return BingRen._zM; }
            set { BingRen._zM = value; }
        }

        /// <summary>
        /// 身体上部的z角度方差
        /// </summary>
        private static double _zD = 0;

        public static double ZD
        {
            get { return BingRen._zD; }
            set { BingRen._zD = value; }
        }

        /// <summary>
        /// 右脚变异度
        /// </summary>
        private static double _rightFootAberrance = 0;

        public static double RightFootAberrance
        {
            get { return BingRen._rightFootAberrance; }
            set { BingRen._rightFootAberrance = value; }
        }

        /// <summary>
        /// 左脚变异度
        /// </summary>
        private static double _leftFootAberrance = 0;

        public static double LeftFootAberrance
        {
            get { return BingRen._leftFootAberrance; }
            set { BingRen._leftFootAberrance = value; }
        }

        

        ///<summary>
        ///the features in Normal Stand Up
        ///</summary>
        private static double _NSUVelocityUp = 0;

        public static double NSUVelocityUp
        {
            get
            {
                return _NSUVelocityUp;
            }

            set
            {
                _NSUVelocityUp = value;
            }
        }

        private static double _NSUVelocityDown = 0;

        public static double NSUVelocityDown
        {
            get
            {
                return _NSUVelocityDown;
            }

            set
            {
                _NSUVelocityDown = value;
            }
        }

        private static double _NSUAccelerationUp = 0;

        public static double NSUAccelerationUp
        {
            get
            {
                return _NSUAccelerationUp;
            }

            set
            {
                _NSUAccelerationUp = value;
            }
        }

        private static double _NSUAccelerationDown = 0;

        public static double NSUAccelerationDown
        {
            get
            {
                return _NSUAccelerationDown;
            }

            set
            {
                _NSUAccelerationDown = value;
            }
        }

        

        ///<summary>
        ///features in the Cross Stand Up
        ///</summary>


        private static double _CSUVelocityUp = 0;
        public static double CSUVelocityUp
        {
            get
            {
                return _CSUVelocityUp;
            }

            set
            {
                _CSUVelocityUp = value;
            }
        }
        private static double _CSUVelocityUp_variance = 0;

        public static double CSUVelocityUp_variance
        {
            get
            {
                return _CSUVelocityUp_variance;
            }

            set
            {
                _CSUVelocityUp_variance = value;
            }
        }
        private static double _CSUVelocityDown = 0;

        public static double CSUVelocityDown
        {
            get
            {
                return _CSUVelocityDown;
            }

            set
            {
                _CSUVelocityDown = value;
            }
        }
        private static double _CSUVelocityDown_variance = 0;

        public static double CSUVelocityDown_variance
        {
            get
            {
                return _CSUVelocityDown_variance;
            }

            set
            {
                _CSUVelocityDown_variance = value;
            }
        }
        private static double _CSUAccelerationUp = 0;

        public static double CSUAccelerationUp
        {
            get
            {
                return _CSUAccelerationUp;
            }

            set
            {
                _CSUAccelerationUp = value;
            }
        }
        private static double _CSUAccelerationUp_variance = 0;

        public static double CSUAccelerationUp_variance
        {
            get
            {
                return _CSUAccelerationUp_variance;
            }

            set
            {
                _CSUAccelerationUp_variance = value;
            }
        }
        private static double _CSUAccelerationDown = 0;

        public static double CSUAccelerationDown
        {
            get
            {
                return _CSUAccelerationDown;
            }

            set
            {
                _CSUAccelerationDown = value;
            }
        }
        private static double _CSUAccelerationDown_variance = 0;

        public static double CSUAccelerationDown_variance
        {
            get
            {
                return _CSUAccelerationDown_variance;
            }

            set
            {
                _CSUAccelerationDown_variance = value;
            }
        }
        private static double _CSUVelocityOneCircle = 0;

        public static double CSUVelocityOneCircle
        {
            get
            {
                return _CSUVelocityOneCircle;
            }

            set
            {
                _CSUVelocityOneCircle = value;
            }
        }
        private static double _CSUVelocityOneCircle_variance = 0;

        public static double CSUVelocityOneCircle_variance
        {
            get
            {
                return _CSUVelocityOneCircle_variance;
            }

            set
            {
                _CSUVelocityOneCircle_variance = value;
            }
        }
        private static double _CSUAccelerationOneCircle = 0;

        public static double CSUAccelerationOneCircle
        {
            get
            {
                return _CSUAccelerationOneCircle;
            }

            set
            {
                _CSUAccelerationOneCircle = value;
            }
        }
        private static double _CSUAccelerationOneCircle_variance = 0;

        public static double CSUAccelerationOneCircle_variance
        {
            get
            {
                return _CSUAccelerationOneCircle_variance;
            }

            set
            {
                _CSUAccelerationOneCircle_variance = value;
            }
        }
        private static double _CSUVelocityWholeAction = 0;

        public static double CSUVelocityWholeAction
        {
            get
            {
                return _CSUVelocityWholeAction;
            }

            set
            {
                _CSUVelocityWholeAction = value;
            }
        }
        private static double _CSUVelocityWholeAction_variance = 0;

        public static double CSUVelocityWholeAction_variance
        {
            get
            {
                return _CSUVelocityWholeAction_variance;
            }

            set
            {
                _CSUVelocityWholeAction_variance = value;
            }
        }
        private static double _CSUAccelerationWholeAction = 0;

        public static double CSUAccelerationWholeAction
        {
            get
            {
                return _CSUAccelerationWholeAction;
            }

            set
            {
                _CSUAccelerationWholeAction = value;
            }
        }
        private static double _CSUAccelerationWholeAction_variance = 0;

        public static double CSUAccelerationWholeAction_variance
        {
            get
            {
                return _CSUAccelerationWholeAction_variance;
            }

            set
            {
                _CSUAccelerationWholeAction_variance = value;
            }
        }



        /// <summary>
        /// 得视频文件中病人的id
        /// </summary>
        /// <param name="videofilename">视频文件的文件名</param>
        /// <returns></returns>
        public static string GetBingRenVideoFileId(string videofilename)
        {
            string id = "";
            try
            {
                string[] spiltresult = videofilename.Split('_');
                id = spiltresult[0];
            }
            catch
            {
                id = "载入的视频名称有误";
            }
            return id;
        }

    }
}
