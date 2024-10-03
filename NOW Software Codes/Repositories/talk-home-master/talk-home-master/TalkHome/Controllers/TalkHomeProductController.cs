using System.Web.Mvc;
using TalkHome.Interfaces;
using TalkHome.Models.ViewModels.Umbraco;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;
using WebMarkupMin.AspNet4.Mvc;
using TalkHome.WebServices.Interfaces;
using System.Threading.Tasks;
using TalkHome.Filters;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Manages Talk Home landing pages
    /// </summary>
    [GCLIDFilter]
    [MinifyHtml]
    public class TalkHomeProductController : BaseController
    {
        private readonly IContentService ContentService;
        private readonly ITalkHomeWebService TalkHomeWebService;
        
        private Properties.URLs Urls = Properties.URLs.Default;

        public TalkHomeProductController(IContentService contentService,ITalkHomeWebService talkHomeWebService)
        {
            ContentService = contentService;
            TalkHomeWebService = talkHomeWebService;
        }

        public override ActionResult Index(RenderModel model)
        {
            return ErrorPage(); // There is no default Talk Home Product content.
        }

        /// <summary>
        /// Landing page for Talk Home Mobile
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult TalkHomeMobile(RenderModel model)
        {
            var Payload = GetPayload();

            Payload.HomeRoot = "Homepage";

            if (!Payload.TwoLetterISORegionName.Equals("GB"))
                return Redirect(Urls.NonGBHome);

            var Page = (TalkHomeProduct)model.Content;
            var TopProducts = ContentService.GetTopProducts(Page.Id, 4);
            SetPayload(Payload);
            return View(new TalkHomeProductViewModel(Payload, Page, TopProducts));
        }

        /// <summary>
        /// Landing page for Talk Home App
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public  async Task<ActionResult> TalkHomeApp(RenderModel model)
        {
            return View();
            //var Payload = GetPayload();
            //var Page = (TalkHomeProduct)model.Content;
            //var TopProducts = ContentService.GetTopProducts(Page.Id, 4);

            //var InternationalRates = await TalkHomeWebService.GetTalkHomeMobileInternationalRates();

            //return View(new TalkHomeProductViewModel(Payload, Page, TopProducts, InternationalRates.payload));
        }

        /// <summary>
        /// Landing page for Calling Cards
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult CallingCards(RenderModel model)
        {
            var Payload = GetPayload();

            if (!Payload.TwoLetterISORegionName.Equals("GB"))
                return Redirect(Urls.NonGBHome);

            var Page = (TalkHomeProduct)model.Content;
            var TopProducts = ContentService.GetTopUps(Page.Id, 4);

            return View(new TalkHomeProductViewModel(Payload, Page, TopProducts));
        }
    }
}
