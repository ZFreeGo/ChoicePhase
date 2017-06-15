using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Threading;
using ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer;
using ZFreeGo.ChoicePhase.Modbus;
using ZFreeGo.ChoicePhase.PlatformModel;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;
using ZFreeGo.Monitor.AutoStudio.Secure;

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
        private  byte _downAddress ;
       

        private PlatformModelServer modelServer;


        private static string redLed =  @"../Pictures/dp1.png";
        private static string greenLed =  @"../Pictures/green.png";
        private static string yellowLed = @"../Pictures/yellow.png";
        private static string offLed = @"../Pictures/off.jpg";

        

        /// <summary>
        /// Initializes a new instance of the ControlViewModel class.
        /// </summary>
        public ControlViewModel()
        {
             _actionSelect = new ObservableCollection<string>();
             _actionSelect.Add("合闸");
             _actionSelect.Add("分闸");

             ActionCommand = new RelayCommand<string>(ExecuteReadyCommand);
           
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

             LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);
             SecureCheckCommand = new RelayCommand<String>(ExecuteSecureCheckCommand);


             Messenger.Default.Register<string>(this, "ControlViewPassword", UpdatePassword);

             ExecuteLoadDataCommand();
              
        }

      

        #region 加载数据命令：LoadDataCommand
        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand LoadDataCommand { get; private set; }

        //加载用户数据
        void ExecuteLoadDataCommand()
        {
            if ( modelServer == null)
            {
                modelServer = PlatformModelServer.GetServer();
                _downAddress = modelServer.CommServer.DownAddress;             
                modelServer.MonitorData.NodeStatusList[1].StatusUpdateEvent +=PhaseA_StatusUpdateEvent;
                modelServer.MonitorData.NodeStatusList[2].StatusUpdateEvent += PhaseB_StatusUpdateEvent;
                modelServer.MonitorData.NodeStatusList[3].StatusUpdateEvent += PhaseC_StatusUpdateEvent;
                modelServer.MonitorData.UserControlEnable.PropertyChanged += UserControlEnable_PropertyChanged;              
            }           
        }

        /// <summary>
        /// 控件使能属性通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserControlEnable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

       

        #endregion

        #region 安全认证


        /// <summary>
        /// 开关操作控制使能
        /// </summary>
        public bool ControlEnable
        {
            get
            {
                return modelServer.MonitorData.UserControlEnable.ControlEnable;
            }
        }

        /// <summary>
        /// 开关操作使能
        /// </summary>
        public bool SwitchOperateEnable
        {
            get
            {
                return modelServer.MonitorData.UserControlEnable.SwitchOperateEnable;
            }
        }


        /// <summary>
        /// 安全密钥,安全字符串
        /// </summary>
        private string _passWord;
        private void UpdatePassword(string obj)
        {
            _passWord = obj;
        }

        private string _secureTip = "未进行认证";
        public string SecureTip
        {
            get
            {
                return _secureTip;
            }
            set
            {
                _secureTip = value;
                RaisePropertyChanged("SecureTip");
            }
        }

        private string _secureColor = "Red";
        /// <summary>
        /// 安全颜色表示
        /// </summary>
        public string SecureColor
        {
           get
            {
                return _secureColor;
            }
            set
            {
                _secureColor = value;
                RaisePropertyChanged("SecureColor");
            }
            
        }


        /// <summary>
        /// 安全检测命令
        /// </summary>
        public RelayCommand<String> SecureCheckCommand { get; private set; }



        private void ExecuteSecureCheckCommand(string obj)
        {

            switch (obj)
            {
                case "Check":
                    {
                        if ("12345" == _passWord)
                        {
                            SecureTip = "认证通过";
                            SecureColor = "Green";
                            modelServer.MonitorData.UserControlEnable.SwitchOperateEnable = true;
                        }
                        else
                        {
                            SecureTip = "认证失败";
                            modelServer.MonitorData.UserControlEnable.SwitchOperateEnable = false;
                        }
                       
                        break;
                    }
                case "Exit":
                    {
                        SecureTip = "退出认证";
                        SecureColor = "Red";
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("", "ControlViewClrPassword");
                        modelServer.MonitorData.UserControlEnable.SwitchOperateEnable = false;
                        break;
                    }
            }

        }






        #endregion




        #region 指示灯操作

        private void PhaseA_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                UpdatePositionStatus(e.Node.StatusLoopCollect, "A");
                UpdateEnergyStatus(e.Node.EnergyStatusLoopCollect, "A");
            }
            else
            {
                OverTimeCycleA();
            }
        }
        private void PhaseB_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                UpdatePositionStatus(e.Node.StatusLoopCollect, "B");
                UpdateEnergyStatus(e.Node.EnergyStatusLoopCollect, "B");
            }
            else
            {
                OverTimeCycleB();
            }
        }
        private void PhaseC_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                UpdatePositionStatus(e.Node.StatusLoopCollect, "C");
                UpdateEnergyStatus(e.Node.EnergyStatusLoopCollect, "C");
            }
            else
            {
                OverTimeCycleC();
            }
        }
       
        void OverTimeCycleA()
        {
            LedCloseA1 = offLed;
            LedOpenA1 = offLed;            
            LedEneryA1 = offLed;
            LedCloseA2 = offLed;
            LedOpenA2 = offLed;
            LedEneryA2 = offLed;

            LedCloseA = offLed;
            LedOpenA = offLed;
            LedErrorA = redLed;
            LedEneryA = offLed;          
        }

        void OverTimeCycleB()
        {
            LedCloseB1 = offLed;
            LedOpenB1 = offLed;
            LedEneryB1 = offLed;
            LedCloseB2 = offLed;
            LedOpenB2 = offLed;
            LedEneryB2 = offLed;

            LedCloseB = offLed;
            LedOpenB = offLed;
            LedErrorB = redLed;
            LedEneryB = offLed;
            
        }
        void OverTimeCycleC()
        {
            LedCloseC1 = offLed;
            LedOpenC1 = offLed;
            LedEneryC1 = offLed;
            LedCloseC2 = offLed;
            LedOpenC2 = offLed;
            LedEneryC2 = offLed;

            LedCloseC = offLed;
            LedOpenC = offLed;
            LedErrorC = redLed;
            LedEneryC = offLed;
            
        }
        void UpdatePositionStatus(StatusLoop[] status, string ph)
        {                  
          
            var statusLoop = status[0];

            //针对双路
            if(status[0] == status[1])
            {
                UpdateWholeLedStatus(status[0], ph);                
            }
            else
            {
                //认为为故障状态
               // LedCloseA = offLed;
                //LedOpenA = offLed;
                //LedErrorA = redLed;
                SetLed("LedClose" + ph, offLed);
                SetLed("LedOpen" + ph, offLed);
                SetLed("LedError" + ph, offLed);

            }
            statusLoop = status[0];
            UpdateLedStatus(status[0], ph + "1");

            statusLoop = status[1];
            UpdateLedStatus(status[1], ph + "2");
                     
        }
        void UpdateEnergyStatus(EnergyStatusLoop[] status, string ph)
        {
           
           if (status[0] == status[1])
           {
               UpdateEnerggLedStatus(status[0], ph);
                        
           }
           else
           {
               SetLed("LedEnery" + ph, offLed);              
           }

           UpdateEnerggLedStatus(status[0], ph + "1");
           UpdateEnerggLedStatus(status[1], ph + "2");  





        }
        /// <summary>
        /// 更新单只 储能LED状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ph"></param>
        void UpdateEnerggLedStatus(EnergyStatusLoop status, string ph)
        {
            string led = "LedEnery" + ph;

            switch (status)
            {
                case EnergyStatusLoop.Less: //欠压
                    {
                        SetLed(led, yellowLed);
                        break;
                    }
                case EnergyStatusLoop.Normal: //正常范围
                    {
                        SetLed(led, greenLed);
                        break;
                    }
                case EnergyStatusLoop.More: //过压或为空
                    {
                        SetLed(led, redLed);
                        break;
                    }
                case EnergyStatusLoop.Null:
                    {
                        SetLed(led, offLed);
                        break;
                    }
            }
                         
        }
        /// <summary>
        /// 针对多个机构，更新整体的合位状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ph"></param>
        void UpdateWholeLedStatus(StatusLoop status, string ph)
        {
            string close = "LedClose" + ph;
            string open = "LedOpen" + ph;
            string error = "LedError" + ph;

            switch (status)
            {
                case StatusLoop.Null:
                case StatusLoop.Error:
                    {
                        SetLed(close, offLed);
                        SetLed(open, offLed);
                        SetLed(error, redLed);
                        break;
                    }
                case StatusLoop.Close:
                    {
                        SetLed(close, redLed);
                        SetLed(open, offLed);
                        SetLed(error, offLed);
                        break;
                    }
                case StatusLoop.Open:
                    {
                        SetLed(close, offLed);
                        SetLed(open, greenLed);
                        SetLed(error, offLed);
                        break;
                    }

            }
        }
        /// <summary>
        /// 更新合分位状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ph"></param>
        void UpdateLedStatus(StatusLoop status, string ph)
        {
            string close = "LedClose" + ph;
            string open = "LedOpen" + ph;
          

            switch (status)
            {
                case StatusLoop.Null:
                case StatusLoop.Error:
                    {
                        SetLed(close, offLed);
                        SetLed(open, offLed);
                        
                        break;
                    }
                case StatusLoop.Close:
                    {
                        SetLed(close, redLed);
                        SetLed(open, offLed);
                       
                        break;
                    }
                case StatusLoop.Open:
                    {
                        SetLed(close, offLed);
                        SetLed(open, greenLed);
                        
                        break;
                    }

            }
        }

        /// <summary>
        /// 设置LED
        /// </summary>
        /// <param name="led">指定等号</param>
        /// <param name="state">led状态</param>
        void SetLed(string led, string state)
        {
            switch(led)
            {
                case "LedCloseA1":
                    {
                        LedCloseA1 = state;
                        break;
                    }
                case "LedCloseA2":
                    {
                        LedCloseA2 = state;
                        break;
                    }
                case "LedOpenA1":
                    {
                        LedOpenA1 = state;
                        break;
                    }
                case "LedOpenA2":
                    {
                        LedOpenA2 = state;
                        break;
                    }
                case "LedCloseA":
                    {
                        LedCloseA= state;
                        break;
                    }
                case "LedOpenA":
                    {
                        LedOpenA = state;
                        break;
                    }
                case "LedErrorA":
                    {
                        LedErrorA = state;
                        break;
                    }
                case "LedEneryA":
                    {
                        LedEneryA = state;
                        break;
                    }
                case "LedEneryA1":
                    {
                        LedEneryA1 = state;
                        break;
                    }
                case "LedEneryA2":
                    {
                        LedEneryA2 = state;
                        break;
                    }
                case "LedCloseB1":
                    {
                        LedCloseB1 = state;
                        break;
                    }
                case "LedCloseB2":
                    {
                        LedCloseB2 = state;
                        break;
                    }
                case "LedOpenB1":
                    {
                        LedOpenB1 = state;
                        break;
                    }
                case "LedOpenB2":
                    {
                        LedOpenB2 = state;
                        break;
                    }
                case "LedCloseB":
                    {
                        LedCloseB = state;
                        break;
                    }
                case "LedOpenB":
                    {
                        LedOpenB = state;
                        break;
                    }
                case "LedErrorB":
                    {
                        LedErrorB = state;
                        break;
                    }
                case "LedEneryB":
                    {
                        LedEneryB = state;
                        break;
                    }
                case "LedEneryB1":
                    {
                        LedEneryB1 = state;
                        break;
                    }
                case "LedEneryB2":
                    {
                        LedEneryB2 = state;
                        break;
                    }
                case "LedCloseC1":
                    {
                        LedCloseC1 = state;
                        break;
                    }
                case "LedCloseC2":
                    {
                        LedCloseC2 = state;
                        break;
                    }
                case "LedOpenC1":
                    {
                        LedOpenC1 = state;
                        break;
                    }
                case "LedOpenC2":
                    {
                        LedOpenC2 = state;
                        break;
                    }
                case "LedCloseC":
                    {
                        LedCloseC = state;
                        break;
                    }
                case "LedOpenC":
                    {
                        LedOpenC = state;
                        break;
                    }
                case "LedErrorC":
                    {
                        LedErrorC = state;
                        break;
                    }
                case "LedEneryC":
                    {
                        LedEneryC = state;
                        break;
                    }
                case "LedEneryC1":
                    {
                        LedEneryC1 = state;
                        break;
                    }
                case "LedEneryC2":
                    {
                        LedEneryC2 = state;
                        break;
                    } 
                default:
                    {
                        throw new Exception("没有指示灯");
                    }

            }
        }


       



        #endregion


        #region 状态指示灯 A相
        
         
        private string ledCloseA1 = offLed;
        /// <summary>
        /// 合闸指示A1
        /// </summary>
        public String LedCloseA1
        {
            get
            {
                return ledCloseA1;
            }
            set
            {
                ledCloseA1 = value;
                RaisePropertyChanged("LedCloseA1");
            }
        }
        private string ledCloseA2 = offLed;
        /// <summary>
        /// 合闸指示A2
        /// </summary>
        public String LedCloseA2
        {
            get
            {
                return ledCloseA2;
            }
            set
            {
                ledCloseA2 = value;
                RaisePropertyChanged("LedCloseA2");
            }
        }


        private string ledOpenA1 = offLed;
        /// <summary>
        /// 分闸指示A1
        /// </summary>
        public String LedOpenA1
        {
            get
            {
                return ledOpenA1;
            }
            set
            {
                ledOpenA1 = value;
                RaisePropertyChanged("LedOpenA1");
            }
        }
        private string ledOpenA2 = offLed;
        /// <summary>
        /// 分闸指示A2
        /// </summary>
        public String LedOpenA2
        {
            get
            {
                return ledOpenA2;
            }
            set
            {
                ledOpenA2 = value;
                RaisePropertyChanged("LedOpenA2");
            }
        }



        private string ledCloseA = offLed;
        /// <summary>
        /// 总合闸指示
        /// </summary>
        public String LedCloseA
        {
            get
            {
                return ledCloseA;
            }
            set
            {
                ledCloseA = value;
                RaisePropertyChanged("LedCloseA");
            }
        }


        private string ledOpenA = offLed;
        /// <summary>
        /// 总分闸指示A1
        /// </summary>
        public String LedOpenA
        {
            get
            {
                return ledOpenA;
            }
            set
            {
                ledOpenA = value;
                RaisePropertyChanged("LedOpenA");
            }
        }

        private string ledErrorA = offLed;
        /// <summary>
        /// 故障指示A
        /// </summary>
        public String LedErrorA 
        {
            get
            {
                return ledErrorA;
            }
            set
            {
                ledErrorA = value;
                RaisePropertyChanged("LedErrorA");
            }
        }
        private string ledEneryA = offLed;
        public String LedEneryA
        {
            get
            {
                return ledEneryA;
            }
            set
            {
                ledEneryA = value;
                RaisePropertyChanged("LedEneryA");
            }
        }
        private string ledEneryA1 = offLed;
        public String LedEneryA1
        {
            get
            {
                return ledEneryA1;
            }
            set
            {
                ledEneryA1 = value;
                RaisePropertyChanged("LedEneryA1");
            }
        }
        private string ledEneryA2 = offLed;
        public String LedEneryA2
        {
            get
            {
                return ledEneryA2;
            }
            set
            {
                ledEneryA2 = value;
                RaisePropertyChanged("LedEneryA2");
            }
        }

        #endregion
        #region 状态指示灯 B相


        private string ledCloseB1 = offLed;
        /// <summary>
        /// 合闸指示B1
        /// </summary>
        public String LedCloseB1
        {
            get
            {
                return ledCloseB1;
            }
            set
            {
                ledCloseB1 = value;
                RaisePropertyChanged("LedCloseB1");
            }
        }
        private string ledCloseB2 = offLed;
        /// <summary>
        /// 合闸指示B2
        /// </summary>
        public String LedCloseB2
        {
            get
            {
                return ledCloseB2;
            }
            set
            {
                ledCloseB2 = value;
                RaisePropertyChanged("LedCloseB2");
            }
        }


        private string ledOpenB1 = offLed;
        /// <summary>
        /// 分闸指示B1
        /// </summary>
        public String LedOpenB1
        {
            get
            {
                return ledOpenB1;
            }
            set
            {
                ledOpenB1 = value;
                RaisePropertyChanged("LedOpenB1");
            }
        }
        private string ledOpenB2 = offLed;
        /// <summary>
        /// 分闸指示B2
        /// </summary>
        public String LedOpenB2
        {
            get
            {
                return ledOpenB2;
            }
            set
            {
                ledOpenB2 = value;
                RaisePropertyChanged("LedOpenB2");
            }
        }



        private string ledCloseB = offLed;
        /// <summary>
        /// 总合闸指示
        /// </summary>
        public String LedCloseB
        {
            get
            {
                return ledCloseB;
            }
            set
            {
                ledCloseB = value;
                RaisePropertyChanged("LedCloseB");
            }
        }


        private string ledOpenB = offLed;
        /// <summary>
        /// 总分闸指示B1
        /// </summary>
        public String LedOpenB
        {
            get
            {
                return ledOpenB;
            }
            set
            {
                ledOpenB = value;
                RaisePropertyChanged("LedOpenB");
            }
        }

        private string ledErrorB = offLed;
        /// <summary>
        /// 故障指示B
        /// </summary>
        public String LedErrorB
        {
            get
            {
                return ledErrorB;
            }
            set
            {
                ledErrorB = value;
                RaisePropertyChanged("LedErrorB");
            }
        }
        private string ledEneryB = offLed;
        public String LedEneryB
        {
            get
            {
                return ledEneryB;
            }
            set
            {
                ledEneryB = value;
                RaisePropertyChanged("LedEneryB");
            }
        }
        private string ledEneryB1 = offLed;
        public String LedEneryB1
        {
            get
            {
                return ledEneryB1;
            }
            set
            {
                ledEneryB1 = value;
                RaisePropertyChanged("LedEneryB1");
            }
        }
        private string ledEneryB2 = offLed;
        public String LedEneryB2
        {
            get
            {
                return ledEneryB2;
            }
            set
            {
                ledEneryB2 = value;
                RaisePropertyChanged("LedEneryB2");
            }
        }

        #endregion

        #region 状态指示灯 C相


        private string ledCloseC1 = offLed;
        /// <summary>
        /// 合闸指示C1
        /// </summary>
        public String LedCloseC1
        {
            get
            {
                return ledCloseC1;
            }
            set
            {
                ledCloseC1 = value;
                RaisePropertyChanged("LedCloseC1");
            }
        }
        private string ledCloseC2 = offLed;
        /// <summary>
        /// 合闸指示C2
        /// </summary>
        public String LedCloseC2
        {
            get
            {
                return ledCloseC2;
            }
            set
            {
                ledCloseC2 = value;
                RaisePropertyChanged("LedCloseC2");
            }
        }


        private string ledOpenC1 = offLed;
        /// <summary>
        /// 分闸指示C1
        /// </summary>
        public String LedOpenC1
        {
            get
            {
                return ledOpenC1;
            }
            set
            {
                ledOpenC1 = value;
                RaisePropertyChanged("LedOpenC1");
            }
        }
        private string ledOpenC2 = offLed;
        /// <summary>
        /// 分闸指示C2
        /// </summary>
        public String LedOpenC2
        {
            get
            {
                return ledOpenC2;
            }
            set
            {
                ledOpenC2 = value;
                RaisePropertyChanged("LedOpenC2");
            }
        }



        private string ledCloseC = offLed;
        /// <summary>
        /// 总合闸指示
        /// </summary>
        public String LedCloseC
        {
            get
            {
                return ledCloseC;
            }
            set
            {
                ledCloseC = value;
                RaisePropertyChanged("LedCloseC");
            }
        }


        private string ledOpenC = offLed;
        /// <summary>
        /// 总分闸指示C1
        /// </summary>
        public String LedOpenC
        {
            get
            {
                return ledOpenC;
            }
            set
            {
                ledOpenC = value;
                RaisePropertyChanged("LedOpenC");
            }
        }

        private string ledErrorC = offLed;
        /// <summary>
        /// 故障指示C
        /// </summary>
        public String LedErrorC
        {
            get
            {
                return ledErrorC;
            }
            set
            {
                ledErrorC = value;
                RaisePropertyChanged("LedErrorC");
            }
        }
        private string ledEneryC = offLed;
        public String LedEneryC
        {
            get
            {
                return ledEneryC;
            }
            set
            {
                ledEneryC = value;
                RaisePropertyChanged("LedEneryC");
            }
        }
        private string ledEneryC1 = offLed;
        public String LedEneryC1
        {
            get
            {
                return ledEneryC1;
            }
            set
            {
                ledEneryC1 = value;
                RaisePropertyChanged("LedEneryC1");
            }
        }
        private string ledEneryC2 = offLed;
        public String LedEneryC2
        {
            get
            {
                return ledEneryC2;
            }
            set
            {
                ledEneryC2 = value;
                RaisePropertyChanged("LedEneryC2");
            }
        }

        #endregion

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
                            modelServer.MonitorData.GetNdoe(_macAddress).LastSendData = command;
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
                            modelServer.MonitorData.GetNdoe(_macAddress).LastSendData = command;
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
                    case "SynActionHeDSP":
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
            modelServer.MonitorData.GetNdoe(0x10).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(0x10, command, 0, command.Length);
            Thread.Sleep(20);


            cb = modelServer.MonitorData.GetNdoe(0x12).SynConfigByte;
            t1 = (byte)(modelServer.MonitorData.GetNdoe(0x12).DelayTime1 & 0x00FF);
            t2 = (byte)(modelServer.MonitorData.GetNdoe(0x12).DelayTime1 >> 8);
            command = new byte[] { (byte)cmd, cb, t1, t2 };            
            modelServer.MonitorData.GetNdoe(0x12).ResetState();//复位状态  
            modelServer.MonitorData.GetNdoe(0x12).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(0x12, command, 0, command.Length);
            Thread.Sleep(20);

            cb = modelServer.MonitorData.GetNdoe(0x14).SynConfigByte;
            t1 = (byte)(modelServer.MonitorData.GetNdoe(0x14).DelayTime1 & 0x00FF);
            t2 = (byte)(modelServer.MonitorData.GetNdoe(0x14).DelayTime1 >> 8);
            command = new byte[] { (byte)cmd, cb, t1, t2 };      
            modelServer.MonitorData.GetNdoe(0x14).ResetState();//复位状态 
            modelServer.MonitorData.GetNdoe(0x14).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(0x14, command, 0, command.Length);
        }
      
        /// <summary>
        /// 将命令发送到A,B,C
        /// </summary>
        /// <param name="command"></param>
        private void SendToABC(byte[] command)
        {
            modelServer.MonitorData.GetNdoe(0x10).ResetState();//复位状态
            modelServer.MonitorData.GetNdoe(0x10).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(0x10, command, 0, command.Length);
            Thread.Sleep(20);
            modelServer.MonitorData.GetNdoe(0x12).ResetState();//复位状态        
            modelServer.MonitorData.GetNdoe(0x12).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(0x12, command, 0, command.Length);
            Thread.Sleep(20);
            modelServer.MonitorData.GetNdoe(0x14).ResetState();//复位状态 
            modelServer.MonitorData.GetNdoe(0x14).LastSendData = command;
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
                modelServer.MonitorData.GetNdoe(_macAddress).LastSendData = command;
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
                 if (p == "SynReadyHeDSP")
                 {
                     p = "Ready";
                 }
                 else if (p == "SynActionHeDSP")
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
                 modelServer.MonitorData.GetNdoe(_macAddress).LastSendData = command.Item1;
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