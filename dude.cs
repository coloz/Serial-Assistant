using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.ComponentModel;
using System.Threading;

namespace SPA
{
    public partial class OJ_Serial
    {
        
        Process avrdude = new Process();

        //private OJ_Serial()
        //{
        //    this.backgroundWorker1.WorkerSupportsCancellation = true;
        //    this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
        //}



        String Leonardo_SerialPort;
        /*Hex下载模式*/
        //组建avrdude所需参数
        public string Set_board_info()
        {
            string IC_info = null;
            string speed = null;
            string Board = "-carduino";
            string Serial_PortName = Serial_info.Text;
            switch (Board_info.Text)
            {
                case ("Arduino Uno"):
                    IC_info = "atmega328p";
                    speed = "115200";
                    break;
                case ("Arduino Duemilanove w/ ATmega328"):
                    IC_info = "atmega328p";
                    speed = "57600";
                    break;
                case ("Arduino Diecimila or Duemilanove w/ ATmega168"):
                    IC_info = "atmega168";
                    speed = "19200";
                    break;
                case ("Arduino Nano w/ ATmega328"):
                    IC_info = "atmega328p";
                    speed = "57600";
                    break;
                case ("Arduino Nano w/ ATmega168"):
                    IC_info = "atmega168";
                    speed = "19200";
                    break;
                case ("Arduino Mega 2560 or Mega ADK"):
                    IC_info = "atmega2560";
                    speed = "115200";
                    Board = "-cwiring";
                    break;
                case ("Arduino Mega (ATmega1280)"):
                    IC_info = "atmega1280";
                    speed = "57600";
                    break;
                case ("Arduino Leonardo or Micro or Yún"):
                    IC_info = "atmega32u4";
                    speed = "57600";
                    Board = "-cavr109";
                    Serial_PortName = Leonardo_SerialPort;
                    break;
                case ("Arduino Mini w/ ATmega328"):
                    IC_info = "atmega328p";
                    speed = "115200";
                    break;
                case ("Arduino Mini w/ ATmega168"):
                    IC_info = "atmega168";
                    speed = "19200";
                    break;
                case ("Arduino Ethernet"):
                    IC_info = "atmega328p";
                    speed = "115200";
                    break;
                case ("Arduino Fio"):
                    IC_info = "atmega328p";
                    speed = "57600";
                    break;
                case ("Arduino BT w/ ATmega328"):
                    IC_info = "atmega328p";
                    speed = "19200";
                    break;
                case ("Arduino BT w/ ATmega168"):
                    IC_info = "atmega168";
                    speed = "19200";
                    break;
                case ("LilyPad Arduino USB"):
                    IC_info = "atmega32u4";
                    speed = "57600";
                    Board = "-cavr109";
                    break;
                case ("LilyPad Arduino w/ ATmega328"):
                    IC_info = "atmega328p";
                    speed = "57600";
                    break;
                case ("LilyPad Arduino w/ ATmega168"):
                    IC_info = "atmega168";
                    speed = "19200";
                    break;
                case ("Arduino Pro or Pro Mini (5V, 16 MHz) w/ ATmega328"):
                    IC_info = "atmega328p";
                    speed = "57600";
                    break;
                case ("Arduino Pro or Pro Mini (3.3V, 8 MHz) w/ ATmega328"):
                    IC_info = "atmega328p";
                    speed = "57600";
                    break;
                case ("Arduino Pro or Pro Mini (5V, 16 MHz) w/ ATmega168"):
                    IC_info = "atmega168";
                    speed = "19200";
                    break;
                case ("Arduino Pro or Pro Mini (3.3V, 8 MHz) w/ ATmega168"):
                    IC_info = "atmega168";
                    speed = "19200";
                    break;
                case ("Arduino NG or older w/ ATmega168"):
                    IC_info = "atmega168";
                    speed = "19200";
                    break;
                case ("Arduino NG or older w/ ATmega8"):
                    IC_info = "atmega8";
                    speed = "19200";
                    break;
            }
            string info = "-Cavrdude\\avrdude.conf " + "-v -v -v -v" + " -p" + IC_info + " " + Board + " -P" + Serial_PortName + " -b" + speed + " -D -Uflash:w:" + File_Name.Text + ":i";
            return info;
        }

        //调用avrdude
        private void StartThread()
        {
            
            if (backgroundWorker1.IsBusy != true)
            {
                if (_SerialPort.IsOpen)
                {
                    try
                    {
                        _SerialPort.Close();
                    }
                    catch
                    {
                        _SerialPort.Dispose();
                    }
                }
                Open_or_Close.Text = "打开串口";
                Start_avrdude.Text = "下载中";
                //debug_box.Text = Set_board_info();
                debug_box.AppendText(DateTime.Now + "开始任务\r\n");
                backgroundWorker1.RunWorkerAsync();
            }
            else
                avrdude_messagebox();
        }

