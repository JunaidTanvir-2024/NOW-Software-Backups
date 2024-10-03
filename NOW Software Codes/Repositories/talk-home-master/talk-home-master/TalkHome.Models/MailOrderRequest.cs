using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// Defines the request model for ordering a SIM or a Calling card
    /// </summary>
    public class MailOrderRequest
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
        [Required(ErrorMessage = "Enter email address"), MaxLength(length: 50, ErrorMessage = "Maximum 50 characters allowed")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is invalid")]
        public string EmailAddress { get; set; }

        [CustomValidation(typeof(RequestValidations), "MailOrderProduct")]
        public string MailOrderProduct { get; set; }

        //[CustomValidation(typeof(RequestValidations), "Password")]
        //[DataType(DataType.Password)]
        //[Compare("ConfirmPassword", ErrorMessage = "721")]
        public string Password { get; set; }

        //[CustomValidation(typeof(RequestValidations), "ConfirmPassword")]
        //[DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string SignUp { get; set; }

        public string ThaMsisdn { get; set; }

        public bool SubscribeSignUp { get; set; }

        private bool _simSwap;

        public bool IsSimSwap
        {
            get
            {
                return _simSwap;
            }
        }

        public string SimSwap {
            set
            {
                if (value == "on")
                    _simSwap = true;
                else
                    _simSwap = false;
            }
            get
            {
                if (_simSwap)
                    return "on";
                else
                    return "off";
            }
        }

        public string PortInMsisdn { get; set; }
        public string PAC { get; set; }
        public string PortDate { get; set; }
        public string FromOfferLandingPage { get; set; }
        public string digit1 { get; set; }
        public string digit2 { get; set; }
        public string digit3 { get; set; }
        public string digit4 { get; set; }


    }
}
