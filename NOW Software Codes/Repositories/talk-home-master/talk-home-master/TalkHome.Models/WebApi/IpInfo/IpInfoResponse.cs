namespace TalkHome.Models.WebApi.IpInfo
{
    /// <summary>
    /// Defines the model for a response for the IpInfo service.
    /// </summary>
    public class IpInfoResponse
    {
        public string ip { get; set; }

        public string hostname { get; set; }

        public string loc { get; set; }

        public string org { get; set; }

        public string city { get; set; }

        public string region { get; set; }

        public string country { get; set; }

        public int phone { get; set; }
    }
}
