namespace TalkHome.Models
{
    /// <summary>
    /// Defines the data structure for an individual record of a country
    /// </summary>
    public class I18nCountry
    {
        public I18nCountryName name { get; set; }

        public string cca2 { get; set; }

        public string ccn3 { get; set; }

        public string cca3 { get; set; }

        public string cioc { get; set; }
    }
}
