using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace SPA
{
    class command
    {
        
        int device;
        string name,hsmode,baud,pin,power;
        string[] HM_baud=new string[]{ "9600", "19200", "38400", "57600", "115200", "4800", "2400" };
        public string GetName, Gethsmode, GetBaud, GetPin, SetName, Sethsmode, SetBaud, SetPin;
        public string GetPower, SetPower;
        SerialPort serialport;
        command(int DEVICE,string NAME,string HSMODE,string BAUD,string PIN) 
        {
            device = DEVICE;
            name = NAME;
            hsmode = HSMODE;
            baud = BAUD;
            pin = PIN;
            switch (device)
            {
                case 0:
                    GetName = "NAME?\r\n";
                    SetName = "NAME=" + name + "\r\n";
                    GetBaud = "UART?\r\n";
                    SetBaud = "UART=" + baud + "," + "1,2\r\n";
                    Gethsmode = "ROLE?\r\n";
                    Sethsmode = "ROLE=" + hsmode + "\r\n";
                    GetPin = "PSWD?\r\n";
                    SetPin = "PSWD=" + pin + "\r\n";
                    break;
                case 1:
                    GetName = "NAME?";
                    SetName = "NAME" + name;
                    GetBaud = "BAUD?";
                    for (int i = 0; i < HM_baud.Length; i++)
                    {
                        if (HM_baud[i] == baud)
                        {
                            SetBaud = "BAUD" + i;
                            break;
                        }
                    }
                    Gethsmode = "ROLE?";
                    if (hsmode == "0") hsmode = "1";
                    else 
                        if (hsmode == "1") hsmode = "0";
                    Sethsmode = "ROLE=" + hsmode;
                    GetPin = "PASS?";
                    SetPin = "PASS=" + pin;
                    //HM系列特有
                    SetPower = "POWE?";
                    GetPower = "POWE" + power;
                    break;
            }
        }

        public bool AutoBaud()
        {
            serialport.Open();
            for (int i = 0; i < HM_baud.Length; i++)
            {
                serialport.BaudRate = Convert.ToInt32(HM_baud[i]);
                serialport.Write("AT");
                Thread.Sleep(300);
                if (serialport.ReadLine() == "OK")
                {
                    return true;
                }
            }
            return false;
        }

        public string[] read()
        {
            string[] redata = new string[] { "error", "error", "error", "error", "error" };
            serialport.Write(GetName);
            Thread.Sleep(300);
            string temp= serialport.ReadLine();
            //temp.inde
            serialport.Write(GetBaud);
            Thread.Sleep(300);
            serialport.Write(Gethsmode);
            Thread.Sleep(300);
            serialport.Write(GetPin);
            Thread.Sleep(300);
            return redata;

        }

        public void write() 
        {

        }


    }
}
