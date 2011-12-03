using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Product_Class;
using Transaction_Class;
namespace CEGMarket
{
    public struct ProductSellInfo
    {
        public string barcode;
        public int amount;
        public string date;
        public double price;

    }

    public struct CategoryManufacturerSellInfo
    {
        public string name;
        public int amount;
        public string date;
    }
    static class LocalDBInterface
    {
        private static MySqlConnection l_DBConn;
        private static void openConnection()
        {
            l_DBConn = new MySqlConnection();
            l_DBConn.ConnectionString = Properties.Settings.Default.DBConnectionString;
            l_DBConn.Open();
        }
        private static void closeConnection()
        {
            l_DBConn.Close();
        }

        public static void reset()
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "UPDATE product SET is_sold_today= 'FALSE', number_in_stock = 0, number_sold_today = 0";
            command.CommandText = query;
            command.ExecuteNonQuery();
            closeConnection();
        }
        // PRODUCT RELATED INTERFACE



        public static List<string> convertListProductToName(List<Product> listP)
        {
            List<string> listName = new List<string>();
            for (int i = 0; i < listP.Count; i++)
            {
                listName.Add(listP.ElementAt(i).getName());
            }
            return listName;
        }

        // TODO : add reset is_sold_today and number_sold_today
        public static void addProduct(Product newProduct)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            try
            {
                String query = "REPLACE product(barcode,number_in_stock,name,category,price,manufacturer) VALUES('" + newProduct.getBarcode() + "','" + newProduct.getNumberInStock().ToString() +
                                "','" + newProduct.getName() + "','" + newProduct.getCategory() +
                                "','" + newProduct.getPrice().ToString() + "','" + newProduct.getManufacturer() + "')";
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            catch { };
            closeConnection();
        }

