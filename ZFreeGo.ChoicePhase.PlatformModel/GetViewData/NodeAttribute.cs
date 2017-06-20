using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer;

namespace ZFreeGo.ChoicePhase.PlatformModel.GetViewData
{
    /// <summary>
    /// 节点属性集合
    /// </summary>
    public class NodeAttribute
    {


        /// <summary>
        /// MAC ID列表
        /// </summary>
        static private byte[]  _macID = new byte[] { 0x0D, 0x10, 0x12, 0x14,0x02,};
 

          /// <summary>
        /// 同步控制器
        /// </summary>
        public const byte IndexSynController = 0;
        /// <summary>
        /// A相
        /// </summary>
        public const byte IndexPhaseA = 1;
        /// <summary>
        /// A相
        /// </summary>
        public const byte IndexPhaseB = 2;

        /// <summary>
        /// C相
        /// </summary>
        public const byte IndexPhaseC = 3;

        /// <summary>
        /// 主站索引
        /// </summary>
        public const byte IndexMainStation = 4;
        /// <summary>
        /// MAC ID列表
        /// </summary>
        static public byte[] MacID
        {
            get
            {
                return _macID;
            }
        }

        /// <summary>
        /// 主站 MAC
        /// </summary>
        static public byte MacMainStation
        {
            get
            {
                return _macID[IndexMainStation];
            }
        }

        /// <summary>
        /// 同步控制器MAC
        /// </summary>
        static public byte MacSynController
        {
            get
            {
                return _macID[IndexSynController];
            }
        }
        /// <summary>
        /// A相 MAC
        /// </summary>
        static public byte MacPhaseA
        {
            get
            {
                return _macID[IndexPhaseA];
            }
        }
        /// <summary>
        /// B相 MAC
        /// </summary>
        static public byte MacPhaseB
        {
            get
            {
                return _macID[IndexPhaseB];
            }
        }
        /// <summary>
        /// C相 MAC
        /// </summary>
        static public byte MacPhaseC
        {
            get
            {
                return _macID[IndexPhaseC];
            }
        }

        /// <summary>
        /// 合闸时间
        /// </summary>
        private static byte[] _closeTime = new byte[] {0, 50, 50, 50 };

        /// <summary>
        /// 合闸时间,索引1开始为A,B,C
        /// </summary>
        public static byte[] CloseTime
        {
            get
            {
                return _closeTime;
            }
        }

        /// <summary>
        /// 分闸时间
        /// </summary>
        private static byte[] _openTime = new byte[] {0, 40, 40, 40 };

        /// <summary>
        /// 合闸时间,索引1开始为A,B,C
        /// </summary>
        public static byte[] OpenTime
        {
            get
            {
                return _openTime;
            }
        }


        private static int _closeActionOverTime = 10000;

        /// <summary>
        /// 合闸操作超时时间ms
        /// </summary>
        public static int CloseActionOverTime
        {
            get
            {
                return _closeActionOverTime;
            }
        }



        private static int _openActionOverTime = 10000;
        /// <summary>
        /// 合闸操作超时时间ms
        /// </summary>
        public static int OpenActionOverTime
        {
            get
            {
                return _openActionOverTime;
            }
        }

        private static int _synCloseActionOverTime = 10000;

        /// <summary>
        /// 同步合闸操作超时时间
        /// </summary>
        public static int SynCloseActionOverTime
        {
            get
            {
                return _synCloseActionOverTime;
            }
        }


        private static string _passWordII = "12345";
            
        /// <summary>
        /// 二级执行密码
        /// </summary>
        public static  string  PasswordII
        {
            get
            {
                return _passWordII;
            }
        }



        /// <summary>
        /// 获取超时时间
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static int GetOverTime(CommandIdentify cmd)
        {
            switch (cmd)
            {
                case CommandIdentify.ReadyClose:
                    {
                        return CloseActionOverTime;
                    }
                case CommandIdentify.ReadyOpen:
                    {
                        return OpenActionOverTime;
                    }
                case CommandIdentify.SyncReadyClose:
                    {
                        return SynCloseActionOverTime;
                    }
                default:
                    {
                        throw new Exception("为实现的超时选相");
                    }
            }

        }

    }


  



}
