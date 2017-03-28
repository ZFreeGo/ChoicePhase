using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace ZFreeGo.ChoicePhase.ControlPlatform.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ControlViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ControlViewModel class.
        /// </summary>
        public ControlViewModel()
        {
             _actionSelect = new ObservableCollection<string>();
             _actionSelect.Add("合闸");
             _actionSelect.Add("分闸");

             ActionCommand = new RelayCommand<string>(ExecuteReadyCommand);

        }

        #region 合分闸控制，同步预制


      

        public RelayCommand<string> ActionCommand { get; private set; }

        void ExecuteReadyCommand(string str)
        {
            switch (str)
            {
                case "ReadyAction":
                    {
                        byte id = 0;
                        if (SelectActionIndex == 0)//合闸预制
                        {
                            id = 1;
                        }
                        else if (SelectActionIndex == 1)//分闸预制
                        {
                            id = 3;
                        }
                        var command = new byte[] { id, _circleSelectByte, (byte)_actionTime };
                        //此处发送控制命令
                       

                        break;
                    }
                case "ExecuteAction":
                    {
                        byte id = 0;
                        if (SelectActionIndex == 0)//合闸执行
                        {
                            id = 2;
                        }
                        else if (SelectActionIndex == 1)//分闸执行
                        {
                            id = 4;
                        }
                        var command = new byte[] { id, _circleSelectByte, (byte)_actionTime };
                        break;
                    }
                default:
                    {
                        break;
                    }
            
            }
        }
        private int selectActionIndex = 0;

        public int SelectActionIndex
        {
            get
            {
                return selectActionIndex;
            }
            set
            {
                selectActionIndex = value;
                RaisePropertyChanged("SelectActionIndex");
            }
        }



        private ObservableCollection<string> _actionSelect;

         public ObservableCollection<string> ActionSelect
        {
            get
            {
                return _actionSelect;
            }
        }
        /// <summary>
        /// 三相选择字
        /// </summary>
        private byte _phaseSelectByte = 0;
        public bool CheckPhaseA
        {
            set
            {
                if (value)
                {
                    _phaseSelectByte |= 0x01;
                }
                else
                {
                    _phaseSelectByte &= 0xFE;
                }
                RaisePropertyChanged("CheckPhaseA");
            }
            get
            {
                var state = ((_phaseSelectByte & 0x01) == 0x01) ? (true) : (false);
                return state;
            }
        }

        public bool CheckPhaseB
        {
            set
            {
                if (value)
                {
                    _phaseSelectByte |= 0x02;
                }
                else
                {
                    _phaseSelectByte &= 0xFC;
                }
                RaisePropertyChanged("CheckPhaseB");
            }
            get
            {
                var state = ((_phaseSelectByte & 0x02) == 0x02) ? (true) : (false);
                return state;
            }
        }
      
        public bool CheckPhaseC
        {
            set
            {
                if (value)
                {
                    _phaseSelectByte |= 0x04;
                }
                else
                {
                    _phaseSelectByte &= 0xFB;
                }
                RaisePropertyChanged("CheckPhaseC");
            }
            get
            {
                var state = ((_phaseSelectByte & 0x04) == 0x04) ? (true) : (false);
                return state;
            }
        }

        /// <summary>
        /// 回路选择字
        /// </summary>
        private byte _circleSelectByte = 0;
        public bool CircleI
        {
            set
            {
                if (value)
                {
                    _circleSelectByte |= 0x01;
                }
                else
                {
                    _circleSelectByte &= 0xFE;
                }
                RaisePropertyChanged("CircleI");
            }
            get
            {
                var state = ((_circleSelectByte & 0x01) == 0x01) ? (true) : (false);
                return state;
            }
        }

        public bool CircleII
        {
            set
            {
                if (value)
                {
                    _circleSelectByte |= 0x02;
                }
                else
                {
                    _circleSelectByte &= 0xFC;
                }
                RaisePropertyChanged("CircleII");
            }
            get
            {
                var state = ((_circleSelectByte & 0x02) == 0x02) ? (true) : (false);
                return state;
            }
        }

        public bool CircleIII
        {
            set
            {
                if (value)
                {
                    _circleSelectByte |= 0x04;
                }
                else
                {
                    _circleSelectByte &= 0xFB;
                }
                RaisePropertyChanged("CircleIII");
            }
            get
            {
                var state = ((_circleSelectByte & 0x04) == 0x04) ? (true) : (false);
                return state;
            }
        }
        /// <summary>
        /// 合分闸动作时间
        /// </summary>
        private int _actionTime = 50;
        public string ActionTime
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 10) && (time <= 100))
                    {
                        _actionTime = time;
                    }
                }
                RaisePropertyChanged("ActionTime");
            }
            get
            {
                return _actionTime.ToString();
            }
        }

        /// <summary>
        /// 偏移时间
        /// </summary>
        private int _offsetTime;

        public string OffsetTime
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 10) && (time <= 65535))
                    {
                        _offsetTime = time;
                    }
                }
                RaisePropertyChanged("OffsetTime");
            }
            get
            {
                return _offsetTime.ToString();
            }
        }




        #endregion
    }
}