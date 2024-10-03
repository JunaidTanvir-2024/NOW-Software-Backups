namespace TalkHome.Models.WebApi.Payment
{
    public class BillingAddress
    {
        public string addressLine1 { get; set; }

        public string addressLine2 { get; set; }

        public string city { get; set; }

        public string county { get; set; }

        public string postCode { get; set; }

        public string country { get; set; }
    }
}
