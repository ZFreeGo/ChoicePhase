using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;

using System;

using System.Threading;
using System.Net;
using ZFreeGo.Monitor.DASModel.GetViewData;
using ZFreeGo.ChoicePhase.PlatformModel;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;


namespace ZFreeGo.ChoicePhase.ControlPlatform.ViewModel
{
   
    public class CommunicationViewModel : ViewModelBase
    {
        private PlatformModelServer modelServer;

        private CommunicationServer commServer;



        /// <summary>
        /// Initializes a new instance of the DataGridPageViewModel class.
        /// </summary>
        public CommunicationViewModel()
        {

            modelServer = PlatformModelServer.GetServer();

            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);

            
         
            ClearText = new RelayCommand<string>(ExecuteClearText);
            ToEnd = new RelayCommand<string>(ExecuteToEnd);

            SerialCommand = new RelayCommand<string>(ExecuteSerialCommand);
            serialPortParameter = new SerialPortParameterItem();
            commServer = new CommunicationServer();
            RaisePropertyChanged("Baud");
            RaisePropertyChanged("DataBit");
            RaisePropertyChanged("ParityBit");
            RaisePropertyChanged("StopBit");
            RaisePropertyChanged("CommonPort");
        }

        /// <summary>
        /// 服务数据
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteDASModelServer(PlatformModelServer obj)
        {
            if (obj != null)
            {
                modelServer = obj;
                serialPortParameter = obj.CommServer.SerialPortParameter;
                commServer = obj.CommServer;
                commServer.PropertyChanged += ServerInformation_PropertyChanged;

           
                RaisePropertyChanged("Baud");
                RaisePropertyChanged("DataBit");
                RaisePropertyChanged("ParityBit");
                RaisePropertyChanged("StopBit");
                RaisePropertyChanged("CommonPort");

            }         
        }

        /// <summary>
        /// 服务信息到来
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServerInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }


       

        #region 串口数据处理

        private int selectedIndexCommonPort;

        public int SelectedIndexCommonPort
        {
            get
            {
                return selectedIndexCommonPort;
            }
            set
            {
                selectedIndexCommonPort = value;
                RaisePropertyChanged("SelectedIndexCommonPort");
            }
        }

        private int selectedIndexBaud;
        public int SelectedIndexBaud
        {
            get
            {
                return selectedIndexBaud;
            }
            set
            {
                selectedIndexBaud = value;
                RaisePropertyChanged("SelectedIndexBaud");
            }
        }

        private int selectedIndexDataBit =3;

        public int SelectedIndexDataBit
        {
            get
            {
                return selectedIndexDataBit;
            }
            set
            {
                selectedIndexDataBit = value;
                RaisePropertyChanged("SelectedIndexDataBit");
            }
        }


        private int selectedIndexStopBit;
        public int SelectedIndexStopBit
        {
            get
            {
                return selectedIndexStopBit;
            }
            set
            {
                selectedIndexStopBit = value;
                RaisePropertyChanged("SelectedIndexStopBit");
            }
        }

        private int selectedIndexParity;

        public int SelectedIndexParity
        {
            get
            {
                return selectedIndexParity;
            }
            set
            {
                selectedIndexParity = value;
                RaisePropertyChanged("SelectedIndexParity");
            }
        }

        private SerialPortParameterItem serialPortParameter;
        /// <summary>
        /// 波特率
        /// </summary>
        public ObservableCollection<SerialPortParamer<int>> Baud
        {
            get
            {
                return serialPortParameter.Baud;
            }
           
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public ObservableCollection<SerialPortParamer<int>> DataBit
        {
            get
            {
                return serialPortParameter.DataBit;
            }
        }

        /// <summary>
        /// 校验位
        /// </summary>
        public ObservableCollection<SerialPortParamer<System.IO.Ports.Parity>> ParityBit
        {
            get
            {
                return serialPortParameter.ParityBit;
            }
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public ObservableCollection<SerialPortParamer<System.IO.Ports.StopBits>> StopBit
        {
            get
            {
                return serialPortParameter.StopBit;
            }
        }
        /// <summary>
        /// 串口号
        /// </summary>
        public ObservableCollection<SerialPortParamer<String>> CommonPort
        {
            get
            {
                return serialPortParameter.CommonPort;
            }
        }

        public RelayCommand<string> SerialCommand { get; private set; }

        public void ExecuteSerialCommand(string arg)
        {
            try
            {
                switch (arg)
                {
                    case "OpeanSerial":
                        {
                            break;
                        }
                    case "CloseSerial":
                        {
                            break;
                        }
                    case "Command1":
                        {
                            break;
                        }
                    case "Command2":
                        {
                            break;
                        }
                    default:
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

        /// <summary>
        /// 连接信息
        /// </summary>
        public string LinkMessage
        {
            get
            {
                InitserverData();
                return commServer.LinkMessage;
            }
            set
            {
                commServer.LinkMessage = value;
                RaisePropertyChanged("LinkMessage");

            }
        }

        private void InitserverData()
        {
            
        }

        #region 加载数据命令：LoadDataCommand
        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand LoadDataCommand { get; private set; }

        //加载用户数据
        void ExecuteLoadDataCommand()
        {
           // var get = new GetViewData();
            
        }
        #endregion


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
        
        #region ToEnd
        public RelayCommand<string> ToEnd { get; private set; }

        void ExecuteToEnd(string name)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }
        #endregion
       
       
    }
}