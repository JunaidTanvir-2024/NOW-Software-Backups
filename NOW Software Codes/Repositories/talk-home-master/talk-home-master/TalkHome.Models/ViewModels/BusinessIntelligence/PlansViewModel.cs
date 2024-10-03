using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace TalkHome.Models.ViewModels.BusinessIntelligence
{

    public class UpgradePlan
    {
        public List<string> UpgradePlans { get; set; }

        public string CurrentPlan { get; set; }

        public UpgradePlan (List<string> up, string cp)
        {
            UpgradePlans = up;
            CurrentPlan = cp;
        }

       
    }
    public class PlansViewModel
    {
        
        public string UpgradePlanRules { get; set; }

        public List<UpgradePlan> TopPlans { get; set; }

        public List<UpgradePlan> UpgradePlans { get; set; }
     
        public PlansViewModel (List<UpgradePlan> upgradePlans, string ugRules, List<UpgradePlan> topPlans)
        {
            UpgradePlans = upgradePlans;
            UpgradePlanRules = ugRules;
            TopPlans = topPlans;
        }
    }
}
