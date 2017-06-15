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

        private bool _synCloseReady;

        /// <summary>
        /// 同步合闸预制
        /// </summary>
        public bool SynCloseReady
        {
            get
            {
                return _synCloseReady;
            }
            set
            {
                SynCloseReady = value;
                RaisePropertyChanged("SynCloseReady");
            }
        }

        private bool _synCloseAction;
        /// <summary>
        /// 同步合闸执行
        /// </summary>
        public bool SynCloseAction
        {
            get
            {
                return _synCloseAction;
            }
            set
            {
                SynCloseAction = value;
                RaisePropertyChanged("SynCloseAction");
            }
        }

        private bool _closeReady;
        /// <summary>
        /// 合闸预制
        /// </summary>
        public bool CloseReady
        {
            get
            {
                return _closeReady;
            }
            set
            {
                CloseReady = value;
                RaisePropertyChanged("CloseReady");
            }
        }

        private bool _closeAction;
        /// <summary>
        /// 合闸执行
        /// </summary>
        public bool CloseAction
        {
            get
            {
                return _closeAction;
            }
            set
            {
                CloseAction = value;
                RaisePropertyChanged("CloseAction");
            }
        }

        private bool _openReady;
        /// <summary>
        ///分闸预制
        /// </summary>
        public bool OpenReady
        {
            get
            {
                return _openReady;
            }
            set
            {
                OpenReady = value;
                RaisePropertyChanged("OpenReady");
            }
        }

        private bool _openAction;
        /// <summary>
        /// 分闸执行
        /// </summary>
        public bool OpenAction
        {
            get
            {
                return _openAction;
            }
            set
            {
                OpenAction = value;
                RaisePropertyChanged("OpenAction");
            }
        }
        /// <summary>
        /// 初始化使能控制
        /// </summary>
        public EnableControl()
        {
            _controlEnable = false;
            _switchOperateEnable = false;
            _synCloseReady = false;
            _synCloseAction = false;
            _closeReady = false;
            _closeAction = false;
            _openReady = false;
            _openAction = false;
        }
    }
}
