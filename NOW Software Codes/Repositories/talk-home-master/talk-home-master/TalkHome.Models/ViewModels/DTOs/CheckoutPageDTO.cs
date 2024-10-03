using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;
using TalkHome.Models.WebApi.DTOs;

namespace TalkHome.Models.ViewModels.DTOs
{
    /// <summary>
    /// Defines the minimum required data to load the checkout page
    /// </summary>
    public class CheckoutPageDTO
    {
        public string Reference { get; set; }

        [CustomValidation(typeof(RequestValidations), "ProductCode")]
        public string Verify { get; set; }

        [CustomValidation(typeof(RequestValidations), "ProductType")]
        public string ProductType { get; set; }

        public MailOrderRequestDTO MailOrder { get; set; }

        public HashSet<int> Basket { get; set; }

        public decimal Total { get; set; }

        public bool DeliveryIsBilling { get; set; }
    }
}
