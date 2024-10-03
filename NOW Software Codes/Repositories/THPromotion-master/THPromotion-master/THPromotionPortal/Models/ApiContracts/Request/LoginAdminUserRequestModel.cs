using System.ComponentModel.DataAnnotations;


namespace THPromotionPortal.Models.ApiContracts.Request
{
    public class LoginAdminUserRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(length: 8, ErrorMessage = "Minimum length should be 8 characters")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}
