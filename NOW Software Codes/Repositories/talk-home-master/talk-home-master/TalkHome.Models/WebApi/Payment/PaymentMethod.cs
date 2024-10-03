namespace TalkHome.Models.WebApi.Payment
{
    public class PaymentMethod
    {
        public string primaryAccountNumber { get; set; }

        public string issueNumber { get; set; }

        public string startYear { get; set; }

        public string startMonth { get; set; }

        public string expiryYear { get; set; }

        public string expiryMonth { get; set; }

        public string cvv { get; set; }

        public string nameOnCard { get; set; }

        public string cardType { get; set; }

        public bool isAllowedToBeDeleted { get; set; }

        public bool enabled { get; set; }

        public string typeOfMethodOfPayment { get; set; }

        public string methodOfPaymentIdentifier { get; set; }

        public bool isDefault { get; set; }

        public bool storeDetails { get; set; }
    }
}
