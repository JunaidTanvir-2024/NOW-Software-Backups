using System.Web.Mvc;
using TalkHome.Filters;
using TalkHome.Models.ViewModels.Umbraco;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Handles front-end requests for Welcome content type
    /// </summary>
    [GCLIDFilter]
    public class FriendzoneController: BaseController
    {
        public override ActionResult Index(RenderModel model)
        {
            return ErrorPage(); // There is no default Welcome content.
        }

        /// <summary>
        /// GET /welcome
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult Friendzone(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<FriendZone>(model.Content, Payload));
        }
    }
}