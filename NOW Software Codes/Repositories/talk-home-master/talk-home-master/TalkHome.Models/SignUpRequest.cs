using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary> 
    /// Defines the model for creating a new customer account request 
    /// </summary> 
    public class SignUpRequest
    {
        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [CustomValidation(typeof(RequestValidations), "Password")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "721")]
        public string Password { get; set; }

        [CustomValidation(typeof(RequestValidations), "ConfirmPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Salutation { get; set; }

        [CustomValidation(typeof(RequestValidations), "FirstName")]
        public string FirstName { get; set; }

        [CustomValidation(typeof(RequestValidations), "LastName")]
        public string LastName { get; set; }
        
        public bool TermsAndConditions { get; set; } = true;

        public bool SubscribeSignUp { get; set; }

        public bool LegacyMigration { get; set; }

        public string LegacyCallingCardNumber { get; set; }

        public string LegacyPinNumber { get; set; }
    }
}
