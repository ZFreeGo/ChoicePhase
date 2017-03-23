
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZFreeGo.ChoicePhase.ControlUI.CommCenter;
using ZFreeGo.ChoicePhase.Modbus;

namespace ZFreeGo.ChoicePhase.ControlCenter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
       //private int sampleLen = 128*10*2; // 过零检测长度
        private int sampleLen = 128  * 2; //FFT 长度
       // private int tpLen = 16 * 2; //周期数据长度
        List<SampleDataBase> downComputeSampleData;
        List<double> realCalArray;
        SampleDataBase sampleData;

        private double hezhaPhase = 0; //合闸相角 弧度


        double xcenter = 120;
        double ycenter = 90;
        int pointNum = 120;

        private SerialControlCenter serialControlCenter; //用于DSP通讯
        private SerialControlCenter yongciSerialControlCenter;//用于永磁通讯
        private SystemStateMessage systemState; //用于状态显示

        private byte NowAck = 0; //当前ACK应答值
        
        public MainWindow()
        {
            InitializeComponent();

            
            
            baseFrame = new RTUFrame(downComputeAddress, (byte)FunEnum.None);

            downComputeSampleData = new List<SampleDataBase>();
            sampleData = new SampleDataBase(sampleLen, "原始采样数据");
            downComputeSampleData.Add(sampleData);
            
            realCalArray = new List<double>();
            realCalArray.Add(0);
            realCalArray.Add(1);
            realCalArray.Add(2);
            realCalArray.Add(3);
            realCalArray.Add(4);

            //曲线显示与转换
            setHezhaPhase.TextChanged += setHezhaPhase_TextChanged;
            PlotSineWave();
            jiaoDuRadio.IsChecked = true;

            NowAck = 0; //初始化为0
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //状态信息栏
            systemState = new SystemStateMessage();
            systemStateGroup.DataContext = systemState;


            InitDownMcuComb(); //下位机选择

            //用于DSP通讯
            serialControlCenter = new SerialControlCenter(downComputeAddress);
            UpdatePortShow(serialControlCenter.SerialPort);
            serialControlCenter.RtuFrameArrived += serialControlCenter_RtuFrameArrived;
            //用于永磁通讯
            yongciSerialControlCenter = new SerialControlCenter(yongcidownComputeAddress);
            UpdatePortShowB(yongciSerialControlCenter.SerialPort);
            yongciSerialControlCenter.RtuFrameArrived += yongciSerialControlCenter_RtuFrameArrived;
            
            InitLedSelected(); //永磁 LED下拉选择
            InitYongciControlComb(); //永磁下位机
            initYongciOperateSeleted();//开关操作下拉菜单 
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            //释放资源
            serialControlCenter.CloseCenter();
            yongciSerialControlCenter.CloseCenter();
        }
        private void InitDownMcuComb()
        {
            downMcuSelected.Items.Add("DSP控制器");       //0xA1
            downMcuSelected.Items.Add("永磁控制器A");      //0xA2
            downMcuSelected.SelectedItem = downMcuSelected.Items[0];
        }
        private void InitYongciControlComb()
        {

            yongciSelectedComb.Items.Add("永磁控制器A");       //0xA1
            yongciSelectedComb.Items.Add("永磁控制器B");      //0xA2
            yongciSelectedComb.Items.Add("永磁控制器C"); 
            yongciSelectedComb.SelectedItem = yongciSelectedComb.Items[0];
        }
        private void InitLedSelected()
        {
            ledSelected.Items.Add("LED1");
            ledSelected.Items.Add("LED2");
            ledSelected.Items.Add("LED3");
            ledSelected.Items.Add("LED4");
            ledSelected.Items.Add("LED5");
            ledSelected.Items.Add("LED6");
            ledSelected.Items.Add("LED7");
            ledSelected.Items.Add("LED8");
 
            ledSelected.SelectedItem = ledSelected.Items[0];
        }

        private void initYongciOperateSeleted()
        {
            operateLoopSelected.Items.Add("C-t1-O-t2-");//0
            operateLoopSelected.Items.Add("O-0.3-CO-t1-C-t2-");//1
            operateLoopSelected.Items.Add("CO-t1-CO-t2-CO");//2
            operateLoopSelected.SelectedItem = operateLoopSelected.Items[0];
        }
        private void SaveDataToFile(SampleDataBase rawData, string path)
        {

            //此方法是每次获取后立刻写入磁盘文件，亦可以考虑先保存数据待全部接收完毕，一起写入磁盘文件。
            //文件名

            FileStream saveFileStream = new FileStream(path, FileMode.Create,
             FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter charStream = new StreamWriter(saveFileStream);
            int len = rawData.RawData.Length;
            for (int i = 0; i < len; i += 2)
            {
                var str = (rawData.RawData[i] + rawData.RawData[i + 1] * 256).ToString();
                charStream.WriteLine(str);
            }
            if (realCalArray.Count > 0)
            {
                foreach (var ele in realCalArray)
                {
                    charStream.WriteLine(ele.ToString());
                }
            }

            charStream.Flush();
            charStream.Close();
        }
        private void SaveSampleData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = DateTime.Now.ToLongTimeString().Replace(':', '_') + ".txt"; // Default file name
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.FilterIndex = 1;
                dlg.Filter = "Text documents (.txt)|*.txt|Hex documents (.hex)|*.hex|All files (*.*)|*.*"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string saveFilePath = dlg.FileName;
                 
                    //var saveFilePath = new FileStream(filename, FileMode.Create,
                    // FileAccess.ReadWrite, FileShare.ReadWrite);

                    SaveDataToFile(downComputeSampleData[0], saveFilePath);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("新建保存文件::" + ex.Message);
            }
        }

        private void clearStateMessage_Click(object sender, RoutedEventArgs e)
        {
            //reciveStataTxt.Text = "";
            historyRecord.Items.Clear();
            //nextIndex = 250;
            
        }


        private void fftRadioButton_Click(object sender, RoutedEventArgs e)
        {
                sampleLen = 128 * 2;
                if (downComputeSampleData != null)
                {
                    downComputeSampleData.Remove(sampleData);
                    sampleData = new SampleDataBase(sampleLen, "原始采样数据");
                    downComputeSampleData.Add(sampleData);
                }

        }

        private void zvdRaio_Checked(object sender, RoutedEventArgs e)
        {
            sampleLen = 128 * 10 * 2;
            if (downComputeSampleData != null)
            {
                downComputeSampleData.Remove(sampleData);
                sampleData = new SampleDataBase(sampleLen, "原始采样数据");
                downComputeSampleData.Add(sampleData);
            }
        }
        private void setHezhaPhase_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Convert.ToDouble
            double phase = 0;
            if (Double.TryParse(setHezhaPhase.Text, out phase))
            {
                double angle = 0;
                double rate = 1;


                //选择角度模式
                if (jiaoDuRadio.IsChecked == true)
                {
                    angle = phase * 2 * Math.PI / 360; //角度转化为弧度

                }
                else
                {
                    angle = phase;

                }

                angle = angle % (2 * Math.PI);
                //若小于0，则周期化整
                if (angle < 0)
                {
                    angle = 2 * Math.PI + angle;
                }

                rate = angle / (2 * Math.PI); //计算比例
                if (lineCursor != null)
                {
                    lineCursor.X2 = lineCursor.X1 = pointNum * rate + xcenter / 2;
                    roundCursor.X2 = 80 * Math.Cos(angle) + xcenter;
                    roundCursor.Y2 = -80 * Math.Sin(angle) + ycenter;
                }

                hezhaPhase = angle;

            }
            else
            {
                // MessageBox.Show("请输入浮点数");
            }


        }
        private void PlotSineWave()
        {

            int i = 0;

            var array = new Point[pointNum + 1];

            for (i = 0; i < pointNum + 1; i++)
            {
                array[i] = new Point((double)i + xcenter / 2, ycenter - 50 * Math.Sin((double)i / (0.5 * pointNum) * Math.PI));
                sinWave.Points.Add(array[i]);
            }

        }
        private void jiaoDuRadio_Checked(object sender, RoutedEventArgs e)
        {
            setHezhaPhase_TextChanged(null, null);
        }

        private void huDuRadio_Checked(object sender, RoutedEventArgs e)
        {
            setHezhaPhase_TextChanged(null, null);
        }

 
      

       

       

      

        

       

   
      

     


       

 




       


   
      
       
       



       
  
       
    }
}
