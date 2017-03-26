/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:ZFreeGo.ChoicePhase.ControlPlatform.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;


namespace ZFreeGo.ChoicePhase.ControlPlatform.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<CommunicationViewModel>();
            SimpleIoc.Default.Register<SetpointViewModel>();
            SimpleIoc.Default.Register<MonitorViewModel >();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public static MainViewModel main;
        public static MainViewModel Main
        {
            get
            {
                if (main == null)
                {
                    main = ServiceLocator.Current.GetInstance<MainViewModel>();
                }
                return main;

            }
        }
        public static CommunicationViewModel communication;
        public static CommunicationViewModel Communication
        {
            get
            {
                if (communication == null)
                {
                    communication = ServiceLocator.Current.GetInstance<CommunicationViewModel>();
                }
                return communication;
            }
        }
        public static SetpointViewModel setpoint;
        public static SetpointViewModel Setpoint
        {
            get
            {
                if (setpoint == null)
                {
                    setpoint = ServiceLocator.Current.GetInstance<SetpointViewModel>();
                }
                return setpoint;
            }
        }
        public static MonitorViewModel monitor;
        public static MonitorViewModel Monitor
        {
            get
            {
                if (monitor == null)
                {
                    monitor = ServiceLocator.Current.GetInstance<MonitorViewModel>();
                }
                return monitor;
            }
        }
        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}