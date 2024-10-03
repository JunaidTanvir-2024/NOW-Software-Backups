namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the response model after a request to verify the token
    /// </summary>
    public class ResetPasswordConfirmResponseDTO
    {
        public PasswordResetTokenValidation passwordResetTokenValidation { get; set; }
    }
}
