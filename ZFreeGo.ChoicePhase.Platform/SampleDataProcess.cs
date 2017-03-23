using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.ControlCenter
{
    public class SampleDataBase
    {
        public byte[] RawData;
        public int SaveLen = 0;
        public string Dscr = "";
        public  SampleDataBase(int len)
        {
            RawData = new byte[len];
            SaveLen = 0;
        }
        public SampleDataBase(int len, string dscr)
        {
            RawData = new byte[len];
            SaveLen = 0;
            Dscr = dscr;
        }
    }
    public class FrameDataCollect
    {
        public List<SampleDataBase> RawData;
    }
    public class SampDataType
    {
        public SampleDataBase RawMainVolatageA;//主回路A相电压
        public SampleDataBase RawMainVolatageB;//主回路B相电压
        public SampleDataBase RawMainVolatageC;//主回路C相电压

        public SampleDataBase RawMainCurrentA;//主回路A相电流 
        public SampleDataBase RawMainCurrentB;//主回路B相电流 
        public SampleDataBase RawMainCurrentC;//主回路C相电流 

        public SampleDataBase RawMainDuankouVolatageA;//主回路A相断口电压
        public SampleDataBase RawMainDuankouVolatageB;//主回路B相断口电压
        public SampleDataBase RawMainDuankouVolatageC;//主回路C相断口电压

        public SampleDataBase RawXianquanCurrent;//线圈电流A

        public SampleDataBase RawJiasuduA;//A相 加速度
        public SampleDataBase RawJiasuduB;//B相 加速度
        public SampleDataBase RawJiasuduC;//C相 加速度

        public SampleDataBase RawLineA;//A相 位移
        public SampleDataBase RawLineB;//B相 位移
        public SampleDataBase RawLineC;//C相 位移

        

        


        public List<SampleDataBase> RawData; //集合
        public SampDataType(int len)
        {
            RawMainVolatageA = new SampleDataBase(len);//主回路A相电压
            RawMainVolatageB = new SampleDataBase(len);//主回路B相电压
            RawMainVolatageC = new SampleDataBase(len);//主回路C相电压

            RawMainCurrentA = new SampleDataBase(len);//主回路A相电流 
            RawMainCurrentB = new SampleDataBase(len);//主回路B相电流 
            RawMainCurrentC = new SampleDataBase(len);//主回路C相电流 

            RawMainDuankouVolatageA = new SampleDataBase(len);//主回路A相断口电压
            RawMainDuankouVolatageB = new SampleDataBase(len);//主回路B相断口电压
            RawMainDuankouVolatageC = new SampleDataBase(len);//主回路C相断口电压

            RawXianquanCurrent = new SampleDataBase(len);//线圈电流A

            RawJiasuduA = new SampleDataBase(len);//A相 加速度
            RawJiasuduB = new SampleDataBase(len);//B相 加速度
            RawJiasuduC = new SampleDataBase(len);//C相 加速度

            RawLineA = new SampleDataBase(len);//A相 位移
            RawLineB = new SampleDataBase(len);//B相 位移
            RawLineC = new SampleDataBase(len);//C相 位移

           

            RawData = new List<SampleDataBase>();

            RawData.Add(RawXianquanCurrent);

            RawData.Add(RawMainDuankouVolatageA);
            RawData.Add(RawMainDuankouVolatageB);
            RawData.Add(RawMainDuankouVolatageC);

            RawData.Add(RawMainCurrentA);
            RawData.Add(RawMainCurrentB);
            RawData.Add(RawMainCurrentC);


            //仅仅用到以上七组

            RawData.Add(RawMainVolatageA);
            RawData.Add(RawMainVolatageB);
            RawData.Add(RawMainVolatageC);

            RawData.Add(RawJiasuduA);
            RawData.Add(RawJiasuduB);
            RawData.Add(RawJiasuduC);

            RawData.Add(RawLineA);
            RawData.Add(RawLineB);
            RawData.Add(RawLineC);

        }
      
        //public void Clear()
        //{
        //    RawMainVolatageA = null;;//主回路A相电压
        //    RawMainVolatageB = null;;//主回路B相电压
        //    RawMainVolatageC = null;;//主回路C相电压

        //    RawMainCurrentA = null;;//主回路A相电流 
        //    RawMainCurrentB = null;;//主回路B相电流 
        //    RawMainCurrentC = null;;//主回路C相电流 

        //    RawMainDuankouVolatageA = null;;//主回路A相断口电压
        //    RawMainDuankouVolatageB = null;;//主回路B相断口电压
        //    RawMainDuankouVolatageC = null;;//主回路C相断口电压

        //    RawXianquanCurrent = null;;//线圈电流A

        //    RawJiasuduA = null;;//A相 加速度
        //    RawJiasuduB = null;;//B相 加速度
        //    RawJiasuduC = null;;//C相 加速度

        //    RawLineA = null;;//A相 位移
        //    RawLineB = null;;//B相 位移
        //    RawLineC = null;;//C相 位移
        //}

        
       

    }

     
}
