using System.Linq;
using System.Web.Mvc;
using TalkHome.Filters;
using TalkHome.Interfaces;
using TalkHome.Models;
using TalkHome.Models.Enums;
using TalkHome.Models.ViewModels;
using TalkHome.Models.ViewModels.DTOs;
using TalkHome.Models.ViewModels.Umbraco;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Controllers
{
    [GCLIDFilter]
    public class OffersController : BaseController
    {
        private readonly IContentService ContentService;
        private readonly IAccountService AccountService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public OffersController(IContentService contentService, IAccountService accountService)
        {
            ContentService = contentService;
            AccountService = accountService;
        }
        // GET: Offers
        public ActionResult Index()
        {
            return View();
        }
        [Route("Offers/tripleData")]
        [Route("tripleData")]
        public ActionResult TripleData()
        {
            return RedirectToActionPermanent("DataMaxOffer");
        }


        [Route("pakistan")]
        public ActionResult Pakistan()
        {
            return RedirectToActionPermanent("CheapCallsPakistan");
        }

        [Route("cheap-calls-to-pakistan")]
        public ActionResult CheapCallsPakistan()
        {
            return View("Pakistan");
        }


        [Route("bangladesh")]
        public ActionResult Bangladesh()
        {
            return RedirectToActionPermanent("CheapCallBangladesh");
        }

        [Route("cheap-call-to-bangladesh")]
        public ActionResult CheapCallBangladesh()
        {
            return View("bangladesh");
        }



        [Route("nigeria")]
        public ActionResult Nigeria()
        {
            return RedirectToActionPermanent("CheapCallsNigeria");
        }

        [Route("cheap-calls-to-nigeria")]
        public ActionResult CheapCallsNigeria()
        {
            return View("Nigeria");
        }


        [Route("ghana")]
        public ActionResult Ghana()
        {
            return View();
        }
        [Route("philippines")]
        public ActionResult Philippines()
        {
            return View();
        }

        [Route("datamax")]
        public ActionResult datamax()
        {
            var dataMaxId = 1459;
            string productCode = "THM";
            var Product = ContentService.GetProducts(dataMaxId);

            var Payload = GetPayload();

            Payload.TopUp.Clear();
            Payload.Purchase.Clear();
            Payload.Purchase.Add(dataMaxId);
            Payload.Checkout = new CheckoutPageDTO { Verify = productCode, ProductType = ProductType.Bundle.ToString(), Total = Product.ProductPrice };
            Response.Cookies.Add(AccountService.EncodeCookie(Payload));

            if (AccountService.IsAuthorized(Payload))
            {
                var Reference = "";

                try
                {
                    Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(productCode)).Select(x => x.Reference).First();
                }
                catch // The registred user doesn't have that product. Send to add a product screen on MyAccount
                {
                    return ErrorRedirect(((int)Messages.ProductNotRegisteredForPurchase).ToString(), Urls.MyAccount + "/" + productCode);
                }

                Payload.Checkout.Reference = Reference;
                SetPayload(Payload);
                return Redirect(Urls.Checkout);
            }

            return Redirect(Urls.Checkout);
        }

        [Route("data-max-offer")]
        public ActionResult DataMaxOffer()
        {
            return RedirectToActionPermanent("MobileDataPlansUk");
        }

        [Route("mobile-data-plans-uk")]
        public ActionResult MobileDataPlansUk()
        {
            return View("DataMaxOffer");
        }

        [Route("competition-plan")]
        public ActionResult CompetitionPlan()
        {
            return View();
        }

        [Route("giveaway")]
        public ActionResult GiveAway()
        {
            return View();
        }


        //Festive Bundles

        [Route("festivebundles/nigeria")]
        public ActionResult FestiveBundlesNigeria()
        {
            return View();
        }


        [Route("festivebundles/nigeria1")]
        public ActionResult FestiveBundlesNigeria1()
        {
            return View();
        }

        [Route("festivebundles/bangladesh")]
        public ActionResult FestiveBundlesBangladesh()
        {
            return View();
        }

        [Route("festivebundles/bangladesh1")]
        public ActionResult FestiveBundlesBangladesh1()
        {
            return View();
        }

        [Route("festivebundles/ghana")]
        public ActionResult FestiveBundlesGhana()
        {
            return View();
        }

        [Route("festivebundles/ghana1")]
        public ActionResult FestiveBundlesGhana1()
        {
            return View();
        }

        [Route("festivebundles/pakistan")]
        public ActionResult FestiveBundlesPakistan()
        {
            return View();
        }

        [Route("festivebundles/pakistan1")]
        public ActionResult FestiveBundlesPakistan1()
        {
            return View();
        }

        [Route("festivebundles/uk")]
        public ActionResult FestiveBundlesUK()
        {
            return View();
        }

        [Route("festivebundles/uk1")]
        public ActionResult FestiveBundlesUK1()
        {
            return View();
        }

        [Route("prizedraw")]
        public ActionResult PrizeDraw()
        {
            return View();
        }

        [Route("App")]
        public ActionResult AppLandingPage()
        {
            return View();
        }


        [Route("ramadan")]
        public ActionResult Ramazan()
        {
            return View();
        }

        [Route("eid")]
        public ActionResult Eid()
        {
            return View();
        }

        [Route("minutemaker")]
        [Route("app/minutemaker")]
        public ActionResult MinuteMaker()
        {
            return View();
        }

        [Route("reconnecting")]
        [Route("app/reconnecting")]
        public ActionResult Reconnecting()
        {
            return View();
        }

        [HttpPost]
        [Route("Insertemail_minutemaker")]
        public ActionResult Insertemail_minutemaker(Insertemail_minutemaker_model model)
        {
            model.type = "2";
            var result = AccountService.Insertemail_minutemaker(model);
            return Json(result);
        }


        [HttpPost]
        [Route("Insertemail_reconnecting")]
        public ActionResult Insertemail_reconnecting(Insertemail_minutemaker_model model)
        {
            model.type = "1";
            var result = AccountService.Insertemail_minutemaker(model);
            return Json(result);
        }
    }
}