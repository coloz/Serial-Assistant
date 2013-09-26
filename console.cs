using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SPA
{
    public partial class OJ_Serial : Form
    {
        /*控制模式*/
        //器件a
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if ((checkBox3.Checked == true) && (_SerialPort.IsOpen))
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
    }
}
