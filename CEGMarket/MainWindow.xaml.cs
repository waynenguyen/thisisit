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
            HQServerInterface.sync();

		}

        private void ReadSerialPort(object sender, System.Windows.RoutedEventArgs e)
        {
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
            //cmbPortName.Items.Clear();
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
                MessageBox.Show("There are no COM Ports detected on this computer.\nPlease install a COM Port and restart this app.", "No COM Ports Installed");
                rtbTxData.AppendText(String.Format("{0:dd.MM.yy HH:mm:ss }", DateTime.Now) + "There are no COM Ports detected on this computer.\nPlease install a COM Port and restart this app.\n");
                //this.Close();
            }
        }
        private void openSerialPort()
        {
            bool openPort = SerialConnection.openSerialConnection(cmbPortName.Text);
            if (openPort == true) rtbTxData.AppendText(String.Format("{0:dd.MM.yy HH:mm:ss }", DateTime.Now) + "Serial Port "+cmbPortName.Text+" connected successfully\n");
            else rtbTxData.AppendText(String.Format("{0:dd.MM.yy HH:mm:ss }", DateTime.Now) + "Can't connect to serial port " + cmbPortName.Text + "\n");
        }

        private void closeSerialPort()
        {
            SerialConnection.closeSerialConnection();
            rtbTxData.AppendText(String.Format("{0:dd.MM.yy HH:mm:ss }", DateTime.Now) + "Serial Port Closed Successfully\n");
        }
       

        private void Exit_App(object sender, System.EventArgs e)
        {
            // TODO: Add event handler implementation here.
        }

    }

}