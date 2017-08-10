using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using ZFreeGo.ChoicePhase.DeviceNet;
using ZFreeGo.ChoicePhase.DeviceNet.Element;
using ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer;
using ZFreeGo.ChoicePhase.Modbus;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;
using ZFreeGo.ChoicePhase.PlatformModel.Helper;
using ZFreeGo.Monitor.DASModel.GetViewData;

namespace ZFreeGo.ChoicePhase.PlatformModel
{

    /// <summary>
    /// PlatformModel服务
    /// </summary>
    public partial class PlatformModelServer
    {
   


        /// <summary>
        /// 获取监控数据
        /// </summary>
        public MonitorViewData MonitorData
        {
            private set;
            get;
        }
       

        /// <summary>
        /// 逻辑呈现UI
        /// </summary>
        public LogicalPresentation LogicalUI
        {
            private set;
            get;
        }


        /// <summary>
        /// 通讯服务
        /// </summary>
        public CommunicationServer CommServer
        {
            private set;
            get;
        }
        /// <summary>
        ///RTU处理服务
        /// </summary>
        public RtuServer RtuServer
        {
            private set;
            get;
        }

        /// <summary>
        /// DeviceNet服务
        /// </summary>
        public DeviceNetServer ControlNetServer
        {
            private set;
            get;
        }

        /// <summary>
        /// 站点信息列表
        /// </summary>
        public List<DefStationInformation> StationInformation
        {
            get;
            set;
        }

        /// <summary>
        /// 本地地址
        /// </summary>
        private readonly byte _localAddr;

        /// <summary>
        /// 目的地址
        /// </summary>
        private readonly byte _destinateAddr;



        /// <summary>
        /// 光纤CAN地址
        /// </summary>
        private readonly byte _laserCANAddr;


        /// <summary>
        /// 常规功能码--用于DeviceNet数据下行，上位机到终端
        /// </summary>
        private readonly byte _downCode;
        /// <summary>
        /// 常规功能码--用于DeviceNet数据上行，终端到上位机
        /// </summary>
        private readonly byte _upCode;

        /// <summary>
        /// 超时定时器用于设备离线状态
        /// </summary>
        private OverTimeTimer _deviceOverTimer;

        /// <summary>
        /// 设备离线超时时间
        /// </summary>
        private int _DeviceOverTime;

        /// <summary>
        /// 任务调度器
        /// </summary>
        private readonly TaskScheduler syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        /// <summary>
        /// 初始化控制平台Model服务
        /// </summary>
        public PlatformModelServer()
        {
            try
            {
                _localAddr = 0x1A;
                _laserCANAddr = 0xF1;
                _DeviceOverTime = 7000;
                _destinateAddr = 0xA1;
                _downCode = 0x55;
                _upCode = 0xAA;
                _multiFrameBuffer = new List<byte>();
                _lastIndex = 0;
                

                UpadteConfigParameter();


                MonitorData = new MonitorViewData();

                CommServer = new CommunicationServer(_destinateAddr);
                CommServer.CommonServer.SerialDataArrived += CommonServer_SerialDataArrived;

                RtuServer = new RtuServer(_localAddr, 500, SendRtuData, ExceptionDeal);
                RtuServer.RtuFrameArrived += RtuServer_RtuFrameArrived;
                

                StationInformation = new List<DefStationInformation>();
                StationInformation.Add(new DefStationInformation(NodeAttribute.MacSynController, ((NodeAttribute.EnabitSelect & 0x01) == 0x01), "同步控制器"));
                StationInformation.Add(new DefStationInformation(NodeAttribute.MacPhaseA, ((NodeAttribute.EnabitSelect & 0x02) == 0x02), "A相"));
                StationInformation.Add(new DefStationInformation(NodeAttribute.MacPhaseB, ((NodeAttribute.EnabitSelect & 0x04) == 0x04), "B相"));
                StationInformation.Add(new DefStationInformation(NodeAttribute.MacPhaseC, ((NodeAttribute.EnabitSelect & 0x08) == 0x08), "C相"));


                ControlNetServer = new DeviceNetServer(PacketDevicetNetData, ExceptionDeal, StationInformation);
                ControlNetServer.PollingService.ArrtributesArrived += PollingService_ArrtributesArrived;
                ControlNetServer.PollingService.MultiFrameArrived += PollingService_MultiFrameArrived;
                ControlNetServer.PollingService.ReadyActionArrived += PollingService_ReadyActionArrived;
                ControlNetServer.PollingService.SubStationStatusChanged +=PollingService_SubStationStatusChanged;
                ControlNetServer.PollingService.ErrorAckChanged += PollingService_ErrorAckChanged;
                ControlNetServer.PollingService.NormalStatusArrived += PollingService_NormalStatusArrived;
                ControlNetServer.StationArrived +=ControlNetServer_StationArrived;

                LogicalUI = new LogicalPresentation(MonitorData.UpdateStatus);
                _deviceOverTimer = new OverTimeTimer(_DeviceOverTime, DeviceOverTimeDeal);
                
            }
            catch(Exception ex)
            {               
                ZFreeGo.Common.LogTrace.CLog.LogError(ex.StackTrace);
            }

        }


