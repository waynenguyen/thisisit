using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using Transaction_Class;
using Product_Class;
using System.Windows;
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

            try
            {
                    if (!_serialPort.IsOpen) _serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening/writing to serial port: " + ex.Message, "Error!");
            }
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
        }
        public static void closeSerialConnection()
        {
            if (_serialPort.IsOpen) _serialPort.Close();
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
                    //reset transaction
                    tempTransaction = new Transaction();
                    //Get Barcode from serial
                    string barcode="333333";
                    //Get quantity from serial
                    string quantity="0";
                    //Get price from Local DB
                    tempProduct = LocalDBInterface.getProduct(barcode);
                    double price = tempProduct.getPrice();
                    //Create Transaction
                    tempTransaction.insertProductIntoShoppingBag(barcode, 1, 12);
                    //Return subtotal
                    _serialPort.Write(new byte[] { 0x0A, 0xE2, 0xFF }, 0, 3);//need to change data.
                }
                //Check for End Transaction
                if (portBuffer[0] == 0x9E || portBuffer[7] == 0x9E)   //1001 1110?
                {
                    LocalDBInterface.addTransaction(tempTransaction);
                    tempTransaction = new Transaction();
                
                
                }
            }

            RxString = System.Text.ASCIIEncoding.ASCII.GetString(portBuffer.ToArray());
            
            //While loop Receive Barcode, Receive Quantity, Create Transaction, ReturnSubtotal
            //check for total signal (1001 1100)

        }
        public static byte[] DEC_to_BCD(string DEC)
        {
            string price = DEC;
            price = ConvertDECtoBCD(Convert.ToInt32(price));
            byte[] bytes = SplitBCD(price, "00");
            return bytes;
        }

        public static string ConvertDECtoBCD(int dec)
        {
            string result = "";
            while (dec > 0)
            {

                int temp1 = dec % 10;
                dec /= 10;
                switch (temp1)
                {
                    case 0:
                        result += "0000";
                        break;
                    case 1:
                        result += "0001";
                        break;
                    case 2:
                        result += "0010";
                        break;
                    case 3:
                        result += "0011";
                        break;
                    case 4:
                        result += "0100";
                        break;
                    case 5:
                        result += "0101";
                        break;
                    case 6:
                        result += "0110";
                        break;
                    case 7:
                        result += "0111";
                        break;
                    case 8:
                        result += "1000";
                        break;
                    case 9:
                        result += "1001";
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public static byte[] SplitBCD(string orig, string header)
        {
            int _length = orig.Length;
            if (_length < 24)
            {
                int temp = 24 - _length;
                for (int i = 0; i < temp; i++)
                    orig = "0" + orig;
            }
            _length = 24;
            byte[] bytes = new byte[4];
            int j = 0;// variable count from 0 to 4
            while (_length >= 6)
            {
                string temp = orig.Substring(_length - 6, 6);
                string temp2 = header + temp;
                Console.WriteLine(temp2);
                bytes[j] = Convert.ToByte(temp2, 2);
                orig = orig.Substring(0, _length - 6);
                _length -= 6;
                j++;
            }
            return bytes;
        }

    }
            
}
