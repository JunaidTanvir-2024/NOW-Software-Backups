namespace TalkHome.Models.ViewModels.App
{
    /// <summary>
    /// Contains the message label to localise the text in the view
    /// </summary>
    public class AppMessageViewModel
    {
        public string Message { get; set; }

        public AppMessageViewModel(string message)
        {
            Message = message;
        }
    }
}
