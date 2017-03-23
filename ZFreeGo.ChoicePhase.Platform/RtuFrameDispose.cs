using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ZFreeGo.ChoicePhase.ControlUI.CommCenter;
using ZFreeGo.ChoicePhase.Modbus;

namespace ZFreeGo.ChoicePhase.ControlCenter
{
    public partial class MainWindow
    {
        //发送超时检测定时器
       private void OverTimeDectTimer(object o)
        {
           if (o is byte)
           {
               if ((byte)o != NowAck)
               {
                   UpdateHistoryRecord("无应答：" + ((byte)o).ToString());
                   NowAck = 0;
               }
           }
        }
        private void serialControlCenter_RtuFrameArrived(object sender, RtuFrameArrivedEventArgs e)
        {
            
            try
            {
                var frame = e.DataFrame;
               
                switch ((FunEnum)frame.Function)
                {
                    case FunEnum.ACK:
                        {

                            UpdateHistoryRecord("ACK:" + ((FunEnum)frame.FrameData[0]).ToString());
                            NowAck = frame.FrameData[0];
                            break;
                        }
 
                    case FunEnum.GET_REAL_QUENCY:
                        {
                            double freq = Tool.Byte4ToFloat(frame.FrameData, 1e-5);
                            double tp = 1e6 / freq;
                            UpdateHistoryRecord("频率" + freq.ToString() + " " + string.Format("{0:0.000}", tp));
                            
                            break;
                        }

                    case FunEnum.GET_DATA_TP:
                        {
                            double tp = Tool.Byte4ToFloat(frame.FrameData, 1e-5);

                            UpdateHistoryRecord("tp:"  + tp.ToString());
                            
                            break;
                        }
                    case FunEnum.GET_DATA_PHASE:
                        {
                            double phase = Tool.Byte4ToFloat(frame.FrameData, 1e-5);
                            UpdateHistoryRecord("相位phase:" + phase.ToString());
                         
                            break;
                        }
                    case FunEnum.GET_DATA_T0:
                        {
                            double t0 = Tool.Byte4ToFloat(frame.FrameData, 1e-5);
                            UpdateHistoryRecord("T0" + t0.ToString());
                            
                            break;
                        }
                    case FunEnum.GET_DATA_TIMEDIFF:
                        {
                            double time = Tool.Byte4ToFloat(frame.FrameData, 1e-5);
                            UpdateHistoryRecord("timediff:" + time.ToString());
               
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }


            }
            catch (Exception ex)
            {
                //Action<Exception> call = ar => { MessageBox.Show(ar.Message, "接收帧"); };

                MessageBox.Show(ex.Message, "接收帧");
            }
        }
        private void yongciSerialControlCenter_RtuFrameArrived(object sender, RtuFrameArrivedEventArgs e)
        {

            try
            {
                var frame = e.DataFrame;
                switch ((YongciFunEnum)frame.Function)
                {
                    case YongciFunEnum.ACK:
                        {

                            UpdateHistoryRecord("永磁 ACK:" + ((YongciFunEnum)frame.FrameData[0]).ToString());
                            break;
                        }


                    default:
                        {
                            break;
                        }
                }


            }
            catch (Exception ex)
            {
                //Action<Exception> call = ar => { MessageBox.Show(ar.Message, "接收帧"); };

                MessageBox.Show(ex.Message, "接收帧");
            }
        }
        static int index = 0;
        //CallbackUpdate cu = UpdateHistoryRecord;
        //delegate void CallbackUpdate(string s);
        void UpdateHistoryRecord(string ar)
        {
            Action<string> updateCall = str =>
                {
                    if (historyRecord.Items.Count > 1000)
                    {
                        historyRecord.Items.Clear();
                       
                    }
                    ListViewItem litem = new ListViewItem();
                    litem.Content = index.ToString() + "  " + DateTime.Now.ToLongTimeString() + " " + str;
                    index++;
                    historyRecord.Items.Add(litem);
                    historyRecord.ScrollIntoView(litem);
                };
            
            Dispatcher.BeginInvoke(updateCall, ar);
            
        }
    }
}
