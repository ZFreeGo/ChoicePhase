using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.DeviceNet.Element;

namespace ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer
{
    //主站轮询，从站应答服务
    public class StationPollingService
    {
        public event EventHandler<ArrtributesEventArgs> ArrtributesArrived;
        
        public event EventHandler<MultiFrameEventArgs> MultiFrameArrived;

        /// <summary>
        /// 异常处理委托
        /// </summary>
        private Action<Exception> ExceptionDelegate;

      
        public StationPollingService(Action<Exception> exceptionDelegate)
        {
            ExceptionDelegate = exceptionDelegate;
        }
       

       
        /// <summary>
        /// 控制器Server处理
        /// </summary>
        /// <param name="serverData">服务数据</param>
        public void Server(byte[] serverData, byte mac)
        {
            try
            {
                if (serverData.Length == 0)
                {
                    throw new Exception("serverData.Length == 0");
                }
                byte ackID = serverData[0];
                if ((ackID & 0x80) != 0x80)
                {
                    throw new Exception("不是应答ID|0x80");
                }
                byte id =(byte)(ackID & 0x7F);

                //if (!Enum.IsDefined(Type.GetType("CommandIdentify"), id))
                //{
                //    throw new Exception("ID不在定义列表之内");
                //}
                switch ((CommandIdentify)id)
                {
                    case CommandIdentify.MasterParameterRead:
                        {

                            if (ArrtributesArrived != null)
                            {
                                ArrtributesArrived(this, new ArrtributesEventArgs(serverData[1], mac , serverData, 2, serverData.Length - 2));
                            }
                            break;
                        }
                    case CommandIdentify.MutltiFrame:
                        {
                            if (MultiFrameArrived != null)
                            {
                                MultiFrameArrived(this, new MultiFrameEventArgs(mac, serverData[1], serverData, 2, serverData.Length - 2));
                            }
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
                ExceptionDelegate(ex);
            }
        }
    }

    public class ExceptionMessage :EventArgs
    {
        /// <summary>
        /// 原始异常
        /// </summary>
        public Exception Ex
        {
            get;
            set;
        }
        public string Comment
        {
            get;
            set;
        }

        public ExceptionMessage(Exception ex, string comment)
        {
            Ex =ex;
            Comment = comment;
        }


    }

    /// <summary>
    /// 多帧事件参数
    /// </summary>
    public class MultiFrameEventArgs: EventArgs
    {
        /// <summary>
        /// 属性字节数组
        /// </summary>
        public byte[] ByteData
        {
            get;
            set;
        } 

        /// <summary>
        /// MAC地址
        /// </summary>
        public int MAC
        {
            get;
            set;
        }
        /// <summary>
        /// Index号
        /// </summary>
        public int Index
        {
            get;
            set;

        }
        /// <summary>
        /// 多帧事件参数
        /// </summary>
        /// <param name="mac">MAC地址</param>
        /// <param name="data">数据</param>
        /// <param name="start">开始索引</param>
        /// <param name="len">数据长度</param>
        public MultiFrameEventArgs(int mac, int index,  byte[] data, int start, int len)
        {
            MAC = mac;
            ByteData = new byte[len];
            Array.Copy(data, start, ByteData, 0, len);
            Index = index;
        }

    }

    /// <summary>
    /// 属性事件参数
    /// </summary>
    public class ArrtributesEventArgs : EventArgs
    {
        /// <summary>
        /// 属性字节数组
        /// </summary>
        public byte[] AttributeByte
        {
            get;
            set;
        }

        /// <summary>
        /// ID号
        /// </summary>
        public int ID
        {
            get;
            set;

        }
        public int MAC
        {
            get;
            set;
        }

        public ArrtributesEventArgs(byte id,int mac, byte[] data, int start, int len)
        {
           
            if (data.Length < (start + len))
            {
                throw new ArgumentException("data.Length < end");
            }
            AttributeByte = new byte[len];
            Array.Copy(data, start, AttributeByte, 0, len);
            ID = id;
            MAC = mac;
        }
      
     


    }

}
