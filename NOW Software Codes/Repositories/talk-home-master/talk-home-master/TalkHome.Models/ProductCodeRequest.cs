using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// Defines a product code request model
    /// </summary>
    public class ProductCodeRequest
    {
        [CustomValidation(typeof(RequestValidations), "ProductCode")]
        public string ProductCode { get; set; }
    }
}
