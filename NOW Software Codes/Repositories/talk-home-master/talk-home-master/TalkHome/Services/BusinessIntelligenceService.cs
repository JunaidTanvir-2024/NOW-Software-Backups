
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkHome.Interfaces;
using TalkHome.Models.ViewModels.BusinessIntelligence;
using PhoneNumbers;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Logger;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedContentModels;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

namespace TalkHome.Services
{

    public class BusinessIntelligenceService : IBusinessIntellienceService
    {
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly IAccountService AccountService;
        private readonly ILoggerService LoggerService;
        private readonly IContentService ContentService;
        
        public BusinessIntelligenceService(ITalkHomeWebService talkHomeWebService, IAccountService accountService, ILoggerService loggerService, IContentService contentService)
        {
            TalkHomeWebService = talkHomeWebService;
            LoggerService = loggerService;
            AccountService = accountService;
            ContentService = contentService;

        }

        #region Support methods for Plans/Bundle widget

        private bool OverThreshold(string type, UserAccountBundles bundle, int threshold, out double usage)
        {
            double max = 0.0;
            double remaining = 0.0;
            switch (type)
            {
                case "mins":
                    max = double.Parse(bundle.bundleminutes);
                    remaining = double.Parse(bundle.remainingminutes);
                    break;
                case "text":
                    max = double.Parse(bundle.bundletext);
                    remaining = double.Parse(bundle.remainingtext);
                    break;
                case "data":
                    string rm = bundle.remaininingdata.ToLower().Replace(" mb", "");
                    rm = rm.ToLower().Replace(" gb", "");
                    max = double.Parse(bundle.bundletext);
                    remaining = double.Parse(rm);
                    break;
            }
            usage = 100 - ((remaining / max) * 100);
            //test
            usage = 5;
            if (usage > threshold)
            {
                return true;
            }
            return false;
        }
        private IPublishedContent GetUpgradeBundle(IEnumerable<IPublishedContent> bundles, UserAccountBundles bundle, JObject rules, out JToken rule, out bool ruleApplied, out double absUsage)
        {
            string jPath = string.Format("$.currentPlan[?(@bundleId == '{0}')]", bundle.bundleguid.ToUpper());
            rule = rules.SelectToken(jPath);
            ruleApplied = false;
            string upgradeId = rule["thresholdUpgradeId"].ToString();
            string dUpgradeId = rule["defaultUpgradeId"].ToString();
            string upsellId = "";
            absUsage = 0.0;

            if (upgradeId != dUpgradeId)
            {
                string thresholdUnits = rule["thresholdUnits"].ToString();
                string criteria = rule["criteria"].ToString();
                int threshold = (int)rule["planThreshold"];

                string[] aUnits = thresholdUnits.Split(',');
                bool satisfied = false;
                int satisfiedCount = 0;
                double usage = 0.0;

                foreach (string u in aUnits)
                {
                    bool over = OverThreshold(u, bundle, threshold, out usage);
                    absUsage = usage;
                    if (over && criteria == "or")
                    {
                        satisfied = true; break;
                    }
                    else if (over && criteria == "and")
                    {
                        satisfiedCount++;
                    }
                }
                if (satisfied || (satisfiedCount == aUnits.Length))
                {
                    ruleApplied = true;
                    upsellId = upgradeId;
                }
                else
                {
                    upsellId = dUpgradeId;
                }
            }
            else
            {
                upsellId = upgradeId;
            }

            return bundles.OfType<Product>().Where(x => x.ProductUuid.TrimEnd() == upsellId).First();

        }
        private IPublishedContent GetRenewBundle(IEnumerable<IPublishedContent> bundles, List<UserAccountBundles> uBundles, int expireThreshold, out bool ruleApplied, out int expiryDays)
        {

            ruleApplied = false;
            expiryDays = 0;

            var renewId = "";

            foreach (var b in uBundles)
            {
                if (int.Parse(b.expiresindays) <= expireThreshold)
                {
                    renewId = b.bundleguid;
                    expiryDays = int.Parse(b.expiresindays);
                    ruleApplied = true;
                    break;
                }
            }

            if (renewId != "")
                return bundles.OfType<Product>().Where(x => x.ProductUuid.TrimEnd() == renewId.ToUpper()).First();
            return null;

        }

        #endregion

