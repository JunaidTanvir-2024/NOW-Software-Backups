using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// Defines a user-generated address model
    /// </summary>
    public class AddressModel
    {
        [CustomValidation(typeof(RequestValidations), "AddressLine1")]
        public string addressL1 { get; set; }

        public string addressL2 { get; set; }

        [CustomValidation(typeof(RequestValidations), "City")]
        public string city { get; set; }

        public string county { get; set; }

        [CustomValidation(typeof(RequestValidations), "PostCode")]
        public string postCode { get; set; }

        [CustomValidation(typeof(RequestValidations), "CountryCode")]
        public string country { get; set; }

        public AddressModel()
        {
        }

        public AddressModel(string addressL1, string addressL2, string city, string county, string postCode, string country)
        {
            this.addressL1 = addressL1;

            this.addressL2 = addressL2;

            this.city = city;

            this.county = county;

            this.postCode = postCode;

            this.country = country;
        }

        /// <summary>
        /// Provides ToString() override for display purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(", ",
                !string.IsNullOrWhiteSpace(addressL1) ? addressL1 : "",
                !string.IsNullOrWhiteSpace(addressL2) ? addressL2 : "",
                !string.IsNullOrWhiteSpace(city) ? city : "",
                !string.IsNullOrWhiteSpace(county) ? county : "",
                !string.IsNullOrWhiteSpace(postCode) ? postCode : "");
        }
    }
}
