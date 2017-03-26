using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZFreeGo.ChoicePhase.ControlPlatform.Model;
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
        public MainViewModel(IDataService dataService)
        {
            modelServer =  PlatformModelServer.GetServer();

            ShowUserView = new RelayCommand<string>(ExecuteShowUserView);

        }

        /// <summary>
        /// 显示UserView窗口
        /// </summary>
        public RelayCommand<string> ShowUserView { get; private set; }

        //发送显示UserView的消息
        void ExecuteShowUserView(string name)
        {
            Messenger.Default.Send<string>(name, "ShowUserView");
        }


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}