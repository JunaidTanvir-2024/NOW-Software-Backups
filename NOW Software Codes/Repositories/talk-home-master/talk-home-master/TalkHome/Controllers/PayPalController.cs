using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.App_LocalResources;
using TalkHome.ErrorLog;
using TalkHome.Extensions.Html;
using TalkHome.Filters;
using TalkHome.Interfaces;
using TalkHome.Models;
using TalkHome.Models.Enums;
using TalkHome.Models.PayPal;
using TalkHome.Models.ViewModels.Payment;
using TalkHome.Models.ViewModels.PayPal;
using TalkHome.Models.WebApi;
using TalkHome.WebServices;
using TalkHome.WebServices.Interfaces;

namespace TalkHome.Controllers
{
    [GCLIDFilter]
    public class PayPalController : BaseController
    {
        private readonly IPayPalService PayPalService;
        private Properties.URLs Urls = Properties.URLs.Default;
        private readonly IAccountService AccountService;
        private readonly ITalkHomeWebService TalkHomeWebService;

        public PayPalController(IPayPalService payPalService, IAccountService accountService, ITalkHomeWebService talkHomeWebService)
        {
            PayPalService = payPalService;
            AccountService = accountService;
            TalkHomeWebService = talkHomeWebService;
        }

        // GET: PayPal
        [HttpPost]
        [ActionName("payPalStartPayment")]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> PayPalStartPayment(StartPaymentViewModel model)
        {
            var payload = GetPayload();
            string uRef = "";
            string customerName = "";

            if (AccountService.IsAuthorized(payload))
            {
                uRef = payload.FullName.Email;
                customerName = payload.FullName.FirstName;
            }
            else if (AccountService.IsAuthorized(payload) && payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && payload.Checkout.ProductType.Equals(Models.Enums.ProductType.Bundle.ToString()) && payload.isTHCCPin.Equals(true))
            {
                uRef = model.EmailAddress;
                customerName = "Guest";
            }
            else if (AccountService.IsAuthorized(payload) && payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && payload.isTHCCPin.Equals(false))
            {
                uRef = payload.FullName.Email;
                customerName = "Guest";
            }
            else
            {
                uRef = payload.Checkout.Reference;
                customerName = "Guest";
            }
            if (payload.Checkout.ProductType == ProductType.TopUp.ToString())
            {
                payload.Checkout.Total = model.Amount;
            }

            string purchaseUrl = "payPal/purchaseSuccessReturn?uRef=" + uRef + "&ProductCode=" + payload.Checkout.Verify;
            string topUpUrl = "payPal/topUpSuccessReturn?uRef=" + uRef + "&ProductCode=" + payload.Checkout.Verify;
            string creditsimUrl = "payPal/creditsimSuccessReturn?uRef=" + uRef + "&ProductCode=" + payload.Checkout.Verify;
            string returnUrl = "";

            if (TempData["IsInternationalTopUp"] != null && System.Convert.ToBoolean(TempData["IsInternationalTopUp"]) == true)
            {
                TempData["IsInternationalTopUp"] = true;
            }

            if (payload.Purchase.Count > 0)
            {
                returnUrl = purchaseUrl;
            }
            else if (payload.TopUp.Count > 0)
            {
                returnUrl = topUpUrl;
            }
            else if (payload.Checkout.Basket.Count > 0)
            {
                returnUrl = creditsimUrl;
                
            }
            else
            {
                returnUrl = purchaseUrl;
            }
     
            PayPalCreateSalePaymentRequest request = new PayPalCreateSalePaymentRequest
            {
                CustomerName = customerName,
                CustomerEmail = !string.IsNullOrEmpty(model.EmailAddress) ? model.EmailAddress : null,
                RedirectUrl = new RedirectUrls
                {
                    ReturnUrl = PayPalService.GetResumeUrl(returnUrl),
                    CancelUrl = PayPalService.GetResumeUrl("payPal/cancelReturn")
                }
            };
            bool Iscreditsim = false;
            if (model.CreditSim != null && model.CreditSim.ProductType == "CreditSimOrder")
            {
                payload.CreditSim = new CreditSimPayload { };
                Iscreditsim = true;
                payload.CreditSim.userId = model.CreditSim.userId;
                payload.CreditSim.orderId = model.CreditSim.orderId;
                payload.CreditSim.Email = model.CreditSim.Email;
                payload.CreditSim.signedUp = model.CreditSim.signedUp;
                payload.CreditSim.Name = model.CreditSim.Name;
                payload.CreditSim.Gclid = model.CreditSim.Gclid;
            }
            var response = await PayPalService.PayPalCreateSalePayment(request, payload, Iscreditsim);
            if (response == null)
            {
                if (payload.CreditSim != null)
                {
                    payload.CreditSim = null;
                }
                SetPayload(payload);

                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = payload.Checkout.Verify;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            if (response.errorCode > 0)
            {
                if (payload.CreditSim != null)
                {
                    payload.CreditSim = null;
                }
                SetPayload(payload);
                //if (response.errorCode == 2)
                //{
                //    TempData["ErrorCode"] = 999;
                //    ViewBag.Msg = "You have reached the maximum number of bundles for your subscription.";
                //    ViewBag.ErrorCode = response.errorCode;
                //    ViewBag.ProductCode = payload.Checkout.Verify;
                //}
                //else
                //{
                TempData["ErrorCode"] = 999;
                ViewBag.Msg = response.message;
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = payload.Checkout.Verify;
                //}

                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            return Redirect(response.payload.RedirectUrl);
        }

        [HttpGet]
        [ActionName("purchaseSuccessReturn")]
        public async Task<ActionResult> PurchaseSuccessReturn(SuccessReturnPayPalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Account/Checkout");
            }
            PayPalExecuteSalePaymentRequest PaymentRequest = new PayPalExecuteSalePaymentRequest
            {
                PayerId = model.PayerID,
                PaymentId = model.paymentId,
                CustomerUniqueRef = model.uRef,
                ProductCode = model.ProductCode
            };

            var response = await PayPalService.PayPalExecuteSalePayment(PaymentRequest);
            if (response == null)
            {
                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (response.errorCode > 0)
            {
                TempData["ErrorCode"] = 999;
                ViewBag.Msg = response.message;
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            return Redirect(Urls.SuccessfulPurchase);
        }

        [HttpGet]
        [ActionName("topUpSuccessReturn")]
        public async Task<ActionResult> TopUpSuccessReturn(SuccessReturnPayPalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Account/Checkout");
            }
            PayPalExecuteSalePaymentRequest PaymentRequest = new PayPalExecuteSalePaymentRequest
            {
                PayerId = model.PayerID,
                PaymentId = model.paymentId,
                CustomerUniqueRef = model.uRef,
                ProductCode = model.ProductCode
            };

            var response = await PayPalService.PayPalExecuteSalePayment(PaymentRequest);
            if (response == null)
            {
                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (response.errorCode > 0)
            {
                TempData["ErrorCode"] = 999;
                ViewBag.Msg = response.message;
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            if (TempData["IsInternationalTopUp"] != null && System.Convert.ToBoolean(TempData["IsInternationalTopUp"]) == true)
            {
                ViewBag.IsInternationalTopUp = true;
            }
            return Redirect(Urls.SuccessfulTopUp);
        }


        [HttpGet]
        [ActionName("creditsimSuccessReturn")]
        public async Task<ActionResult> creditsimSuccessReturn(SuccessReturnPayPalViewModel model)
        {
            var Payload = GetPayload();
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Account/Checkout");
            }
            PayPalExecuteSalePaymentRequest PaymentRequest = new PayPalExecuteSalePaymentRequest
            {
                PayerId = model.PayerID,
                PaymentId = model.paymentId,
                CustomerUniqueRef = (AccountService.IsAuthorized(Payload)) ? Payload.FullName.Email : (Payload.CreditSim != null && Payload.CreditSim.Email != null) ? Payload.CreditSim.Email : String.Empty,
                ProductCode = model.ProductCode
            };

            var response = await PayPalService.PayPalExecuteSalePayment(PaymentRequest);
            if (response == null)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }
                SetPayload(Payload);

                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (response.errorCode > 0)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }
                SetPayload(Payload);
                TempData["ErrorCode"] = 999;
                ViewBag.Msg = response.message;
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }


            CreditSimPaymentApiRequest creditsimrequest = new CreditSimPaymentApiRequest
            {
                userId = Payload.CreditSim.userId,
                orderId = Payload.CreditSim.orderId,
                paymentType = 2,
                paymentTransactionId = response.payload.PaypalTransactionId,
                paymentErrorCode = response.errorCode,
                paymentDate = DateTime.Now,
                paymentErrorMessage = response.message,
            };

            var creditsimresponse = await TalkHomeWebService.CreditSimPayment(creditsimrequest);

            if (creditsimresponse == null)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }
                SetPayload(Payload);


