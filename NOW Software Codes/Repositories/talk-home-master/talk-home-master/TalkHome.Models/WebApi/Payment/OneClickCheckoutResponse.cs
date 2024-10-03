namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Defines the data structure for a One-click checkout response
    /// </summary>
    public class OneClickCheckoutResponse
    {
        public string authCode { get; set; }

        public string transactionDateTime { get; set; }

        public string retrievalReferenceNumber { get; set; }

        public string dplTransactionID { get; set; }

        public string operatorTransactionID { get; set; }

        public string responseCode { get; set; }
    }
}
