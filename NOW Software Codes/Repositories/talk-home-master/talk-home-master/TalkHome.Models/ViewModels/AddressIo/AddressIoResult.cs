namespace TalkHome.Models.ViewModels.AddressIo
{
    public class AddressIoResult
    {
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Line3 { get; set; }

        public string Line4 { get; set; }

        public string Locality { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public AddressIoResult(string line1, string line2, string line3, string line4, string locality, string city, string county)
        {
            Line1 = line1;

            Line2 = line2;

            Line3 = line3;

            Line4 = line4;

            Locality = locality;

            City = city;

            County = county;  
        }
    }
}
