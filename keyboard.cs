using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Configuration;

namespace SPA
{
    public partial class OJ_Serial : Form
    {
        //键盘按键发送
        void OJ_Serial_KeyPress(object sender, KeyPressEventArgs e)
        {
            //判断控制模式选项卡是否最上层
            if ((tabControl.SelectedTab == tabPage3) && (_SerialPort.IsOpen))
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
            //throw new NotImplementedException();
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
        //
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
    }
}
