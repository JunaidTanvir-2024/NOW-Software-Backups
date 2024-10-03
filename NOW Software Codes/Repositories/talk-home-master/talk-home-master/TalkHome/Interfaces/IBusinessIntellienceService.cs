using System.Collections.Generic;
using System.Threading.Tasks;
using TalkHome.Models.ViewModels.BusinessIntelligence;
using TalkHome.Models.WebApi;



namespace TalkHome.Interfaces
{
    public interface IBusinessIntellienceService
    {
        Task<BICallsViewModel>  CreateBundleAnalysis (string apiToken, string rules);

        Task<TransferPromotionsViewModel> CreatePromotions(string apiToken,string defaultCountries);

        Task<TransferPromotionsViewModel> CreatePromotions(string defaultCountries);

        List<UpgradePlan> GetRecommendedPlans(string json, List<UserAccountBundles> bundles);

        List<UpgradePlan> GetTopPlans(string topJson);
    }
}
