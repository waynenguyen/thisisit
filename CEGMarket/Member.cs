using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Member_Class
{
    class Member
    {
        private string _fin_number, _email, _name, _phone_number;
        public Member(string fin_number, string email, string name, string phone_number)
        {
            _fin_number = fin_number;
            _email = email;
            _name = name;
            _phone_number = phone_number;
        }
        //getters
        public string getFinNumber()
        {
            return _fin_number;
        }
        public string getEmail()
        {
            return _email;
        }
        public string getName()
        {
            return _name;
        }
        public string getPhoneNumber()
        {
            return _phone_number;
        }
        //setters
        public void setFinNumber(string finnumber)
        {
            _fin_number = finnumber;
        }
        public void setEmail(string email)
        {
            _email = email;
        }
        public void setName(string name)
        {
            _name = name;
        }
        public void setPhoneNumber(string phonenumber)
        {
            _phone_number = phonenumber;
        }
    }
}
