namespace TalkHome.Models.WebApi.Rates
{
    /// <summary>
    /// Describes the model for a roaming rate record.
    /// </summary>
    public class RoamingRate
    {
        public string iso_code { get; set; }

        public int id { get; set; }

        public string country { get; set; }

        public string countryCode { get; set; }

        public string ukCall { get; set; }

        public string localCall { get; set; }

        public string euCall { get; set; }

        public string internationalCall { get; set; }

        public string outboundSMS { get; set; }

        public string inboundSMS { get; set; }

        public string data { get; set; }

        public string inboundCall { get; set; }

        public RoamingRate(string iso_code, int id, string country, string countryCode, string ukCall, string localCall, string euCall, string internationalCall, string outboundSMS, string inboundSMS, string data, string inboundCall)
        {
            this.iso_code = iso_code;

            this.id = id;

            this.country = country;

            this.countryCode = countryCode;

            this.ukCall = (float.Parse(ukCall) * 100).ToString();

            this.localCall = (float.Parse(localCall) * 100).ToString();

            this.euCall = (float.Parse(euCall) * 100).ToString();

            this.internationalCall = (float.Parse(internationalCall) * 100).ToString();

            this.outboundSMS = (float.Parse(outboundSMS) * 100).ToString();

            this.inboundSMS = (float.Parse(inboundSMS) * 100).ToString();

            this.data = (float.Parse(data) * 100).ToString();

            this.inboundCall = (float.Parse(inboundCall) * 100).ToString();
        }
    }
}
