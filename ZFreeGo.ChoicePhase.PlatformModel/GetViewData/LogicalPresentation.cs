using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.DeviceNet.Element;
using ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;

namespace ZFreeGo.ChoicePhase.PlatformModel.GetViewData
{
    /// <summary>
    /// 逻辑呈现， 合分闸指示灯状态，
    /// </summary>
    public class LogicalPresentation : ObservableObject
    {
        private StatusBarMessage statusBar;
        public StatusBarMessage StatusBar
        {
            get
            {
                return statusBar;
            }
            set
            {
                statusBar = value;
                RaisePropertyChanged("StatusBar");
            }
        }

        /// <summary>
        /// 三相指示灯
        /// </summary>
        public IndicatorLight IndicatorLightABC
        {
            get;
            set;
        }

        /// <summary>
        /// 用户控件使能控制
        /// </summary>
        public EnableControl UserControlEnable
        {
            set;
            get;
        }

        /// <summary>
        /// 节点状态
        /// </summary>
        public ObservableCollection<NodeStatus> NodeStatusList
        {
            private set;
            get;
        }

        /// <summary>
        /// 同步相角选择
        /// </summary>
        public PhaseChoice SynPhaseChoice
        {
            private set;
            get;
        }


        private byte _enabitSelect;
        /// <summary>
        /// 使能bit bit0-同步控制器 bit1-A相 bit2-B相 bit3-C相
        /// </summary>
        private byte EnabitSelect
        {
            get
            {
                return _enabitSelect;
            }
            set
            {
                _enabitSelect = value; 
            }
        }


        private byte _onlineBit;
        /// <summary>
        /// 在线位 bit0-同步控制器 bit1-A相 bit2-B相 bit3-C相
        /// </summary>
        public byte OnlineBit
        {
            get
            {
                return _onlineBit;
            }
            private set
            {
                _onlineBit = value;
                if (_onlineBit == _enabitSelect)//A,B,C使能控制
                {
                    UserControlEnable.ControlEnable = true;
                }
                else
                {
                    UserControlEnable.ControlEnable = false;
                }
            }
        }


        #region 同步控制器 A/B/C 离线/在线

        /// <summary>
        /// 更新节点在线状态
        /// </summary>
        /// <param name="index"></param>
        private void UpdateNodeOnlineState(byte mac)
        {
            byte index = 0;
            string des = "";
            switch (mac)
            {
                case 0x0D:
                    {
                        index = 0;
                        des = "同步控制器在线";
                        break;
                    }
                case 0x10:
                    {
                        index = 1;
                        des = "A相在线";
                        break;
                    }
                case 0x12:
                    {
                        index = 2;
                        des = "B相在线";
                        break;
                    }
                case 0x14:
                    {
                        index = 3;
                        des = "C相在线";
                        break;
                    }
                default:
                    {
                        return;
                    }
            }

            //重新启动超时定时器
            var node = GetNdoe(mac);
            node.ReStartOverTimer();
            node.IsOnline = true;
            OnlineBit = SetBit(OnlineBit, index);
            StatusBar.SetSyn(true, des);
           
        }
      

        /// <summary>
        /// 同步控制器 离线/在线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SynController_StatusUpdateEvent(object sender, DataItemSet.StatusMessage e)
        {
            if (e.IsOnline)
            {
                for (int i = 0; i < 1; i++)
                {
                    if (NodeStatusList[0].VoltageLoopCollect[i] != EnergyStatusLoop.Normal)
                    {
                        UpdateStatus("系统电压:" + NodeStatusList[0].VoltageLoopCollect[i].ToString());
                    }
                }
                if (NodeStatusList[0].FrequencyLoopCollect[0] != EnergyStatusLoop.Normal)
                {
                    UpdateStatus("系统频率:" + NodeStatusList[0].FrequencyLoopCollect[0].ToString());
                }

                if (!e.Node.LastOnline)
                {
                    StatusBar.SetSyn(true, "同步控制器在线");
                    UpdateStatus("同步控制器恢复在线");
                }
                OnlineBit = SetBit(OnlineBit, 0);
            }
            else
            {
                var comment = "同步控制超时离线";
                StatusBar.SetSyn(false, comment);
                UpdateStatus(comment);
                OnlineBit = ClearBit(OnlineBit, 0);

            }
        }

