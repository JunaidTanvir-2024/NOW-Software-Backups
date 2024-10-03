namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the model for a Web Api request for a mail order
    /// </summary>
    public class MailOrderRequestDTO
    {
        public string title { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public string addressL1 { get; set; }

        public string addressL2 { get; set; }

        public string city { get; set; }

        public string county { get; set; }

        public string postCode { get; set; }

        public string country { get; set; }

        public string product { get; set; }

        public bool isSimSwap { get; set; }

        public string thaMsisdn { get; set; }
    }
}
