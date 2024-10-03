namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Response model for a log in
    /// </summary>
    /// <typeparam name="AuthenticationContent">The Type of the response content</typeparam>
    public class LoginResponseDTO<AuthenticationContent>
    {
        public AuthenticationContent authentication { get; set; }
    }
}
