using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// View model for a product
    /// </summary>
    public class PlansBundleViewModel
    {
        public Product Product { get; set; }

        public JWTPayload Payload { get; set; }

        public bool Summary { get; set; }

        public bool National { get; set; }

        public bool IsTopPlan { get; set; }

        public string Destination { get; set; }
    }
}
