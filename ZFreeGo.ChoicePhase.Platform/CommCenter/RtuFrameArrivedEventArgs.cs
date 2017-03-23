using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.Modbus;

namespace ZFreeGo.ChoicePhase.ControlUI.CommCenter
{

    public class RtuFrameArrivedEventArgs : EventArgs
    {
        public RtuFrameArrivedEventArgs(RTUFrame frame)
        {
            DataFrame = frame;
        }
        public RTUFrame DataFrame;
    }
}
