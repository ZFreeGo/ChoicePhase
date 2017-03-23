using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using ZFreeGo.ChoicePhase.Modbus;

namespace ZFreeGo.ChoicePhase.ControlUI.CommCenter
{
    public class SerialControlCenter
    {
        private SerialPort serialPort; //串口实例
        public bool portState = false; //串口状态
        private Thread readThread;
        private byte downComputeAddress = 0xA1;  //下位机地址
        private RTUFrame baseFrame;

        public event EventHandler<RtuFrameArrivedEventArgs> RtuFrameArrived;//事件

        public bool CommState
        {
            get { return portState;}
        }
        public SerialPort SerialPort
        {
            get { return serialPort; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        public SerialControlCenter(byte addr)
        {
            downComputeAddress = addr;
            baseFrame = new RTUFrame(downComputeAddress, (byte)FunEnum.None);
            InitSerialPort();
        }
        public void CloseCenter()
        {
            if (serialPort != null)
            {

                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }

            }
            if (readThread != null)
            {
                readThread.Join(500);
                readThread.Abort();
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
        }
        void InitSerialPort()
        {
            serialPort = new SerialPort();
    
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;

            serialPort.DtrEnable = true;
            serialPort.ReadBufferSize = 2 * 20 * 1024;//默认值4096
        }

        public bool OpenSerialPort()
        {
            try
            {
                serialPort.Open();

                
                readThread = new Thread(SerialRead);
                readThread.Priority = ThreadPriority.AboveNormal;
                readThread.Start();
                portState = true;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         public bool CloseSerialPort()
        {
            portState = false;
            readThread.Join(500);
            readThread.Abort();
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            return true;
        }
        //接收若有超时溢出，会有异常抛出
        private void SerialRead()
        {
            Func<byte> reciveByte = () =>
            {

                byte ch = (byte)serialPort.ReadByte();
                // Dispatcher.BeginInvoke(call, ch);//实时显示
                return ch;
            };
            var reciveTool = new ReciveRtuFrame();
            reciveTool.ReciveAbyte = reciveByte;

            Action<RTUFrame> genEvent = ar =>
            {
                this.RtuFrameArrived(this, new RtuFrameArrivedEventArgs(ar));
            };
            while (portState)
            {
                try
                {
                    try
                    {
                        baseFrame.Address = downComputeAddress;
                        if (reciveTool.JudgeGetByte(baseFrame))
                        {//reciveTool.ReciveFrame
                            //完成一帧接收后，产生帧接收完成事件
                            Dispatcher.CurrentDispatcher.Invoke(genEvent, reciveTool.ReciveFrame);
                            
                        }
                        Thread.Sleep(10);

                    }
                    catch (TimeoutException)
                    {
   
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("串口接收进程::" + ex.Message);
                }
            }
        }

        public void SendMessageToDowncomputer(byte addr, byte funcode)
        {
            if (portState)
            {
                serialPort.RtsEnable = true;
                var frame = new RTUFrame(addr, funcode);
                serialPort.Write(frame.Frame, 0, frame.Frame.Length);
            }
            else
            {
                throw new Exception("未开启串口");
            }
        }
        public void SendMessageToDowncomputer(byte addr, byte funcode, byte[] senddata, byte datalen)
        {
            if (portState)
            {
                serialPort.RtsEnable = true;

                var frame = new RTUFrame(addr, funcode, senddata, datalen);
                serialPort.Write(frame.Frame, 0, frame.Frame.Length);
            }
            else
            {
                throw new Exception("未开启串口");
            }
        }
       
    }
}
