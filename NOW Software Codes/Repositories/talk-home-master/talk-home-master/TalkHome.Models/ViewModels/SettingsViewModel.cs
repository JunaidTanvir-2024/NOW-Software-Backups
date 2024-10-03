using System.Collections.Generic;
using TalkHome.Models.WebApi.DTOs;
using Umbraco.Core.Models;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// View model for the settings page
    /// </summary>
    public class SettingsViewModel
    {
        public JWTPayload Payload { get; set; }

        public string ProductCode { get; set; }

        public AccountSummaryResponseDTO AccountSummary { get; set; }

        public IEnumerable<IPublishedContent> TopUps { get; set; }

    

    }

}
