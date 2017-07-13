using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.PlatformModel;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;

namespace ZFreeGo.ChoicePhase.ControlPlatform.ViewModel
{
    public class OptionConfigUIViewModel : ViewModelBase
    {
        private PlatformModelServer modelServer;



        private const string Hidden = "Hidden";
        private const string Collapsed = "Collapsed";
        private const string Visible = "Visible";


        public OptionConfigUIViewModel()
        {
            modelServer = PlatformModelServer.GetServer();
            NodeParameter.PropertyChanged += NodeParameter_PropertyChanged;
            SelectedItemCommand = new RelayCommand<string>(ExecuteSelectedItemCommand);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<Tuple<string, string>>(this, "OptionViewPassword", NodeParameter.UpadeOptionViewPassword);
            NodeParameter.Tips = "";
        }



        





        private string _visibleParameter = Collapsed;

        /// <summary>
        /// 参数可见性
        /// </summary>
        public string VisibleParameter
        {
            get
            {
                return _visibleParameter;
            }
            set
            {
                _visibleParameter = value;
                RaisePropertyChanged("VisibleParameter");
            }
        }
        private string _visiblePassword = Collapsed;
        /// <summary>
        /// 修改密码
        /// </summary>
        public string VisiblePassword
        {
            get
            {
                return _visiblePassword;
            }
            set
            {
                _visiblePassword = value;
                RaisePropertyChanged("VisiblePassword");
            }
        }
     



        public RelayCommand<string> SelectedItemCommand { get; private set; }

        private void ExecuteSelectedItemCommand(string opt)
        {
            switch(opt)
            {
                case "SetParameter":
                    {
                        VisibleParameter = Visible;
                        VisiblePassword = Collapsed;
                        break;
                    }
                case "FixPassword":
                    {
                        VisibleParameter = Collapsed;
                        VisiblePassword = Visible;
                        break;
                    }
            }
        }


        /// <summary>
        /// 结点参数设置
        /// </summary>
        public ConfigParameter NodeParameter
        {
            get
            {
                return modelServer.LogicalUI.NodeParameter;
            }
        }

        void NodeParameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EnabitSelect")
            {
                modelServer.StationInformation[NodeAttribute.IndexPhaseA].Enable = NodeParameter.IsEnablePhaseA;

                modelServer.StationInformation[NodeAttribute.IndexPhaseB].Enable = NodeParameter.IsEnablePhaseB;
                modelServer.StationInformation[NodeAttribute.IndexPhaseC].Enable = NodeParameter.IsEnablePhaseC;
                modelServer.StationInformation[NodeAttribute.IndexSynController].Enable
                    = NodeParameter.IsEnableSyncontroller;
            }
        }

      
    }
}
