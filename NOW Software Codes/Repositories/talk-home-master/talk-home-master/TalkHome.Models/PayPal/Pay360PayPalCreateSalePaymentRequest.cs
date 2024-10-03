using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.Pay360;

namespace TalkHome.Models.PayPal
{
    public class Pay360PayPalCreateSalePaymentRequest
    {
        [JsonProperty("customerName")]
        [Required]
        public string CustomerName { get; set; }

        [JsonProperty("customerUniqueRef")]
        [Required]
        public string CustomerUniqueRef { get; set; }

        [JsonProperty("customerMsisdn")]
        public string CustomerMsisdn { get; set; }

        [JsonProperty("CustomerEmail")]
        public string CustomerEmail { get; set; }

        [JsonProperty("transactionCurrency")]
        public string transactionCurrency { get; set; }

        [JsonProperty("transactionAmount")]
        public float transactionAmount { get; set; }

        [JsonProperty("isDirectFullfilment")]
        public bool isDirectFullfilment { get; set; }

        [JsonProperty("ipAddress")]
        public string ipAddress { get; set; }

        [JsonProperty("productCode")]
        [Required]
        public string ProductCode { get; set; }

        [Required]
        public paymentMethod paymentMethod { get; set; }

        [JsonProperty("basket")]
        [Required]
        public List<ProductBasket> Basket { get; set; }
        public customerBillingAddress customerBillingAddress { get; set; }
        public billingAddress shippingAddress { get; set; }
        public billingAddress billingAddress { get; set; }

        public customFields customFields { get; set; }
    }


    public class paymentMethod
    {
        public paypal paypal { get; set; }
    }

    public class paypal
    {
        [Required]
        public string returnUrl { get; set; }

        [Required]
        public string cancelUrl { get; set; }
    }
}
