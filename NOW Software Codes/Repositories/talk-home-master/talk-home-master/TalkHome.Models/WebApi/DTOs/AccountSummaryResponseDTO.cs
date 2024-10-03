using System.Collections.Generic;

namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Response model for an account summary request
    /// </summary>
    public class AccountSummaryResponseDTO
    {
        public UserAccountSummary userAccountSummary { get; set; }

        public UserAccountLastTopup userAccountLastTopup { get; set; }

        public List<UserAccountBundles> userAccountBundles { get; set; } = new List<UserAccountBundles>();

        public AutoRenewSettingsRequestDTO autoRenewSummary { get; set; }

        public AutoTopUpSettingsRequestDTO autoTopUpSummary { get; set; }
    }
}
