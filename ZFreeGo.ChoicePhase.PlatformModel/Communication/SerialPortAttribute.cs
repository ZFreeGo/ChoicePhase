using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.PlatformModel.Communication
{
    /// <summary>
    /// 串口参数
    /// </summary>
    public class SerialPortAttribute
    {
        /// <summary>
        /// 串口号
        /// </summary>
        public string CommonPort
        {
            get;
            set;
        }


        /// <summary>
        /// 波特率
        /// </summary>

        public int Baud
        {
            get;
            set;
        }
        /// <summary>
        /// 数据位
        /// </summary>

        public int DataBit
        {
            get;
            set;
        }

        /// <summary>
        /// 奇偶校验位
        /// </summary>

        public Parity ParityBit
        {
            get;
            set;
        }
        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBit
        {
            get;
            set;
        }

        /// <summary>
        /// 串口属性初始化
        /// </summary>
        /// <param name="comNum"></param>
        /// <param name="baud"></param>
        /// <param name="databit"></param>
        /// <param name="parityBit"></param>
        /// <param name="stopbit"></param>
        public SerialPortAttribute(int comNum, int baud, int databit, int parityBit, int stopbit)
        {

            string comstr = "COM" + comNum.ToString();
            var ports = SerialPort.GetPortNames();
            bool isExist = false;
            foreach (var m in ports)
            {
                if (comstr == m)
                {
                    isExist = true;
                }
            }
            if (!isExist)
            {
                throw new ArgumentNullException("指定的串口号，不存在");
            }
            CommonPort = comstr;

            if (baud < 0)
            {
                throw new ArgumentNullException("波特率不能为负");
            }
            Baud = baud;

            if (databit < 0)
            {
                throw new ArgumentNullException("波特率不能为负");
            }
            DataBit = databit;

            switch (parityBit)
            {
                case 0:
                    {
                        ParityBit = Parity.None;
                        break;
                    }
                case 1:
                    {
                        ParityBit = Parity.Odd;
                        break;
                    }
                case 2:
                    {
                        ParityBit = Parity.Even;
                        break;
                    }
                case 3:
                    {
                        ParityBit = Parity.Space;
                        break;
                    }
                case 5:
                    {
                        ParityBit = Parity.Mark;
                        break;
                    }
                default:
                    {
                        ParityBit = Parity.None;
                        break;
                    }
            }

            switch (stopbit)
            {

                case 0:
                    {
                        StopBit = StopBits.None;
                        break;
                    }

                case 1:
                    {
                        StopBit = StopBits.One;
                        break;
                    }
                case 2:
                    {
                        StopBit = StopBits.Two;
                        break;
                    }
                case 3:
                    {
                        StopBit = StopBits.OnePointFive;
                        break;
                    }
                default:
                    {
                        StopBit = StopBits.One;
                        break;
                    }

            }


        }

        /// <summary>
        /// 串口属性初始化
        /// </summary>
        /// <param name="comNum"></param>
        /// <param name="baud"></param>
        /// <param name="databit"></param>
        /// <param name="parityBit"></param>
        /// <param name="stopbit"></param>

        public SerialPortAttribute(string comNum, int baud, int databit, Parity parityBit, StopBits stopbit)
        {
            CommonPort = comNum;
            Baud = baud;
            DataBit = databit;
            ParityBit = parityBit;
            StopBit = stopbit;
        }


    }
}
