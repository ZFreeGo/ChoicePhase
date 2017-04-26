﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFreeGo.ChoicePhase.Modbus;
using ZFreeGo.ChoicePhase.PlatformModel.GetViewData;
using ZFreeGo.ChoicePhase.PlatformModel.LogicApplyer;
using ZFreeGo.Monitor.DASModel.GetViewData;

namespace ZFreeGo.ChoicePhase.PlatformModel
{


    public class PlatformModelServer
    {
        private byte _localAddr;

        /// <summary>
        /// 监控数据
        /// </summary>
        private MonitorViewData _monitorViewData;

        /// <summary>
        /// 获取监控数据
        /// </summary>
        public MonitorViewData MonitorData
        {
            get
            {
                return _monitorViewData;
            }
        }


        /// <summary>
        /// 通讯服务
        /// </summary>
        public CommunicationServer CommServer
        {
            private set;
            get;
        }
        /// <summary>
        ///RTU处理服务
        /// </summary>
        public RtuServer RtuServer
        {
            private set;
            get;
        }

        /// <summary>
        /// 站服务列表
        /// </summary>
        public MasterStationServer StationServer
        {
            private set;
            get;
        }

        /// <summary>
        /// 多帧帧缓冲
        /// </summary>
        private List<byte> _multiFrameBuffer;
        private byte _lastIndex;
        /// <summary>
        /// 初始化控制平台Model服务
        /// </summary>
        public PlatformModelServer()
        {
            _localAddr = 0x1A;
            _monitorViewData = new MonitorViewData();
            CommServer = new CommunicationServer();
            CommServer.CommonServer.SerialDataArrived += CommonServer_SerialDataArrived;
            RtuServer = new RtuServer(_localAddr, 500, sendRtuData);
            RtuServer.RtuFrameArrived += RtuServer_RtuFrameArrived;

            StationServer = new MasterStationServer(_monitorViewData.MacList);
            StationServer.ArrtributesArrived += StationServer_ArrtributesArrived;
            StationServer.ExceptionArrived += StationServer_ExceptionArrived;
            StationServer.MultiFrameArrived += StationServer_MultiFrameArrived;
            _multiFrameBuffer = new List<byte>();
            _lastIndex = 0;
        }

        void StationServer_MultiFrameArrived(object sender, MultiFrameEventArgs e)
        {
            //判断是否为首帧
            if (e.Index == 0)
            {
                _multiFrameBuffer.Clear();//清空历史数据
                _lastIndex = 0;
                 _multiFrameBuffer.AddRange(e.ByteData);
                return;
            }

            int currentIndex = e.Index & 0x7F; //当前索引
            //是否为递增索引,正常接收
            if ((_lastIndex + 1) == currentIndex)
            {
                _multiFrameBuffer.AddRange(e.ByteData);
                _lastIndex++;
            }
            else
            {

            }
            //判断是否为最后一帧
            if ((e.Index & 0x80) == 0x80)
            {

                //TODO:显示
                CommServer.LinkMessage += "\n\n" + DateTime.Now.ToLongTimeString() + "  多帧接收:\n";
                CommServer.LinkMessage += "接收完成"+ "\n";
                //适当处理
                StringBuilder stb = new StringBuilder(_multiFrameBuffer.Count*4);
                if (_multiFrameBuffer.Count % 2 == 0)
                {
                    for (int i = 0; i < _multiFrameBuffer.Count; i += 2)
                    {
                        stb.AppendFormat("{0},", _multiFrameBuffer[i] + 256*_multiFrameBuffer[i + 1]);
                    }
                    CommServer.LinkMessage += "\n\n" + "  有效数据:\n";
                    CommServer.LinkMessage += stb.ToString() + "\n";
                }
            }

            
        }

        void StationServer_ExceptionArrived(object sender, ExceptionMessage e)
        {
            CommServer.LinkMessage += "\n\n" + DateTime.Now.ToLongTimeString() + "  异常:\n";

            CommServer.LinkMessage += e.Comment + "\n";
            CommServer.LinkMessage += e.Ex.Message + "\n";
            CommServer.LinkMessage += e.Ex.StackTrace + "\n";
        }

        void StationServer_ArrtributesArrived(object sender, ArrtributesEventArgs e)
        {
            
            _monitorViewData.UpdateAttributeData(e.MAC, e.ID, e.AttributeByte);
        }

        /// <summary>
        /// 发送数据转包
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>true--正常发送，false--端口关闭</returns>
        private bool sendRtuData(byte[] data)
        {
            if(CommServer.CommonServer.CommState)
            {
                CommServer.CommonServer.SendMessage(data);
                CommServer.LinkMessage += "\n" +DateTime.Now.ToLongTimeString() + "  RTU发送帧:\n";
                CommServer.LinkMessage += ByteToString(data, 0, data.Length);
                CommServer.RawSendMessage += ByteToString(data, 0, data.Length);
            }
            return CommServer.CommonServer.CommState;
            

        }


        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Close()
        {
            CommServer.CommonServer.Close();
            RtuServer.Close();
        }



        /// <summary>
        /// RTU帧数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void RtuServer_RtuFrameArrived(object sender, RtuFrameArgs e)
        {
            CommServer.LinkMessage += "\n" + DateTime.Now.ToLongTimeString() + "  接收RTU帧:\n";
            CommServer.LinkMessage += ByteToString(e.Frame.Frame, 0, e.Frame.Frame.Length);
            
            StationServer.StationDeal(e.Frame.FrameData);

        }

        /// <summary>
        /// 通讯数据到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommonServer_SerialDataArrived(object sender, Communication.SerialDataEvent e)
        {

            RtuServer.AddBuffer(e.SerialData);
            CommServer.RawReciveMessage += ByteToString(e.SerialData, 0, e.SerialData.Length);
        }

        private static PlatformModelServer _modelServer; 


        /// <summary>
        /// 获取Model服务
        /// </summary>
        /// <returns>Model Server</returns>
        public static PlatformModelServer GetServer()
        {
            return GetServer(false);
        }


        /// <summary>
        /// 获取Model服务
        /// </summary>
        /// <param name="flag">ture-重新新建， false--采用默认</param>
        /// <returns>Model Server</returns>
        public static PlatformModelServer GetServer(bool flag)
        {
            if((_modelServer == null) || flag)
            {
                _modelServer =  new PlatformModelServer();
                return _modelServer;
            }
            else
            {
                return _modelServer;
            }

        }

        private string ByteToString(byte[] data, int start, int len)
        {
            StringBuilder strBuild = new StringBuilder(len*3 + 10);
            for(int i = start; i < start + len; i++)
            {
                strBuild.AppendFormat("{0:X2} ", data[i]);
            }
           
            return strBuild.ToString();
        }


        



    }
}
