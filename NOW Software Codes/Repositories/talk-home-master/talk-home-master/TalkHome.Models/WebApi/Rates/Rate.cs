namespace TalkHome.Models.WebApi.Rates
{
    /// <summary>
    /// Describes the model for a rate record.
    /// </summary>
    public class Rate
    {
        public string iso_code { get; set; }

        public string destination { get; set; }

        public string landline { get; set; }

        public string mobile { get; set; }

        public string sms { get; set; }

        public Rate(string iso_code, string destination, string landline, string mobile, string sms)
        {
            if (destination.Contains("Telenor"))
                this.iso_code = iso_code + "-Telenor";
            else
                this.iso_code = iso_code;

            this.destination = destination;



            this.landline = (float.Parse(landline) * 100).ToString();

            this.mobile = (float.Parse(mobile) * 100).ToString();

            this.sms = (float.Parse(sms) * 100).ToString();
        }
    }
}
