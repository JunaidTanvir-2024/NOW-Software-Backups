namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the response when validating a Msisdn or Card number
    /// </summary>
    public class VerifyNumberResponseDTO
    {
        public bool numberStatus { get; set; }
    }
}