        /// <summary>
        /// 其它功能码统一处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_NormalStatusArrived(object sender, StatusChangeMessage e)
        {
            LogicalUI.UpdatePramter(e);
        }

        /// <summary>
        /// 子站主动错误应答事件,信息栏显示主动应答错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_ErrorAckChanged(object sender, StatusChangeMessage e)
        {
            MonitorData.UpadeExceptionMessage(GetErrorComment(e.MAC, e.Data));
        }

        /// <summary>
        /// 更新站点连接信息，建立连接等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlNetServer_StationArrived(object sender, StationEventArgs e)
        {
            Task.Factory.StartNew(() => LogicalUI.UpdateStationStatus(e.Station),
                    new System.Threading.CancellationTokenSource().Token, TaskCreationOptions.None, syncContextTaskScheduler).Wait();           
        }
        
        /// <summary>
        /// 从站状态改变信息,循环报告信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_SubStationStatusChanged(object sender, StatusChangeMessage e)
        {
            LogicalUI.UpdateNodeStatusChange(e.MAC, e.Data);            
        }

        /// <summary>
        /// 预制分闸，合闸，同步设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PollingService_ReadyActionArrived(object sender, StatusChangeMessage e)
        {
            LogicalUI.UpdateNodeStatus(e.MAC, e.Data);
        }


     


        /// <summary>
        /// 从站上传属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PollingService_ArrtributesArrived(object sender, ArrtributesEventArgs e)
        {
            MonitorData.UpdateAttributeData(e.MAC, e.ID, e.AttributeByte);
        }



        /// <summary>
        /// 发送RTU数据包，并更新到信息栏显示
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>true--正常发送，false--端口关闭</returns>
        private bool SendRtuData(byte[] data)
        {
            if(CommServer.CommonServer.CommState)
            {
                CommServer.CommonServer.SendMessage(data);

                var rawDataStr = ByteToString(data, 0, data.Length);
                CommServer.UpadteSendLinkMessage(rawDataStr);
                CommServer.RawSendMessage += rawDataStr;
            }
            return CommServer.CommonServer.CommState;            
        }

        /// <summary>
        /// 打包DevicetNet数据为RTU数据并发送。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool PacketDevicetNetData(byte[] data)
        {
            var frame = new RTUFrame(_destinateAddr, _downCode, data, (byte)data.Length);
            SendRtuData(frame.Frame);
            return true;
        }

        



        /// <summary>
        /// RTU帧数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void RtuServer_RtuFrameArrived(object sender, RtuFrameArgs e)
        {
            var rawStr = ByteToString(e.Frame.Frame, 0, e.Frame.Frame.Length) ;
            CommServer.UpadteReciveLinkMessage(rawStr);          
           
            var can = GetCanMessage(e.Frame);
            if(can != null)
            {
                //是否为光纤CAN直接上传信息
                if (can.ID == _laserCANAddr)
                {
                    if (can.DataLen >= 6)
                    {
                        switch(can.Data[0])
                        {
                                //状态返回
                            case 0x91:
                                {
                                    LogicalUI.StatusBar.SetDevice(true);
                                    var des = GetCANError(can);
                                    //为空正常状态则不进行更新
                                    if (des != "")
                                    {
                                        MonitorData.UpdateStatus("设备状态:" + des);
                                    }                                    
                                    _deviceOverTimer.ReStartTimer();
                                    //如果不是激活状态，则在收到连接后重新建立连接
                                    if (!ControlNetServer.IsActive)
                                    {
                                        ControlNetServer.RestartLinkServer();
                                    }
                                    break;
                                }
                            default:
                                {
                                    //状态返回
                                    break;
                                }
                        }
                    }                  
                }
                else
                {
                    ControlNetServer.ReciveCenter(can);
                }                
            }                      
        }

   

        /// <summary>
        /// 通讯数据到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommonServer_SerialDataArrived(object sender, Communication.SerialDataEvent e)
        {
            RtuServer.AddBuffer(e.SerialData);
            CommServer.RawReciveMessage += ByteToString(e.SerialData, 0, e.SerialData.Length);                        
        }
        /// <summary>
        /// 异常消息处理，此处用于显示
        /// </summary>
        /// <param name="ex"></param>
        void ExceptionDeal(Exception ex)
        {         
            MonitorData.UpadeExceptionMessage(ex.Message);            
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Close()
        {
            CommServer.CommonServer.Close();
            RtuServer.Close();
            ControlNetServer.Close();
        }



        private static PlatformModelServer _modelServer; 


        /// <summary>
        /// 获取Model服务
        /// </summary>
        /// <returns>Model Server</returns>
        public static PlatformModelServer GetServer()
        {
            return GetServer(false);
        }


        /// <summary>
        /// 获取Model服务
        /// </summary>
        /// <param name="flag">ture-重新新建， false--采用默认</param>
        /// <returns>Model Server</returns>
        public static PlatformModelServer GetServer(bool flag)
        {
           
            if((_modelServer == null) || flag)
            {
                _modelServer =  new PlatformModelServer();
                return _modelServer;
            }
            else
            {
                return _modelServer;
            }

        }
    }


    
}
