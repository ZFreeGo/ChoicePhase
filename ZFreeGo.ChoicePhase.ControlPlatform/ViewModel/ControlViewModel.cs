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
using ZFreeGo.ChoicePhase.PlatformModel.Helper;
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


        /// <summary>
        /// DSP同步合命令
        /// </summary>
        private const string SynActionHeDSP = "SynActionHeDSP";
        private const string SynReadyHeDSP = "SynReadyHeDSP";

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
      

                modelServer.LogicalUI.UserControlEnable.PropertyChanged += UserControlEnable_PropertyChanged;
                ///执行代码委托
                modelServer.LogicalUI.UserControlEnable.ExecuteReadyCommandDelegate = ExecuteUserReadyActionCommand;               
                modelServer.ControlNetServer.PollingService.ErrorAckChanged += PollingService_ErrorAckChanged;

                  ///执行同步命令委托
                modelServer.LogicalUI.SynPhaseChoice.SynCommandDelegate = ExecuteSynCommand;



            }           
        }


        /// <summary>
        /// 指示灯
        /// </summary>
        public IndicatorLight IndicatorLightABC
        {
            get
            {
                return modelServer.LogicalUI.IndicatorLightABC;
            }
        }
     

        /// <summary>
        /// 同步相角选择
        /// </summary>
         public PhaseChoice SynPhaseChoice
        {
             get
            {
                return modelServer.LogicalUI.SynPhaseChoice;
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
                return modelServer.LogicalUI.UserControlEnable.ControlEnable;
            }
        }

        /// <summary>
        /// 开关操作使能
        /// </summary>
        public bool SwitchOperateEnable
        {
            get
            {
                return modelServer.LogicalUI.UserControlEnable.SwitchOperateEnable;
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
        /// 控制集合
        /// </summary>
        public EnableControl ControlCollect
        {
            get
            {
                return modelServer.LogicalUI.UserControlEnable;
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
                            modelServer.LogicalUI.UserControlEnable.SwitchOperateEnable = true;
                        }
                        else
                        {
                            SecureTip = "认证失败";
                            modelServer.LogicalUI.UserControlEnable.SwitchOperateEnable = false;
                        }
                       
                        break;
                    }
                case "Exit":
                    {
                        SecureTip = "退出认证";
                        SecureColor = "Red";
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("", "ControlViewClrPassword");
                        modelServer.LogicalUI.UserControlEnable.SwitchOperateEnable = false;
                        break;
                    }
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
                            modelServer.LogicalUI.GetNdoe(_macAddress).ResetState();//复位状态
                            modelServer.LogicalUI.GetNdoe(_macAddress).LastSendData = command;
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
                            modelServer.LogicalUI.GetNdoe(_macAddress).ResetState();//复位状态
                            var command = new byte[] { (byte)cmd, _circleByte, (byte)_actionTime };
                            modelServer.LogicalUI.GetNdoe(_macAddress).LastSendData = command;
                            //此处发送控制命令                     
                            modelServer.ControlNetServer.MasterSendCommand(_macAddress, command, 0, command.Length);  

                            break;
                        }
                    case "Synchronization":
                        {
                            ExecuteSynReadyCommand("Synchronization");
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
        /// 发送同步命令
        /// </summary>
        /// <param name="str">命令字符</param>
        private void SendSynCommand(string str)
        {
            byte[] command;
            switch (str)
            {
                case SynReadyHeDSP:
                    {
                        command = modelServer.LogicalUI.SynPhaseChoice.GetSynCommand(CommandIdentify.SyncOrchestratorReadyClose);                        
                        break;
                    }
                case SynActionHeDSP:
                    {
                        
                        command = modelServer.LogicalUI.SynPhaseChoice.GetSynCommand(CommandIdentify.SyncOrchestratorCloseAction);
                        break;
                    }
               
                default:
                    {
                        command = null;
                        throw new Exception("同步命令,未识别！");                        
                    }
            }
            if (command != null)
            {                
                SendCMD(NodeAttribute.MacSynControllerMac, command);                
            }
            else
            {
                throw new Exception("同步合闸相角设置错误,未选择任何相角！");
            }

        }

        /// <summary>
        /// 执行同步命令委托
        /// </summary>
        /// <param name="str"></param>
        private void ExecuteSynCommand(string str)
        {
            try
            {
                SendSynCommand(str);

            }
            catch(Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }


        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ShowMessageBox(string message, string caption)
        {
            var resutlt = System.Windows.MessageBox.Show(message, caption, System.Windows.MessageBoxButton.OKCancel);
            if (resutlt == System.Windows.MessageBoxResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        
        
        /// <summary>
        /// 执行面向用户的合分闸命令
        /// </summary>
        /// <param name="str">参数</param>
        void ExecuteUserReadyActionCommand(string str)
        {
            try
            {
                
                switch (str)
                {

                    case "SynCloseReady"://同步合闸预制
                        {
                            if (modelServer.LogicalUI.UserControlEnable.OperateState &&
                               (!modelServer.LogicalUI.UserControlEnable.OperateABC))
                            {
                                ShowMessageBox("有正在处理的其它操作", "预制操作");
                            }

                            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacSynControllerMac).ResetState();//复位状态                           

                            SendSynCommand(SynReadyHeDSP);//首先发送命令到同步控制器，置为同步合闸预制状态
                            Thread.Sleep(200);

                            SendSynCMDToABC(CommandIdentify.SyncReadyClose); //分别发送到三相执行同步合闸预制

                            modelServer.LogicalUI.UserControlEnable.OperateSyn = true;
                            modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionSyn =
                                   new OverTimeTimer(10000, () =>
                                   {
                                       ShowMessageBox("同步合闸操作超时", "单相操作");
                                       modelServer.MonitorData.UpdateStatus("同步合闸操作超时");
                                       modelServer.LogicalUI.UserControlEnable.OperateSyn = false;
                                       modelServer.LogicalUI.UserControlEnable.SynCloseReady = true;
                                       modelServer.LogicalUI.UserControlEnable.SynCloseAction = false;
                                   });
                            modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionSyn.ReStartTimer();
                            
                            break;
                        }
                    case "SynCloseAction"://同步合闸执行
                        {
                            SendSynCommand(SynActionHeDSP);
                            break;
                        }    

                    case "CloseReady":
                        {

                            if (modelServer.LogicalUI.UserControlEnable.OperateState &&
                               (!modelServer.LogicalUI.UserControlEnable.OperateABC))
                            {
                                ShowMessageBox("有正在处理的其它相操作", "整体操作");
                            }


                            if (ShowMessageBox("是否确认 三相合闸预制？", "三相合闸操作"))
                            {
                                var command = new byte[] { (byte)CommandIdentify.ReadyClose, 0x03, (byte)_actionTime };
                                SendCMD(NodeAttribute.MacPhaseA, command);
                                Thread.Sleep(10);
                                SendCMD(NodeAttribute.MacPhaseB, command);
                                Thread.Sleep(10);
                                SendCMD(NodeAttribute.MacPhaseC, command);

                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态 
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).ResetState();//复位状态 
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).ResetState();//复位状态 
                                modelServer.LogicalUI.UserControlEnable.OperateABC = true;
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC =
                                    new OverTimeTimer(10000, () =>
                                    {
                                          ShowMessageBox("三相合闸操作超时", "单相操作");
                                        modelServer.MonitorData.UpdateStatus("三相合闸操作超时");
                                        modelServer.LogicalUI.UserControlEnable.OperateABC = false;
                                        modelServer.LogicalUI.UserControlEnable.CloseReady = true;
                                        modelServer.LogicalUI.UserControlEnable.CloseAction = false;
                                    });
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC.ReStartTimer();
                            }
                            


                            break;
                        }
                    case "CloseAction":
                        {                            
                            var command = new byte[] { (byte)CommandIdentify.CloseAction, 0x03, (byte)_actionTime };
                            SendCMD(NodeAttribute.MacPhaseA, command);
                            Thread.Sleep(10);
                            SendCMD(NodeAttribute.MacPhaseB, command);
                            Thread.Sleep(10);
                            SendCMD(NodeAttribute.MacPhaseC, command);

                            break;
                        }
                    case "CloseReadyA":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseA, CommandIdentify.ReadyClose);
                            break;
                        }
                    case "CloseActionA":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseA, CommandIdentify.CloseAction);
                            break;
                        }
                    case "CloseReadyB":
                        {                          
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseB, CommandIdentify.ReadyClose);
                            break;
                        }
                    case "CloseActionB":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseB, CommandIdentify.CloseAction);
                            break;
                        }
                    case "CloseReadyC":
                        {

                            SinglePhaseReadyAction(NodeAttribute.MacPhaseC, CommandIdentify.ReadyClose);
                            break;
                        }
                    case "CloseActionC":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseC, CommandIdentify.CloseAction);
                            break;
                        }
                    case "OpenReady":
                        {
                            if (modelServer.LogicalUI.UserControlEnable.OperateState &&
                               (!modelServer.LogicalUI.UserControlEnable.OperateABC))
                            {
                                ShowMessageBox("有正在处理的其它相操作", "整体操作");
                            }


                            if (ShowMessageBox("是否确认 三相分闸预制？", "三相分闸操作"))
                            {
                                var command = new byte[] { (byte)CommandIdentify.ReadyOpen, 0x03, (byte)_actionTime };
                                SendCMD(NodeAttribute.MacPhaseA, command);
                                Thread.Sleep(10);
                                SendCMD(NodeAttribute.MacPhaseB, command);
                                Thread.Sleep(10);
                                SendCMD(NodeAttribute.MacPhaseC, command);

                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态 
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).ResetState();//复位状态 
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).ResetState();//复位状态 
                                modelServer.LogicalUI.UserControlEnable.OperateABC = true;
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC =
                                    new OverTimeTimer(10000, () =>
                                    {
                                        ShowMessageBox("三相分闸操作超时", "单相操作");
                                        modelServer.MonitorData.UpdateStatus("三相分闸操作超时");
                                        modelServer.LogicalUI.UserControlEnable.OperateABC = false;
                                        modelServer.LogicalUI.UserControlEnable.OpenReady = true;
                                        modelServer.LogicalUI.UserControlEnable.OpenAction = false;
                                    });
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC.ReStartTimer();
                            }
                            break;
                        }
                    case "OpenAction":
                        {
                            var command = new byte[] { (byte)CommandIdentify.OpenAction, 0x03, (byte)_actionTime };
                            SendCMD(NodeAttribute.MacPhaseA, command);
                            Thread.Sleep(10);
                            SendCMD(NodeAttribute.MacPhaseB, command);
                            Thread.Sleep(10);
                            SendCMD(NodeAttribute.MacPhaseC, command);
                            break;
                        }
                    case "OpenReadyA":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseA, CommandIdentify.ReadyOpen);
                            break;
                        }
                    case "OpenActionA":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseA, CommandIdentify.OpenAction);
                            break;
                        }
                    case "OpenReadyB":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseB, CommandIdentify.ReadyOpen);
                            break;
                        }
                    case "OpenActionB":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseB, CommandIdentify.OpenAction);
                            break;
                            
                        }
                    case "OpenReadyC":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseC, CommandIdentify.ReadyOpen);
                            break;
                        }
                    case "OpenActionC":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseC, CommandIdentify.OpenAction);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                //TODO:错误处理
            }
        }

        /// <summary>
        /// 单相合闸预制
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="cmd"></param>
        private void SinglePhaseReadyAction(byte mac, CommandIdentify cmd)
        {

            Action<bool> actDelegate;
            string des = "";
            string cmdDes =  modelServer.GetIDDescription(cmd);
            bool opetate = false;

            if (mac == NodeAttribute.MacPhaseA)
            {
                actDelegate = ar => { modelServer.LogicalUI.UserControlEnable.OperateA = ar; };
                opetate = modelServer.LogicalUI.UserControlEnable.OperateA;
                des = "A";
            }
            else if (mac == NodeAttribute.MacPhaseB)
            {
                actDelegate = ar => { modelServer.LogicalUI.UserControlEnable.OperateB = ar; };
                opetate = modelServer.LogicalUI.UserControlEnable.OperateB;
                des = "B";
            }
            else if (mac == NodeAttribute.MacPhaseC)
            {
                actDelegate = ar => { modelServer.LogicalUI.UserControlEnable.OperateC = ar; };
                opetate = modelServer.LogicalUI.UserControlEnable.OperateC;
                des = "C";
            }
            else
            {
                return;
            }
            


            if (modelServer.LogicalUI.UserControlEnable.OperateState &&
                               (!opetate))
            {
                ShowMessageBox("有正在处理的其它相操作", "单相操作");
            }


            if (ShowMessageBox(string.Format("是否确认 {0}相{1}？", des, cmdDes), "单相操作"))
            {
                var command = new byte[] { (byte)cmd, 0x03, (byte)_actionTime };
                SendCMD(mac, command);

                if ((cmd == CommandIdentify.SyncReadyClose) ||
                    (cmd == CommandIdentify.ReadyClose) ||
                    (cmd == CommandIdentify.ReadyOpen))
                {
                    modelServer.LogicalUI.GetNdoe(mac).ResetState();//复位状态 
                    actDelegate(true);

                    var timer = new OverTimeTimer(10000, () =>
                                {
                                    actDelegate(false);
                                    ShowMessageBox(string.Format("{0}相操作超时！", des), "单相操作");
                                    modelServer.MonitorData.UpdateStatus(string.Format("{0}相操作超时！", des));
                                });

                    if (mac == NodeAttribute.MacPhaseA)
                    {
                        modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionA = timer;

                    }
                    else if (mac == NodeAttribute.MacPhaseB)
                    {
                        modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionB = timer;

                    }
                    else if (mac == NodeAttribute.MacPhaseC)
                    {
                        modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionC = timer;

                    }
                    else
                    {
                        return;
                    }
                    timer.ReStartTimer();
                }
            }
        }
        /// <summary>
        /// 错误状态主动应答
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_ErrorAckChanged(object sender, StatusChangeMessage e)
        {
            try
            {
                var node = modelServer.LogicalUI.GetNdoe(e.MAC);
                if (node == null)
                {
                    return;
                }
               

                switch ((CommandIdentify)e.Data[1])
                {
                    case CommandIdentify.ReadyClose:
                    case CommandIdentify.CloseAction:
                    case CommandIdentify.ReadyOpen:
                    case CommandIdentify.OpenAction:
                    case CommandIdentify.SyncReadyClose:
                        {
                            OverTimeTimer timer;

                            if (e.MAC == NodeAttribute.MacSynControllerMac)
                            {
                                timer = modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionSyn;

                            }
                            else if (e.MAC == NodeAttribute.MacPhaseA)
                            {
                                timer = modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionA;

                            }
                            else if (e.MAC == NodeAttribute.MacPhaseB)
                            {
                                timer = modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionB;
                            }
                            else if (e.MAC == NodeAttribute.MacPhaseC)
                            {
                                timer = modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionC;
                                break;
                            }
                            else
                            {
                                timer = null;
                                break;
                            }
                            if (timer != null)
                            {
                                timer.StopTimer();
                            }
                            if (modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC != null)
                            {
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC.StopTimer();
                            }
                            if (modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionSyn != null)
                            {
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionSyn.StopTimer();
                            }


                            modelServer.LogicalUI.UserControlEnable.OperateA = false;
                            modelServer.LogicalUI.UserControlEnable.OperateB = false;
                            modelServer.LogicalUI.UserControlEnable.OperateC = false;
                            modelServer.LogicalUI.UserControlEnable.OperateABC = false;
                            modelServer.LogicalUI.UserControlEnable.OperateSyn = false;

                            var serverData = e.Data;
                            var des = modelServer.GetIDDescription((CommandIdentify)serverData[1]);
                            string error1 = "错误代码:" + serverData[2].ToString("X2");
                            string error2 = "附加错误代码:" + serverData[3].ToString("X2");

                            ShowMessageBox(e.MAC.ToString("x2") + des + " " + error1 + " " + error2, "应答错误");


                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }
        /// <summary>
        /// 根据使能情况，发送同步命令发送到A,B,C
        /// </summary>
        /// <param name="command"></param>
        private void SendSynCMDToABC(CommandIdentify cmd)
        {
            if (modelServer.LogicalUI.SynPhaseChoice.GetPhaseEnable(1))
            {
                var cb = modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).SynConfigByte;
                var t1 = (byte)(modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).DelayTime1 & 0x00FF);
                var t2 = (byte)(modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).DelayTime1 >> 8);
                var command = new byte[] { (byte)cmd, cb, t1, t2 };
                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态
                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).LastSendData = command;
                modelServer.ControlNetServer.MasterSendCommand(NodeAttribute.MacPhaseA, command, 0, command.Length);
                Thread.Sleep(20);
            }
            if (modelServer.LogicalUI.SynPhaseChoice.GetPhaseEnable(2))
            {
                var cb = modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).SynConfigByte;
                var t1 = (byte)(modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).DelayTime1 & 0x00FF);
                var t2 = (byte)(modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).DelayTime1 >> 8);
                var command = new byte[] { (byte)cmd, cb, t1, t2 };
                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).ResetState();//复位状态  
                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).LastSendData = command;
                modelServer.ControlNetServer.MasterSendCommand(NodeAttribute.MacPhaseB, command, 0, command.Length);
                Thread.Sleep(20);
            }
            if (modelServer.LogicalUI.SynPhaseChoice.GetPhaseEnable(3))
            {
                var cb = modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).SynConfigByte;
                var t1 = (byte)(modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).DelayTime1 & 0x00FF);
                var t2 = (byte)(modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).DelayTime1 >> 8);
                var command = new byte[] { (byte)cmd, cb, t1, t2 };
                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).ResetState();//复位状态 
                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).LastSendData = command;
                modelServer.ControlNetServer.MasterSendCommand(NodeAttribute.MacPhaseC, command, 0, command.Length);
            }
        }
      
        /// <summary>
        /// 将命令发送到A,B,C
        /// </summary>
        /// <param name="command"></param>
        private void SendToABC(byte[] command)
        {
            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态
            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(NodeAttribute.MacPhaseA, command, 0, command.Length);
            Thread.Sleep(20);
            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).ResetState();//复位状态        
            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(NodeAttribute.MacPhaseB, command, 0, command.Length);
            Thread.Sleep(20);
            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).ResetState();//复位状态 
            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(NodeAttribute.MacPhaseC, command, 0, command.Length);
        }
        /// <summary>
        /// 发送命令状态到子站
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="command"></param>
        private void SendCMD(byte mac, byte[] command)
        {          
            modelServer.LogicalUI.GetNdoe(mac).ResetState();//复位状态 
            modelServer.LogicalUI.GetNdoe(mac).LastSendData = command;
            modelServer.ControlNetServer.MasterSendCommand(mac, command, 0, command.Length);
        }


        #endregion

        #region 永磁控制器控制
        private byte _macAddress = NodeAttribute.MacPhaseA;

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
                modelServer.LogicalUI.GetNdoe(_macAddress).ResetState();//复位状态
                modelServer.LogicalUI.GetNdoe(_macAddress).LastSendData = command;
                //此处发送控制命令                     
                modelServer.ControlNetServer.MasterSendCommand(_macAddress, command, 0, 2 + 2*i);  



            }
            catch(Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }

        #endregion

      
        
    }
}