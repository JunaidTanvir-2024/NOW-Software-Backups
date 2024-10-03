using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// User-generated log in request
    /// </summary>
    public class LoginRequest
    {
        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string IpAddress { get; set; }

        public string ReturnUrl { get; set; }

        public AirtimeTransfer AirTimeTransfer { get; set; }
    }
}
