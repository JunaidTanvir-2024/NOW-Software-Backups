namespace TalkHome.Models.WebApi
{
    /// <summary>
    /// Body of the sign up response DTO
    /// </summary>
    public class SignUp
    {
        public bool isRegistered { get; set; }

        public string token { get; set; }
    }
}
