using System.Collections.Generic;

namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Request model for auto renewing bundles settings
    /// </summary>
    public class AutoRenewSettingsRequestDTO
    {
        public string msisdn { get; set; }

        public string productCode { get; set; }

        public List<AutoRenew> autoRenew { get; set; }
    }
}
