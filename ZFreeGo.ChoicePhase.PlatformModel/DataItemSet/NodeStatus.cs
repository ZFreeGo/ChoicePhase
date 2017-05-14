using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.DeviceNet.Element;

namespace ZFreeGo.ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 节点状态，永磁控制器，同步控制器
    /// </summary>
    public class NodeStatus : ObservableObject
    {
        /// <summary>
        /// MAC地址
        /// </summary>
        public byte Mac
        {
            get;
            private set;
        }

        /// <summary>
        /// 节点名称
        /// </summary>
        public String Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 同步配置字
        /// </summary>
        public byte SynConfigByte
        {
            get;
            set;
        }
        /// <summary>
        /// 延时时间1 us
        /// </summary>
        public ushort DelayTime1
        {
            get;
            set;
        }
        /// <summary>
        /// 延时时间2  us
        /// </summary>
        public ushort DelayTime2
        {
            get;
            set;
        }
        /// <summary>
        /// 分闸预制状态
        /// </summary>
        public bool ReadyOpenState
        {
            get;
            set;
        }
        /// <summary>
        /// 合闸预制状态
        /// </summary>
        public bool ReadyCloseState
        {
            get;
            set;
        }
        /// <summary>
        /// 同步合闸预制状态
        /// </summary>
        public bool SynReadyCloseState
        {
            get;
            set;
        }
        /// <summary>
        /// 分闸执行状态
        /// </summary>
        public bool ActionOpenState
        {
            get;
            set;
        }
        /// <summary>
        /// 合闸执行状态
        /// </summary>
        public bool ActionCloseState
        {
            get;
            set;
        }
        /// <summary>
        /// 同步合闸执行状态
        /// </summary>
        public bool SynActionCloseState
        {
            get;
            set;
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        public byte[] LastSendData
        {
            get;
            set;
        }

        



        /// <summary>
        /// 比较应答数据是否有效
        /// </summary>
        /// <param name="reciveData"></param>
        /// <returns></returns>
        public bool IsValid(byte[] reciveData)
        {
            if (LastSendData == null)
            {
                return false;
            }
            if (reciveData == null)
            {
                return false;
            }
            if (reciveData.Length != LastSendData.Length)
            {
                return false;
            }
            //比较是否为应答
            if ((LastSendData[0] | 0x80) != reciveData[0] )
            {
                return false;
            }
            for (int i = 1; i < reciveData.Length; i++)
            {
                if (reciveData[i] != LastSendData[i])
                {
                    return false;
                }
            }
            return true;            
        }

        /// <summary>
        /// 复位指示状态
        /// </summary>
        public void ResetState()
        {
           
           
            ReadyOpenState = false;
            ReadyCloseState = false;
           
            ActionOpenState = false;
            ActionCloseState = false;

            SynReadyCloseState = false;
            SynActionCloseState = false;

        }
        /// <summary>
        /// 节点状态
        /// </summary>
        /// <param name="mac">MAC地址</param>
        /// <param name="name">名称</param>
        public NodeStatus(byte mac, string name)
        {
            Mac = mac;
            Name = name;
        }

    }

        
    
}
