using System.Collections.Generic;

namespace TalkHome.Models.WebApi.AddressIo
{
    public class AddressIoResponse
    {
        public string Latitude { get; set;}

        public string Longitude { get; set; }

        public IList<string> Addresses { get; set; }

        public string Message { get; set; }
    }
}
