namespace TalkHome.Models.App
{
    /// <summary>
    /// Represent an address on the DigiTalk database.
    /// </summary>
    public class AppUserBillingAddress
    {
        public string addr1 { get; set; }

        public string addr2 { get; set; }

        public string addr4 { get; set; } // city

        public string addr5 { get; set; } // county/state

        public string postal_code { get; set; }

        public string country { get; set; }

        public string email { get; set; }

        public AppUserBillingAddress(string addr1, string addr2, string addr4, string addr5, string postal_code, string country, string email)
        {
            this.addr1 = addr1;

            this.addr2 = addr2;

            this.addr4 = addr4;

            this.addr5 = addr5;

            this.postal_code = postal_code;

            this.country = country;

            this.email = email;
        }
    }
}
