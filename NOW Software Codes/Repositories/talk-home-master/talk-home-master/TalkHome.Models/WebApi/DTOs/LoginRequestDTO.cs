namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Request model for a log in
    /// </summary>
    public class LoginRequestDTO
    {
        public string email { get; set; }

        public string password { get; set; }

        public string ipAddress { get; set; }

        public string returnUrl { get; set; }
    }
}
