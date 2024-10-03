namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Describes a request for retrieving information about a transaction
    /// </summary>
    public class PaymentRetrieveRequest
    {
        public string Token { get; set; }

        public string ClientReference { get; set; }

        public string PaymentType { get; set; }

        public string PaymentMethod { get; set; }

        public string ChannelType { get; set; }

        public PaymentRetrieveRequest() { }

        public PaymentRetrieveRequest(string token, string clientReference, string paymentType, string paymentMethod, string channelType)
        {
            Token = token;

            ClientReference = clientReference;

            PaymentType = paymentType;

            PaymentMethod = paymentMethod;

            ChannelType = channelType;
        }
    }
}
