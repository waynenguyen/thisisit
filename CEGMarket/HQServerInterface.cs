using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Product_Class;

namespace CEGMarket
{
    
    class productDataToSend
    {
        public string barcode { get; set; }
        public int sold_number { get; set; }
    }
    static class HQServerInterface
    {
        
        
        public static void sendTodayReport()
        {
            // TODO: call LocalDBInterface.getProductSoldToday()
            // parse Product id + number of stock sold today
            // convert to JSON string
            // make a post request, data: "content=" + jsonstring
            List<Product> todayReport = LocalDBInterface.getProductSoldToday();
            List<productDataToSend> finalReport = new List<productDataToSend>();
            for (int i = 0; i < todayReport.Count; i++)
            {
                finalReport.Add(new productDataToSend{barcode=todayReport.ElementAt(i).getBarcode(),sold_number=todayReport.ElementAt(i).getNumberSoldToday()});
            }
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(finalReport);

            PostSubmitter post = new PostSubmitter();
            post.Url = "http://ec2-50-17-68-237.compute-1.amazonaws.com/2102/post/14";
            post.PostItems.Add("content", sJSON);
            //post.PostItems.Add("rel_code", "1102");
            //post.PostItems.Add("FREE_TEXT", "c# jobs");
            //post.PostItems.Add("SEARCH", "");
            post.Type = PostSubmitter.PostTypeEnum.Post;

            string result = post.Post();
          
        }





    }
}
