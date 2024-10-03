namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the model for an initial password reset request
    /// </summary>
    public class ResetPasswordRequestDTO
    {
        public string email { get; set; }
    }
}
