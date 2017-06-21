using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.PlatformModel.Communication;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;



namespace ZFreeGo.ChoicePhase.PlatformModel.Helper
{
    public class XMLOperate
    {

        static public DataSet ReadXml(string xmlPath, string xsdPath)
        {
            try
            {
                if (!File.Exists(xmlPath))
                {
                    throw new Exception("路径:" + xmlPath + ", 所指向的文件不存在，请重新选择");
                }
                if (!File.Exists(xsdPath))
                {
                    throw new Exception("路径:" + xsdPath + ", 所指向的文件不存在，请重新选择");
                }

                var ds = new DataSet();
                ds.ReadXmlSchema(xsdPath);
                ds.ReadXml(xmlPath);
                
                return ds;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            

        }

        /// <summary>
        /// 获取串口数据集合
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="typeEnum"></param>
        /// <returns></returns>
        public  static SerialPortAttribute ReadLastPortRecod()
        {
            try
            {

                var ds = ReadXml(CommonPath.CommonPortXmlPath, CommonPath.CommonPortXsdPath);

                DataRow productRow = ds.Tables["SerialPort"].Rows[0];

                return new ZFreeGo.ChoicePhase.PlatformModel.Communication.SerialPortAttribute(
                    (int)productRow["CommonPort"],
                    (int)productRow["Baud"],
                    (int)productRow["DataBit"],
                    (int)productRow["ParityBit"],
                    (int)productRow["StopBit"]);                           

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取配置参数集合
        /// </summary>
        public static void ReadLastConfigRecod()
        {
            try
            {
                var ds = ReadXml(CommonPath.CommonPortXmlPath, CommonPath.CommonPortXsdPath);

                DataRow productRow = ds.Tables["ConfigParameter"].Rows[0];

                NodeAttribute.EnabitSelect =(byte) (int)productRow["EnabitSelect"];
                NodeAttribute.SynCloseActionOverTime = (int)productRow["SynCloseActionOverTime"];
                NodeAttribute.CloseActionOverTime = (int)productRow["CloseActionOverTime"];
                NodeAttribute.OpenActionOverTime = (byte)(int)productRow["OpenActionOverTime"];

                NodeAttribute.ClosePowerOnTime = (byte)(int)productRow["ClosePowerOnTime"];
                NodeAttribute.OpenPowerOnTime = (byte)(int)productRow["OpenPowerOnTime"];
                NodeAttribute.WorkMode = (int)productRow["WorkMode"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 存储配置参数集合
        /// </summary>
        public static void WriteLastConfigRecod()
        {
            try
            {
                var ds = ReadXml(CommonPath.CommonPortXmlPath, CommonPath.CommonPortXsdPath);

                DataRow productRow = ds.Tables["ConfigParameter"].Rows[0];

                productRow["EnabitSelect"] = (int)NodeAttribute.EnabitSelect;
                productRow["SynCloseActionOverTime"] = (int)NodeAttribute.SynCloseActionOverTime;
                productRow["CloseActionOverTime"]  = (int)NodeAttribute.CloseActionOverTime;
                productRow["OpenActionOverTime"] = (int) NodeAttribute.OpenActionOverTime;

                productRow["ClosePowerOnTime"]  = (int)NodeAttribute.ClosePowerOnTime;
                productRow["OpenPowerOnTime"] = (int) NodeAttribute.OpenPowerOnTime;
                productRow["WorkMode"]  = (int)NodeAttribute.WorkMode;

                ds.WriteXml(CommonPath.CommonPortXmlPath); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>        
        /// 保存命令
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="typeEnum"></param>
        /// <returns></returns>
        public static void WriteLastPortRecod(SerialPortAttribute attribute)
        {
            try
            {
                var ds = ReadXml(CommonPath.CommonPortXmlPath, CommonPath.CommonPortXsdPath);

                ds.Tables["SerialPort"].Rows.Clear();

                var productRow = ds.Tables["SerialPort"].NewRow();
               
                productRow["CommonPort"] = attribute.CommonPortNum;
                productRow["Baud"]  = attribute.Baud;
                productRow["DataBit"] = attribute.DataBit;
                productRow["ParityBit"] = attribute.ParityBitMark;
                productRow["StopBit"] = attribute.StopBitMark;
                ds.Tables["SerialPort"].Rows.Add(productRow);
                ds.WriteXml(CommonPath.CommonPortXmlPath);               

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
