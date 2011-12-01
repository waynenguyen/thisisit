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
using Product_Class;
using Transaction_Class;
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




            // testing protocols
            //LocalDBInterface.openConnection();
            //Product newP = new Product("333333", "beer", "F&B", "Sai Gon", 350, 333);
            //LocalDBInterface.addProduct(newP); done
            //LocalDBInterface.addProductNumberInStock("333333", 300); done
            //LocalDBInterface.addProductNumberSoldToday("333333", 300); done
            //List<Product> t = LocalDBInterface.getCategoryDayReport("fruit", "2011-10-25");
            //t = LocalDBInterface.getManufacturerDayReport("Apple", "2011-10-24");
            //newP = LocalDBInterface.getProduct("111150");
            //t = LocalDBInterface.getProductByCategory("household");
            //t = LocalDBInterface.getProductByManufacturer("Farm");
            //t = LocalDBInterface.getProductDayReport("2011-10-25");
            //t = LocalDBInterface.getProductSoldToday();
            //Transaction trans = LocalDBInterface.getTransaction("6");
            //newP = LocalDBInterface.getProduct("333333");
            //newP.setManufacturer("Bia Sai Gon");
            //LocalDBInterface.modifyProduct(newP);
            //LocalDBInterface.removeProductNumberInStock("333333", 20);
            //LocalDBInterface.setProductNumberInStock("333333", 1234);
            //LocalDBInterface.closeConnection();
            /* HTTPGet example
            HTTPGet req = new HTTPGet();
            req.Request("http://ec2-50-17-68-237.compute-1.amazonaws.com/RTB/listpurchase/123");
            Console.WriteLine(req.StatusLine);
            Console.WriteLine(req.ResponseTime);
            */
            //HTTPGet req = new HTTPGet();
            //req.Request("http://3B.cegmarket.appspot.com/store/sync?id=11001&from=0&to=100");
            //Console.WriteLine(req.StatusLine);
            //Console.WriteLine(req.ResponseTime);
            HQServerInterface.sendTodayReport();
		}

		private void ReadSerialPort(object sender, System.Windows.RoutedEventArgs e)
		{
            //SerialConnection.createSerialConnection();
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