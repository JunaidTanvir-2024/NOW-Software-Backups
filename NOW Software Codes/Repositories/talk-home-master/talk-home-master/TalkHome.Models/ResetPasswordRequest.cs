using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// User-generated request to reset a password
    /// </summary>
    public class ResetPasswordRequest
    {
        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public string Number { get; set; }
    }
}
