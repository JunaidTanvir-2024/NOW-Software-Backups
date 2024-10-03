using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// User-generated request for a new password
    /// </summary>
    public class NewPasswordRequest
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
    }
}
