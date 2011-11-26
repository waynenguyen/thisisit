using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.IO.Ports;
using System.Threading;

using CEGMarketSystem_Class;
namespace CEGMarket
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
            this.InitializeComponent();
			// Insert code required on object creation below this point.
            CEGMarketSystem CEGSystem = new CEGMarketSystem();
            CEGSystem.CalculateProductProfit("123");
           
           


		}

		private void ReadSerialPort(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: Add event handler implementation here.
            bool _continue;
            SerialPort _serialPort;
            
            //string name;
            //string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            //Thread readThread = new Thread(Read);

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            //_serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            //_serialPort.PortName = SetPortName(_serialPort.PortName);
            //_serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            //_serialPort.Parity = SetPortParity(_serialPort.Parity);
            //_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            //_serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            //_serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();
            _continue = true;
            //readThread.Start();
            int n=0;
            //name = "TuanVu";
            while (n<1)
            {
                try
                {
                    string message2 = _serialPort.ReadLine();
                    if (message2 != "")
                    {
                        statusbox.Text += DateTime.Now.ToString("HH:mm:ss tt");
                        statusbox.Text += message2;
                        n++;
                    }
                    //Console.WriteLine(message2);
                }
                catch (TimeoutException) { }
            }
            
            //readThread.Join();
            _serialPort.Close();
        }

        private void openSerialPort(SerialPort port) { 
        
        }

        private void closeSerialPort(SerialPort port) {
            port.Close();
        }

        private void DEC_to_BCD(object sender, RoutedEventArgs e)
        {
            string price= PriceTextBox.Text;
            price = ConvertDECtoBCD(Convert.ToInt32(price));
            byte[] bytes=SplitBCD(price, "00");

            PriceTextBox.Text = bytes[0].ToString() + " " + bytes[1].ToString() + " " + bytes[2].ToString() + " " + bytes[3].ToString();
        }

        private string ConvertDECtoBCD(int dec) {
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

        private byte[] SplitBCD(string orig, string header)
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