        /// <summary>
        /// A相状态更新事件 离线/在线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhaseA_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                //说明最后一次为非在线状态
                if (!e.Node.LastOnline)
                {
                    StatusBar.SetPhaseA(true, "A相在线");
                    UpdateStatus("A相恢复在线");
                }
                OnlineBit = SetBit(OnlineBit, 1);
            }
            else
            {
                StatusBar.SetPhaseA(false, "");
                UpdateStatus("A相超时离线");
                OnlineBit = ClearBit(OnlineBit, 1);
            }

            SetControlButtonState();

        }

        /// <summary>
        /// B相状态更新事件 离线/在线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhaseB_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                //说明最后一次为非在线状态
                if (!e.Node.LastOnline)
                {
                    StatusBar.SetPhaseB(true, "B相在线");
                    UpdateStatus("B相恢复在线");
                }
                OnlineBit = SetBit(OnlineBit, 2);
            }
            else
            {
                StatusBar.SetPhaseB(false, "");
                UpdateStatus("B相超时离线");
                OnlineBit = ClearBit(OnlineBit, 2);
            }

            SetControlButtonState();
        }

        /// <summary>
        /// C相状态更新事件 离线/在线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhaseC_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                //说明最后一次为非在线状态
                if (!e.Node.LastOnline)
                {
                    StatusBar.SetPhaseC(true, "C相在线");
                    UpdateStatus("C相恢复在线");
                }
                OnlineBit = SetBit(OnlineBit, 3);
            }
            else
            {
                StatusBar.SetPhaseC(false, "");
                UpdateStatus("C相超时离线");
                OnlineBit = ClearBit(OnlineBit, 3);
            }
            SetControlButtonState();
        }
        #endregion


        #region 操作按钮
        /// <summary>
        /// 设置用户控件使能
        /// </summary>
        /// <param name="mac">mac地址</param>
        /// <param name="cmd">应答命令</param>
        private void SetUserControlEnable(byte mac, CommandIdentify cmd)
        {

            switch (mac)
            {
                case 0x0D://同步控制器                    
                    {
                        switch (cmd)
                        {
                            //执行合闸
                            case CommandIdentify.SyncOrchestratorCloseAction:
                                {

                                    break;
                                }
                            //预备合闸
                            case CommandIdentify.SyncOrchestratorReadyClose:
                                {

                                    break;
                                }
                        }
                        break;
                    }
                case 0x10: //A相  
                    {
                        switch (cmd)
                        {
                            case CommandIdentify.CloseAction://合闸执行
                                {
                                    UserControlEnable.CloseReadyA = true;
                                    UserControlEnable.CloseActionA = false;

                                    //停止响应的超时定时器
                                    if (UserControlEnable.OverTimerReadyActionA != null)
                                    {
                                        UserControlEnable.OverTimerReadyActionA.StopTimer();
                                    }
                                    UserControlEnable.OperateA = false;
                                    break;
                                }
                            case CommandIdentify.ReadyClose: // 合闸预制
                                {
                                    UserControlEnable.CloseReadyA = false;
                                    UserControlEnable.CloseActionA = true;


                                    break;
                                }
                            case CommandIdentify.ReadyOpen:  //分闸预制                   
                                {
                                    UserControlEnable.OpenReadyA = false;
                                    UserControlEnable.OpenActionA = true;
                                    break;
                                }
                            case CommandIdentify.OpenAction: //分闸执行
                                {
                                    UserControlEnable.OpenReadyA = true;
                                    UserControlEnable.OpenActionA = false;

                                    //停止响应的超时定时器
                                    if (UserControlEnable.OverTimerReadyActionA != null)
                                    {
                                        UserControlEnable.OverTimerReadyActionA.StopTimer();
                                    }
                                    UserControlEnable.OperateA = false;
                                    break;
                                }

                            case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                                {

                                    break;
                                }

                        }

                        break;
                    }
                case 0x12://B相 
                    {
                        switch (cmd)
                        {
                            case CommandIdentify.CloseAction://合闸执行
                                {
                                    UserControlEnable.CloseReadyB = true;
                                    UserControlEnable.CloseActionB = false;

                                    //停止响应的超时定时器
                                    if (UserControlEnable.OverTimerReadyActionB != null)
                                    {
                                        UserControlEnable.OverTimerReadyActionB.StopTimer();
                                    }
                                    UserControlEnable.OperateB = false;
                                    break;
                                }
                            case CommandIdentify.ReadyClose: // 合闸预制
                                {
                                    UserControlEnable.CloseReadyB = false;
                                    UserControlEnable.CloseActionB = true;


                                    break;
                                }
                            case CommandIdentify.ReadyOpen:  //分闸预制                   
                                {
                                    UserControlEnable.OpenReadyB = false;
                                    UserControlEnable.OpenActionB = true;
                                    break;
                                }
                            case CommandIdentify.OpenAction: //分闸执行
                                {
                                    UserControlEnable.OpenReadyB = true;
                                    UserControlEnable.OpenActionB = false;

                                    //停止响应的超时定时器
                                    if (UserControlEnable.OverTimerReadyActionB != null)
                                    {
                                        UserControlEnable.OverTimerReadyActionB.StopTimer();
                                    }
                                    UserControlEnable.OperateB = false;

                                    break;
                                }

                            case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                                {

                                    break;
                                }

                        }
                        break;
                    }
                case 0x14: //C相
                    {

                        switch (cmd)
                        {
                            case CommandIdentify.CloseAction://合闸执行
                                {
                                    UserControlEnable.CloseReadyC = true;
                                    UserControlEnable.CloseActionC = false;

                                    //停止响应的超时定时器
                                    if (UserControlEnable.OverTimerReadyActionC != null)
                                    {
                                        UserControlEnable.OverTimerReadyActionC.StopTimer();
                                    }
                                    UserControlEnable.OperateC = false;
                                    break;
                                }
                            case CommandIdentify.ReadyClose: // 合闸预制
                                {
                                    UserControlEnable.CloseReadyC = false;
                                    UserControlEnable.CloseActionC = true;
                                    break;
                                }
                            case CommandIdentify.ReadyOpen:  //分闸预制                   
                                {
                                    UserControlEnable.OpenReadyC = false;
                                    UserControlEnable.OpenActionC = true;
                                    break;
                                }
                            case CommandIdentify.OpenAction: //分闸执行
                                {
                                    UserControlEnable.OpenReadyC = true;
                                    UserControlEnable.OpenActionC = false;

                                    //停止响应的超时定时器
                                    if (UserControlEnable.OverTimerReadyActionC != null)
                                    {
                                        UserControlEnable.OverTimerReadyActionC.StopTimer();
                                    }
                                    UserControlEnable.OperateC = false;
                                    break;
                                }

                            case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                                {

                                    break;
                                }

                        }

                        break;
                    }

            }
        }

        /// <summary>
        /// 更新设置控件状态
        /// </summary>
        public void SetControlButtonState()
        {
            for (int i = 1; i < 4; i++)
            {
                //电能正常，且属于合位，使能分闸按钮
                if (NodeStatusList[i].EnergyStatus == EnergyStatusLoop.Normal)
                {
                    UserControlEnable.ChooiceEnableButton((UInt16)((i << 8) | ((byte)NodeStatusList[i].PositStatus)));
                }
                else
                {
                    UserControlEnable.ChooiceEnableButton((UInt16)((i << 8)));//关闭所有
                }
            }
        }
        #endregion


        #region 合分闸预制执行状态
        /// <summary>
        /// 合闸预制状态
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="data"></param>
        public void UpdateNodeStatus(byte mac, byte[] data)
        {
            var node = GetNdoe(mac);
            if (node == null)
            {
                return;
            }
            UpdateNodeOnlineState(mac);
            //if (!node.IsValid(data))
            //{
            //    UpdateStatus(node.Mac.ToString("X2") + "应答数据无效！");
            //    return;
            //}
            var cmd = (CommandIdentify)(data[0] & 0x7F);

            node.ResetState();
            switch (mac)
            {
                case 0x0D://同步控制器                    
                    {
                        switch (cmd)
                        {
                            //执行合闸
                            case CommandIdentify.SyncOrchestratorCloseAction:
                                {
                                    node.SynActionCloseState = true;
                                    node.SynReadyCloseState = false;
                                    UpdateStatus("同步控制器，执行合闸");
                                    break;
                                }
                            //预备合闸
                            case CommandIdentify.SyncOrchestratorReadyClose:
                                {
                                    node.SynReadyCloseState = true;
                                    node.SynActionCloseState = false;
                                    UpdateStatus("同步控制器，预制合闸");
                                    break;
                                }
                        }
                        break;
                    }
                case 0x10: //A相                  
                case 0x12://B相             
                case 0x14: //C相
                    {

                        switch (cmd)
                        {
                            case CommandIdentify.CloseAction://合闸执行
                                {
                                    node.ActionCloseState = true;
                                    node.ReadyCloseState = false;
                                    UpdateStatus(mac, "合闸执行");
                                    break;
                                }
                            case CommandIdentify.OpenAction: //分闸执行
                                {
                                    node.ActionOpenState = true;
                                    node.ReadyOpenState = false;
                                    UpdateStatus(mac, "分闸预制");
                                    break;
                                }
                            case CommandIdentify.ReadyClose: // 合闸预制
                                {
                                    node.ReadyCloseState = true;
                                    node.ActionCloseState = false;
                                    UpdateStatus(mac, "合闸预制");

                                    break;
                                }
                            case CommandIdentify.ReadyOpen:  //分闸预制                   
                                {
                                    node.ReadyOpenState = true;
                                    node.ActionOpenState = false;
                                    UpdateStatus(mac, "分闸预制");
                                    break;
                                }
                            case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                                {
                                    UpdateStatus(mac, "同步合闸预制");
                                    node.SynReadyCloseState = true;

                                    break;
                                }

                        }
                        break;
                    }

            }
            //设置用户控件使能状态

            //不是ABC 一起动模式

            if (!UserControlEnable.OperateABC)
            {
                SetUserControlEnable(mac, cmd);
            }
            else
            {
                //ABC一起动模式
                //均为合闸预制模式
                if (NodeStatusList[1].ReadyCloseState && NodeStatusList[2].ReadyCloseState && NodeStatusList[3].ReadyCloseState)
                {
                    UserControlEnable.CloseReady = false;
                    UserControlEnable.CloseAction = true;
                }
                if (NodeStatusList[1].ActionCloseState && NodeStatusList[2].ActionCloseState && NodeStatusList[3].ActionCloseState)
                {
                    UserControlEnable.CloseReady = true;
                    UserControlEnable.CloseAction = false;
                    UserControlEnable.OverTimerReadyActionABC.StopTimer();
                    UserControlEnable.OperateABC = false;
                }

                //均为分闸预制模式
                if (NodeStatusList[1].ReadyOpenState && NodeStatusList[2].ReadyOpenState && NodeStatusList[3].ReadyOpenState)
                {
                    UserControlEnable.OpenReady = false;
                    UserControlEnable.OpenAction = true;
                }
                if (NodeStatusList[1].ActionOpenState && NodeStatusList[2].ActionOpenState && NodeStatusList[3].ActionOpenState)
                {
                    UserControlEnable.OpenReady = true;
                    UserControlEnable.OpenAction = false;
                    UserControlEnable.OverTimerReadyActionABC.StopTimer();
                    UserControlEnable.OperateABC = false;
                }

            }
            //同步合闸模式
            if (UserControlEnable.OperateSyn)
            {
                if (NodeStatusList[0].SynReadyCloseState && NodeStatusList[1].SynReadyCloseState &&
                    NodeStatusList[2].SynReadyCloseState && NodeStatusList[3].SynReadyCloseState)
                {
                    UserControlEnable.SynCloseAction = true;
                    UserControlEnable.SynCloseReady = false;
                }
                if (NodeStatusList[0].SynActionCloseState)
                {
                    UserControlEnable.SynCloseAction = false;
                    UserControlEnable.SynCloseReady = true;

                    UserControlEnable.OverTimerReadyActionSyn.StopTimer();
                    UserControlEnable.OperateSyn = false;
                }

            }
        }

        #endregion




        /// <summary>
        /// 获根据MAC地址获取节点状态
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public NodeStatus GetNdoe(byte mac)
        {
            foreach (var m in NodeStatusList)
            {
                if (m.Mac == mac)
                {
                    return m;
                }
            }
            return null;
        }






        /// <summary>
        /// 更新节点状态改变信息，循环心跳报告等
        /// </summary>
        /// <param name="mac">MAC ID</param>
        /// <param name="data">数据</param>
        internal void UpdateNodeStatusChange(byte mac, byte[] data)
        {
            var node = GetNdoe(mac);
            if (node == null)
            {
                return;
            }

            switch (mac)
            {
                case 0x0D:
                    {
                        node.UpdateSynStatus(data);
                        break;
                    }
                case 0x10:
                case 0x12:
                case 0x14:
                    {
                        node.UpdateSwitchStatus(data);
                        break;
                    }


            }

        }


        #region 更新状态栏信息
        /// <summary>
        /// 更新站点信息
        /// </summary>
        /// <param name="defStationInformation"></param>
        internal void UpdateStationStatus(DeviceNet.Element.DefStationInformation defStationInformation)
        {
            Action<bool, string> action;
            string comment = "";


            switch (defStationInformation.MacID)
            {

                case 0x0d:
                    {
                        action = statusBar.SetSyn;
                        comment = "同步控制器";
                        break;
                    }
                case 0x10:
                    {
                        action = statusBar.SetPhaseA;
                        comment = "A相";
                        break;
                    }
                case 0x12:
                    {
                        action = statusBar.SetPhaseB;
                        comment = "B相";
                        break;
                    }
                case 0x14:
                    {
                        action = statusBar.SetPhaseC;
                        comment = "C相";
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
            switch (defStationInformation.Step)
            {
                case NetStep.Start:
                    {
                        comment += "启动连接";
                        action(false, comment);
                        break;
                    }
                case NetStep.Linking:
                    {
                        comment += "建立显示连接";
                        action(true, comment);
                        break;
                    }
                case NetStep.StatusChange:
                    {
                        comment += "建立状态变化连接";
                        action(true, comment);
                        break;
                    }
                case NetStep.Cycle:
                    {
                        comment += "在线";
                        action(true, comment);
                        break;
                    }
                default:
                    {
                        return;
                    }

            }

            UpdateStatus(comment);



        }
        #endregion


        private Action<string> UpdateStatusDelegate;

     /// <summary>
     /// 更新状态信息
     /// </summary>
     /// <param name="des"></param>
       public void UpdateStatus(string des)
        {
           if (UpdateStatusDelegate != null)
           {
               UpdateStatusDelegate(des);
           }
        }
        public void UpdateStatus(byte mac, string des)
        {
            switch (mac)
            {
                case 0x10:
                    {

                        UpdateStatus("A相:" + des);
                        break;
                    }
                case 0x12:
                    {

                        UpdateStatus("B相:" + des);
                        break;
                    }
                case 0x14:
                    {

                        UpdateStatus("C相:" + des);
                        break;
                    }
                default:
                    {
                        UpdateStatus(des);
                        break;
                    }

            }

        }

        /// <summary>
        /// 将其中deq位设置1，[0,7]
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="seq">位数</param>
        /// <returns>置位后的数据</returns>
        private byte SetBit(byte value, byte seq)
        {
            return (byte)(value | (1 << seq));
        }
        /// <summary>
        /// 将其中deq位设清0，[0,7]
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="seq">位数</param>
        /// <returns>清除相应位后的数据</returns>
        private byte ClearBit(byte value, byte seq)
        {
            return (byte)(value & (~(1 << seq)));
        }

        /// <summary>
        /// 初始化逻辑呈现
        /// </summary>
        public LogicalPresentation(Action<string> updateDelegate)
        {
            try
            {
                IndicatorLightABC = new IndicatorLight();
                NodeStatusList = new ObservableCollection<NodeStatus>();
                NodeStatusList.Add(new NodeStatus(0x0D, "同步控制器", 7000));
                NodeStatusList.Add(new NodeStatus(0x10, "A相控制器", 7000));
                NodeStatusList.Add(new NodeStatus(0x12, "B相控制器", 7000));
                NodeStatusList.Add(new NodeStatus(0x14, "C相控制器", 7000));




                NodeStatusList[0].StatusUpdateEvent += SynController_StatusUpdateEvent;
                NodeStatusList[1].StatusUpdateEvent += PhaseA_StatusUpdateEvent;
                NodeStatusList[2].StatusUpdateEvent += PhaseB_StatusUpdateEvent;
                NodeStatusList[3].StatusUpdateEvent += PhaseC_StatusUpdateEvent;
                //ABC指示灯
                NodeStatusList[1].StatusUpdateEvent += IndicatorLightABC.PhaseA_StatusUpdateEvent;
                NodeStatusList[2].StatusUpdateEvent += IndicatorLightABC.PhaseB_StatusUpdateEvent;
                NodeStatusList[3].StatusUpdateEvent += IndicatorLightABC.PhaseC_StatusUpdateEvent;



                StatusBar = new StatusBarMessage("Admin");

                _enabitSelect = 0x07;//使能选择bit 
                _onlineBit = 0;

                UserControlEnable = new EnableControl();

                UpdateStatusDelegate = updateDelegate;

                SynPhaseChoice = new PhaseChoice();

                
            }
            catch(Exception ex)
            {

            }
        }
        
    }
}
