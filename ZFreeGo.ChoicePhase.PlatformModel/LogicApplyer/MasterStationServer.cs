using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.PlatformModel.LogicApplyer
{
    //主站服务——处理子站节点信息
    public class MasterStationServer
    {
        public event EventHandler<ArrtributesEventArgs> ArrtributesArrived;
        public event EventHandler<ExceptionMessage> ExceptionArrived;


        private List<byte> _macList;

        private int _mac;

        public MasterStationServer(List<byte> macList)
        {
            _macList = macList;
        }

        /// <summary>
        /// 站信息处理
        /// </summary>
        /// <param name="reciveData"></param>
        public void StationDeal(byte[] reciveData)
        {
            try
            {
                if (reciveData.Length < 3)
                {
                    throw new ArgumentException("reciveData.Length < 3");
                }
                if (reciveData[1] != 0xAA) //简单返回并不是主动上传
                {
                   // throw new ArgumentException("reciveData[1] != 0xAA");
                    return;
                }

                foreach (var m in _macList)
                {
                    _mac = m;
                    if (reciveData[0] == m)
                    {
                        byte[] serverData = new byte[reciveData.Length - 2];
                        Array.Copy(reciveData, 2, serverData, 0, reciveData.Length - 2);
                        YongciServer(serverData);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionArrived(this, new ExceptionMessage(ex, "MaterStation"));
            }
            
            
        }
        /// <summary>
        /// 永磁控制器Server处理
        /// </summary>
        /// <param name="serverData">服务数据</param>
        private void YongciServer(byte[] serverData)
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
                                ArrtributesArrived(this, new ArrtributesEventArgs(serverData[1],_mac , serverData, 2, serverData.Length - 2));
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
                throw ex;
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
