using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace CEGMarket
{
    static class SerialConnection
    {
        /****************************************************************************************************/
        // ATTRIBUTES (Basic)
        /****************************************************************************************************/
        //static bool _continue;
        static SerialPort _serialPort;

        /****************************************************************************************************/
        // Functions
        /****************************************************************************************************/
        public static string[] GetSerialPorts()
        {
            string[] SerialPortList = SerialPort.GetPortNames();
            return SerialPortList;
        }
        public static void openSerialConnection(string COMPort) {

            _serialPort = new SerialPort(COMPort, 9600, Parity.None, 8, StopBits.One);
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //readThread.Start();
        }
        public static void closeSerialConnection()
        {
            _serialPort.Close();
        }
        public static void WriteToSerialPort(byte[] bytes)
        {
            _serialPort.Write(bytes,0,4);   // not sure what Offset 0 means
        
        }
        public static void openCashRegister(string id)
        {
            _serialPort.Write(new byte[] { 0x46 }, 0, 1);
        }
        public static void DataReceived(object sender,SerialDataReceivedEventArgs e)
        { 
            //Do something with it
        }
        public static void createSerialConnection()
        {
            // TODO: Add event handler implementation here.
            //bool _continue;
            //SerialPort _serialPort;

            ////string name;
            ////string message;
            //StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            ////Thread readThread = new Thread(Read);

            //// Create a new SerialPort object with default settings.
            //_serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            ////_serialPort = new SerialPort();

            //// Allow the user to set the appropriate properties.
            ////_serialPort.PortName = SetPortName(_serialPort.PortName);
            ////_serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            ////_serialPort.Parity = SetPortParity(_serialPort.Parity);
            ////_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            ////_serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            ////_serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

            //// Set the read/write timeouts
            //_serialPort.ReadTimeout = 500;
            //_serialPort.WriteTimeout = 500;

            //_serialPort.Open();
            //_continue = true;
            //readThread.Start();
            int n = 0;
            //name = "TuanVu";
            while (n < 1)
            {
                try
                {
                    string message2 = _serialPort.ReadLine();
                    if (message2 != "")
                    {
                        //statusbox.Text += DateTime.Now.ToString("HH:mm:ss tt");
                        //statusbox.Text += message2;
                        n++;
                    }
                    //Console.WriteLine(message2);
                }
                catch (TimeoutException) { }
            }

            //readThread.Join();
            _serialPort.Close();
        }
    }
            
}