                ViewBag.Msg = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimPaymentError).ToString()));
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            if (creditsimresponse.errorCode != 0)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }
                if (creditsimresponse.errorCode == 4)
                {
                    ViewBag.Msg = creditsimresponse.message;
                }
                else
                {
                    ViewBag.Msg = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimPaymentError).ToString()));
                }

                SetPayload(Payload);
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            TempData["IssignedUp"] = Payload.CreditSim.signedUp;
            await MailExtentions.CreditSimSuccessMailAsync(Payload);

            if (Payload.CreditSim != null)
            {
                TempData["OrderId"] = Payload.CreditSim.orderId != 0 ? Payload.CreditSim.orderId : 0;
                Payload.CreditSim = null;
            }
            SetPayload(Payload);
            return RedirectToAction("CreditSimSuccess", "CustomPage");
        }


        [HttpGet]
        [ActionName("cancelReturn")]
        public async Task<ActionResult> CancelReturn(CancelReturnPayPalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Account/Checkout");
            }
            ViewBag.Msg = "PayPal Payment Cancelled";
            ViewBag.ErrorCode = 2;
            return View("~/Views/Shared/ErrorPay360.cshtml");
        }


        [HttpPost]
        [ActionName("pay369PayPalStartPayment")]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> Pay360PayPalStartPayment(StartPay360PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Checkout");
            }
            var payload = GetPayload();
            string uRef = "";
            string customerName = "";

            if (AccountService.IsAuthorized(payload))
            {
                uRef = payload.FullName.Email;
                customerName = payload.FullName.FirstName;
            }

            else
            {
                uRef = payload.Checkout.Reference;
                customerName = "Guest";
            }
            if (payload.Checkout.ProductType == ProductType.TopUp.ToString())
            {
                payload.Checkout.Total = model.Amount;
            }
            model.customerName = customerName;

            string purchaseUrl = "payPal/pay360purchaseSuccessReturn?uRef=" + uRef + "&ProductCode=" + payload.Checkout.Verify;

            string topUpUrl = "payPal/pay360topUpSuccessReturn?uRef=" + uRef + "&ProductCode=" + payload.Checkout.Verify;


            string creditsimUrl = "payPal/pay360creditsimSuccessReturn?uRef=" + uRef + "&ProductCode=" + payload.Checkout.Verify;

            string returnUrl = "";
            if (payload.Purchase.Count > 0)
            {
                returnUrl = purchaseUrl;
            }
            else if (payload.TopUp.Count > 0)
            {
                returnUrl = topUpUrl;
            }
            else if (payload.Checkout.Basket.Count > 0)
            {
                returnUrl = creditsimUrl;
            }
            else
            {
                returnUrl = purchaseUrl;
            }
            model.returnUrl = PayPalService.GetResumeUrl(returnUrl);
            model.cancelUrl = PayPalService.GetResumeUrl("payPal/cancelReturn");


            bool Iscreditsim = false;
            if (model.CreditSim != null && model.CreditSim.ProductType == "CreditSimOrder")
            {
                payload.CreditSim = new CreditSimPayload { };
                Iscreditsim = true;
                payload.CreditSim.userId = model.CreditSim.userId;
                payload.CreditSim.orderId = model.CreditSim.orderId;
                payload.CreditSim.Email = model.CreditSim.Email;
                payload.CreditSim.signedUp = model.CreditSim.signedUp;
                payload.CreditSim.Name = model.CreditSim.Name;
                payload.CreditSim.orderId = model.CreditSim.orderId;
            }

            var response = await PayPalService.Pay360PayPalCreateSalePayment(model, payload, GetRequestIP(), Iscreditsim);
            if (response == null)
            {
                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = payload.Checkout.Verify;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            if (response.errorCode > 0)
            {

                if (response.errorCode == 2)
                {
                    TempData["ErrorCode"] = 999;
                    ViewBag.Msg = response.message;
                    ViewBag.ErrorCode = response.errorCode;
                    ViewBag.ProductCode = payload.Checkout.Verify;
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }
                else if (response.errorCode == 3)   // exceeded daily limit
                {
                    TempData["ErrorCode"] = 999;
                    ViewBag.Msg = response.message;
                    ViewBag.ErrorCode = response.errorCode;
                    ViewBag.ProductCode = payload.Checkout.Verify;
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }
                else
                {
                    TempData["ErrorCode"] = response.errorCode;
                    ViewBag.Msg = response.message;
                    ViewBag.ErrorCode = response.errorCode;
                    ViewBag.ProductCode = payload.Checkout.Verify;
                }
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            return Redirect(response.payload.clientRedirectUrl);
        }

        [HttpGet]
        [ActionName("pay360topUpSuccessReturn")]
        public async Task<ActionResult> Pay360TopUpSuccessReturn(SuccessReturnPayPalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Checkout");
            }
            Pay360PayPalResumePaymentRequest request = new Pay360PayPalResumePaymentRequest
            {
                PaypalCheckoutToken = model.token
            };
            var response = await PayPalService.Pay360ResumePayment(request);

            if (response == null)
            {
                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (response.errorCode > 0)
            {
                TempData["ErrorCode"] = 999;
                ViewBag.Msg = response.message;
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            if (TempData["IsInternationalTopUp"] != null && System.Convert.ToBoolean(TempData["IsInternationalTopUp"]) == true)
            {
                ViewBag.IsInternationalTopUp = true;
            }
            return Redirect(Urls.SuccessfulTopUp);
        }

        [HttpGet]
        [ActionName("pay360purchaseSuccessReturn")]
        public async Task<ActionResult> Pay360PurchaseSuccessReturn(SuccessReturnPayPalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Checkout");
            }
            Pay360PayPalResumePaymentRequest request = new Pay360PayPalResumePaymentRequest
            {
                PaypalCheckoutToken = model.token
            };
            var response = await PayPalService.Pay360ResumePayment(request);

            if (response == null)
            {
                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (response.errorCode > 0)
            {
                TempData["ErrorCode"] = 999;
                ViewBag.Msg = response.message;
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            return Redirect(Urls.SuccessfulPurchase);
        }



        [HttpGet]
        [ActionName("pay360creditsimSuccessReturn")]
        public async Task<ActionResult> pay360CreditsimSuccessReturn(SuccessReturnPayPalViewModel model)
        {
            var Payload = GetPayload();
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Checkout);
            }


            Pay360PayPalResumePaymentRequest request = new Pay360PayPalResumePaymentRequest
            {
                PaypalCheckoutToken = model.token
            };

            var response = await PayPalService.Pay360ResumePayment(request);

            if (response == null)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }
                SetPayload(Payload);

                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (response.errorCode > 0)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }
                SetPayload(Payload);
                TempData["ErrorCode"] = 999;
                ViewBag.Msg = response.message;
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }


            CreditSimPaymentApiRequest creditsimrequest = new CreditSimPaymentApiRequest
            {
                userId = Payload.CreditSim.userId,
                orderId = Payload.CreditSim.orderId,
                paymentType = 2,
                paymentTransactionId = response.payload.transactionId,
                paymentErrorCode = response.errorCode,
                paymentDate = DateTime.Now,
                paymentErrorMessage = response.message,
            };

            var creditsimresponse = await TalkHomeWebService.CreditSimPayment(creditsimrequest);

            if (creditsimresponse == null)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }
                SetPayload(Payload);


                ViewBag.Msg = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimPaymentError).ToString()));
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            if (creditsimresponse.errorCode != 0)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }
                if (creditsimresponse.errorCode == 4)
                {
                    ViewBag.Msg = creditsimresponse.message;
                }
                else
                {
                    ViewBag.Msg = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimPaymentError).ToString()));
                }

                SetPayload(Payload);
                ViewBag.ErrorCode = response.errorCode;
                ViewBag.ProductCode = model.ProductCode;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            TempData["IssignedUp"] = Payload.CreditSim.signedUp;
            await MailExtentions.CreditSimSuccessMailAsync(Payload);

            if (Payload.CreditSim != null)
            {
                TempData["OrderId"] = Payload.CreditSim.orderId != 0 ? Payload.CreditSim.orderId : 0;
                Payload.CreditSim = null;
            }
            SetPayload(Payload);
            return RedirectToAction("CreditSimSuccess", "CustomPage");
        }

    }
}