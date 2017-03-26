using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;
using ZFreeGo.Monitor.DASModel.GetViewData;

namespace ZFreeGo.ChoicePhase.PlatformModel
{


    public class PlatformModelServer
    {
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
        /// 初始化控制平台Model服务
        /// </summary>
        public PlatformModelServer()
        {
            _monitorViewData = new MonitorViewData();
            CommServer = new CommunicationServer();
            
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





    }
}
