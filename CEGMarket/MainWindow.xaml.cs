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

using System.Windows.Media.Animation;
using System.Timers;

namespace CEGMarket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Timers.Timer HideAllButLogin = new System.Timers.Timer(1000);
        private System.Timers.Timer HideAllButMainMenu = new System.Timers.Timer(1000);
        private System.Timers.Timer HideAllButProduct = new System.Timers.Timer(100);
        private System.Timers.Timer HideAllButDevice = new System.Timers.Timer(100);
        private System.Timers.Timer Relmousecap1 = new System.Timers.Timer(2000);
        private System.Timers.Timer LoginAnimate = new System.Timers.Timer(1000);

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
            List<string> test = new List<string>();
            test = LocalDBInterface.getListCategory();

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

            Main_Menu.Visibility = Visibility.Hidden;
            Product_Manager.Visibility = Visibility.Hidden;
            Device_Manager.Visibility = Visibility.Hidden;
            animationTranslateXGrid(Login, 50, 0, 1000);
            animationFadeGrid(Login, 0, 1, 1000);
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

//----------------------------------------------Animation Functions----------------------------------------------\\
        private void animationTranslateX(Button btn, double from, double to, int duration)
        {
            DoubleAnimation AnimationX = new DoubleAnimation();
            TranslateTransform TranslateX = new TranslateTransform();

            AnimationX.From = from;
            AnimationX.To = to;

            AnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            btn.RenderTransform = TranslateX;

            TranslateX.BeginAnimation(TranslateTransform.XProperty, AnimationX);
        }

        private void animationTranslateXCanvas(Canvas canvas, double from, double to, int duration)
        {
            DoubleAnimation AnimationX = new DoubleAnimation();
            TranslateTransform TranslateX = new TranslateTransform();

            AnimationX.From = from;
            AnimationX.To = to;

            AnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            canvas.RenderTransform = TranslateX;

            TranslateX.BeginAnimation(TranslateTransform.XProperty, AnimationX);
        }

        private void animationTranslateXGrid(Grid grid, double from, double to, int duration)
        {
            DoubleAnimation Animation = new DoubleAnimation();
            TranslateTransform Translate = new TranslateTransform();

            Animation.From = from;
            Animation.To = to;

            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            grid.RenderTransform = Translate;

            Translate.BeginAnimation(TranslateTransform.XProperty, Animation);
        }

        private void animationTranslateXText(TextBlock text, double from, double to, int duration)
        {
            DoubleAnimation Animation = new DoubleAnimation();
            TranslateTransform Translate = new TranslateTransform();

            Animation.From = from;
            Animation.To = to;

            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            text.RenderTransform = Translate;

            Translate.BeginAnimation(TranslateTransform.XProperty, Animation);
        }

        private void animationTranslateY(Button btn, double from, double to, int duration)
        {
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = from;
            AnimationY.To = to;

            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            btn.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);
        }

        private void animationTranslateYGrid(Grid grid, double from, double to, int duration)
        {
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = from;
            AnimationY.To = to;

            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            grid.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);
        }

        private void animationTranslateYText(TextBlock text, double from, double to, int duration)
        {
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = from;
            AnimationY.To = to;

            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            text.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);
        }

        private void animationXandY(Button btn, double fromX, double toX, int durationX, double fromY, double toY, int durationY)
        {
            DoubleAnimation AnimationX = new DoubleAnimation();
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateX = new TranslateTransform();
            TranslateTransform TranslateY = new TranslateTransform();
            TransformGroup animationGroup = new TransformGroup();

            animationGroup.Children.Add(TranslateX);
            animationGroup.Children.Add(TranslateY);
            AnimationX.From = fromX;
            AnimationX.To = toX;
            AnimationY.From = fromY;
            AnimationY.To = toY;
            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(durationY));
            AnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(durationX));
            btn.RenderTransform = animationGroup;
            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationX);
            TranslateX.BeginAnimation(TranslateTransform.XProperty, AnimationY);
        }

        private void animationFade(Button btn, double from, double to, int duration)
        {
            DoubleAnimation animationFade = new DoubleAnimation();
            animationFade.From = from;
            animationFade.To = to;
            animationFade.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            btn.BeginAnimation(Button.OpacityProperty, animationFade);
        }

        private void animationFade(Image btn, double from, double to, int duration)
        {
            DoubleAnimation animationFade = new DoubleAnimation();
            animationFade.From = from;
            animationFade.To = to;
            animationFade.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            btn.BeginAnimation(Button.OpacityProperty, animationFade);
        }

        private void animationFadeText(TextBlock text, double from, double to, int duration)
        {
            DoubleAnimation animationFade = new DoubleAnimation();
            animationFade.From = from;
            animationFade.To = to;
            animationFade.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            text.BeginAnimation(Button.OpacityProperty, animationFade);
        }

        private void animationFadeGrid(Grid grid, double from, double to, int duration)
        {
            DoubleAnimation animationFade = new DoubleAnimation();
            animationFade.From = from;
            animationFade.To = to;
            animationFade.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            grid.BeginAnimation(Grid.OpacityProperty, animationFade);
        }

        private void animationScale(Button btn, double scale, int duration)
        {
            DoubleAnimation animationResizeX = new DoubleAnimation();
            DoubleAnimation animationResizeY = new DoubleAnimation();
            ScaleTransform transform = new ScaleTransform();

            btn.RenderTransform = transform;

            animationResizeX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeX.From = transform.ScaleX;
            animationResizeX.To = transform.ScaleX + scale;

            animationResizeY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeY.From = transform.ScaleY;
            animationResizeY.To = transform.ScaleY + scale;

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationResizeX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationResizeY);
        }

        private void animationScale(Image btn, double scale, int duration)
        {
            DoubleAnimation animationResizeX = new DoubleAnimation();
            DoubleAnimation animationResizeY = new DoubleAnimation();
            ScaleTransform transform = new ScaleTransform();

            btn.RenderTransform = transform;

            animationResizeX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeX.From = transform.ScaleX;
            animationResizeX.To = transform.ScaleX + scale;
            animationResizeX.AutoReverse = true;

            animationResizeY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeY.From = transform.ScaleY;
            animationResizeY.To = transform.ScaleY + scale;
            animationResizeY.AutoReverse = true;


            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationResizeX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationResizeY);
        }

        private void animationScaleAR(Button btn, double scale, int duration)
        {
            DoubleAnimation animationResizeX = new DoubleAnimation();
            DoubleAnimation animationResizeY = new DoubleAnimation();
            ScaleTransform transform = new ScaleTransform();

            btn.RenderTransform = transform;

            animationResizeX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeX.From = transform.ScaleX;
            animationResizeX.To = transform.ScaleX + scale;

            animationResizeY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeY.From = transform.ScaleY;
            animationResizeY.To = transform.ScaleY + scale;

            animationResizeX.AutoReverse = true;
            animationResizeY.AutoReverse = true;

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationResizeX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationResizeY);
        }



