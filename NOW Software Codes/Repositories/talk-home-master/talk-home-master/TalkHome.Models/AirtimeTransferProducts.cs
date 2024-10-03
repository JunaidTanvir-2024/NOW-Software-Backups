using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models
{
    public class AirtimeTransferProducts
    {
        public string message { get; set; }
        public string status { get; set; }
        public int errorCode { get; set; }
        public payload payload { get; set; }
    }

    public class AirtimeProduct
    {
        public string clientccy { get; set; }
        public string receiverccy { get; set; }
        public string product { get; set; }
        public string itemPriceClientccy { get; set; }
        public string transactionfeeClientccy { get; set; }
        public string totalPriceClientccy { get; set; }
    }

    public class operators
    {
        public string id { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string nowtelTransactionReference { get; set; }
        public string iconUri { get; set; }
        public IList<AirtimeProduct> products { get; set; }
    }

    public class payload
    {
        public IList<operators> operators { get; set; }
    }

}
