using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.PlatformModel.Communication;
using ZFreeGo.Monitor.DASModel.GetViewData;

namespace ZFreeGo.ChoicePhase.PlatformModel
{
    public class CommunicationServer
    {
        /// <summary>
        /// 通讯服务
        /// </summary>
        private SerialPortServer _commonServer;

        /// <summary>
        /// 通讯服务
        /// </summary>
        private SerialPortServer CommonServer
        {
            get
            {
                return _commonServer;
            }
        }

        /// <summary>
        /// 串口参数
        /// </summary>
        public SerialPortParameterItem SerialPortParameter
        {
            private set;
            get;
        }

        /// <summary>
        /// 初始化通讯服务
        /// </summary>
        public CommunicationServer()
        {
            _commonServer = new SerialPortServer();
            _commonServer.Open();
            SerialPortParameter = new SerialPortParameterItem();
        }
    }
}
