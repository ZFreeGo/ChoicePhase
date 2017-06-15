using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 空间使能控制
    /// </summary>
    public class EnableControl : ObservableObject
    {
        /// <summary>
        /// 总控制使能
        /// </summary>
        private bool _controlEnable;
        public  bool ControlEnable
        {
            get
            {
                return _controlEnable;
            }
            set
            {
                _controlEnable = value;
                RaisePropertyChanged("ControlEnable");
            }
        }
        /// <summary>
        /// 开关操作
        /// </summary>
        private bool _switchOperateEnable;
        public bool SwitchOperateEnable
        {
            get
            {
                return _switchOperateEnable;
            }
            set
            {
                _switchOperateEnable = value;
                RaisePropertyChanged("SwitchOperateEnable");
            }
        }
        /// <summary>
        /// 初始化使能控制
        /// </summary>
        public EnableControl()
        {
            _controlEnable = false;
        }
    }
}
