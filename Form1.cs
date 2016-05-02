//OPEN JUMPER 奈何col！
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Windows.Controls.Primitives;


namespace SPA
{
    public partial class OJ_Serial : Form
    {
        string caption = "OPEN JUMPER Serial Assistant";
        //string InputData = String.Empty;
        int TXL, RXL;
        string[] Wkey = { "q", "w", "e", "a", "s", "d", "u", "i", "o", "j", "k", "l", " " };
        string buttonName;
        string[] ButtonList = { "buttonQ", "buttonW", "buttonE", "buttonA", "buttonS", "buttonD", "buttonU", "buttonI", "buttonO", "buttonJ", "buttonK", "buttonL", "buttonSpace" };
        //Stream htmlbox;

        //初始化
        public OJ_Serial()
        {
            InitializeComponent();
            NewSerial();
            Control.CheckForIllegalCrossThreadCalls = false;//没搞懂
            comboBox1.Text = "9600";
            comboBox2.Text = "8";
            comboBox3.Text = "无";
            comboBox4.Text = "1";

            //RX_Browser.DoubleBuffered = true;
            //RX_Browser.DocumentStream = htmlbox;

        }
        //刷新获取串口
        public void NewSerial()
        {
            //获取当前计算机的串行端口名称数组
            string[] ports = SerialPort.GetPortNames();
            //将数组添加到NameOfSerial
            foreach (string port in ports)
            {
                NameOfSerial.Items.Add(port);
                Serial_info.Items.Add(port);
            }
            if (ports.Length != 0)
            {
                NameOfSerial.Text = ports[0];
                //Serial_info.Text = ports[0];
            }
            else
            {
                NameOfSerial.Text = "请选择串口";
                MessageBox.Show("哎呀，没找到串口设备哦，亲~", "OJ调试器", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //刷新按钮
        private void button1_Click(object sender, EventArgs e)
        {
            NameOfSerial.Items.Clear();
            NewSerial();
        }
        //打开串口
        private void button2_Click(object sender, EventArgs e)
        {
            this.tabControl.Focus();
            try
            {
                if (_SerialPort.IsOpen)
                {
                    _SerialPort.Close();
                    Open_or_Close.Text = "打开串口";
                    toolStripStatusLabel_COM.Text = null;
                    toolStripStatusLabel_RX.Text = null;
                    toolStripStatusLabel_TX.Text = null;
                    toolStripStatusLabel_R.Text = null;
                    toolStripStatusLabel_T.Text = null;
                    //RXbox.MustHideCaret = false;

                }
                else
                {
                    if (NameOfSerial.Text != "无")
                    {
                        _SerialPort.PortName = NameOfSerial.SelectedItem.ToString();
                        try
                        {
                            _SerialPort.Open();
                            toolStripStatusLabel_ERROR.Text = null;
                            toolStripStatusLabel_COM.Text = _SerialPort.PortName + ": " + _SerialPort.BaudRate + " " + _SerialPort.DataBits + " " + _SerialPort.Parity + " " + _SerialPort.StopBits + "     ";
                            toolStripStatusLabel_RX.Text = "RX";
                            toolStripStatusLabel_TX.Text = "TX";
                            RXL = 0; TXL = 0;
                            toolStripStatusLabel_R.Text = RXL.ToString();
                            toolStripStatusLabel_T.Text = TXL.ToString();
                            Open_or_Close.Text = "关闭串口";
                            //RXbox.MustHideCaret = true;
                        }
                        catch
                        {
                            MessageBox.Show("无法打开串口，该串口可能已被其他进程占用");
                        }

                    }
                    else
                    {
                        toolStripStatusLabel_ERROR.Text = "请先选择通信串口号";
                    }
                }
            }
            catch
            {

                toolStripStatusLabel_ERROR.Text = "未知错误";
                NameOfSerial.Items.Clear();
                NewSerial();
                Open_or_Close.Text = "打开串口";
            }

        }
        //16进转换
        public string HEX_To_String(byte[] bytes)
        {
            string HEX = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    HEX += bytes[i].ToString("X2");
                    if (i != bytes.Length)
                        HEX += " ";
                }
            }
            return HEX;
        }
        //16进制转ASCII
        public string HEX_To_ASCII(byte[] bytes)
        {
            string HEX = "";
            if (bytes != null)
            {
                int temp;
                if (flagRN) temp = 1;
                else temp = 0;
                for (int i = temp; i < bytes.Length; i++)
                {
                    if ((bytes[i] == 0x0a) || (bytes[i] == 0x0d))
                        HEX += "\r\n";
                    else
                        HEX += (char)bytes[i];
                }
            }
            return HEX;
        }
        //16进转换
        public byte[] String_To_HEX(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        //用于记录换行符
        Boolean flagRN = false;
        //接收
        private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //RX_backgroundWorker.RunWorkerAsync();
            RX_thread();

        }
        private void RX_backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        //private void Start_RXthread()
        //{
        //    if (RX_backgroundWorker.IsBusy != true) {  }
        //}

        private void RX_thread()
        {
            int buffersize = _SerialPort.BytesToRead;   //十六进制数的大小
            byte[] buffer = new Byte[buffersize];   //创建缓冲区
            _SerialPort.Read(buffer, 0, buffersize);
            if (RX_HEX_checkBox.Checked)
            {
                RXbox.AppendText(HEX_To_String(buffer));
            }
            else
            {
                try
                {
                    RXbox.AppendText(HEX_To_ASCII(buffer));
                    //蛋疼的处理richtextbox
                    //richtextbox中\n \r两者都是换行
                    //if ((buffer[0] == 0x0A) && (flagRN)) { } else { flagRN = false; }
                    
                    //if (buffer[buffer.Length - 1] == 0x0D) { flagRN = true; }
                }
                catch { }

            }
            //if (scroll_checkBox.Checked)
            //{
            //    //RXbox.AutoScrollOffset
            //    //RXbox.view
            //    //RXbox_Scroll.Maximum = RXbox.ViewportHeight;
            //}
            RXL = RXL + buffer.Length;
            toolStripStatusLabel_R.Text = RXL.ToString();
        }

        //发送
        private void _Serialport_tx(string tx_info)
        {
            try
            {
                if (_SerialPort.IsOpen)
                    if (TX_HEX_checkBox.Checked)
                    {
                        HEX_Mode();
                        if (checkBox2.Checked == true)
                        {
                            youSend();
                        }
                    }
                    else
                    {
                        _SerialPort.Write(tx_info);
                        if (checkBox2.Checked == true)
                        {
                            youSend();
                            //textBox1.AppendText("\r\n" + DateTime.Now.ToLongTimeString() + " OpenJumper SPA transmit:" + "\r\n" + textBox2.Text + "\r\n");
                        }
                        TXL = TXL + textBox2.Text.Length;
                        toolStripStatusLabel_T.Text = TXL.ToString();
                    }
                else toolStripStatusLabel_ERROR.Text = "错误:串口未打开，请先打开串口";
            }
            catch
            {
                toolStripStatusLabel_ERROR.Text = "错误:未检测到串口" + NameOfSerial.Text;
                toolStripStatusLabel_COM.Text = null;
                toolStripStatusLabel_RX.Text = null;
                toolStripStatusLabel_TX.Text = null;
                toolStripStatusLabel_R.Text = null;
                toolStripStatusLabel_T.Text = null;
                //_SerialPort.Close();
                Open_or_Close.Text = "打开串口";
            }
        }

        private void youSend()
        {
            //RXbox.SelectionColor = Color.Red;
            RXbox.AppendText("\r\n" + DateTime.Now.ToLongTimeString() + " You Send : " + textBox2.Text + "\r\n");
            //RXbox.SelectionColor = Color.Black;
        }
        //发送按钮
        private void button1_Click_1(object sender, EventArgs e)
        {
            _Serialport_tx(textBox2.Text);

        }
        //自动发送
        private void timer1_Tick(object sender, EventArgs e)
        {
            _Serialport_tx(textBox2.Text);
        }
        //清空接收区
        private void clear_button_Click(object sender, EventArgs e)
        {
            RXbox.Clear();
        }
        //更新底部串口参数显示
        private void statusStrip_Update()
        {
            if (_SerialPort.IsOpen)
            {
                toolStripStatusLabel_COM.Text = _SerialPort.PortName + ": " + _SerialPort.BaudRate + " " + _SerialPort.DataBits + " " + _SerialPort.Parity + " " + _SerialPort.StopBits + "     ";
            }
        }
        //设置数据位
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SerialPort.DataBits = int.Parse(comboBox2.SelectedItem.ToString());
            statusStrip_Update();
        }
        //设置停止位
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            string temp = comboBox4.SelectedItem.ToString();
            switch (temp)
            {
                case "1":
                    _SerialPort.StopBits = StopBits.One;
                    break;
                //case "1.5":
                // _SerialPort.StopBits = StopBits.OnePointFive;
                // break;
                case "2":
                    _SerialPort.StopBits = StopBits.Two;
                    break;
            }
            statusStrip_Update();
        }
        //设置波特率
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SerialPort.BaudRate = int.Parse(comboBox1.SelectedItem.ToString());
            statusStrip_Update();
        }
        //设置校验位
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SerialPort.Parity = Parity.None;
            string temp = comboBox3.SelectedItem.ToString();
            switch (temp)
            {
                case "无":
                    _SerialPort.Parity = Parity.None;
                    break;
                case "偶":
                    _SerialPort.Parity = Parity.Even;
                    break;
                case "奇":
                    _SerialPort.Parity = Parity.Odd;
                    break;
                case "标志":
                    _SerialPort.Parity = Parity.Mark;
                    break;
                case "空格":
                    _SerialPort.Parity = Parity.Space;
                    break;
            }
            statusStrip_Update();
        }
        //广告
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.openjumper.com");
        }
        //logo广告
        private void OPENJUMPER_logo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.openjumper.com");
        }
        //开启自动发送
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                timer1.Enabled = true;
                try
                {
                    timer1.Interval = Convert.ToInt32(textBox4.Text);
                    toolStripStatusLabel_ERROR.Text = null;
                }
                catch
                {
                    toolStripStatusLabel_ERROR.Text = "错误:您输入发送时间可能含有字符";
                }
            }
            else timer1.Enabled = false;
        }
        //判断选项卡
        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            TabControl page = (TabControl)sender;
            if (page.SelectedTab.Text == "键盘模式")
            {
                int i = 0;
                foreach (string temp in ButtonList)
                {
                    string ConString = System.Configuration.ConfigurationManager.AppSettings[temp];
                    Wkey[i] = ConString;
                    i++;
                }
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                KeyPress += OJ_Serial_KeyPress;
            }
            else
                KeyPress -= OJ_Serial_KeyPress;
            if (page.SelectedTab.Text == "Hex下载")
            {
                SerialGroupBox.Visible = false;
            }
            else SerialGroupBox.Visible = true;
        }

        //计数器显示更新
        void Update_TX_Show(string Wkey)
        {
            TXL = TXL + Wkey.Length;
            toolStripStatusLabel_T.Text = TXL.ToString();
        }
        //获取软件更新
        private void Get_update(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://x.openjumper.com/serial/");
        }

        private void label17_Click(object sender, EventArgs e)
        {
            MessageBox.Show("作者：奈何col  From OpenJumper\r\nbug报告及建议提交E-mail： zhou@openjumper.com", caption);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RXbox.WordWrap = !RXbox.WordWrap;
        }

        private void HEX_Mode()
        {
            bool IsHex = false;
            string inputHEX = textBox2.Text;
            //if (textBox2.Text.Contains(" "))
            //inputHEX = inputHEX.Replace(" ", " ");
            if (inputHEX.Contains("0X"))
                inputHEX = inputHEX.Replace("0X", " ");
            if (inputHEX.Contains("0x"))
                inputHEX = inputHEX.Replace("0x", " ");
            if (inputHEX.Contains("\r"))
                inputHEX = inputHEX.Replace("\r", " ");
            if (inputHEX.Contains("\n"))
                inputHEX = inputHEX.Replace("\n", " ");
            //foreach (char T1char in textBox2.Text)
            foreach (char Tchar in inputHEX)
                if (((Tchar >= 48) && (Tchar <= 57)) || ((Tchar >= 65) && (Tchar <= 70)) || ((Tchar >= 97) && (Tchar <= 102)) || (Tchar == 32))
                {
                    IsHex = true;
                    toolStripStatusLabel_ERROR.Text = null;
                }
                else
                {
                    IsHex = false;
                    toolStripStatusLabel_ERROR.Text = "错误:您输入的可能不是纯16进制数";
                    break;
                }
            if (IsHex == true)
                try
                {
                    byte[] HEX = String_To_HEX(inputHEX);
                    _SerialPort.Write(HEX, 0, HEX.Length);
                    TXL = TXL + HEX.Length;
                    toolStripStatusLabel_T.Text = TXL.ToString();
                }
                catch
                {
                    toolStripStatusLabel_ERROR.Text = "错误:您输入的可能不是纯16进制数";
                }
        }


        private void button_D_Click(object sender, EventArgs e)
        {
            _Serialport_tx("D");
        }

        private void button_E_Click(object sender, EventArgs e)
        {
            _Serialport_tx("E");
        }

        private void button_F_Click(object sender, EventArgs e)
        {
            _Serialport_tx("F");
        }

        private void button_G_Click(object sender, EventArgs e)
        {
            _Serialport_tx("G");
        }
        Timer Dtr_timer = new Timer();
        private void Reset_button_Click(object sender, EventArgs e)
        {
            _SerialPort.DtrEnable = true;
            _SerialPort.RtsEnable = true;
            //Timer Dtr_timer = new Timer();
            Dtr_timer.Interval = 20;
            Dtr_timer.Tick += Dtr_timer_Tick;
            Dtr_timer.Enabled = true;
        }

        void Dtr_timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            _SerialPort.DtrEnable = false;
            _SerialPort.RtsEnable = false;
            //Dtr_timer.Stop();
            Dtr_timer.Enabled = false;
        }

        private void OJ_Serial_FormClosing(object sender, FormClosingEventArgs e)
        {
            _SerialPort.Close();
        }

        private void NameOfSerial_DropDown(object sender, EventArgs e)
        {
            NameOfSerial.Items.Clear();
            NewSerial();
        }
        //自动滚动
        private void scroll_checkBox_CheckedChanged(object sender, EventArgs e)
        {

        }


        bool RXbox_AutoScroll = true;
        private void autoScroll_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            RXbox_AutoScroll = !RXbox_AutoScroll;
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void label19_Click(object sender, EventArgs e)
        {

        }


    }
}

