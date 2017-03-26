using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Net;
using GalaSoft.MvvmLight.Command;

namespace ZFreeGo.ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 属性参数包含可以应用于设定与参数读取
    /// </summary>
    public class AttributeItem : ObservableObject
    {
        private int _configID;

        /// <summary>
        /// 配置号
        /// </summary>
        public int ConfigID
        {
            get
            {
                return _configID;
            }
            set
            {
                _configID = value;
                RaisePropertyChanged("ConfigID");
            }
        }


        private  string _name;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name= value;
                RaisePropertyChanged("Name");
            }
        }
        private int _rawValue;
        /// <summary>
        /// 原始值
        /// </summary>
        public int RawValue
        {
            get
            {
                return _rawValue;
            }
            set
            {
                _rawValue = value;
                RaisePropertyChanged("RawValue");
            }
        }
        private  int _dataType;

        /// <summary>
        /// 数据类型
        /// </summary>
        public int DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
                RaisePropertyChanged("DataType");
            }
        }
        private double _value;
           /// <summary>
        /// 属性值
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

        private  string _comment;

        /// <summary>
        /// 说明内容
        /// </summary>
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        
      

        /// <summary>
        /// 属性值
        /// </summary>
        public AttributeItem(int configID, string name, int rawValue, int dataType, double value, string comment)
        {
            _configID = configID;
            _name = name;
            _rawValue = rawValue;
            _dataType = dataType;
            _value = value;           
            _comment = comment;           
        }






    }
}