        public static void addListProduct(List<Product> listProduct)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            for (int i = 0; i < listProduct.Count; i++)
            {
                Product newProduct = listProduct.ElementAt(i);
                try
                {
                    String query = "REPLACE product(barcode,number_in_stock,name,category,price,manufacturer) VALUES('" + newProduct.getBarcode() + "','" + newProduct.getNumberInStock().ToString() +
                                    "','" + newProduct.getName() + "','" + newProduct.getCategory() +
                                    "','" + newProduct.getPrice().ToString() + "','" + newProduct.getManufacturer() + "')";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                catch { };
            }
            closeConnection();
        }
        public static Product getProduct(String barcode)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "SELECT * FROM product WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int number_in_stock = (int)Int64.Parse(reader.GetValue(1).ToString());
                string name = reader.GetValue(2).ToString();
                string category = reader.GetValue(3).ToString();
                double price =  Double.Parse(reader.GetValue(4).ToString());
                string manufacturer = reader.GetValue(5).ToString();
                int number_sold_today = (int) Int64.Parse(reader.GetValue(7).ToString());
                Product newProduct = new Product(barcode, name, category, manufacturer, price, number_in_stock);
                newProduct.setNumberSoldToday(number_sold_today);
                reader.Close();
                return (newProduct);
            }
            reader.Close();
            closeConnection();
            return null;
        }
        public static void modifyProduct(Product changedProduct)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            try
            {
                String query = "UPDATE product SET number_in_stock='" + changedProduct.getNumberInStock() +
                                "' ,name='" + changedProduct.getName() + "' ,category='" + changedProduct.getCategory() +
                                "' ,price='" + changedProduct.getPrice() + "' ,manufacturer='" + changedProduct.getManufacturer() +
                                "' WHERE barcode ='" + changedProduct.getBarcode() + "'";
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            catch { };
            closeConnection();
        }

        public static List<Product> getProductSoldToday()
        {
            openConnection();
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;

            query = "SELECT barcode FROM product WHERE is_sold_today='TRUE'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            List<string> barcode_list = new List<string>();
            while (reader.Read())
            {
                barcode_list.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            for (int i = 0; i < barcode_list.Count; i++)
            {
                productSellInfo.Add(getProduct(barcode_list.ElementAt(i)));
            }
            closeConnection();
            return productSellInfo;
        }

        public static void addProductNumberSoldToday(string barcode, int number_sold_today)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "SELECT * FROM product WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int number_s_today = (int)Int64.Parse(reader.GetValue(7).ToString());
                number_sold_today += number_s_today;
            }
            reader.Close();
            query = "UPDATE product SET number_sold_today='" + number_sold_today.ToString() +
                            "' WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
            closeConnection();
        }
        public static void setProductNumberInStock(string barcode, int number_in_stock)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "UPDATE product SET number_in_stock='" + number_in_stock +
                            "' WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
            closeConnection();
        }

        // means set current number_in_stock += added_number_in_stock
        public static void addProductNumberInStock(string barcode, int added_number_in_stock)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "SELECT * FROM product WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int number_in_stock = (int)Int64.Parse(reader.GetValue(1).ToString());
                added_number_in_stock += number_in_stock;
            }
            reader.Close();
            query = "UPDATE product SET number_in_stock='" + added_number_in_stock.ToString() +
                            "' WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
            closeConnection();
        }



        public static void removeProductNumberInStock(string barcode, int removed_number_in_stock)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "SELECT * FROM product WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int number_in_stock = (int)Int64.Parse(reader.GetValue(1).ToString());
                removed_number_in_stock = number_in_stock - removed_number_in_stock;
            }
            reader.Close();
            query = "UPDATE product SET number_in_stock='" + removed_number_in_stock.ToString() +
                            "', is_sold_today='TRUE' WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
            closeConnection();
        }
        



        // TRANSACTION RELATED INTERFACE
        public static void addTransaction(Transaction transaction)
        {
            openConnection();
            // TODO: adjust price of product
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;
            if (transaction.getMemberId()!=null)
                query = "INSERT INTO transaction(money_receive,money_change,date,member_id,total_price) VALUES('" + 
                            transaction.getMoneyReceive() + "','" + transaction.getMoneyChange() +
                            "','" + transaction.getDate() + "','" + transaction.getMemberId() +
                            "','" + transaction.getTotalPrice() + "')";
            else
                query = "INSERT INTO transaction(money_receive,money_change,date,total_price) VALUES('" +
                            transaction.getMoneyReceive() + "','" + transaction.getMoneyChange() +
                            "','" + transaction.getDate() + "','" + transaction.getTotalPrice() + "')";
            command.CommandText = query;
            command.ExecuteNonQuery();
            var transactionId = command.LastInsertedId;
            
            // add Transaction-Product relation
            Dictionary<string, Dictionary<int,double>> shopping_bag = transaction.getShoppingBag();
            for (int i = 0; i < shopping_bag.Count; i++)
            {
                string productId = shopping_bag.ElementAt(i).Key;
                Dictionary<int, double> amountPrice = shopping_bag.ElementAt(i).Value;
                query = "INSERT INTO product_transaction_relation VALUES('" +
                            productId + "','" + transactionId +
                            "','" + amountPrice.ElementAt(0).Key +
                            "','" + amountPrice.ElementAt(0).Value + "')";
                command.CommandText = query;
                command.ExecuteNonQuery();

                // remove number in stock of product
                removeProductNumberInStock(productId,amountPrice.ElementAt(i).Key);

                // add number of product sold today

            }
            closeConnection();
        }

        public static Transaction getTransaction(string transactionId)
        {
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;
            query = "SELECT * FROM transaction WHERE id='" + transactionId + "'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            Transaction transaction = null;
            while (reader.Read())
            {
                double money_receive = Double.Parse(reader.GetValue(1).ToString());
                double money_change = Double.Parse(reader.GetValue(2).ToString());
                string date = reader.GetValue(3).ToString();
                string member_id = reader.GetValue(4).ToString();
                double total_price = Double.Parse(reader.GetValue(5).ToString());
                transaction = new Transaction(transactionId, date, money_receive, money_change, total_price);
            }
            reader.Close();
            query = "SELECT * FROM product_transaction_relation WHERE transaction_id='" + transactionId + "'";
            command.CommandText = query;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                string product_id = reader.GetValue(0).ToString();
                int amount = (int)Int64.Parse(reader.GetValue(2).ToString());
                double total_price = Double.Parse(reader.GetValue(3).ToString());
                transaction.insertProductIntoShoppingBag(product_id, amount, total_price);
            }
            reader.Close();
            closeConnection();
            return transaction;
        }


        // REPORT GENERATION MODULE
       
        // dayValue should have type "YYYY-MM-DD"
        public static List<Product> getProductDayReport(string dayValue)
        {
            openConnection();
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;

            query = "SELECT distinct barcode FROM product p, transaction t, product_transaction_relation r WHERE p.barcode = r.product_id AND t.id = r.transaction_id AND t.date ='" + dayValue + "' AND t.status='COMPLETED'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            List<string> barcode_list = new List<string>();
            while (reader.Read())
            {
                barcode_list.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            for (int i=0; i< barcode_list.Count; i++)
            {
                productSellInfo.Add(getProduct(barcode_list.ElementAt(i)));
            }

            closeConnection();
            return productSellInfo;
        }












        
        // 
        public static List<Product> getProductByCategory(string categoryName)
        {
            openConnection();
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;
            
            query = "SELECT barcode FROM product WHERE category='" + categoryName + "'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            List<string> barcode_list = new List<string>();
            while (reader.Read())
            {
                barcode_list.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            for (int i=0; i< barcode_list.Count; i++)
            {
                productSellInfo.Add(getProduct(barcode_list.ElementAt(i)));
            }

            closeConnection();
            return productSellInfo;
        }


        // TODO:
        public static List<Product> getCategoryDayReport(string categoryName, string dayValue)
        {
            openConnection();
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;
            
            query = "SELECT distinct barcode FROM product p, transaction t, product_transaction_relation r WHERE t.date='" + dayValue + "' AND p.category='" + categoryName + "' AND r.product_id = p.barcode AND r.transaction_id = t.id AND t.status='COMPLETED'"; 
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            List<string> barcode_list = new List<string>();
            while (reader.Read())
            {
                barcode_list.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            for (int i=0; i< barcode_list.Count; i++)
            {
                productSellInfo.Add(getProduct(barcode_list.ElementAt(i)));
            }

            closeConnection();
            return productSellInfo;
        }

        
        
        

         public static List<Product> getProductByManufacturer(string manufacturerName)
        {
            openConnection();
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;
            
            query = "SELECT barcode FROM product WHERE manufacturer='" + manufacturerName + "'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            List<string> barcode_list = new List<string>();
            while (reader.Read())
            {
                barcode_list.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            for (int i=0; i< barcode_list.Count; i++)
            {
                productSellInfo.Add(getProduct(barcode_list.ElementAt(i)));
            }

            closeConnection();
            return productSellInfo;
        }


        // TODO:
        public static List<Product> getManufacturerDayReport(string manufacturerName, string dayValue)
        {
            openConnection();
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;

            query = "SELECT distinct barcode FROM product p, transaction t, product_transaction_relation r WHERE t.date='" + dayValue + "' AND p.manufacturer='" + manufacturerName + "' AND r.product_id = p.barcode AND r.transaction_id = t.id AND t.status='COMPLETED'"; 
            command.CommandText = query;    
            MySqlDataReader reader = command.ExecuteReader();
            List<string> barcode_list = new List<string>();
            while (reader.Read())
            {
                barcode_list.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            for (int i=0; i< barcode_list.Count; i++)
            {
                productSellInfo.Add(getProduct(barcode_list.ElementAt(i)));
            }

            closeConnection();
            return productSellInfo;
        }

        public static List<string> getListManufacturer()
        {
            List<string> listM = new List<string>();
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "SELECT distinct manufacturer FROM `local_market`.`product`";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string name = reader.GetValue(0).ToString();
                listM.Add(name);
            }
            reader.Close();
            closeConnection();

            return listM;
        }

        public static List<string> getListCategory()
        {
            List<string> listC = new List<string>();
            openConnection();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "SELECT distinct category FROM `local_market`.`product`";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {                
                string name = reader.GetValue(0).ToString();
                listC.Add(name);
            }
            reader.Close();
            closeConnection();
            return listC;            
        }

    }
}
