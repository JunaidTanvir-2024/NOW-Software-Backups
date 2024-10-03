namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Response model for a payment request
    /// </summary>
    public class StartPaymentResponseDTO
    {
        public int responseCode { get; set; }

        public string errorFields { get; set; }

        public string token { get; set; } // MiPay's transaction Id

        public string paymentURL { get; set; }

        public string clientReference { get; set; } // Nowtel's Transaction Id
    }
}
