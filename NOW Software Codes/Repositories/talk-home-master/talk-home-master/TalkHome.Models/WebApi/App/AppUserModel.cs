namespace TalkHome.Models.WebApi.App
{
    /// <summary>
    /// Describes an App user authenticated via MSISDN on the Talk Home App APIs
    /// </summary>
    public class AppUserModel
    {
        public string title { get; set; }

        public string fname { get; set; }

        public string lname { get; set; }

        public string email { get; set; }

        public string addr1 { get; set; }

        public string addr2 { get; set; }

        public string addr4 { get; set; }

        public string addr5 { get; set; }

        public string postal_code { get; set; }

        public string country { get; set; }

        public string currency { get; set; }

        public string pin { get; set; }

        public string locale { get; set; }

        public string appLanguage { get; set; }

        public string accountType { get; set; }

        public string countryCode { get; set; }

        public string currencySymbol { get; set; }
    }
}
