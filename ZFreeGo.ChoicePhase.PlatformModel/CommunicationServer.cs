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

        ///// <summary>
        ///// 串口参数
        ///// </summary>
        //public SerialPortParameterItem SerialPortParameter
        //{
        //    private set;
        //    get;
        //}

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
                if (linkMessage.Length > 5000)
                {
                    linkMessage = "";
                }

            }
        }
        /// <summary>
        /// 更新发送连接信息
        /// </summary>
        /// <param name="message"></param>
        public void UpadteSendLinkMessage(string message)
        {
            LinkMessage += string.Format("{0} {1:#####}: ", DateTime.Now.ToLongTimeString(), "Send");
            LinkMessage += message + "\n";
        }
        /// <summary>
        /// 更新接收连接信息
        /// </summary>
        /// <param name="message"></param>
        public void UpadteReciveLinkMessage(string message)
        {
            LinkMessage += string.Format("{0} {1:#####}: ", DateTime.Now.ToLongTimeString(), "Recive");
            LinkMessage += message + "\n";
        }



        private string rawReciveMessage = "";

        /// <summary>
        /// 原始接收数据
        /// </summary>
        public string RawReciveMessage
        {
            get
            {
                return rawReciveMessage;
            }
            set
            {
                if (rawReciveMessage.Length > 5000)
                {
                    rawReciveMessage = "";
                }
                rawReciveMessage = value;
                RaisePropertyChanged("RawReciveMessage");
                

            }
        }
        private string rawSendMessage = "";

        /// <summary>
        /// 原始发送数据
        /// </summary>
        public string RawSendMessage
        {
            get
            {
                return rawSendMessage;
            }
            set
            {
                if (rawReciveMessage.Length > 5000)
                {
                    rawReciveMessage = "";
                }
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

                //SerialPortParameter = new SerialPortParameterItem();
              DownAddress = 0xA1;
           
        }
    }
}
