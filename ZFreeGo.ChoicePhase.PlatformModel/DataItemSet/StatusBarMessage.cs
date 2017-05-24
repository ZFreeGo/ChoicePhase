using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ZFreeGo.ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 状态栏信息
    /// </summary>
    public class StatusBarMessage : ObservableObject
    {
        private string _userName;

        /// <summary>
        /// 用户名
        /// </summary>
        public String UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }


        private String _comState;
        /// <summary>
        /// 通讯状态
        /// </summary>
        public String ComState
        {
            get
            {
                return _comState;
            }
            set
            {
                _comState = value;
                RaisePropertyChanged("ComState");
            }
        }


        private SolidColorBrush _comBrush;

        /// <summary>
        /// 通讯画刷
        /// </summary>
        public SolidColorBrush ComBrush
        {
            get
            {
                return _comBrush;
            }
            set
            {
                _comBrush = value;
                RaisePropertyChanged("ComBrush");
            }
        }


        private string _deviceState;

        /// <summary>
        /// 设备状态
        /// </summary>
        public String DeviceState
        {
            get
            {
                return _deviceState;
            }
            set
            {
                _deviceState = value;
                RaisePropertyChanged("DeviceState");
            }
        }

        
        private SolidColorBrush _deviceBrush;

         /// <summary>
        ///设备画刷
        /// </summary>
        public SolidColorBrush DeviceBrush
        {
            get
            {
                return _deviceBrush;
            }
            set
            {
                _deviceBrush = value;
                RaisePropertyChanged("DeviceBrush");
            }
        }



        private SolidColorBrush _phaseABrush;

        /// <summary>
        /// A相 画刷
        /// </summary>
        public SolidColorBrush PhaseABrush
        {
            get
            {
                return _phaseABrush;
            }
            set
            {
                _phaseABrush = value;
                RaisePropertyChanged("PhaseABrush");
            }
        }

        private String _phaseA;
        /// <summary>
        /// A相 描述
        /// </summary>
        public String PhaseA
        {
            get
            {
                return _phaseA;
            }
            set
            {
                _phaseA = value;
                RaisePropertyChanged("PhaseA");
            }
        }
        private SolidColorBrush _phaseBBrush;

        /// <summary>
        /// B相 画刷
        /// </summary>
        public SolidColorBrush PhaseBBrush
        {
            get
            {
                return _phaseBBrush;
            }
            set
            {
                _phaseBBrush = value;
                RaisePropertyChanged("PhaseBBrush");
            }
        }

        private String _phaseB;
        /// <summary>
        /// B相 描述
        /// </summary>
        public String PhaseB
        {
            get
            {
                return _phaseB;
            }
            set
            {
                _phaseB = value;
                RaisePropertyChanged("PhaseB");
            }
        }

        private SolidColorBrush _phaseCBrush;

        /// <summary>
        /// C相 画刷
        /// </summary>
        public SolidColorBrush PhaseCBrush
        {
            get
            {
                return _phaseCBrush;
            }
            set
            {
                _phaseCBrush = value;
                RaisePropertyChanged("PhaseCBrush");
            }
        }

        private String _phaseC;
        /// <summary>
        /// C相 描述
        /// </summary>
        public String PhaseC
        {
            get
            {
                return _phaseC;
            }
            set
            {
                _phaseC = value;
                RaisePropertyChanged("PhaseC");
            }
        }

        private SolidColorBrush _synBrush;

        /// <summary>
        /// A相 画刷
        /// </summary>
        public SolidColorBrush SynBrush
        {
            get
            {
                return _synBrush;
            }
            set
            {
                _synBrush = value;
                RaisePropertyChanged("SynBrush");
            }
        }

        private String _syn;
        /// <summary>
        /// 同步 描述
        /// </summary>
        public String Syn
        {
            get
            {
                return _syn;
            }
            set
            {
                _syn = value;
                RaisePropertyChanged("Syn");
            }
        }

        /// <summary>
        /// 设置通讯状态
        /// </summary>
        /// <param name="state">true-激活，false-关闭</param>
        public void SetCom(bool state)
        {
            if (state)
            {
                ComBrush = new SolidColorBrush(Colors.Green);
                ComState = "通讯打开";
            }
            else
            {
                ComState = "通讯关闭";
                ComBrush = new SolidColorBrush(Colors.Red);
            }
        }

        /// <summary>
        /// 设置设备状态
        /// </summary>
        /// <param name="state">true-激活，false-关闭</param>
        public void SetDevice(bool state)
        {
            if (state)
            {
                DeviceBrush = new SolidColorBrush(Colors.Green);
                DeviceState = "设备在线";
            }
            else
            {
                DeviceState = "设备离线";
                DeviceBrush = new SolidColorBrush(Colors.Red);
            }
        }
        /// <summary>
        /// 设置A相
        /// </summary>
        /// <param name="state">true-激活，false-关闭</param>
        public void SetPhaseA(bool state, string comment)
        {
            if (state)
            {
                PhaseA = comment;
                PhaseABrush = new SolidColorBrush(Colors.Green);                
            }
            else
            {
                PhaseA = "A相离线";
                PhaseABrush = new SolidColorBrush(Colors.Red);
            }
        }
        /// <summary>
        /// 设置B相
        /// </summary>
        /// <param name="state">true-激活，false-关闭</param>
        public void SetPhaseB(bool state, string comment)
        {
            if (state)
            {
                PhaseB = comment;
                PhaseBBrush = new SolidColorBrush(Colors.Green);
            }
            else
            {
                PhaseB = "B相离线";
                PhaseBBrush = new SolidColorBrush(Colors.Red);
            }
        }
        /// <summary>
        /// 设置C相
        /// </summary>
        /// <param name="state">true-激活，false-关闭</param>
        public void SetPhaseC(bool state, string comment)
        {
            if (state)
            {
                PhaseC = comment;
                PhaseCBrush = new SolidColorBrush(Colors.Green);
            }
            else
            {
                PhaseC = "C相离线";
                PhaseCBrush = new SolidColorBrush(Colors.Red);
            }
        }
        /// <summary>
        /// 设置同步控制器
        /// </summary>
        /// <param name="state">true-激活，false-关闭</param>
        public void SetSyn(bool state, string comment)
        {
            if (state)
            {
                Syn = comment;
                SynBrush = new SolidColorBrush(Colors.Green);
            }
            else
            {
                Syn = "同步控制器离线";
                SynBrush = new SolidColorBrush(Colors.Red);
            }
        }
        /// <summary>
        /// 初始化状态栏信息
        /// </summary>
        /// <param name="name"></param>
        public  StatusBarMessage(string name)
        {
            _userName = name;
            _comState = "通讯关闭";
            _comBrush = new SolidColorBrush(Colors.Red);

            _deviceState = "设备离线";
            _deviceBrush = new SolidColorBrush(Colors.Red);

            _phaseA = "A相离线";
            _phaseABrush = new SolidColorBrush(Colors.Red);
            _phaseB = "B相离线";
            _phaseBBrush = new SolidColorBrush(Colors.Red);
            _phaseC = "C相离线";
            _phaseCBrush = new SolidColorBrush(Colors.Red);
            _syn = "同步控制器离线";
            _synBrush = new SolidColorBrush(Colors.Red);


        }



    }
}
