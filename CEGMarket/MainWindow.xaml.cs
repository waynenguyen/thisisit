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
            _serialPort = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
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

		}

	}