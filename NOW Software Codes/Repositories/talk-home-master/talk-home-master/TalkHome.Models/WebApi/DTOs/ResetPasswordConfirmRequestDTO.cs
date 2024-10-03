namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Sends a request to verify the validity of the reset password token
    /// </summary>
    public class ResetPasswordConfirmRequestDTO
    {
        public string token { get; set; }
    }
}
