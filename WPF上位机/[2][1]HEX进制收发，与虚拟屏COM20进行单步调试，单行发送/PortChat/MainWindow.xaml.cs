using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO.Ports;


namespace PortChat
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        //串口
        private static readonly SerialPort SerialPortObj = new SerialPort();
        string _recievedData;

        public MainWindow()
        {
            InitializeComponent();
        }

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
                
                SerialPortObj.ReadTimeout = 500;
                SerialPortObj.WriteTimeout = 500;

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
            // 收集缓存中的信息
            _recievedData = SerialPortObj.ReadLine();
            //调用委托，将字符信息显示在TextBox控件上。
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteDate), _recievedData);
        }

        private void WriteDate(string text)
        {
            TbReceive.Text += _recievedData;
        }

        private void SendData_Click(object sender, RoutedEventArgs e)
        {
            //包头包尾
            //Home列表
            byte[] bytHomeUserListConstantData              = new byte[] { 0xEE, 0xB1, 0x52, 0x00, 0x01, 0x00, 0x1B, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
            //Study列表1
            byte[] bytStudyOptionalProtocolListConstantData = new byte[] { 0xEE, 0xB1, 0x52, 0x00, 0x02, 0x00, 0x05, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
            //Study列表2
            byte[] bytStudySelectedBodyListConstantData     = new byte[] { 0xEE, 0xB1, 0x52, 0x00, 0x02, 0x00, 0x21, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
            //高压
            byte[] bytStudyExposeKvConstantData             = new byte[] { 0xEE, 0xB1, 0x10, 0x00, 0x02, 0x00, 0x2F, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
            //mA
            byte[] bytStudyExposemAConstantData             = new byte[] { 0xEE, 0xB1, 0x10, 0x00, 0x02, 0x00, 0x31, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
            //ms
            byte[] bytStudyExposemsConstantData             = new byte[] { 0xEE, 0xB1, 0x10, 0x00, 0x02, 0x00, 0x35, 0xFF, 0xFC, 0xFF, 0xFF };//11个数
            //mAs
            byte[] bytStudExposemAsConstantData             = new byte[] { 0xEE, 0xB1, 0x10, 0x00, 0x02, 0x00, 0x38, 0xFF, 0xFC, 0xFF, 0xFF };//11个数

            //总包
            byte[] bytHomeUserListData = new byte[0];
            byte[] bytStudyOptionalProtocolListData = new byte[0];
            byte[] bytStudyOptionalBodyListData = new byte[0];
            byte[] bytStudyExposeKvData = new byte[0];
            byte[] bytStudyExposemAData = new byte[0];
            byte[] bytStudyExposemsData = new byte[0];
            byte[] bytStudExposemAsData = new byte[0];
            string strALLContentData = null;
            //获取用户列表数据数据并发送
            string strPatientNum = "LM00000011";
            string strPatientName = "刘德华";
            string strPatientSex = "男";
            string strPatientAge = "55";
            string strPatientBody = "肥胖";
            string strStudyDateTime = "2018-8-8";
            //strALLContentData = strPatientNum + ";" + strPatientName + ";"+ strPatientSex + ";" + strPatientAge + ";" + strPatientBody + ";" + strStudyDateTime + ";";
            //bytHomeUserListData = GetStringToHex(strALLContentData, bytHomeUserListConstantData);           
            //SerialCmdSend(bytHomeUserListData);

            //获取可选体位列表数据数据并发送
            string strProtocolName1 = "协议1";
            string strProtocolNum1 = "名称1";
            string strProtocolName2 = "协议2";
            string strProtocolNum2 = "名称2";
            string strProtocolName3 = null;
            string strProtocolNum3 = null;
            strALLContentData = null;
            strALLContentData = strProtocolName1 + ";" + strProtocolNum1 + ";" + strProtocolName2 + ";" + strProtocolNum2 + ";";
            bytHomeUserListData = GetStringToHex(strALLContentData, bytStudyOptionalProtocolListConstantData);
            SerialCmdSend(bytHomeUserListData);

        }

        /// <summary>
        /// 字符转16进制，16进制保存到字节
        /// </summary>
        /// <param name="str"></param>
        /// <param name="bytHeadTailPackageVar"></param>
        /// <returns></returns>
        private byte[] GetStringToHex(string str, byte[] bytHeadTailPackageVar)
        {
            byte[] bytContentDataVar = System.Text.Encoding.Default.GetBytes(str);
            byte[] bytTotalPackageVar = new byte[bytContentDataVar.Length + bytHeadTailPackageVar.Length];
            //拷贝包头:7个固定的字节
            Array.ConstrainedCopy(bytHeadTailPackageVar, 0, bytTotalPackageVar, 0, 7);
            //拷贝包内容:
            Array.ConstrainedCopy(bytContentDataVar, 0, bytTotalPackageVar, 7, bytContentDataVar.Length);
            //拷贝包尾:4个固定的字节
            Array.ConstrainedCopy(bytHeadTailPackageVar, 7, bytTotalPackageVar, (7 + bytContentDataVar.Length), 4);
            return bytTotalPackageVar;
        }

        /// <summary>
        /// 发送数据
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
