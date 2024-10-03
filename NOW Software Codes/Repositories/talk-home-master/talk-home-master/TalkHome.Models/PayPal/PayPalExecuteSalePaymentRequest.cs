using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.PayPal
{
    public class PayPalExecuteSalePaymentRequest
    {
        [JsonProperty("payer_id")]
        [Required]
        public string PayerId { get; set; }


        [JsonProperty("payment_id")]
        [Required]
        public string PaymentId { get; set; }

        [JsonProperty("customerUniqueRef")]
        [Required]
        public string CustomerUniqueRef { get; set; }

        [JsonProperty("productCode")]
        [Required]
        public string ProductCode { get; set; }
    }
}
