using TalkHome.Models.WebApi;

namespace TalkHome.Models.ViewModels
{
    public class AccountDetailsViewModel
    {
        public AccountDetailsResponseDTO AccountDetails { get; set; }

        public string ProductCode { get; set; }

        public AddressDetailsViewModel HomeAddress { get; set; }

        public AddressDetailsViewModel BillingAddress { get; set; }

        public JWTPayload Payload { get; set; }

        public bool Subscribed { get; set; }

        public string ACId { get; set; }

        public AccountDetailsViewModel(AccountDetailsResponseDTO accountDetails, string productCode, 
            AddressDetailsViewModel homeAddress, AddressDetailsViewModel billingAddress, JWTPayload payload, bool subscribed, string acId)
        {
            AccountDetails = accountDetails;

            ProductCode = productCode;

            HomeAddress = homeAddress;

            BillingAddress = billingAddress;

            Payload = payload;

            Subscribed = subscribed;

            ACId = acId;
        }
    }
}
