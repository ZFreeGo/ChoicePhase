using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows.Controls;
using ZFreeGo.ChoicePhase.PlatformModel;

namespace ZFreeGo.ChoicePhase.ControlPlatform.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private PlatformModelServer modelServer;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            showUri = "view/CommunicationView.xaml";
            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);
            TreeSelectedItemCommand = new RelayCommand<object>(ExecuteTreeSelectedItemCommand);
            ClosingCommand = new RelayCommand(ExecuteClosingCommand);
            ClearText = new RelayCommand<string>(ExecuteClearText);
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
            modelServer.CommServer.PropertyChanged += ServerInformation_PropertyChanged;
            
           
        }

        private void ServerInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
        #endregion

        #region 退出处理
        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand ClosingCommand { get; private set; }

        //加载用户数据
        void ExecuteClosingCommand()
        {
            modelServer.Close();            
           
        }
        #endregion
        private string showUri;

        /// <summary>
        /// 显示Uri
        /// </summary>
        public string ShowUri
        {
            get
            {
                return showUri;
            }
            set
            {
                showUri = value;
                RaisePropertyChanged("ShowUri");
            }
        }
        /// <summary>
        /// 绑定树状控件命令
        /// </summary>
        public RelayCommand<object> TreeSelectedItemCommand { get; private set; }

        /// <summary>
        /// 执行选择菜单命令
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteTreeSelectedItemCommand(object obj)
        {
            try
            {
                if(obj is TreeViewItem)
                {
                    var item = obj as TreeViewItem;
                   // Messenger.Default.Send<string>(item.Name, "ShowUserView"); 

                    switch (item.Name)
                    {
                        case "SerialPortConfig":
                            {

                                ShowUri = "view/CommunicationView.xaml";
                                break;
                            }

                        case "SetpointParameter":
                            {
                                ShowUri = "view/SetpointView.xaml";
                                break;
                            }
                        case "MonitorParameter":
                            {
                                ShowUri = "view/MonitorView.xaml";
                                break;
                            }
                        case "RemoteControl":
                            {
                                ShowUri = "view/ControlView.xaml";
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }


                   
                }
            }
            catch(Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }

        }

        public string LinkMessage
        {
            get
            {

                return modelServer.CommServer.LinkMessage;
            }
            set
            {
                modelServer.CommServer.LinkMessage = value;
                RaisePropertyChanged("LinkMessage");

            }
        }

        #region ClearText
        public RelayCommand<string> ClearText { get; private set; }

        void ExecuteClearText(string name)
        {
            try
            {
                LinkMessage = "";
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }
        #endregion
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}