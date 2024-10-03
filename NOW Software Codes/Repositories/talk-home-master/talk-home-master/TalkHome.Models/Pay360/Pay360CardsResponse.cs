using Newtonsoft.Json;
using System.Collections.Generic;
//
namespace TalkHome.Models.Pay360
{

    public class card
    {
        public string cardFingerprint { get; set; }
        public string cardToken { get; set; }
        public string cardType { get; set; }
        [JsonProperty(PropertyName = "new")]
        public bool newCard { get; set;} 
        public string cardUsageType { get; set; }
        public string cardScheme { get; set; }
        public string maskedPan { get; set; }
        public string expiryDate { get; set; }
        public string issuer { get; set; }
        public string issuerCountry { get; set; }
        public string cardHolderName { get; set; }

    }
    
    public class paymentMethodResponse
    {
        public bool registered { get; set; }
        public bool isPrimary { get; set; }
        public card card { get; set; }
        public string paymentClass { get; set; }
    }

    public class Pay360CardsResponse
    {
        public List<paymentMethodResponse> paymentMethodResponses { get; set; }
    }
}