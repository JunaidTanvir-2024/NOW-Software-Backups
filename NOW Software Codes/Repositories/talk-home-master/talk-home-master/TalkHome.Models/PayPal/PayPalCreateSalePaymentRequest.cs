using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.PayPal
{
    public class PayPalCreateSalePaymentRequest
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

        [JsonProperty("productCode")]
        [Required]
        public string ProductCode { get; set; }

        [JsonProperty("redirect_urls")]
        [Required]
        public RedirectUrls RedirectUrl { get; set; }

        [JsonProperty("transactions")]
        [Required]
        public Transactions Transaction { get; set; }

        [JsonProperty("basket")]
        [Required]
        public List<ProductBasket> Basket { get; set; }
    }

    public class ProductBasket
    {
        [JsonProperty("productItemCode")]
        [Required]
        public string ProductItemCode { get; set; }

        [JsonProperty("amount")]
        [Required]
        public float Amount { get; set; }

        [JsonProperty("productRef")]
        public string ProductRef { get; set; }

        [JsonProperty("bundleRef")]
        public string BundleRef { get; set; }

    }

    public class RedirectUrls
    {
        [JsonProperty("return_url")]
        [Required]
        public string ReturnUrl { get; set; }

        [JsonProperty("cancel_url")]
        [Required]
        public string CancelUrl { get; set; }
    }

    public class Transactions
    {
        [JsonProperty("amount")]
        [Required]
        public Amounts Amount { get; set; }

        [JsonProperty("description")]
        [Required]
        public string Description { get; set; }
    }
    public class Amounts
    {
        [JsonProperty("total")]
        [Required]
        public float Total { get; set; }

        [JsonProperty("currency")]
        [Required]
        public string Currency { get; set; }
    }
}
