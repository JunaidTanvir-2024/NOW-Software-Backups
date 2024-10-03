using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// Defines a customer-submitted request model for a promotional sign up
    /// </summary>
    public class PromoSignUpRequest
    {
        [CustomValidation(typeof(RequestValidations), "Number")]
        public string Number { get; set; }

        [CustomValidation(typeof(RequestValidations), "FirstName")]
        public string FirstName { get; set; }

        [CustomValidation(typeof(RequestValidations), "LastName")]
        public string LastName { get; set; }

        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [CustomValidation(typeof(RequestValidations), "ProductCode")]
        public string ProductCode { get; set; }

        [CustomValidation(typeof(RequestValidations), "CountryCode")]
        public string CountryCode { get; set; }
    }
}
