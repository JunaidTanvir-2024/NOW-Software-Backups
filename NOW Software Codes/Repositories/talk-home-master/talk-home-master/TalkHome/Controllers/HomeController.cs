using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.Filters;
using TalkHome.Interfaces;
using TalkHome.Models.ViewModels;
using TalkHome.Models.ViewModels.BusinessIntelligence;
using TalkHome.Models.ViewModels.Umbraco;
using TalkHome.Models.WebApi.CallingCards;
using TalkHome.Models.WebApi.Rates;
using TalkHome.Properties;
using TalkHome.WebServices.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Custom controller for Umbraco Homepage
    /// </summary>


    [GCLIDFilter]
    public class HomeController : BaseController
    {
        private readonly IContentService ContentService;
        private Properties.URLs Urls = Properties.URLs.Default;
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly IAccountService AccountService;
        private readonly IBusinessIntellienceService BusinessIntelligenceService;
        private static Properties.ContentIds ContentIds = Properties.ContentIds.Default;

        public HomeController(IJWTService jwtService, IContentService contentService, ITalkHomeWebService talkHomeWebService, IAccountService accountService, IBusinessIntellienceService businessIntelligenceService)
        {
            ContentService = contentService;
            TalkHomeWebService = talkHomeWebService;
            AccountService = accountService;
            BusinessIntelligenceService = businessIntelligenceService;

        }

        /// <summary>
        /// GET /
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view/returns>
        public async Task<ActionResult> Homepage(RenderModel model)
        {
           
            var Payload = GetPayload();

            if (!Payload.TwoLetterISORegionName.Equals("GB"))
            {
                return Redirect(Urls.NonGBHome);
            }

            var Page = (Home)model.Content;

            bool isHome = false;
            if (Page.Name == "Homepage")
            {
                isHome = true;
            }
            else if (Page.Name == "CallingCards")
            {
                return RedirectPermanent(ConfigurationManager.AppSettings["CallingCardsBaseUrl"]);
                ViewBag.IsCallingCardsPage = true;
            }

            var Campaign = ContentService.GetCampaign(Page.Children);

            TestNestedDoc Fold = ContentService.GetFold(Page.Children);
            BICallsViewModel Calls = null;
            List<UpgradePlan> UpgradePlans = null;
            List<UpgradePlan> TopPlans = null;
            List<string> CurrentPlans = new List<string>();
            IEnumerable<IPublishedContent> TopCCs = null;
            TalkHomeProductViewModel CallingCardModel = null;
            IList<MinutesRecord> CallingCardMinutes = null;
            IList<Rate> ratesVM = null;


            TransferPromotionsViewModel TransferPromotions = new TransferPromotionsViewModel(null, null, "");
            bool biWidget = false;
            bool tpWidget = false;
            bool fbPlans = false;
            bool iRatesPPC = false;
            var defaultCountries = "";
            string ugJson = "";
            string topJson = "";

            foreach (IPublishedContent i in Fold.FoldContainer)
            {
                if (i.DocumentTypeAlias == "intelligentCalls")
                {
                    biWidget = true;
                }

                if (i.DocumentTypeAlias == "transferPromotions")
                {
                    tpWidget = true;
                    defaultCountries = i.GetPropertyValue("defaultCountries").ToString();
                }

                if (i.DocumentTypeAlias == "featureBox")
                {
                    var intelligence = i.GetPropertyValue("intelligence").ToString();
                    if (intelligence == "Plans")
                    {
                        fbPlans = true;
                    }
                    if (i.DocumentTypeAlias == "internationalRates")
                    {
                        iRatesPPC = true;
                    }
                }
            }

            if (AccountService.IsAuthorized(Payload) && !iRatesPPC)
            {
                if (biWidget)
                {
                    Rules rules = (Rules)Umbraco.TypedContent(int.Parse(ContentIds.CallHistoryRules));
                    string json = rules.Json;
                    Calls = await BusinessIntelligenceService.CreateBundleAnalysis(Payload.ApiToken, json);
                }
                if (tpWidget)
                {
                    TransferPromotions = await BusinessIntelligenceService.CreatePromotions(Payload.ApiToken, defaultCountries);
                }
                if (fbPlans)
                {

                    Rules rules = (Rules)Umbraco.TypedContent(int.Parse(ContentIds.UpgradePlans));
                    ugJson = rules.Json;
                    //Comment out the user bundle look up//
                    /*
                    var RequestDTO = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
                    var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

                    if (ResponseDTO != null && ResponseDTO.payload.userAccountBundles.Count > 0)
                    {
                        UpgradePlans = BusinessIntelligenceService.GetRecommendedPlans(ugJson, ResponseDTO.payload.userAccountBundles);

                    }
                    */
                    TopPlans = BusinessIntelligenceService.GetTopPlans(ugJson);
                }
            }
            else if (tpWidget)
            {
                TransferPromotions = await BusinessIntelligenceService.CreatePromotions(defaultCountries);
            }
            else if (fbPlans)
            {
                Rules rules = (Rules)Umbraco.TypedContent(int.Parse(ContentIds.UpgradePlans));
                ugJson = rules.Json;
                TopPlans = BusinessIntelligenceService.GetTopPlans(ugJson);
            }

            if (!isHome && !iRatesPPC)
            {
                TopCCs = ContentService.GetTopUps(1345, 4);
                TalkHomeProduct CCPage = (TalkHomeProduct)Umbraco.TypedContent(1345);
                CallingCardModel = new TalkHomeProductViewModel(Payload, CCPage, TopCCs);
                var Minutes = await TalkHomeWebService.GetCallingCardsMinutes();
                CallingCardMinutes = Minutes.payload;
            }

            if (iRatesPPC)
            {
                var InternationalRates = await TalkHomeWebService.GetTalkHomeMobileInternationalRates();
                ratesVM = InternationalRates.payload;
            }

            Payload.HomeRoot = Page.Name;
            SetPayload(Payload);

            SetPayload(Payload);
            WidgetViewModel Widgets = new WidgetViewModel(Fold, Calls, TransferPromotions, new PlansViewModel(UpgradePlans, ugJson, TopPlans));

            var TopUps = ContentService.GetAllTopUps();

            Widgets.VerifiedTopUps = new VerifiedTopUps
            {
                TopUps = TopUps["THM"].ToList()
            };

            return View(new HomepageViewModel<Home>(Payload, Page, Campaign, Widgets, CallingCardModel, CallingCardMinutes));
        }

        [Route("robots.txt", Name = "GetRobotsText"), OutputCache(Duration = 86400)]
        public ContentResult RobotsText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("user-agent: *");
            stringBuilder.AppendLine("disallow: /terms-conditions");
            stringBuilder.AppendLine("disallow: /privacy-policy");
            stringBuilder.AppendLine("disallow: /app/reconnecting");
            stringBuilder.AppendLine("disallow: /app/minutemaker");
            stringBuilder.AppendLine("noindex: /app/reconnecting");
            stringBuilder.AppendLine("noindex: /app/minutemaker");
            return this.Content(stringBuilder.ToString(), "text/plain", Encoding.UTF8);
        }

        [Route("talkhome-takeover-brandon")]
        public ActionResult brandon()
        {
            return RedirectPermanent("https://talkhome.co.uk/article/talkhome-takeover-brandon");
        }

        [Route("talkhome-takeover-einer")]
        public ActionResult Einer()
        {
            return RedirectPermanent("https://talkhome.co.uk/article/talkhome-takeover-einer");
        }


        [Route("talkhome-takeover-valeriya")]
        public ActionResult Valeriya()
        {
            return RedirectPermanent("https://talkhome.co.uk/article/talkhome-takeover-valeriya");
        }

      
    }
}