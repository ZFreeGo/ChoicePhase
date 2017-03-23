using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ZFreeGo.ChoicePhase.Modbus;

namespace ZFreeGo.ChoicePhase.ControlCenter
{
    public partial class MainWindow
    {
        private  byte downComputeAddress = 0xA1;  //下位机地址 DSP
       
        private byte mainAddress = 0xf0;
       
        private const byte maxPoint = 100;
        private const byte leftBit = 3;

        //  //用于DSP通讯
        private RTUFrame baseFrame;// = new RTUFrame(downComputeAddress, FunEnum.None);
        /// <summary>
        /// 显示发送字节信息。
        /// </summary>
        /// <param name="send">字节数组</param>
        void ShowSendMessage(byte[] send)
        {
            foreach (var data in send)
            {
                sendTxtBox.Text += string.Format("{0:X2} ", data);
            }

        }
      
        private void sendTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                byte len = Convert.ToByte(dataLenTxt.Text, 16);
                var data = new byte[len];
                var hexStr = GetDecimalByHexString(dataTxt.Text);

                if (hexStr != null)
                {
                    int min = Math.Min(hexStr.Count, len);
                    for (int i = 0; i < min; i++)
                    {
                        data[i] = Convert.ToByte(hexStr[i], 16);
                    }
                }

