using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ZFreeGo.ChoicePhase.PlatformModel.Helper
{
    /// <summary>
    /// 超时定时器
    /// </summary>
    public class OverTimeTimer
    {
        private Timer timer;
        private Action delegateAction;

        private int overTime;
        /// <summary>
        /// 超时处理
        /// </summary>
        /// <param name="overTime">超时时间ms</param>
        /// <param name="action">超时后的动作</param>
        public OverTimeTimer(int overT, Action action)
        {
            this.overTime = overT;
            delegateAction = action;
            timer = new Timer(overTime);

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            timer.Stop();
            timer.Close();
            delegateAction();
        }
        /// <summary>
        /// 复位定时器重新开始计时
        /// </summary>
        public void ReStartTimer()
        {
            //先停止以前定时器
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Stop();
                timer.Close();
            }

            timer = new Timer(overTime);
            timer.AutoReset = false;
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }


    }
}