        private void avrdude_messagebox()
        {
            string message = "Hex文件下载中，要终止下载吗？";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                avrdude.Kill();
                avrdude.Close();
                backgroundWorker1.CancelAsync();
                debug_box.SelectionStart = debug_box.TextLength;
                debug_box.Paste(DateTime.Now + "   下载被用户终止\r\n");
                Start_avrdude.Text = "开始下载";
            }
        }
        //选择要载入的HEX文件
        private void Select_file_button_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog_HEX = new OpenFileDialog();

            //openFileDialog_HEX.InitialDirectory = "c:\\";
            openFileDialog_HEX.Filter = "hex files (*.hex)|*.hex";
            openFileDialog_HEX.FilterIndex = 2;
            openFileDialog_HEX.RestoreDirectory = true;

            if (openFileDialog_HEX.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog_HEX.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            File_Name.Text = openFileDialog_HEX.FileName;
        }
        //开始下载按键
        private void Start_avrdude_Click(object sender, EventArgs e)
        {
            try
            {
                if ((Board_info.Text != "") && (Serial_info.Text != "") && (File_Name.Text != "..."))
                {

                    StartThread();
                }
                else
                    MessageBox.Show("请先选择Arduino型号、串口及Hex文件");
            }
            catch 
            {
                //MessageBox.Show("串口可能被占用");
            }
        }
        //avrdude线程结束通知
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // This event handler is called when the background thread finishes.
            // This method runs on the main thread.
            if (e.Error != null)
                MessageBox.Show("Error: " + e.Error.Message);
            else if (e.Cancelled)
                MessageBox.Show("下载终止", caption);
            else
            {
                debug_box.SelectionStart = debug_box.TextLength;
                debug_box.Paste(DateTime.Now + "   任务完成\r\n");
                Start_avrdude.Text = "开始下载";
            }
        }
        //32u4、SAM3X8E规则
        private string Special()
        {
            string info = " ";
            string[] ports_old = SerialPort.GetPortNames();
            _SerialPort.PortName = Serial_info.Text;
            _SerialPort.BaudRate = 1200;
            _SerialPort.Open();
            Thread.Sleep(600);
            string[] ports_new = SerialPort.GetPortNames();
            foreach (var item in ports_new.Except(ports_old))
            {
                //debug_box.AppendText(item+" , ");
                info = item;
            }
            return (info);
        }

        //调用控制台程序avrdude.exe
        private void process_avrdude()
        {
            try
            {
                if (Board_info.Text == "Arduino Leonardo or Micro")
                {
                    Leonardo_SerialPort = Special();
                }

                avrdude = new Process();
                avrdude.StartInfo.UseShellExecute = false;
                avrdude.StartInfo.FileName = "avrdude\\atprogram.exe";
                avrdude.StartInfo.CreateNoWindow = true;
                avrdude.StartInfo.RedirectStandardOutput = true;
                avrdude.StartInfo.RedirectStandardError = true;
                avrdude.EnableRaisingEvents = true;
                //avrdude.StartInfo.Arguments = Set_board_info();
                avrdude.StartInfo.Arguments = "-t avrispmk2 -i ISP -d atmega2560 chiperase program -fl -f D:\\MKIIread.hex verify -fl -f D:\\MKIIread.hex";
                avrdude.OutputDataReceived += new DataReceivedEventHandler(debug_output);
                avrdude.ErrorDataReceived += new DataReceivedEventHandler(debug_output);
                avrdude.Exited += avrdude_Exited;
                avrdude.Start();
                avrdude.BeginOutputReadLine();
                avrdude.BeginErrorReadLine();
                avrdude.WaitForExit();
                avrdude.Close();
            }
            catch
            {
                debug_box.SelectionStart = debug_box.TextLength;
                debug_box.Paste(DateTime.Now + "   OPEN JUMPER Serial Assistant:未知错误！\r\n");
            }
        }

        void avrdude_Exited(object sender, EventArgs e)
        {
            Start_avrdude.Text = "开始下载";
        }

        private void debug_output(object sender, DataReceivedEventArgs e)
        {
            if (e != null)
                debug_box.AppendText(e.Data == null ? "" : e.Data + "\r\n");
        }

        //avrdude运行进度提示
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        //多线程调用avrdude
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            process_avrdude();
        }
        //选择avrdude串口
        private void avrdude_Serial_DropDown(object sender, EventArgs e)
        {
            Serial_info.Items.Clear();
            NewSerial();
        }
    }
}
