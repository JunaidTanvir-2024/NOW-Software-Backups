namespace TalkHome.Models.WebApi
{
    /// <summary>
    /// Model for auto renewing a bundle
    /// </summary>
    public class AutoRenew
    {
        public string guid { get; set; }

        public bool renew { get; set; }
    }
}
