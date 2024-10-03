namespace TalkHome.Models.WebApi
{
    /// <summary>
    /// Represents a record for the last top up
    /// </summary>
    public class UserAccountLastTopup
    {
        public decimal amount { get; set; }

        public string date { get; set; }
    }
}
