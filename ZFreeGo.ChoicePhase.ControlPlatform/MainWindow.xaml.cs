using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Media;
using ZFreeGo.ChoicePhase.ControlPlatform.ViewModel;

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
            Messenger.Default.Register<string>(this, "ShowUserView", ShowUserView);
            Messenger.Default.Register<Exception>(this, "ExceptionMessage", ExceptionMessage);
      
          
           
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        /// <param name="obj"></param>
        private void ExceptionMessage(Exception obj)
        {
            MessageBox.Show(obj.Message, obj.Source);
        }

   

        private void ShowUserView(string obj)
        {
            try
            {
                if (obj != null)
                {

                    switch (obj)
                    {
                        case "SerialPortConfig":
                            {

                              
                                break;
                            }
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ShowUserView");
            }
        }


      

      
    }
}