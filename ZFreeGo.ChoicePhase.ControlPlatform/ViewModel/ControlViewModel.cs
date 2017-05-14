using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer;
using ZFreeGo.ChoicePhase.Modbus;
using ZFreeGo.ChoicePhase.PlatformModel;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;

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
                            CommandIdentify cmd;                            
                            if (SelectActionIndex == 0)//合闸预制
                            {
                                cmd = CommandIdentify.ReadyClose;
                            }
                            else if (SelectActionIndex == 1)//分闸预制
                            {
                                cmd = CommandIdentify.ReadyOpen;
                            }
                            else
                            {
                                return;
                            }
                            var command = new byte[] { (byte)cmd, _circleByte, (byte)_actionTime };

                            //复位状态
                            modelServer.MonitorData.GetNdoe(_macAddress).ResetState();//复位状态
                            //此处发送控制命令                     
                            modelServer.ControlNetServer.MasterSendCommand(_macAddress, command, 0, command.Length);                          


                            break;
                        }
                    case "ExecuteAction":
                        {
                            CommandIdentify cmd;
                            if (SelectActionIndex == 0)//合闸执行
                            {
                                cmd = CommandIdentify.CloseAction;
                            }
                            else if (SelectActionIndex == 1)//分闸执行
                            {
                                cmd = CommandIdentify.OpenAction;
                            }
                            else
                            {
                                return;
                            }
                            //复位状态
                            modelServer.MonitorData.GetNdoe(_macAddress).ResetState();//复位状态
                            var command = new byte[] { (byte)cmd, _circleByte, (byte)_actionTime };
                            //此处发送控制命令                     
                            modelServer.ControlNetServer.MasterSendCommand(_macAddress, command, 0, command.Length);  

                            break;
                        }
                    case "Synchronization":
                        {
                            ExecuteSynReadyCommand("Synchronization");
                            break;
                        }
                    case "SynReadyHeDSP":
                    case "SynHeActionDSP":
                        {
                            ExecuteSynCommand_DSP(str);
                            break;
                        }
                        //以下为总控
                    case "SynSwitchReadyHe":
                        {
                            modelServer.MonitorData.GetNdoe(0x0D).ResetState();//复位状态
                            ExecuteSynCommand_DSP("SynReadyHeDSP"); //首先发送命令到同步控制器，置为同步合闸预制状态
                            Thread.Sleep(20);
                            SendSynCMDToABC(CommandIdentify.SyncReadyClose); //分别发送到三相执行同步合闸预制
                            
                            break;
                        }
                    case "SynSwitchActionHe":
                        {
                            if (modelServer.MonitorData.GetNdoe(0x0D).SynReadyCloseState)
                            {
                                throw new Exception("同步控制器未就绪");
                            }
                            if (modelServer.MonitorData.GetNdoe(0x10).SynReadyCloseState)
                            {
                                throw new Exception("A相未就绪");
                            }
                            if (modelServer.MonitorData.GetNdoe(0x12).SynReadyCloseState)
                            {
                                throw new Exception("B相未就绪");
                            }
                            if (modelServer.MonitorData.GetNdoe(0x14).SynReadyCloseState)
                            {
                                throw new Exception("C相未就绪");
                            }
                            ExecuteSynCommand_DSP("SynHeActionDSP");
                           
                            break;
                        }
                    case "ReadyAllHe":                   
                        {
                            //0x03 -- I,II同时动作
                            var command = new byte[] { (byte)CommandIdentify.ReadyClose, 0x03, (byte)_actionTime };
                            //此处发送控制命令   
                            SendToABC(command);

                            break;
                        }
                    case "ActionAllHe":
                        {
                            //0x03 -- I,II同时动作
                            var command = new byte[] { (byte)CommandIdentify.CloseAction, 0x03, (byte)_actionTime };
                            //此处发送控制命令                     
                            SendToABC(command);
                            break;
                        }
                    case "ReadyAllFen":
                        {
                            //0x03 -- I,II同时动作
                            var command = new byte[] { (byte)CommandIdentify.ReadyOpen, 0x03, (byte)_actionTime };
                            //此处发送控制命令                     
                            SendToABC(command);
                            break;
                        }
                    case "ActionAllFen":
                        {
                            //0x03 -- I,II同时动作
                            var command = new byte[] { (byte)CommandIdentify.OpenAction, 0x03, (byte)_actionTime };
                            //此处发送控制命令                     
                            SendToABC(command);
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
        

       


        /// <summary>
        /// 发送同步命令发送到A,B,C
        /// </summary>
        /// <param name="command"></param>
        private void SendSynCMDToABC(CommandIdentify cmd)
        {
            var cb = modelServer.MonitorData.GetNdoe(0x10).SynConfigByte;
            byte t1 = (byte)(modelServer.MonitorData.GetNdoe(0x10).DelayTime1 & 0x00FF);
            byte t2 = (byte)(modelServer.MonitorData.GetNdoe(0x10).DelayTime1 >> 8);
            var command = new byte[] { (byte)cmd, cb, t1, t2};
            modelServer.MonitorData.GetNdoe(0x10).ResetState();//复位状态
            modelServer.ControlNetServer.MasterSendCommand(0x10, command, 0, command.Length);
            Thread.Sleep(20);


            cb = modelServer.MonitorData.GetNdoe(0x12).SynConfigByte;
            t1 = (byte)(modelServer.MonitorData.GetNdoe(0x12).DelayTime1 & 0x00FF);
            t2 = (byte)(modelServer.MonitorData.GetNdoe(0x12).DelayTime1 >> 8);
            command = new byte[] { (byte)cmd, cb, t1, t2 };            
            modelServer.MonitorData.GetNdoe(0x12).ResetState();//复位状态          
            modelServer.ControlNetServer.MasterSendCommand(0x12, command, 0, command.Length);
            Thread.Sleep(20);

            cb = modelServer.MonitorData.GetNdoe(0x14).SynConfigByte;
            t1 = (byte)(modelServer.MonitorData.GetNdoe(0x14).DelayTime1 & 0x00FF);
            t2 = (byte)(modelServer.MonitorData.GetNdoe(0x14).DelayTime1 >> 8);
            command = new byte[] { (byte)cmd, cb, t1, t2 };      
            modelServer.MonitorData.GetNdoe(0x14).ResetState();//复位状态 
            modelServer.ControlNetServer.MasterSendCommand(0x14, command, 0, command.Length);
        }
      
        /// <summary>
        /// 将命令发送到A,B,C
        /// </summary>
        /// <param name="command"></param>
        private void SendToABC(byte[] command)
        {
            modelServer.MonitorData.GetNdoe(0x10).ResetState();//复位状态
            modelServer.ControlNetServer.MasterSendCommand(0x10, command, 0, command.Length);
            Thread.Sleep(20);
            modelServer.MonitorData.GetNdoe(0x12).ResetState();//复位状态          
            modelServer.ControlNetServer.MasterSendCommand(0x12, command, 0, command.Length);
            Thread.Sleep(20);
            modelServer.MonitorData.GetNdoe(0x14).ResetState();//复位状态 
            modelServer.ControlNetServer.MasterSendCommand(0x14, command, 0, command.Length);
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
 
                var command = new byte[2 + 6];             
                command[0] = (byte)CommandIdentify.SyncReadyClose;
                command[1] = _circleSelectByte;
                Array.Copy(byteArray, 0, command, 2, 2 * i);

                //复位状态
                modelServer.MonitorData.GetNdoe(_macAddress).ResetState();//复位状态
                //此处发送控制命令                     
                modelServer.ControlNetServer.MasterSendCommand(_macAddress, command, 0, 2 + 2*i);  



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



         private int _phaseIndexI = 1;

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
         private int _phaseIndexII = 2;

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
         private int _phaseIndexIII = 3;

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
         /// 角度 I
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
         /// 角度 II
         /// </summary>
         private double _angleII = 0;
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
         /// 角度 III
         /// </summary>
         private double _angleIII = 0;

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

                 var command = new byte[2 + 6];

                 if (p == "Ready")
                 {
                     command[0] = (byte)CommandIdentify.SyncOrchestratorReadyClose;
                 }
                 else if (p == "Action")
                 {
                     command[0] = (byte)CommandIdentify.SyncOrchestratorCloseAction;
                 }
                 else
                 {
                     return null;
                 }


                 command[1] = selectByte;
                 Array.Copy(byteArray, 0, command, 2, 2 * i);

                 return new Tuple<byte[], int>(command, 2 + 2 * i);


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

                 modelServer.MonitorData.GetNdoe(_macAddress).ResetState();//复位状态
                 modelServer.ControlNetServer.MasterSendCommand(0x0D, command.Item1, 0, command.Item2);  


             }
             catch (Exception ex)
             {
                 Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
             }
         }
       
        #endregion
    }
}