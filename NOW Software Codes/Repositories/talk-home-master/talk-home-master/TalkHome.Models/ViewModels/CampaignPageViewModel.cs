using TalkHome.Models;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// Extends Umbraco's RenderModel with a custom class in order to add more properties.
    /// </summary>
    public class CampaignPageViewModel : RenderModel
    {
        public JWTPayload Payload { get; set; }

        // Standard Model Pass Through
        public CampaignPageViewModel(IPublishedContent content) : base(content)
        {
        }
    }
}
