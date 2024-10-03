namespace TalkHome.Models
{
    /// <summary>
    /// Defines the payment Ids required for processing a transaction
    /// </summary>
    public class Transaction
    {
        public string clientReference { get; set; } // Nowtel's Transaction Id

        public string token { get; set; } // MiPay's transaction Id

        public Transaction() { }

        public Transaction(string clientReference, string token)
        {
            this.clientReference = clientReference;
            this.token = token;
        }
    }
}
