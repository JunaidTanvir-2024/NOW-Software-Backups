namespace TalkHome.Models.WebApi
{
    /// <summary>
    /// Represents record for an account summary
    /// </summary>
    public class UserAccountSummary
    {
        public string productRef { get; set; }

        public decimal creditRemaining { get; set; }

        public string currency { get; set; }

        public string activeDate { get; set; }

        public int points { get; set; }
    }
}
