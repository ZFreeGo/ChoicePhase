using GalaSoft.MvvmLight.Messaging;
using System.Windows;
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
        }

        private void ShowUserView(string obj)
        {
            if (obj != null)
            {

                switch (obj)
                {
                    case "Telesignalisation":
                        {

                            break;
                        }
                    case "Telemetering":
                        {

                            break;
                        }
                    case "Telecontrol":
                        {

                            break;
                        }
                    case "SOELog":
                        {

                            break;
                        }
                    case "ProtectSetPoint":
                        {

                            break;
                        }
                    case "SystemParameter":
                        {

                            break;
                        }
                    case "SystemCalibration":
                        {

                            break;
                        }
                    case "Communication":
                        {

                            break;
                        }

                }

            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
           
        }
    }
}