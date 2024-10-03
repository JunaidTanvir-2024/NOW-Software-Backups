using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.PayPal
{
    public class PayPalExecuteSalePaymentResponse
    {
        [JsonProperty("paypalTransactionId")]
        public string PaypalTransactionId { get; set; }

        [JsonProperty("createTime")]
        public DateTime CreateTime { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("saleId")]
        public string SaleId { get; set; }

        [JsonProperty("basketResponse")]
        public List<BasketItemsResponse> BasketResponse { get; set; }
    }

    public class BasketItemsResponse
    {
        public string ProductItemCode { get; set; }
        public bool IsFullFilled { get; set; }
        public string Pin { get; set; }
    }
}
