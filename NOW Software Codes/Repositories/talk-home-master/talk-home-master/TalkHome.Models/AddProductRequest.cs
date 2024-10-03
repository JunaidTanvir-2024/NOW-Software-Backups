using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// User-generated product details confirmation request
    /// </summary>
    public class AddProductRequest
    {
        [CustomValidation(typeof(RequestValidations), "Number")]
        public string Number { get; set; }

        [CustomValidation(typeof(RequestValidations), "Code")]
        public string Code { get; set; }

        [CustomValidation(typeof(RequestValidations), "ProductCode")]
        public string ProductCode { get; set; }
    }
}
