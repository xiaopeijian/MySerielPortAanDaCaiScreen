using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO.Ports;
using System.Collections.Generic;

namespace PortChat
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        List<PatientsAndProtocolsList> protocolsList = new List<PatientsAndProtocolsList>();

        #region 包头包尾指令
        //列表
        byte[] bytHomeUserListConstantData              = new byte[] { 0xEE, 0xB1, 0x5B, 0x00, 0x01, 0x00, 0x1B, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
        byte[] bytStudyOptionalProtocolListConstantData = new byte[] { 0xEE, 0xB1, 0x5B, 0x00, 0x02, 0x00, 0x05, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
        byte[] bytStudySelectedBodyListConstantData     = new byte[] { 0xEE, 0xB1, 0x5B, 0x00, 0x02, 0x00, 0x22, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
        //高压
        byte[] bytStudyExposeKvConstantData             = new byte[] { 0xEE, 0xB1, 0x10, 0x00, 0x02, 0x00, 0x2F, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
        //mA
        byte[] bytStudyExposemAConstantData             = new byte[] { 0xEE, 0xB1, 0x10, 0x00, 0x02, 0x00, 0x31, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
        //ms
        byte[] bytStudyExposemsConstantData             = new byte[] { 0xEE, 0xB1, 0x10, 0x00, 0x02, 0x00, 0x35, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
        //mAs
        byte[] bytStudExposemAsConstantData             = new byte[] { 0xEE, 0xB1, 0x10, 0x00, 0x02, 0x00, 0x38, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
        #endregion

        //总包
        byte[] bytListDataSend = new byte[0];
        byte[] bytStudyOptionalProtocolListData = new byte[0];
        byte[] bytStudyOptionalBodyListData = new byte[0];
        byte[] bytStudyExposeKvData = new byte[0];
        byte[] bytStudyExposemAData = new byte[0];
        byte[] bytStudyExposemsData = new byte[0];
        byte[] bytStudExposemAsData = new byte[0];

        //数据
        string strExposeKvVar = "222";
        string strExposemaVar = "444";
        string strExposemsVar = "666";
        string strExposemasVar = "888";

        //获取可选体位列表数据
        string strProtocolName1 = "协议1";
        string strProtocolNum1 = "名称1";
        string strProtocolName2 = "协议2";
        string strProtocolNum2 = "名称2";
        string strProtocolName3 = "--协faf议2";
        string strProtocolNum3 = "-77名称faf2";
 
        //串口
        private static readonly SerialPort SerialPortObj = new SerialPort();
        string _recievedData = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 串口连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if ((BtnConnect.Content as string) == "连接")
            {
                //端口 默认COM3
                SerialPortObj.PortName = CbbPortName.Text;
                //波特率 默认9600
                SerialPortObj.BaudRate = int.Parse(CbbBaudRate.Text);
                //奇偶校验 默认None
                SerialPortObj.Parity = (Parity)Enum.Parse(typeof(Parity), CbbPortParity.Text, true);
                //数据位 默认8
                SerialPortObj.DataBits = int.Parse(TbDataBits.Text);
                //停止位 默认1
                SerialPortObj.StopBits = (StopBits)Enum.Parse(typeof(StopBits), CbbStopBits.Text, true);
                //握手方式 默认None
                SerialPortObj.Handshake = (Handshake)Enum.Parse(typeof(Handshake), CbbHandshake.Text, true);

                //SerialPortObj.ReceivedBytesThreshold = 1;
                //SerialPortObj.ReadTimeout = 500;//超过ReadTimeout未接收到，则抛出异常。
                //SerialPortObj.WriteTimeout = 500;

                if (!SerialPortObj.IsOpen)
                {
                    SerialPortObj.Open();
                }

                BtnConnect.Content = "断开";
                BtnConnect.Background = Brushes.Green;

                //委托、调用数据接收方法
                SerialPortObj.DataReceived += Recieve;
            }
            else
            {
                try // just in case serial port is not open could also be acheved using if(serial.IsOpen)
                {
                    SerialPortObj.Close();
                    BtnConnect.Content = "连接";
                    BtnConnect.Background = Brushes.White;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private delegate void UpdateUiTextDelegate(string text);

        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            //开辟接收缓冲区
            byte[] bytReceiveData = new byte[SerialPortObj.BytesToRead];
            SerialPortObj.Read(bytReceiveData, 0, bytReceiveData.Length); //从串口读取数据

            //一旦接收彩屏的曝光指令，PC马上返回当前的曝光值

            SendExposeValues(bytReceiveData);
                
            //调用委托，将字符信息显示在TextBox控件上。
            _recievedData = System.Text.Encoding.Default.GetString(bytReceiveData);
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteDate), _recievedData);
        }

        /// <summary>
        /// 发送曝光信息给大彩屏
        /// </summary>
        /// <param name="ExposeValuesVar"></param>
        private void SendExposeValues(byte[] ExposeValuesVar)
        {
            //PC接收来自彩屏的曝光指令
            int iExpose_ReceiveFlag = 0;

            //曝光指令集
            int iKvPlus_ReceiveFlag = 0;
            int iKvReduce_ReceiveFlag = 1;
            int imAPlus_ReceiveFlag = 2;
            int imAReduces_ReceiveFlag = 3;
            int imsPlus_ReceiveFlag = 4;
            int imsReduce_ReceiveFlag = 5;
            int imAsPlus_ReceiveFlag = 6;
            int imAsReduce_ReceiveFlag = 7;

            byte[] bytReceiveData = new byte[4] { 0,0,0,0 };
            bytReceiveData[0] = ExposeValuesVar[0];

            iExpose_ReceiveFlag = System.BitConverter.ToInt32(bytReceiveData, 0);//字节转成整数
            if (iExpose_ReceiveFlag == iKvPlus_ReceiveFlag)//KV 加指令：当前的高压值执行加操作，并返回加后的值给iPads
            {
                protocolsList.Clear();//把之前的数据清零
                protocolsList.Add(new PatientsAndProtocolsList(strExposeKvVar));
                bytListDataSend = GetStringToHex(protocolsList, bytStudyExposeKvConstantData);
                SerialCmdSend(bytListDataSend);
                //SerialCmdSend(bytStudyExposeKvConstantData);
            }
            else if (iExpose_ReceiveFlag == iKvReduce_ReceiveFlag)//KV减
            {

            }
            else if (iExpose_ReceiveFlag == imAPlus_ReceiveFlag)//mA加
            {

            }
            else if (iExpose_ReceiveFlag == imAReduces_ReceiveFlag)//mA减
            {

            }
            else if (iExpose_ReceiveFlag == imsPlus_ReceiveFlag)//ms加
            {

            }
            else if (iExpose_ReceiveFlag == imsReduce_ReceiveFlag)//ms减
            {

            }
            else if (iExpose_ReceiveFlag == imAsPlus_ReceiveFlag)//mAs加
            {

            }
            else if (iExpose_ReceiveFlag == imAsReduce_ReceiveFlag)//mAs减
            {

            }


                
           

        }


        private void WriteDate(string text)
        {
            TbReceive.Text += _recievedData;
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendData_Click(object sender, RoutedEventArgs e)
        {
            //添加列表信息
            protocolsList.Add(new PatientsAndProtocolsList(strProtocolName1, strProtocolNum1));
            protocolsList.Add(new PatientsAndProtocolsList(strProtocolName2, strProtocolNum2));
            protocolsList.Add(new PatientsAndProtocolsList(strProtocolName3, strProtocolNum3));

            //病人检查记录发送
            bytListDataSend = GetStringToHex(protocolsList, bytHomeUserListConstantData);
            SerialCmdSend(bytListDataSend);

            TbSend.Text = System.Text.Encoding.Default.GetString(bytListDataSend); 
            //可选体位发送
            bytListDataSend = GetStringToHex(protocolsList, bytStudyOptionalProtocolListConstantData);
            SerialCmdSend(bytListDataSend);

            //已选体位发送
            bytListDataSend = GetStringToHex(protocolsList, bytStudySelectedBodyListConstantData);
            SerialCmdSend(bytListDataSend);


        }

        /// <summary>
        /// 字符转16进制，16进制保存到字节
        /// </summary>
        /// <param name="str"></param>
        /// <param name="bytHeadTailPackageVar"></param>
        /// <returns></returns>
        private byte[] GetStringToHex( List<PatientsAndProtocolsList> protocolsList, byte[] bytHeadTailPackageVar)
        {
            //总长度
            //int iTotalPackageLong = 0;
            ////包头、包尾长度
            int iHeadPackageLong = 7;
            int iTailPackageLong = 4;
            ////列表行：高八位
            //int iRowHighSize = 0x00;
            ////列表行：低八位
            //int iRowLowSize = 0x02;
            ////列表行：高八位
            //int iColumnHighSize = 0;//每一行的每列字节数都不固定
            ////列表行：低八位
            //int iColumnLowSize = 0;//每一行的每列字节数都不固定

            byte[] bytTotalPackageVar = new byte[500];
            int protocolCount = 0;   

            //拷贝包头:7个固定的字节
            Array.ConstrainedCopy(bytHeadTailPackageVar, 0, bytTotalPackageVar, 0, iHeadPackageLong);

            #region  确定总行数，及其对应的字节  
            Int16 iRowsNum = 0;//总行数
            iRowsNum = (Int16)protocolsList.Count;
            byte[] byt_iRowsNum = new byte[2];
            byt_iRowsNum = BitConverter.GetBytes(iRowsNum);//一般是两个字节
            Array.ConstrainedCopy(byt_iRowsNum, 1, bytTotalPackageVar, iHeadPackageLong, 1);//byt_iRowsNum的高八位，放到缓存区的低字节区。才符合大彩屏的指令接收要求。
            Array.ConstrainedCopy(byt_iRowsNum, 0, bytTotalPackageVar, (iHeadPackageLong + 1), 1);//byt_iRowsNum的低八位，放到缓存区的高字节区。
            #endregion


            #region 每行的字符内容，及其对应的字节数目、字节内容
            int iTotalPackageStartIndex = iHeadPackageLong + byt_iRowsNum.Length;
            while (protocolCount < protocolsList.Count)
            {
                string strProtocolNumVar = null;
                string strProtocolNameVar = null;
                string strEachRowContent = null;
                Int16 iEachRowByteNum = 0;//每行的字节数目
                byte[] byt_strEachRowContent = new byte[0];
                byte[] byt_iEachRowByteNum = new byte[0];

                #region  确定每行的字符内容，及其对应的字节数目、字节内容
                //字符
                strProtocolNumVar = protocolsList[protocolCount].StrPatientNumOrProtocolNum;
                strProtocolNameVar = protocolsList[protocolCount].StrPatientNameOrProtocolName;
                strEachRowContent = strProtocolNumVar + ";" + strProtocolNameVar + ";";//必须加上分号。

                //内容
                byt_strEachRowContent = System.Text.Encoding.Default.GetBytes(strEachRowContent); //每行的字节内容 
                iEachRowByteNum = (Int16)byt_strEachRowContent.Length;
                byt_iEachRowByteNum = BitConverter.GetBytes(iEachRowByteNum);//每行的字节数目，将数目转成字节 
                
                //拷贝
                Array.ConstrainedCopy(byt_iEachRowByteNum, 1, bytTotalPackageVar, iTotalPackageStartIndex, 1);//byt_iEachRowByteNum的高八位，放到缓存区的低字节区。才符合大彩屏的指令接收要求。
                Array.ConstrainedCopy(byt_iEachRowByteNum, 0, bytTotalPackageVar, (iTotalPackageStartIndex + 1), 1);//byt_iEachRowByteNum的低八位，放到缓存区的高字节区。

                Array.ConstrainedCopy(byt_strEachRowContent, 0, bytTotalPackageVar, (iTotalPackageStartIndex + byt_iEachRowByteNum.Length), byt_strEachRowContent.Length);
               
                //下标索引
                iTotalPackageStartIndex = iTotalPackageStartIndex + byt_iEachRowByteNum.Length + byt_strEachRowContent.Length;
                #endregion

                protocolCount++;
            }
            #endregion 

            //拷贝包尾:4个固定的字节
            Array.ConstrainedCopy(bytHeadTailPackageVar, iHeadPackageLong, bytTotalPackageVar, iTotalPackageStartIndex, iTailPackageLong);
            return bytTotalPackageVar;
        }

        /// <summary>
        /// 串口发送函数
        /// </summary>
        /// <param name="data"></param>
        private void SerialCmdSend(byte[] data)
        {
            //若是串口没打开，跳出函数 SerialCmdSend
            if (!SerialPortObj.IsOpen) return;
            try
            {
                //将待发送的内容写到缓冲区
                SerialPortObj.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {

                TbSend.Text = "Failed to SEND" + data + "\n" + ex + "\n";
            }
        }
    }
}
