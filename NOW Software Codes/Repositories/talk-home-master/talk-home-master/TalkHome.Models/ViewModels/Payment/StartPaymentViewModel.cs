using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models.ViewModels.Payment
{
    /// <summary>
    /// Describes a user-generated checkout request to process a payment
    /// </summary>
    public class StartPaymentViewModel
    {
        public string Salutation { get; set; }

        [CustomValidation(typeof(RequestValidations), "FirstName")]
        public string FirstName { get; set; }

        [CustomValidation(typeof(RequestValidations), "LastName")]
        public string LastName { get; set; }

        [CustomValidation(typeof(RequestValidations), "AddressLine1")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [CustomValidation(typeof(RequestValidations), "City")]
        public string City { get; set; }

        public string CountyOrProvince { get; set; }

        [CustomValidation(typeof(RequestValidations), "CountryCode")]
        public string CountryCode { get; set; }

        [CustomValidation(typeof(RequestValidations), "PostCode")]
        public string PostalCode { get; set; }

        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [CustomValidation(typeof(RequestValidations), "PaymentMethod")]
        public string PaymentMethod { get; set; }

        public string CardId { get; set; }

        public string Unreg { get; set; }

        public int Amount { get; set; }

        public string ProductType { get; set; }

        public CreditSimPayload CreditSim { get; set; }
        public string FirstUseDate { get; set; }
    }
}
