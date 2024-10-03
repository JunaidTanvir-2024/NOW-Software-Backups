using System.Web.Mvc;
using TalkHome.Filters;
using TalkHome.Models.ViewModels.Umbraco;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;
using WebMarkupMin.AspNet4.Mvc;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Manages requests for Umbraco Simple Pages content type
    /// </summary>
    [GCLIDFilter]
    [MinifyHtml]
    public class SimplePageController : BaseController
    {
        public override ActionResult Index(RenderModel model)
        {
            return ErrorPage(); // There is no default Simple Page content.
        }

        /// <summary>
        /// GET /about-talk-home
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult AboutTalkHome(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<SimplePage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /contact-us
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult ContactUs(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<SimplePage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /cookie-policy
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult CookiePolicy(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<SimplePage>(model.Content, Payload));
        }



        /// <summary>
        /// GET /using-calling-cards
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult UsingCallingCards(RenderModel model)
        {
            var Payload = GetPayload();
            return RedirectPermanent("https://talk-home.co.uk/using-calling-cards/");
            //return View(new CustomPageViewModel<SimplePage>(model.Content, Payload));
        }



    }
}