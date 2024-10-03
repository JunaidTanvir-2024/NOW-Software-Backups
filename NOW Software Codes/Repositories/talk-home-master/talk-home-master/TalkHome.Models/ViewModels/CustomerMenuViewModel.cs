namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// The view model for the cutomer menu on account pages
    /// </summary>
    public class CustomerMenuViewModel
    {
        public JWTPayload Payload { get; set; }

        public string ProductCode { get; set; }
    }
}
