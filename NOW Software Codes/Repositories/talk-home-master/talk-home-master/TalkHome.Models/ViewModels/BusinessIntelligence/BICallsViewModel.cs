using Umbraco.Core.Models;
using Newtonsoft.Json.Linq;
using TalkHome.Models.WebApi;

namespace TalkHome.Models.ViewModels.BusinessIntelligence
{
    /// <summary>
    /// Defines the available payment methods at checkout.
    /// </summary>
    public enum RuleType
    {
        Upgrade = 1,

        Renew = 2

    }

    public class BICallsViewModel
    {
        public string MobileRate {get;set;}
        public string LandLineRate { get; set;}
        public string SMSRate { get; set; }
        public bool HasBundle { get; set; }
        public string BundleId { get; set; }
        public UserAccountBundles  CurrentPlan { set; get; }
        public string CountryName { get; set; }
        public IPublishedContent Content { get; set; }
        public IPublishedContent UpSellBundle { get; set; }
        public JToken Rule { get; set; }
        public bool RuleApplied { get; set; }
        public IPublishedContent RenewalBundle { get; set; }
        public RuleType RuleType { get; set; }
        public double Usage { get; set; }

        public BICallsViewModel()
        {

        
        }
    }
}
