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
            //CEGSystem.CalculateProductProfit("123");
            LocalDBInterface.openConnection();
            //// testing protocols
            ////LocalDBInterface.openConnection();
            //Product newP = new Product("333333", "beer", "F&B", "Sai Gon", 350, 333);
            ////LocalDBInterface.addProduct(newP); done
            ////LocalDBInterface.addProductNumberInStock("333333", 300); done
            ////LocalDBInterface.addProductNumberSoldToday("333333", 300); done
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
            ////LocalDBInterface.closeConnection();
            ///* HTTPGet example
            //HTTPGet req = new HTTPGet();
            //req.Request("http://ec2-50-17-68-237.compute-1.amazonaws.com/RTB/listpurchase/123");
            //Console.WriteLine(req.StatusLine);
            //Console.WriteLine(req.ResponseTime);
            //*/
            //HTTPGet req = new HTTPGet();
            //req.Request("http://3B.cegmarket.appspot.com/store/sync?id=11001&from=0&to=100");
            //Console.WriteLine(req.StatusLine);
            //Console.WriteLine(req.ResponseTime);


            //HQServerInterface.sendTodayReport();
            //HQServerInterface.sync();


            //HQServerInterface.sendTodayReport();
            //HQServerInterface.sync();
            //SerialConnection.stringToByte("ngotuanvu");
            //Transaction tempTransaction = new Transaction();
            //string barcode = "123445";
            //int quantity = 1;
            //Product temp2 = LocalDBInterface.getProduct(Convert.ToString(barcode));
            //double price = 0;
            //if (temp2 == null) price = 0;
            //else
            //{
            //    price = temp2.getPrice();
            //    // Add product to transaction
            //    tempTransaction.insertProductIntoShoppingBag(Convert.ToString(barcode), quantity, price * quantity);
            //}
            //LocalDBInterface.addTransaction(tempTransaction);
		}

        private void ConvertToByte(object sender, System.Windows.RoutedEventArgs e)
        {
            string price = PriceTextBox.Text;
            //price = ConvertDECtoBCD(Convert.ToInt32(price));
            byte[] bytes = SerialConnection.DEC_to_BCD(price);

            PriceTextBox.Text = bytes[0].ToString() + " " + bytes[1].ToString() + " " + bytes[2].ToString() + " " + bytes[3].ToString();
        }
        private void GetSerialPorts(object sender, System.Windows.RoutedEventArgs e)
        {
            int index = 0;
            string[] nameArray = SerialConnection.GetSerialPorts();
            cmbPortName.Items.Clear();
            while (index <= nameArray.GetUpperBound(0))
            {
                cmbPortName.Items.Add(nameArray[index]);
                index += 1;
            }
            if (cmbPortName.Items.Contains("COM1")) cmbPortName.Text = "COM1";
            else if (cmbPortName.Items.Count > 0) cmbPortName.SelectedIndex = 0;
            else
            {
                cmbPortName.Text = "None";
                //MessageBox.Show("There are no COM Ports detected on this computer.\nPlease install a COM Port and restart this app.", "No COM Ports Installed");
                rtbTxData.AppendText(String.Format("{0:dd.MM.yy HH:mm:ss }", DateTime.Now) + "There are no COM Ports detected on this computer. Please install a COM Port and restart this app.\n");
                //this.Close();
                
            }
        }
        private void openSerialPort()
        {
            SerialConnection.openSerialConnection(cmbPortName.Text);
            rtbTxData.AppendText(String.Format("{0:dd.MM.yy HH:mm:ss }", DateTime.Now) + SerialConnection.rtxtemp);
        }

        private void closeSerialPort()
        {
            SerialConnection.closeSerialConnection();
            rtbTxData.AppendText(String.Format("{0:dd.MM.yy HH:mm:ss }", DateTime.Now) + SerialConnection.rtxtemp);
        }

        private void Exit_App(object sender, System.EventArgs e)
        {
            // TODO: Add event handler implementation here.
            //SerialConnection.closeSerialConnection();
            LocalDBInterface.closeConnection();
        }

        private void SendToLED(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            SerialConnection.sendToLED(PriceTextBox.Text);
        }

        private void ConnectSerialPort(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            SerialConnection.openSerialConnection(cmbPortName.Text);
            rtbTxData.AppendText(String.Format("{0:dd.MM.yy HH:mm:ss }", DateTime.Now) + SerialConnection.rtxtemp);
        }

        private void GetProductSoldToday(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            //Clear Product List View
            //ProductListView.Items.Clear();

            // Create and get list of Products
            List<Product> temp = new List<Product>();
            temp = LocalDBInterface.getProductSoldToday();

            // Convert to list of string and bind
            List<string> temp2 = new List<string>();
            temp2 = LocalDBInterface.convertListProductToName(temp);
            ProductListView.ItemsSource = temp2;
        }


        private void GetProductByCategory(object sender, System.Windows.RoutedEventArgs e)
        {
            // Create and get list of Products
            List<Product> temp = new List<Product>();
            temp = LocalDBInterface.getProductByCategory(CatManListBox.Text);

            // Convert to list of string and bind
            List<string> temp2 = new List<string>();
            temp2 = LocalDBInterface.convertListProductToName(temp);
            ProductListView.ItemsSource = temp2;
        }

        private void GetProductByManufacturer(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            // Create and get list of Products
            List<Product> temp = new List<Product>();
            temp = LocalDBInterface.getProductByManufacturer(CatManListBox.Text);

            // Convert to list of string and bind
            List<string> temp2 = new List<string>();
            temp2= LocalDBInterface.convertListProductToName(temp);
            ProductListView.ItemsSource = temp2;
        }

        private void GetProductSoldByDay(object sender, System.Windows.RoutedEventArgs e)
        {
            // Create and get list of Products
            List<Product> temp = new List<Product>();
            temp = LocalDBInterface.getProductDayReport(DateTB.Text);

            // Convert to list of string and bind
            List<string> temp2 = new List<string>();
            temp2 = LocalDBInterface.convertListProductToName(temp);
            ProductListView.ItemsSource = temp2;
            
        }

        private void GetCategoryList(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            // CatManListBox.ItemsSource = null;

            List<string> temp = new List<string>();
            temp = LocalDBInterface.getListCategory();
            // Get list of Manufacturer and binding
            CatManListBox.ItemsSource = temp;
        }

        private void GetManufacturerList(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            List<string> temp = new List<string>();
            temp = LocalDBInterface.getListManufacturer();
            // Get list of Manufacturer and binding
            CatManListBox.ItemsSource = temp;
            
        }

    }

}