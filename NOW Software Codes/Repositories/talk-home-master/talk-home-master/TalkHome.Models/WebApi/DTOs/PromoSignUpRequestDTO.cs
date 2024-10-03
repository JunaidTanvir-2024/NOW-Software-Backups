namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the model for a Promotional sign up request
    /// </summary>
    public class PromoSignUpRequestDTO
    {
        public string Msisdn { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ProductCode { get; set; }

        public string CountryCode { get; set; }
    }
}
