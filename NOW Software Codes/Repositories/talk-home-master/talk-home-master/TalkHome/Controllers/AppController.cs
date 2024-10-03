using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.Interfaces;
using TalkHome.Models.ViewModels.App;
using TalkHome.Models.WebApi.Payment;
using System.Linq;
using TalkHome.Models.ViewModels.Payment;
using TalkHome.Models.App;
using TalkHome.Models.Enums;
using TalkHome.Models;
using AutoMapper;
using TalkHome.Models.ViewModels;
using System;
using TalkHome.Logger;
using TalkHome.ErrorLog;
using TalkHome.Filters;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Manages HTTP requests for in-app purchase pages.
    /// Allows for a single product checkout, either a bundle or a top up
    /// </summary>
    [GCLIDFilter]
    public class AppController : BaseController
    {
        private readonly IAccountService AccountService;
        private readonly IContentService ContentService;
        private readonly IPaymentService PaymentService;
        private readonly ILoggerService LoggerService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public AppController(IContentService contentService, IPaymentService paymentService, IAccountService accountService, ILoggerService loggerService)
        {
            ContentService = contentService;
            PaymentService = paymentService;
            AccountService = accountService;
            LoggerService = loggerService;
        }

        /// <summary>
        /// Overrrides default action. None are allowed, shows the error page
        /// </summary>
        /// <returns>The view</returns>
        public ActionResult Index()
        {
            return RedirectToAction("failure", new { message = "19" });
        }

        /// <summary>
        /// Responds to GET /app/topup. 
        /// Ensures the Msisdn is provided with the request. Tries to authenticate the App user and gets the top up products.
        /// Finally, updates the customer cookie and returns the amount selection page.
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view or the error page</returns>
        public async Task<ActionResult> TopUp(TopUpRequestModel model)
        {
            AccountService.SetCultureOnCurrentThread(Request.UserLanguages[0]);

            if (!ModelState.IsValid)
                return CorruptedAppTopUpUrl(LoggerService);

            var AppUser = await AccountService.GetAppUserByMsisdn(model.Msisdn);

            if (AppUser == null || AppUser.errorCode != 0)
                return RedirectToAction("failure", new { message = "0" });

            var TopUps = ContentService.GetAppTopUps();

            var Payload = GetPayload();
            Payload.FullName.FirstName = AppUser.payload.fname;
            Payload.TwoLetterISORegionName = AppUser.payload.countryCode;
            Payload.Checkout.Reference = model.Msisdn;
            Payload.Created = DateTime.Now;
            SetPayload(Payload);
            return View(new AppTopUpViewModel(Payload, AppUser.payload, TopUps));
        }

        /// <summary>
        /// Responds to GET /app/add-bundle. 
        /// Allows for an in-app bundle purchase request. Loads the checkout form.
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view or the error page</returns>
        [ActionName("add-bundle")]
        public async Task<ActionResult> AddBundle(AddBundleRequestModel model)
        {
            if (!ModelState.IsValid)
                return CorruptedAddBundleUrl(LoggerService);

            var Bundle = ContentService.GetAppBundleByGuid(model.Guid);

            if (Bundle == null)
                return AppBundleNotFound(LoggerService);

            var AppUser = await AccountService.GetAppUserByMsisdn(model.Msisdn);

            if (AppUser == null || AppUser.errorCode != 0)
                return RedirectToAction("failure", new { message = "0" });

            string CustId;
            var Payload = GetPayload();

            var FullName = Mapper.Map<FullNameModel>(AppUser.payload);
            var AddressModel = Mapper.Map<AddressModel>(AppUser.payload);

            var MiPayCustomer = await PaymentService.GetCustomer(new GetCustomerRequestModel(ChannelType.App, model.Msisdn, UniqueIDType.Msisdn));

            if (MiPayCustomer == null)
                return GetMiPayCustomerFailed(LoggerService);

            if (MiPayCustomer == null)
                return GetMiPayCustomerFailed(LoggerService);

            if (PaymentService.TryGetCustId(MiPayCustomer, out CustId))
            {
                FullName = Mapper.Map<FullNameModel>(MiPayCustomer.payload);
                AddressModel = Mapper.Map<AddressModel>(MiPayCustomer.payload.billingAddress);
            }

            var Countries = AccountService.GetCountryList();

            Payload.Purchase.Clear();
            Payload.Purchase.Add(Bundle.Id); // Users don't manually set the basket product. Clear basket and add product Id before checkout.
            Payload.FullName.FirstName = AppUser.payload.fname;
            Payload.TwoLetterISORegionName = AppUser.payload.countryCode;
            Payload.Checkout.Reference = model.Msisdn;
            Payload.Created = DateTime.Now;
            SetPayload(Payload);
            return View(new AppCheckoutViewModel(Payload, Bundle, MiPayCustomer.payload, new CustomerDetailsViewModel(FullName, Countries, AddressModel, Payload.TwoLetterISORegionName.Equals("GB"))));
        }

        /// <summary>
        /// Responds to POST /app/checkout. 
        /// Verifies the Checkout request, gets App and MiPay users and returns the checkout form.
        /// </summary>
        /// <returns>The view or the error page</returns>
        public async Task<ActionResult> Checkout()
        {
            AccountService.SetCultureOnCurrentThread(Request.UserLanguages[0]);

            string CustId;
            var AddressModel = new AddressModel();
            var FullName = new FullNameModel();
            var Payload = GetPayload();

            if (string.IsNullOrEmpty(Payload.Checkout.Reference))
                return AppUserNotVerifiedAtCheckout(LoggerService);

            var AppUser = await AccountService.GetAppUserByMsisdn(Payload.Checkout.Reference);

            if (AppUser == null || AppUser.errorCode != 0)
                return RedirectToAction("failure", new { message = "0" });

            FullName = Mapper.Map<FullNameModel>(AppUser.payload);
            AddressModel = Mapper.Map<AddressModel>(AppUser.payload);

            var MiPayCustomer = await PaymentService.GetCustomer(new GetCustomerRequestModel(ChannelType.App, Payload.Checkout.Reference, UniqueIDType.Msisdn));

            if (MiPayCustomer == null)
                return GetMiPayCustomerFailed(LoggerService);

            if (PaymentService.TryGetCustId(MiPayCustomer, out CustId))
            {
                FullName = Mapper.Map<FullNameModel>(MiPayCustomer.payload);
                AddressModel = Mapper.Map<AddressModel>(MiPayCustomer.payload.billingAddress);
            }

            var Product = ContentService.GetProducts(Payload.TopUp.First());

            var Countries = AccountService.GetCountryList();

            Payload.CustId = CustId;
            SetPayload(Payload);
            return View(new AppCheckoutViewModel(Payload, Product, MiPayCustomer.payload, new CustomerDetailsViewModel(FullName, Countries, AddressModel, Payload.TwoLetterISORegionName.Equals("GB"))));
        }

        /// <summary>
        /// Responds to POST /app/start-payment.
        /// Verifies the validity of the user-submitted payment request data, formats the payment object and sends the request to the Payment service.
        /// If successful, the customer is redirected to MiPay Url for the payment.
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("start-payment")]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> StartPayment(StartPaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.AppCheckout);

            string Error = "";
            var Payload = GetPayload();

            if (string.IsNullOrEmpty(Payload.Checkout.Reference))
                return AppUserNotVerifiedAtCheckout(LoggerService);

            if (!Payload.TopUp.Any())
                return EmptyAppBasket(LoggerService);

            var MiPayCustomer = await PaymentService.GetCustomer(new GetCustomerRequestModel(ChannelType.App, Payload.Checkout.Reference, UniqueIDType.Msisdn));

            if (MiPayCustomer == null)
                return GetMiPayCustomerFailed(LoggerService);

            if (PaymentService.IsOneClickElegible(Payload, MiPayCustomer.payload, model))
            {
                var OneClick = await PaymentService.TryOneClickTopUp(model, Payload, ChannelType.App, Error); // One-click checkout

                if (OneClick == null)
                    return RedirectToAction("failure", new { message = Error });

                Payload.OneClick = OneClick;
                Response.Cookies.Add(AccountService.EncodeCookie(Payload));
                return RedirectToAction("OneClickComplete");
            }

            var Request = PaymentService.CreatePaymentRequest(Payload, model, ChannelType.App);

            var StartPayment = await PaymentService.StartPayment(Request);

            if (StartPayment == null)
                return StartPaymentFailed(LoggerService);

            if (!PaymentService.TryPaymentSuccess(StartPayment, out Error))
                return HandleRedirect(Error, Urls.AppCheckout);

            Payload.Payment = new PaymentRetrieveRequest(StartPayment.payload.token, StartPayment.payload.clientReference, Request.PaymentType, model.PaymentMethod, Request.ChannelType);
            SetPayload(Payload);
            return Redirect(StartPayment.payload.paymentURL);
        }

        /// <summary>
        /// Requests information about a transaction
        /// </summary>
        /// <returns>The view</returns>
        public async Task<ActionResult> Complete()
        {
            string Error;
            var Payload = GetPayload();

            if (!PaymentService.TryFindTransaction(Payload, out Error))
                return RedirectToAction("failure", new { message = Error });

            var Result = await PaymentService.PaymentRetrieve(Payload.Payment);

            if (Result == null)
                return RetrievePaymentFailed(LoggerService);

            if (!PaymentService.TryTransactionSuccess(Result, out Error))
                return HandleRedirect(Error, Urls.AppCheckout);

            return RedirectToAction("success", new { message = "10" });
        }

        /// <summary>
        /// Requests information about a One-click checkout
        /// </summary>
        /// <returns>The view</returns>
        public async Task<ActionResult> OneClickComplete()
        {
            string Error;
            var Payload = GetPayload();

            if (!PaymentService.TryFindOneClick(Payload, out Error))
                return RedirectToAction("failure", new { message = Error });

            var Result = await PaymentService.OneClickRetrieve(Payload.OneClick);

            if (Result == null)
                return RetrievePaymentFailed(LoggerService);

            if (!PaymentService.TryOneClickSuccess(Result, out Error))
                return RedirectToAction("failure", new { message = Error });

            return RedirectToAction("success", new { message = "10" });
        }

        /// <summary>
        /// Returns the app error page. Given an error code displays the localised message
        /// </summary>
        /// <returns>The view</returns>
        public ActionResult Failure(string message)
        {
            return View(new AppMessageViewModel(message));
        }

        /// <summary>
        /// Returns the app success page. Given a message code, displays the localised message
        /// </summary>
        /// <returns>The view</returns>
        public ActionResult Success(string message)
        {
            return View(new AppMessageViewModel(message));
        }
    }
}
