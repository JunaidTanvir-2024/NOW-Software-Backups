using System.ComponentModel.DataAnnotations;

namespace TalkHome.Models.ViewModels.Pay360
{
    /// <summary>
    /// Describes a user-generated checkout request to process a payment
    /// </summary>
    public class StartPay360ViewModel
    {

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        public string CountyOrProvince { get; set; }

        public string CountryCode { get; set; }

        public string PostalCode { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PaymentMethod { get; set; }

        public string CardId { get; set; }

        public string Unreg { get; set; }

        public string Msisdn { get; set; }

        public string Currency { get; set; }

        public int Amount { get; set; }

        public string CustId { get; set; }

        public string NoCards { get; set; }

        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiryMonth { get; set; }
        public string CardExpiryYear { get; set; }
        public string SecurityCode { get; set; }
        public string CardCv2 { get; set; }
        public string Default { get; set; }
        public bool AutoTopUpEnabled { get; set; }
        public bool AutoTopUpSwitchVisibleOnCheckout { get; set; }
        public string FirstUseDate { get; set; }

        public string ProductType { get; set; }
        public CreditSimPayload CreditSim { get; set; }

    }
}
