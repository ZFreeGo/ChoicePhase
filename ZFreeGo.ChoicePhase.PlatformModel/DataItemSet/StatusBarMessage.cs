using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;

namespace ZFreeGo.ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 状态栏信息
    /// </summary>
    public class StatusBarMessage : ObservableObject
    {
        private string _icoOff = @"ICO/off1.png";
        private string _icoOn = @"ICO/on1.png";



        private const string Hidden = "Hidden";
        private const string Collapsed = "Collapsed";
        private const string Visible = "Visible";


        #region 底部状态栏
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


        private string  _comBrush;

        /// <summary>
        /// 通讯画刷
        /// </summary>
        public string  ComBrush
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

        
        private string  _deviceBrush;

         /// <summary>
        ///设备画刷
        /// </summary>
        public string  DeviceBrush
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



        private string  _phaseABrush;

        /// <summary>
        /// A相 画刷
        /// </summary>
        public string  PhaseABrush
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
        private string  _phaseBBrush;

        /// <summary>
        /// B相 画刷
        /// </summary>
        public string  PhaseBBrush
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

        private string  _phaseCBrush;

        /// <summary>
        /// C相 画刷
        /// </summary>
        public string  PhaseCBrush
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

        private string  _synBrush;

        /// <summary>
        /// A相 画刷
        /// </summary>
        public string  SynBrush
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
        

      
        private string _synICO;

        /// <summary>
        /// 同步画刷
        /// </summary>
        public string SynICO
        {
            get
            {
                return _synICO;
            }
            set
            {
                _synICO = value;
                RaisePropertyChanged("SynICO");
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

        private string _visibleB;
        /// <summary>
        /// B相可见性
        /// </summary>
        public string VisibleB
        {
            get
            {
                return _visibleB;
            }
            set
            {
                _visibleB = value;
                RaisePropertyChanged("VisibleB");
            }
        }

        private string _visibleC;
        /// <summary>
        /// C相可见性
        /// </summary>
        public string VisibleC
        {
            get
            {
                return _visibleC;
            }
            set
            {
                _visibleC = value;
                RaisePropertyChanged("VisibleC");
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
                ComBrush = "Green";
                ComState = "通讯打开";
            }
            else
            {
                ComState = "通讯关闭";
                ComBrush = "Red";
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
                DeviceBrush = "Green";
                DeviceState = "设备在线";
            }
            else
            {
                DeviceState = "设备离线";
                DeviceBrush = "Red";
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
                PhaseABrush = "Green";  
              
              
            }
            else
            {
                PhaseA = "A相离线";
                PhaseABrush = "Red";
                
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
                PhaseBBrush = "Green";
            }
            else
            {
                PhaseB = "B相离线";
                PhaseBBrush = "Red";
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
                PhaseCBrush = "Green";
            }
            else
            {
                PhaseC = "C相离线";
                PhaseCBrush = "Red";
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
                SynICO = _icoOn;
                SynBrush = "Green";
            }
            else
            {
                Syn = "同步控制器离线";
                SynICO = _icoOff;
                SynBrush = "Red";
            }
        }







        /// <summary>
        /// 关闭所有状态
        /// </summary>
        public void CloseAll()
        {
            ComState = "通讯关闭";
            ComBrush = "Red";

            DeviceState = "设备离线";
            DeviceBrush = "Red";

            PhaseA = "A相离线";
            PhaseABrush = "Red";
            PhaseB = "B相离线";
            PhaseBBrush = "Red";
            PhaseC = "C相离线";
            PhaseCBrush = "Red";
            Syn = "同步控制器离线";
            SynICO = _icoOff;
            SynBrush = "Red";


            for (int i = 0; i < _statusStyleBrush.Length; i++)
            {
                _statusStyleBrush[i] = "Red";
                _statusStyleDescribe[i] = "离线";
            }
        }

        #endregion


       
        /// <summary>
        /// 状态描述
        /// </summary>
        private string[] _statusStyleDescribe;

        public string[] StatusStyleDescribe
        {
            get
            {
                return _statusStyleDescribe;
            }
            set
            {
                _statusStyleDescribe = value;
                RaisePropertyChanged("StatusStyleDescribe");
            }
        }
        /// <summary>
        /// 状态类型画刷
        /// </summary>
        private string[] _statusStyleBrush;


        public string[] StatusStyleBrush
        {
            get
            {
                return _statusStyleBrush;
            }
            set
            {
                _statusStyleBrush = value;
                RaisePropertyChanged("StatusStyleBrush");
            }
        }

        #region 远方/就地
        /// <summary>
        /// 远方/就地 A相 描述
        /// </summary>
        public string RemoteLocalA
        {
            get
            {
                return _statusStyleDescribe[0];
            }
            set
            {
                _statusStyleDescribe[0] = value;
                RaisePropertyChanged("RemoteLocalA");
            }
            
        }
        /// <summary>
        /// 远方/就地 B相 描述
        /// </summary>
        public string RemoteLocalB
        {
            get
            {
                return _statusStyleDescribe[1];
            }
            set
            {
                _statusStyleDescribe[1] = value;
                RaisePropertyChanged("RemoteLocalB");
            }

        }
        /// <summary>
        /// 远方/就地 C相 描述
        /// </summary>
        public string RemoteLocalC
        {
            get
            {
                return _statusStyleDescribe[2];
            }
            set
            {
                _statusStyleDescribe[2] = value;
                RaisePropertyChanged("RemoteLocalC");
            }

        }

        /// <summary>
        /// 远方/就地 A相 描述
        /// </summary>
        public string RemoteLocalBrushA
        {
            get
            {
                return _statusStyleBrush[0];
            }
            set
            {
                _statusStyleBrush[0] = value;
                RaisePropertyChanged("RemoteLocalBrushA");
            }
        }
        /// <summary>
        /// 远方/就地 B相 画刷
        /// </summary>
        public string RemoteLocalBrushB
        {
            get
            {
                return _statusStyleBrush[1];
            }
            set
            {
                _statusStyleBrush[1] = value;
                RaisePropertyChanged("RemoteLocalBrushB");
            }

        }
        /// <summary>
        /// 远方/就地 C相 画刷
        /// </summary>
        public string RemoteLocalBrushC
        {
            get
            {
                return _statusStyleBrush[2];
            }
            set
            {
                _statusStyleBrush[2] = value;
                RaisePropertyChanged("RemoteLocalBrushC");
            }

        }
        #endregion

        #region 调试/工作
        /// <summary>
        /// 调试工作 A
        /// </summary>
        public string DebugWorkA
        {
            get
            {
                return _statusStyleDescribe[3];
            }
            set
            {
                _statusStyleDescribe[3] = value;
                RaisePropertyChanged("DebugWorkA");
            }

        }
        /// <summary>
        /// 调试工作 B
        /// </summary>
        public string DebugWorkB
        {
            get
            {
                return _statusStyleDescribe[4];
            }
            set
            {
                _statusStyleDescribe[4] = value;
                RaisePropertyChanged("DebugWorkB");
            }

        }
        /// <summary>
        /// 调试工作 C
        /// </summary>
        public string DebugWorkC
        {
            get
            {
                return _statusStyleDescribe[5];
            }
            set
            {
                _statusStyleDescribe[5] = value;
                RaisePropertyChanged("DebugWorkC");
            }

        }


        /// <summary>
        /// 调试工作 A 画刷
        /// </summary>
        public string DebugWorkBrushA
        {
            get
            {
                return _statusStyleBrush[3];
            }
            set
            {
                _statusStyleBrush[3] = value;
                RaisePropertyChanged("DebugWorkBrushA");
            }

        }
        /// <summary>
        /// 调试工作 B 画刷
        /// </summary>
        public string DebugWorkBrushB
        {
            get
            {
                return _statusStyleBrush[4];
            }
            set
            {
                _statusStyleBrush[4] = value;
                RaisePropertyChanged("DebugWorkBrushB");
            }

        }
        /// <summary>
        /// 调试工作 C 画刷
        /// </summary>
        public string DebugWorkBrushC
        {
            get
            {
                return _statusStyleBrush[5];
            }
            set
            {
                _statusStyleBrush[5] = value;
                RaisePropertyChanged("DebugWorkBrushC");
            }
        }


        #endregion

        #region 数据载入
        /// <summary>
        /// 数据载入 A
        /// </summary>
        public string LoadA
        {
            get
            {
                return _statusStyleDescribe[6];
            }
            set
            {
                _statusStyleDescribe[6] = value;
                RaisePropertyChanged("LoadA");
            }

        }
        /// <summary>
        /// 数据载入 B
        /// </summary>
        public string LoadB
        {
            get
            {
                return _statusStyleDescribe[7];
            }
            set
            {
                _statusStyleDescribe[7] = value;
                RaisePropertyChanged("LoadB");
            }

        }

        /// <summary>
        /// 数据载入 C
        /// </summary>
        public string LoadC
        {
            get
            {
                return _statusStyleDescribe[8];
            }
            set
            {
                _statusStyleDescribe[8] = value;
                RaisePropertyChanged("LoadC");
            }

        }

        /// <summary>
        /// 数据载入 A 画刷
        /// </summary>
        public string LoadBrushA
        {
            get
            {
                return _statusStyleBrush[6];
            }
            set
            {
                _statusStyleBrush[6] = value;
                RaisePropertyChanged("LoadBrushA");
            }

        }
        /// <summary>
        /// 数据载入 B 画刷
        /// </summary>
        public string LoadBrushB
        {
            get
            {
                return _statusStyleBrush[7];
            }
            set
            {
                _statusStyleBrush[7] = value;
                RaisePropertyChanged("LoadBrushB");
            }
        }
        /// <summary>
        /// 数据载入 C 画刷
        /// </summary>
        public string LoadBrushC
        {
            get
            {
                return _statusStyleBrush[8];
            }
            set
            {
                _statusStyleBrush[8] = value;
                RaisePropertyChanged("LoadBrushC");
            }
        }
      
        #endregion


        #region 同步控制器



        /// <summary>
        /// 系统电压
        /// </summary>
        public string Voltage
        {
            get
            {
                return _statusStyleDescribe[9];
            }
            set
            {
                _statusStyleDescribe[9] = value;
                RaisePropertyChanged("Voltage");
            }
        }
        /// <summary>
        /// 系统电压 brush
        /// </summary>
        public string VoltageBrush
        {
            get
            {
                return _statusStyleBrush[9];
            }
            set
            {
                _statusStyleBrush[9] = value;
                RaisePropertyChanged("VoltageBrush");
            }
        }
        /// <summary>
        /// 更新电压状态
        /// </summary>
        public void UpadateVoltageStatus(EnergyStatusLoop status)
        {
           
            switch(status)
            {
                case EnergyStatusLoop.More:
                    {
                        Voltage = "系统电压过高";
                        VoltageBrush = "Red";
                        break;
                    }
                case EnergyStatusLoop.Less:
                    {
                        Voltage = "系统电压过低";
                        VoltageBrush = "Orange";
                        break;
                    }
                case EnergyStatusLoop.Null:
                    {
                        Voltage = "系统电压空";
                        VoltageBrush = "Red";
                        break;
                    }
                case EnergyStatusLoop.Normal:
                    {
                        Voltage = "系统电压正常";
                        VoltageBrush = "Green";
                        break;
                    }
            }
        }

        /// <summary>
        /// 系统频率
        /// </summary>
        public string Frequency
        {
            get
            {
                return _statusStyleDescribe[9];
            }
            set
            {
                _statusStyleDescribe[9] = value;
                RaisePropertyChanged("Frequency");
            }
        }

       
        /// <summary>
        /// 系统频率 brush
        /// </summary>
        public string FrequencyBrush
        {
            get
            {
                return _statusStyleBrush[9];
            }
            set
            {
                _statusStyleBrush[9] = value;
                RaisePropertyChanged("FrequencyBrush");
            }
        }
        /// <summary>
        /// 更新频率状态
        /// </summary>
        /// <param name="status"></param>
        public void UpadateFrequencyStatus(double freq)
        {
            EnergyStatusLoop status;


            if (freq > 52)
            {
                status = EnergyStatusLoop.More;
            }
            else if (freq >= 48)
            {
                status = EnergyStatusLoop.Normal;
            }
            else
            {
                status = EnergyStatusLoop.Less;
            }
            switch (status)
            {
                case EnergyStatusLoop.More:
                    {
                        Voltage = "系统频率过高";
                        VoltageBrush = "Red";
                        break;
                    }
                case EnergyStatusLoop.Less:
                    {
                        Voltage = "系统频率过低";
                        VoltageBrush = "Orange";
                        break;
                    }
                case EnergyStatusLoop.Null:
                    {
                        Voltage = "系统频率空";
                        VoltageBrush = "Red";
                        break;
                    }
                case EnergyStatusLoop.Normal:
                    {
                        Voltage = "系统频率正常";
                        VoltageBrush = "Green";
                        break;
                    }
            }
        }
        #endregion

        /// <summary>
        /// 初始化状态栏信息
        /// </summary>
        /// <param name="name"></param>
        public  StatusBarMessage(string name)
        {
            _userName = name;
            _comState = "通讯关闭";
            _comBrush = "Red";

            _deviceState = "设备离线";
            _deviceBrush = "Red";

            _phaseA = "A相离线";
            _phaseABrush = "Red";
            _phaseB = "B相离线";
            _phaseBBrush = "Red";
            _phaseC = "C相离线";
            _phaseCBrush = "Red";
            _syn = "同步控制器离线";
            _synICO = _icoOff;
            _synBrush = "Red";


            _statusStyleDescribe = new string[11];
            _statusStyleBrush = new string[11];

            for (int i = 0; i < _statusStyleBrush.Length; i++)
            {
                _statusStyleBrush[i] = "Red";
                _statusStyleDescribe[i] = "离线";
            }
            _statusStyleBrush[1] = "Green";
            //单相模式不可见
            if (NodeAttribute.SingleMode)
            {
                _visibleB = Collapsed;
                _visibleC = Collapsed;
            }
            else
            {
                _visibleB = Visible;
                _visibleC = Visible;
            }
        }
    }
}
