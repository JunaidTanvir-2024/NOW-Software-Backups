namespace TalkHome.Models.WebApi
{
    /// <summary>
    /// Describes the summary of a Talk Home Web API customer account
    /// </summary>
    public class AccountDetailsResponseDTO
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public string title { get; set; }

        public string contactNo { get; set; }

        public string mobileNo { get; set; }

        public Addresses addresses { get; set; }
    }
}
