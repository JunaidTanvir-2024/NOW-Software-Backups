using System.Collections.Generic;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// Describes the view moel for providing an address
    /// </summary>
    public class AddressDetailsViewModel
    {
        public List<I18nCountry> Countries { get; set; }

        public AddressModel Address { get; set; }

        public bool UKAddress { get; set; }

        public AddressDetailsViewModel(List<I18nCountry> countries, AddressModel address, bool uKAddress)
        {
            Countries = countries;

            Address = address;

            UKAddress = uKAddress;
        }
    }
}
