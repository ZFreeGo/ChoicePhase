using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;
using ZFreeGo.ChoicePhase.PlatformModel.Helper;

namespace ZFreeGo.Monitor.DASModel.GetViewData
{
    /// <summary>
    /// 串口参数列表
    /// </summary>
    public class SerialPortParameterItem : ObservableObject
    {

        /// <summary>
        /// 波特率
        /// </summary>
        public ObservableCollection<SerialPortParamer<int>> Baud;

        /// <summary>
        /// 数据位
        /// </summary>
        public ObservableCollection<SerialPortParamer<int>> DataBit;

        /// <summary>
        /// 校验位
        /// </summary>
        public ObservableCollection<SerialPortParamer<Parity>> ParityBit;

        /// <summary>
        /// 停止位
        /// </summary>
        public ObservableCollection<SerialPortParamer<StopBits>> StopBit;


        /// <summary>
        /// 串口号
        /// </summary>
        public ObservableCollection<SerialPortParamer<String>> CommonPort;


        public SerialPortAttribute LastRecord;

        /// <summary>
        /// 获取索引，通过ToString() 进行比较
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="select">选择值</param>
        /// <returns>如果有返回相应索引，否则返回0</returns>
        public int GetIndex<T>(ObservableCollection<SerialPortParamer<T>> list, T select)
            
        {
           for(int i = 0; i <　list.Count; i++)
           {
               if (list[i].Paramer.ToString() == select.ToString())
               {
                   return i;
               }
           }
           return 0;
        }



        /// <summary>
        /// 初始化一个串口参数合集
        /// </summary>
        public SerialPortParameterItem()
        {
            try
            {
                Baud = new ObservableCollection<SerialPortParamer<int>>();
                Baud.Add(new SerialPortParamer<int>(1200));
                Baud.Add(new SerialPortParamer<int>(2400));
                Baud.Add(new SerialPortParamer<int>(4800));
                Baud.Add(new SerialPortParamer<int>(9600));
                Baud.Add(new SerialPortParamer<int>(14400));
                Baud.Add(new SerialPortParamer<int>(28800));
                Baud.Add(new SerialPortParamer<int>(38400));
                Baud.Add(new SerialPortParamer<int>(56000));
                Baud.Add(new SerialPortParamer<int>(57600));
                Baud.Add(new SerialPortParamer<int>(115200));


                DataBit = new ObservableCollection<SerialPortParamer<int>>();
                DataBit.Add(new SerialPortParamer<int>(5));
                DataBit.Add(new SerialPortParamer<int>(6));
                DataBit.Add(new SerialPortParamer<int>(7));
                DataBit.Add(new SerialPortParamer<int>(8));

                ParityBit = new ObservableCollection<SerialPortParamer<Parity>>();
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.Even));
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.Odd));
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.Mark));
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.Space));
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.None));

                StopBit = new ObservableCollection<SerialPortParamer<StopBits>>();
                StopBit.Add(new SerialPortParamer<StopBits>(StopBits.One));
                StopBit.Add(new SerialPortParamer<StopBits>(StopBits.OnePointFive));
                StopBit.Add(new SerialPortParamer<StopBits>(StopBits.Two));


                CommonPort = new ObservableCollection<SerialPortParamer<string>>();
                foreach (string s in SerialPort.GetPortNames())
                {
                    CommonPort.Add(new SerialPortParamer<string>(s));
                }

                LastRecord = XMLOperate.ReadLastCommonRecod();

            }
            catch(Exception ex)
            {

                ZFreeGo.Common.LogTrace.CLog.LogError(ex.StackTrace);
            }
        }
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

            public SerialPortAttribute(int comNum, int baud, int databit, int parityBit, int stopbit)
            {
               
                string comstr = "COM" + comNum.ToString();
                var ports = SerialPort.GetPortNames();
                bool isExist = false;
                foreach(var m in  ports)
                {
                    if (comstr == m)
                    {
                        isExist = true;
                    }
                }
                if(!isExist)
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

                switch(parityBit)
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
               
                switch(stopbit)
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


        }
    }
}
