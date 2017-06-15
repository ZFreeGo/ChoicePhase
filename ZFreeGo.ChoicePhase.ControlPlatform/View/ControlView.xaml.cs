using System.Windows;
using System.Windows.Controls;

namespace ZFreeGo.ChoicePhase.ControlPlatform.View
{
    /// <summary>
    /// Description for ControlView.
    /// </summary>
    public partial class ControlView : Page
    {
        
        /// <summary>
        /// Initializes a new instance of the ControlView class.
        /// </summary>
        public ControlView()
        {
            InitializeComponent();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<string>(this, "ControlViewClrPassword", ClrPassword);
           
        }

        private void ClrPassword(string obj)
        {
            passWordBox.Clear();
        }

        private void passWordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var str = passWordBox.Password;
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>(str, "ControlViewPassword");
        }

        
    }
}