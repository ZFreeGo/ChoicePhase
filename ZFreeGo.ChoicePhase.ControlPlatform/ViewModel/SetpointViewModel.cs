using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using ZFreeGo.Monitor.DASModel.GetViewData;

using System;

using System.Collections.Generic;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;
using ZFreeGo.ChoicePhase.PlatformModel;

namespace ZFreeGo.ChoicePhase.ControlPlatform.ViewModel
{

    public class SetpointViewModel : ViewModelBase
    {

        private PlatformModelServer modelServer;

        /// <summary>
        /// Initializes a new instance of the DataGridPageViewModel class.
        /// </summary>
        public SetpointViewModel()
        {
            
            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);

            SetpointOperate = new RelayCommand<string>(ExecuteSetpointOperate);
            DataGridMenumSelected = new RelayCommand<string>(ExecuteDataGridMenumSelected);
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
            modelServer = PlatformModelServer.GetServer();
            UserData = modelServer.MonitorData.ReadYongciAttribute(false);
            
        }
        #endregion

        #region 值选择，下载,读取
        /// <summary>
        /// 定值功能
        /// </summary>
        public RelayCommand<string> SetpointOperate { get; private set; }


        void ExecuteSetpointOperate(string str)
        {
            try
            {
                
                switch(str)
                {
                    case "Read":
                        {
                        
                            break;
                        }
                    case "Update":
                        {
                            
                            break;
                        }
                }

                
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

                        UserData =   modelServer.MonitorData.ReadYongciAttribute(true);
                        break;
                    }
                case "Save":
                    {
                        modelServer.MonitorData.InsertYongciAttribute();
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