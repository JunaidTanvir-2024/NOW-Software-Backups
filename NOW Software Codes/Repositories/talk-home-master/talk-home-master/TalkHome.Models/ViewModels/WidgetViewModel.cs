using TalkHome.Models.ViewModels.BusinessIntelligence;
using Umbraco.Web.PublishedContentModels;
using Umbraco.Core.Models;
using System.Collections.Generic;

namespace TalkHome.Models.ViewModels
{
    public class PlansFilter
    {
        public string CurrentPlan { get; set; }
        public string UpgradePlan { get; set; }

        public PlansFilter(string cp, string up)
        {
            CurrentPlan = cp;
            UpgradePlan = up;
        }
    }

    public class WidgetViewModel
    {
        public TestNestedDoc Fold { get; set; }

        public BICallsViewModel BICalls { get; set; }

        public TransferPromotionsViewModel TransferPromotionViewModel { get; set; }

        public PlansViewModel PlansViewModel { get; set; }

        public VerifiedTopUps VerifiedTopUps { get; set; }


        public WidgetViewModel(TestNestedDoc fold, BICallsViewModel calls, TransferPromotionsViewModel tpvm, PlansViewModel pvm)
        {
            Fold = fold;
            BICalls = calls;
            TransferPromotionViewModel = tpvm;
            PlansViewModel = pvm;
        }

    }

    public class VerifiedTopUps
    {
        public IEnumerable<IPublishedContent> TopUps { get; set; }
    }
}
