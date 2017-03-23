using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.ControlCenter
{
    public class SystemStateMessage : INotifyPropertyChanged
    {
      
        public SystemStateMessage()
        {
            commonStateA = "异常";
            
            commonStateB = "异常";
            mainCircleVoltageA = "未知";
            mainVCBStateA = "未知";
            synSwitchStateA = "未知";
            controllerStateA = "未知";
            capVoltageA = "未知";
            colorCommonStateA = "red";
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }



        /// <summary>
        /// 通信信息 状态
        /// </summary> 
       
        /// <summary>
        /// 串口状态 A --此处指与DSP通信的串口
        /// </summary>
        private string commonStateA;
        public string CommonStateA
        {
            get { return commonStateA; }
            set { commonStateA = value; 
                OnPropertyChanged(new PropertyChangedEventArgs("CommonStateA"));
                if (commonStateA.Contains("正常"))
                {
                    this.ColorCommonStateA = "green";
                }
                else
                {
                    this.ColorCommonStateA = "red";
                }
            }
        }
        private string colorCommonStateA;
        public string ColorCommonStateA
        {
            get { return colorCommonStateA; }
            set { colorCommonStateA = value; OnPropertyChanged(new PropertyChangedEventArgs("ColorCommonStateA")); }
        }
        /// <summary>
        /// 通信状态 A--此处指与LABVIEW检测系统通信
        /// </summary>
        private string commonStateB;
        public string CommonStateB
        {
            get { return commonStateB; }
            set { commonStateB = value; OnPropertyChanged(new PropertyChangedEventArgs("CommonStateB")); }
        }

        /// <summary>
        /// 主回路电压--A
        /// </summary>
        private string mainCircleVoltageA;
        public string MainCircleVoltageA
        {
            get { return mainCircleVoltageA; }
            set { mainCircleVoltageA = value; OnPropertyChanged(new PropertyChangedEventArgs("MainCircleVoltageA")); }
        }
        /// <summary>
        /// 主回路开关状态---主回路断路器状态
        /// </summary>
        private string mainVCBStateA;
        public string MainVCBStateA
        {
            get { return mainVCBStateA; }
            set { mainVCBStateA = value; OnPropertyChanged(new PropertyChangedEventArgs("MainVCBStateA")); }
        }

        /// <summary>
        /// 同步开关状态 --合闸 分闸
        /// </summary>
        private string synSwitchStateA;
        public string SynSwitchStateA
        {
            get { return synSwitchStateA; }
            set { synSwitchStateA = value; OnPropertyChanged(new PropertyChangedEventArgs("SynSwitchStateA")); }
        }


        /// <summary>
        /// 控制器状态 controllerState
        /// </summary>
        private string controllerStateA;
        public string ControllerStateA
        {
            get { return controllerStateA; }
            set { controllerStateA = value; OnPropertyChanged(new PropertyChangedEventArgs("ControllerState")); }
        }

        /// <summary>
        /// 电容电压 capVoltageA
        /// </summary>
        private string capVoltageA;
        public string CapVoltageA
        {
            get { return capVoltageA; }
            set { capVoltageA = value; OnPropertyChanged(new PropertyChangedEventArgs("CapVoltageA")); }
        }      
    }
}
