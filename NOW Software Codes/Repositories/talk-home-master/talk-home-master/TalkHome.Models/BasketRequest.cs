using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// A request to amend an item from the basket
    /// </summary>
    public class BasketRequest
    {
        [CustomValidation(typeof(RequestValidations), "Id")]
        public int Id { get; set; }
    }
}
