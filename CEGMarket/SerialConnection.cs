//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO.Ports;
//using System.Threading;

//namespace CEGMarket
//{
//    static class SerialConnection
//    {
//        /****************************************************************************************************/
//        // ATTRIBUTES (Basic)
//        /****************************************************************************************************/
//        static bool _continue;
//        static SerialPort _serialPort;

//        /****************************************************************************************************/
//        // CONSTRUCTOR
//        /****************************************************************************************************/

//        public static void createSerialConnection()
//        {

//            string name;
//            string message;
//            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
//            Thread readThread = new Thread(Read);

//            // Create a new SerialPort object with default settings.
//            _serialPort = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
//            //_serialPort = new SerialPort();

//            // Allow the user to set the appropriate properties.
//            //_serialPort.PortName = SetPortName(_serialPort.PortName);
//            //_serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
//            //_serialPort.Parity = SetPortParity(_serialPort.Parity);
//            //_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
//            //_serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
//            //_serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

//            // Set the read/write timeouts
//            _serialPort.ReadTimeout = 500;
//            _serialPort.WriteTimeout = 500;

//            _serialPort.Open();
//            _continue = true;
//            readThread.Start();

//            name = "TuanVu";
//            while (_continue)
//            {
//                message = Console.ReadLine();

//                if (stringComparer.Equals("quit", message))
//                {
//                    _continue = false;
//                }
//                else
//                {
//                    _serialPort.WriteLine(
//                        String.Format("<{0}>: {1}", name, message));
//                }
//            }
            
//            readThread.Join();
//            _serialPort.Close();
//        }


//        ~SerialConnection() { }


//        /****************************************************************************************************/
//        // METHODS
//        /****************************************************************************************************/
//        public static void Read()
//        {
//            while (_continue)
//            {
//                try
//                {
//                    string message = _serialPort.ReadLine();
//                    Console.WriteLine(message);
//                }
//                catch (TimeoutException) { }
//            }
//        }


//    }
//}
