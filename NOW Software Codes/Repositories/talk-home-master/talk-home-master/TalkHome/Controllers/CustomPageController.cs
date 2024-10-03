using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.Filters;
using TalkHome.Interfaces;
using TalkHome.Logger;
using TalkHome.Models;
using TalkHome.Models.Enums;
using TalkHome.Models.Pay360;
using TalkHome.Models.ViewModels;
using TalkHome.Models.ViewModels.Umbraco;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.CallingCards;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.WebApi.Payment;
using TalkHome.Models.WebApi.Rates;
using TalkHome.WebServices.Interfaces;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;
using WebMarkupMin.AspNet4.Mvc;
using System.Web;
using System.Linq;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Manages Custom Pages requests and views.
    /// </summary>
    [GCLIDFilter]
    public class CustomPageController : BaseController
    {
        private readonly IContentService ContentService;
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly IAccountService AccountService;
        private readonly IPaymentService PaymentService;
        private readonly ILoggerService LoggerService;
        private readonly IPay360Service Pay360Service;
        private readonly ITalkHomeAppWebService TalkHomeAppWebService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public CustomPageController(IContentService contentService, ITalkHomeWebService talkHomeWebService, IAccountService accountService, IPaymentService paymentService, ILoggerService loggerService, IPay360Service pay360Service, ITalkHomeAppWebService talkHomeAppWebService)
        {
            ContentService = contentService;
            TalkHomeWebService = talkHomeWebService;
            AccountService = accountService;
            PaymentService = paymentService;
            LoggerService = loggerService;
            Pay360Service = pay360Service;
            TalkHomeAppWebService = talkHomeAppWebService;
        }

        public override ActionResult Index(RenderModel model)
        {
            return ErrorPage(); // There is no default Custom Page content.
        }

        /// <summary>
        /// GET /basket
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult Basket(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /login
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult Login(RenderModel model, string ReturnUrl)
        {

            var Payload = GetPayload();
            ViewBag.ReturnUrl = ReturnUrl;
            if (AccountService.IsAuthorized(Payload) && Payload.ProductCodes.Count > 0)
            {
                return Redirect(Urls.MyAccount);
            }

            if (AccountService.IsAuthorized(Payload) && Payload.ProductCodes.Count == 0)
            {
                return Redirect(Urls.RegisterProduct);
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /free-sim/uk-plans
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <param name="NavigateToDataPackages">In Footer, when user clicks on 'Data Plans', On page load, simulate mouseclick on Data to load Data div</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult TalkHomeMobileUKPlans(RenderModel model, bool NavigateToDataPackages = false)
        {
            var Payload = GetPayload();

            var Plans = ContentService.GetTalkHomeMobilePlans();

            var CountryList = AccountService.GetCountryList();

            ViewBag.NavigateToDataPackages = NavigateToDataPackages;

            //return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, Plans, CountryList, null));
            return View("TalkHomeMobileUKPlansNew", new CustomPageViewModel<CustomPage>(model.Content, Payload, Plans, CountryList, null));
        }

        [Route("mobile/plans/{countryname}")]
        [Route("mobile/plans/{countryname}/{planprice}")]
        public async Task<ActionResult> TalkHomeMobileUKPlansCustom(string countryname, string planprice, bool NavigateToDataPackages = false)
        {
            var Payload = GetPayload();
            ViewBag.countryname = countryname;
            ViewBag.planprice = planprice;
            var Plans = ContentService.GetTalkHomeMobilePlans();

            var CountryList = AccountService.GetCountryList();

            ViewBag.NavigateToDataPackages = NavigateToDataPackages;

            //return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, Plans, CountryList, null));
            return View("TalkHomeMobileUKPlansCustom", new TalkHomeMobileInternationalPlanCustom(Payload, Plans, CountryList, null));
        }




        /// <summary>
        /// GET /free-sim/international-plans
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public async Task<ActionResult> TalkHomeMobileInternationalPlans(RenderModel model, bool SelectAllPlans = false)
        {

            var Payload = GetPayload();

            var Plans = ContentService.GetTalkHomeMobilePlans();

            var InternationalRates = await TalkHomeWebService.GetTalkHomeMobileInternationalRates();

            var CountryList = AccountService.GetCountryList();

            ViewBag.SelectAllPlans = SelectAllPlans;
            //return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, Plans, CountryList, InternationalRates.payload));
            return View("TalkHomeMobileInternationalPlansNew", new CustomPageViewModel<CustomPage>(model.Content, Payload, Plans, CountryList, InternationalRates?.payload ?? null));

        }

        [Route("mobile/international-plans/{countryname}")]
        [Route("mobile/international-plans/{countryname}/{planprice}")]
        public async Task<ActionResult> TalkHomeMobileInternationalPlansCustom(string countryname, string planprice, bool SelectAllPlans = false)
        {
            if (Request.QueryString["gclid"] != null)
            {
                string GCLID = Request.QueryString["gclid"];
                if (!Request.Cookies.AllKeys.Contains("gclid"))
                {
                    HttpCookie gclidcookie = new HttpCookie("gclid");
                    gclidcookie.Value = GCLID;
                    gclidcookie.Expires = DateTime.Now.AddDays(Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GCLIDDurationTime"]));
                    Response.Cookies.Add(gclidcookie);
                    TempData["GCLID"] = GCLID;
                }

            }
            var Payload = GetPayload();
            var Plans = ContentService.GetTalkHomeMobilePlans();
            var InternationalRates = await TalkHomeWebService.GetTalkHomeMobileInternationalRates();
            var CountryList = AccountService.GetCountryList();
            ViewBag.countryname = countryname;
            ViewBag.planprice = planprice;
            ViewBag.SelectAllPlans = SelectAllPlans;
            //return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, Plans, CountryList, InternationalRates.payload));
            return View(new TalkHomeMobileInternationalPlanCustom(Payload, Plans, CountryList, InternationalRates.payload));
        }

        /// <summary>
        /// GET /free-sim/uk-rates
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public async Task<ActionResult> TalkHomeMobileUKRates(RenderModel model)
        {
            var Payload = GetPayload();
            var UKRates = await TalkHomeWebService.GetTalkHomeMobileUKRates();

            if (UKRates == null)
            {
                return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, new List<UKNationalRate>()));
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, UKRates.payload));
        }

        /// <summary>
        /// GET /free-sim/international-rates
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public async Task<ActionResult> TalkHomeMobileInternationalRates(RenderModel model)
        {
            var Payload = GetPayload();
            var InternationalRates = await TalkHomeWebService.GetTalkHomeMobileInternationalRates();

            if (InternationalRates == null)
            {
                return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, new List<Rate>()));
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, InternationalRates.payload));
        }

        /// <summary>
        /// GET /free-sim/roaming-rates
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public async Task<ActionResult> TalkHomeMobileRoamingRates(RenderModel model)
        {
            var Payload = GetPayload();
            var RoamingRates = await TalkHomeWebService.GetTalkHomeMobileRoamingRates();

            if (RoamingRates == null)
            {
                return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, new List<RoamingRate>()));
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, RoamingRates.payload));
        }

        /// <summary>
        /// GET /order-a-sim
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult SimOrder(RenderModel model)
        {
            if (Request.QueryString["gclid"] != null)
            {
                string GCLID = Request.QueryString["gclid"];
                if (!Request.Cookies.AllKeys.Contains("gclid"))
                {
                    HttpCookie gclidcookie = new HttpCookie("gclid");
                    gclidcookie.Value = GCLID;
                    gclidcookie.Expires = DateTime.Now.AddDays(Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GCLIDDurationTime"]));
                    Response.Cookies.Add(gclidcookie);
                    TempData["GCLID"] = GCLID;
                }

            }

            var Payload = GetPayload();
            string route = HttpContext.Request.Path;
            var CountryList = AccountService.GetCountryList();
            if (route.ToLower() == "/order-sim" || route.ToLower() == "/order-sim/")
            {
                return Redirect(Urls.OrderSIM);
            }
            else
            {
                TempData["FromOffer"] = null;
            }
     
            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, new AddressDetailsViewModel(CountryList, new AddressModel(), Payload.TwoLetterISORegionName.Equals("GB"))));
        }

        /// <summary>
        /// GET /talk-home-app/bundles
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult TalkHomeAppBundles(RenderModel model)
        {
            var Payload = GetPayload();

            var Bundles = ContentService.GetBundles();

            var CountryList = AccountService.GetCountryList();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, Bundles, CountryList, null));
        }

        /// <summary>
        /// GET /talk-home-app/download
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult TalkHomeAppDownload(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /account/sim-order
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        //public ActionResult SuccessfulSimOrder(RenderModel model)
        //{
        //    var Payload = GetPayload();

        //    return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        //}

        [Route("order-a-sim/success")]
        public ActionResult SimOrderSuccess(RenderModel model)
        {

            long OrderId = TempData["OrderId"] != null ? Convert.ToInt64(TempData["OrderId"]) : 0;

            var gclidcookie = Request.Cookies["gclid"] != null ? Request.Cookies["gclid"].Value : null;
            if (gclidcookie != null)
                AccountService.SaveGCLID_NormalSim(gclidcookie, OrderId);
            var Payload = GetPayload();
            return View(Payload);
        }

        /// <summary>
        /// GET /account/card-order
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult SuccessfulCallingCardOrder(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /talk-home-app/rates
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public async Task<ActionResult> TalkHomeAppRates(RenderModel model)
        {
            var Payload = GetPayload();
            var Rates = await TalkHomeWebService.GetTalkHomeAppRates(Payload.TwoLetterISORegionName ?? "GB");

            if (Rates == null)
            {
                return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, new List<Rate>()));
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, Rates.payload));
        }

        [HttpGet]
        [Route("TalkHomeAppInternationalRatesByCurrency")]
        public async Task<JsonResult> TalkHomeAppInternationalRatesByCurrency(string CurrencyType)
        {
            var Payload = GetPayload();
            var InternationalRates = await TalkHomeAppWebService.GetTalkHomeAppInternationalRates(CurrencyType);

            if (InternationalRates == null)
            {
                return Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
            }

            return Json(InternationalRates, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// GET /calling-cards/minutes
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public async Task<ActionResult> CallingCardsMinutes(RenderModel model)
        {
            var Payload = GetPayload();
            var Minutes = await TalkHomeWebService.GetCallingCardsMinutes();

            if (Minutes == null)
            {
                return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, new List<MinutesRecord>()));
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, Minutes.payload));
        }

        /// <summary>
        /// GET /order-a-calling-card
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult CallingCardOrder(RenderModel model)
        {
            return RedirectPermanent(ConfigurationManager.AppSettings["CallingCardsBaseUrl"] + "order-a-calling-card");
            var Payload = GetPayload();

            var CountryList = AccountService.GetCountryList();

            var Products = ContentService.GetAllTopUps()["THCC"];

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, Products, new AddressDetailsViewModel(CountryList, new AddressModel(), Payload.TwoLetterISORegionName.Equals("GB"))));
        }

        /// <summary>
        /// GET /sign-up
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult SignUp(RenderModel model)
        {
            var Payload = GetPayload();

            if (AccountService.IsAuthorized(Payload))
            {
                return Redirect(Urls.MyAccount);
            }

            var CountryList = AccountService.GetCountryList();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, new AddressDetailsViewModel(CountryList, new AddressModel(), Payload.TwoLetterISORegionName.Equals("GB"))));
        }

        /// <summary>
        /// GET /top-up
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult TopUp(RenderModel model)
        {
            var Payload = GetPayload();

            if (AccountService.IsAuthorized(Payload)) // Sets default top up for checkout given the only product code
            {
                if (Payload.ProductCodes.Count == 1)
                {
                    Payload.TopUp.Clear();
                    Payload.Purchase.Clear();
                    Payload.TopUp.Add(ContentService.GetDefaultTopUpByProductCode(Payload.ProductCodes[0].ProductCode).Id);
                    Payload.Checkout = new Models.ViewModels.DTOs.CheckoutPageDTO { Reference = Payload.ProductCodes[0].Reference, ProductType = ProductType.TopUp.ToString() };
                    SetPayload(Payload);
                }
            }

            var TopUps = ContentService.GetAllTopUps();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, TopUps));
        }

        /// <summary>
        /// GET /verify-number
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult VerifyNumber(RenderModel model, string Source = "")
        {
            var Payload = GetPayload();

            if (AccountService.IsAuthorized(Payload))
            {
                return Redirect(Urls.Checkout);
            }

            var CountryList = AccountService.GetCountryList();
            ViewBag.Source = Source;

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, CountryList));
        }

        /// <summary>
        /// GET /checkout
        /// </summary>
        /// <param name="model">The Umbraco model</param>pay360account
        /// <returns>The view</returns>
        [CheckoutPageRequest]
        public async Task<ActionResult> Checkout(RenderModel model)
        {
            var Payload = GetPayload();

            string paymentProvider = string.Empty;

            if (TempData["IsInternationalTopUp"] != null && System.Convert.ToBoolean(TempData["IsInternationalTopUp"]) == true)
            {
                TempData["IsInternationalTopUp"] = true;
            }
            //if (Payload.Checkout.Verify == "THM")
            //{
            paymentProvider = ConfigurationManager.AppSettings["PaymentProvider"];
            //}
            //else
            //{
            //    paymentProvider = "MIPay";
            //}

            string CustId = "";
            var AddressModel = new AddressModel();
            var FullName = new FullNameModel();
            bool reg = false;
            var MiPayCustomer = (dynamic)null;
            var Pay360Customer = (dynamic)null;
            ViewBag.CheckoutPage = true;

            var TopUps = ContentService.GetAllTopUps();
            var VerifiedTopUps = TopUps[Payload.Checkout.Verify];

            if (AccountService.IsAuthorized(Payload))
            {
                var UserAccount = await AccountService.GetAccountDetails(Payload.ApiToken);
                AccountService.TryGetBillingAddress(UserAccount, out AddressModel);
                FullName = Mapper.Map<FullNameModel>(UserAccount.payload);
                reg = true;
            }

            if (reg && paymentProvider == "MIPay")
            {
                MiPayCustomer = await PaymentService.GetCustomer(new GetCustomerRequestModel(ChannelType.Web, Payload.Checkout.Reference, Payload.Checkout.Verify.Equals("THCC") ? UniqueIDType.Card : UniqueIDType.Msisdn));

                if (MiPayCustomer == null)
                {
                    return GetMiPayCustomerFailed(LoggerService);
                }

                if (PaymentService.TryGetCustId(MiPayCustomer, out CustId))
                {
                    AddressModel = Mapper.Map<AddressModel>(MiPayCustomer.payload.billingAddress);
                    FullName = Mapper.Map<FullNameModel>(MiPayCustomer.payload);
                }
            }
            else if (reg && paymentProvider == "Pay360")
            {
                Pay360Customer = new Pay360CustomerModel();

                if (Payload.FullName != null)
                {
                    Pay360CustomerRequestModel pay360RequestRequest = new Pay360CustomerRequestModel
                    {
                        customerUniqueRef = Payload.FullName.Email,
                        productCode = Payload.Checkout.Verify
                    };

                    Pay360Customer = await Pay360Service.GetCustomer(pay360RequestRequest);
                    var Pay360CardsResponse = await Pay360Service.Pay360GetCards(pay360RequestRequest);

                    if (Pay360CardsResponse.errorCode == 0)
                    {
                        Pay360Customer.payload.PaymentMethods = Pay360CardsResponse.payload.paymentMethodResponses;
                    }
                }
            }

            if (Payload.Checkout.DeliveryIsBilling)
            {
                var mo = Payload.Checkout.MailOrder;
                AddressModel = new AddressModel
                {
                    addressL1 = mo.addressL1,
                    addressL2 = mo.addressL2,
                    city = mo.city,
                    county = mo.county,
                    country = mo.country,
                    postCode = mo.postCode
                };
            }

            if (reg)
            {
                decimal Total = Payload.Checkout.Total;

                var RequestDTO = new AccountSummaryRequestDTO { productCode = Payload.Checkout.Verify, token = Payload.ApiToken };
                var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

                if (ResponseDTO != null && ResponseDTO.payload != null)
                {
                    DateTime _date;
                    string date = "";
                    _date = DateTime.Parse(ResponseDTO.payload.userAccountSummary.activeDate);
                    date = _date.ToString("yyyy-MM-dd");
                    TempData["sim_activationdate"] = date;

                    if (Total > ResponseDTO.payload.userAccountSummary.creditRemaining)
                    {
                        TempData["NoCredit"] = "no-credit";
                    }
                }
                else
                {
                    TempData["NoCredit"] = "no-credit";
                }
            }
            else
            {
                TempData["NoCredit"] = "no-credit";
            }

            var CountryList = AccountService.GetCountryList();

            if (reg)
            {
                Payload.CustId = CustId;
            }
            else
            {
                Payload.FullName.FirstName = "";
            }

            SetPayload(Payload);

            if (TempData["AutoTopUp"] != null)
            {
                TempData["AutoTopUp"] = TempData["AutoTopUp"];
            }

            ViewBag.IsLoggedIn = reg;

            if (reg && paymentProvider == "MIPay")
            {
                return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, VerifiedTopUps, new CustomerDetailsViewModel(FullName, CountryList, AddressModel, Payload.TwoLetterISORegionName.Equals("GB")), MiPayCustomer.payload, null, paymentProvider));
            }
            else if (reg && paymentProvider == "Pay360")
            {
                return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, VerifiedTopUps, new CustomerDetailsViewModel(FullName, CountryList, AddressModel, Payload.TwoLetterISORegionName.Equals("GB")), null, Pay360Customer.payload, paymentProvider));
            }
            else
            {
                return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, VerifiedTopUps, new CustomerDetailsViewModel(FullName, CountryList, AddressModel, Payload.TwoLetterISORegionName.Equals("GB")), null, null, paymentProvider));
            }

            //}



        }

        /// <summary>
        /// GET /top-up/confirmation
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public async Task<ActionResult> TopUpFailure(RenderModel model)
        {
            string Error;
            var Payload = GetPayload();

            if (!PaymentService.TryFindTransaction(Payload, out Error))
            {
                return ErrorRedirect(Error, Urls.TopUp);
            }

            var Result = await PaymentService.PaymentRetrieve(Payload.Payment);

            if (Result == null)
            {
                return RetrievePaymentFailed(LoggerService);
            }

            if (!PaymentService.TryTransactionSuccess(Result, out Error))
            {
                return ErrorRedirect(Error, Urls.Checkout);
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /top-up/confirmation
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public async Task<ActionResult> TopUpConfirmation(RenderModel model)
        {
            string Error;
            var Payload = GetPayload();
            var paymentProvider = ConfigurationManager.AppSettings["PaymentProvider"];

            if (paymentProvider != "Pay360")
            {
                if (!PaymentService.TryFindTransaction(Payload, out Error))
                {
                    return ErrorRedirect(Error, Urls.TopUp);
                }

                var Result = await PaymentService.PaymentRetrieve(Payload.Payment);

                if (Result == null)
                {
                    return RetrievePaymentFailed(LoggerService);
                }

                if (Result.payload != null)
                {
                    TempData["GoogleAmount"] = Result.payload.authAmount;
                }

                if (!PaymentService.TryTransactionSuccess(Result, out Error))
                {
                    return ErrorRedirect(Error, Urls.Checkout);
                }
            }

            if (TempData["IsInternationalTopUp"] != null && System.Convert.ToBoolean(TempData["IsInternationalTopUp"]) == true)
            {
                ViewBag.IsInternationalTopUp = true;
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /purchase/confirmation
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult PurchaseConfirmation(RenderModel model)
        {
            var Payload = GetPayload();

            if (Payload.Checkout != null)
            {
                TempData["GoogleAmount"] = Payload.Checkout.Total;
            }

            if (Payload.Basket != null)
            {
                Payload.Basket.Clear();
                SetPayload(Payload);
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /one-click/confirmation
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public async Task<ActionResult> OneClickCheckoutConfirmation(RenderModel model)
        {
            string Error;
            var Payload = GetPayload();


            if (Payload.Checkout != null)
            {
                TempData["GoogleAmount"] = Payload.Checkout.Total;
            }

            if (!PaymentService.TryFindOneClick(Payload, out Error))
            {
                return ErrorRedirect(Error, Urls.Checkout);
            }

            var Result = await PaymentService.OneClickRetrieve(Payload.OneClick);

            if (Result == null)
            {
                return RetrievePaymentFailed(LoggerService);
            }

            if (!PaymentService.TryOneClickSuccess(Result, out Error))
            {
                return ErrorRedirect(Error, Urls.Checkout);
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /sign-up/success
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult SuccessfulSignUp(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /register-product
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult RegisterProduct(RenderModel model)
        {
            var Payload = GetPayload();
            Payload.TwoLetterISORegionName = "GB";
            Payload.OpenRegistration = "THM";
            SetPayload(Payload);
            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /product-details
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [ApiAuthentication(ReturnUrl = "/confirm-product-details")]
        public ActionResult ConfirmProductDetails(RenderModel model)
        {
            var Payload = GetPayload();
            Payload.OpenRegistration = "THM";

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /promo
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult Promo(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /forgotten-password
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult ForgottenPassword(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }
        /// <summary>
        /// GET /
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult CreateNewPassword(RenderModel model)
        {
            var Payload = GetPayload();

            if (!Payload.VerifiedReset)
            {
                return ErrorRedirect(((int)Messages.InvalidResetToken).ToString(), Urls.ForgottenPassword);
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /air-time-transfer
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public ActionResult AirTimeTransfer(RenderModel model)
        {
            return RedirectToActionPermanent("Internationaltopup");
            var Payload = GetPayload();

            if (AccountService.IsAuthorized(Payload))
            {
                TempData["LoggedOn"] = true;
            }
            else
            {
                TempData["LoggedOn"] = false;
            }

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }

        /// <summary>
        /// GET /air-time-transfer
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        [MinifyHtml]
        public async Task<ActionResult> TransferCheckout(RenderModel model)
        {

            string CustId;
            var AddressModel = new AddressModel();
            var FullName = new FullNameModel();
            var Payload = GetPayload();
            Payload.Checkout.Verify = "THATT";

            var TopUps = ContentService.GetAllTopUps();
            var VerifiedTopUps = TopUps[Payload.Checkout.Verify];

            if (AccountService.IsAuthorized(Payload))
            {
                var UserAccount = await AccountService.GetAccountDetails(Payload.ApiToken);
                AccountService.TryGetBillingAddress(UserAccount, out AddressModel);
                FullName = Mapper.Map<FullNameModel>(UserAccount.payload);
            }
            var MiPayCustomer = await PaymentService.GetCustomer(new GetCustomerRequestModel(ChannelType.Web, Payload.Checkout.Reference, UniqueIDType.Msisdn));
            if (MiPayCustomer == null)
            {
                return GetMiPayCustomerFailed(LoggerService);
            }

            if (PaymentService.TryGetCustId(MiPayCustomer, out CustId))
            {
                AddressModel = Mapper.Map<AddressModel>(MiPayCustomer.payload.billingAddress);
                FullName = Mapper.Map<FullNameModel>(MiPayCustomer.payload);
            }
            var CountryList = AccountService.GetCountryList();

            Payload.CustId = CustId;
            Payload.CheckoutProduct = "THATT";
            SetPayload(Payload);
            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload, VerifiedTopUps, new CustomerDetailsViewModel(FullName, CountryList, AddressModel, Payload.TwoLetterISORegionName.Equals("GB")), MiPayCustomer.payload, null, "MIPay"));
        }


        /// <summary>
        /// GET /calling-cards-benefits
        /// </summary>
        /// <param name="model">The Umbraco model</param>
        /// <returns>The view</returns>
        public ActionResult CallingCardsBenefits(RenderModel model)
        {
            var Payload = GetPayload();

            return View(new CustomPageViewModel<CustomPage>(model.Content, Payload));
        }


        [Route("international-topup")]
        public ActionResult Internationaltopup()
        {

            var Payload = GetPayload();

            if (AccountService.IsAuthorized(Payload))
            {
                TempData["LoggedOn"] = true;
            }
            else
            {
                TempData["LoggedOn"] = false;
            }

            return View("Internationaltopup", Payload);
        }


        [Route("register-compition")]
        public ActionResult RegisterCompition(AddCompitionUserRequestModel model)
        {
            var result = AccountService.AddCompitionUser(model);
            return Json(new { status = (result == 1) ? true : false });
        }


        [Route("isregistered")]
        [HttpPost]
        public ActionResult IsRegistered(string Email, string Promoname)
        {
            var result = AccountService.IsRegistered(Email, Promoname);
            return Json(new { status = (result == 1) ? true : false });
        }

        [Route("creditsimsuccess")]
        public ActionResult CreditSimSuccess()
        {

            //TempData["IssignedUp"] = (TempData["IssignedUp"] != null) ? TempData["IssignedUp"].ToString() : "false";

            TempData["IssignedUp"] = "false";

            long OrderId = TempData["OrderId"] != null ? Convert.ToInt64(TempData["OrderId"]) : 0;

            var gclidcookie = Request.Cookies["gclid"] != null ? Request.Cookies["gclid"].Value : null;
            if (gclidcookie != null)
                AccountService.SaveGCLID_CreditSim(gclidcookie, OrderId);
            return View();
        }


    }
}