//----------------------------------------------Animation Functions End----------------------------------------------\\ 


//----------------------------------------------Timer Functions Start----------------------------------------------\\ 

        private void HideAllButLogin_Tick(object sender, EventArgs e)
        {
            Main_Menu.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                Login.Visibility = Visibility.Visible;
                Main_Menu.Visibility = Visibility.Hidden;
                Product_Manager.Visibility = Visibility.Hidden;
                Device_Manager.Visibility = Visibility.Hidden;

            }));
        }
        
        private void HideAllButMainMenu_Tick(object sender, EventArgs e)
        {
            Main_Menu.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                Login.Visibility = Visibility.Hidden;
                Main_Menu.Visibility = Visibility.Visible;
                Product_Manager.Visibility = Visibility.Hidden;
                Device_Manager.Visibility = Visibility.Hidden;

            }));
        }
        
        private void HideAllButProduct_Tick(object sender, EventArgs e)
        {
            Main_Menu.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                Login.Visibility = Visibility.Hidden;
                Main_Menu.Visibility = Visibility.Hidden;
                Product_Manager.Visibility = Visibility.Visible;
                Device_Manager.Visibility = Visibility.Hidden;

            }));
        }

        private void HideAllButDevice_Tick(object sender, EventArgs e)
        {
            Main_Menu.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                Login.Visibility = Visibility.Hidden;
                Main_Menu.Visibility = Visibility.Hidden;
                Product_Manager.Visibility = Visibility.Hidden;
                Device_Manager.Visibility = Visibility.Visible;

            }));
        }

        private void Relmousecap1_Tick(object sender, EventArgs e)
        {
            Main_Menu.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                Mouse.Capture(null);

            }));
        }

        private void LoginAnimate_Tick(object sender, EventArgs e)
        {
            Main_Menu.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                animationTranslateXGrid(Login, 50, 0, 1000);
                animationFadeGrid(Login, 0, 1, 1000);

            }));
        }

