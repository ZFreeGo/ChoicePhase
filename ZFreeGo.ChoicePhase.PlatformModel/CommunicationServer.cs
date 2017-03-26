using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.PlatformModel.Communication;
using ZFreeGo.Monitor.DASModel.GetViewData;

namespace ZFreeGo.ChoicePhase.PlatformModel
{
    public class CommunicationServer : ObservableObject
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

        private string linkMessage;

        /// <summary>
        /// 连接信息
        /// </summary>
        public string LinkMessage
        {
            get
            {
                return linkMessage;
            }
            set
            {
                linkMessage = value;
                RaisePropertyChanged("LinkMessage");

            }
        }

        /// <summary>
        /// 初始化通讯服务
        /// </summary>
        public CommunicationServer()
        {
            _commonServer = new SerialPortServer();
          
            SerialPortParameter = new SerialPortParameterItem();
        }
    }
}