        /*
        {
            "creditAction": {
            "top":1,
            "countries":[{
            "countryCode":"GB",
            "upgradeId":"3CD97F38-75B8-4DE4-9ABF-9E4775FCF2A0"
            }]
            },
            "currentPlan":[{
	            "bundleId": "3CD97F38-75B8-4DE4-9ABF-9E4775FCF2A0",
	            "planThreshold": 80,
	            "thresholdUnits": "mins,data",
	            "criteria": "or",
                "thresholdUpgradeId":"4120A211-ABD1-4AE8-A911-258C7E2A81E2",
                "defaultUpgradeId":"4120A211-ABD1-4AE8-A911-258C7E2A81E2"
	            }
            ]
            "expiryNotice": 10,
        }
        */

        #region Bundle Analysis
   
        public async Task<BICallsViewModel> CreateBundleAnalysis(string apiToken, string json)
        {
            JObject rules = JObject.Parse(json);
            BICallsViewModel Calls = null;

            var res = await TalkHomeWebService.GetCallHistoryPage(new HistoryPageDTO("THM", 1, 25, apiToken));
            if (res != null)
            {
                Dictionary<string, int> counts = res.payload.GroupBy(x => x.called_party_number).ToDictionary(x => x.Key, x => x.Count());
                string mostCalledNumber = counts.OrderByDescending(x => x.Value).First().Key;
                var util = PhoneNumberUtil.GetInstance();
                var isoCode = util.GetRegionCodeForCountryCode(util.Parse("+" + mostCalledNumber, "").CountryCode);

                var RequestDTO = new AccountSummaryRequestDTO { productCode = "THM", token = apiToken };
                var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);


                var currentPlan = ResponseDTO.payload.userAccountBundles.First();

                var bundles = ContentService.GetTalkHomeMobilePlans();

                JToken rule;
                bool ruleApplied = false;
                double usage = 0.0;
                var upsell = GetUpgradeBundle(bundles, currentPlan, rules, out rule, out ruleApplied, out usage);

                //if we haven't reached bundle threshold //
                //then look for expiry in all their bundles//
                int expiryDays = 0;

                RuleType rt = RuleType.Upgrade;
                IPublishedContent renewalBundle = null;
                if (!ruleApplied)
                {
                    int renewThreshold = (int)rules["expiryAlert"];
                    renewalBundle = GetRenewBundle(bundles, ResponseDTO.payload.userAccountBundles, renewThreshold, out ruleApplied, out expiryDays);
                    if (ruleApplied)
                        rt = RuleType.Renew;
                }


                if (isoCode == "GB")
                {
                    var nationalRates = await TalkHomeWebService.GetTalkHomeMobileUKRates();

                    Calls = new BICallsViewModel
                    {
                        HasBundle = (currentPlan == null ? false : true),
                        CurrentPlan = currentPlan,
                        SMSRate = nationalRates.payload.Where(x => x.content == "Text Messages").First().description,
                        MobileRate = nationalRates.payload.Where(x => x.content == "Mobile").First().description,
                        LandLineRate = nationalRates.payload.Where(x => x.content == "National").First().description,
                        CountryName = "United Kingdom",
                        UpSellBundle = upsell,
                        Rule = rule,
                        RuleApplied = ruleApplied,
                        Usage = usage,
                        RuleType = rt,
                        RenewalBundle = renewalBundle

                    };

                }
                else
                {
                    var InternationalRates = await TalkHomeWebService.GetTalkHomeMobileInternationalRates();
                    var countryRate = InternationalRates.payload.Where(x => x.iso_code == isoCode).First();
                    Calls = new BICallsViewModel
                    {
                        HasBundle = false,
                        CurrentPlan = currentPlan,
                        SMSRate = countryRate.sms,
                        MobileRate = countryRate.mobile,
                        LandLineRate = countryRate.landline,
                        CountryName = countryRate.destination,
                        UpSellBundle = upsell,
                        Rule = rule,
                        RuleApplied = ruleApplied,
                        Usage = usage,
                        RuleType = rt,
                        RenewalBundle = renewalBundle
                    };
                }

                return Calls;
            }

            return Calls;

        }

        #endregion

        #region Airtime transfer


        public async Task<TransferPromotionsViewModel> CreatePromotions(string defaultCountries)
        {
            return await CreatePromotions("",defaultCountries);
        }

