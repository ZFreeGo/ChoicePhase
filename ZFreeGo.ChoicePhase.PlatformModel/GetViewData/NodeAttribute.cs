using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        static private byte[]  MacID = new byte[] {0x02, 0x0D, 0x10, 0x12, 0x14};
 

        /// <summary>
        /// 主站 MAC
        /// </summary>
        static public byte MacMainStation
        {
            get
            {
                return MacID[0];
            }
        }

        /// <summary>
        /// 同步控制器MAC
        /// </summary>
        static public byte MacSynControllerMac
        {
            get
            {
                return MacID[1];
            }
        }
        /// <summary>
        /// A相 MAC
        /// </summary>
        static public byte MacPhaseA
        {
            get
            {
                return MacID[2];
            }
        }
        /// <summary>
        /// B相 MAC
        /// </summary>
        static public byte MacPhaseB
        {
            get
            {
                return MacID[3];
            }
        }
        /// <summary>
        /// C相 MAC
        /// </summary>
        static public byte MacPhaseC
        {
            get
            {
                return MacID[4];
            }
        }

       


    }
}
