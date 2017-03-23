using System;
using System.Threading;

using System.Diagnostics;
using System.Windows;


namespace ZFreeGo.ChoicePhase.ControlCenter
{
    public partial class MainWindow
    {
        private Thread loopCallThread; //定义循环调用函数线程
        private int loopCount = 0;
        private int loopMax = 0;
        private static float loopTime = 1000;
        private static float loopTimeB = 1000;
        //delegate void updateListViewCallback(object, OperateEventArgs);
        System.Timers.Timer loopTimer = null;
        private bool loopRunState = false;
        //private updateListViewCallback callBack;

        //Action updateLoopEndRecord;
        private Action<object, RoutedEventArgs> callBack;

        void NewloopTimer(Action<object, RoutedEventArgs> callbackAction, float time, float timeB, int cn)
        {
            try
            {
              //  updateRecord = new  Action<string, object>(SendEventMessage);

               
                if (loopRunState) //判断是否正在运行
                {
                    return;
                }
                openAndColseState = true;
                callBack = callbackAction;

                if (cn > 0)
                {
                    loopMax = cn;
                }
                else
                {
                    throw new Exception("循环次数应该不小于 1");
                }
                if (time >= 0.1)
                {
                    loopTime = time * 1000;//转化为ms
                }
                else
                {
                    throw new Exception("循环时间间隔应该不小于 0.2s");
                }
                if (timeB > 0.09)
                {
                    loopTimeB = timeB * 1000;//转化为ms
                }
                else
                {
                    throw new Exception("循环时间间隔应该不小于B 0.2s");
                }
               
                loopCallThread = new Thread(new ThreadStart(LoopCall));
                loopCallThread.Priority = ThreadPriority.Normal;
                loopCallThread.IsBackground = true;
                loopCallThread.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("::USB循环命令:: " + ex.Message);
            }
        }
        private void LoopCall()
        {
            //if (null != loopTimer)
            //{
           
            loopTimer = new System.Timers.Timer(loopTimeB);
            // }
            loopTimer.AutoReset = false; //不自动跟新
            loopTimer.Elapsed += TimerCall;
            loopTimer.Start();
            loopRunState = true;
            //Thread.Sleep(10000);
            //loopTimer.Stop();
            //loopTimer.Dispose();


            loopCount = 0;
           
        }
        
       // Action<string, object> updateRecord;
       
        //Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
        
        private void TimerCall(object sender, System.Timers.ElapsedEventArgs e)
        {

            //
            try
            {
            
            if (loopCount++ < loopMax)
            {
               
                
                callBack(this, null);
                UpdateHistoryRecord(string.Format("共 {0} 次,当前为第 {1} ", loopMax, loopCount));
               // currentDispatcher.Invoke(callBack,this,null);
                //更新状态

                loopTimer.Stop();
                loopTimer.Dispose();
                if (loopCount % 2 == 0) //偶数
                {
                    loopTimer = new System.Timers.Timer(loopTimeB); //重新设置间隔时间
                }
                else //奇数
                {
                    loopTimer = new System.Timers.Timer(loopTime); //重新设置间隔时间
                }
                
                // }
                loopTimer.AutoReset = false; 
                loopTimer.Elapsed += TimerCall;
                loopTimer.Start();
                loopRunState = true;
            }
            else
            {
                loopTimer.Stop();
                loopTimer.Dispose();
               

                loopRunState = false;
                loopCallThread.Abort();
                loopRunState = false;

                Action callUp = () => { RunloopControl.Content = "开始循环"; };
                Dispatcher.Invoke(callUp);
            }
            }
            catch (System.Exception ex)
            {
                Trace.WriteLine("::循环定时器::" + ex.Message);
            }
        }
        private bool openAndColseState = true;
        private void CMDOpenAndCloseBreak(object sender, RoutedEventArgs e)
        {
            if (openAndColseState)
            {
                openAndColseState = false;
                //增加同步
               //SendOrderToYongci(YongciFunEnum.LED4_OFF, "合分闸指示切换");
               //Thread.Sleep(100);
               SendOrderToYongci(YongciFunEnum.HEZHA, "合闸");
            }
            else
            {
                openAndColseState = true;
                //增加同步
                //SendOrderToYongci(YongciFunEnum.LED4_ON, "合分闸指示切换");
                //Thread.Sleep(100);
                SendOrderToYongci(YongciFunEnum.FENZHA  , "分闸");
            }
        }
        private void CMD_O_CO_Break(object sender, RoutedEventArgs e)
        {
            if (openAndColseState)
            {
                openAndColseState = false;
                SendOrderToYongci(YongciFunEnum.O_CO_ACTION, "O-CO操作");
                
                

            }
            else
            {
                openAndColseState = true;

                SendOrderToYongci(YongciFunEnum.HEZHA, "合闸");

            }
        }
        private void CMD_CO_Break(object sender, RoutedEventArgs e)
        {
            if (openAndColseState)
            {
                openAndColseState = false;
                SendOrderToYongci(YongciFunEnum.CO_ACTION, "CO操作");

            }
            else
            {
                openAndColseState = true;

                SendOrderToYongci(YongciFunEnum.CO_ACTION, "CO操作");

            }
        }
        private void CMDEndLoop(object sender)
        {
            if (null == loopCallThread)
            {
                return;
            }
            if (loopRunState)
            {
                loopTimer.Stop();
                loopTimer.Dispose();
                loopCallThread = null;
                //loopCallThread.Abort();
                // listBoxMessage.Items.Add("终止本次循环");
                // listBoxMessage.SetSelected(listBoxMessage.Items.Count - 1, true);
                loopRunState = false;
                Action callUp = () => { RunloopControl.Content = "开始循环"; };
                Dispatcher.Invoke(callUp);
            }

         
        }
       
     
    }
}
