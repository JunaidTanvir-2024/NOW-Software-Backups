using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// Defines the customer request model for verifying a phone or card number
    /// </summary>
    public class VerifyNumberRequest
    {
        [CustomValidation(typeof(RequestValidations), "Number")]
        public string Number { get; set; }

        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "716")]
        [Compare("ConfirmEmail", ErrorMessage = "787")]
        public string Email { get; set; }


        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "716")]
        public string ConfirmEmail { get; set; }

        public string CountryCode { get; set; }
    }
}
