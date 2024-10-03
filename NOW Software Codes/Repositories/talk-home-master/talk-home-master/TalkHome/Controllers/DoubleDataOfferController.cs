using System.Web.Mvc;
using TalkHome.Models.ViewModels.Umbraco;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;
using TalkHome.Interfaces;
using TalkHome.Models;
using TalkHome.Models.ViewModels;
using TalkHome.Filters;

namespace TalkHome.Controllers
{
    [GCLIDFilter]
    public class DoubleDataOfferController : BaseController
    {
        private readonly IAccountService AccountService;

        public DoubleDataOfferController(IAccountService accountService)
        {
            AccountService = accountService;
        }

       

        /// <summary>
        /// GET /welcome
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult DoubleDataOffer(RenderModel model)
        {
            return RedirectToAction("TripleData", "Offers");

        }
    }
}