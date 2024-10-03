using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// Defines the request model for a bundle/plan purchase request
    /// </summary>
    public class CheckoutRequest
    {
        [CustomValidation(typeof(RequestValidations), "ProductCode")]
        public string ProductCode { get; set; }

        public int Id { get; set; }

        public string Source { get; set; }
    }
}
