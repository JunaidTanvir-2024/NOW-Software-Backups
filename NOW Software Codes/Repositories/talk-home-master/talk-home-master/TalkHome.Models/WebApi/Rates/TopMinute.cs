namespace TalkHome.Models.WebApi.Rates
{
    public class TopMinute
    {
        public string iso_code { get; set; }

        public string destination { get; set; }

        public string topminutes { get; set; }

        public TopMinute(string iso_code, string destination, string topminutes)
        {
            this.iso_code = iso_code;

            this.destination = destination;

            this.topminutes = float.Parse(topminutes).ToString();
        }
    }
}
