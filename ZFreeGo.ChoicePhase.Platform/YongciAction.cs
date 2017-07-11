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
        private byte yongcidownComputeAddress = 0xA2;  //永磁控制器 下位机地址
       

        private void SendOrderToYongci(YongciFunEnum fun, string str)
        {
            try
            {
                if (yongciSerialControlCenter.portState)
                {
                    yongciSerialControlCenter.SendMessageToDowncomputer(yongcidownComputeAddress, (byte)fun);
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
        private void SendOrderToYongci(byte addr, byte funcode, byte[] senddata, byte datalen, string str)
        {
            try
            {
                if (yongciSerialControlCenter.portState)
                {
                    yongciSerialControlCenter.SendMessageToDowncomputer(addr, funcode, senddata, datalen);
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
        private void Ledx_Click(object sender, RoutedEventArgs e)
        {
            //选定的索引

            try
            {
                byte selectedIndex = (byte)(ledSelected.SelectedIndex + 1);
                if (e.OriginalSource is RadioButton)
                {
                    var radio = e.OriginalSource as RadioButton;
                    YongciFunEnum fun = YongciFunEnum.None;
                    switch (radio.Name)
                    {
                        case "LedXOn":
                            {
                                fun = (YongciFunEnum)(0x18 + selectedIndex * 1);
                                break;
                            }
                        case "LedXOff":
                            {
                                fun = (YongciFunEnum)(0x20 + selectedIndex * 1);
                                break;
                            }
                        case "LedXToggle":
                            {
                                fun = (YongciFunEnum)(0x10 + selectedIndex * 1);
                                break;
                            }
                        default:
                            {
                                fun = YongciFunEnum.None;
                                break;
                            }
                    }
                    if (fun != YongciFunEnum.None && ((byte)fun >= 0x11) && ((byte)fun <= 0x28))
                    {

                        //添加发送命令指令
                        SendOrderToYongci(fun, "LEDX控制");

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LEDx:" + ex.Message);
            }
        }
        private void YongciHezha_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToYongci(YongciFunEnum.LED4_OFF, "合分闸指示切换");
            Thread.Sleep(100);
            SendOrderToYongci(YongciFunEnum.HEZHA, "合闸");
           
        }

        private void YongciFenzha_Click(object sender, RoutedEventArgs e)
        {
            
            SendOrderToYongci(YongciFunEnum.LED4_ON, "合分闸指示切换");
            Thread.Sleep(100);
            SendOrderToYongci(YongciFunEnum.FENZHA, "分闸");
           
        }
        private void YongciCO_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToYongci(YongciFunEnum.CO_ACTION, "CO");
        }

        private void YongciO_CO_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToYongci(YongciFunEnum.O_CO_ACTION, "O_CO");
        }
        private void YongciTriggerWait_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToYongci(YongciFunEnum.YONGCI_WAIT_HE_ACTION, "触发等待-合闸");
        }
        //发送合闸时间 时间精度ms 为整数最大255ms
        private void HezhaTimeSet_Click(object sender, RoutedEventArgs e)
        {
            byte time = Convert.ToByte(hezhaTimeTxt.Text);
            byte[] timedata = new byte[2];
            timedata[0] = time;
            SendOrderToYongci(yongcidownComputeAddress, (byte)YongciFunEnum.HEZHA_TIME, timedata, 1, "设定合闸通电时间");
        }

        private void FenzhaTimeSet_Click(object sender, RoutedEventArgs e)
        {
            byte time = Convert.ToByte(fenzhaTimeTxt.Text);
            byte[] timedata = new byte[2];
            timedata[0] = time;
            SendOrderToYongci(yongcidownComputeAddress, (byte)YongciFunEnum.FENZHA_TIME, timedata, 1, "设定分闸通电时间");
        }

        bool loopState = false;
        private void YongciLedToggle_Click(object sender, RoutedEventArgs e)
        {
            SendOrderToYongci(YongciFunEnum.LED1_TOGLE, "LED取反");
        }
         private void RunloopControl_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (false == loopState)
                {

                    int testcount = Convert.ToInt32(TestCount.Text);
                    float testtime = Convert.ToSingle(TestTime.Text);
                    float testtimeB = Convert.ToSingle(TestTimeB.Text);
                    if (testtime < 0.09)
                    {
                        throw new Exception("时间参数应不小于 0.1s");
                    }
                    if (testtimeB < 0.09)
                    {
                        throw new Exception("时间参数应不小于 0.1s");
                    }
                    if (testcount < 1)
                    {
                        throw new Exception("测试次数应大于 1");
                    }

                    if (!yongciSerialControlCenter.portState)
                    {
                        throw new Exception("未设置总线通讯");
                    }
                
                    loopState = true;
                    RunloopControl.Content = "结束循环";
                    openAndColseState = true;//
                    switch (operateLoopSelected.SelectedIndex)
                    {
                        case 0://C-t1-O-t2-
                            {
                                NewloopTimer(CMDOpenAndCloseBreak, testtime, testtimeB, testcount);
                                break;
                            }
                        case 1://O-0.3-CO-t1-C-t2-
                            {
                                NewloopTimer(CMD_O_CO_Break, testtime, testtimeB, testcount);
                                break;
                            }
                        case 2://CO-t1-CO-t2-CO
                            {
                                NewloopTimer(CMD_CO_Break, testtime, testtimeB, testcount);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                   
                    //YongciLedToggle_Click
                }
                else
                {
                    loopState = false;
                    RunloopControl.Content = "开始循环";
                    CMDEndLoop(null);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("下位机循环操作：" + ex.Message);
            }
        }

    }
        
    

}
