using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// User-generated request for setting up auto top up
    /// </summary>
    public class AutoTopUpRequest
    {
        [CustomValidation(typeof(RequestValidations), "ProductCode")]
        public string ProductCode { get; set; }

        [CustomValidation(typeof(RequestValidations), "Threshold")]
        public decimal Threshold { get; set; }

        public int TopUpId { get; set; }

        public string AutoTopUp { get; set; }

        
    }

    public class AutoRenewalsRequest
    {
        [CustomValidation(typeof(RequestValidations), "ProductCode")]
        public string ProductCode { get; set; }

        public string BundleStatus { get; set; }

        public string BundleGuid { get; set; }

        public string ProductRef { get; set; }
    }

}
