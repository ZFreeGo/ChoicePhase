using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;



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
        /// 获取数据集合
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="typeEnum"></param>
        /// <returns></returns>
        public  static ZFreeGo.ChoicePhase.PlatformModel.Communication.SerialPortAttribute ReadLastCommonRecod()
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

        //public void  UpdateTable(DataSet ds, DataTypeEnum typeEnum, object dataObserver)
        //{
        //    try
        //    {
               
        //        switch (typeEnum)
        //        {
        //            case DataTypeEnum.Telesignalisation:
        //                {
        //                    ds.Tables["Telesignalisation"].Rows.Clear();
        //                    var datas = dataObserver as ObservableCollection<Telesignalisation>;
        //                    foreach (var m in datas)
        //                    {
        //                        var productRow = ds.Tables["Telesignalisation"].NewRow();
        //                        productRow["InternalID"] = m.InternalID;
        //                        productRow["TelesignalisationName"] = m.TelesignalisationName;
        //                        productRow["TelesignalisationID"] = m.TelesignalisationID;
        //                        productRow["TelesignalisationValue"] = m.TelesignalisationResult;
        //                        productRow["IsNot"] = m.IsNot;
        //                        productRow["Date"] = m.Date;
        //                        productRow["Comment"] = m.Comment;
        //                        productRow["StateA"] = m.StateA;
        //                        productRow["StateB"] = m.StateB;
        //                        ds.Tables["Telesignalisation"].Rows.Add(productRow);
        //                    }
                           

                   

        //                    break;
        //                }
        //            default:
        //                {
        //                    throw new ArgumentOutOfRangeException("所使用的枚举值不在范围之内!");
        //                }
        //        }

                
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
       

        

    }
}
