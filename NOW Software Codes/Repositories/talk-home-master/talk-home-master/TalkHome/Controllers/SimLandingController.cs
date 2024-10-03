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
    /// <summary>
    /// Handles front-end requests for Welcome content type
    /// </summary>
    [GCLIDFilter]
    public class SimLandingController : BaseController
    {
        private readonly IAccountService AccountService;
        
        public SimLandingController(IAccountService accountService)
        { 
            AccountService = accountService;
        }

        public override ActionResult Index(RenderModel model)
        {
            return ErrorPage(); // There is no default Welcome content.
        }

        /// <summary>
        /// GET /welcome
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult SimLanding(RenderModel model)
        {
            var Payload = GetPayload();

            var CountryList = AccountService.GetCountryList();

            return View(new CustomPageViewModel<SimLanding>(model.Content, Payload, new AddressDetailsViewModel(CountryList, new AddressModel(), Payload.TwoLetterISORegionName.Equals("GB"))));

        }
    }
}