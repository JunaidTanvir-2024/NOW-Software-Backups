using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// Request for a Calling card mail order with credit
    /// </summary>
    public class CallingCardCreditOrderRequest
    {
        public string Salutation { get; set; }

        [CustomValidation(typeof(RequestValidations), "FirstName")]
        public string FirstName { get; set; }

        [CustomValidation(typeof(RequestValidations), "LastName")]
        public string LastName { get; set; }

        //[CustomValidation(typeof(RequestValidations), "AddressLine1")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        //[CustomValidation(typeof(RequestValidations), "City")]
        public string City { get; set; }

        public string CountyOrProvince { get; set; }

        //[CustomValidation(typeof(RequestValidations), "CountryCode")]
        public string CountryCode { get; set; }

        //[CustomValidation(typeof(RequestValidations), "PostCode")]
        public string PostalCode { get; set; }

        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        [Compare("ConfirmEmailAddress", ErrorMessage = "787")]
        public string EmailAddress { get; set; }

        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string ConfirmEmailAddress { get; set; }

        [CustomValidation(typeof(RequestValidations), "MailOrderProduct")]
        public string MailOrderProduct { get; set; }

        public int Credit { get; set; }

        public bool DeliveryIsBilling { get; set; }
    }
}
