using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Product_Class;
//using Json;
using Newtonsoft.Json;
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
        public string time;
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
            dataSend.time = DateTime.Now.ToString();
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(finalReport);
            string sJSON = oSerializer.Serialize(dataSend);

            

            PostSubmitter postReq = new PostSubmitter();
            postReq.Url = HQ_updateURL;
            postReq.PostItems.Add("", sJSON);
            string result = postReq.Post();

            // update again

            HTTPGet req = new HTTPGet();
            string reqString = HQ_updateURL + "?id=11001&from=0&to=100";
            req.Request(reqString);
            Console.WriteLine(req.StatusLine);
            Console.WriteLine(req.ResponseTime);




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
            // reset local DB
            LocalDBInterface.reset();
            HTTPGet req = new HTTPGet();
            List<Product> listP = new List<Product>();
            string syncURL = "http://cegmarket.appspot.com/store/sync?id=11001&";
            int start = 0, end = 10000;

            while (true)
            {
                string fromString = "from=";
                string toString = "to=";
                fromString = fromString + start.ToString() + "&";
                toString = toString + end.ToString();
                req.Request(syncURL+fromString+toString);
                if (req.StatusCode == 204) break;
                start = end;
                end = end + 10000;
                Console.WriteLine(req.StatusLine);
                Console.WriteLine(req.ResponseTime);                
                //Json.JsonArray data = JsonParser.Deserialize(req.ResponseBody);
                //System.Collections.IEnumerator ite = data.GetEnumerator();
                string jsonString = req.ResponseBody;
                jsonString = jsonString.Replace("id", "barcode");
                jsonString = jsonString.Replace("brand", "manufacturer");
                jsonString = jsonString.Replace("transistNum", "number_in_stock");
                jsonString = jsonString.Replace("transistPrice", "price");

                dynamic deserializedProduct = (List<Product>)JsonConvert.DeserializeObject<List<Product>>(jsonString);
                listP.AddRange(deserializedProduct);
            }
            LocalDBInterface.addListProduct(listP);
        }



    }
}
