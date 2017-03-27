using GalaSoft.MvvmLight;

namespace ZFreeGo.ChoicePhase.ControlPlatform.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ControlViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ControlViewModel class.
        /// </summary>
        public ControlViewModel()
        {

        }

        #region 控制

        private bool _checkPhaseA;
        public bool CheckPhaseA
        {
            set
            {
                _checkPhaseA = value;
            }
            get
            {
                return _checkPhaseA;
            }
        }
        private bool _checkPhaseB = false;
        public bool CheckPhaseB
        {
            set
            {
                _checkPhaseB = value;
            }
            get
            {
                return _checkPhaseB;
            }
        }
        private bool _checkPhaseC = false;
        public bool CheckPhaseC
        {
            set
            {
                _checkPhaseC = value;
            }
            get
            {
                return _checkPhaseC;
            }
        }



        #endregion
    }
}