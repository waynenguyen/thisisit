using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Member_Class;
using Transaction_Class;
using Product_Class;

namespace CEGMarketSystem_Class
{
    class CEGMarketSystem
    {
        public CEGMarketSystem() { 
        
            //to be implemented
        }
        public void initialize()
        {
            //testing only
            Product book = new Product("123", "C# for dummies", "Dummies", "McGrawHill", 23, 5);
            Member TuanVu = new Member("G12345678", "ngotuanvu@gmail.com", "Tuan Vu", "82255790");
            Transaction asd = new Transaction("1", "3/10/2011", "G12345678", 10, 2, 8);
        }
        public void CalculateProductProfit(string barc)
        { 
            //to be implemented        
        }
        public void CalculateTotalProfit()
        { 
            //to be implemented
        }
    }
}
