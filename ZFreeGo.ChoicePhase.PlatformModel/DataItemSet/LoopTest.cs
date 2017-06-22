using GalaSoft.MvvmLight;

namespace ZFreeGo.ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LoopTest : ObservableObject
    {
        private uint _coTime = 10;

        /// <summary>
        /// 合分时间间隔/s
        /// </summary>
        public uint CoTime
        {
            get
            {
                return _coTime;
            }
            set
            {
                _coTime = value;
                RaisePropertyChanged("CoTime");
            }
        }

        private uint _ocTime = 10;

        /// <summary>
        /// 分合时间间隔/s
        /// </summary>
        public uint OcTime
        {
            get
            {
                return _ocTime;
            }
            set
            {
                _ocTime = value;
                RaisePropertyChanged("OcTime");
            }
        }

        private uint _setCount = 1000;
        /// <summary>
        /// 设定次数
        /// </summary>
        public uint SetCount
        {
            get
            {
                return _setCount;
            }
            set
            {
                _setCount = value;
                RaisePropertyChanged("SetCount");
            }
        }

        private uint _currentCount = 1000;
        /// <summary>
        /// 设定次数
        /// </summary>
        public uint CurrentCount
        {
            get
            {
                return _currentCount;
            }
            set
            {
                _currentCount = value;
                RaisePropertyChanged("CurrentCount");
            }
        }

        private string _tips = "";
        /// <summary>
        /// 提示
        /// </summary>
        public string Tips
        {
            get
            {
                return _tips;
            }
            set
            {
                _tips = value;
                RaisePropertyChanged("Tips");
            }
        }

        private bool _enableChoice = true;

        public bool EnableChoice
        {
            get
            {
                return _enableChoice;
            }
            set
            {
                _enableChoice = value;
                RaisePropertyChanged("EnableChoice");
            }
        }
   
        /// <summary>
        /// Initializes a new instance of the LoopTest class.
        /// </summary>
        public LoopTest()
        {


        }
    }
}