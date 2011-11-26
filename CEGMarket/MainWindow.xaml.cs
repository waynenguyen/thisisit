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

            /* HTTPGet example
            HTTPGet req = new HTTPGet();
            req.Request("http://ec2-50-17-68-237.compute-1.amazonaws.com/RTB/listpurchase/123");
            Console.WriteLine(req.StatusLine);
            Console.WriteLine(req.ResponseTime);
            */
            HTTPGet req = new HTTPGet();
            req.Request("http://3B.cegmarket.appspot.com/store/sync?id=11001&from=0&to=100");
            Console.WriteLine(req.StatusLine);
            Console.WriteLine(req.ResponseTime);
		}

	}
}