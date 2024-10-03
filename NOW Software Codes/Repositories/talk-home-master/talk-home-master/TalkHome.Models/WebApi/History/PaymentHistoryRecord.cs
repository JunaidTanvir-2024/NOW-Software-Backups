namespace TalkHome.Models.WebApi.History
{
    /// <summary>
    /// Represents an individual payment record
    /// </summary>
    public class PaymentHistoryRecord
    {
        public string paymentDate { get; set; }

        public string amount { get; set; }

        public string reason { get; set; }
    }
}
