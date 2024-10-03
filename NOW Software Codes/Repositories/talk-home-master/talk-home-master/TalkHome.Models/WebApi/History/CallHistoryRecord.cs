namespace TalkHome.Models.WebApi.History
{
    /// <summary>
    /// Represents a call record
    /// </summary>
    public class CallHistoryRecord
    {
        public string start_time { get; set; }

        public string called_party_number { get; set; }

        public string duration { get; set; }

        public string subscriber_charge { get; set; }
    }
}
