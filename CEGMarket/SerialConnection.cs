using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using Transaction_Class;
using Product_Class;
namespace CEGMarket
{
    static class SerialConnection
    {
        /****************************************************************************************************/
        // ATTRIBUTES (Basic)
        /****************************************************************************************************/
        //static bool _continue;
        static SerialPort _serialPort;
        static List<Byte> portBuffer = new List<Byte>();
        static string RxString;
        static bool TransactionInProgress=false;
        static Transaction tempTransaction;
        static Product tempProduct;
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
        public static void WriteToSerialPort(byte[] bytes,int num)
        {
            _serialPort.Write(bytes,0,num);   // start from byte[0], for num bytes
        
        }
        public static void openCashRegister(string id)
        {
            _serialPort.Write(new byte[] { 0x46 }, 0, 1);
        }
        public static void DataReceived(object sender,SerialDataReceivedEventArgs e)
<<<<<<< HEAD
=======
        { 
            //Do something with it
        }
        public static void createSerialConnection()
>>>>>>> 74329fe673584751998b1979fc6d111702050f50
        {
            // Get the number of bytes available to read.
            while (true)
            {
                int numberOfBytesToRead = _serialPort.BytesToRead;
                if (numberOfBytesToRead <= 0) break;
                Byte[] newReceivedData = new Byte[numberOfBytesToRead];// Create a byte array large enough to hold the bytes to be read.
                _serialPort.Read(newReceivedData, 0, numberOfBytesToRead);// Read the bytes into the byte array.
                portBuffer.AddRange(newReceivedData);// Add the bytes to the end of the list.
                Thread.Sleep(30);// in ms; 9600bps = 1B/ms
            }
            List<Byte> portBufferCopy = new List<Byte>(portBuffer);
            if (portBuffer.Count == 1)
            {
                //Check if it is Start Signal(1001 1011)
                if (portBuffer[0] == 0x9B)//1001 1011?
                {
                    TransactionInProgress = true;
                    //Get Barcode from serial
                    string barcode="333333";
                    //Get quantity from serial
                    string quantity="0";
                    //Get price from Local DB
                    tempProduct = LocalDBInterface.getProduct(barcode);
                    double price = tempProduct.getPrice();
                    //Create Transaction
                    //Return subtotal
                    
                }
            }

            RxString = System.Text.ASCIIEncoding.ASCII.GetString(portBuffer.ToArray());
            
            //While loop Receive Barcode, Receive Quantity, Create Transaction, ReturnSubtotal
            //check for total signal (1001 1100)

        }

    }
            
}
