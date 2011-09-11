﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Product_Class
{
    class Product
    {
        private string _barcode, _name, _category, _manufacturer;
        private double _price;
        private int _number_in_stock;

        public Product(string barc, string name, string category, string manufacturer, double price, int number_in_stock)
        {
            _barcode = barc;
            _name = name;
            _category = category;
            _manufacturer = manufacturer;
            _price = price;
            _number_in_stock = number_in_stock;
        }

        public string getBarcode()
        {
            return _barcode;
        }
        public string getName()
        {
            return _name;
        }
        public string getCategory()
        {
            return _category;
        }
        public string getManufacturer()
        {
            return _manufacturer;
        }
        public double getPrice()
        {
            return _price;
        }
        public int getNumberInStock()
        {
            return _number_in_stock;
        }

        // setters
        public void setBarcode(string barcode)
        {
            _barcode = barcode;
        }
        public void setName(string name)
        {
            _name = name;
        }
        public void setCategory(string category)
        {
            _category = category;
        }
        public void setManufacturer(string manufacturer)
        {
            _manufacturer = manufacturer;
        }
        public void setPrice(double price)
        {
            _price = price;
        }
        public void setNumberInStock(int num)
        {
            _number_in_stock = num;
        }

    }
}
