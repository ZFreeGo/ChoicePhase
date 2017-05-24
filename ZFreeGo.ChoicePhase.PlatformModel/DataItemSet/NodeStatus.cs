using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.DeviceNet.Element;
using ZFreeGo.ChoicePhase.PlatformModel.Helper;

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
        /// 更新视图委托
        /// </summary>
        public Action UpdateViewDelegate;


        private StatusLoop[] statusLoopCollect;
        /// <summary>
        /// 合分状态1
        /// </summary>
        public StatusLoop[] StatusLoopCollect
        {
            get
            {
                return statusLoopCollect;
            }
            set
            {
                statusLoopCollect = value;
                RaisePropertyChanged("StatusLoopCollect");
               
            }

        }

        private EnergyStatusLoop[] energyStatusLoopCollect;
        /// <summary>
        /// 回路储能状态1
        /// </summary>
        public EnergyStatusLoop[] EnergyStatusLoopCollect
        {
            get
            {
                return energyStatusLoopCollect;
            }
            set
            {
                energyStatusLoopCollect = value;
                RaisePropertyChanged("EnergyStatusLoopCollect");
            }
        }
        /// <summary>
        /// 超时时间默认为3000 ms
        /// </summary>
        public int OverTimeMs
        {
            get;
            private set;
        }


        /// <summary>
        /// 超时定时器--循环判断是否有接收数据
        /// </summary>
        private OverTimeTimer overTimerCycle;


        /// <summary>
        /// 循环接收状态，超时处理委托
        /// </summary>
        public Action CycleOverTimeDelegate;


        

        /// <summary>
        /// 循环接收状态，超时处理
        /// </summary>
        private void CycleOverTime()
        {
            if (CycleOverTimeDelegate != null)
            {
                CycleOverTimeDelegate();
            }
        }
        /// <summary>
        /// 更新同步状态
        /// </summary>
        /// <param name="statusByte"></param>
        public void UpdateSynStatus(byte[] statusByte)
        {
            //判断长度
            if (statusByte.Length != 6)
            {
                return;
            }
            for (int k = 0; k < 4; k++)
            {
                StatusLoopCollect[k] = (StatusLoop)((statusByte[1] >> (2 * k)) & (0x03));
            }
            for (int k = 0; k < 4; k++)
            {
                EnergyStatusLoopCollect[k] = (EnergyStatusLoop)((statusByte[3] >> (2 * k)) & (0x03));
            }



            if (UpdateViewDelegate != null)
            {
                UpdateViewDelegate();
            }

            overTimerCycle.ReStartTimer();
        }
       
        /// <summary>
        /// 更新开关状态
        /// </summary>
        /// <param name="statusByte"></param>
        public void UpdateSwitchStatus(byte[] statusByte)
        {
            //判断长度
            if (statusByte.Length != 6)
            {
                return;
            }         
            for( int k = 0; k < 4; k++)
            {
                StatusLoopCollect[k] = (StatusLoop)((statusByte[1] >> (2 * k)) & (0x03));
            }
            for (int k = 0; k < 4; k++)
            {
                EnergyStatusLoopCollect[k] = (EnergyStatusLoop)((statusByte[3] >> (2 * k)) & (0x03));
            }



            if (UpdateViewDelegate != null)
            {
                UpdateViewDelegate();
            }

            overTimerCycle.ReStartTimer();
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
            //if (reciveData.Length != LastSendData.Length)
            //{
            //    return false;
            //}
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
        /// <param name="overMs">超时时间ms</param>
        public NodeStatus(byte mac, string name, int overMs)
        {
            Mac = mac;
            Name = name;

            StatusLoopCollect = new StatusLoop[4];
            for (int i = 0; i < StatusLoopCollect.Length; i++ )
            {
                StatusLoopCollect[i] = DataItemSet.StatusLoop.Null;
            }

            EnergyStatusLoopCollect = new EnergyStatusLoop[4];
            for (int i = 0; i < EnergyStatusLoopCollect.Length; i++)
            {
                EnergyStatusLoopCollect[i] = DataItemSet.EnergyStatusLoop.Null;
            }
            OverTimeMs = overMs;
            overTimerCycle = new OverTimeTimer(OverTimeMs, CycleOverTime);

        }

        

    }

    /// <summary>
    /// 回路状态
    /// </summary>
    public enum StatusLoop
    {
        /// <summary>
        /// 空
        /// </summary>
        Null = 0,
        /// <summary>
        /// 合位
        /// </summary>
        Close = 2,
        /// <summary>
        /// 分位
        /// </summary>
        Open = 1,
        /// <summary>
        /// 故障
        /// </summary>
        Error = 3,
    }
    /// <summary>
    /// 储能状态
    /// </summary>
    public enum EnergyStatusLoop
    {
        /// <summary>
        /// 空
        /// </summary>
        Null = 0,

        /// <summary>
        /// 范围内
        Normal = 2,
        /// <summary>
        /// 小于下限
        /// </summary>
        Less = 1,
        /// <summary>
        /// 超过
        /// </summary>
        More = 3,
    }
        
    
}
