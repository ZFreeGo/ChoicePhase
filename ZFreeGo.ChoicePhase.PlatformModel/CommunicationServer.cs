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
        /// 下位机地址
        /// </summary>
        public byte DownAddress
        {
            private set;
            get;
        }
   

        /// <summary>
        /// 通讯服务
        /// </summary>
        private SerialPortServer _commonServer;

        /// <summary>
        /// 通讯服务
        /// </summary>
        public SerialPortServer CommonServer
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

        private string linkMessage = "";

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
                if (linkMessage.Length > 10000)
                {
                    linkMessage = "";
                }

            }
        }
        private string rawReciveMessage = "";

        /// <summary>
        /// 原始接收
        /// </summary>
        public string RawReciveMessage
        {
            get
            {
                return rawReciveMessage;
            }
            set
            {
                rawReciveMessage = value;
                RaisePropertyChanged("RawReciveMessage");
                if (rawReciveMessage.Length > 10000)
                {
                    rawReciveMessage = "";
                }

            }
        }
        private string rawSendMessage = "";

        /// <summary>
        /// 原始发送信息
        /// </summary>
        public string RawSendMessage
        {
            get
            {
                return rawSendMessage;
            }
            set
            {
                rawSendMessage = value;
                RaisePropertyChanged("RawSendMessage");

            }
        }
        /// <summary>
        /// 初始化通讯服务
        /// </summary>
        public CommunicationServer()
        {
            _commonServer = new SerialPortServer();
            
            SerialPortParameter = new SerialPortParameterItem();

            DownAddress = 0xA1;
        }
    }
}
