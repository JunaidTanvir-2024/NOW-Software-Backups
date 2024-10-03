using System.Web.Mvc;
using TalkHome.Filters;
using TalkHome.Models.ViewModels.Umbraco;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Handles requests for Umbraco Help content type
    /// </summary>
    [GCLIDFilter]
    public class HelpController : BaseController
    {
        private Properties.URLs Urls = Properties.URLs.Default;

        /// <summary>
        /// GET /help
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult Help(RenderModel model)
        {
            var Payload = GetPayload();

            if (!Payload.TwoLetterISORegionName.Equals("GB"))
                return Redirect(Urls.NonGBHelp);

            return View(new CustomPageViewModel<Help>(model.Content, Payload));
        }

        /// <summary>
        /// GET /terms-and-conditions
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult TermsAndConditions(RenderModel model)
        {
            var Payload = GetPayload();

            if (!Payload.TwoLetterISORegionName.Equals("GB"))
                return Redirect(Urls.NonGBTermsAndConditions);

            return View(new CustomPageViewModel<Help>(model.Content, Payload));
        }

        /// <summary>
        /// GET /privacy-policy
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult PrivacyPolicy(RenderModel model)
        {
            var Payload = GetPayload();

            if (!Payload.TwoLetterISORegionName.Equals("GB"))
                return Redirect(Urls.NonGBPrivacyPolicy);

            return View(new CustomPageViewModel<Help>(model.Content, Payload));
        }

        [Route("unsubscribe")]
        public ActionResult Unsubscribe()
        {
            

            return View();
        }

    }
}
