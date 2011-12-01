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
        public string itemId { get; set; }
        public int num { get; set; }
    }

    class dataSender
    {
        public string storeId;
        public List<productDataToSend> data;
    }
    static class HQServerInterface
    {
        public const string HQ_updateURL = "http://cegmarket.appspot.com/store/update";
        
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
                finalReport.Add(new productDataToSend{itemId=todayReport.ElementAt(i).getBarcode(),num=todayReport.ElementAt(i).getNumberSoldToday()});
            }
            dataSender dataSend = new dataSender();
            dataSend.storeId = "11001";
            dataSend.data = finalReport;
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(finalReport);
            string sJSON = oSerializer.Serialize(dataSend);

            //HTTPGet req = new HTTPGet();
            //string reqString = HQ_updateURL + "content=" + sJSON;
            //req.Request(reqString);
            //Console.WriteLine(req.StatusLine);
            //Console.WriteLine(req.ResponseTime);

            PostSubmitter postReq = new PostSubmitter();
            postReq.Url = HQ_updateURL;
            postReq.PostItems.Add("content", sJSON);
            string result = postReq.Post();
            /* HTTPPost example
            PostSubmitter post = new PostSubmitter();
            //post.Url = "http://ec2-50-17-68-237.compute-1.amazonaws.com/2102/post/14";
            post.Url = "http://3B.cegmarket.appspot.com/store/update?id=11001&from=&to";
            post.PostItems.Add("content", sJSON);
            //post.PostItems.Add("rel_code", "1102");
            //post.PostItems.Add("FREE_TEXT", "c# jobs");
            //post.PostItems.Add("SEARCH", "");
            post.Type = PostSubmitter.PostTypeEnum.Post;

            string result = post.Post();
            */

            // TODO:


        }

        public static void sync()
        {
            HTTPGet req = new HTTPGet();
            req.Request("http://cegmarket.appspot.com/store/sync?id=11001&from=0&to=100");
            Console.WriteLine(req.StatusLine);
            Console.WriteLine(req.ResponseTime);


        }



    }
}
