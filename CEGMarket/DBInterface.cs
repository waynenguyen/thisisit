using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Product_Class;
using Transaction_Class;
namespace CEGMarket
{
    static class DBInterface
    {
        private static MySqlConnection l_DBConn;
        public static void openConnection()
        {
            l_DBConn = new MySqlConnection();
            l_DBConn.ConnectionString = Properties.Settings.Default.DBConnectionString;
            l_DBConn.Open();
        }
        public static void closeConnection()
        {
            l_DBConn.Close();
        }

        // PRODUCT RELATED INTERFACE
        public static void addProduct(Product newProduct)
        {
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "INSERT INTO product VALUES('" + newProduct.getBarcode() + "','0" +
                            "','" + newProduct.getName() + "','" + newProduct.getCategory() +
                            "','" + newProduct.getPrice() + "','" + newProduct.getManufacturer() + "')";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }
        public static Product getProduct(String barcode)
        {
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
                return (new Product(barcode, name, category, manufacturer, price, number_in_stock));
            }
            return null;
        }
        public static void modifyProduct(Product changedProduct)
        {
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "UPDATE product SET number_in_stock='" + changedProduct.getNumberInStock() +
                            "' name='" + changedProduct.getName() + "' category='" + changedProduct.getCategory() +
                            "' price='" + changedProduct.getPrice() + "' manufacturer='" + changedProduct.getManufacturer() +
                            "WHERE barcode ='" + changedProduct.getBarcode() + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }
        

        // TRANSACTION RELATED INTERFACE
        public static void addTransaction(Transaction transaction)
        {
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
            }
        }

        public static Transaction getTransaction(string transactionId)
        {
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
            return transaction;
        }


    }
}
