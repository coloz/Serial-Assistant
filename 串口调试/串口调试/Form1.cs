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


namespace 串口调试
{
    public partial class Form1 : Form
    {
        string InputData = String.Empty;
        int TXL, RXL;
        string[] Wkey={"q","w","e","a","s","d","u","i","o","j","k","l"," "};
        string buttonName;
        string[] ButtonList = { "buttonQ", "buttonW", "buttonE", "buttonA", "buttonS", "buttonD", "buttonU", "buttonI", "buttonO", "buttonJ", "buttonK", "buttonL", "buttonSpace" };
        //初始化
        public Form1()
        {
            InitializeComponent();
            NewSerial();
            Control.CheckForIllegalCrossThreadCalls = false;//没搞懂
            comboBox1.Text = "9600";
            comboBox2.Text = "8";
            comboBox3.Text = "无";
            comboBox4.Text = "1";
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
            }
            if (ports.Length != 0)
            {
            NameOfSerial.Text = ports[0];
            }
            else 
            {
                NameOfSerial.Text = "请选择串口";
                MessageBox.Show("哎呀，没找到串口设备哦，亲~", "OJ调试器", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //16进制显示
        public void ToHEX(string input)
        {
            string hexOutput="";
            char[] values = input.ToCharArray();
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                hexOutput = String.Format("{0:X}", value);
                //_SerialPort.Write(hexOutput+" ");
                textBox1.Paste(hexOutput + " ");
                RXL = RXL + 1;
                toolStripStatusLabel_R.Text = RXL.ToString();
            }
            
        }
        //16进发送
        public void HEXTo(string input)
        {
            string hexValues = input;
            string[] hexValuesSplit = hexValues.Split(' ');
            foreach (String hex in hexValuesSplit)
            {
                // Convert the number expressed in base-16 to an integer.
                int value = Convert.ToInt32(hex, 16);
                // Get the character corresponding to the integral value.
                string stringValue = Char.ConvertFromUtf32(value);
                char charValue = (char)value;
                _SerialPort.Write(stringValue);
                TXL = TXL + 1;
                toolStripStatusLabel_T.Text = TXL.ToString();
                
            }


        }
        //刷新按钮
        private void CheckCom() 
        {
            NameOfSerial.Items.Clear();
            NewSerial();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CheckCom();
        }
        //打开串口
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (_SerialPort.IsOpen)
                {
                    _SerialPort.Close();
                    开关串口.Text = "打开串口";
                    toolStripStatusLabel_COM.Text = null;
                    toolStripStatusLabel_RX.Text = null;
                    toolStripStatusLabel_TX.Text = null;
                    toolStripStatusLabel_R.Text = null;
                    toolStripStatusLabel_T.Text = null;

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
                            开关串口.Text = "关闭串口";
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
                CheckCom();
                开关串口.Text = "打开串口";
            }

        }
        //接收
        private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            InputData = _SerialPort.ReadExisting();
            InputData = InputData.Replace("\n","\r\n");
            if (checkBox7.Checked)
                ToHEX(InputData);
            else
            {
                textBox1.SelectionStart = textBox1.TextLength;
                textBox1.Paste(InputData);
                RXL = RXL + InputData.Length;
                toolStripStatusLabel_R.Text = RXL.ToString();
            }
        }
        //发送
        private void _Serialport_tx()
        {
            bool IsHex = false;
            if (_SerialPort.IsOpen)
                if (checkBox8.Checked)
                {
                    string inputHEX = textBox2.Text;
                    //if (textBox2.Text.Contains(" "))
                    //inputHEX = inputHEX.Replace(" ", " ");
                    if (inputHEX.Contains("0X"))
                        inputHEX = inputHEX.Replace("0X", " ");
                    if (inputHEX.Contains("0x"))
                        inputHEX = inputHEX.Replace("0x", " ");
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
                            break;

                        }
                    if (IsHex == true)
                        try
                        {
                            HEXTo(inputHEX);
                        }
                        catch
                        {
                            toolStripStatusLabel_ERROR.Text = "错误:您输入的可能不是纯16进制数";
                        }
                }
                else
                {
                    _SerialPort.Write(textBox2.Text);
                    TXL = TXL + textBox2.Text.Length;
                    toolStripStatusLabel_T.Text = TXL.ToString();
                }
            else toolStripStatusLabel_ERROR.Text = "错误:串口未打开，请先打开串口";
        }
        //发送按钮
        private void button1_Click_1(object sender, EventArgs e)
        {
            _Serialport_tx();
        }
        //自动发送
        private void timer1_Tick(object sender, EventArgs e)
        {
            _Serialport_tx();
        }
        //清空接收区
        private void button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Clear();
        }
        //更新底部串口参数显示
        private void Update()
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
            Update();
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
            Update();
        }
        //设置波特率
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SerialPort.BaudRate = int.Parse(comboBox1.SelectedItem.ToString());
            Update();
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
         Update();
        }
        //广告2
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.arduino.cn/thread-1183-1-1.html");
        }
        //器件a
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if((checkBox3.Checked == true) && (_SerialPort.IsOpen))
            {
                label9.Text = trackBar1.Value.ToString();
                _SerialPort.Write("A");
                _SerialPort.WriteLine(label9.Text);
            }
        }
        //器件b
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if ((checkBox4.Checked == true) && (_SerialPort.IsOpen))
            {
                label10.Text = trackBar2.Value.ToString();
                _SerialPort.Write("B");
                _SerialPort.WriteLine(label10.Text);
            }
        }
        //器件c
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if ((checkBox5.Checked == true) && (_SerialPort.IsOpen))
            {
                label11.Text = trackBar3.Value.ToString();
                _SerialPort.Write("C");
                _SerialPort.WriteLine(label11.Text);
            }
        }
        
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
                //timer1.Interval
            }
            else timer1.Enabled = false;
        }
        //logo广告
        private void OPENJUMPER_logo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.openjumper.com");
        }

        private void KP_Q(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == 'q') || (e.KeyChar == 'w') || (e.KeyChar == 'e') || (e.KeyChar == 'a') || (e.KeyChar == 's') || (e.KeyChar == 'd') || (e.KeyChar == 'u') || (e.KeyChar == 'i') || (e.KeyChar == 'o') || (e.KeyChar == 'j') || (e.KeyChar == 'k') || (e.KeyChar == 'l'))
            {
                try
                {
                    string Key_temp = e.KeyChar + "\n";
                    _SerialPort.Write(Key_temp);
                }
                catch
                {
                    toolStripStatusLabel_ERROR.Text = "未知错误";
                }
            }
        }
        
        /*控制模式*/
        //读出数据
        private void Read(object sender, TabControlEventArgs e)
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
            }
        }

        //点击button显示对应字符串
        private void WKey_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string ConString = System.Configuration.ConfigurationManager.AppSettings[button.Name];
            Key_t.Text = ConString;
            groupBox4.Text = "自定义" + button.Name;
            buttonName = button.Name;
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }
        //计数器显示更新
        void Update_TX_Show(string Wkey)
        {
            TXL = TXL + Wkey.Length;
            toolStripStatusLabel_T.Text = TXL.ToString();
        }
        //键盘按键发送
        private void Key_tx(object sender, KeyPressEventArgs e)
        {
            //判断控制模式选项卡是否最上层
            if ((tabControl.SelectedTab == tabPage3)&&(_SerialPort.IsOpen)) 
            {
                switch (e.KeyChar)
                {
                    case 'q':
                        _SerialPort.Write(Wkey[0]);
                        Update_TX_Show(Wkey[0]);
                        break;
                    case 'w':
                        _SerialPort.Write(Wkey[1]);
                        Update_TX_Show(Wkey[1]);
                        break;
                    case 'e':
                        _SerialPort.Write(Wkey[2]);
                        Update_TX_Show(Wkey[2]);
                        break;
                    case 'a':
                        _SerialPort.Write(Wkey[3]);
                        Update_TX_Show(Wkey[3]);
                        break;
                    case 's':
                        _SerialPort.Write(Wkey[4]);
                        Update_TX_Show(Wkey[4]);
                        break;
                    case 'd':
                        _SerialPort.Write(Wkey[5]);
                        Update_TX_Show(Wkey[5]);
                        break;
                    case 'u':
                        _SerialPort.Write(Wkey[6]);
                        Update_TX_Show(Wkey[6]);
                        break;
                    case 'i':
                        _SerialPort.Write(Wkey[7]);
                        Update_TX_Show(Wkey[7]);
                        break;
                    case 'o':
                        _SerialPort.Write(Wkey[8]);
                        Update_TX_Show(Wkey[8]);
                        break;
                    case 'j':
                        _SerialPort.Write(Wkey[9]);
                        Update_TX_Show(Wkey[9]);
                        break;
                    case 'k':
                        _SerialPort.Write(Wkey[10]);
                        Update_TX_Show(Wkey[10]);
                        break;
                    case 'l':
                        _SerialPort.Write(Wkey[11]);
                        Update_TX_Show(Wkey[11]);
                        break;
                    case ' ':
                        _SerialPort.Write(Wkey[12]);
                        Update_TX_Show(Wkey[12]);
                        break;
                }
            }
            
        }
        //保存自定义键值
        private void Save_Click(object sender, EventArgs e)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings[buttonName].Value = Key_t.Text;
            cfa.Save();
            int i = 0;
            foreach (string temp in ButtonList)
            {
                string ConString = System.Configuration.ConfigurationManager.AppSettings[temp];
                Wkey[i] = ConString;
                i++;
            }
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        private void label6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.arduino.cn/thread-1183-1-1.html");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.arduino.cn/thread-1183-1-1.html");
        }

    }
}
