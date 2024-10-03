namespace TalkHome.Models.WebApi
{
    /// <summary>
    /// Defines the model for a Top up request
    /// </summary>
    public class TopUpRequest
    {
        public string ProductCode { get; set; }

        public int ProductId { get; set; }

        public string MsisdnOrCardNumber { get; set; }
    }
}
