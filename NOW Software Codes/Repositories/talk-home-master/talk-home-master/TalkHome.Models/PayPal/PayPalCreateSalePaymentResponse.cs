using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.PayPal
{
    public class PayPalCreateSalePaymentResponse
    {
        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }
    }


}
