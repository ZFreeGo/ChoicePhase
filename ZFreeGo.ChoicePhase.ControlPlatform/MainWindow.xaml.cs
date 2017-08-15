using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZFreeGo.ChoicePhase.ControlPlatform.ViewModel;
using ZFreeGo.ChoicePhase.PlatformModel;

namespace ZFreeGo.ChoicePhase.ControlPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
         


        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            
            //注册MVVMLight消息
          //  Messenger.Default.Register<string>(this, "ShowUserView", ShowUserView);
            Messenger.Default.Register<Exception>(this, "ExceptionMessage", ExceptionMessage);
            Messenger.Default.Register<string>(this, "MessengerSrcollToEnd", SrcollToEnd);                
        }

        public new void Close()
        {
            PlatformModelServer.GetServer().Close();
            base.Close();
        }
        /// <summary>
        /// 异常信息
        /// </summary>
        /// <param name="obj"></param>
        private void ExceptionMessage(Exception obj)
        {
            MessageBox.Show(obj.Message, obj.Source);
        }


      

        /// <summary>
        /// 将文本框下拉到最后
        /// </summary>
        /// <param name="obj"></param>
        private void SrcollToEnd(string obj)
        {

            Action<string> toend = ar =>
            {
                switch (ar)
                {
                    case "txtLinkMessage":
                        {
                            txtLinkMessage.ScrollToEnd();
                            break;
                        }
                    case "txtRawSendMessage":
                        {
                            txtRawSendMessage.ScrollToEnd();
                            break;
                        }
                    case "txtRawReciveMessage":
                        {
                            txtRawReciveMessage.ScrollToEnd();
                            break;
                        }
                    case "txtExceptionMessage":
                        {
                            txtExceptionMessage.ScrollToEnd();
                            break;
                        }
                    case "txtStatusMessage":
                        {
                            txtStatusMessage.ScrollToEnd();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            };
           // Dispatcher.Invoke(toend, obj);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if( sender is TextBox)
            {
                ((TextBox)sender).ScrollToEnd();
            }
        }      
    }
}