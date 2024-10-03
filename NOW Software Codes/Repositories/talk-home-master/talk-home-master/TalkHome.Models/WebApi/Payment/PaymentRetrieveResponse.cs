namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Describes the information regarding the outcome of a transaction
    /// </summary>
    public class PaymentRetrieveResponse
    {
        public string responseCode { get; set; }

        public string errorFields { get; set; }

        public string token { get; set; }

        public string clientReference { get; set; }

        public string merchantId { get; set; }

        public decimal authAmount { get; set; }

        public string currency { get; set; }

        public string captureAmount { get; set; }

        public string lastUpdated { get; set; }

        public string orderResult { get; set; }

        public string description { get; set; }

        public string salutation { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string addressLine1 { get; set; }

        public string addressLine2 { get; set; }

        public string city { get; set; }

        public string countyOrProvince { get; set; }

        public string country { get; set; }

        public string postalCode { get; set; }

        public string homeTelNumber { get; set; }

        public string emailAddress { get; set; }

        public string dateTimeLocal { get; set; }

        public string clientData { get; set; }

        public decimal finalAmount { get; set; }

        public int statusCode { get; set; }

        public string successURL { get; set; }

        public string failureURL { get; set; }

        public string locale { get; set; }

        public string msisdn { get; set; }

        public string uniqueIdValue { get; set; }

        public string uniqueIdType { get; set; }

        public string registeredMerchantId { get; set; }

        public string agentUserName { get; set; }

        public string userID { get; set; }

        public string originalSwitchKey  { get; set; }
    }
}
