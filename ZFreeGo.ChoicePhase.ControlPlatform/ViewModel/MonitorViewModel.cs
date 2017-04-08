using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using ZFreeGo.Monitor.DASModel.GetViewData;

using System;

using System.Collections.Generic;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;
using ZFreeGo.ChoicePhase.PlatformModel;
using ZFreeGo.ChoicePhase.Modbus;

namespace ZFreeGo.ChoicePhase.ControlPlatform.ViewModel
{

    public class MonitorViewModel : ViewModelBase
    {

        private PlatformModelServer modelServer;
        private readonly byte _downAddress;
        private readonly byte _triansFunction = 1;
        /// <summary>
        /// Initializes a new instance of the DataGridPageViewModel class.
        /// </summary>
        public MonitorViewModel()
        {
            
            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);

            UpdateOperate = new RelayCommand<string>(ExecuteUpdateOperate);
            DataGridMenumSelected = new RelayCommand<string>(ExecuteDataGridMenumSelected);
            modelServer = PlatformModelServer.GetServer();
            _downAddress = modelServer.CommServer.DownAddress;
        }


      
        /************** 属性 **************/
        private ObservableCollection<AttributeItem> _userData;
        /// <summary>
        /// 用户信息数据
        /// </summary>
        public ObservableCollection<AttributeItem> UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                RaisePropertyChanged("UserData");
            }
        }

        #region 加载数据命令：LoadDataCommand
        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand LoadDataCommand { get; private set; }

        //加载用户数据
        void ExecuteLoadDataCommand()
        {
            
            UserData = modelServer.MonitorData.ReadYongciMonitorAttribute(false);
            
        }
        #endregion

        #region 值选择，下载,读取
        private byte _macAddress = 0x10;

        public string MacAddress
        {
            get
            {
                return _macAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out  _macAddress);
                RaisePropertyChanged("MacAddress");
            }
        }
        private byte _startAddress = 0x41;

        public string StartAddress
        {
            get
            {
                return _startAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out  _startAddress);
                RaisePropertyChanged("StartAddress");
            }
        }
        private byte _endAddress = 0x70;

        public string EndAddress
        {
            get
            {
                return _endAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out  _endAddress);
                RaisePropertyChanged("EndAddress");
            }
        }
        /// <summary>
        /// 定值功能
        /// </summary>
        public RelayCommand<string> UpdateOperate { get; private set; }


        void ExecuteUpdateOperate(string str)
        {
            try
            {
                var command = new byte[] { _macAddress, 0, 0x12, _startAddress, _endAddress };
                //此处发送控制命令
                var frame = new RTUFrame(_downAddress, _triansFunction, command, (byte)command.Length);
                modelServer.RtuServer.SendFrame(frame);                
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }


       
        #endregion

        #region 表格操作
        private bool fixCheck = false;
        /// <summary>
        /// 检测使能
        /// </summary>
        public bool FixCheck
        {
            get
            {
                return fixCheck;
            }
            set
            {
                fixCheck = value;
                RaisePropertyChanged("FixCheck");
                RaisePropertyChanged("ReadOnly");

            }
        }

        /// <summary>
        /// 检测使能
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return !fixCheck;
            }

        }


        private int selectedIndex = 0;
        /// <summary>
        /// 选择索引
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                RaisePropertyChanged("SelectedIndex");
            }
        }


       

        public RelayCommand<string> DataGridMenumSelected { get; private set; }

        private void ExecuteDataGridMenumSelected(string name)
        {
            if (!FixCheck)
            {
                return;
            }
            switch (name)
            {
                case "Reload":
                    {

                        UserData = modelServer.MonitorData.ReadYongciMonitorAttribute(true);
                        break;
                    }
                case "Save":
                    {
                        modelServer.MonitorData.InsertYongciMonitorAttribute();
                        break;
                    }
                case "AddUp":
                    {
                        if (SelectedIndex > -1)
                        {
                            var item = new AttributeItem(0, "ABC", 1, 1, 1.0, "A");
                            UserData.Insert(SelectedIndex, item);
                        }
                        break;
                    }
                case "AddDown":
                    {
                        if (SelectedIndex > -1)
                        {
                            var item = new AttributeItem(0, "ABC", 1, 1, 1.0, "A");
                            if (SelectedIndex < UserData.Count - 1)
                            {

                                UserData.Insert(SelectedIndex + 1, item);
                            }
                            else
                            {
                                UserData.Add(item);
                            }
                        }
                        break;
                    }
                case "DeleteSelect":
                    {
                        if (SelectedIndex > -1)
                        {
                            //var result = MessageBox.Show("是否删除选中行:" + gridTelesignalisation.SelectedItem.ToString(),
                            //    "确认删除", MessageBoxButton.OKCancel);
                            var result = true;
                            if (result)
                            {
                                UserData.RemoveAt(SelectedIndex);
                            }
                        }
                        break;
                    }
            }
        }
        #endregion
    }


}