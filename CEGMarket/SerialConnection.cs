﻿using System;
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
        public static string rtxtemp;

        static int receiveStartSig = 0;

        // Atrributes (Phong)
        const int bufferSize = 6;
        
        const byte startSig = 251;
        const byte paySig = 252;
        const byte endSig = 250;
        const byte contSig = 253;

        const int dataPadding = 0;
        const int startPadding = 1;
        const int endPadding = 2;
        const int cashRegisterID = 58; //00111010B

        static byte[] buffer = new byte[bufferSize];
        static int idx = 0;

        const string priceTagLCD = "019047";

        static int mode = 0; // 0 barcode-quantity; 1 payment; 2 end; 3 cont trans

        static double subtotal = 0.0;
        static int barcode = 0;
        static int quantity = 0;

        static double pay = 0.0;
        static double change = 0.0;
        static Thread readingThread;

        /****************************************************************************************************/
        // Serial Port Related Functions
        /****************************************************************************************************/
        public static string[] GetSerialPorts()
        {
            string[] SerialPortList = SerialPort.GetPortNames();
            return SerialPortList;
        }
        public static void openSerialConnection(string COMPort) {

            _serialPort = new SerialPort(COMPort, 9600, Parity.None, 8, StopBits.One);
            //_serialPort.ReadTimeout = 500;
            //_serialPort.WriteTimeout = 500;

            try
            {
                    if (!_serialPort.IsOpen) _serialPort.Open();
                    rtxtemp = "Serial Port "+ COMPort +" opens successfully/n";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening/writing to serial port: " + ex.Message, "Error!");
                rtxtemp = "Error opening serial port" + COMPort +"/n";
            }
            readingThread = new Thread(new ThreadStart(readSerial));
            readingThread.Start();
            byte[] output = new byte[4];
            output[0] = (byte)(cashRegisterID | (startPadding << 6));
            _serialPort.Write(output, 0, 1);

            Thread.Sleep(100);

            // send 10 + id of cash register to end communication
            output = new byte[4];
            output[0] = (byte)(cashRegisterID | (endPadding << 6));
            _serialPort.Write(output, 0, 1);
            receiveStartSig = 0;

            // start comm with price tag
            Product p = LocalDBInterface.getProduct(priceTagLCD);
            string toSend;

            if (p != null)
            {
                connectLCD(3855, "1100");
                toSend = formatDisplay(p.getName(), p.getPrice(), p.getNumberInStock());
            }
            else
            {
                toSend = "No valid product";
                int size = toSend.Length;
                for (int i = 0; i < 32 - size; i++)
                {
                    toSend += " ";
                }
            }

            sendToLED(toSend);

            // send signal to start comm with cash register again
            output[0] = (byte)(cashRegisterID | (startPadding << 6));
            _serialPort.Write(output, 0, 1);

            receiveStartSig = 0;

            Console.WriteLine("Finish initialization");
        }
        public static void closeSerialConnection()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                rtxtemp = "Serial Port closed successfully";
            }
            else rtxtemp = "No openning serial port";
        }
        public static void WriteToSerialPort(byte[] bytes,int num)
        {
            _serialPort.Write(bytes,0,num);   // start from byte[0], for num bytes
        
        }
        public static void openCashRegister(string id)
        {
            _serialPort.Write(new byte[] { 0x46 }, 0, 1);
        }

        public static void readSerial()
        {
            while (true)
            {
                byte tmp = Convert.ToByte(_serialPort.ReadByte());

                string binstr;
                convertToBinary(out binstr, tmp);
                Console.WriteLine("--Rec: " + binstr);

                if (tmp == startSig && receiveStartSig == 0)
                {
                    receiveStartSig = 1;
                    Console.WriteLine("Received start signal");
                    mode = 0;
                    idx = 0;
                    subtotal = 0.0;

                    //Create Transaction
                    tempTransaction= new Transaction();
                }
                else if (tmp == paySig)
                {
                    Console.WriteLine("Received pay signal");
                    mode = 1;
                    idx = 0;
                }
                else if (tmp == endSig)
                {
                    receiveStartSig = 0;
                    Console.WriteLine("Received end signal");

                    // send 10 + id of cash register to end communication
                    byte[] output = new byte[4];
                    output[0] = (byte)(cashRegisterID | (endPadding << 6));
                    _serialPort.Write(output, 0, 1);
                    receiveStartSig = 0;

                    // start comm with price tag
                    Product p = LocalDBInterface.getProduct(priceTagLCD);
                    string toSend;

                    if (p != null)
                    {
                        connectLCD(3855, "1100");
                        toSend = formatDisplay(p.getName(), p.getPrice(), p.getNumberInStock());
                    }
                    else
                    {
                        toSend = "No valid product";
                        int size = toSend.Length;
                        for (int i = 0; i < 32 - size; i++)
                        {
                            toSend += " ";
                        }
                    }

                    sendToLED(toSend);

                    // send signal to start comm with cash register again
                    output[0] = (byte)(cashRegisterID | (startPadding << 6));
                    _serialPort.Write(output, 0, 1);
                    receiveStartSig = 0;

                    mode = 2;
                }
                else if (tmp == contSig)
                {
                    receiveStartSig = 1;
                    Console.WriteLine("Received cont signal");
                    mode = 3;
                    idx = 0;
                }

                if (idx < bufferSize)
                {
                    int lowerNibble = tmp & 15;
                    int higherNibble = tmp >> 4;

                    if (lowerNibble < 10 && lowerNibble >= 0
                        && higherNibble < 10 && higherNibble >= 0)
                    {
                        buffer[idx++] = tmp;

                        if (idx == 6) // get full data
                        {
                            // full buffer -> get enough data
                            //    mode = 0: barcode and quantity
                            //    mode = 1: payment and change
                            //    mode = 2: payment and change
                            if (mode == 0)
                            {
                                // get barcode and quantity
                                convertBCDToNumber(new byte[] { buffer[0], buffer[1], buffer[2] }, out barcode);
                                convertBCDToNumber(new byte[] { buffer[3], buffer[4], buffer[5] }, out quantity);

                                Console.WriteLine("Received raw: " + buffer[0] + "." + buffer[1] + "." + buffer[2]
                                                                   + "." + buffer[3] + "." + buffer[4] + "." + buffer[5]);
                                Console.WriteLine("Received: Barcode: " + barcode + " Quantity: " + quantity);
                                
                                //Get Price of product with barcode
                                Product temp2 = LocalDBInterface.getProduct(Convert.ToString(barcode));
                                double price = 0;
                                if (temp2 == null) price = 0;
                                else
                                {
                                    price = temp2.getPrice();
                                    // Add product to transaction
                                    tempTransaction.insertProductIntoShoppingBag(Convert.ToString(barcode), quantity, price * quantity);
                                }
                            

                                subtotal += quantity * price; // suppose 10 dollars per item
                                                      
                                int tmpSubtt = (int)(subtotal * 100);
                                byte[] output = new byte[4];
                                convertToBCD(tmpSubtt, out output, dataPadding);

                                Console.Write("Sent: Subtotal: " + subtotal + " in BCD: ");
                                
                                for (int j = 3; j >= 0; j--)
                                {
                                    string tmpBCDByte;
                                    convertToBinary(out tmpBCDByte, output[j]);
                                    Console.Write(tmpBCDByte + ", ");
                                }
                                Console.WriteLine();

                                // send back subtotal
                                _serialPort.Write(output, 0, 4);

                                idx = 0;
                            }
                            else if (mode == 1 || mode == 2)// if (mode == 1 || mode == 2)
                            {
                                int tmpPay = 0;
                                // get payment and change
                                convertBCDToNumber(new byte[] { buffer[0], buffer[1], buffer[2] }, out tmpPay);
                                pay = ((double)tmpPay) / 100;
                                convertBCDToNumber(new byte[] { buffer[3], buffer[4], buffer[5] }, out tmpPay);
                                change = ((double)tmpPay) / 100;

                                Console.WriteLine("Received: pay: " + pay + " change: " + change);
                                
                                {
                                    // send ack
                                    byte[] output = new byte[4];
                                    output[0] = output[1] = output[2] = output[3] = 63;
                                    Console.WriteLine("Ack sent");
                                    _serialPort.Write(output, 0, 4);

                                    idx = 0;
                                    mode = 0;

                                    //Add Transaction
                                    DateTime time = DateTime.Now;              // Use current time
                                    string format = "yyyy-MM-dd";    // Use this format
                                    Console.WriteLine(time.ToString(format));
                                    if (tempTransaction != null)
                                    {
                                        tempTransaction.setDate(time.ToString(format));
                                        tempTransaction.setTotalPrice(subtotal);
                                        tempTransaction.setMoneyReceive(pay);
                                        tempTransaction.setMoneyChange(change);
                                        LocalDBInterface.addTransaction(tempTransaction);
                                        Console.WriteLine("Transaction completed successfully");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Transaction is null");
                                    }

                                }
                            }
                        } // if idx == 6
                        //else if (mode == 2) // end signal, stop communication with cash register, start comm with price tag
                        //{
                        //    // send 10 + id of cash register to end communication
                        //    byte[] output = new byte[4];
                        //    output[0] = (byte)(cashRegisterID | (endPadding << 6));
                        //    _serialPort.Write(output, 0, 1);
                        //    receiveStartSig = 0;

                        //    // start comm with price tag
                        //    connectLCD(3855, "1100");
                        //    string toSend = formatDisplay("Panda Candy", 5.8, 100);
                        //    sendToLED(toSend);
                            
                        //    //// send signal to start comm with cash register again
                        //    //output[0] = (byte)(cashRegisterID | (startPadding << 6));
                        //    //_serialPort.Write(output, 0, 1);
                        //    //receiveStartSig = 0;

                        //    mode = 0;
                        //}
                        else if (mode == 3 && idx == 3) // continue transaction
                        {
                            int transID;
                            convertBCDToNumber(new byte[] { buffer[0], buffer[1], buffer[2] }, out transID);

                            Console.WriteLine("Recieved: transID: " + transID);
                            int subtotalInt = 0;
                            tempTransaction = LocalDBInterface.getTransaction(Convert.ToString(transID));
                            if (tempTransaction == null) 
                                subtotal = 0;
                            else
                                subtotal = tempTransaction.getTotalPrice();

                            subtotalInt = (int)(subtotal * 100);
                            byte[] output = new byte[4];
                            convertToBCD(subtotalInt, out output, dataPadding);
                            _serialPort.Write(output, 0, 4); // sent back subtotal

                            idx = 0;
                            mode = 0;
                        }
                    } // if data is bcd (data)
                }
            }
        }


        /****************************************************************************************************/
        // Conversion for LED Related Functions
        /****************************************************************************************************/
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
                bytes[j] = Convert.ToByte(temp2, 2);
                orig = orig.Substring(0, _length - 6);
                _length -= 6;
                j++;
            }
            return bytes;
        }

        /****************************************************************************************************/
        // Conversion for LED Related Functions (by Phong)
        /****************************************************************************************************/
        static void convertToBinary(out string binarystr, int number)
        {
            int r;

            binarystr = "";

            if (number == 0)
            {
                binarystr = "0";
                return;
            }

            while (number != 0)
            {
                r = number % 2;
                binarystr = (r == 0 ? "0" : "1") + binarystr;
                number /= 2;
            }
        }
        
        static void convertBCDToNumber(byte[] input, out int number)
        {
            // input[0]: LSB
            number = 0;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                number = number * 100 + (input[i] >> 4) * 10 + (input[i] & 15);
            }
        }

        static void convertToBCD(int number, out byte[] output, int padding)
        {
            byte[] tmp1 = new byte[6];

            padding <<= 6;

            for (int i = 0; i < 6; i++)
            {
                tmp1[i] = Convert.ToByte(number % 10);
                number /= 10;
            }

            byte tmp = 0;
            output = new byte[4];

            tmp = (byte)((tmp1[1] & 3) << 4);
            output[0] = (byte)(tmp1[0] | tmp);

            tmp = (byte)(tmp1[1] >> 2);
            output[1] = (byte)(tmp | (tmp1[2] << 2));

            tmp = (byte)((tmp1[4] & 3) << 4);
            output[2] = (byte)(tmp1[3] | tmp);

            tmp = (byte)(tmp1[4] >> 2);
            output[3] = (byte)((tmp1[5] << 2) | tmp);

            for (int i = 0; i < 4; i++)
            {
                output[i] = (byte)(output[i] | padding);
            }
        }


        /****************************************************************************************************/
        // Conversion for LCD Related Functions
        /****************************************************************************************************/
        //==============================================================================
        // communication with LCD
        public static string formatDisplay(string productName, double price, int quantity)
        {
            string output = "";

            string priceStr = price.ToString();
            string quanStr = quantity.ToString();
            if (quantity <= 0)
            {
                priceStr = "";
                quanStr = "Out of stock";
            }

            int padding;

            padding = (16 - productName.Length) / 2;

            if (padding < 0) padding = 0;

            for (int i = 0; i < padding; i++)
                output += " ";
            output += productName;
            for (int i = 0; i < padding; i++)
                output += " ";

            if ((16 - productName.Length) % 2 != 0)
                output += " ";

            output += priceStr + "$";
            padding = 16 - priceStr.Length - quanStr.Length - 1;

            for (int i = 0; i < padding; i++)
                output += " ";

            output += quanStr;

            return output;
        }

        public static void connectLCD(int id, string header)
        {
            //string header = "1100";
            string LCDId = Convert.ToString(id, 2); // Convert to Binary
            if (LCDId.Length < 12)
            {
                int temp = 12 - LCDId.Length;
                for (int i = 0; i < temp; i++)
                    LCDId = "0" + LCDId;
            }
            byte[] bytes = new byte[3];
            int j = 0;// variable count from 0 to 4
            while (LCDId.Length >= 4)
            {
                string temp = LCDId.Substring(LCDId.Length - 4, 4);
                string temp2 = header + temp;
                bytes[j] = Convert.ToByte(temp2, 2);
                LCDId = LCDId.Substring(0, LCDId.Length - 4);
                j++;
            }
            _serialPort.Write(bytes, 0, 1);
            _serialPort.Write(bytes, 1, 1);
            _serialPort.Write(bytes, 2, 1);
        }

        public static byte[] charToByte(char c)
        {
            byte[] bytes = new byte[2];

            // Higher bit of a byte
            byte byte1 = Convert.ToByte(c >> 4);
            bytes[1] = Convert.ToByte(byte1 | 208); //  Logical OR with 1101 0000

            //Lower bit of a byte
            byte byte2 = Convert.ToByte(c & 15); // Char && with 15
            bytes[0] = Convert.ToByte(byte2 | 208); // Char || with 1101 0000

            return bytes;
        }

        public static byte[] stringToByte(string s)
        {
            int i = 0;
            int noOfByte = s.Length * 2;
            byte[] tempBytes = new byte[2];
            byte[] bytes = new byte[noOfByte];
            while (i < s.Length)
            {
                tempBytes = charToByte(s.ElementAt(i));
                bytes[2 * i] = tempBytes[1];
                bytes[2 * i + 1] = tempBytes[0];
                i++;
            }
            return bytes;
        }

        public static void sendToLED(string s)
        {
            //// if Length < 16 -> add space after
            //if (s.Length < 16)
            //    for (int i = 0; i < 16 - s.Length; i++)
            //    {
            //        s += " ";
            //    }
            //// if length > 16 -> cut it
            //else if (s.Length > 16)
            //    s = s.Substring(0, 16);

            // Convert to bytes
            byte[] buffer = new byte[64];
            buffer = stringToByte(s);

            Console.WriteLine("To serial port");
            for (int i = 0; i < 64; i++)
            {
                string binstr;
                convertToBinary(out binstr, buffer[i]);
                Console.WriteLine(i + ":" + binstr);
            }

            Console.WriteLine();
            Console.WriteLine("From serial port");
            _serialPort.Write(buffer, 0, 64);

            //// Write via serial port
            //for (int i = 0; i < 64; i++)
            //{
            //    mySerialPort.Write(buffer, i, 1);
            //    Thread.Sleep(100);
            //}
        }
    }
            
}
