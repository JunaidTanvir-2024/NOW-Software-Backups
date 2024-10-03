namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Transfer object for a sign up request
    /// </summary>
    public class SignUpRequestDTO
    {
        public string email { get; set; }

        public string password { get; set; }

        public string confirmPassword { get; set; }

        public string title { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string dateOfBirth { get; set; }

        public int contactNo { get; set; }

        public int mobileNo { get; set; }

        public bool isSubscribedToNewsletter { get; set; }

        public string subscriberId { get; set; }

        public int clientID { get; set; }

        public bool TermsAndConditions { get; set; }
    }
}
