using System.Configuration;
using System.Web.Mvc;
using TalkHome.Filters;
using TalkHome.Models.ViewModels.Umbraco;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Handles requests for Umbraco Help Page content type
    /// </summary>
    [GCLIDFilter]
    public class HelpPageController : BaseController
    {
        /// <summary>
        /// Displays help, terms and conditions, privacy policy pages
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view with the required model</returns>
        public ActionResult HelpPage(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<HelpPage>(model.Content, Payload));
        }

        /// <summary>
        /// /privacy-policy/calling-cards/
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("privacy-policy/calling-cards/")]
        public ActionResult PrivacyPolicyCallingCards(RenderModel model)
        {
            return RedirectPermanent(ConfigurationManager.AppSettings["CallingCardsBaseUrl"] + "privacy-policy/calling-cards/");
        }

        /// <summary>
        /// /terms-and-conditions/calling-cards/
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("terms-and-conditions/calling-cards/")]
        public ActionResult TermsCallingCards(RenderModel model)
        {
            return RedirectPermanent(ConfigurationManager.AppSettings["CallingCardsBaseUrl"] + "terms-and-conditions/calling-cards/");
        }

        /// <summary>
        /// /help/calling-cards/
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("help/calling-cards/")]
        public ActionResult HelpCallingCards(RenderModel model)
        {
            return RedirectPermanent(ConfigurationManager.AppSettings["CallingCardsBaseUrl"] + "help/calling-cards/");
        }


        [Route("complaints-procedure")]
        public ActionResult ComplaintsProcedure()
        {
            return View();
        }

    }
}
