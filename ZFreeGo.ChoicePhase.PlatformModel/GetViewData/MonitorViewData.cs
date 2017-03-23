using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.PlatformModel.DataItemSet;
using ZFreeGo.ChoicePhase.PlatformModel.Helper;

namespace ZFreeGo.ChoicePhase.PlatformModel.GetViewData
{
    public class MonitorViewData
    {
        /// <summary>
        /// 属性列表
        /// </summary>
        private ObservableCollection<AttributeItem> _yongciAttribute;
        /// <summary>
        /// 数据库操作
        /// </summary>
        private SQLliteDatabase dataBase;

        public MonitorViewData()
        {
            dataBase = new SQLliteDatabase(CommonPath.DataBase);
        }


        #region 属性列表操作--单个永磁控制器

        /// <summary>
        /// 创建YongciAttribute
        /// </summary>
        public void CrateYongciAttributeTable()
        {
            string sql =
                "CREATE TABLE YongciAttribute( ConfigID int, Name text,rawValue rawValue,  dataType int, value double, comment text)";
            dataBase.CreateTale(sql);
            
        }



        /// <summary>
        /// 清空历史数据，将数据插入表格
        /// </summary>       
        public void InsertYongciAttribute()
        {
            var collect = _yongciAttribute;
            var listStr = new List<String>();
            foreach (var m in collect)
            {
                string sql = string.Format("INSERT INTO  YongciAttribute VALUES({0},\'{1}\',{2},{3},{4},\'{5}\')",
                   m.ConfigID, m.Name, m.RawValue, m.DataType, m.Value, m.Comment);
                listStr.Add(sql);
            }
            if (listStr.Count > 0)
            {
                string sqlClear = "delete from  YongciAttribute";
                dataBase.InsertTable(listStr, sqlClear);
            }
        }

        /// <summary>
        /// 读取永磁属性数据表格
        /// </summary>
        /// <param name="flag">true--重新更新, false--若当前已存在则直接使用</param>
        /// <returns>永磁属性合集</returns>       
        public ObservableCollection<AttributeItem> ReadYongciAttribute(bool flag)
        {
            if (_yongciAttribute == null || flag)
            {
                _yongciAttribute = new ObservableCollection<AttributeItem>();
                string sql = "SELECT * from YongciAttribute";
                dataBase.ReadTable(sql, GetYongciAttribute);
                return _yongciAttribute;
            }
            else
            {
                return _yongciAttribute;
            }
        }
        private bool GetYongciAttribute(System.Data.SQLite.SQLiteDataReader reader)
        {

            _yongciAttribute.Add(new AttributeItem(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2),
                reader.GetInt32(3), reader.GetDouble(4), reader.GetString(5)));

            return true;
        }


        #endregion
    }
}
