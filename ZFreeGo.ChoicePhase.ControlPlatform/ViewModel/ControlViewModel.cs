using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using ZFreeGo.ChoicePhase.Modbus;
using ZFreeGo.ChoicePhase.PlatformModel;

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
        private readonly byte _downAddress ;
        private readonly byte _triansFunction = 1;

        private PlatformModelServer modelServer;
        /// <summary>
        /// Initializes a new instance of the ControlViewModel class.
        /// </summary>
        public ControlViewModel()
        {
             _actionSelect = new ObservableCollection<string>();
             _actionSelect.Add("合闸");
             _actionSelect.Add("分闸");

             ActionCommand = new RelayCommand<string>(ExecuteReadyCommand);
             ActionCommandsynchronization = new RelayCommand<string>(ExecuteSynReadyCommand);
             modelServer = PlatformModelServer.GetServer();
             _downAddress = modelServer.CommServer.DownAddress;

             _loopSelect = new ObservableCollection<string>();
             _loopSelect.Add("未选择");
             _loopSelect.Add("回路I");
             _loopSelect.Add("回路II");
             _loopSelect.Add("回路III");

             _phaseSelect = new ObservableCollection<string>();
             _phaseSelect.Add("未选择");
             _phaseSelect.Add("A相");
             _phaseSelect.Add("B相");
             _phaseSelect.Add("C相");
        }

    

        #region 合分闸控制，同步预制


      

        public RelayCommand<string> ActionCommand { get; private set; }

        void ExecuteReadyCommand(string str)
        {
            try
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
                            var command = new byte[] {_macAddress, 0,  id, _circleByte, (byte)_actionTime };
                            //此处发送控制命令
                            var frame = new RTUFrame(_downAddress, _triansFunction, command, (byte)command.Length);
                            modelServer.RtuServer.SendFrame(frame);

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
                            var command = new byte[] { _macAddress, 0, id, _circleByte, (byte)_actionTime };
                            //此处发送控制命令
                            var frame = new RTUFrame(_downAddress, _triansFunction, command, (byte)command.Length);

                            modelServer.RtuServer.SendFrame(frame);
                            break;
                        }
                    case "Synchronization":
                        {
                            ExecuteSynReadyCommand("Synchronization");
                            break;
                        }

                    case "SynHeDSP":
                    case "SynHeActionDSP":
                        {
                            ExecuteSynCommand_DSP(str);
                            break;
                        }
                    case "SynHeARM":
                    case "SynHeActionARM":
                        {
                            ExecuteSynCommand_ARM(str);
                            break;
                        }
                    default:
                        {
                            break;
                        }

                }
            }
            catch(Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }

     

        #endregion

        #region 永磁控制器控制
        private byte _macAddress = 0x10;

        public string MacAddress
        {
            get
            {
                return _macAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out  _macAddress);              
                RaisePropertyChanged("MacAddress");
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
        /// 合闸分闸控制选择的回路
        /// </summary>
        private byte _circleByte = 0;
        public bool CircleI
        {
            set
            {
                if (value)
                {
                    _circleByte |= 0x01;
                }
                else
                {
                    _circleByte &= 0xFE;
                }
                RaisePropertyChanged("CircleI");
            }
            get
            {
                var state = ((_circleByte & 0x01) == 0x01) ? (true) : (false);
                return state;
            }
        }

        public bool CircleII
        {
            set
            {
                if (value)
                {
                    _circleByte |= 0x02;
                }
                else
                {
                    _circleByte &= 0xFC;
                }
                RaisePropertyChanged("CircleII");
            }
            get
            {
                var state = ((_circleByte & 0x02) == 0x02) ? (true) : (false);
                return state;
            }
        }

        public bool CircleIII
        {
            set
            {
                if (value)
                {
                    _circleByte |= 0x04;
                }
                else
                {
                    _circleByte &= 0xFB;
                }
                RaisePropertyChanged("CircleIII");
            }
            get
            {
                var state = ((_circleByte & 0x04) == 0x04) ? (true) : (false);
                return state;
            }
        }
        /// <summary>
        /// 回路选择字
        /// </summary>
        private byte _circleSelectByte = 0;

        private ObservableCollection<string> _loopSelect;

        public ObservableCollection<string> LoopSelect
        {
            get
            {
                return _loopSelect;
            }
        }



        private int _loopIndexI = 0;

        public int LoopIndexI
        {
            get
            {
                return _loopIndexI;
            }
            set
            {
                _loopIndexI = value;
                RaisePropertyChanged("LoopIndexI");
            }
        }
        private int _loopIndexII = 0;

        public int LoopIndexII
        {
            get
            {
                return _loopIndexII;
            }
            set
            {
                _loopIndexII = value;
                RaisePropertyChanged("LoopIndexII");
            }
        }
        private int _loopIndexIII = 0;

        public int LoopIndexIII
        {
            get
            {
                return _loopIndexIII;
            }
            set
            {
                _loopIndexIII = value;
                RaisePropertyChanged("LoopIndexIII");
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

        #endregion

        #region 偏移时间
        /// <summary>
        /// 偏移时间 I
        /// </summary>
        private int _offsetTimeI = 0;

        public string OffsetTimeI
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 0) && (time <= 65535))
                    {
                        _offsetTimeI = time;
                    }
                }
                RaisePropertyChanged("OffsetTimeI");
            }
            get
            {
                return _offsetTimeI.ToString();
            }
        }
        /// <summary>
        /// 偏移时间 II
        /// </summary>
        private int _offsetTimeII = 0;
        public string OffsetTimeII
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 0) && (time <= 65535))
                    {
                        _offsetTimeII = time;
                    }
                }
                RaisePropertyChanged("OffsetTimeII");
            }
            get
            {
                return _offsetTimeII.ToString();
            }
        }
        /// <summary>
        /// 偏移时间 III
        /// </summary>
        private int _offsetTimeIII = 0;
       
        public string OffsetTimeIII
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 0) && (time <= 65535))
                    {
                        _offsetTimeIII = time;
                    }
                }
                RaisePropertyChanged("OffsetTimeIII");
            }
            get
            {
                return _offsetTimeIII.ToString();
            }
        }



        /// <summary>
        /// 同步合闸预制命令
        /// </summary>
        public RelayCommand<string> ActionCommandsynchronization;


         void ExecuteSynReadyCommand(string obj)
        {
            try
            {

                var data = new int[3];
                int i = 0;
                _circleSelectByte = (byte)(LoopIndexI-1);
                if (LoopIndexI == 0)
                {
                    throw new ArgumentNullException("你没有选择任何回路!");
                }

               
                if (LoopIndexII != 0)
                {
                    data[i++] = _offsetTimeI;
                    _circleSelectByte = (byte)(_circleSelectByte | ((LoopIndexII - 1) << 2));

                    if (LoopIndexIII != 0)
                    {
                        data[i++] = _offsetTimeII;
                        _circleSelectByte = (byte)(_circleSelectByte | ((LoopIndexIII - 1) << 4));
                    }                   
                }
                var byteArray = new byte[i  * 2];
                for(int k = 0; k < i; k++)
                {
                    byteArray[2 * k] = (byte)(data[k] & 0x00FF);
                    byteArray[2 * k + 1] = (byte)(data[k] >> 8);
                }              
 
                var command = new byte[2 + 2 + 6];
                command[0] = _macAddress;
                command[1] = 0;
                command[2] = 5;
                command[3] = _circleSelectByte;
                Array.Copy(byteArray, 0, command, 4, 2 * i);
                //此处发送控制命令
                var frame = new RTUFrame(_downAddress, _triansFunction, command, (byte)(4 + 2 * i));

                modelServer.RtuServer.SendFrame(frame);


            }
            catch(Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }

        #endregion

        #region DSP控制
         /// <summary>
         /// 三相选择字
         /// </summary>       

         private ObservableCollection<string> _phaseSelect;

         public ObservableCollection<string> PhaseSelect
         {
             get
             {
                 return _phaseSelect;
             }
         }



         private int _phaseIndexI = 0;

         public int PhaseIndexI
         {
             get
             {
                 return _phaseIndexI;
             }
             set
             {
                 _phaseIndexI = value;
                 RaisePropertyChanged("PhaseIndexI");
             }
         }
         private int _phaseIndexII = 0;

         public int PhaseIndexII
         {
             get
             {
                 return _phaseIndexII;
             }
             set
             {
                 _phaseIndexII = value;
                 RaisePropertyChanged("PhaseIndexII");
             }
         }
         private int _phaseIndexIII = 0;

         public int PhaseIndexIII
         {
             get
             {
                 return _phaseIndexIII;
             }
             set
             {
                 _phaseIndexIII = value;
                 RaisePropertyChanged("PhaseIndexIII");
             }
         }

        /// <summary>
        /// 将字符串转化为，浮点数分辨率为0.5
         /// "1.23"转化为1，"1.43"转化为1.5,"1.83"转化为2.0
        /// </summary>
        /// <param name="str">字符串</param>
      
         void CalAngle(string str, out double angle)
         {

             if (double.TryParse(str, out angle))
             {
                 angle = (angle+360) % 360;
                 double remain = angle - (UInt32)angle;
                 if (remain <= 0.3)
                 {
                      angle=  (double)((UInt32)angle);
                 }
                 else if (remain < 0.7)
                 {
                      angle = (double)((UInt32)angle) + 0.5;
                 }
                 else
                 {
                      angle = (double)((UInt32)angle) + 1;
                 }
             }             
         }


         /// <summary>
         /// 弧度 I
         /// </summary>
         private double _angleI = 0;

         public string AngleI
         {
             set
             {
                 CalAngle(value, out _angleI);
                 RaisePropertyChanged("AngleI");
             }
             get
             {
                 return _angleI.ToString("f1");
             }
         }
         /// <summary>
         /// 弧度 II
         /// </summary>
         private double _angleII = 120;
         public string AngleII
         {
             set
             {
                 CalAngle(value, out _angleII);
                 RaisePropertyChanged("AngleII");
             }
             get
             {
                 return _angleII.ToString("f1");
             }
         }
         /// <summary>
         /// 弧度 III
         /// </summary>
         private double _angleIII = 240;

         public string AngleIII
         {
             set
             {
                 CalAngle(value, out _angleIII);
                 RaisePropertyChanged("AngleIII");
             }
             get
             {
                 return _angleIII.ToString("f1");
             }
         }
        /// <summary>
        /// 获取同步命令字
        /// </summary>
        /// <param name="p">命令参数</param>
        /// <returns>带有长度的命令字节</returns>
         private Tuple<byte[], int> GetSynCommand(string p)
         {
             try
             {

                 var data = new double[3];
                 int i = 0;
                 byte selectByte = (byte)(PhaseIndexI);
                 if (PhaseIndexI == 0)
                 {
                     throw new ArgumentNullException("你没有选择任何相!");
                 }

                 data[i++] = _angleI;
                 if (PhaseIndexII != 0)
                 {
                     selectByte = (byte)(selectByte | ((PhaseIndexII) << 2));
                     data[i++] = _angleII;

                     if (PhaseIndexIII != 0)
                     {
                         data[i++] = _angleIII;
                         selectByte = (byte)(selectByte | ((PhaseIndexIII) << 4));
                     }
                 }
                 var byteArray = new byte[i * 2];
                 for (int k = 0; k < i; k++)
                 {
                     var angle = data[k];
                     UInt16 tris = (ushort)(angle / 360 * 65536);//转化为以65536为为基准的归一化值

                     byteArray[2 * k] = (byte)(tris & 0x00FF);
                     byteArray[2 * k + 1] = (byte)(tris >> 8);
                 }

                 var command = new byte[2 + 2 + 6];
                 command[0] = 0x0D; //DSP地址
                 command[1] = 0;
                 if (p == "Ready")
                 {
                     command[2] = 0x30;
                 }
                 else if (p == "Action")
                 {
                     command[2] = 0x31;
                 }
                 else
                 {
                     return null;
                 }


                 command[3] = selectByte;
                 Array.Copy(byteArray, 0, command, 4, 2 * i);

                 return new Tuple<byte[], int>(command, 4 + 2 * i);


             }
             catch (Exception ex)
             {
                 Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
                 return null;
             }
         }
         private void ExecuteSynCommand_DSP(string p)
         {
             try
             {
                 if(p == "SynHeDSP")
                 {
                     p = "Ready";
                 }
                 else if (p == "SynHeActionDSP")
                 {
                     p = "Action";
                 }
                 var command = GetSynCommand(p);
                
                 if (command == null)
                 {
                     return;
                 }
                 //此处发送控制命令
                 var frame = new RTUFrame(_downAddress, _triansFunction, command.Item1, (byte)command.Item2);

                 modelServer.RtuServer.SendFrame(frame);


             }
             catch (Exception ex)
             {
                 Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
             }
         }
         private void ExecuteSynCommand_ARM(string p)
         {
             try
             {
                 byte commandByte = 0;
                 if (p == "SynHeARM")
                 {
                     p = "Ready";
                     commandByte = 0x20;
                 }
                 else if (p == "SynHeActionARM")
                 {
                     p = "Action";
                     commandByte = 0x21;
                 }
                 var command = GetSynCommand(p);

                 if (command == null)
                 {
                     return;
                 }
                 var commandData = new byte[3 + command.Item2];
                 commandData[0] = 0x02; //ARM地址
                 commandData[1] = 0;
                 commandData[2] = commandByte; //命令字节
                 Array.Copy(command.Item1, 0, commandData, 3, command.Item2);



                 //此处发送控制命令
                 var frame = new RTUFrame(_downAddress, _triansFunction, commandData, (byte)commandData.Length);

                 modelServer.RtuServer.SendFrame(frame);


             }
             catch (Exception ex)
             {
                 Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
             }
         }
        #endregion
    }
}