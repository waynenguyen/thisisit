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
            PostSubmitter post = new PostSubmitter();
            post.Url = "http://ec2-50-17-68-237.compute-1.amazonaws.com/2102/post/14";
            //post.PostItems.Add("op", "100");
            //post.PostItems.Add("rel_code", "1102");
            //post.PostItems.Add("FREE_TEXT", "c# jobs");
            //post.PostItems.Add("SEARCH", "");
            post.Type = PostSubmitter.PostTypeEnum.Post;
            
            string result = post.Post();
            var x = 0;
            this.InitializeComponent();
			// Insert code required on object creation below this point.
            CEGMarketSystem CEGSystem = new CEGMarketSystem();
            CEGSystem.CalculateProductProfit("123");
            //PostSubmitter myPost = new PostSubmitter("http://ec2-50-17-68-237.compute-1.amazonaws.com/2102/post/14.php");
           


		}

	}
}