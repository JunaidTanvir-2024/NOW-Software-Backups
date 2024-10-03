namespace TalkHome.Models
{
    public class Alert
    {
        public const string TempDataKey = "TempDataAlerts";

        public string AlertStyle { get; set; }

        public bool Dismissable { get; set; }

        public string Message { get; set; }

        public Alert() { }

        public Alert(string alertStyle, string message)
        {
            AlertStyle = alertStyle;

            Message = message;
        }
    }
}