        public async Task<TransferPromotionsViewModel> CreatePromotions(string apiToken,string defaultCountries)
        {

            TransferPromotionItems promotions = await ReadTransferToFeed();
            string countryName = "";
            if (!string.IsNullOrEmpty(apiToken))
            {
                var res = await TalkHomeWebService.GetCallHistoryPage(new HistoryPageDTO("THM", 1, 25, apiToken));
               
                if (res != null)
                {
                    Dictionary<string, int> counts = res.payload.GroupBy(x => x.called_party_number).ToDictionary(x => x.Key, x => x.Count());
                    string mostCalledNumber = counts.OrderByDescending(x => x.Value).First().Key;
                    var util = PhoneNumberUtil.GetInstance();
                    var isoCode = util.GetRegionCodeForCountryCode(util.Parse("+" + mostCalledNumber, "").CountryCode);
                    countryName = AccountService.GetCountryList().Where(x => x.cca2 == isoCode).First().name.common;
                }
                if (promotions.TransferPromotions.Where(x => x.CountryName == countryName).Count() == 0)
                    countryName = defaultCountries;
            }
            else
            {
                //Get the default promotional country if not logged in
                countryName = defaultCountries;
            }


            IEnumerable<string> countries = promotions.TransferPromotions.Select(x => x.CountryName).Distinct().OrderBy(x => x);
            TransferPromotionsViewModel tvm = new TransferPromotionsViewModel(promotions,countries,countryName);

            return tvm;
        }

        private async Task<TransferPromotionItems> ReadTransferToFeed()
        {

            var httpClient = new HttpClient();
            string result = "";
            try
            {
                result = await httpClient.GetStringAsync("https://shop.transferto.com/shop/promotions2.xml");
            }
            catch(Exception e)
            {

            }
            XmlSerializer serializer = new XmlSerializer(typeof(TransferPromotionItems));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);

            XmlNode channel = doc.SelectSingleNode("//rss");

            using (TextReader reader = new StringReader(channel.InnerXml))
            {
                TransferPromotionItems tps = (TransferPromotionItems)serializer.Deserialize(reader);
                return tps;
            }

        }

        #endregion

        #region Upgrade Plans intelligence

        public List<UpgradePlan> GetRecommendedPlans(string json, List<UserAccountBundles> bundles)
        {
            bool test = false;
            List<UpgradePlan> recommendedPlans = new List<UpgradePlan>();
            if (!test)
            {
                JObject rules = JObject.Parse(json);
                foreach (UserAccountBundles b in bundles)
                {
                    
                    List<string> ups = new List<string>();
                    string jPath = string.Format("$.UpgradeRules[?(@CurrentPlan == '{0}')]", b.bundleguid.ToUpper());
                    JToken cp = rules.SelectToken(jPath);
                    if (cp != null)
                    {
                        JToken up = cp.SelectToken("UpgradePlans");

                        for (int i = 0; i < up.Count(); i++)
                        {
                            string s = up[i].SelectToken("UpgradePlan").ToString();
                            
                            ups.Add(s);
                        }

                    }
                    recommendedPlans.Add(new UpgradePlan(ups, b.bundleguid.ToUpper()));
                }
            }
            else if (test)
            {
                JObject rules = JObject.Parse(json);
                string[] ss = { "145DE44E-633C-45E6-ACBE-061E945C23A2", "3CD97F38-75B8-4DE4-9ABF-9E4775FCF2A0" };
                foreach (string it in ss)
                {
                    string id = it;
                    List<string> ups = new List<string>();
                    string jPath = string.Format("$.UpgradeRules[?(@CurrentPlan == '{0}')]", it.ToUpper());
                    JToken cp = rules.SelectToken(jPath);
                    if (cp != null)
                    {
                        JToken up = cp.SelectToken("UpgradePlans");

                        for (int i = 0; i < up.Count(); i++)
                        {
                            string s = up[i].SelectToken("UpgradePlan").ToString();
                            //if (!recommendedPlans.Contains(s))
                            ups.Add(s);
                            
                        }

                    }

                    recommendedPlans.Add(new UpgradePlan(ups, id));
                }
            }
            

            return recommendedPlans;
        }

        public List<UpgradePlan> GetTopPlans(string json)
        {
            List<string> topPlans = new List<string>();
            JObject rules = JObject.Parse(json);

            JToken tp = rules.SelectToken("TopPlans");

            if (tp != null)
            {  
                for (int i = 0; i < tp.Count(); i++)
                {
                    string s = tp[i].SelectToken("CurrentPlan").ToString();
                    topPlans.Add(s);
                }
            }

            List<UpgradePlan> up = new List<UpgradePlan>();

            up.Add(new UpgradePlan(topPlans, ""));
            return up;
               
        }
        

        #endregion

    }
}