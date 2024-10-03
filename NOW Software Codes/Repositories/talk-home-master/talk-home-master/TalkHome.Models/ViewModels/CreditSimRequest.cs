using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.ViewModels
{
    public class CreditSimRequest
    {
        public int? UserId { get; set; }
        public int TopUpId { get; set; }
        public string BundleId { get; set; }
        public int Amount { get; set; }
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter email address"), MaxLength(length: 50, ErrorMessage = "Maximum 50 characters allowed")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is invalid")]
        public string EmailAddress { get; set; }

        public int creditSimType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string CountyOrProvince { get; set; }
        public string CountryCode { get; set; }
        public string country { get; set; }
        public string PostalCode { get; set; }
        public Basket[] basket { get; set; }

        public string SignUp { get; set; }
        public  string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool SubscribeSignUp { get; set; }

        public  string ProductCode { get; set; }
        public  string Gclid { get; set; }
    }
}
