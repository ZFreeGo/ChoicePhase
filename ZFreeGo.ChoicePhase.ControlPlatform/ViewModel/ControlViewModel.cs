using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer;
using ZFreeGo.ChoicePhase.Modbus;
using ZFreeGo.ChoicePhase.PlatformModel;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;
using ZFreeGo.ChoicePhase.PlatformModel.Helper;
using ZFreeGo.Monitor.AutoStudio.Secure;
using System.Timers;
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



        private readonly TaskScheduler syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        Dispatcher DispatcherShow; 
        /// <summary>
        /// Initializes a new instance of the ControlViewModel class.
        /// </summary>
        public ControlViewModel()
        {         

             LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);
             SecureCheckCommand = new RelayCommand<String>(ExecuteSecureCheckCommand);

             _actionLoopChoice = new LoopChoice();


             Messenger.Default.Register<string>(this, "ControlViewPassword", UpdatePassword);

             ExecuteLoadDataCommand();
             DispatcherShow = Dispatcher.CurrentDispatcher;

             
             LoopTestCommand = new RelayCommand<string>(ExecuteLoopTestCommand);

             
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
                if (NodeAttribute.SingleThreeMode)
                {
                    modelServer.LogicalUI.UserControlEnable.ExecuteReadyCommandDelegate = ExecuteUserReadyActionCommandSingleThree;
                }
                else
                {
                    modelServer.LogicalUI.UserControlEnable.ExecuteReadyCommandDelegate = ExecuteUserReadyActionCommand;
                }
                

                modelServer.ControlNetServer.PollingService.ErrorAckChanged += PollingService_ErrorAckChanged;

                  ///执行同步命令委托
                modelServer.LogicalUI.SynPhaseChoice.SynCommandDelegate = ExecuteSynCommand;


                _actionLoopChoice.ExecuteOpearateCommandDelegate = ExecuteOperateCommand;

                ControlCollect.PropertyChanged += ControlCollect_PropertyChanged;

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
        #region 测试
        

        public LoopTest TestParameter
        {
            get
            {
                return modelServer.LogicalUI.TestParameter;
            }
            set
            {
                modelServer.LogicalUI.TestParameter = value;
                RaisePropertyChanged("TestParameter");
            }
        }
        private bool _enableStart = true;

        public bool EnableStart
        {
            get
            {
                return _enableStart;
            }
            set
            {
                _enableStart = value;
                RaisePropertyChanged("EnableStart");
            }
        }

        private bool _enableStop = false;

        public bool EnableStop
        {
            get
            {
                return _enableStop;
            }
            set
            {
                _enableStop = value;
                RaisePropertyChanged("EnableStop");
            }
        }
        
        /// <summary>
        /// 循环测试标志
        /// </summary>
        private bool _loopTestFlag = false;
        private bool _loopActionFlag = true;//false-进行分组 true--进行合闸


        void CloseLoopTime()
        {
            if (LoopTime != null)
            {
                LoopTime.Enabled = false;
                LoopTime.Elapsed -= LoopTestAction_Elapsed;
                LoopTime.Stop();
                LoopTime.Close();
                LoopTime.Dispose();
            }
        }
        void ReStartTimer(int time)
        {
            LoopTime = new System.Timers.Timer(time);
            LoopTime.Elapsed += LoopTestAction_Elapsed;
            LoopTime.AutoReset = false;
            LoopTime.Start();
            EnableStart = false;
            EnableStop = true;
        }



        
        /// <summary>
        /// 控制合集使能按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ControlCollect_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_loopTestFlag && ExecuteFinished)
            {
                switch (e.PropertyName)
                {
                    case "CloseAction"://合闸执行使能
                        {
                            if (ControlCollect.CloseAction)
                            {
                                ExecuteUserReadyActionCommand("CloseAction");
                                ExecuteFinished = false;
                                TestParameter.UpadeTip("执行合闸。");
                            }
                            break;
                        }
                    case "OpenAction":
                        {
                            if (ControlCollect.OpenAction)
                            {
                                ExecuteUserReadyActionCommand("OpenAction");
                                TestParameter.CurrentCount++;
                                ExecuteFinished = false;
                                TestParameter.UpadeTip("执行分闸。");
                            }

                            break;
                        }
                  
                }
            }
        }
        /// <summary>
        /// 安全检测命令
        /// </summary>
        public RelayCommand<String> LoopTestCommand { get; private set; }


        private void ExecuteLoopTestCommand(string obj)
        {
            try
            {
                switch (obj)
                {
                    case "Start":
                        {
                           // OverTimeTimer
                           // TimerCallback time = ;
                            if (ControlCollect.CloseReady)
                            {                               
                                _loopTestFlag = true;
                                _loopActionFlag = true;

                                ReStartTimer(TestParameter.CoMs);
                                TestParameter.CurrentCount = 0;
                                TestParameter.UpadeTip ("启动循环");
                                ExecuteFinished = false;
                                EnableStart = false;
                                EnableStop = true;
                            }
                            else
                            {
                                TestParameter.UpadeTip("总合闸预制未使能");

                            }
                            break;
                        }
                    case "Stop":
                        {
                            TestParameter.UpadeTip("停止循环");
                            _loopTestFlag = false;
                            CloseLoopTime();
                            EnableStart = true;
                            EnableStop = false;
                            ExecuteFinished = false;
                            break;
                        }
                }
            }
                    catch(Exception ex)
            {
                         Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
                    }
        }

        private bool ExecuteFinished = false;
        private void LoopTestAction_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_loopTestFlag)//是否处于测试模式
            {
                if (TestParameter.CurrentCount > TestParameter.SetCount)
                {
                    _loopTestFlag = false;
                    CloseLoopTime();
                    EnableStart = true;
                    EnableStop = false;
                    TestParameter.UpadeTip("超过规定测试，停止测试");
                    return;

                }

                //合闸
                if(_loopActionFlag )
                {
                    if (ControlCollect.CloseReady)
                    {
                       
                        ExecuteUserReadyActionCommand("CloseReady");
                        ExecuteFinished = true;
                        _loopActionFlag = false;
                        ReStartTimer(TestParameter.OcMs);
                        TestParameter.UpadeTip(
                            string.Format("当前进行第{0}次合闸，共{1}次。", TestParameter.CurrentCount, TestParameter.SetCount));
                    }
                    else
                    {                      
                        _loopTestFlag = false;
                        CloseLoopTime();
                        EnableStart = true;
                        EnableStop = false;
                        ExecuteFinished = false;
                        TestParameter.UpadeTip("总合闸预制未使能");

                    }
                }
                else
                {
                    if (ControlCollect.OpenReady)
                    {                         
                        ExecuteUserReadyActionCommand("OpenReady");
                        _loopActionFlag = true;
                        ExecuteFinished = true;
                        ReStartTimer(TestParameter.CoMs);
                        TestParameter.UpadeTip(
                           string.Format("当前进行第{0}次分闸，共{1}次。", TestParameter.CurrentCount, TestParameter.SetCount));
                    }
                    else
                    {
                        TestParameter.UpadeTip("总分闸预制未使能");                      
                        _loopTestFlag = false;
                        CloseLoopTime();
                        EnableStart = true;
                        EnableStop = false;
                        ExecuteFinished = false;
                    }
                }
            }
            else
            {
                CloseLoopTime();
                TestParameter.UpadeTip("当前不是测试模式。");
            }

        }




        /// <summary>
        /// 循环定时器
        /// </summary>
        System.Timers.Timer LoopTime;

        #endregion


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
                        if (NodeAttribute.PasswordII == _passWord)
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

        private LoopChoice _actionLoopChoice;

        public LoopChoice ActionLoopChoice
        {
            get
            {
                return _actionLoopChoice;
            }
            set
            {
                _actionLoopChoice = value;
                RaisePropertyChanged("ActionLoopChoice");
            }
        }


        /// <summary>
        /// 执行每相合分闸/同步合闸操作
        /// </summary>
        /// <param name="str"></param>
      
        void ExecuteOperateCommand(string str)
        {
            try
            {
                byte[] command;
                switch (str)
                {
                    case "ReadyAction":
                        {
                            command = _actionLoopChoice.GetReadyCommand();                                                      
                            break;
                        }
                    case "ExecuteAction":
                        {
                            command = _actionLoopChoice.GetExecuteCommand();                 

                            break;
                        }
                    case "Synchronization":
                        {
                            command = _actionLoopChoice.GetSynReadyCommand();
                            break;
                        }                  
                    default:
                        {
                            throw new Exception("ExecuteOperateCommand:未识别的操作命令");                            
                        }

                }
                if (command != null)
                {
                    SendCMD(_actionLoopChoice.MacID, command);
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
                SendCMD(NodeAttribute.MacSynController, command);                
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
        void ExecuteUserReadyActionCommandSingleThree(string str)
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

                            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacSynController).ResetState();//复位状态                           

                            SendSynCommand(SynReadyHeDSP);//首先发送命令到同步控制器，置为同步合闸预制状态
                            Thread.Sleep(200);

                           // SendSynCMDToABC(CommandIdentify.SyncReadyClose); //分别发送到三相执行同步合闸预制

                            modelServer.LogicalUI.UserControlEnable.OperateSyn = true;
                            modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionSyn =
                                   new OverTimeTimer(NodeAttribute.SynCloseActionOverTime, () =>
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

                            //执行测试时，不提示确认
                            if (_loopTestFlag || ShowMessageBox("是否确认 三相合闸预制？", "三相合闸操作"))
                            {
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态 
                                                               
                                var command = new byte[] { (byte)CommandIdentify.ReadyClose, 
                                    (byte)(NodeAttribute.LoopI | NodeAttribute.LoopII | NodeAttribute.LoopIII), NodeAttribute.ClosePowerOnTime };
                                SendCMD(NodeAttribute.MacPhaseA, command);                             


                                modelServer.LogicalUI.UserControlEnable.OperateABC = true;
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC =
                                    new OverTimeTimer(NodeAttribute.CloseActionOverTime, () =>
                                    {
                                        ShowMessageBox("三相合闸操作超时", "单相操作");
                                        modelServer.MonitorData.UpdateStatus("三相合闸操作超时");
                                        modelServer.LogicalUI.UserControlEnable.OperateABC = false;
                                        modelServer.LogicalUI.UserControlEnable.CloseReady = true;
                                        modelServer.LogicalUI.UserControlEnable.CloseAction = false;
                                    });
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC.ReStartTimer();
                                if (_loopTestFlag)
                                {
                                    TestParameter.UpadeTip("合闸预制。");
                                }
                            }
                            break;
                        }
                    case "CloseAction":
                        {
                            //默认为两个回路，50ms
                            var command = new byte[] { (byte)CommandIdentify.CloseAction, 
                                (byte)(NodeAttribute.LoopI | NodeAttribute.LoopII | NodeAttribute.LoopIII), NodeAttribute.ClosePowerOnTime };
                            SendCMD(NodeAttribute.MacPhaseA, command);                           

                            break;
                        }
                    case "CloseReadyA":
                        {
                            SinglePhaseReadyAction(CommandIdentify.ReadyClose, NodeAttribute.LoopI, NodeAttribute.ClosePowerOnTime);                           
                            break;
                        }
                    case "CloseActionA":
                        {
                            SinglePhaseReadyAction(CommandIdentify.CloseAction, NodeAttribute.LoopI, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseReadyB":
                        {
                            SinglePhaseReadyAction(CommandIdentify.ReadyClose, NodeAttribute.LoopII, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseActionB":
                        {
                            SinglePhaseReadyAction(CommandIdentify.CloseAction, NodeAttribute.LoopII, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseReadyC":
                        {

                            SinglePhaseReadyAction(CommandIdentify.ReadyClose, NodeAttribute.LoopIII, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseActionC":
                        {
                            SinglePhaseReadyAction(CommandIdentify.CloseAction, NodeAttribute.LoopIII, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "OpenReady":
                        {
                            if (modelServer.LogicalUI.UserControlEnable.OperateState &&
                               (!modelServer.LogicalUI.UserControlEnable.OperateABC))
                            {
                                ShowMessageBox("有正在处理的其它相操作", "整体操作");
                            }


                            if (_loopTestFlag || ShowMessageBox("是否确认 三相分闸预制？", "三相分闸操作"))
                            {
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态 

                                var command = new byte[] { (byte)CommandIdentify.ReadyClose, 
                                    (byte)(NodeAttribute.LoopI | NodeAttribute.LoopII | NodeAttribute.LoopIII), NodeAttribute.OpenPowerOnTime };
                                SendCMD(NodeAttribute.MacPhaseA, command);        


                                modelServer.LogicalUI.UserControlEnable.OperateABC = true;
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC =
                                    new OverTimeTimer(NodeAttribute.OpenActionOverTime, () =>
                                    {
                                        ShowMessageBox("三相分闸操作超时", "单相操作");
                                        modelServer.MonitorData.UpdateStatus("三相分闸操作超时");
                                        modelServer.LogicalUI.UserControlEnable.OperateABC = false;
                                        modelServer.LogicalUI.UserControlEnable.OpenReady = true;
                                        modelServer.LogicalUI.UserControlEnable.OpenAction = false;
                                    });
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC.ReStartTimer();


                                if (_loopTestFlag)
                                {
                                    TestParameter.UpadeTip("分闸预制。");
                                }
                            }
                            break;
                        }
                    case "OpenAction":
                        {
                            var command = new byte[] { (byte)CommandIdentify.OpenAction,
                                (byte)(NodeAttribute.LoopI | NodeAttribute.LoopII | NodeAttribute.LoopIII), (byte)NodeAttribute.OpenPowerOnTime };
                            SendCMD(NodeAttribute.MacPhaseA, command);                            
                            break;
                        }
                    case "OpenReadyA":
                        {
                            SinglePhaseReadyAction(CommandIdentify.ReadyOpen, NodeAttribute.LoopI, NodeAttribute.OpenPowerOnTime);  
                            break;
                        }
                    case "OpenActionA":
                        {
                            SinglePhaseReadyAction(CommandIdentify.OpenAction, NodeAttribute.LoopI, NodeAttribute.OpenPowerOnTime); 
                            break;
                        }
                    case "OpenReadyB":
                        {
                            SinglePhaseReadyAction(CommandIdentify.ReadyOpen, NodeAttribute.LoopII, NodeAttribute.OpenPowerOnTime); 
                            break;
                        }
                    case "OpenActionB":
                        {
                            SinglePhaseReadyAction(CommandIdentify.OpenAction, NodeAttribute.LoopII, NodeAttribute.OpenPowerOnTime); 
                            break;

                        }
                    case "OpenReadyC":
                        {
                            SinglePhaseReadyAction(CommandIdentify.ReadyOpen, NodeAttribute.LoopIII, NodeAttribute.OpenPowerOnTime); 
                            break;
                        }
                    case "OpenActionC":
                        {
                            SinglePhaseReadyAction(CommandIdentify.OpenAction, NodeAttribute.LoopIII, NodeAttribute.OpenPowerOnTime); 
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
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
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

                            modelServer.LogicalUI.GetNdoe(NodeAttribute.MacSynController).ResetState();//复位状态                           

                            SendSynCommand(SynReadyHeDSP);//首先发送命令到同步控制器，置为同步合闸预制状态
                            Thread.Sleep(200);

                            SendSynCMDToABC(CommandIdentify.SyncReadyClose); //分别发送到三相执行同步合闸预制

                            modelServer.LogicalUI.UserControlEnable.OperateSyn = true;
                            modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionSyn =
                                   new OverTimeTimer(NodeAttribute.SynCloseActionOverTime, () =>
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

                            //执行测试时，不提示确认
                            if (_loopTestFlag || ShowMessageBox("是否确认 三相合闸预制？", "三相合闸操作"))
                            {
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态 
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).ResetState();//复位状态 
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).ResetState();//复位状态 
                                //默认为两个回路，50ms
                                var command = new byte[] { (byte)CommandIdentify.ReadyClose, 0x03, NodeAttribute.ClosePowerOnTime };
                                SendCMD(NodeAttribute.MacPhaseA, command);
                                Thread.Sleep(10);
                                SendCMD(NodeAttribute.MacPhaseB, command);
                                Thread.Sleep(10);
                                SendCMD(NodeAttribute.MacPhaseC, command);

                      
                                modelServer.LogicalUI.UserControlEnable.OperateABC = true;
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC =
                                    new OverTimeTimer(NodeAttribute.CloseActionOverTime, () =>
                                    {
                                          ShowMessageBox("三相合闸操作超时", "单相操作");
                                        modelServer.MonitorData.UpdateStatus("三相合闸操作超时");
                                        modelServer.LogicalUI.UserControlEnable.OperateABC = false;
                                        modelServer.LogicalUI.UserControlEnable.CloseReady = true;
                                        modelServer.LogicalUI.UserControlEnable.CloseAction = false;
                                    });
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC.ReStartTimer();
                                if(_loopTestFlag)
                                {
                                    TestParameter.UpadeTip("合闸预制。");
                                }
                            }                          
                            break;
                        }
                    case "CloseAction":
                        {
                            //默认为两个回路，50ms
                            var command = new byte[] { (byte)CommandIdentify.CloseAction, 0x03, NodeAttribute.ClosePowerOnTime };
                            SendCMD(NodeAttribute.MacPhaseA, command);
                            Thread.Sleep(10);
                            SendCMD(NodeAttribute.MacPhaseB, command);
                            Thread.Sleep(10);
                            SendCMD(NodeAttribute.MacPhaseC, command);

                            break;
                        }
                    case "CloseReadyA":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseA, CommandIdentify.ReadyClose, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseActionA":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseA, CommandIdentify.CloseAction, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseReadyB":
                        {                          
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseB, CommandIdentify.ReadyClose, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseActionB":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseB, CommandIdentify.CloseAction, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseReadyC":
                        {

                            SinglePhaseReadyAction(NodeAttribute.MacPhaseC, CommandIdentify.ReadyClose, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "CloseActionC":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseC, CommandIdentify.CloseAction, NodeAttribute.ClosePowerOnTime);
                            break;
                        }
                    case "OpenReady":
                        {
                            if (modelServer.LogicalUI.UserControlEnable.OperateState &&
                               (!modelServer.LogicalUI.UserControlEnable.OperateABC))
                            {
                                ShowMessageBox("有正在处理的其它相操作", "整体操作");
                            }


                            if (_loopTestFlag || ShowMessageBox("是否确认 三相分闸预制？", "三相分闸操作"))
                            {
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态 
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseB).ResetState();//复位状态 
                                modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseC).ResetState();//复位状态 
                                //默认为两个回路，40ms
                                var command = new byte[] { (byte)CommandIdentify.ReadyOpen, 0x03, (byte)NodeAttribute.OpenPowerOnTime };
                                SendCMD(NodeAttribute.MacPhaseA, command);
                                Thread.Sleep(10);
                                SendCMD(NodeAttribute.MacPhaseB, command);
                                Thread.Sleep(10);
                                SendCMD(NodeAttribute.MacPhaseC, command);

                            
                                modelServer.LogicalUI.UserControlEnable.OperateABC = true;
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC =
                                    new OverTimeTimer(NodeAttribute.OpenActionOverTime, () =>
                                    {
                                        ShowMessageBox("三相分闸操作超时", "单相操作");
                                        modelServer.MonitorData.UpdateStatus("三相分闸操作超时");
                                        modelServer.LogicalUI.UserControlEnable.OperateABC = false;
                                        modelServer.LogicalUI.UserControlEnable.OpenReady = true;
                                        modelServer.LogicalUI.UserControlEnable.OpenAction = false;
                                    });
                                modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionABC.ReStartTimer();


                                if (_loopTestFlag)
                                {
                                    TestParameter.UpadeTip("分闸预制。");
                                }
                            }
                            break;
                        }
                    case "OpenAction":
                        {
                            //默认为两个回路，40ms
                            var command = new byte[] { (byte)CommandIdentify.OpenAction, 0x03, (byte)NodeAttribute.OpenPowerOnTime };
                            SendCMD(NodeAttribute.MacPhaseA, command);
                            Thread.Sleep(10);
                            SendCMD(NodeAttribute.MacPhaseB, command);
                            Thread.Sleep(10);
                            SendCMD(NodeAttribute.MacPhaseC, command);
                            break;
                        }
                    case "OpenReadyA":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseA, CommandIdentify.ReadyOpen, NodeAttribute.OpenPowerOnTime);
                            break;
                        }
                    case "OpenActionA":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseA, CommandIdentify.OpenAction, NodeAttribute.OpenPowerOnTime);
                            break;
                        }
                    case "OpenReadyB":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseB, CommandIdentify.ReadyOpen, NodeAttribute.OpenPowerOnTime);
                            break;
                        }
                    case "OpenActionB":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseB, CommandIdentify.OpenAction, NodeAttribute.OpenPowerOnTime);
                            break;
                            
                        }
                    case "OpenReadyC":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseC, CommandIdentify.ReadyOpen, NodeAttribute.OpenPowerOnTime);
                            break;
                        }
                    case "OpenActionC":
                        {
                            SinglePhaseReadyAction(NodeAttribute.MacPhaseC, CommandIdentify.OpenAction, NodeAttribute.OpenPowerOnTime);
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
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }

        /// <summary>
        /// 单相合闸预制
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="cmd"></param>
        private void SinglePhaseReadyAction(byte mac, CommandIdentify cmd, byte time)
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

            var ActionState = (cmd == CommandIdentify.CloseAction) ||
                            (cmd == CommandIdentify.OpenAction);
            //执行状态不在进行确认
            if (ActionState || ShowMessageBox(string.Format("是否确认 {0}相{1}？", des, cmdDes), "单相操作"))
            {                                    
                var command = new byte[] { (byte)cmd, 0x03, (byte)time };
                SendCMD(mac, command);            

                if ((cmd == CommandIdentify.SyncReadyClose) ||
                    (cmd == CommandIdentify.ReadyClose) ||
                    (cmd == CommandIdentify.ReadyOpen))
                {
                    modelServer.LogicalUI.GetNdoe(mac).ResetState();//复位状态 
                    actDelegate(true);

                    var timer = new OverTimeTimer(NodeAttribute.GetOverTime(cmd), () =>
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
        /// 单相合闸预制
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="cmd"></param>
        private void SinglePhaseReadyAction(CommandIdentify cmd,byte loopByte, byte time)
        {

            Action<bool> actDelegate;
            string des = "";
            string cmdDes = modelServer.GetIDDescription(cmd);
            bool opetate = false;

            switch(loopByte)
            {
                case 0x01://A相
                    {
                        actDelegate = ar => { modelServer.LogicalUI.UserControlEnable.OperateA = ar; };
                        opetate = modelServer.LogicalUI.UserControlEnable.OperateA;
                        des = "A";
                        break;
                    }
                case 0x02://B相
                    {
                        actDelegate = ar => { modelServer.LogicalUI.UserControlEnable.OperateB = ar; };
                        opetate = modelServer.LogicalUI.UserControlEnable.OperateB;
                        des = "B";
                        break;
                    }
                case 0x04://C相
                    {
                        actDelegate = ar => { modelServer.LogicalUI.UserControlEnable.OperateC = ar; };
                        opetate = modelServer.LogicalUI.UserControlEnable.OperateC;
                        des = "C";
                        break;
                    }                
                default:
                    {
                        throw new ArgumentException(string.Format("不能使用的回路控制字(0x{0:X2})", loopByte));                       
                    }
            }




            if (modelServer.LogicalUI.UserControlEnable.OperateState &&
                               (!opetate))
            {
                ShowMessageBox("有正在处理的其它相操作", "单相操作");
            }

            var ActionState = (cmd == CommandIdentify.CloseAction) ||
                            (cmd == CommandIdentify.OpenAction);
            //执行状态不在进行确认
            if (ActionState || ShowMessageBox(string.Format("是否确认 {0}相{1}？", des, cmdDes), "单相操作"))
            {
                var command = new byte[] { (byte)cmd, loopByte, (byte)time };

                SendCMD(NodeAttribute.MacPhaseA, command);


                if ((cmd == CommandIdentify.SyncReadyClose) ||
                    (cmd == CommandIdentify.ReadyClose) ||
                    (cmd == CommandIdentify.ReadyOpen))
                {
                    modelServer.LogicalUI.GetNdoe(NodeAttribute.MacPhaseA).ResetState();//复位状态 
                    actDelegate(true);

                    var timer = new OverTimeTimer(NodeAttribute.GetOverTime(cmd), () =>
                    {
                        actDelegate(false);
                        ShowMessageBox(string.Format("{0}相操作超时！", des), "单相操作");
                        modelServer.MonitorData.UpdateStatus(string.Format("{0}相操作超时！", des));
                    });

                    if (loopByte == 0x01)
                    {
                        modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionA = timer;

                    }
                    else if (loopByte == 0x02)
                    {
                        modelServer.LogicalUI.UserControlEnable.OverTimerReadyActionB = timer;

                    }
                    else if (loopByte == 0x04)
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
        private void PollingService_ErrorAckChanged(object sender, StatusChangeMessage e)
        {
            Action errorAckChanged = () => { PollingService_ErrorAckChanged(e); };
            //Task.Factory.StartNew(errorAckChanged,
            //       new System.Threading.CancellationTokenSource().Token, TaskCreationOptions.None, syncContextTaskScheduler).Wait();

            if(Dispatcher.CurrentDispatcher == DispatcherShow)
            {
                Dispatcher.CurrentDispatcher.Invoke(errorAckChanged);
            }
            else
            {
                DispatcherShow.BeginInvoke(errorAckChanged);
            }

        }
        /// <summary>
        /// 错误状态主动应答
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_ErrorAckChanged(StatusChangeMessage e)
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
                    case CommandIdentify.SyncOrchestratorReadyClose:
                    case CommandIdentify.SyncOrchestratorCloseAction:
                        {
                            OverTimeTimer timer;

                            if (e.MAC == NodeAttribute.MacSynController)
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


                            var str = modelServer.GetErrorComment(e.MAC, e.Data);
                            ShowMessageBox(str, "应答错误");


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
        
    }
}