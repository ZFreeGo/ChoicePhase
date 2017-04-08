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
        /// 设定值 属性列表
        /// </summary>
        private ObservableCollection<AttributeItem> _yongciAttribute;

        /// <summary>
        /// 监控量 属性列表
        /// </summary>
        private ObservableCollection<AttributeItem> _yongciMonitorAttribute;


        /// <summary>
        /// 设定值 属性列表--按顺序分别为永磁控制器A，永磁控制器B，永磁控制器C，DSP,ARM，监控A，监控B，监控C
        /// 永磁控制器A 0 -- 设置属性合集， 1--参数属性合集
        /// 永磁控制器B 2 -- 设置属性合集， 3--参数属性合集
        /// 永磁控制器C 4 -- 设置属性合集， 5--参数属性合集 ---以此类推
        /// </summary>
        private List<ObservableCollection<AttributeItem>> _AttributeCollect;

        /// <summary>
        /// 属性表格名称
        /// </summary>
        private List<string> _tableAttributeName;

        /// <summary>
        /// 数据库操作
        /// </summary>
        private SQLliteDatabase dataBase;

        public MonitorViewData()
        {
            dataBase = new SQLliteDatabase(CommonPath.DataBase);
            _AttributeCollect = new List<ObservableCollection<AttributeItem>>();
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);
            _AttributeCollect.Add(null);


            _tableAttributeName = new List<string>();

            _tableAttributeName.Add("AttributeSetYongciA");
            _tableAttributeName.Add("AttributeReadYongciA");
            _tableAttributeName.Add("AttributeSetYongciB");
            _tableAttributeName.Add("AttributeReadYongciB");
            _tableAttributeName.Add("AttributeSetYongciC");
            _tableAttributeName.Add("AttributeReadYongciC");
            _tableAttributeName.Add("AttributeSetDSP");
            _tableAttributeName.Add("AttributeReadDSP");
            _tableAttributeName.Add("AttributeSetARM");
            _tableAttributeName.Add("AttributeReadARM");
            _tableAttributeName.Add("AttributeSetMonitorA");
            _tableAttributeName.Add("AttributeReadMonitorA");
            _tableAttributeName.Add("AttributeSetMonitorB");
            _tableAttributeName.Add("AttributeReadMonitorB");
            _tableAttributeName.Add("AttributeSetMonitorC");
            _tableAttributeName.Add("AttributeReadMonitorC");
        }



        #region 属性列表操作--单个永磁控制器 设定量

        /// <summary>
        /// 创建YongciAttribute
        /// </summary>
        public void CrateYongciAttributeTable()
        {
            string sql =
                "CREATE TABLE YongciAttribute( ConfigID int, Name text,RawValue int,  dataType int, value double, comment text)";
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

            _yongciAttribute.Add(new AttributeItem(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2),
                reader.GetInt32(3), reader.GetDouble(4), reader.GetString(5)));
         
            return true;
        }


        #endregion

        #region 属性列表操作--单个永磁控制器 监控量

        /// <summary>
        /// 创建YongciAttribute
        /// </summary>
        public void CrateYongciMonitorAttributeTable()
        {
            string sql =
                "CREATE TABLE YongciMonitorAttribute( ConfigID int, Name text,RawValue int,  dataType int, value double, comment text)";
            dataBase.CreateTale(sql);

        }



        /// <summary>
        /// 清空历史数据，将数据插入表格
        /// </summary>       
        public void InsertYongciMonitorAttribute()
        {
            var collect = _yongciMonitorAttribute;
            var listStr = new List<String>();
            foreach (var m in collect)
            {
                string sql = string.Format("INSERT INTO  YongciMonitorAttribute VALUES({0},\'{1}\',{2},{3},{4},\'{5}\')",
                   m.ConfigID, m.Name, m.RawValue, m.DataType, m.Value, m.Comment);
                listStr.Add(sql);
            }
            if (listStr.Count > 0)
            {
                string sqlClear = "delete from  YongciMonitorAttribute";
                dataBase.InsertTable(listStr, sqlClear);
            }
        }

        /// <summary>
        /// 读取永磁属性数据表格
        /// </summary>
        /// <param name="flag">true--重新更新, false--若当前已存在则直接使用</param>
        /// <returns>永磁属性合集</returns>       
        public ObservableCollection<AttributeItem> ReadYongciMonitorAttribute(bool flag)
        {
            if (_yongciMonitorAttribute == null || flag)
            {
                _yongciMonitorAttribute = new ObservableCollection<AttributeItem>();
                string sql = "SELECT * from YongciMonitorAttribute";
                dataBase.ReadTable(sql, GetYongciMonitorAttribute);
                return _yongciMonitorAttribute;
            }
            else
            {
                return _yongciMonitorAttribute;
            }
        }
        private bool GetYongciMonitorAttribute(System.Data.SQLite.SQLiteDataReader reader)
        {

            _yongciMonitorAttribute.Add(new AttributeItem(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2),
                reader.GetInt32(3), reader.GetDouble(4), reader.GetString(5)));

            return true;
        }
        #endregion


        #region 属性列表操作--通用形式

        /// <summary>
        /// 创建Attribute
        /// </summary>
        public void CrateAttributeTable(AttributeIndex index)
        {

            string sql =
                "CREATE TABLE " + _tableAttributeName[(int)index] + "( ConfigID int, Name text,RawValue int,  dataType int, value double, comment text)";
            dataBase.CreateTale(sql);

        }



        /// <summary>
        /// 清空历史数据，将数据插入表格
        /// </summary>       
        public void InsertAttribute(AttributeIndex index)
        {

            var tableName = _tableAttributeName[(int)index];
            var collect = _AttributeCollect[(int)index];
         
            var listStr = new List<String>();
            foreach (var m in collect)
            {
                string sql = string.Format("INSERT INTO " + tableName+ " VALUES({0},\'{1}\',{2},{3},{4},\'{5}\')",
                   m.ConfigID, m.Name, m.RawValue, m.DataType, m.Value, m.Comment);
                listStr.Add(sql);
            }
            if (listStr.Count > 0)
            {
                string sqlClear = "delete from " + tableName;
                dataBase.InsertTable(listStr, sqlClear);
            }
        }
      
        private AttributeIndex _currentAttributeIndex;
        /// <summary>
        /// 读取属性数据表格
        /// </summary>
        /// <param name="flag">true--重新更新, false--若当前已存在则直接使用</param>
        /// <param name="tableName">表格名称</param>
        /// <returns>永磁属性合集</returns>       
        public ObservableCollection<AttributeItem> ReadAttribute(AttributeIndex index, bool flag)
        {
            var tableName = _tableAttributeName[(int)index];
            var collect = _AttributeCollect[(int)index];
           

            if (collect == null || flag)
            {
                collect = new ObservableCollection<AttributeItem>();
                string sql = "SELECT * from " + tableName;
                _currentAttributeIndex = index; //同步更新currentAttributeIndex
                _AttributeCollect[(int)index] = collect;
                dataBase.ReadTable(sql, GetAttribute);
                return collect;
            }
            else
            {
                return collect;
            }
        }


        private bool GetAttribute(System.Data.SQLite.SQLiteDataReader reader)
        {

            _AttributeCollect[(int)_currentAttributeIndex].Add(new AttributeItem(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2),
                reader.GetInt32(3), reader.GetDouble(4), reader.GetString(5)));

            return true;
        }


        #endregion

       





        /// <summary>
        /// 更新属性字节
        /// </summary>
        /// <param name="id">ID号</param>
        /// <param name="data">数据字节</param>
        public void UpdateYongciAttributeData(int id, byte[] data)
        {

            if(id < 0x41)//设置属性
            {
                ReadYongciAttribute(false);
                for(int i = 0; i< _yongciAttribute.Count; i++)
                {
                    if(_yongciAttribute[i].ConfigID == id)
                    {
                        _yongciAttribute[i].UpdateAttribute(data);
                    }
                }
            }
            else  //读取属性
            {
                ReadYongciMonitorAttribute(false);
                for (int i = 0; i < _yongciMonitorAttribute.Count; i++)
                {
                    if (_yongciMonitorAttribute[i].ConfigID == id)
                    {
                        _yongciMonitorAttribute[i].UpdateAttribute(data);
                    }
                }
            }
        }
        /// <summary>
        /// 更新属性字节
        /// </summary>
        /// <param name="id">ID号</param>
        /// <param name="data">数据字节</param>
        public void UpdateAttributeData(int mac, int id, byte[] data)
        {
            if (mac == 0x10)//测试使用
            {
                
                if (id < 0x41)//设置属性
                {
                    ReadAttribute(AttributeIndex.YongciSetA, false);
                    var collect = _AttributeCollect[(int)AttributeIndex.YongciSetA];
                    for (int i = 0; i < collect.Count; i++)
                    {
                        if (collect[i].ConfigID == id)
                        {
                            collect[i].UpdateAttribute(data);
                        }
                    }
                }
                else  //读取属性
                {
                    ReadAttribute(AttributeIndex.YongciReadA, false);
                    var collect = _AttributeCollect[(int)AttributeIndex.YongciReadA];
                    for (int i = 0; i < collect.Count; i++)
                    {
                        if (collect[i].ConfigID == id)
                        {
                            collect[i].UpdateAttribute(data);
                        }
                    }
                }
            }
        }
    
    }
    /// <summary>
    /// 参数表格索引
    /// </summary>
    public enum AttributeIndex
    {
        /// <summary>
        /// 永磁设置参数A
        /// </summary>
        YongciSetA  = 0,
        /// <summary>
        /// 永磁只读参数A
        /// </summary>
        YongciReadA = 1,
        /// <summary>
        /// 永磁设置参数B
        /// </summary>
        YongciSetB = 2,
        /// <summary>
        /// 永磁只读参数B
        /// </summary>
        YongciReadB = 3,
        /// <summary>
        /// 永磁设置参数C
        /// </summary>
        YongciSetC = 4,
        /// <summary>
        /// 永磁只读参数A
        /// </summary>
        YongciReadC = 5,
        /// <summary>
        /// DSP设置参数
        /// </summary>
        DspSet = 6,
        /// <summary>
        /// DSP只读参数
        /// </summary>
        DspRead = 7,
        /// <summary>
        /// DSP设置参数
        /// </summary>
        ArmSet = 8,
        /// <summary>
        /// DSP只读参数
        /// </summary>
        ArmRead = 9,

  
        /// <summary>
        /// 监控设置参数A
        /// </summary>
        MonitorSetA = 10,
        /// <summary>
        /// 监控只读参数A
        /// </summary>
        MonitorReadA = 11,
        /// <summary>
        /// 监控设置参数B
        /// </summary>
        MonitorSetB = 12,
        /// <summary>
        /// 监控只读参数B
        /// </summary>
        MonitorReadB = 13,
        /// <summary>
        /// 监控设置参数C
        /// </summary>
        MonitorSetC = 14,
        /// <summary>
        /// 监控只读参数C
        /// </summary>
        MonitorReadC = 15,
    }
}
