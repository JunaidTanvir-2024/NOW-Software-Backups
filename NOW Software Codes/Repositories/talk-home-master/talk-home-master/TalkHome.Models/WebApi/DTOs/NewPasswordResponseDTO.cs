namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the response model for a new password request
    /// </summary>
    public class NewPasswordResponseDTO
    {
        public PasswordChange passwordChange { get; set; }
    }
}
