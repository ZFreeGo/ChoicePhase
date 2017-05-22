using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using ZFreeGo.ChoicePhase.DeviceNet.LogicApplyer;
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

        private  List<byte> _macList;

        public List<byte> MacList
        {
            get
            {
                return _macList;
            }
        }

        private const byte readonlyStartIndex  =0x41;

        /// <summary>
        /// 站点名称列表
        /// </summary>
        public ObservableCollection<string> StationNameList
        {
            private set;
            get;

        }


        /// <summary>
        /// 节点状态
        /// </summary>
        public ObservableCollection< NodeStatus> NodeStatusList
        {
            private set;
            get;
        }
       

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


            //站点名称
            StationNameList = new ObservableCollection<string>();
            StationNameList.Add("A相控制器");
            StationNameList.Add("B相控制器");
            StationNameList.Add("C相控制器");
            StationNameList.Add("同步控制器");
            StationNameList.Add("网络控制器");
            StationNameList.Add("A相监测");
            StationNameList.Add("B相监测");
            StationNameList.Add("C相监测");

            _macList = new List<byte>();
            _macList.Add(0x10);
            _macList.Add(0x12);
            _macList.Add(0x14);
            _macList.Add(0x0D);
            _macList.Add(0x02);
            _macList.Add(0x16);
            _macList.Add(0x18);
            _macList.Add(0x1A);



            NodeStatusList = new ObservableCollection<NodeStatus>();
            NodeStatusList.Add(new NodeStatus(0x0D, "同步控制器"));
            NodeStatusList.Add(new NodeStatus(0x10, "A相控制器"));
            NodeStatusList.Add(new NodeStatus(0x12, "B相控制器"));
            NodeStatusList.Add(new NodeStatus(0x14, "C相控制器"));
            

        }


        /// <summary>
        /// 更具索引获取MAC地址
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public  byte GetMacAddr(AttributeIndex index)
        {
            switch (index)
            {
                case AttributeIndex.YongciReadA:
                case AttributeIndex.YongciSetA:
                    {
                        return _macList[0];
                    }
                case AttributeIndex.YongciReadB:
                case AttributeIndex.YongciSetB:
                    {
                        return _macList[1];
                    }
                case AttributeIndex.YongciReadC:
                case AttributeIndex.YongciSetC:
                    {
                        return _macList[2];
                    }
                case AttributeIndex.DspRead:
                case AttributeIndex.DspSet:
                    {
                        return _macList[3];
                    }
                case AttributeIndex.ArmRead:
                case AttributeIndex.ArmSet:
                    {
                        return _macList[4];
                    }
                case AttributeIndex.MonitorReadA:
                case AttributeIndex.MonitorSetA:
                    {
                        return _macList[5];
                    }
                case AttributeIndex.MonitorReadB:
                case AttributeIndex.MonitorSetB:
                    {
                        return _macList[6];
                    }
                case AttributeIndex.MonitorReadC:
                case AttributeIndex.MonitorSetC:
                    {
                        return _macList[7];
                    }
                default:
                    {
                        return 0xFF;
                    }
            }
        }


        public AttributeIndex GetAttributeIndex(int mac, int id)
        {


            if (_macList[0] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.YongciSetA;
                }
                else
                {
                    return AttributeIndex.YongciReadA;
                }
            }
            if (_macList[1] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.YongciSetB;
                }
                else
                {
                    return AttributeIndex.YongciReadB;
                }
            }
            if (_macList[2] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.YongciSetC;
                }
                else
                {
                    return AttributeIndex.YongciReadC;
                }
            }

            if (_macList[3] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.DspSet;
                }
                else
                {
                    return AttributeIndex.DspRead;
                }
            }
            if (_macList[4] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.ArmSet;
                }
                else
                {
                    return AttributeIndex.ArmRead;
                }
            }
            if (_macList[5] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.MonitorSetA;
                }
                else
                {
                    return AttributeIndex.MonitorReadA;
                }
            }

            if (_macList[6] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.MonitorSetB;
                }
                else
                {
                    return AttributeIndex.MonitorReadB;
                }
            }

            if (_macList[7] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.MonitorSetC;
                }
                else
                {
                    return AttributeIndex.MonitorReadC;
                }
            }
            return AttributeIndex.Null;
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
          
            var index= GetAttributeIndex (mac, id);
            if (index != AttributeIndex.Null)
            {
                ReadAttribute(index, false);
                var collect = _AttributeCollect[(int)index];
                for (int i = 0; i < collect.Count; i++)
                {
                    if (collect[i].ConfigID == id)
                    {
                        collect[i].UpdateAttribute(data);
                    }
                }
            }
        }

        /// <summary>
        /// 合闸预制状态
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="data"></param>
        public void UpdateNodeStatus(byte mac, byte[] data)
        {
            var node = GetNdoe(mac);
            if (node == null)
            {
                return;
            }
            node.IsValid(data);

            var cmd = (CommandIdentify)(data[0] & 0x7F);

            node.ResetState();
            switch (mac)
            {
                case 0x0D://同步控制器                    
                    {
                        switch (cmd)
                        {
                            case CommandIdentify.SyncOrchestratorCloseAction:
                                {
                                    node.SynActionCloseState = true;
                                    break;
                                }
                            case CommandIdentify.SyncOrchestratorReadyClose:
                                {
                                    node.SynReadyCloseState = true;
                                    break;
                                }
                        }
                        break;
                    }
                case 0x10: //A相                  
                case 0x12://B相             
                case 0x14: //C相
                    {
                       
                        switch (cmd)
                        {
                            case CommandIdentify.CloseAction://合闸执行
                                {
                                    node.ActionCloseState = true;
                                    break;
                                }
                            case CommandIdentify.OpenAction: //分闸执行
                                {
                                    node.ActionOpenState = true;
                                    break;
                                }
                            case CommandIdentify.ReadyClose: // 合闸预制
                                {
                                    node.ReadyCloseState = true;
                                    break;
                                }
                            case CommandIdentify.ReadyOpen:  //分闸预制                   
                                {
                                    node.ReadyOpenState = true;
                                    break;
                                }
                            case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                                {
                                    node.SynReadyCloseState = true;
                                    break;
                                }
                           
                        }
                        break;
                    }

            }
        }       

       




        public NodeStatus GetNdoe(byte mac)
        {
            foreach(var m in NodeStatusList)
            {
                if (m.Mac == mac)
                {
                    return m;
                }
            }
            return null;
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


        Null = 0xFF,
    }
}
