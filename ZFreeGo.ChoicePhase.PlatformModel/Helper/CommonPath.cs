using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.PlatformModel.Helper
{
    /// <summary>
    /// 通用的公共路径,  
    /// TODO：需要检查每个路径
    /// </summary>
    public class CommonPath
    {
        /// <summary>
        /// 账户xml路径
        /// </summary>
        public static string AccountXmlPath = @"Config\Account\UserAccount.xml";
        // string pathxmlAccount = @"t2.xml";
        /// <summary>
        /// 账户xsd路径
        /// </summary>
        public static string AccountXsdPath = @"Config\Account\UserAccount.xsd";

        /// <summary>
        /// 日志文件夹路径
        /// </summary>
        public static string LogDirectoryPath = @"log\log";


        /// <summary>
        /// 串口XML表格路径
        /// </summary>
        public static string CommonPortXmlPath = @"Config\XML\Config.xml";

        /// <summary>
        /// 串口XSD架构路径
        /// </summary>
        public static string CommonPortXsdPath = @"Config\XSD\Config.xsd";

      

        /// <summary>
        /// 事件记录XML路径
        /// </summary>
        public static string SOEXmlPath = @"Config\SOE.xml";
        /// <summary>
        /// 事件记录XSD架构路径
        /// </summary>
        public static string SOEXsdPath = @"Config\SOE.xsd";


        public static string DataBase =  @"config\Database\Das.db";
    }
}
