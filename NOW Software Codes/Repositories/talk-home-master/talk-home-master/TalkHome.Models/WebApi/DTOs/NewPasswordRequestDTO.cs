namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the request model for setting a new password
    /// </summary>
    public class NewPasswordRequestDTO
    {
        public string email { get; set; }

        public string newPassword { get; set; }

        public string confirmPassword { get; set; }
    }
}
