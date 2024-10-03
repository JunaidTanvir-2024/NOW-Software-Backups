namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Request model for the account summary
    /// </summary>
    public class AccountSummaryRequestDTO
    {
        public string productCode { get; set; }

        public string token { get; set; }
    }
}
