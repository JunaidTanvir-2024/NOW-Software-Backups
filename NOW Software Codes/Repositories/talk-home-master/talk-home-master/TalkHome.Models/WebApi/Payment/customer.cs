namespace TalkHome.Models.WebApi.Payment
{
    public class Customer
    {
        public string defaultMethodOfPaymentType { get; set; }

        public bool notifyByEmail { get; set; }

        public string pdocumentIdType { get; set; }

        public string documentIdNumber { get; set; }

        public string registrationLocation { get; set; }

        public string documentIdExpiryDate { get; set; }

        public string documentIdIssueDate { get; set; }

    }
}
