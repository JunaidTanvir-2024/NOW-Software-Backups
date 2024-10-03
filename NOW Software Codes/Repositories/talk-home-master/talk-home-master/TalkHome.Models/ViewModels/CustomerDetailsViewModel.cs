using System.Collections.Generic;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// Defines the view model to display customer detials sections on form
    /// </summary>
    public class CustomerDetailsViewModel
    {
        public FullNameModel FullName { get; set; }

        public List<I18nCountry> Countries { get; set; }

        public AddressModel Address { get; set; }

        public bool UKAddress { get; set; }

        public CustomerDetailsViewModel(FullNameModel fullName, List<I18nCountry> countries, AddressModel address, bool uKAddress)
        {
            FullName = fullName;

            Countries = countries;

            Address = address;

            UKAddress = uKAddress;
        }
    }
}
