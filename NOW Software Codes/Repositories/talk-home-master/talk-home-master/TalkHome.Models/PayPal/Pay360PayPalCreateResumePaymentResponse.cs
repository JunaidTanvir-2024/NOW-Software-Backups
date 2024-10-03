using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.Pay360;

namespace TalkHome.Models.PayPal
{
    public class Pay360PayPalCreateResumePaymentResponse
    {
        public string customerId { get; set; }
        public string transactionId { get; set; }
        public string transactionAmount { get; set; }
        public outcome outcome { get; set; }

        public Pay360ResponsepaymentMethod paymentMethod { get; set; }

        public List<Pay360BasketItemsResponse> BasketResponse { get; set; }


        public string pay360ApiCode { get; set; }
     
    }

    public class Pay360BasketItemsResponse
    {
        public string ProductItemCode { get; set; }
        public bool IsFullFilled { get; set; }
        public string Pin { get; set; }
        public string card { get; set; }
    }
    public class Pay360ResponsepaymentMethod
    {
        public Pay360Responsepaypal paypal { get; set; }
        public string paymentClass { get; set; }
    }
    public class Pay360Responsepaypal
    {
        public string payerID { get; set; }
        public string email { get; set; }
        public bool accountVerified { get; set; }
        public string checkoutToken { get; set; }
        public string source { get; set; }
    }

}
