using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.ControlCenter
{
    class Tool
    {
        /// <summary>
        /// 32bit无符号整型转换为字节数组，由低位到高位排列
        /// </summary>
        /// <param name="num">32bit 无符号整型</param>
        /// <returns>字节数组</returns>
        static public byte[] Uint32ToByte(uint num)
        {
            byte[] array = new byte[4];
            array[0] = (byte)(num & 0x000000FF);
            array[1] = (byte)((num & 0x0000FF00) >> 8);
            array[2] = (byte)((num & 0x00FF0000) >> 16);
            array[3] = (byte)((num & 0xFF000000) >> 24);
            return array;
        }

        /// <summary>
        /// 限制边界
        /// </summary>
        /// <param name="value">被检测的值</param>
        /// <param name="left">左边界</param>
        /// <param name="right">右边界</param>
        /// <returns>限制后的值</returns>
        static public  double  CheckBound(double value, double left, double right)
        {
            if (value < left)
            {
                return left;
            }
            if (value > right)
            {
                return right;
            }
            return value;

        }
        /// <summary>
        /// 由真实电流值得出下位机需要的电流表达形式
        /// </summary>
        /// <param name="real">电流值(A)</param>
        /// <returns>转换后的电流形式</returns>
        static public uint CalMcuCurrentByReal(double real)
        {
            double rat = 10f / (1f/5f * 1024f); //%0.0488 A/div 


            double mcu_value =  Math.Pow(real/rat, 2) * 20; //%对应的计算值 4bytes

            return (uint)mcu_value;
        }
        /// <summary>
        /// 由真实电流值得出下位机需要的电流表达形式
        /// </summary>
        /// <param name="real">电流值(A)</param>
        /// <returns>转换后的电流形式</returns>
        static public ushort CalMcuCurrentByReal(double real, byte leftbit)
        {
            double rat = 10f / (1f / 5f * 1024f); //%0.0488 A/div 


            double mcu_value = Math.Pow(real / rat, 2) * 20; //%对应的计算值 4bytes
            mcu_value = mcu_value / Math.Pow(2, leftbit);
            return (ushort)Math.Round(mcu_value, 0);
        }
        /// <summary>
        /// 由时间得出下位机需要的时间形式
        /// </summary>
        /// <param name="time">时间 (s)</param>
        /// <returns>转换后的时间形式</returns>
        static public uint CalMcuTimeByReal(double time)
        {
            double t = 1/ time  * 1e5 * 3; // %需要4 bytes
            return (uint)t;
        }
        static public ushort CalMcuTimeByRealTwo(double time)
        {
            double t = 1 / time * 1e5 * 3; // %需要4 bytes
            return (ushort)t;
        }

        static public Tuple<double[], double[]> StringFoamtToNum(string xstr, string ystr)
        {
            string strCurrent = xstr;
            char[] charSeparators = new char[] { ' ' };
            var resultCurrentStr = strCurrent.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            string strTime = ystr;
            var resultTimeStr = strTime.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

            int len = resultCurrentStr.Length;

            if (resultCurrentStr.Length == 0)
            {
                throw new Exception("电流区域为空！");
            }
            if (resultTimeStr.Length == 0)
            {
                throw new Exception("时间区域为空！");
            }
            if (resultCurrentStr.Length != resultTimeStr.Length)
            {
                throw new Exception("电流与电压长度不匹配");
            }
            if (resultCurrentStr.Length > 100)
            {
                throw new Exception(string.Format("输入点数过多，应小于等于{0}", 100));
            }

            var resultCurrentNum = new double[len];
            var resultTimeNum = new double[len];



            for (int i = 0; i < len; i++)
            {
                resultCurrentNum[i] = Convert.ToDouble(resultCurrentStr[i]);
                resultTimeNum[i] = Convert.ToDouble(resultTimeStr[i]);
            }

            return new Tuple<double[], double[]>(resultCurrentNum, resultTimeNum);
        }

        static public double Byte4ToFloat(byte[] data, double rate)
        {
            float res = data[0] + data[1] * 256 +
                                  ((UInt32)data[2]) * 256 * 256 + (UInt32)data[3] * 256 * 256 * 256;
            return res * rate;
        }
    }
}