                serialControlCenter.SendMessageToDowncomputer(Convert.ToByte(deviceAddrTxt.Text, 16), Convert.ToByte(funCodeTxt.Text, 16),
                                             data, len);
                //sendFrame = new RTUFrame(Convert.ToByte(deviceAddrTxt.Text, 16), Convert.ToByte(funCodeTxt.Text, 16),
                //                         data, len);
                //serialPort.Write(sendFrame.Frame, 0, sendFrame.Frame.Length);
                //ShowSendMessage(sendFrame.Frame);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "发送数据");
            }

        }
        
        private void downMcuSelected_DropDownClosed(object sender, EventArgs e)
        {
            downComputeAddress = (byte)(0xA1 + downMcuSelected.SelectedIndex);
        }
        private SampleDataBase GetObjectByIndex(SampDataType sampData, string name)
        {
            try
            {
                switch (name)
                {
                    case "主回路电压A":
                        {
                            return sampData.RawMainVolatageA;
                        }
                    case "主回路电压B":
                        {
                            return sampData.RawMainVolatageB;
                           
                        }
                    case "主回路电压C":    //0xA3
                        {
                            return sampData.RawMainVolatageC;
                            
                        }
                    case "主回路电流A":    //0xA4
                        {
                            return sampData.RawMainCurrentA;
                            
                        }
                    case "主回路电流B":    //0xA5
                        {
                            return sampData.RawMainCurrentB;
                            
                        }
                    case "主回路电流C":    //0xA6
                        {
                            return sampData.RawMainCurrentC;
                            
                        }
                    case "断口电压A":     //0xA7
                        {
                            return sampData.RawMainDuankouVolatageA;
                            
                        }
                    case "断口电压B":     //0xA8
                        {
                            return sampData.RawMainDuankouVolatageB;
                            
                        }
                    case "断口电压C":     //0xA9
                        {
                            return sampData.RawMainDuankouVolatageC;
                            
                        }
                    case "线圈电流":      //0xAA
                        {
                            return sampData.RawXianquanCurrent;
                           
                        }
                    case "加速度":       //0xAB
                        {
                            return sampData.RawJiasuduA;
                           
                        }
                    default:
                        {
                            return null;
                        }
                }
             
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "由名称查找字符");
                return null;
            }
        }
        private SampleDataBase GetObjectByAddress(SampDataType sampData, byte addres)
        {
            try
            {
                switch (addres)
                {
                    case 0xA1:    //0xA6
                        {
                            return sampData.RawXianquanCurrent;

                        }
                    case 0xA2:     //0xA7
                        {
                            return sampData.RawMainDuankouVolatageA;

                        }
                    case 0xA3:     //0xA8
                        {
                            return sampData.RawMainDuankouVolatageB;

                        }
                    case 0xA4:     //0xA9
                        {
                            return sampData.RawMainDuankouVolatageC;

                        }
                  
                    case 0xA5:    //0xA3
                        {
                            return sampData.RawMainCurrentA;

                        }
                    case 0xA6:    //0xA4
                        {
                            return sampData.RawMainCurrentB;

                        }
                    case 0xA7:   //0xA5
                        {
                            return sampData.RawMainCurrentC;

                        }
                  
                  
                    default:
                        {
                            return null;
                        }
                }
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "由地址查找字符");
                return null;
            }
        }

        private void Led1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is RadioButton)
                {
                    var radio = e.OriginalSource as RadioButton;
                    FunEnum fun = FunEnum.None;
                    switch (radio.Name)
                    {
                        case "Led1On":
                            {
                                fun = FunEnum.LED1_ON;
                                break;
                            }
                        case "Led1Off":
                            {
                                fun = FunEnum.LED1_OFF;
                                break;
                            }
                        case "Led1Toggle":
                            {
                                fun = FunEnum.LED1_TOGLE;
                                break;
                            }
                        default:
                            {
                                fun = FunEnum.None;
                                break;
                            }
                    }
                    if (fun != FunEnum.None)
                    {
                        //添加发送命令指令
                        NowAck = 0;
                        serialControlCenter.SendMessageToDowncomputer(downComputeAddress, (byte)fun);
                        var t1 = new System.Threading.Timer(
 OverTimeDectTimer, (byte)fun, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(-1));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LED1:" + ex.Message);
            }
        }

        private void Led2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is RadioButton)
                {
                    var radio = e.OriginalSource as RadioButton;
                    FunEnum fun = FunEnum.None;
                    switch (radio.Name)
                    {
                        case "Led2On":
                            {
                                fun = FunEnum.LED2_ON;
                                break;
                            }
                        case "Led2Off":
                            {
                                fun = FunEnum.LED2_OFF;
                                break;
                            }
                        case "Led2Toggle":
                            {
                                fun = FunEnum.LED2_TOGLE;
                                break;
                            }
                        default:
                            {
                                fun = FunEnum.None;
                                break;
                            }
                    }
                    if (fun != FunEnum.None)
                    {
                        //添加发送命令指令                       
                        serialControlCenter.SendMessageToDowncomputer(downComputeAddress, (byte)fun);                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LED2:" + ex.Message);
            }
        }

        private void Led3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is RadioButton)
                {
                    var radio = e.OriginalSource as RadioButton;
                    FunEnum fun = FunEnum.None;
                    switch (radio.Name)
                    {
                        case "Led3On":
                            {
                                fun = FunEnum.LED3_ON;
                                break;
                            }
                        case "Led3Off":
                            {
                                fun = FunEnum.LED3_OFF;
                                break;
                            }
                        case "Led3Toggle":
                            {
                                fun = FunEnum.LED3_TOGLE;
                                break;
                            }
                        default:
                            {
                                fun = FunEnum.None;
                                break;
                            }
                    }
                    if (fun != FunEnum.None)
                    {
                        //添加发送命令指令
                        NowAck = 0;
                        serialControlCenter.SendMessageToDowncomputer(downComputeAddress, (byte)fun);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LED2:" + ex.Message);
            }
        }

        private void Led4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is RadioButton)
                {
                    var radio = e.OriginalSource as RadioButton;
                    FunEnum fun = FunEnum.None;
                    switch (radio.Name)
                    {
                        case "Led4On":
                            {
                                fun = FunEnum.LED4_ON;
                                break;
                            }
                        case "Led4Off":
                            {
                                fun = FunEnum.LED4_OFF;
                                break;
                            }
                        case "Led4Toggle":
                            {
                                fun = FunEnum.LED4_TOGLE;
                                break;
                            }
                        default:
                            {
                                fun = FunEnum.None;
                                break;
                            }
                    }
                    if (fun != FunEnum.None)
                    {
                        //添加发送命令指令
                        serialControlCenter.SendMessageToDowncomputer(downComputeAddress, (byte)fun);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LED2:" + ex.Message);
            }
        }
       
       
       
        private void SendAllFlag(FunEnum fe)
        {
            try
            {
                if (serialControlCenter.portState)
                {
                    for (byte addr = 0xA1; addr <= 0xA7; addr++)
                    {
                        serialControlCenter.SendMessageToDowncomputer(addr, (byte)fe);
                        Thread.Sleep(10);
                    }
                }
                else
                {
                    throw new Exception("未设置串口");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "发送所有状态标识");
            }
        }
        private void StartSampleDownComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.START_SAMPLE, "开始采样");
        }

        private void StopSampleComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.STOP_SAMPLE, "停止采样");
        }
        private void GetRealFreqDownComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.GET_REAL_QUENCY, "获取频率 ");
        }
        private void GetRealTpTDownComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.GET_DATA_TP, "获取实际周期");
        }
        private void GetRealPhaseTDownComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.GET_DATA_PHASE, "获取计算相位");
        }

        private void GetRealTimeDiffTDownComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.GET_DATA_TIMEDIFF, "获取最终时间差");
        }

        private void GetRealT0TDownComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.GET_DATA_T0, "获取初次时间差值");
        }
        private void resetDownComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.RESET_MCU, "复位所选择的下位机");
        }
        private void GetSampleDownComputer_Click(object sender, RoutedEventArgs e)
        {
            tipReciveTxt.Text = "正在获取...";
            SendOrderToDownCompute(FunEnum.GET_SAMPLE_DATA, "获取采样数据");
        }
        private void GetOvdDownComputer_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.GET_OVD, "获取时间差值");
        }
        private void TestDownComputer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serialControlCenter.portState)
                {
                    //停止采样信号
                    SendOrderToDownCompute(FunEnum.STOP_SAMPLE, "停止采样");
                    Thread.Sleep(50);
                    //作为触发启动信号 ——启动监控采样
                    SendOrderToDownCompute(FunEnum.LED3_OFF, "LED3OFF");
                    Thread.Sleep(10);
                    SendOrderToDownCompute(FunEnum.LED3_ON, "LED3ON");
                    Thread.Sleep(10);
                    
                    //置下位机标志位准备测量过零点
                    SendOrderToDownCompute(FunEnum.GET_OVD, "启动零点侦测");
                    Thread.Sleep(3);
                    SendOrderToDownCompute(FunEnum.START_SAMPLE, "开始采样");
                    Thread.Sleep(1000);

                    SendOrderToDownCompute(FunEnum.GET_REAL_QUENCY, "获取频率 ");
                    Thread.Sleep(10);
                    //SendOrderToDownCompute(FunEnum.GET_DATA_TP, "获取实际周期");
                    //Thread.Sleep(10);

                    SendOrderToDownCompute(FunEnum.START_SAMPLE, "开始采样");

                    //SendOrderToDownCompute(FunEnum.GET_DATA_T0, "获取初次时间差值");
                    //Thread.Sleep(10);
                    //SendOrderToDownCompute(FunEnum.GET_DATA_PHASE, "获取计算相位");
                    //Thread.Sleep(10);
                    //SendOrderToDownCompute(FunEnum.GET_DATA_TIMEDIFF, "获取最终时间差");
                    //Thread.Sleep(10);
                    //GetSampleDownComputer_Click(null, null);
                    //Thread.Sleep(1000);
                    
                    //string path = @"G:\MainProject\选相分合闸\data\" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".txt"; // Default file names
                    //SaveDataToFile(downComputeSampleData[0], path);
                }
                else
                {
                    throw new Exception("未设置串口");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "综合测试");
            }
           
        }
        private void SendOrderToDownCompute(FunEnum fun, string str)
        {
            try
            {
                if (serialControlCenter.portState)
                {
                    serialControlCenter.SendMessageToDowncomputer(downComputeAddress, (byte)fun);
                }
                else
                {
                    throw new Exception("未设置串口");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, str);
            }
        }
        private void setYongciTongbu_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.YONGCI_TONGBU, "设置永磁同步");
        }

        private void resetYongciTongbu_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.YONGCI_TONGBU_RESET, "复位永磁同步");
        }
        private void testYongciWaitTongbu_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToDownCompute(FunEnum.TEST_YONGCI_WAIT, "测试永磁同步指令");
        }
        /// <summary>
        /// 同步合闸控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tongbuHezha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serialControlCenter.portState)
                {
                    var phase = Convert.ToDouble(setHezhaPhase.Text);
                  
                    if (jiaoDuRadio.IsChecked == true)
                    {
                        phase = phase * Math.PI/ 180 ; //角度转化为弧度

                    }
                    phase = phase - Math.PI / 2; //sin 到 cos
                    phase = (phase % (2 * Math.PI)); //周期函数转换
                    
                    if (phase < 0)
                    {
                        phase += Math.PI * 2;
                    }
                    byte[] timedata = new byte[2];
                    var phw = (UInt16)(Math.Round(phase * 10000)); //65536
                    timedata[0] = (byte)(phw % 256);
                    timedata[1] = (byte)(phw / 256);
                    serialControlCenter.SendMessageToDowncomputer(downComputeAddress, (byte)FunEnum.SET_HEZHA_PHASE, timedata, 2);
                  
                }
                else
                {
                    throw new Exception("未设置串口");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"设置同步合闸相角");
            }
        }
        private void setRealHezhaTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serialControlCenter.portState)
                {
                    var time = Convert.ToUInt16(realHezhaTime.Text);
                    if (time > 60000)
                    {
                        throw new ArgumentOutOfRangeException("合闸时间应不大于60000us");
                    }
                    byte[] timedata = new byte[2];
                    timedata[0] = (byte)(time % 256);
                    timedata[1] = (byte)(time / 256);
                    serialControlCenter.SendMessageToDowncomputer(downComputeAddress, (byte)FunEnum.SET_HEZHA_TIME, timedata, 2);

                }
                else
                {
                    throw new Exception("未设置串口");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "设置试试合闸时间");
            }
        }
     
    }
}
