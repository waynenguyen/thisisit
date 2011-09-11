using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transaction_Class
{
    class Transaction
    {
        private string _id, _date, _member_id;
        private double _money_receive, _money_change, _total_price;
        private Dictionary<string, int> _shopping_bag;
        public Transaction(string id, string date, string member_id, double money_receive, double money_change, double total_price)
        {
            _id = id;
            _date = date;
            _member_id = member_id;
            _money_receive = money_receive;
            _money_change = money_change;
            _total_price = total_price;
        }
        // getters
        public String getId()
        {
            return _id;
        }
        public String getDate()
        {
            return _date;
        }
        public String getMemberId()
        {
            return _member_id;
        }
        public double getMoneyReceive()
        {
            return _money_receive;
        }
        public double getMoneyChange()
        {
            return _money_change;
        }
        public double getTotalPrice()
        {
            return _total_price;
        }
        public Dictionary<string, int> getShoppingBag()
        {
            return _shopping_bag;
        }
        //setters
        public void setId(String id)
        {
            _id = id;
        }
        public void setDate(string date)
        {
            _date = date;
        }
        public void setMemberId(string id)
        {
            _member_id = id;
        }
        public void setMoneyReceive(double amount)
        {
            _money_receive = amount;
        }
        public void setMoneyChange(double amount)
        {
            _money_change = amount;
        }
        public void setTotalPrice(double amount)
        {
            _total_price = amount;
        }
        public void insertProductIntoShoppingBag(string productId, int amount)
        {
            _shopping_bag.Add(productId, amount);
        }
    }
}
