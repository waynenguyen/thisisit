﻿using System;
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

        // TODO : add reset is_sold_today and number_sold_today

        public static void addProduct(Product newProduct)
        {
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "INSERT INTO product VALUES('" + newProduct.getBarcode() + "','" + newProduct.getNumberInStock().ToString() +
                            "','" + newProduct.getName() + "','" + newProduct.getCategory() +
                            "','" + newProduct.getPrice().ToString() + "','" + newProduct.getManufacturer() + "')";
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

        public static List<Product> getProductSoldToday()
        {
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
            return productSellInfo;
        }

        public static void addProductNumberSoldToday(string barcode, int number_sold_today)
        {
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
                            "WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }
        public static void setProductNumberInStock(string barcode, int number_in_stock)
        {
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "UPDATE product SET number_in_stock='" + number_in_stock +
                            "WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }


        public static void addProductNumberInStock(string barcode, int added_number_in_stock)
        {
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
                            "WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }



        public static void removeProductNumberInStock(string barcode, int removed_number_in_stock)
        {
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = "SELECT * FROM product WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int number_in_stock = (int)Int64.Parse(reader.GetValue(1).ToString());
                removed_number_in_stock -= number_in_stock;
            }
            reader.Close();
            query = "UPDATE product SET number_in_stock='" + removed_number_in_stock.ToString() +
                            "', is_sold_today='TRUE' WHERE barcode ='" + barcode + "'";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }
        



        // TRANSACTION RELATED INTERFACE
        public static void addTransaction(Transaction transaction)
        {
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


        // REPORT GENERATION MODULE
       
        // dayValue should have type "YYYY-MM-DD"
        public static List<Product> getProductDayReport(string dayValue)
        {
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;
            
            query = "SELECT distinct barcode FROM product p, transaction t, product_transaction_relation r WHERE p.barcode = r.product_id AND t.id = r.transaction_id AND t.date ='" + dayValue + "'";
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


            return productSellInfo;
        }












        
        // 
        public static List<Product> getProductByCategory(string categoryName)
        {
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


            return productSellInfo;
        }


        // TODO:
        public static List<Product> getCategoryDayReport(string categoryName, string dayValue)
        {
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;
            
            query = "SELECT distinct barcode FROM product p, transaction t, product_transaction_relation r WHERE t.date='" + dayValue + "' AND p.category='" + categoryName + "' AND r.product_id = p.barcode AND r.transaction_id = t.id"; 
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


            return productSellInfo;
        }

        
        
        

         public static List<Product> getProductByManufacturer(string manufacturerName)
        {
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


            return productSellInfo;
        }


        // TODO:
        public static List<Product> getManufacturerDayReport(string manufacturerName, string dayValue)
        {
            List<Product> productSellInfo = new List<Product>();
            MySqlCommand command = l_DBConn.CreateCommand();
            String query = null;
            
            query = "SELECT distinct barcode FROM product p, transaction t, product_transaction_relation r WHERE t.date='" + dayValue + "' AND p.manufacturer='" + manufacturerName + "' AND r.product_id = p.barcode AND r.transaction_id = t.id"; 
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


            return productSellInfo;
        }


    }
}
