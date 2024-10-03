using System.Collections.Generic;
using System.Globalization;
using TalkHome.Models;

namespace TalkHome.Models.ViewModels
{
    public class RegisterProductViewModel
    {
        public string Slug { get; set; }

        public JWTPayload Payload { get; set; }

        public List<RegionInfo> Countries { get; set; }

        public RegisterProductViewModel(string slug, JWTPayload payload, List<RegionInfo> countries)
        {
            Slug = slug;

            Payload = payload;

            Countries = countries;
        }
    }
}
