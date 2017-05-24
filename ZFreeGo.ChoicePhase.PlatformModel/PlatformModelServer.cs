using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ZFreeGo.ChoicePhase.DeviceNet;
using ZFreeGo.ChoicePhase.DeviceNet.Element;
using ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer;
using ZFreeGo.ChoicePhase.Modbus;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;
using ZFreeGo.Monitor.DASModel.GetViewData;

namespace ZFreeGo.ChoicePhase.PlatformModel
{




    public class PlatformModelServer
    {
        private byte _localAddr;

        /// <summary>
        /// 监控数据
        /// </summary>
        private MonitorViewData _monitorViewData;

        /// <summary>
        /// 获取监控数据
        /// </summary>
        public MonitorViewData MonitorData
        {
            get
            {
                return _monitorViewData;
            }
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

        ///// <summary>
        ///// 站服务列表
        ///// </summary>
        //public MasterStationServer StationServer
        //{
        //    private set;
        //    get;
        //}


        /// <summary>
        /// DeviceNet服务
        /// </summary>
        public DeviceNetServer ControlNetServer
        {
            private set;
            get;
        }


        /// <summary>
        /// 多帧帧缓冲
        /// </summary>
        private List<byte> _multiFrameBuffer;
        private byte _lastIndex;
        /// <summary>
        /// 初始化控制平台Model服务
        /// </summary>
        public PlatformModelServer()
        {
            try
            {
                _localAddr = 0x1A;
                _monitorViewData = new MonitorViewData();
                CommServer = new CommunicationServer();
                CommServer.CommonServer.SerialDataArrived += CommonServer_SerialDataArrived;
                RtuServer = new RtuServer(_localAddr, 500, sendRtuData, ExceptionDeal);
                RtuServer.RtuFrameArrived += RtuServer_RtuFrameArrived;

                _multiFrameBuffer = new List<byte>();
                _lastIndex = 0;

                ControlNetServer = new DeviceNetServer(PacketDevicetNetData, ExceptionDeal);
                ControlNetServer.PollingService.ArrtributesArrived += PollingService_ArrtributesArrived;
                ControlNetServer.PollingService.MultiFrameArrived += PollingService_MultiFrameArrived;
                ControlNetServer.PollingService.ReadyActionArrived += PollingService_ReadyActionArrived;
                ControlNetServer.PollingService.SubStationStatusChanged +=PollingService_SubStationStatusChanged;

                ControlNetServer.StationArrived +=ControlNetServer_StationArrived;


                FlashDelegate = ar =>
                {
                    if (ar)
                    {
                       
                        MonitorData.StatusBar.ComBrush = new System.Windows.Media.SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                       
                        MonitorData.StatusBar.ComBrush = new System.Windows.Media.SolidColorBrush(Colors.Red);
                    }
                };

            }
            catch(Exception ex)
            {
                //CommServer.LinkMessage += ex.StackTrace;
                ZFreeGo.Common.LogTrace.CLog.LogError(ex.StackTrace);
            }

        }

        private readonly TaskScheduler syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        /// <summary>
        /// 连接信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlNetServer_StationArrived(object sender, StationEventArgs e)
        {
            Task.Factory.StartNew(() => _monitorViewData.UpdateStationStatus(e.Station),
                    new System.Threading.CancellationTokenSource().Token, TaskCreationOptions.None, syncContextTaskScheduler).Wait();
           ;
        }
        

        /// <summary>
        /// 从站状态改变信息,循环报告信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_SubStationStatusChanged(object sender, StatusChangeMessage e)
        {
            _monitorViewData.UpdateNodeStatusChange(e.MAC, e.Data);
        }

        /// <summary>
        /// 预制分闸，合闸，同步设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PollingService_ReadyActionArrived(object sender, StatusChangeMessage e)
        {
            _monitorViewData.UpdateNodeStatus(e.MAC,  e.Data);
        }



        void PollingService_MultiFrameArrived(object sender, MultiFrameEventArgs e)
        {
            //判断是否为首帧
            if (e.Index == 0)
            {
                _multiFrameBuffer.Clear();//清空历史数据
                _lastIndex = 0;
                 _multiFrameBuffer.AddRange(e.ByteData);
                return;
            }

            int currentIndex = e.Index & 0x7F; //当前索引
            //是否为递增索引,正常接收
            if ((_lastIndex + 1) == currentIndex)
            {
                _multiFrameBuffer.AddRange(e.ByteData);
                _lastIndex++;
            }
            else
            {

            }
            //判断是否为最后一帧
            if ((e.Index & 0x80) == 0x80)
            {

                //TODO:显示
                MonitorData.StatusMessage += "\n\n" + DateTime.Now.ToLongTimeString() + "  多帧接收:\n";
                MonitorData.StatusMessage += "接收完成"+ "\n";
                //适当处理
                StringBuilder stb = new StringBuilder(_multiFrameBuffer.Count*4);
                if (_multiFrameBuffer.Count % 2 == 0)
                {
                    for (int i = 0; i < _multiFrameBuffer.Count; i += 2)
                    {
                        stb.AppendFormat("{0},", _multiFrameBuffer[i] + 256*_multiFrameBuffer[i + 1]);
                    }
                    MonitorData.StatusMessage += "\n\n" + "  有效数据:\n";
                    MonitorData.StatusMessage += stb.ToString() + "\n";
                }
            }

            
        }






        void PollingService_ArrtributesArrived(object sender, ArrtributesEventArgs e)
        {
            
            _monitorViewData.UpdateAttributeData(e.MAC, e.ID, e.AttributeByte);
        }

        /// <summary>
        /// 发送数据转包
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>true--正常发送，false--端口关闭</returns>
        private bool sendRtuData(byte[] data)
        {
            if(CommServer.CommonServer.CommState)
            {
                CommServer.CommonServer.SendMessage(data);
                CommServer.LinkMessage += "\n" +DateTime.Now.ToLongTimeString() + "  RTU发送帧:\n";
                CommServer.LinkMessage += ByteToString(data, 0, data.Length);
                CommServer.RawSendMessage += ByteToString(data, 0, data.Length);
            }
            return CommServer.CommonServer.CommState;            
        }

        /// <summary>
        /// 打包发送数据并发送
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool PacketDevicetNetData(byte[] data)
        {
            var frame = new RTUFrame(0xA1, 0x55, data, (byte)data.Length);
            sendRtuData(frame.Frame);
            return true;
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



        /// <summary>
        /// RTU帧数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void RtuServer_RtuFrameArrived(object sender, RtuFrameArgs e)
        {
            CommServer.LinkMessage += "\n" + DateTime.Now.ToLongTimeString() + "  接收RTU帧:\n";
            CommServer.LinkMessage += ByteToString(e.Frame.Frame, 0, e.Frame.Frame.Length);
            
            //StationServer.StationDeal(e.Frame.FrameData);
            var can = GetCanMessage(e.Frame);
            if(can != null)
            {
                ControlNetServer.ReciveCenter(can);
            }            
            
        }
        private CanMessage GetCanMessage(RTUFrame frame)
        {
            if (frame.Function != 0xAA)//是否为0xAA 是否为上送
            {
                return null;
            }
            if (frame.DataLen <= 2)
            {
                return null;
            }
            var id = frame.FrameData[0] + ((ushort)(frame.FrameData[1]) << 8);
            var can = new CanMessage((ushort)id, frame.FrameData, 2, frame.DataLen - 2);
            return can;

        }
        bool flag = false;
        Action<bool> FlashDelegate;
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

        void ExceptionDeal(Exception ex)
        {
            MonitorData.ExceptionMessage += "\n" + DateTime.Now.ToLongTimeString() + "异常处理:\n";
            MonitorData.ExceptionMessage += ex.Message;
            MonitorData.ExceptionMessage += ex.StackTrace;
            
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

        private string ByteToString(byte[] data, int start, int len)
        {
            StringBuilder strBuild = new StringBuilder(len*3 + 10);
            for(int i = start; i < start + len; i++)
            {
                strBuild.AppendFormat("{0:X2} ", data[i]);
            }
           
            return strBuild.ToString();
        }


        



    }


    
}
