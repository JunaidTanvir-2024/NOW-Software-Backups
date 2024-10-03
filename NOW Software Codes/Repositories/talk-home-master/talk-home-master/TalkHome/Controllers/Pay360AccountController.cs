using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.App_LocalResources;
using TalkHome.ErrorLog;
using TalkHome.Extensions.Html;
using TalkHome.Filters;
using TalkHome.Interfaces;
using TalkHome.Logger;
using TalkHome.Models;
using TalkHome.Models.Enums;
using TalkHome.Models.Pay360;
using TalkHome.Models.ViewModels;
using TalkHome.Models.ViewModels.DTOs;
using TalkHome.Models.ViewModels.Pay360;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.WebServices.Interfaces;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Manages HTTP request/responses for My Account pages that are not related to customer history
    /// </summary>
    [GCLIDFilter]
    public class Pay360AccountController : BaseController
    {
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly IAccountService AccountService;
        private readonly ILoggerService LoggerService;
        private readonly IPay360Service Pay360Service;
        private readonly IContentService ContentService;
        private readonly IActiveCampaignService ActiveCampaignService;
        private readonly IAirTimeTransferService AirTimeTransferService;
        private readonly IPaymentService PaymentService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public Pay360AccountController(IPaymentService paymentService, IAccountService accountService, ITalkHomeWebService talkHomeWebService, IContentService contentService, ILoggerService loggerService, IActiveCampaignService activeCampaignService, IAirTimeTransferService airtimetransferService, IPay360Service pay360Service)
        {
            TalkHomeWebService = talkHomeWebService;
            AccountService = accountService;
            ContentService = contentService;
            LoggerService = loggerService;
            ActiveCampaignService = activeCampaignService;
            AirTimeTransferService = airtimetransferService;
            Pay360Service = pay360Service;
            PaymentService = paymentService;
        }

        [ActionName("securereturn")]
        public async Task<ActionResult> SecureReturn(Secure3DViewModel model)
        {

            if (!ModelState.IsValid)
            {

                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Checkout);
            }

            Pay360Resume3DRequest request = new Pay360Resume3DRequest
            {
                pareq = model.PaRes,
                pay360TransactionId = model.MD
            };

            var response = await Pay360Service.Resume3DTransaction(request);

            if (response == null)
            {

                return ErrorRedirect(((int)Messages.RejectionWithError).ToString(), Urls.Checkout);
            }

            if (response.errorCode > 0)
            {

                TempData["ErrorCode"] = response.errorCode;
                return ErrorRedirect(((int)Messages.RejectionWithError).ToString(), Urls.Checkout);
            }

            var payload = response.payload;

            Pay360TransactionViewModel pvm = new Pay360TransactionViewModel
            {

                Amount = payload.transactionAmount,
                TransactionId = payload.transactionId,
                isSuccess = payload.outcome.status == "SUCCESS" ? true : false,
                Reason = payload.outcome.reasonMessage
            };
            try
            {
                var Paylaod = GetPayload();
                if (AccountService.IsAuthorized(Paylaod) &&
                   Paylaod.ProductCodes != null
                  && Paylaod.ProductCodes.Count > 0
                  && Paylaod.ProductCodes.Where(x => x.ProductCode == "THM").First().Reference.Trim() == Paylaod.Checkout.Reference.Trim())
                {
                    if (TempData["AutoBundleRenewal"] != null && ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
                    {
                        try
                        {
                            var autoRenewModel = (AutoRenewalsSettingsRequestDTO)TempData["AutoBundleRenewal"];
                            var ResponseDTO = await TalkHomeWebService.AutoRenewSettings(autoRenewModel, Paylaod.ApiToken);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception EXX)
            {

            }

            return Redirect(Urls.SuccessfulPurchase);
            //return View(view, pvm);
        }

        [ActionName("securereturntopup")]
        public async Task<ActionResult> securereturntopup(Secure3DViewModel model)
        {
            var Paylaod = GetPayload();

            if (!ModelState.IsValid)
            {
                //redirect back with validation errors
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Checkout);
            }

            Pay360Resume3DRequest request = new Pay360Resume3DRequest
            {
                pareq = model.PaRes,
                pay360TransactionId = model.MD
            };

            var response = await Pay360Service.Resume3DTransaction(request);

            if (response == null)
            {
                //log.Error(String.Format("Resume3DTransaction Request failed"));
                return ErrorRedirect(((int)Messages.RejectionWithError).ToString(), Urls.Checkout);
            }

            if (response.errorCode > 0)
            {
                //log.Error(String.Format("Resume3DTransaction Request returned Error: {0}", response.errorCode.ToString()));
                TempData["ErrorCode"] = response.errorCode;
                return ErrorRedirect(((int)Messages.RejectionWithError).ToString(), Urls.Checkout);
            }

            //var payload = response.payload;

            //Pay360TransactionViewModel pvm = new Pay360TransactionViewModel
            //{

            //    Amount = payload.transactionAmount,
            //    TransactionId = payload.transactionId,
            //    isSuccess = payload.outcome.status == "SUCCESS" ? true : false,
            //    Reason = payload.outcome.reasonMessage
            //};

            if (AccountService.IsAuthorized(Paylaod) &&
                 Paylaod.ProductCodes != null
                && Paylaod.ProductCodes.Count > 0
                && Paylaod.ProductCodes.Where(x => x.ProductCode == "THM").First().Reference.Trim() == Paylaod.Checkout.Reference.Trim())
            {
                if (TempData["AutoTopUp"] != null && ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
                {
                    try
                    {
                        var autoTopUpModel = (QuickTopUpViewModel)TempData["AutoTopUp"];
                        await SetAutoToUpSettings(autoTopUpModel);
                    }
                    catch (Exception)
                    {
                    }
                }
            }


            if (TempData["IsInternationalTopUp"] != null && System.Convert.ToBoolean(TempData["IsInternationalTopUp"]) == true)
            {
                TempData["IsInternationalTopUp"] = true;
            }


            return Redirect(Urls.SuccessfulTopUp);
            //return View(view, pvm);
        }

        [ActionName("securereturncreditsim")]
        public async Task<ActionResult> securereturncreditsim(Secure3DViewModel model)
        {
            var Paylaod = GetPayload();

            if (!ModelState.IsValid)
            {
                TempData["PurchaseType"] = "CreditSimOrder";
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Checkout);
            }

            Pay360Resume3DRequest request = new Pay360Resume3DRequest
            {
                pareq = model.PaRes,
                pay360TransactionId = model.MD
            };

            var response = await Pay360Service.Resume3DTransaction(request);

            if (response == null)
            {
                TempData["PurchaseType"] = "CreditSimOrder";
                return ErrorRedirect(((int)Messages.RejectionWithError).ToString(), Urls.Checkout);
            }

            if (response.errorCode > 0)
            {

                LoggerService.Debug(GetType(), $"Method: securereturncreditsim, " +
                            $"PaymentResponse=>: { JsonConvert.SerializeObject(response) } ");


                TempData["ErrorCode"] = response.errorCode;
                TempData["PurchaseType"] = "CreditSimOrder";
                return ErrorRedirect(((int)Messages.RejectionWithError).ToString(), Urls.Checkout);
            }


            LoggerService.Debug(GetType(), $"Method: securereturncreditsim, " +
                $"Payload=>: {JsonConvert.SerializeObject(Paylaod.CreditSim)}, " +
                $"PaymentResponse=>: { JsonConvert.SerializeObject(response) } ");


            CreditSimPaymentApiRequest creditsimrequest = new CreditSimPaymentApiRequest
            {
                userId = Paylaod.CreditSim.userId,
                orderId = Paylaod.CreditSim.orderId,
                paymentType = 1,
                paymentTransactionId = response.payload.transactionId,
                paymentErrorCode = response.errorCode,
                paymentDate = DateTime.Now,
                paymentErrorMessage = response.message
            };

            var creditsimresponse = await TalkHomeWebService.CreditSimPayment(creditsimrequest);


            LoggerService.Debug(GetType(), $"Method: securereturncreditsim, " +
                                                $"CreditSimResponse=>: {JsonConvert.SerializeObject(creditsimresponse)}");

            if (creditsimresponse == null)
            {
                if (Paylaod.CreditSim != null)
                {
                    Paylaod.CreditSim = null;
                    SetPayload(Paylaod);
                }
                ViewBag.Msg = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimPaymentError).ToString()));
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = Paylaod.Checkout.Verify;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (creditsimresponse.errorCode != 0)
            {
                if (Paylaod.CreditSim != null)
                {
                    Paylaod.CreditSim = null;
                    SetPayload(Paylaod);
                }
                if (creditsimresponse.errorCode == 4)
                {
                    ViewBag.Msg = creditsimresponse.message;
                }
                else
                {
                    ViewBag.Msg = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimPaymentError).ToString()));
                }

                TempData["ErrorCode"] = creditsimresponse.errorCode;
                ViewBag.ErrorCode = creditsimresponse.errorCode;
                ViewBag.ProductCode = Paylaod.Checkout.Verify;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            TempData["IssignedUp"] = (Paylaod.CreditSim.signedUp != null) ? Paylaod.CreditSim.signedUp : "false";
            await MailExtentions.CreditSimSuccessMailAsync(Paylaod);
            TempData["OrderId"] = Paylaod.CreditSim.orderId != 0 ? Paylaod.CreditSim.orderId : 0;
            if (Paylaod.CreditSim != null)
            {
                Paylaod.CreditSim = null;
                SetPayload(Paylaod);
            }
            return RedirectToAction("CreditSimSuccess", "CustomPage");
        }

        async Task SetAutoToUpSettings(QuickTopUpViewModel autoTopUpModel)
        {
            string Msisdn = "";
            var Payload = GetPayload();
            AccountService.TryValidateNumber(autoTopUpModel.msisdn, "GB", out Msisdn);

            Pay360GetAutoTopUpRequest autoTopUpRequest = new Pay360GetAutoTopUpRequest
            {
                Msisdn = Msisdn,
                Email = Payload.FullName.Email
            };

            var pay360AutoTopUpDefaultSettings = await Pay360Service.GetAutoTopUp(autoTopUpRequest);


            var modelAutoTopUpRequest = new Pay360SetAutoTopUpRequest
            {
                isAutoTopup = autoTopUpModel.AutoTopUpEnabled,
                topupAmount = Convert.ToDecimal(autoTopUpModel.amount),
                productRef = autoTopUpRequest.Msisdn,
                topupCurrency = "GBP",
                productCode = "THM",
                productItemCode = "THM",
                thresholdBalanceAmount = pay360AutoTopUpDefaultSettings.payload.ThresHold,
                Email = Payload.FullName.Email
            };

            var result = await Pay360Service.SetAutoTopUp(modelAutoTopUpRequest);

            if (result != null)
            {
                Payload.AutoTopUpEnabled = modelAutoTopUpRequest.isAutoTopup;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> StartPurchasePayment(StartPay360ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //redirect back with validation errors
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Account/Checkout");
            }

            if (model.PaymentMethod == "NewCard")
            {
                model.PaymentMethod = "Card";
            }
            else if (model.PaymentMethod == "PaypalNew")
            {
                model.PaymentMethod = "Paypal";
            }
            else if (model.PaymentMethod == "CreditNew")
            {
                model.PaymentMethod = "Credit";
            }

            var Payload = GetPayload();
            bool isAuthenticated = false;
            isAuthenticated = AccountService.IsAuthorized(Payload);
            string Error = "";
            string custId = "";
            var ProductCode = "";
            var Product = (dynamic)null;
            Pay360PaymentType paymentType = Pay360PaymentType.New;

            if (Payload.FullName != null && AccountService.IsAuthorized(Payload))
            {
                custId = Payload.FullName.Email;
            }

            //Check if PayWithCredit
            if (Enum.Parse(typeof(TalkHome.Models.Enums.PaymentMethod), model.PaymentMethod).ToString() == TalkHome.Models.Enums.PaymentMethod.Credit.ToString())
            {
                if (Payload.Purchase.Count() > 0)
                {
                    Product = ContentService.GetProducts(Payload.Purchase.First()); // Get the basket item
                    ProductCode = ContentService.GetProductCode(Product);
                }
                else if (Payload.CheckoutProduct == "THATT")
                {
                    ProductCode = "THATT";
                }
                else if (Payload.Checkout.Basket.Count > 0)
                {
                    ProductCode = "THM";
                }

                if (Product == null)
                {
                    return RedirectToAction("CreditPayment", new CheckoutRequest { Id = 0, ProductCode = ProductCode });
                }

                CheckoutRequest co = new CheckoutRequest { Id = Product.Id, ProductCode = ProductCode };
                var Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(co.ProductCode)).Select(x => x.Reference).First();
                var AccountId = Payload.ProductCodes.Where(x => x.ProductCode.Equals(co.ProductCode)).Select(x => x.AccountId).First();
                List<string> successList = new List<string>();


                //Attempt to add the bundle
                //If successful redirect to credit payment successful page

                string bundleid = "";
                if (co.ProductCode == (string)Models.Enums.ProductCodes.THA.ToString())
                {
                    bundleid = ContentService.GetAppBundleGuid(co.Id).Trim();
                }
                else if (co.ProductCode == (string)Models.Enums.ProductCodes.THM.ToString())
                {
                    bundleid = ContentService.GetMobileBundleGuid(co.Id).Trim();
                }

                AddBundleDTO requestAddBundle = new AddBundleDTO
                {
                    MsisdnOrCardNumber = Reference,
                    BundleId = bundleid.Trim(),
                    ProductCode = co.ProductCode
                };

                var result = await TalkHomeWebService.AddBundleWithCredit(requestAddBundle, Payload.ApiToken);

                //var result = new GenericApiResponse<string> { errorCode = 0 };
                string bundleName = "";
                if (co.ProductCode == (string)Models.Enums.ProductCodes.THA.ToString())
                {
                    bundleName = ContentService.GetAppBundleByGuid(bundleid).Name;
                }
                else if (co.ProductCode == (string)Models.Enums.ProductCodes.THM.ToString())
                {
                    bundleName = ContentService.GetMobilePlanByGuid(bundleid).Name;
                }

                if (result == null)
                {
                    ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Checkout); // Request failed
                }
                else if (result.errorCode == 101)
                {
                    return ErrorRedirect(result.errorCode.ToString(), Urls.Checkout); // Request failed
                }
                else if (result.errorCode == 300)
                {
                    return ErrorRedirect(((int)Messages.MaxBundleLimitReached).ToString(), Urls.Checkout); // Max Bundle Reached
                }
                else if (result.errorCode != 0)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Checkout); // Request failed
                }

                successList.Add(bundleName);

                var RequestDTO1 = new AccountSummaryRequestDTO { productCode = co.ProductCode, token = Payload.ApiToken };
                var ResponseDTO1 = await AccountService.GetAccountSummary(RequestDTO1);

                if (ResponseDTO1 == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Checkout); // Request failed
                }

                SetPayload(Payload);

                TempData["bundlenamelist"] = successList.ToList();
                CreditPurchasesViewModel creditmodel = new CreditPurchasesViewModel { BundleNames = successList, Payload = Payload, CreditRemaining = ResponseDTO1.payload.userAccountSummary.creditRemaining };

                try
                {
                    if (isAuthenticated)
                    {
                        ///Bundle Auto Renewal 

                        AutoRenewalsSettingsRequestDTO renewal = new AutoRenewalsSettingsRequestDTO
                        {
                            msisdn = Reference,
                            productCode = co.ProductCode,
                            isAutoRenew = model.AutoTopUpEnabled,
                            calling_packageid = bundleid.Trim(),
                            AccountId = AccountId,
                            Email = Payload.FullName.Email,
                            BundleAmount = model.Amount.ToString(),
                            BundleName = bundleName

                        };
                        var ResponseDTO = await TalkHomeWebService.AutoRenewSettings(renewal, Payload.ApiToken);
                    }
                }
                catch (Exception ex)
                {

                }
                string SmsMsg = "";
                try
                {
                    SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["EmailHost"]);
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("", "");

                    SmsMsg = $"Talkhome \r\nYou have successfully purchased bundle. \r\nYour new balance is €{ResponseDTO1.payload.userAccountSummary.creditRemaining.ToString("0.00")}\r\nBundle Name: {bundleName}\r\n";


                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom_ForgotPassword"]);
                    mailMessage.To.Add(Payload.FullName.Email);

                    mailMessage.Body = SmsMsg;
                    mailMessage.Subject = "Talkhome Payment";
                    await client.SendMailAsync(mailMessage);

                }
                catch (Exception ex)
                {
                    LoggerService.Error(GetType(), $"Class: BL_Pay360CashierApi, Method: SendEmailToCustomer, Parameters=> customerEmail: {Payload.FullName.Email}, Message: {SmsMsg}, ErrorMessage: {ex.Message}", ex);

                }




                return RedirectToAction("PurchaseConfirmatioSimCredit", creditmodel);
                //return View("~/Views/Account/CreditPayment.cshtml", new CreditPurchasesViewModel { BundleNames = successList, Payload = Payload, CreditRemaining = ResponseDTO1.payload.userAccountSummary.creditRemaining });
            }

            else
            {

                if (Payload.Purchase.Count() > 0)
                {
                    Product = ContentService.GetProducts(Payload.Purchase.First()); // Get the basket item
                    ProductCode = ContentService.GetProductCode(Product);
                }
                else if (Payload.CheckoutProduct == "THATT")
                {
                    ProductCode = "THATT";
                }
                else if (Payload.Checkout.Basket.Count > 0)
                {
                    ProductCode = "THM";
                }

                if (Product == null)
                {
                    return RedirectToAction("CreditPayment", new CheckoutRequest { Id = 0, ProductCode = ProductCode });
                }

                CheckoutRequest co = new CheckoutRequest { Id = Product.Id, ProductCode = ProductCode };
                var Reference = isAuthenticated ? Payload.ProductCodes.Where(x => x.ProductCode.Equals(co.ProductCode)).Select(x => x.Reference).First() : "";
                var AccountId = isAuthenticated ? Payload.ProductCodes.Where(x => x.ProductCode.Equals(co.ProductCode)).Select(x => x.AccountId).First() : "";
                List<string> successList = new List<string>();


                //Attempt to add the bundle
                //If successful redirect to credit payment successful page

                string bundleid = "";
                if (co.ProductCode == (string)Models.Enums.ProductCodes.THA.ToString())
                {
                    bundleid = ContentService.GetAppBundleGuid(co.Id).Trim();
                }
                else if (co.ProductCode == (string)Models.Enums.ProductCodes.THM.ToString())
                {
                    bundleid = ContentService.GetMobileBundleGuid(co.Id).Trim();
                }

                string bundleName = "";
                if (co.ProductCode == (string)Models.Enums.ProductCodes.THA.ToString())
                {
                    bundleName = ContentService.GetAppBundleByGuid(bundleid).Name;
                }
                else if (co.ProductCode == (string)Models.Enums.ProductCodes.THM.ToString())
                {
                    bundleName = ContentService.GetMobilePlanByGuid(bundleid).Name;
                }

                ////Pay with Paypal
                //if (Enum.Parse(typeof(TalkHome.Models.Enums.PaymentMethod), model.PaymentMethod).ToString() == TalkHome.Models.Enums.PaymentMethod.Paypal.ToString())
                //{

                //}
                ////Pay with Card - Pay360
                //else
                //{

                //if (custId != "")
                //{
                //    var CustomerModel = await Pay360Service.GetCustomer(new Pay360CustomerRequestModel
                //    {
                //        customerUniqueRef = custId,
                //        productCode = "THM"
                //    });
                //}

                //Error checke failure case


                string view = string.Format("~/Views/Account/Redirect3dSecure.cshtml");
                Pay360TransactionViewModel pvm = null;
                Pay360PaymentRequest request = null;
                if (String.IsNullOrEmpty(custId))
                {
                    //NEW Customer
                    var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestNew(model);
                    request = new Pay360PaymentRequest
                    {
                        Pay360PaymentRequestNew = pay360PaymentRequest
                    };

                }
                else if (!String.IsNullOrEmpty(model.CardId) && model.CardId != "Card")
                {
                    //EXISTING Customer non default card
                    var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestToken(model);

                    request = new Pay360PaymentRequest
                    {
                        Pay360PaymentRequestToken = pay360PaymentRequest
                    };

                    paymentType = Pay360PaymentType.Token;

                }
                else if (String.IsNullOrEmpty(model.CardId) || model.CardId == "Card")
                {
                    if (!String.IsNullOrEmpty(custId))
                    {
                        //NEW Customer
                        var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestNew(model);
                        request = new Pay360PaymentRequest
                        {
                            Pay360PaymentRequestNew = pay360PaymentRequest
                        };
                    }
                    else
                    {
                        var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestExistingNew(model);

                        if (model.NoCards == "Yes")
                        {
                            pay360PaymentRequest.isDefaultCard = true;
                        }

                        request = new Pay360PaymentRequest
                        {
                            Pay360PaymentRequestExistingNew = pay360PaymentRequest
                        };

                        //EXISTING Customer new payment
                        paymentType = Pay360PaymentType.ExistingNew;
                    }
                }


                var Pay360PaymentResponse = await Pay360Service.Pay360Payment(request, paymentType, Payload, GetRequestIP());

                if (Pay360PaymentResponse == null)
                {
                    //log.Error(String.Format("PaymentMethod request failed for msisdn:{0}, Request Type {1}", model.Msisdn, model.PaymentMethod));
                    ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                    ViewBag.ErrorCode = 2;
                    ViewBag.ProductCode = Payload.Checkout.Verify;
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }

                if (Pay360PaymentResponse.errorCode > 0)
                {
                    //log.Error(String.Format("PaymentMethod request failed for msisdn:{0}, Request Type {1}, Error Code {2}", model.Msisdn, model.PaymentMethod, Pay360PaymentResponse.errorCode.ToString()));

                    if (Pay360PaymentResponse.errorCode == 2)
                    {
                        TempData["ErrorCode"] = 999;
                        ViewBag.Msg = Pay360PaymentResponse.message;
                        ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                        ViewBag.ProductCode = Payload.Checkout.Verify;
                        return View("~/Views/Shared/ErrorPay360.cshtml");
                    }
                    else if (Pay360PaymentResponse.errorCode == 3)   // exceeded daily limit
                    {
                        TempData["ErrorCode"] = 999;
                        ViewBag.Msg = Pay360PaymentResponse.message;
                        ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                        ViewBag.ProductCode = Payload.Checkout.Verify;
                        return View("~/Views/Shared/ErrorPay360.cshtml");
                    }
                    else
                    {
                        TempData["ErrorCode"] = Pay360PaymentResponse.errorCode;
                        ViewBag.Msg = Pay360PaymentResponse.message;
                        ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                        ViewBag.ProductCode = Payload.Checkout.Verify;
                    }
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }

                var payload = Pay360PaymentResponse.payload;

                string domain = Pay360Service.GetResumeUrl("pay360account/securereturn");

                if (TempData["AutoTopUp"] != null)
                {
                    TempData["AutoTopUp"] = TempData["AutoTopUp"];
                }

                if (payload.outcome.reasonCode == "U100")
                {

                    try
                    {
                        if (isAuthenticated)
                        {
                            ///Bundle Auto Renewal 

                            AutoRenewalsSettingsRequestDTO renewal = new AutoRenewalsSettingsRequestDTO
                            {
                                msisdn = Reference,
                                productCode = co.ProductCode,
                                isAutoRenew = model.AutoTopUpEnabled,
                                calling_packageid = bundleid.Trim(),
                                AccountId = AccountId,
                                Email = Payload.FullName.Email,
                                BundleAmount = model.Amount.ToString(),
                                BundleName = bundleName

                            };
                            TempData["AutoBundleRenewal"] = renewal;

                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    pvm = new Pay360TransactionViewModel
                    {
                        url = payload.clientRedirect.url,
                        pareq = payload.clientRedirect.pareq,
                        type = payload.clientRedirect.type,
                        TransactionId = Pay360PaymentResponse.payload.transactionId,
                        returnUrl = domain
                    };

                    return View(view, pvm);
                }
                else
                {

                    if (AccountService.IsAuthorized(Payload))
                    {
                        if (TempData["AutoTopUp"] != null && ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
                        {
                            try
                            {
                                var autoTopUpModel = (QuickTopUpViewModel)TempData["AutoTopUp"];
                                await SetAutoToUpSettings(autoTopUpModel);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    try
                    {
                        if (isAuthenticated)
                        {
                            ///Bundle Auto Renewal 

                            AutoRenewalsSettingsRequestDTO renewal = new AutoRenewalsSettingsRequestDTO
                            {
                                msisdn = Reference,
                                productCode = co.ProductCode,
                                isAutoRenew = model.AutoTopUpEnabled,
                                calling_packageid = bundleid.Trim(),
                                AccountId = AccountId,
                                Email = Payload.FullName.Email,
                                BundleAmount = model.Amount.ToString(),
                                BundleName = bundleName

                            };
                            var ResponseDTO = await TalkHomeWebService.AutoRenewSettings(renewal, Payload.ApiToken);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    return Redirect(Urls.SuccessfulPurchase);
                }
                //}
            }

        }

        /// Recieves a POST request to begin a Top up checkout
        /// </summary>
        /// <param name="model">The checkout request model</param>
        /// <returns>The details confirmation view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public JsonResult TopUpCheckout(QuickTopUpViewModel model)
        {
            var responseModel = new QuickTopUpJsonResponse();

            if (!ModelState.IsValid)
            {
                responseModel.Message = "Failure";
                return Json(responseModel);
            }

            var Payload = GetPayload();

            Payload.TopUp.Clear();
            Payload.Purchase.Clear();
            Payload.TopUp.Add(model.TopUpId);
            Payload.Checkout = new CheckoutPageDTO { Verify = "THM", ProductType = ProductType.TopUp.ToString(), Total = decimal.Parse(model.amount) };


            var Reference = "";

            try
            {
                Reference = AccountService.GetMsisdnFromNumber(model.msisdn, "GB");
            }
            catch (Exception)
            {
                // Calling cards numbers will always fail, it's expected. Proceed
            }


            TempData["PurchaseType"] = "QuickTopup";

            TempData["AutoTopUp"] = model;
            Payload.Checkout.Reference = Reference;
            Response.Cookies.Add(AccountService.EncodeCookie(Payload));
            responseModel.Message = "Success";
            responseModel.Url = Urls.Checkout;
            return Json(responseModel);
        }

        [HttpPost]
        public async Task<JsonResult> VerifyNumber(QuickTopUpVerifyNumber model)
        {
            var Msisdn = "";
            string Sim_ActivationDate = "";
            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            var Payload = GetPayload();
            //var Verify = await PaymentService.VerifyQuickTopUpNumber(model.msisdn, "GB");

            //if (!Verify)
            //{
            //    return Json("Failure");
            //}

            try
            {
                Msisdn = AccountService.GetMsisdnFromNumber(model.msisdn, "GB");
                Sim_ActivationDate = AccountService.Get_Sim_ActivationDate(Msisdn);
            }
            catch (Exception)
            {
                // Calling cards numbers will always fail, it's expected. Proceed
            }

            return Json(new { errorCode = 0, message = "Success", sim_activationdate = Sim_ActivationDate });
        }
        [HttpPost]
        public async Task<JsonResult> VerifyNumberForCheckOut(QuickTopUpVerifyNumber model)

        {
            var Payload = GetPayload();

            var Msisdn = "";
            string Sim_ActivationDate = "";
            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            //var Verify = await PaymentService.VerifyQuickTopUpNumber(model.msisdn, "GB");

            //if (!Verify)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            //    return Json("Failure");
            //}

            try
            {
                Msisdn = AccountService.GetMsisdnFromNumber(model.msisdn, "GB");
                Sim_ActivationDate = AccountService.Get_Sim_ActivationDate(Msisdn);
            }
            catch (Exception)
            {
                // Calling cards numbers will always fail, it's expected. Proceed
            }

            Payload.Checkout.Reference = Msisdn;
            // Payload.FullName.Email = model.Email;
            SetPayload(Payload);
            return Json(new { errorCode = 0, message = "success", Sim_activationdate = Sim_ActivationDate });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public ActionResult PurchaseCheckout(CheckoutRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/");
            }

            var Payload = GetPayload();

            if (!String.IsNullOrEmpty(model.Source) && (model.Source == "blog" || model.Source == "landing"))
            {
                Payload.HomeRoot = "Homepage";
            }

            var Product = ContentService.GetProducts(model.Id);

            Payload.TopUp.Clear();
            Payload.Purchase.Clear();
            Payload.Purchase.Add(model.Id);
            Payload.Checkout = new CheckoutPageDTO { Verify = model.ProductCode, ProductType = ProductType.Bundle.ToString(), Total = Product.ProductPrice };
            Response.Cookies.Add(AccountService.EncodeCookie(Payload));

            //if (model.ProductCode.ToLower() == "thcc")
            //{
            //    Payload.isTHCCPin = true;
            //    Payload.CheckoutProduct = "THCC";
            //}

            //if (model.ProductCode.Equals(Models.Enums.ProductCodes.THCC.ToString())) // Calling card PIN. No check is required
            //{
            //    return Redirect(Urls.Checkout);
            //}

            if (AccountService.IsAuthorized(Payload))
            {
                var Reference = "";

                try
                {
                    Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(model.ProductCode)).Select(x => x.Reference).First();
                }
                catch // The registred user doesn't have that product. Send to add a product screen on MyAccount
                {
                    return ErrorRedirect(((int)Messages.ProductNotRegisteredForPurchase).ToString(), Urls.MyAccount + "/" + model.ProductCode);
                }

                Payload.Checkout.Reference = Reference;
                SetPayload(Payload);
                return Redirect(Urls.Checkout);
            }

            return Redirect(Urls.Checkout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> StartTopUpPayment(StartPay360ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //redirect back with validation errors
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/Account/Checkout");
            }

            var Payload = GetPayload();
            Payload.Checkout.Total = model.Amount;
            string Error = "";
            string custId = "";
            //GenericPay360ApiResponse<Pay360CustomerModel> customerModel = new GenericPay360ApiResponse<Pay360CustomerModel>();

            if (Payload.FullName != null)
            {
                custId = Payload.FullName.Email;
            }

            //if (custId != "")
            //{
            //    customerModel = await Pay360Service.GetCustomer(new Pay360CustomerRequestModel
            //    {
            //        customerUniqueRef = custId,
            //        productCode = "THM"
            //    });
            //}
            //Error checke failure case
            if (!string.IsNullOrEmpty(model.EmailAddress))
            {
                TempData["emailAddrssforAnalytics"] = model.EmailAddress;
            }


            string view = string.Format("~/Views/Account/Redirect3dSecure.cshtml");
            Pay360TransactionViewModel pvm = null;
            Pay360PaymentRequest request = null;
            Pay360PaymentType paymentType = Pay360PaymentType.New;
            if (String.IsNullOrEmpty(custId))
            {
                //NEW Customer
                var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestNew(model);
                request = new Pay360PaymentRequest
                {
                    Pay360PaymentRequestNew = pay360PaymentRequest
                };
            }
            else if (!String.IsNullOrEmpty(model.CardId) && model.CardId != "Card")
            {
                //EXISTING Customer non default card
                var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestToken(model);
                request = new Pay360PaymentRequest
                {
                    Pay360PaymentRequestToken = pay360PaymentRequest
                };
                paymentType = Pay360PaymentType.Token;
            }
            else if (String.IsNullOrEmpty(model.CardId) || model.CardId == "Card")
            {
                if (!String.IsNullOrEmpty(custId))
                {
                    //NEW Customer
                    var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestNew(model);
                    request = new Pay360PaymentRequest
                    {
                        Pay360PaymentRequestNew = pay360PaymentRequest
                    };
                }
                else
                {
                    var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestExistingNew(model);

                    if (model.NoCards == "Yes")
                    {
                        pay360PaymentRequest.isDefaultCard = true;
                    }

                    request = new Pay360PaymentRequest
                    {
                        Pay360PaymentRequestExistingNew = pay360PaymentRequest
                    };

                    //EXISTING Customer new payment
                    paymentType = Pay360PaymentType.ExistingNew;
                }
            }

            var Pay360PaymentResponse = await Pay360Service.Pay360Payment(request, paymentType, Payload, GetRequestIP());

            if (Pay360PaymentResponse == null)
            {
                //log.Error(String.Format("PaymentMethod request failed for msisdn:{0}, Request Type {1}", model.Msisdn, model.PaymentMethod));
                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = Payload.Checkout.Verify;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (Pay360PaymentResponse.errorCode > 0)
            {
                //log.Error(String.Format("PaymentMethod request failed for msisdn:{0}, Request Type {1}, Error Code {2}", model.Msisdn, model.PaymentMethod, Pay360PaymentResponse.errorCode.ToString()));

                if (Pay360PaymentResponse.errorCode == 2)
                {
                    TempData["ErrorCode"] = 999;
                    ViewBag.Msg = Pay360PaymentResponse.message;
                    ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                    ViewBag.ProductCode = Payload.Checkout.Verify;
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }
                else if (Pay360PaymentResponse.errorCode == 3)   // exceeded daily limit
                {
                    TempData["ErrorCode"] = 999;
                    ViewBag.Msg = Pay360PaymentResponse.message;
                    ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                    ViewBag.ProductCode = Payload.Checkout.Verify;
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }
                else
                {
                    TempData["ErrorCode"] = Pay360PaymentResponse.errorCode;
                    ViewBag.Msg = Pay360PaymentResponse.message;
                    ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                    ViewBag.ProductCode = Payload.Checkout.Verify;
                }
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            var payload = Pay360PaymentResponse.payload;


            string domain = Pay360Service.GetResumeUrl("pay360account/securereturntopup");



            if (model.AutoTopUpSwitchVisibleOnCheckout)
            {
                var autoTopUpModel = new QuickTopUpViewModel
                {
                    amount = model.Amount.ToString(),
                    AutoTopUpEnabled = model.AutoTopUpEnabled,
                    msisdn = Payload.Checkout.Reference
                };
                TempData["AutoTopUp"] = autoTopUpModel;
            }

            if (payload.outcome.reasonCode == "U100")
            {
                if (TempData["IsInternationalTopUp"] != null && System.Convert.ToBoolean(TempData["IsInternationalTopUp"]) == true)
                {
                    TempData["IsInternationalTopUp"] = true;
                }

                pvm = new Pay360TransactionViewModel
                {
                    url = payload.clientRedirect.url,
                    pareq = payload.clientRedirect.pareq,
                    type = payload.clientRedirect.type,
                    TransactionId = Pay360PaymentResponse.payload.transactionId,
                    returnUrl = domain
                };

                return View(view, pvm);
            }
            else
            {
                if (TempData["IsInternationalTopUp"] != null && System.Convert.ToBoolean(TempData["IsInternationalTopUp"]) == true)
                {
                    TempData["IsInternationalTopUp"] = true;
                }

                view = string.Format("~/Views/Account/Confirmation.cshtml");
                pvm = new Pay360TransactionViewModel
                {
                    Amount = Pay360PaymentResponse.payload.transactionAmount,
                    TransactionId = Pay360PaymentResponse.payload.transactionId,
                    isSuccess = Pay360PaymentResponse.payload.outcome.status == "SUCCESS" ? true : false,
                    Reason = Pay360PaymentResponse.payload.outcome.reasonMessage

                };

                if (AccountService.IsAuthorized(Payload)
                    && Payload.ProductCodes != null
                    && Payload.ProductCodes.Count > 0
                    && Payload.ProductCodes.Where(x => x.ProductCode == "THM").First().Reference.Trim() == Payload.Checkout.Reference.Trim())
                {
                    if (TempData["AutoTopUp"] != null && ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
                    {
                        try
                        {
                            var autoTopUpModel = (QuickTopUpViewModel)TempData["AutoTopUp"];
                            await SetAutoToUpSettings(autoTopUpModel);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                //return View(view, pvm);
                return Redirect(Urls.SuccessfulTopUp);
            }

        }

        public void ExceptionTest()
        {
            throw new Exception();
        }

        [HttpPost]
        [ActionName("checkappnumber")]
        public async Task<ActionResult> CheckAppNumber(string AppNumber)
        {
            var response = await AccountService.ValidAppUser(AppNumber);

            var json = JsonConvert.SerializeObject(response);


            return Json(json);
        }

        public ActionResult PurchaseConfirmatioSimCredit(CreditPurchasesViewModel model)
        {
            var Payload = GetPayload();
            if (TempData["bundlenamelist"] != null)
            {
                model.BundleNames = TempData["bundlenamelist"] as List<string>;
            }

            if (Payload.Checkout != null)
            {
                TempData["GoogleAmount"] = Payload.Checkout.Total;
            }

            if (Payload.Basket != null)
            {
                Payload.Basket.Clear();
                SetPayload(Payload);
            }

            return View("~/Views/Account/CreditPayment.cshtml", model);
        }

        [HttpPost]
        //[Route("startcreditsimpayment")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreditSimPayment(StartPay360ViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Checkout);
            }
            model.FirstUseDate = null;
            var Payload = GetPayload();
            Payload.Checkout.Total = model.Amount;
            string Error = "";
            string custId = "";

            if (Payload.FullName != null)
            {
                custId = Payload.FullName.Email;
            }

            //Error checke failure case
            if (!string.IsNullOrEmpty(model.EmailAddress))
            {
                TempData["emailAddrssforAnalytics"] = model.EmailAddress;
            }
            string view = string.Format("~/Views/Account/Redirect3dSecure.cshtml");
            Pay360TransactionViewModel pvm = null;
            Pay360PaymentRequest request = null;
            Pay360PaymentType paymentType = Pay360PaymentType.New;
            if (String.IsNullOrEmpty(custId))
            {
                //NEW Customer
                var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestNew(model);
                request = new Pay360PaymentRequest
                {
                    Pay360PaymentRequestNew = pay360PaymentRequest
                };
                request.Pay360PaymentRequestNew.isAuthorizationOnly = false;
                request.Pay360PaymentRequestNew.isDirectFullfilment = false;
                request.Pay360PaymentRequestNew.do3DSecure = true;

            }
            else if (!String.IsNullOrEmpty(model.CardId) && model.CardId != "Card")
            {
                //EXISTING Customer non default card
                var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestToken(model);

                request = new Pay360PaymentRequest
                {
                    Pay360PaymentRequestToken = pay360PaymentRequest
                };

                request.Pay360PaymentRequestToken.isAuthorizationOnly = false;
                request.Pay360PaymentRequestToken.isDirectFullfilment = false;
                request.Pay360PaymentRequestToken.do3DSecure = true;


                paymentType = Pay360PaymentType.Token;

            }
            else if (String.IsNullOrEmpty(model.CardId) || model.CardId == "Card")
            {
                if (!String.IsNullOrEmpty(custId))
                {
                    //NEW Customer
                    var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestNew(model);
                    request = new Pay360PaymentRequest
                    {
                        Pay360PaymentRequestNew = pay360PaymentRequest
                    };

                    request.Pay360PaymentRequestNew.isAuthorizationOnly = false;
                    request.Pay360PaymentRequestNew.isDirectFullfilment = false;
                    request.Pay360PaymentRequestNew.do3DSecure = true;

                }
                else
                {
                    var pay360PaymentRequest = Pay360Service.CreatePay360PaymentRequestExistingNew(model);

                    if (model.NoCards == "Yes")
                    {
                        pay360PaymentRequest.isDefaultCard = true;
                    }

                    request = new Pay360PaymentRequest
                    {
                        Pay360PaymentRequestExistingNew = pay360PaymentRequest
                    };

                    request.Pay360PaymentRequestExistingNew.isAuthorizationOnly = false;
                    request.Pay360PaymentRequestExistingNew.isDirectFullfilment = false;
                    request.Pay360PaymentRequestExistingNew.do3DSecure = true;

                    //EXISTING Customer new payment
                    paymentType = Pay360PaymentType.ExistingNew;
                }
            }

            bool Iscreditsim = false;
            if (!string.IsNullOrEmpty(model.CreditSim.ProductType) && model.CreditSim.ProductType.Equals(ProductType.CreditSimOrder.ToString()))
            {
                Payload.CreditSim = new CreditSimPayload();
                Payload.CreditSim.Name = model.CreditSim.Name;
                Payload.CreditSim.userId = model.CreditSim.userId;
                Payload.CreditSim.orderId = model.CreditSim.orderId;
                Payload.CreditSim.Email = model.CreditSim.Email;
                Payload.CreditSim.signedUp = model.CreditSim.signedUp;
                Payload.CreditSim.orderId = model.CreditSim.orderId;
                Iscreditsim = true;
            }
            var Pay360PaymentResponse = await Pay360Service.Pay360Payment(request, paymentType, Payload, GetRequestIP(), Iscreditsim);

            if (Pay360PaymentResponse == null)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                    SetPayload(Payload);
                }
                ViewBag.Msg = "Payment Service is not responding at the moment. Please try again later";
                ViewBag.ErrorCode = 2;
                ViewBag.ProductCode = Payload.Checkout.Verify;
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }

            if (Pay360PaymentResponse.errorCode > 0)
            {
                if (Payload.CreditSim != null)
                {
                    Payload.CreditSim = null;
                }

                if (Pay360PaymentResponse.errorCode == 2)
                {
                    TempData["ErrorCode"] = 999;
                    ViewBag.Msg = Pay360PaymentResponse.message;
                    ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                    ViewBag.ProductCode = Payload.Checkout.Verify;
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }
                else if (Pay360PaymentResponse.errorCode == 3)   // exceeded daily limit
                {
                    TempData["ErrorCode"] = 999;
                    ViewBag.Msg = Pay360PaymentResponse.message;
                    ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                    ViewBag.ProductCode = Payload.Checkout.Verify;
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }
                else
                {
                    TempData["ErrorCode"] = Pay360PaymentResponse.errorCode;
                    ViewBag.Msg = Pay360PaymentResponse.message;
                    ViewBag.ErrorCode = Pay360PaymentResponse.errorCode;
                    ViewBag.ProductCode = Payload.Checkout.Verify;
                }
                return View("~/Views/Shared/ErrorPay360.cshtml");
            }
            Payload.CreditSim.PaymentType = (int)paymentType;

            var payload = Pay360PaymentResponse.payload;
            string domain = Pay360Service.GetResumeUrl("pay360account/securereturncreditsim");

            if (payload.outcome.reasonCode == "U100")
            {
                pvm = new Pay360TransactionViewModel
                {
                    url = payload.clientRedirect.url,
                    pareq = payload.clientRedirect.pareq,
                    type = payload.clientRedirect.type,
                    TransactionId = Pay360PaymentResponse.payload.transactionId,
                    returnUrl = domain
                };
                TempData["OrderId"] = Payload.CreditSim.orderId != 0 ? Payload.CreditSim.orderId : 0;

                return View(view, pvm);
            }
            else
            {
                CreditSimPaymentApiRequest creditsimrequest = new CreditSimPaymentApiRequest
                {
                    userId = Payload.CreditSim.userId,
                    orderId = Payload.CreditSim.orderId,
                    paymentType = 1,
                    paymentTransactionId = Pay360PaymentResponse.payload.transactionId,
                    paymentErrorCode = Pay360PaymentResponse.errorCode,
                    paymentDate = DateTime.Now,
                    paymentErrorMessage = Pay360PaymentResponse.message
                };
                var creditsimresponse = await TalkHomeWebService.CreditSimPayment(creditsimrequest);

                LoggerService.Debug(GetType(), $"Method: CreditSimPayment, " +
                                        $"Payload=>: {JsonConvert.SerializeObject(Payload.CreditSim)}, " +
                                        $"PaymentResponse=>: { JsonConvert.SerializeObject(creditsimresponse) } ");

                if (creditsimresponse == null)
                {
                    if (Payload.CreditSim != null)
                    {
                        Payload.CreditSim = null;
                        SetPayload(Payload);
                    }

                    ViewBag.Msg = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimPaymentError).ToString()));
                    return View("~/Views/Shared/ErrorPay360.cshtml");
                }
                if (creditsimresponse.errorCode != 0)
                {
                    if (Payload.CreditSim != null)
                    {
                        Payload.CreditSim = null;
                        SetPayload(Payload);
                    }
                    if (creditsimresponse.errorCode == 4)
                    {
                        ViewBag.Msg = creditsimresponse.message;
                    }
                    else
                    {
                        ViewBag.Msg = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimPaymentError).ToString()));
                    }
                    TempData["ErrorCode"] = creditsimresponse.errorCode;
                    ViewBag.ErrorCode = creditsimresponse.errorCode;
                    ViewBag.ProductCode = Payload.Checkout.Verify;
                    return View("~/Views/Shared/ErrorPay360.cshtml");

                }
                TempData["IssignedUp"] = Payload.CreditSim.signedUp;

                await MailExtentions.CreditSimSuccessMailAsync(Payload);


                if (Payload.CreditSim != null)
                {
                    TempData["OrderId"] = Payload.CreditSim.orderId != 0 ? Payload.CreditSim.orderId : 0;
                    Payload.CreditSim = null;
                    SetPayload(Payload);
                }

                return RedirectToAction("CreditSimSuccess", "CustomPage");
            }

        }

        public ActionResult CookieAccept()
        {
            return PartialView("_CookieAccept");
        }
    }


}
