﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.DeviceNet.Element;

namespace ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer
{
    //主站轮询，从站应答服务，从站状态变换服务
    public class StationPollingService
    {
        public event EventHandler<ArrtributesEventArgs> ArrtributesArrived;
        
        public event EventHandler<MultiFrameEventArgs> MultiFrameArrived;

        public event EventHandler<StatusChangeMessage> ReadyActionArrived;

        public event EventHandler<StatusChangeMessage> SubStationStatusChanged;

        /// <summary>
        /// 异常处理委托
        /// </summary>
        private Action<Exception> ExceptionDelegate;

       // private DeviceNetServer controlNetServer;

        public StationPollingService( Action<Exception> exceptionDelegate)
        {
            ExceptionDelegate = exceptionDelegate;
           // controlNetServer = netServer;
        }
       
        public void SendCommand(byte mac, byte[] command)
        {
           //  var command = new byte[] { (byte)cmd, _circleByte, (byte)_actionTime };
            //此处发送控制命令                     
            //controlNetServer.MasterSendCommand(mac, command, 0, command.Length); 
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

                //错误信息
                if (ackID == (byte)CommandIdentify.ErrorACK)
                {
                    if (serverData.Length < 4)//错误响应，长度不小于4
                    {
                        throw new Exception("错误代码长度小于4");
                    }
                   
                    //是否为定义内
                    if (Enum.IsDefined(typeof(CommandIdentify), serverData[1]))
                   {

                       var des = GetIDDescription((CommandIdentify)serverData[1]);
                       string error1 = "错误代码:" + serverData[2].ToString("X2");
                       string error2 = "附加错误代码:" + serverData[3].ToString("X2");
                       throw new Exception(des + " " + error1 + " " + error2);


                   }
                   else
                   {
                       throw new Exception("未识别ID");
                   }
                    
                }


                if ((ackID & 0x80) != 0x80)
                {
                    throw new Exception(string.Format("不是应答ID|0x80, ID={0:X2}", ackID));
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
                    case CommandIdentify.CloseAction://合闸执行
                    case CommandIdentify.OpenAction: //分闸执行
                    case CommandIdentify.ReadyClose: // 合闸预制
                    case CommandIdentify.ReadyOpen:  //分闸预制                   
                    case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                    case CommandIdentify.SyncOrchestratorReadyClose:  //同步控制器合闸预制
                    case CommandIdentify.SyncOrchestratorCloseAction:  //同步控制合闸执行
                        {

                            if (ReadyActionArrived != null)
                            {
                                ReadyActionArrived(this, new StatusChangeMessage(mac, serverData));
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

        /// <summary>
        /// 从站状态改变报告
        /// </summary>
        /// <param name="serverData"></param>
        /// <param name="mac"></param>
        public void StatusChangeServer(byte[] serverData, byte mac)
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
                if ((CommandIdentify)id == CommandIdentify.SubstationStatuesChange)
                {
                    if (SubStationStatusChanged != null)
                    {
                        SubStationStatusChanged(this, new StatusChangeMessage(mac, serverData));
                    }
                }

                
            }
            catch (Exception ex)
            {
                ExceptionDelegate(ex);
            }
        }
        /// <summary>
        /// 获取ID描述
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>描述词</returns>
        public string GetIDDescription(CommandIdentify id)
        {
            string des = "";
            switch(id)
            {
                case CommandIdentify.CloseAction:
                    {
                        des =  "合闸执行" ;
                        break;
                    }
                case CommandIdentify.MasterParameterRead:
                    {
                        des =  "主站参数读取" ;
                        break;
                    }
                case CommandIdentify.MasterParameterSetOne:
                    {
                        des =  "主站参数设置" ;
                        break;
                    }
                case CommandIdentify.MutltiFrame:
                    {
                        des =  "多帧" ;
                        break;
                    }
                case CommandIdentify.OpenAction:
                    {
                        des =  "分闸执行" ;
                        break;
                    }
                case CommandIdentify.ReadyClose:
                    {
                        des =  "合闸预制" ;
                        break;
                    }
                case CommandIdentify.ReadyOpen:
                    {
                        des =  "分闸预制" ;
                        break;
                    }
                case CommandIdentify.SubstationStatuesChange:
                    {
                        des =  "子站状态上传" ;
                        break;
                    }
                case CommandIdentify.SyncOrchestratorCloseAction:
                    {
                        des =  "同步控制器合闸执行" ;
                        break;
                    }
                case CommandIdentify.SyncOrchestratorReadyClose:
                    {
                        des =  "同步控制器合闸预制" ;
                        break;
                    }
                case CommandIdentify.SyncReadyClose:
                    {
                        des =  "同步合闸预制" ;
                        break;
                    }
                default:
                    {
                        des =  "未识别的ID" ;
                        break;
                    }
            }
            return des + " " + ((byte)id).ToString("X2");
        }

    }

 

    /// <summary>
    /// 节点预制，执行等信息
    /// </summary>
    public class StatusChangeMessage : EventArgs
    {
         /// <summary>
        /// MAC地址
        /// </summary>
        public byte MAC
        {
            get;
            set;
        }

        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// 预制，执行消息
        /// </summary>
        /// <param name="mac">MAC</param>
        /// <param name="serverData">服务数据</param>
        public StatusChangeMessage(byte mac, byte[] serverData)
        {
            MAC = mac;

            Data = new byte[serverData.Length];
            Array.Copy(serverData, Data, Data.Length);
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