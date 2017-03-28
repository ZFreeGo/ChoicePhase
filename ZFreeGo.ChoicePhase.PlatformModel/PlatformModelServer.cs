using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <summary>
        /// 初始化控制平台Model服务
        /// </summary>
        public PlatformModelServer()
        {
            _localAddr = 0xA1;
            _monitorViewData = new MonitorViewData();
            CommServer = new CommunicationServer();
            CommServer.CommonServer.SerialDataArrived += CommonServer_SerialDataArrived;
            RtuServer = new RtuServer(_localAddr, 500, sendRtuData);
            RtuServer.RtuFrameArrived += RtuServer_RtuFrameArrived;


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
        /// RTU帧数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void RtuServer_RtuFrameArrived(object sender, RtuFrameArgs e)
        {
            CommServer.LinkMessage += "\n" + DateTime.Now.ToLongTimeString() + "  接收RTU帧:\n";
            CommServer.LinkMessage += ByteToString(e.Frame.Frame, 0, e.Frame.Frame.Length);
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
                return new PlatformModelServer();
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
                strBuild.AppendFormat("{0:X3} ", data[i]);
            }
           
            return strBuild.ToString();
        }



    }
}
