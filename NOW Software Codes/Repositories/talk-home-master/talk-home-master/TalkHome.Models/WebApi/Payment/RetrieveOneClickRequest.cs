namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Defines the data structure to retrieve a One-click transaction
    /// </summary>
    public class RetrieveOneClickRequest
    {
        public string UniqueID { get; set; }

        public RetrieveOneClickRequest(string uniqueID)
        {
            UniqueID = uniqueID;
        }
    }
}
