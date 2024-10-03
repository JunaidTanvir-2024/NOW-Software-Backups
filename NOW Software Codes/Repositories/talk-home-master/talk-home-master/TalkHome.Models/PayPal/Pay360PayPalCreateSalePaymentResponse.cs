using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.Pay360;

namespace TalkHome.Models.PayPal
{
   public class Pay360PayPalCreateSalePaymentResponse
    {
         public string customerId { get; set; }
        public string transactionId { get; set; }
        public string transactionAmount { get; set; }
        public outcome outcome { get; set; }
        public string clientRedirectUrl { get; set; }
        public string checkoutToken { get; set; }
    }
}