//----------------------------------------------Timer Functions end----------------------------------------------\\ 
		

        private void Login_Password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Login_Password.Password == "123" && Login_Username.Text == "kai")
                {
                    Login_Invalid.Text = null;

                    Login_Password.Password = null;
                    Login_Username.Text = null;

                    //call timer to hide rest
                    this.HideAllButMainMenu.Elapsed += new System.Timers.ElapsedEventHandler(this.HideAllButMainMenu_Tick);
                    HideAllButMainMenu.AutoReset = false;
                    HideAllButMainMenu.Enabled = true;
                    HideAllButMainMenu.Start();

                    Mouse.Capture(this);
                    //call timer to release mouse capture
                    this.Relmousecap1.Elapsed += new System.Timers.ElapsedEventHandler(this.Relmousecap1_Tick);
                    Relmousecap1.AutoReset = false;
                    Relmousecap1.Enabled = true;
                    Relmousecap1.Start();

                    animationTranslateXGrid(Login, 0, 100, 1000);
                    animationFadeGrid(Login, 1, 0, 1000);

                    Main_Menu.Visibility = Visibility.Visible;
                    animationFadeGrid(Main_Menu, 0, 1, 4000);

                    animationTranslateXText(Main_Menu_TItle, 50, 0, 1500);
                    animationFadeText(Main_Menu_TItle, 0, 1, 1500);

                    animationTranslateX(Main_Menue_Logout, 100, 0, 2200);
                    animationFade(Main_Menue_Logout, 0, 1, 2200);

                    animationTranslateX(Main_Menu_Product_Manager, 50, 0, 2200);
                    animationFade(Main_Menu_Product_Manager, 0, 1, 2200);

                    animationTranslateX(Main_Menu_Device_Manager, 50, 0, 2200);
                    animationFade(Main_Menu_Device_Manager, 0, 1, 2200);

                    animationScale(Main_Menu_Circle_1, 0.3, 1000);
                    animationFade(Main_Menu_Circle_1, 0, 1, 1000);

                    animationScale(Main_Menu_Circle_2, 0.3, 1300);
                    animationFade(Main_Menu_Circle_2, 0, 1, 1300);


                }
                else
                    Login_Invalid.Text = "Invalid Username/Password";
            }
        }

        private void Main_Menu_Product_Manager_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Main_Menu_Product_Manager, .06, 500);
        }

        private void Main_Menu_Product_Manager_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Main_Menu_Product_Manager, -.06, 500);
        }

        private void Main_Menu_Product_Manager_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Main_menu_Grid.Visibility = Visibility.Visible; need to set timer to set visibility to hidden
            Product_Manager.Visibility = Visibility.Visible;

            //timer to hide other grids
            this.HideAllButProduct.Elapsed += new System.Timers.ElapsedEventHandler(this.HideAllButProduct_Tick);
            HideAllButProduct.AutoReset = false;
            HideAllButProduct.Enabled = true;
            HideAllButProduct.Start();

            Mouse.Capture(this);
            //call timer to release mouse capture
            this.Relmousecap1.Elapsed += new System.Timers.ElapsedEventHandler(this.Relmousecap1_Tick);
            Relmousecap1.AutoReset = false;
            Relmousecap1.Enabled = true;
            Relmousecap1.Start();

            animationFadeGrid(Main_Menu, 1, 0, 1000);

            animationTranslateXText(Main_Menu_TItle, 0, 50, 1500);
            animationFadeText(Main_Menu_TItle, 1, 0, 1500);

            animationTranslateX(Main_Menu_Product_Manager, 0, 50, 2200);
            animationFade(Main_Menu_Product_Manager, 1, 0, 2200);

            animationTranslateX(Main_Menu_Device_Manager, 0, 50, 2200);
            animationFade(Main_Menu_Device_Manager, 1, 0, 2200);

            animationScale(Main_Menu_Circle_1, 0.3, 1000);
            animationFade(Main_Menu_Circle_1, 1, 0, 1000);

            animationScale(Main_Menu_Circle_2, 0.3, 1300);
            animationFade(Main_Menu_Circle_2, 1, 0, 1300);


            animationTranslateXGrid(Product_Manager, 500, 0, 1000);
            animationFadeGrid(Product_Manager, 0, 1, 1000);

            animationTranslateXText(Product_Manager_Title, 50, 0, 2500);
            animationFadeText(Product_Manager_Title, 0, 1, 2500);
        }

        private void Main_Menu_Device_Manager_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Main_Menu_Device_Manager, .06, 500);
        }

        private void Main_Menu_Device_Manager_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Main_Menu_Device_Manager, -.06, 500);
        }

        private void Main_Menu_Device_Manager_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Main_menu_Grid.Visibility = Visibility.Visible; need to set timer to set visibility to hidden
            Device_Manager.Visibility = Visibility.Visible;

            //timer to hide other grids
            this.HideAllButDevice.Elapsed += new System.Timers.ElapsedEventHandler(this.HideAllButDevice_Tick);
            HideAllButDevice.AutoReset = false;
            HideAllButDevice.Enabled = true;
            HideAllButDevice.Start();

            Mouse.Capture(this);
            //call timer to release mouse capture
            this.Relmousecap1.Elapsed += new System.Timers.ElapsedEventHandler(this.Relmousecap1_Tick);
            Relmousecap1.AutoReset = false;
            Relmousecap1.Enabled = true;
            Relmousecap1.Start();

            animationFadeGrid(Main_Menu, 1, 0, 1000);

            animationTranslateXText(Main_Menu_TItle, 0, 50, 1500);
            animationFadeText(Main_Menu_TItle, 1, 0, 1500);

            animationTranslateX(Main_Menu_Product_Manager, 0, 50, 2200);
            animationFade(Main_Menu_Product_Manager, 1, 0, 2200);

            animationTranslateX(Main_Menu_Device_Manager, 0, 50, 2200);
            animationFade(Main_Menu_Device_Manager, 1, 0, 2200);

            animationScale(Main_Menu_Circle_1, 0.3, 1000);
            animationFade(Main_Menu_Circle_1, 1, 0, 1000);

            animationScale(Main_Menu_Circle_2, 0.3, 1300);
            animationFade(Main_Menu_Circle_2, 1, 0, 1300);


            animationTranslateXGrid(Device_Manager, 500, 0, 1000);
            animationFadeGrid(Device_Manager, 0, 1, 1000);

            animationTranslateXText(Device_Manager_Title, 50, 0, 2500);
            animationFadeText(Device_Manager_Title, 0, 1, 2500);
        }

        private void Product_Manager_Return1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Product_Manager_Return1, .06, 500);
        }

        private void Product_Manager_Return1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Product_Manager_Return1, -.06, 500);
        }

        private void Product_Manager_Return1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Main_Menu.Visibility = Visibility.Visible;

            //call timer to hide rest
            this.HideAllButMainMenu.Elapsed += new System.Timers.ElapsedEventHandler(this.HideAllButMainMenu_Tick);
            HideAllButMainMenu.AutoReset = false;
            HideAllButMainMenu.Enabled = true;
            HideAllButMainMenu.Start();

            Mouse.Capture(this);
            //call timer to release mouse capture
            this.Relmousecap1.Elapsed += new System.Timers.ElapsedEventHandler(this.Relmousecap1_Tick);
            Relmousecap1.AutoReset = false;
            Relmousecap1.Enabled = true;
            Relmousecap1.Start();

            animationTranslateXGrid(Product_Manager, 0, 500, 1000);
            animationFadeGrid(Product_Manager, 1, 0, 1000);

            animationFadeGrid(Main_Menu, 0, 1, 4000);

            animationTranslateXText(Main_Menu_TItle, 50, 0, 1500);
            animationFadeText(Main_Menu_TItle, 0, 1, 1500);

            animationTranslateX(Main_Menu_Product_Manager, 50, 0, 2200);
            animationFade(Main_Menu_Product_Manager, 0, 1, 2200);

            animationTranslateX(Main_Menu_Device_Manager, 50, 0, 2200);
            animationFade(Main_Menu_Device_Manager, 0, 1, 2200);

            animationScale(Main_Menu_Circle_1, 0.3, 1000);
            animationFade(Main_Menu_Circle_1, 0, 1, 1000);

            animationScale(Main_Menu_Circle_2, 0.3, 1300);
            animationFade(Main_Menu_Circle_2, 0, 1, 1300);
        }

        private void Device_Manager_Return_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Device_Manager_Return, .06, 500);
        }

        private void Device_Manager_Return_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Device_Manager_Return, -.06, 500);
        }

        private void Device_Manager_Return_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Main_Menu.Visibility = Visibility.Visible;

            //call timer to hide rest
            this.HideAllButMainMenu.Elapsed += new System.Timers.ElapsedEventHandler(this.HideAllButMainMenu_Tick);
            HideAllButMainMenu.AutoReset = false;
            HideAllButMainMenu.Enabled = true;
            HideAllButMainMenu.Start();

            Mouse.Capture(this);
            //call timer to release mouse capture
            this.Relmousecap1.Elapsed += new System.Timers.ElapsedEventHandler(this.Relmousecap1_Tick);
            Relmousecap1.AutoReset = false;
            Relmousecap1.Enabled = true;
            Relmousecap1.Start();

            animationTranslateXGrid(Device_Manager, 0, 500, 1000);
            animationFadeGrid(Device_Manager, 1, 0, 1000);

            animationFadeGrid(Main_Menu, 0, 1, 4000);

            animationTranslateXText(Main_Menu_TItle, 50, 0, 1500);
            animationFadeText(Main_Menu_TItle, 0, 1, 1500);

            animationTranslateX(Main_Menu_Product_Manager, 50, 0, 2200);
            animationFade(Main_Menu_Product_Manager, 0, 1, 2200);

            animationTranslateX(Main_Menu_Device_Manager, 50, 0, 2200);
            animationFade(Main_Menu_Device_Manager, 0, 1, 2200);

            animationScale(Main_Menu_Circle_1, 0.3, 1000);
            animationFade(Main_Menu_Circle_1, 0, 1, 1000);

            animationScale(Main_Menu_Circle_2, 0.3, 1300);
            animationFade(Main_Menu_Circle_2, 0, 1, 1300);
        }

        private void Login_Username_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        	if (Login_Password.Password == "123" && Login_Username.Text == "kai")
            {
                Login_Invalid.Text = null;

                Login_Password.Password = null;
                Login_Username.Text = null;

                //call timer to hide rest
                this.HideAllButMainMenu.Elapsed += new System.Timers.ElapsedEventHandler(this.HideAllButMainMenu_Tick);
                HideAllButMainMenu.AutoReset = false;
                HideAllButMainMenu.Enabled = true;
                HideAllButMainMenu.Start();
                animationTranslateXGrid(Login, 0, 100, 1000);
                animationFadeGrid(Login, 1, 0, 1000);

                Mouse.Capture(this);
                //call timer to release mouse capture
                this.Relmousecap1.Elapsed += new System.Timers.ElapsedEventHandler(this.Relmousecap1_Tick);
                Relmousecap1.AutoReset = false;
                Relmousecap1.Enabled = true;
                Relmousecap1.Start();

                Main_Menu.Visibility = Visibility.Visible;
                animationFadeGrid(Main_Menu, 0, 1, 4000);

                animationTranslateXText(Main_Menu_TItle, 50, 0, 1500);
                animationFadeText(Main_Menu_TItle, 0, 1, 1500);

                animationTranslateX(Main_Menu_Product_Manager, 50, 0, 2200);
                animationFade(Main_Menu_Product_Manager, 0, 1, 2200);

                animationTranslateX(Main_Menu_Device_Manager, 50, 0, 2200);
                animationFade(Main_Menu_Device_Manager, 0, 1, 2200);

                animationScale(Main_Menu_Circle_1, 0.3, 1000);
                animationFade(Main_Menu_Circle_1, 0, 1, 1000);

                animationScale(Main_Menu_Circle_2, 0.3, 1300);
                animationFade(Main_Menu_Circle_2, 0, 1, 1300);


            }
        }

        private void Login_Username_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Login_Username.Text == "Guest")
            Login_Username.Text = null;
        }

        private void Login_Username_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Login_Username.Text.Length == 0)
            Login_Username.Text = "Guest";
        }

        private void Login_Password_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Login_Password.Password == "12345678")
            Login_Password.Password = null;
        }

        private void Login_Password_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Login_Password.Password.Length == 0)
            Login_Password.Password = "12345678";
        }

        private void Image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Main_Menue_Logout, -.06, 500);
        }

        private void Image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animationScale(Main_Menue_Logout, .06, 500);
        }

        private void Image_ContextMenuClosing(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            
        }

        private void Main_Menue_Logout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Main_menu_Grid.Visibility = Visibility.Visible; need to set timer to set visibility to hidden
            Login.Visibility = Visibility.Visible;
			
			Login_Password.Password = "12345678";
            Login_Username.Text = "Guest";

            //timer to hide other grids
            this.HideAllButLogin.Elapsed += new System.Timers.ElapsedEventHandler(this.HideAllButLogin_Tick);
            HideAllButLogin.AutoReset = false;
            HideAllButLogin.Enabled = true;
            HideAllButLogin.Start();

            Mouse.Capture(this);
            //call timer to release mouse capture
            this.Relmousecap1.Elapsed += new System.Timers.ElapsedEventHandler(this.Relmousecap1_Tick);
            Relmousecap1.AutoReset = false;
            Relmousecap1.Enabled = true;
            Relmousecap1.Start();

            this.LoginAnimate.Elapsed += new System.Timers.ElapsedEventHandler(this.LoginAnimate_Tick);
            LoginAnimate.AutoReset = false;
            LoginAnimate.Enabled = true;
            LoginAnimate.Start();

            animationFadeGrid(Main_Menu, 1, 0, 1000);

            animationTranslateXText(Main_Menu_TItle, 0, 50, 1500);
            animationFadeText(Main_Menu_TItle, 1, 0, 1500);

            animationTranslateX(Main_Menu_Product_Manager, 0, 50, 1700);
            animationFade(Main_Menu_Product_Manager, 1, 0, 1700);

            animationTranslateX(Main_Menu_Device_Manager, 0, 50, 1700);
            animationFade(Main_Menu_Device_Manager, 1, 0, 1700);

            animationScale(Main_Menu_Circle_1, 0.3, 1000);
            animationFade(Main_Menu_Circle_1, 1, 0, 1000);

            animationScale(Main_Menu_Circle_2, 0.3, 1300);
            animationFade(Main_Menu_Circle_2, 1, 0, 1300);

        }

        private void Update_Server_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void Sync_To_Server_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

    }

}