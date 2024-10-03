using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.Rates
{
    public class AppRate
    {
        public List<countryDestinations> countryDestinations { get; set; }
        public List<topCountryDestinations> topCountryDestinations { get; set; }
    }

    public class countryDestinations
    {
        public int id { get; set; }
        public string name { get; set; }
        public string flagImageUrl { get; set; }
        public bool hasBundles { get; set; }
        public bool hasOffers { get; set; }
        public string isoCode { get; set; }
        public bool showOffpeak { get; set; }
        public List<rates> rates { get; set; }
    }

    public class topCountryDestinations
    {
        public int id { get; set; }
        public string name { get; set; }
        public string flagImageUrl { get; set; }
        public bool hasBundles { get; set; }
        public bool hasOffers { get; set; }
        public string isoCode { get; set; }
        public bool showOffpeak { get; set; }
        public List<rates> rates { get; set; }
    }
    public class rates
    {
        public string Name { get; set; }
        public string Price { get; set; }
    }
}
