using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.PlatformModel.LogicData
{
    /// <summary>
    /// 命令ID
    /// </summary>
    enum CommandIdentify : byte
    {
        /// <summary>
        /// 合闸预制
        /// </summary>
        ReadyClose = 0,
        /// <summary>
        /// 合闸执行
        /// </summary>
        CloseAction = 1,
        /// <summary>
        /// 分闸预制
        /// </summary>
        ReadyOpen = 2,
        /// <summary>
        /// 分闸执行
        /// </summary>
        OpenAction = 3,

        /// <summary>
        /// 同步合闸预制
        /// </summary>
        SyncReadyClose = 4,
        /// <summary>
        /// 同步分闸预制
        /// </summary>
        SyncReadyAction = 5,

        /// <summary>
        /// 主站参数设置
        /// </summary>
        MasterParameterSet = 0x10,

        /// <summary>
        /// 主站参数读取
        /// </summary>
        MasterParameterRead = 0x12,


        /// <summary>
        /// 主站参数读取
        /// </summary>
        ErrorACK = 0x14,


        /// <summary>
        /// 子站状态改变信息上传
        /// </summary>
        SubstationStatuesChange = 0x20,
    }
}
