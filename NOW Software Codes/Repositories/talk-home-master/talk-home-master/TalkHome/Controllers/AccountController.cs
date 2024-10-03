using AutoMapper;
using Newtonsoft.Json;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.ErrorLog;
using TalkHome.Extensions;
using TalkHome.Filters;
using TalkHome.Interfaces;
using TalkHome.Logger;
using TalkHome.Models;
using TalkHome.Models.Enums;
using TalkHome.Models.Pay360;
using TalkHome.Models.Porting;
using TalkHome.Models.ViewModels;
using TalkHome.Models.ViewModels.DTOs;
using TalkHome.Models.ViewModels.Pay360;
using TalkHome.Models.ViewModels.Payment;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.WebApi.Payment;
using TalkHome.WebServices.Interfaces;
using TalkHome.App_LocalResources;
using System.IO;
using Umbraco.Core.Models.PublishedContent;
using TalkHome.Models.WebApi;
using TalkHome.Services;
using System.Web;
using Umbraco.Web.PublishedContentModels;
using TalkHome.Models.ViewModels.Umbraco;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Manages HTTP request/responses for My Account pages that are not related to customer history
    /// </summary>
    [GCLIDFilter]
    public class AccountController : BaseController
    {
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly IAccountService AccountService;
        private readonly ILoggerService LoggerService;
        private readonly IPaymentService PaymentService;
        private readonly IContentService ContentService;
        private readonly IActiveCampaignService ActiveCampaignService;
        private readonly IAirTimeTransferService AirTimeTransferService;
        private readonly IPortService PortService;
        private Properties.URLs Urls = Properties.URLs.Default;
        private readonly IPay360Service Pay360Service;

        public AccountController(IAccountService accountService, ITalkHomeWebService talkHomeWebService, IPaymentService paymentService, IContentService contentService, ILoggerService loggerService, IActiveCampaignService activeCampaignService, IAirTimeTransferService airtimetransferService, IPay360Service pay360Service, IPortService portService)
        {
            TalkHomeWebService = talkHomeWebService;
            AccountService = accountService;
            PaymentService = paymentService;
            ContentService = contentService;
            LoggerService = loggerService;
            ActiveCampaignService = activeCampaignService;
            AirTimeTransferService = airtimetransferService;
            PortService = portService;
            Pay360Service = pay360Service;
        }

        /// <summary>
        /// Redirects and logs failed log in
        /// </summary>
        /// <param name="model">The request model</param>
        /// <param name="returnUrl">The Url to redirect to after login</param>
        /// <returns>The login view</returns>
        private ActionResult FailedLogin(string email, int errorCode, string returnUrl)
        {
            if (errorCode == (int)Messages.AuthenticationFailed || errorCode == 997)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.AuthenticationFailed.ToString(), "for", email));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.AuthenticationFailed.ToString(), "for", email));

                return ErrorRedirect(((int)Messages.AuthenticationFailed).ToString(), Urls.Login, returnUrl);
            }

            else if (errorCode == (int)Messages.TooManyLogInAttempts)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.TooManyLogInAttempts.ToString(), "for", email));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.TooManyLogInAttempts.ToString(), "for", email));

                return ErrorRedirect(((int)Messages.TooManyLogInAttempts).ToString(), Urls.Login, returnUrl);
            }

            else if (errorCode == 2)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.AccountNotFound.ToString(), "for", email));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.AccountNotFound.ToString(), "for", email));

                return ErrorRedirect(((int)Messages.AccountNotFound).ToString(), Urls.Login, returnUrl);
            }

            else if (errorCode == (int)Messages.AccountNotConfirmed)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.AccountNotConfirmed.ToString(), "for", email));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.AccountNotConfirmed.ToString(), "for", email));

                return ErrorRedirect(((int)Messages.AccountNotConfirmed).ToString(), Urls.Login, returnUrl);
            }

            else
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.UnknownLogInError.ToString(), "for", email));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.UnknownLogInError.ToString(), "for", email));

                return ErrorRedirect(((int)Messages.UnknownLogInError).ToString(), Urls.Login, returnUrl);
            }
        }

        /// <summary>
        /// Redirects and logs failed sign up
        /// </summary>
        /// <param name="email">The user email</param>
        /// <param name="errorCode">The error code</param>
        /// <returns>The view</returns>
        private ActionResult FailedSignUp(string email, int errorCode)
        {
            if (errorCode == (int)Messages.AccountAlreadyExists)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.AccountAlreadyExists.ToString(), "for", email));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.AccountAlreadyExists.ToString(), "for", email));

                return ErrorRedirect(((int)Messages.AccountAlreadyExists).ToString(), Urls.Login);
            }

            else
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.SignUpFailed.ToString(), "for", email));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.SignUpFailed.ToString(), "for", email));

                return ErrorRedirect(((int)Messages.AccountAlreadyExists).ToString(), Urls.SignUp);
            }
        }



        /// <summary>
        /// Redirects and logs failed Sign up verifications
        /// </summary>
        /// <param name="email">The user email</param>
        /// <param name="errorCode">The error code</param>
        /// <returns>The view</returns>
        private ActionResult FailedAccountVerification(int errorCode)
        {
            if (errorCode == (int)Messages.AccountAlreadyConfirmed)
            {
                LoggerService.SendInfoAlert(Messages.AccountAlreadyConfirmed.ToString());
                LoggerService.Info(GetType(), Messages.AccountAlreadyConfirmed.ToString());

                return ErrorRedirect(((int)Messages.AccountAlreadyConfirmed).ToString(), Urls.Login);
            }

            if (errorCode == (int)Messages.VerifySignUpFailed)
            {
                LoggerService.SendInfoAlert(Messages.VerifySignUpFailed.ToString());
                LoggerService.Info(GetType(), Messages.VerifySignUpFailed.ToString());

                return ErrorRedirect(((int)Messages.VerifySignUpFailed).ToString(), Urls.Login);
            }

            else
            {
                LoggerService.SendInfoAlert(Messages.UnknownVerifySignUpFailed.ToString());
                LoggerService.Info(GetType(), Messages.UnknownVerifySignUpFailed.ToString());

                return ErrorRedirect(((int)Messages.UnknownVerifySignUpFailed).ToString(), Urls.SignUp);
            }
        }

        /// <summary>
        /// Redirects and logs failed product registrations
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="number">The number</param>
        /// <returns>the view</returns>
        private ActionResult FailedAddProduct(int errorCode, string number)
        {
            if (errorCode == (int)Messages.InvalidMsisdnOrPUK)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.InvalidMsisdnOrPUK.ToString(), "for", number));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.InvalidMsisdnOrPUK.ToString(), "for", number));

                return ErrorRedirect(((int)Messages.InvalidMsisdnOrPUK).ToString(), Urls.ConfirmProductDetails);
            }

            else if (errorCode == (int)Messages.InvalidMsisdnOrPIN)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.InvalidMsisdnOrPIN.ToString(), "for", number));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.InvalidMsisdnOrPIN.ToString(), "for", number));

                return ErrorRedirect(((int)Messages.InvalidMsisdnOrPIN).ToString(), Urls.ConfirmProductDetails);
            }

            else if (errorCode == (int)Messages.ProductAlreadyAdded)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.ProductAlreadyAdded.ToString(), "for", number));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.ProductAlreadyAdded.ToString(), "for", number));

                return ErrorRedirect(((int)Messages.ProductAlreadyAdded).ToString(), Urls.ConfirmProductDetails);
            }

            else if (errorCode == (int)Messages.UnknownAccount)
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.UnknownAccount.ToString(), "for", number));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.UnknownAccount.ToString(), "for", number));

                return ErrorRedirect(((int)Messages.UnknownAccount).ToString(), Urls.ConfirmProductDetails);
            }

            else
            {
                LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.AddProductFailed.ToString(), "for", number));
                LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.AddProductFailed.ToString(), "for", number));

                return ErrorRedirect(((int)Messages.AddProductFailed).ToString(), Urls.ConfirmProductDetails);
            }
        }

        /// <summary>
        /// Processes a login request
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> Login(LoginRequest model)
        {

            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.Login, model.ReturnUrl);
            }

            var Payload = GetPayload();

            if (AccountService.IsAuthorized(Payload) && Payload.ProductCodes.Count > 0)
            {
                return Redirect(Urls.MyAccount);
            }

            var RequestDTO = new LoginRequestDTO { email = model.EmailAddress, password = model.Password, ipAddress = GetRequestIP() };
            var ResponseDTO = await TalkHomeWebService.AuthenticateCustomer(RequestDTO);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Login, model.ReturnUrl);
            }
            else if (ResponseDTO.errorCode != 0 && Payload.HomeRoot != null && Payload.HomeRoot == "Homepage")
            {
                return FailedLogin(model.EmailAddress, ResponseDTO.errorCode, model.ReturnUrl); // Unsuccessful login
            }
            else if (ResponseDTO.errorCode != 0)
            {
                await MigrationMail("Calling card user failed login", "email:" + model.EmailAddress + " pwd:" + model.Password);
                var LegacyAccount = await AccountService.LegacyCardUserExists(model.EmailAddress, model.Password);
                if (LegacyAccount.ErrorCode != 0)
                {
                    return FailedLogin(model.EmailAddress, (int)Messages.AuthenticationFailed, model.ReturnUrl); // Unsuccessful login
                }
                else
                {
                    await MigrationMail("Legacy User", model.EmailAddress);
                    LegacyAccount.FirstName = (String.IsNullOrEmpty(LegacyAccount.FirstName) ? "First Name" : LegacyAccount.FirstName);
                    LegacyAccount.LastName = (String.IsNullOrEmpty(LegacyAccount.LastName) ? "Last Name" : LegacyAccount.LastName);

                    return RedirectToAction("GenericSignUp", new SignUpRequest
                    {
                        EmailAddress = model.EmailAddress,
                        Password = model.Password,
                        ConfirmPassword = model.Password,
                        LegacyMigration = true,
                        LegacyCallingCardNumber = LegacyAccount.CallingCardNumber,
                        LegacyPinNumber = LegacyAccount.CallingPinNumber,
                        FirstName = LegacyAccount.FirstName,
                        LastName = LegacyAccount.LastName
                    });


                }
            }

            //if (Payload.HomeRoot != null && Payload.HomeRoot != "Homepage")
            //{
            //    await MigrationMail("Successful Calling Card Login", model.EmailAddress);
            //}

            //Payload.TwoLetterISORegionName = !string.IsNullOrWhiteSpace(ResponseDTO.payload.authentication.thaCountryCode) ? ResponseDTO.payload.authentication.thaCountryCode : "GB"; // If foreign App account, set a different country code
            Payload.TwoLetterISORegionName = "GB";
            Payload.FullName.FirstName = ResponseDTO.payload.authentication.firstName;
            Payload.FullName.Email = model.EmailAddress;
            Payload.ApiToken = ResponseDTO.payload.authentication.token;
            Payload.ApiTokenExpiry = ResponseDTO.payload.authentication.expiryDate.AddDays(3); //DateTime.Now.AddDays(2);
            Payload.ProductCodes = ResponseDTO.payload.authentication.productCodes.Select(x => Mapper.Map<AccountCodes>(x)).ToList();
            Payload.UserId = await AccountService.Get_UserId(model.EmailAddress);

            var ProductCodeRequest = new ProductCodeRequest { ProductCode = "THM" };
            var RequestDTO1 = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
            var ResponseDTO1 = await AccountService.GetAccountSummary(RequestDTO1);

            if (ResponseDTO1 == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
            }

            if (ResponseDTO1.errorCode != (int)Messages.NoProductFoundForUser)
            {
                string msisdn = ResponseDTO1.payload.userAccountSummary.productRef;
                if (Payload.ProductCodes.Where(x => x.ProductCode == "THM").Count() > 0)
                {
                    Payload.ProductCodes.Where(x => x.ProductCode == "THM").First().Reference = msisdn;
                }
            }

            if (ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
            {
                var Reference = "";

                try
                {
                    Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals("THM")).Select(x => x.Reference).First();
                }
                catch
                {
                    return ErrorRedirect(((int)Messages.ProductNotRegistered).ToString(), Urls.MyAccount + "THM"); // Product was not registered
                }
                Pay360GetAutoTopUpRequest autoTopUpRequest = new Pay360GetAutoTopUpRequest
                {
                    Msisdn = Reference,
                    Email = Payload.FullName.Email
                };
                var result = await Pay360Service.GetAutoTopUp(autoTopUpRequest);
                if (result != null && result.errorCode == 0 && result.payload != null)
                {
                    if (ResponseDTO1.payload.autoTopUpSummary == null)
                    {
                        ResponseDTO1.payload.autoTopUpSummary = new AutoTopUpSettingsRequestDTO
                        {
                            autoTopUp = result.payload.Status,
                            threshold = (decimal)result.payload.ThresHold,
                            topUpAmount = (decimal)result.payload.Topup,
                            msisdn = result.payload.Msisdn
                        };
                    }
                    else
                    {
                        ResponseDTO1.payload.autoTopUpSummary.autoTopUp = result.payload.Status;
                        ResponseDTO1.payload.autoTopUpSummary.threshold = (decimal)result.payload.ThresHold;
                        ResponseDTO1.payload.autoTopUpSummary.topUpAmount = (decimal)result.payload.Topup;
                    }
                }
            }
            /*
            Payload.ApiToken = "759CF611CEF545C2AA0C57172BB705D6";
            Payload.ApiTokenExpiry = DateTime.Now.AddDays(2);

            List<AccountCodes> acc = new List<AccountCodes>();
            acc.Add(new AccountCodes { Reference = "8944878255274716919", ProductCode = "THCC" });
            Payload.ProductCodes = acc;
            */
            Payload.AutoTopUpEnabled = ResponseDTO1.payload.autoTopUpSummary == null ? Convert.ToBoolean(ConfigurationManager.AppSettings["AutoTopUpEnabledByDefault"]) : ResponseDTO1.payload.autoTopUpSummary.autoTopUp;

            SetPayload(Payload);

            LoggerService.SendInfoAlert(string.Format("{0} {1}", "Successful log in for:", model.EmailAddress));
            LoggerService.Info(GetType(), string.Format("{0} {1}", "Successful log in for:", model.EmailAddress));

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else if (Payload.ProductCodes.Count > 0)
            {
                return Redirect(Urls.MyAccount);
            }
            else
            {
                return Redirect(Urls.RegisterProduct);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<JsonResult> LoginInternationalTopUpWidget(LoginRequest model)
        {
            var responseModel = new InternationalTopUpResponseModel();
            if (!ModelState.IsValid)
            {
                responseModel.Message = "Failure";
                return Json(responseModel);
            }
            var Payload = GetPayload();
            if (AccountService.IsAuthorized(Payload) && Payload.ProductCodes.Count > 0)
            {
                responseModel.Message = "User Already Logged In.";
                return Json(responseModel);
            }
            var RequestDTO = new LoginRequestDTO { email = model.EmailAddress, password = model.Password, ipAddress = GetRequestIP() };
            var ResponseDTO = await TalkHomeWebService.AuthenticateCustomer(RequestDTO);
            if (ResponseDTO == null)
            {
                responseModel.Message = "An error occurred when processing the request. Please try again.";
                return Json(responseModel);
            }
            else if (ResponseDTO.errorCode != 0 && Payload.HomeRoot != null && Payload.HomeRoot == "Homepage")
            {
                if (ResponseDTO.errorCode == (int)Messages.AuthenticationFailed || ResponseDTO.errorCode == 997)
                {
                    responseModel.Message = "The email or password provided do not match our records.";
                    return Json(responseModel);
                }
                else if (ResponseDTO.errorCode == (int)Messages.TooManyLogInAttempts)
                {
                    responseModel.Message = "There were too many failed login attempts. Please try again later";
                    return Json(responseModel);
                }
                else if (ResponseDTO.errorCode == 2)
                {
                    responseModel.Message = "We could not find that account. Have you registered that email address?";
                    return Json(responseModel);
                }
                else if (ResponseDTO.errorCode == (int)Messages.AccountNotConfirmed)
                {
                    responseModel.Message = "Please activate your account by following the link in your email.";
                    return Json(responseModel);
                }
                else
                {
                    responseModel.Message = "An unkwown error occurred while logging in. Please try again";
                    return Json(responseModel);
                }
            }
            else if (ResponseDTO.errorCode != 0)
            {
                await MigrationMail("Calling card user failed login", "email:" + model.EmailAddress + " pwd:" + model.Password);
                var LegacyAccount = await AccountService.LegacyCardUserExists(model.EmailAddress, model.Password);
                if (LegacyAccount.ErrorCode != 0)
                {
                    int errorCode = (int)Messages.AuthenticationFailed;
                    if (errorCode == (int)Messages.AuthenticationFailed || errorCode == 997)
                    {
                        responseModel.Message = "The email or password provided do not match our records.";
                        return Json(responseModel);
                    }
                    else if (errorCode == (int)Messages.TooManyLogInAttempts)
                    {
                        responseModel.Message = "There were too many failed login attempts. Please try again later";
                        return Json(responseModel);
                    }
                    else if (errorCode == 2)
                    {
                        responseModel.Message = "We could not find that account. Have you registered that email address?";
                        return Json(responseModel);
                    }
                    else if (errorCode == (int)Messages.AccountNotConfirmed)
                    {
                        responseModel.Message = "Please activate your account by following the link in your email.";
                        return Json(responseModel);
                    }
                    else
                    {
                        responseModel.Message = "An unkwown error occurred while logging in. Please try again";
                        return Json(responseModel);
                    }
                }
                else
                {
                    responseModel.Message = "Please Sign Up";
                    return Json(responseModel.Message);
                }
            }

            if (Payload.HomeRoot != null && Payload.HomeRoot != "Homepage")
            {
                MigrationMail("Successful Calling Card Login", model.EmailAddress);
            }
            // Payload.TwoLetterISORegionName = !string.IsNullOrWhiteSpace(ResponseDTO.payload.authentication.thaCountryCode) ? ResponseDTO.payload.authentication.thaCountryCode : "GB"; // If foreign App account, set a different country code
            Payload.TwoLetterISORegionName = "GB";
            Payload.FullName.FirstName = ResponseDTO.payload.authentication.firstName;
            Payload.FullName.Email = model.EmailAddress;
            Payload.ApiToken = ResponseDTO.payload.authentication.token;
            Payload.ApiTokenExpiry = ResponseDTO.payload.authentication.expiryDate; //DateTime.Now.AddDays(2);
            Payload.ProductCodes = ResponseDTO.payload.authentication.productCodes.Select(x => Mapper.Map<AccountCodes>(x)).ToList();

            var ProductCodeRequest = new ProductCodeRequest { ProductCode = "THM" };
            var RequestDTO1 = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
            var ResponseDTO1 = await AccountService.GetAccountSummary(RequestDTO1);

            if (ResponseDTO1 == null)
            {
                responseModel.Message = "An unkwown error occurred while logging in. Please try again";
                return Json(responseModel);
            }
            if (ResponseDTO1.errorCode != (int)Messages.NoProductFoundForUser)
            {
                string msisdn = ResponseDTO1.payload.userAccountSummary.productRef;
                if (Payload.ProductCodes.Where(x => x.ProductCode == "THM").Count() > 0)
                {
                    Payload.ProductCodes.Where(x => x.ProductCode == "THM").First().Reference = msisdn;
                }
            }

            if (ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
            {
                var Reference = "";

                try
                {
                    Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals("THM")).Select(x => x.Reference).First();
                }
                catch
                {
                    responseModel.StatusCode = (int)Messages.ProductNotRegistered;
                    responseModel.Message = "The product is not yet registered";
                    return Json(responseModel);

                }
                Pay360GetAutoTopUpRequest autoTopUpRequest = new Pay360GetAutoTopUpRequest
                {
                    Msisdn = Reference,
                    Email = Payload.FullName.Email
                };
                var result = await Pay360Service.GetAutoTopUp(autoTopUpRequest);
                if (result != null && result.errorCode == 0 && result.payload != null)
                {
                    if (ResponseDTO1.payload.autoTopUpSummary == null)
                    {
                        ResponseDTO1.payload.autoTopUpSummary = new AutoTopUpSettingsRequestDTO
                        {
                            autoTopUp = result.payload.Status,
                            threshold = (decimal)result.payload.ThresHold,
                            topUpAmount = (decimal)result.payload.Topup,
                            msisdn = result.payload.Msisdn
                        };
                    }
                    else
                    {
                        ResponseDTO1.payload.autoTopUpSummary.autoTopUp = result.payload.Status;
                        ResponseDTO1.payload.autoTopUpSummary.threshold = (decimal)result.payload.ThresHold;
                        ResponseDTO1.payload.autoTopUpSummary.topUpAmount = (decimal)result.payload.Topup;
                    }
                }
            }

            /*
            Payload.ApiToken = "759CF611CEF545C2AA0C57172BB705D6";
            Payload.ApiTokenExpiry = DateTime.Now.AddDays(2);

            List<AccountCodes> acc = new List<AccountCodes>();
            acc.Add(new AccountCodes { Reference = "8944878255274716919", ProductCode = "THCC" });
            Payload.ProductCodes = acc;
            */
            Payload.AutoTopUpEnabled = ResponseDTO1.payload.autoTopUpSummary == null ? Convert.ToBoolean(ConfigurationManager.AppSettings["AutoTopUpEnabledByDefault"]) : ResponseDTO1.payload.autoTopUpSummary.autoTopUp;

            SetPayload(Payload);

            LoggerService.SendInfoAlert(string.Format("{0} {1}", "Successful log in for:", model.EmailAddress));
            LoggerService.Info(GetType(), string.Format("{0} {1}", "Successful log in for:", model.EmailAddress));

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
            {
                responseModel.Url = model.ReturnUrl;
                return Json(responseModel);
            }
            else if (Payload.ProductCodes.Count > 0)
            {
                if (!AccountService.IsAuthorized(Payload))
                {
                    responseModel.Message = "Please Log In.";
                    return Json(responseModel);
                }
                else
                {
                    var PayloadResponse = GetPayload();
                    var jsonpayload = JsonConvert.SerializeObject(PayloadResponse);
                    responseModel.Message = jsonpayload;
                    responseModel.Url = Urls.MyAccount;
                    return Json(responseModel);
                }

            }
            else
            {
                responseModel.Url = Urls.RegisterProduct;
                return Json(responseModel);
            }
        }

        /// <summary>
        /// My Account page view. Changes depending on the customer's active/requested products
        /// </summary>
        /// <returns>The view</returns>
        [ApiAuthentication]
        public async Task<ActionResult> MyDashboard()
        {
            var Payload = GetPayload();

            if (Payload.ProductCodes.Count == 0)
            {
                return Redirect(Urls.RegisterProduct); // No products found, add one first
            }

            return View(new MyDashboardViewModel(Payload));

        }


        private async Task MigrationMail(string action, string email)
        {

            Dictionary<string, string> substitutions = new Dictionary<string, string>();

            substitutions.Add("%EMAIL%", email);
            substitutions.Add("%ACTION%", action);

            MailTemplate mailTemplate = new MailTemplate
            {
                Template = MailTemplate.MIGRATION_TEMPLATE,
                EmailAddress = "asharp@nowtel.co.uk",
                Substitutions = substitutions,
                From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                Subject = "Calling Card Migration " + action
            };

            try
            {
                await mailTemplate.Send();
            }

            catch (Exception e)
            {
                LoggerService.Error(GetType(), e.Message, e);
            }


        }


        /// <summary>
        /// My Account page view. Changes depending on the customer's active/requested products
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <returns>The view</returns>

        //[ApiAuthentication]
        [ApiAuthentication]
        public async Task<ActionResult> MyAccount(string productCode)
        {
            var Payload = GetPayload();

            if (Payload.ProductCodes.Count == 0)
            {
                return Redirect(Urls.RegisterProduct); // No products found, add one first
            }

            if (string.IsNullOrEmpty(productCode))
            {
                //Default to mobile tab if user has that producr
                bool found = false;
                string pCode = "THM";
                foreach (var p in Payload.ProductCodes)
                {
                    if (p.ProductCode == pCode)
                    {
                        productCode = p.ProductCode;
                        found = true;
                        break;
                    }

                }

                if (!found)
                {
                    productCode = pCode;
                }

            }
            Payload.OpenRegistration = productCode;
            SetPayload(Payload);

            var ProductCodeRequest = new ProductCodeRequest { ProductCode = productCode };

            if (!Validator.TryValidateObject(ProductCodeRequest, new ValidationContext(ProductCodeRequest, null, null), null, true))
            {
                productCode = Payload.ProductCodes[0].ProductCode; // Product code is valid. End of validation
            }

            var RequestDTO = new AccountSummaryRequestDTO { productCode = productCode, token = Payload.ApiToken };
            var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
            }

            if (ResponseDTO.errorCode == (int)Messages.NoProductFoundForUser)
            {
                return Redirect(Urls.RegisterProduct); // No products found, add one first
                                                       // return View(new MyAccountViewModel(Payload, productCode, null, null)); // The user hasn't registered this product. Tell the view to display `Add product`
            }

            if (productCode.Equals(Models.Enums.ProductCodes.THCC.ToString()))
            {
                return View(new MyAccountViewModel(Payload, productCode, ResponseDTO.payload, null)); // No call records are needed for calling cards
            }

            var CallRecords = await TalkHomeWebService.GetCallHistoryPage(new HistoryPageDTO(productCode, 1, 7, Payload.ApiToken));


            return View(new MyAccountViewModel(Payload, productCode, ResponseDTO.payload, CallRecords.payload));
        }

        /// <summary>
        /// The settings page
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <returns>The view</returns>
        [ApiAuthentication]
        public async Task<ActionResult> Settings(string productCode)
        {
            var Payload = GetPayload();


            if (Payload.ProductCodes.Count == 0)
            {

                //return Redirect(Urls.RegisterProduct); // No products found, add one first
                return Json(new { errorcode = 1, State = "RegisterProduct", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.ProductNotRegistered).ToString())), Url = Urls.RegisterProduct }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(productCode))
            {
                productCode = Payload.ProductCodes[0].ProductCode; // Plain redirection to Settings page, get the first product
            }

            var ProductCodeRequest = new ProductCodeRequest { ProductCode = productCode };

            if (!Validator.TryValidateObject(ProductCodeRequest, new ValidationContext(ProductCodeRequest, null, null), null, true))
            {
                productCode = Payload.ProductCodes[0].ProductCode; // Product code is valid. End of validation
            }

            var RequestDTO = new AccountSummaryRequestDTO { productCode = productCode, token = Payload.ApiToken };
            var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

            if (ResponseDTO == null)
            {
                //return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings + productCode);// Request failed
                return Json(new { errorcode = 2, State = "AnErrorOccurred", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())) }, JsonRequestBehavior.AllowGet);
            }

            if (ResponseDTO.errorCode == (int)Messages.NoProductFoundForUser)
            {
                // return ErrorRedirect(((int)Messages.ProductNotRegistered).ToString(), Urls.MyAccount + productCode);
                return Json(new { errorcode = 3, State = "ProductNotRegistered", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.ProductNotRegistered).ToString())) }, JsonRequestBehavior.AllowGet);
            }

            if (ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
            {
                var Reference = "";

                try
                {
                    Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(productCode)).Select(x => x.Reference).First();
                }
                catch
                {
                    //return ErrorRedirect(((int)Messages.ProductNotRegistered).ToString(), Urls.MyAccount + productCode); // Product was not registered
                    return Json(new { errorcode = 3, State = "ProductNotRegistered", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.ProductNotRegistered).ToString())) }, JsonRequestBehavior.AllowGet);
                }
                Pay360GetAutoTopUpRequest autoTopUpRequest = new Pay360GetAutoTopUpRequest
                {
                    Msisdn = Reference,
                    Email = Payload.FullName.Email
                };
                var result = await Pay360Service.GetAutoTopUp(autoTopUpRequest);
                if (result != null && result.errorCode == 0 && result.payload != null)
                {
                    if (ResponseDTO.payload.autoTopUpSummary == null)
                    {
                        ResponseDTO.payload.autoTopUpSummary = new AutoTopUpSettingsRequestDTO
                        {
                            autoTopUp = result.payload.Status,
                            threshold = (decimal)result.payload.ThresHold,
                            topUpAmount = (decimal)result.payload.Topup,
                            msisdn = result.payload.Msisdn
                        };
                    }
                    else
                    {
                        ResponseDTO.payload.autoTopUpSummary.autoTopUp = result.payload.Status;
                        ResponseDTO.payload.autoTopUpSummary.threshold = (decimal)result.payload.ThresHold;
                        ResponseDTO.payload.autoTopUpSummary.topUpAmount = (decimal)result.payload.Topup;
                    }
                }
                else
                {
                    return Json(new { errorcode = 1, Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())) }, JsonRequestBehavior.AllowGet);
                }
            }

            var TopUps = ContentService.GetAllTopUps();
            if (Request.IsAjaxRequest())
            {
                return Json(new { View = RenderRazorViewToString(ControllerContext, "AutoTopUpSetting", new SettingsViewModel { Payload = Payload, ProductCode = productCode, AccountSummary = ResponseDTO.payload, TopUps = TopUps[productCode] }), errorcode = 0, }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { URL = Urls.MyAccount, Message = "Success" });

            }
        }

        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            controllerContext.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var ViewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var ViewContext = new ViewContext(controllerContext, ViewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                ViewResult.View.Render(ViewContext, sw);
                ViewResult.ViewEngine.ReleaseView(controllerContext, ViewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public async Task<ActionResult> GenericSignUp(SignUpRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.SignUp);
            }

            var Payload = GetPayload();

            //Payload.FullName.FirstName = model.FirstName;
            //Payload.FullName.LastName = model.LastName;
            //Payload.FullName.Email = model.EmailAddress;

            var RequestDTO = Mapper.Map<SignUpRequestDTO>(model);

            var ResponseDTO = await TalkHomeWebService.SignUp(RequestDTO);


            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.SignUp);
            }

            if (ResponseDTO.errorCode != 0 || !ResponseDTO.payload.signUp.isRegistered)
            {
                return FailedSignUp(model.EmailAddress, ResponseDTO.errorCode);
            }


            //Do the subscription 
            if (model.SubscribeSignUp || model.LegacyMigration == true)
            {
                string listId = "";

                listId = "18";

                //var ACResult = await ActiveCampaignService.AddToList(listId, model.EmailAddress, model.FirstName, model.LastName);

                //if (ACResult != null && ACResult.result_code == 0)
                //{
                //    LoggerService.Info(GetType(), ACResult.result_message);
                //}

                //if (!model.LegacyMigration)
                //{
                //    ACResult = await ActiveCampaignService.AddTag("GDPR Compliant", model.EmailAddress);

                //    if (ACResult != null && ACResult.result_code == 0)
                //    {
                //        LoggerService.Info(GetType(), ACResult.result_message);
                //    }
                //}

            }

            //end subscription calls
            //Get a verification token
            string verifyLink = System.Configuration.ConfigurationManager.AppSettings["SignUpConfirmationPageUrl"];

            if (!model.LegacyMigration)
            {
                Dictionary<string, string> substitutions = new Dictionary<string, string>();

                substitutions.Add("%VERIFY_LINK%", String.Format(verifyLink, ResponseDTO.payload.signUp.token));
                substitutions.Add("%FIRST_NAME%", model.FirstName);
                MailTemplate mailTemplate = new MailTemplate
                {
                    Template = MailTemplate.VERIFY_EMAIL_TEMPLATE,
                    EmailAddress = model.EmailAddress,
                    Substitutions = substitutions,
                    From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                    Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                    Subject = "Click to activate your account"
                };

                try
                {
                    await mailTemplate.Send();
                }

                catch (Exception e)
                {
                    LoggerService.Error(GetType(), e.Message, e);
                }
            }


            if (model.LegacyMigration)
            {
                //Quiet verification    
                var ResponseDTO3 = await AccountService.VerifySignUpEmail(ResponseDTO.payload.signUp.token);
                if (ResponseDTO3 == null)
                {
                    await MigrationMail("Error Verifying Signing Up", model.EmailAddress);
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.SignUp);
                }


                if (ResponseDTO3.errorCode != 0 || !ResponseDTO3.payload.isConfirmed)
                {
                    return FailedAccountVerification(ResponseDTO.errorCode);
                }

                //Quiet Login
                var RequestDTO2 = new LoginRequestDTO { email = model.EmailAddress, password = model.Password, ipAddress = GetRequestIP() };
                var ResponseDTO2 = await TalkHomeWebService.AuthenticateCustomer(RequestDTO2);

                if (ResponseDTO2 == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Login);
                }

                Payload.ApiToken = ResponseDTO2.payload.authentication.token;
                Payload.ApiTokenExpiry = ResponseDTO2.payload.authentication.expiryDate;

                //Quiet Product Add
                var RequestDTO1 = new AddProductRequestDTO { number = model.LegacyCallingCardNumber, code = model.LegacyPinNumber, productCode = "THCC" };
                var ResponseDTO1 = await TalkHomeWebService.AddProduct(RequestDTO1, Payload.ApiToken);

                if (ResponseDTO1 == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.ConfirmProductDetails);
                }

                if (ResponseDTO1.errorCode != 0)
                {
                    return FailedAddProduct(ResponseDTO.errorCode, model.LegacyCallingCardNumber);
                }

                await MigrationMail("Calling Card Producted Added", model.EmailAddress);
                Payload.ProductCodes.Add(new AccountCodes { Reference = model.LegacyCallingCardNumber, ProductCode = "THCC" });
                Payload.OpenSignUp = false;
                SetPayload(Payload);
                return Redirect(Urls.AccountDetails + "/THCC");

            }

            else
            {
                Payload.OpenSignUp = true;
                SetPayload(Payload);
                return Redirect(Urls.SuccessfulSignUp);
            }


        }

        /// <summary>
        /// Create an account form submission. Checks validity of request and forwards to add a product view
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The success view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> SignUp(SignUpRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.SignUp);
            }

            return RedirectToAction("GenericSignUp", model);
        }

        /// <summary>
        /// Verifies the sign up token validity
        /// </summary>
        /// <returns>The view</returns>
        public async Task<ActionResult> Verification(string productCode)
        {
            var Token = productCode;

            if (Token == null)
            {
                return ErrorRedirect(((int)Messages.InvalidResetToken).ToString(), Urls.SignUp);
            }

            var ResponseDTO = await AccountService.VerifySignUpEmail(Token);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.SignUp);
            }

            if (ResponseDTO.errorCode != 0 || !ResponseDTO.payload.isConfirmed)
            {
                return FailedAccountVerification(ResponseDTO.errorCode);
            }

            var Payload = GetPayload();

            Payload.OpenSignUp = false;
            SetPayload(Payload);
            return SuccessRedirect(((int)Messages.VerifiedSignUp).ToString(), Urls.Login, Urls.RegisterProduct);
        }

        /// <summary>
        /// allows a registered user to add a new product from Myaccount page
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        [ApiAuthentication]
        public ActionResult AddProductPage(ProductCodeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.MyAccount);
            }

            var Payload = GetPayload();

            Payload.OpenRegistration = model.ProductCode;
            SetPayload(Payload);
            return Redirect(Urls.ConfirmProductDetails);
        }

        /// <summary>
        /// Validates the request to add a product to an account
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        [ApiAuthentication]
        public async Task<ActionResult> AddProductDetails(AddProductRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.ConfirmProductDetails);
            }

            var Payload = GetPayload();

            if (AccountService.IsAlreadyActive(model.ProductCode, Payload))
            {
                return HandleRedirect(((int)Messages.AlreadyActive).ToString(), Urls.MyAccount);
            }

            string Reference = "";

            if (!model.ProductCode.Equals(Models.Enums.ProductCodes.THCC.ToString()))
            {
                if (!AccountService.TryValidateNumber(model.Number, Payload.TwoLetterISORegionName, out Reference))
                {
                    return ErrorRedirect(((int)Messages.InvalidNumber).ToString(), Urls.ConfirmProductDetails);

                }
            }

            var RequestDTO = new AddProductRequestDTO { number = Reference.Equals("") ? model.Number : Reference, code = model.Code, productCode = model.ProductCode };
            var ResponseDTO = await TalkHomeWebService.AddProduct(RequestDTO, Payload.ApiToken);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.ConfirmProductDetails);
            }

            if (ResponseDTO.errorCode != 0)
            {
                return FailedAddProduct(ResponseDTO.errorCode, model.Number);
            }


            //var CreditSimFullfillmentResponse = await TalkHomeWebService.CreditSimFullfillment(model.Number);
            //if (CreditSimFullfillmentResponse == null)
            //{
            //    return ErrorRedirect(((int)Messages.CreditSimFullfillmentError).ToString(), Urls.ConfirmProductDetails);
            //}

            Payload.OpenSignUp = false;
            Payload.ProductCodes.Add(new AccountCodes { Reference = Reference.Equals("") ? model.Number : Reference, ProductCode = model.ProductCode });

            SetPayload(Payload);
            return SuccessRedirect(((int)Messages.AddProductSuccess).ToString(), Urls.MyAccount + "/" + model.ProductCode);
        }



        [HttpPost]
        [HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        [ApiAuthentication]
        public async Task<JsonResult> AddProductDetailsInternationalTopUp(AddProductRequest model)
        {
            var responseModel = new InternationalTopUpResponseModel();
            if (!ModelState.IsValid)
            {
                responseModel.Message = "Model State Is Invalid";
                return Json(responseModel);
            }

            var Payload = GetPayload();

            if (AccountService.IsAlreadyActive(model.ProductCode, Payload))
            {
                responseModel.Message = "This account has already registered this product. Please visit the page of your account if you wish to make any changes";

                return Json(responseModel, JsonRequestBehavior.AllowGet);
            }

            string Reference = "";

            if (!model.ProductCode.Equals(Models.Enums.ProductCodes.THCC.ToString()))
            {
                if (!AccountService.TryValidateNumber(model.Number, Payload.TwoLetterISORegionName, out Reference))
                {
                    responseModel.Message = "The number provided was not valid";
                    return Json(responseModel, JsonRequestBehavior.AllowGet);
                }
            }

            var RequestDTO = new AddProductRequestDTO { number = Reference.Equals("") ? model.Number : Reference, code = model.Code, productCode = model.ProductCode };
            var ResponseDTO = await TalkHomeWebService.AddProduct(RequestDTO, Payload.ApiToken);

            if (ResponseDTO == null)
            {
                responseModel.Message = "An Internal Server Error Occurred";

                return Json(responseModel, JsonRequestBehavior.AllowGet);
            }

            if (ResponseDTO.errorCode != 0)
            {
                if (ResponseDTO.errorCode == (int)Messages.InvalidMsisdnOrPUK)
                {
                    LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.InvalidMsisdnOrPUK.ToString(), "for", model.Number));
                    LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.InvalidMsisdnOrPUK.ToString(), "for", model.Number));

                    responseModel.Message = "That phone number or PUK code didn't match our records. Please double check them and try again";
                    return Json(responseModel, JsonRequestBehavior.AllowGet);
                }
                else if (ResponseDTO.errorCode == (int)Messages.InvalidMsisdnOrPIN)
                {
                    LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.InvalidMsisdnOrPIN.ToString(), "for", model.Number));
                    LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.InvalidMsisdnOrPIN.ToString(), "for", model.Number));

                    responseModel.Message = "That phone number or PIN didn't match our records. Please double check them and try again";
                    return Json(responseModel, JsonRequestBehavior.AllowGet);
                }
                else if (ResponseDTO.errorCode == (int)Messages.ProductAlreadyAdded)
                {
                    LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.ProductAlreadyAdded.ToString(), "for", model.Number));
                    LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.ProductAlreadyAdded.ToString(), "for", model.Number));
                    responseModel.Message = "A product with those credentials has already been registered";
                    return Json(responseModel, JsonRequestBehavior.AllowGet);
                }

                else if (ResponseDTO.errorCode == (int)Messages.UnknownAccount)
                {
                    LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.UnknownAccount.ToString(), "for", model.Number));
                    LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.UnknownAccount.ToString(), "for", model.Number));
                    responseModel.Message = "The details provided were invalid. Please double check them and try again";
                    return Json(responseModel, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.AddProductFailed.ToString(), "for", model.Number));
                    LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.AddProductFailed.ToString(), "for", model.Number));
                    responseModel.Message = "There was a temporary problem when registering that product. Please try again";

                    return Json(responseModel, JsonRequestBehavior.AllowGet);
                }
            }

            //var CreditSimFullfillmentResponse = await TalkHomeWebService.CreditSimFullfillment(model.Number);
            //if (CreditSimFullfillmentResponse == null)
            //{
            //    responseModel.Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.CreditSimFullfillmentError).ToString()));
            //    return Json(responseModel, JsonRequestBehavior.AllowGet);
            //}


            Payload.OpenSignUp = false;
            Payload.ProductCodes.Add(new AccountCodes { Reference = Reference.Equals("") ? model.Number : Reference, ProductCode = model.ProductCode });

            SetPayload(Payload);
            responseModel.StatusCode = 1;
            responseModel.Message = "The product was successfully added to your account";

            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Clears values of API-related fields in the JWT token
        /// </summary>
        /// <returns>The homepage</returns>
        [ActionName("log-out")]
        [ApiAuthentication]
        public ActionResult LogOut()
        {
            var Payload = GetPayload();


            Payload.ApiToken = null;
            Payload.ApiTokenExpiry = new DateTime();
            Payload.ProductCodes.Clear();
            Payload.Checkout = null;
            Payload.Payment = null;
            Payload.OneClick = null;
            Payload.CustId = null;
            Payload.Basket.Clear();
            Payload.FullName.Email = null;
            Payload.AutoTopUpEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoTopUpEnabledByDefault"]);
            SetPayload(Payload);

            return Redirect("/");
        }



        private void SetTempData(MailOrderRequest model)
        {
            TempData["FirstName"] = model.FirstName;
            TempData["LastName"] = model.LastName;
            TempData["EmailAddress"] = model.EmailAddress;
            TempData["Register"] = "yes";
            TempData["AddressLine1"] = model.AddressLine1;
            TempData["AddressLine2"] = model.AddressLine2;
            TempData["CountyOrProvince"] = model.CountyOrProvince;
            TempData["City"] = model.City;
            TempData["CountryCode"] = model.CountryCode;
            TempData["PostalCode"] = model.PostalCode;
            TempData["SimSwap"] = model.IsSimSwap;
            //TempData["PortDate"] = model.PortDate;
            TempData["PortInMsisdn"] = model.PortInMsisdn;
            TempData["PAC"] = model.PAC;
            TempData["ThaMsisdn"] = model.ThaMsisdn;

            if (model.SignUp == "on")
            {
                TempData["SignUp"] = model.SignUp;
                TempData["Register"] = "yes";
            }
        }

        /// <summary>
        /// Forwards the POST data to the API from the mail order request
        /// </summary>
        /// <param name="model">The model for the posted data.</param>
        /// <param name="slug">The prodcu being ordered.</param>
        /// <returns>Redirects in case of success or failure. Successful transactions are redirected to custom-designed Umbraco pages.</returns>
        [HttpPost]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<string> AddTagTest()
        {
            var ACResult = await ActiveCampaignService.AddTag("asharp@nowtel.co.uk", "SIMINACTIVE");
            return "";

        }


        /// <summary>
        /// Forwards the POST data to the API from the mail order request
        /// </summary>
        /// <param name="model">The model for the posted data.</param>
        /// <param name="slug">The prodcu being ordered.</param>
        /// <returns>Redirects in case of success or failure. Successful transactions are redirected to custom-designed Umbraco pages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> MailOrder(MailOrderRequest model)
        {

            if (!ModelState.IsValid)
            {
                SetTempData(model);
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()) ? Urls.OrderSIM : Urls.OrderCallingCard);
            }
            if (!string.IsNullOrEmpty(model.FromOfferLandingPage) && model.FromOfferLandingPage.ToLower() == "true")
            {
                ViewBag.FromOffer = true;
                TempData["FromOffer"] = true;
            }

            var Payload = GetPayload();

            ////check is user otp verified

            var isuser_otp_verified = await AccountService.Verifyuseremail_against_otpAsync(model.EmailAddress);
            if (isuser_otp_verified == false )
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.OrderSIM);
            }

            //string EmailVerificationToken = "";

            if (!string.IsNullOrEmpty(model.Password))
            {

                SignUpRequestDTO RequestDTO = new SignUpRequestDTO
                {
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    email = model.EmailAddress,
                    password = model.Password,
                    confirmPassword = model.ConfirmPassword,
                    isSubscribedToNewsletter = model.SubscribeSignUp,
                    TermsAndConditions = true
                };

                var ResponseDTO = await TalkHomeWebService.SignUp(RequestDTO);



                if (ResponseDTO == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.SignUp);
                }

                if (ResponseDTO.errorCode != 0)
                {
                    return FailedSignUp(model.EmailAddress, ResponseDTO.errorCode);
                }

                if (ResponseDTO.errorCode == 0)
                {
                    var confirmuser = await TalkHomeWebService.VerifySignUpEmail(ResponseDTO.payload.signUp.token);
                    model.SignUp = "on";
                    TempData["UserExists"] = "yes";
                }


                //EmailVerificationToken = ResponseDTO.payload.signUp.token;

                Payload.OpenSignUp = true;
                TempData["SignUp"] = "on";

            }
            else if (model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()))
            {
                var UserExists = await AccountService.UserExists(model.EmailAddress);



                if (UserExists == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()) ? Urls.OrderSIM : Urls.OrderCallingCard);
                }



                //// if user has ordered SIM
                //if (model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()))
                //{
                //    //ensure they can't create another SIM
                //    if (UserExists.UserExists && UserExists.HasSim)
                //    {
                //        return ErrorRedirect(((int)Messages.UserHasSim).ToString(), Urls.OrderSIM);
                //    }
                //    else if (UserExists.UserExists)
                //    {
                //        TempData["UserExists"] = "yes";
                //    }
                //}
            }


            if (model.CountyOrProvince == null || string.IsNullOrEmpty(model.CountyOrProvince) || (model.CountyOrProvince != null && model.CountyOrProvince.Trim() == ""))
            {
                if (model.City == null || string.IsNullOrEmpty(model.City) || (model.City != null && model.City.Trim() == ""))
                {
                    model.City = "Un-known";
                }
                model.CountyOrProvince = model.City;

            }

            var Request = Mapper.Map<MailOrderRequestDTO>(model);


            if (String.IsNullOrEmpty(model.ThaMsisdn))
            {
                var Result = await AccountService.InsertSimOrder(Request);
                if (Result == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()) ? Urls.OrderSIM : Urls.OrderCallingCard);
                }
                if (Result.errorCode != 0)
                {
                    return ErrorRedirect(((int)Messages.MailOrderError).ToString(), model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()) ? Urls.OrderSIM : Urls.OrderCallingCard);

                }

                TempData["OrderId"] = Result.OrderId;

                if (model.SignUp == "on")
                {
                    await OrderSimEmail(model.EmailAddress);
                    return Redirect(model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()) ? Url.Action("SimOrderSuccess", "CustomPage") : Urls.SuccessfulCardOrder);
                }
                else
                {
                    await OrderSimEmail(model.EmailAddress);
                }

                if (model.PortInMsisdn != null)
                {
                    PortInRequestModel port = new PortInRequestModel
                    {
                        Email = model.EmailAddress,
                        Code = model.PAC,
                        PortMsisdn = model.PortInMsisdn,
                        OrderRefId = Result.reference
                    };

                    var portInResponse = await PortService.PortIn(port, PortTypes.PortInNew);
                    if (!portInResponse.payload)
                    {
                        return ErrorRedirect(((int)Messages.MailOrderSuccessPortInfail).ToString(), Urls.OrderSIM);
                    }
                }
            }
            else
            {
                var Result = await AccountService.SimPromoOrder(model);

                if (Result == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.OrderSIM);
                }

                if (Result.ErrorCode != 0)
                {
                    return ErrorRedirect(((int)Messages.MailOrderError).ToString(), Urls.OrderSIM);
                }

            }


            LoggerService.Info(GetType(), string.Format("{0} {1}", "Successful mail order for:", model.EmailAddress));
            //Payload.FullName.FirstName = Request.firstName;
            TempData["FirstName"] = Request.firstName;
            Payload.MailOrder = Request.product;
            SetPayload(Payload);

            return Redirect(model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()) ? Url.Action("SimOrderSuccess", "CustomPage") : Urls.SuccessfulCardOrder);
        }

        [HttpPost]
        [Route("Verifysimorder")]
        public async Task<ActionResult> Verifysimorder(string EmailAddress, string FirstName)
        {
            if (EmailAddress != null)
            {
                var UserExists = await AccountService.UserExists(EmailAddress);
                var UserDetails = await AccountService.GetUserByEmail(EmailAddress);
                var Isvalid_simorder = AccountService.Isvalid_simorder(EmailAddress);
                if (UserExists == null)
                {
                    return Json(new { status = false });
                }

                //If user does not exist force them to register (nicely)
                if (!UserExists.UserExists && UserDetails != "True" && Isvalid_simorder != 1 && Isvalid_simorder != 2)
                {
                    var result = await Send_otp_to_userAsync(EmailAddress, FirstName);
                    if (result != null)
                    {
                        return Json(new { status = true });
                    }
                    else
                    {
                        return Json(new { status = false });
                    }
                }
                if (Isvalid_simorder == 1)
                {
                    return Json(new { status = "limitexceeds" });
                }
                else
                {
                    return Json(new { status = false });
                }
            }
            else
            {
                return Json(new { status = false });
            }

        }


        [HttpPost]
        [Route("Verifyusersimorder")]
        public async Task<ActionResult> MailOrderRequestValidation(MailOrderValidationRequest model)
        {
            //Check Sim Order Validations
            var SimOrderValidations = await AccountService.SImOrderValidations(model.EmailAddress, model.Address, model.PostCode);
            if (SimOrderValidations > 0)
            {

                return Json(new { isValid = false, errorcode = SimOrderValidations }, JsonRequestBehavior.AllowGet);
            }

            var response = await AccountService.IsEmailRegistered(model.EmailAddress);
            if (response == 3) //user not registered
            {
                //Send OTP
                await Send_otp_to_userAsync(model.EmailAddress, "");
                return Json(new { isValid = true, regsitered = false, confirmed = false }, JsonRequestBehavior.AllowGet);
            }
            else if (response == 2) //user not confirmed
            {
                //Send OTP
                await Send_otp_to_userAsync(model.EmailAddress, "");
                return Json(new { isValid = true, regsitered = true, confirmed = false }, JsonRequestBehavior.AllowGet);
            }

            //Create Sim Order
            return Json(new { isValid = true, regsitered = true, confirmed = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Verifyuserotp")]
        public async Task<JsonResult> Verify_user_otp(string EmailAddress, string digit1, string digit2, string digit3, string digit4)

        {
            string user_otp = (digit1 + digit2 + digit3 + digit4).ToString();
            var result = await AccountService.Verify_user_otp(Convert.ToInt32(user_otp), 10, EmailAddress);
            if (result == 1)
            {
                return Json(new { response = true });
            }
            else
            {
                return Json(new { response = false });
            }
        }

        [HttpPost]
        [Route("Send_otp_to_user")]
        public async Task<int> Send_otp_to_userAsync(string email, string firstname)
        {
            Dictionary<string, string> substitutions = new Dictionary<string, string>();
            //Get a verification token
            int otp = GenerateRandomNo();

            substitutions.Add("%PIN%", otp.ToString());
            MailTemplate mailTemplate = new MailTemplate
            {
                Template = MailTemplate.otp_TEMPLATE,
                EmailAddress = email,
                Substitutions = substitutions,
                From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                Subject = "Verification code"
            };
            await mailTemplate.Send();

            var save_otp = AccountService.Insert_otp_data(Convert.ToInt32(otp), email);

            return 1;
        }
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        /// <summary>
        /// Gets the details of the authorized user and returns the details page.
        /// </summary>
        /// <returns>The view with the required ViewModel.</returns>
        [ActionName("account-details")]
        [ApiAuthentication]
        public async Task<ActionResult> AccountDetials(string productCode)
        {
            var Payload = GetPayload();

            if (Payload.ProductCodes.Count == 0)
            {
                return Redirect(Urls.RegisterProduct);
            }

            if (string.IsNullOrEmpty(productCode))
            {
                productCode = Payload.ProductCodes[0].ProductCode; // Plain redirection to MyAccount, get summery for the first product
            }

            var ResponseDTO = await TalkHomeWebService.GetAccountDetails(Payload.ApiToken);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage);
            }

            var Countries = AccountService.GetCountryList();

            //var ACResult = await ActiveCampaignService.GetContactDetails(ResponseDTO.payload.email);

            bool subscribed = false;
            string acId = "";

            //if (ACResult != null && ACResult.result_code == 0)
            //{
            //    LoggerService.Info(GetType(), ACResult.result_message);
            //}
            //else
            //{
            //    var lists = ACResult.listslist;

            //    var alist = lists.Split('-');

            //    acId = ACResult.id;

            //    if (ACResult != null && ACResult.result_code == 0)
            //    {
            //        LoggerService.Info(GetType(), ACResult.result_message);
            //    }

            //    if (alist.Where(x => x.ToString() == "18").Count() == 1)
            //    {
            //        if (int.Parse(ACResult.lists["18"].SelectToken("status").ToString()) == (int)TalkHome.Services.SubscriptionStatus.Active)
            //        {
            //            subscribed = true;
            //        }
            //    }
            //}



            //GetContactDetails

            bool UKAddress = true;
            if (!Payload.TwoLetterISORegionName.Equals("GB"))
            {
                UKAddress = false;
            }

            var HomeAddress = new AddressDetailsViewModel(Countries, ResponseDTO.payload.addresses.home ?? new AddressModel(), UKAddress);
            var BillingAddress = new AddressDetailsViewModel(Countries, ResponseDTO.payload.addresses.billing ?? new AddressModel(), UKAddress);

            return View(new AccountDetailsViewModel(ResponseDTO.payload, productCode, HomeAddress, BillingAddress, Payload, subscribed, acId));
        }




        //[ApiAuthentication]
        [HttpGet]
        public async Task<ActionResult> EditAccountDetail(string productCode)
        {
            var Payload = GetPayload();

            if (Payload.ProductCodes.Count == 0)
            {
                //return Redirect(Urls.RegisterProduct);
                return Json(new { errorcode = 1, State = "RegisterProduct", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.ProductNotRegistered).ToString())), Url = Urls.RegisterProduct }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(productCode))
            {
                productCode = Payload.ProductCodes[0].ProductCode; // Plain redirection to MyAccount, get summery for the first product
            }

            var ResponseDTO = await TalkHomeWebService.GetAccountDetails(Payload.ApiToken);

            if (ResponseDTO == null)
            {
                return Json(new { errorcode = 1, State = "RegisterProduct", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())), Url = Urls.RegisterProduct }, JsonRequestBehavior.AllowGet);
                //return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage);
            }

            var Countries = AccountService.GetCountryList();

            //var ACResult = await ActiveCampaignService.GetContactDetails(ResponseDTO.payload.email);

            bool subscribed = false;
            string acId = "";

            //if (ACResult != null && ACResult.result_code == 0)
            //{
            //    LoggerService.Info(GetType(), ACResult.result_message);
            //}
            //else if(ACResult != null)
            //{
            //    var lists = ACResult.listslist;

            //    var alist = lists.Split('-');

            //    acId = ACResult.id;

            //    if (ACResult != null && ACResult.result_code == 0)
            //    {
            //        LoggerService.Info(GetType(), ACResult.result_message);
            //    }

            //    if (alist.Where(x => x.ToString() == "18").Count() == 1)
            //    {
            //        if (int.Parse(ACResult.lists["18"].SelectToken("status").ToString()) == (int)TalkHome.Services.SubscriptionStatus.Active)
            //        {
            //            subscribed = true;
            //        }
            //    }
            //}

            //GetContactDetails

            bool UKAddress = true;
            if (!Payload.TwoLetterISORegionName.Equals("GB"))
            {
                UKAddress = false;
            }

            var HomeAddress = new AddressDetailsViewModel(Countries, ResponseDTO.payload.addresses.home ?? new AddressModel(), UKAddress);
            var BillingAddress = new AddressDetailsViewModel(Countries, ResponseDTO.payload.addresses.billing ?? new AddressModel(), UKAddress);
            if (Request.IsAjaxRequest())
            {
                return Json(new { View = RenderRazorViewToString(ControllerContext, "EditAccountDetail", new AccountDetailsViewModel(ResponseDTO.payload, productCode, HomeAddress, BillingAddress, Payload, subscribed, acId)), errorcode = 0, }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View(new AccountDetailsViewModel(ResponseDTO.payload, productCode, HomeAddress, BillingAddress, Payload, subscribed, acId));
            }
        }
        /// <summary>
        /// Verifies the phone or card number to top up
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> VerifyNumber(VerifyNumberRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.VerifyNumber);
            }

            var Payload = GetPayload();

            var Msisdn = "";
            var RechargeNumber = "";

            if (Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THM.ToString()) || Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THA.ToString()))
            {
                var Verify = await PaymentService.VerifyNumber(Payload, model.Number, model.CountryCode);

                if (!Verify)
                {
                    return ErrorRedirect(((int)Messages.NotVerifiedNumber).ToString(), Urls.VerifyNumber);
                }

                try
                {
                    Msisdn = AccountService.GetMsisdnFromNumber(model.Number, model.CountryCode);
                }
                catch (Exception)
                {
                    // Calling cards numbers will always fail, it's expected. Proceed
                }
            }
            else
            {
                int error = 0;
                RechargeNumber = AccountService.GetRechargeableNumber(model.Number, out error);
                if (error == 2)
                {
                    return ErrorRedirect(((int)Messages.MoreThanOnePin).ToString(), Urls.VerifyNumber);
                }
                else if (String.IsNullOrEmpty(RechargeNumber))
                {
                    return ErrorRedirect(((int)Messages.NotVerifiedNumber).ToString(), Urls.VerifyNumber);
                }
            }


            Payload.Checkout.Reference = Msisdn.Equals("") ? RechargeNumber : Msisdn;
            Payload.FullName.Email = model.Email;
            SetPayload(Payload);
            return Redirect(Urls.Checkout);
        }



        public async Task<ActionResult> CreditPayment(CheckoutRequest model)
        {

            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Basket);
            }

            var Payload = GetPayload();

            Payload.TopUp.Clear();
            Payload.Purchase.Clear();
            var Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(model.ProductCode)).Select(x => x.Reference).First();

            List<string> successList = new List<string>();

            foreach (var product in Payload.Basket.ToList())
            {
                //Attempt to add the bundle
                //If successful redirect to credit payment successful page

                string bundleid = "";

                if (model.ProductCode == Models.Enums.ProductCodes.THA.ToString())
                {
                    bundleid = ContentService.GetAppBundleGuid(product);
                }
                else if (model.ProductCode == Models.Enums.ProductCodes.THM.ToString())
                {
                    bundleid = ContentService.GetMobileBundleGuid(product);
                }

                AddBundleDTO request = new AddBundleDTO
                {
                    MsisdnOrCardNumber = Reference,
                    BundleId = bundleid,
                    ProductCode = model.ProductCode
                };

                var result = await TalkHomeWebService.AddBundleWithCredit(request, Payload.ApiToken);


                string bundleName = "";
                if (model.ProductCode == Models.Enums.ProductCodes.THA.ToString())
                {
                    bundleName = ContentService.GetAppBundleByGuid(bundleid).Name;
                }
                else if (model.ProductCode == Models.Enums.ProductCodes.THM.ToString())
                {
                    bundleName = ContentService.GetMobilePlanByGuid(bundleid).Name;
                }

                if (result == null)
                {
                    ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Basket); // Request failed
                }
                else if (result.errorCode == 101)
                {
                    return ErrorRedirect(result.errorCode.ToString(), Urls.Basket); // Request failed
                }
                else if (result.errorCode != 0)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Basket); // Request failed
                }
                else
                {
                    successList.Add(bundleName);
                    Payload.Basket.Remove(product);
                }

            }


            var RequestDTO = new AccountSummaryRequestDTO { productCode = model.ProductCode, token = Payload.ApiToken };
            var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
            }

            SetPayload(Payload);


            return View(new CreditPurchasesViewModel { BundleNames = successList, Payload = Payload, CreditRemaining = ResponseDTO.payload.userAccountSummary.creditRemaining });

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        [ActionName("redeem")]
        public async Task<ActionResult> Redeem(string Points)
        {

            var Payload = GetPayload();

            if (!AccountService.IsAuthorized(Payload))
            {
                return HandleRedirect(GenericMessages.Forbidden, Urls.Login);
            }

            string cardNo = Payload.ProductCodes.Where(x => x.ProductCode.ToLower() == "thcc").First().Reference;

            var Result = await TalkHomeWebService.RedeemPoints(new RedeemPointsRequestDTO
            {
                CardNo = cardNo,
                Points = Points,
                ProductCode = "THCC"
            }, "");


            if (Result != null && Result.status == 0)
            {
                LoggerService.Info(GetType(), Result.message);
            }

            TempData["PointsRedeemed"] = "yes";
            return Redirect(Urls.MyAccount + "/THCC");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        [ActionName("updateuser")]
        public async Task<ActionResult> UpdateUser(AccountPersonalDetailsFormViewModel m)
        {

            if (!ModelState.IsValid)
            {
                //var i = 0;
                //var list = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                //foreach (var item in list)
                //{
                //    var error = Text.ResourceManager.GetString(string.Format("Message_{0}", list[i]));
                //}
                if (Request.IsAjaxRequest())
                    return Json(new { errorcode = 1, Message = "Model state is invalid." });
                else
                    return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.MyAccount);
                // 
            }

            var Payload = GetPayload();

            if (!AccountService.IsAuthorized(Payload))
            {
                if (Request.IsAjaxRequest())
                    return Json(new { errorcode = 1, Message = GenericMessages.Forbidden });
                else
                    return HandleRedirect(GenericMessages.Forbidden, Urls.Login);
            }

            bool onList = false;
            string[] alist = new string[] { };

            var updateRequest = new UpdatePersonalDetailsRequest
            {
                FirstName = m.Firstname,
                LastName = m.Lastname,
                NewEmailAddress = m.Email,
                OldEmailAddress = m.OldEmail
            };
            if (m.Email != m.OldEmail)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { errorcode = 1, Message = "Sorry.You're not authorized to perform this action." });

                var updateResponse = await AccountService.UpdatePersonalDetails(updateRequest);

                if (updateResponse != null && updateResponse.ErrorCode != 0)
                {
                    if (Request.IsAjaxRequest())
                        return Json(new { errorcode = 1, Message = ModelStateExtensions.RetrieveAllModelErrors(ModelState)[0] });
                    else
                        return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.AccountDetails);
                }

                await MigrationMail("Email Changed", m.Email);
            }

            //var ACResult = await ActiveCampaignService.GetContactDetails(Payload.FullName.Email);

            //if (ACResult != null && ACResult.result_code == 0)
            //{
            //    LoggerService.Info(GetType(), ACResult.result_message);
            //}
            //else
            //{
            //    var lists = ACResult.listslist;
            //    alist = lists.Split('-');
            //}


            string listNo = "18";


            //if (m.OldEmail == m.Email)
            //{
            //    if (alist.Where(x => x.ToString() == listNo).Count() == 1)
            //    {
            //        onList = true;
            //    }

            //    if (m.SubscribeMailingList && !onList)
            //    {
            //        ACResult = await ActiveCampaignService.AddToList(listNo, Payload.FullName.Email, Payload.FullName.FirstName, Payload.FullName.LastName);

            //    }

            //    if (ACResult != null && ACResult.result_code == 0)
            //    {
            //        LoggerService.Info(GetType(), ACResult.result_message);
            //    }

            //    if (!m.SubscribeMailingList && onList)
            //    {
            //        ACResult = await ActiveCampaignService.RemoveTag("GDPR Compliant", Payload.FullName.Email);
            //    }
            //    else if (m.SubscribeMailingList && onList)
            //    {
            //        ACResult = await ActiveCampaignService.AddTag("GDPR Compliant", Payload.FullName.Email);

            //    }


            //    if (ACResult != null && ACResult.result_code == 0)
            //    {
            //        LoggerService.Info(GetType(), ACResult.result_message);
            //    }

            //    string status = "1";
            //    if (!m.SubscribeMailingList)
            //    {
            //        status = "2";
            //    }

            //    if (onList)
            //    {
            //        ACResult = await ActiveCampaignService.ChangeSubscriptionStatus(listNo, Payload.FullName.Email, m.ACId, status);

            //    }
            //}
            //else
            //{
            //    var DeleteContactResponse = await ActiveCampaignService.DeleteContact(m.OldEmail);
            //    if (DeleteContactResponse.result_code == 1)
            //    {
            //        await MigrationMail("Old Contact Deleted", m.OldEmail);
            //        ACResult = await ActiveCampaignService.AddToList(listNo, m.Email, Payload.FullName.FirstName, Payload.FullName.LastName);
            //        string action = "New Contact Added";
            //        if (ACResult != null && ACResult.result_code == 0)
            //        {
            //            LoggerService.Info(GetType(), ACResult.result_message);
            //            action = "Failed to add new contact";
            //        }

            //        await MigrationMail(action, m.Email);

            //        if (m.SubscribeMailingList)
            //        {
            //            ACResult = await ActiveCampaignService.RemoveTag("GDPR Compliant", m.Email);
            //            if (ACResult != null && ACResult.result_code == 0)
            //            {
            //                LoggerService.Info(GetType(), ACResult.result_message);
            //            }
            //        }

            //    }

            //}

            //Update User and Email
            Payload.FullName.Email = m.Email;
            Payload.FullName.FirstName = m.Firstname;
            Payload.FullName.LastName = m.Lastname;

            SetPayload(Payload);


            UpdateAccountConfirmationModel model = new UpdateAccountConfirmationModel();
            model.Payload = Payload;
            if (Request.IsAjaxRequest())
                return Json(new { errorcode = 0, Message = "Successfully updated" });
            else
                return View("~/Views/Customer/SubscriptionUpdate.cshtml", model);
        }


        /// <summary>
        /// Beigns the process for a Top up payment. Processes unique orders only
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>Redirect to payment URL or the vew</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> StartTopUpPayment(StartPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Checkout);
            }

            string Error = "";
            var Payload = GetPayload();
            Payload.Payment = null;
            Payload.OneClick = null; // Reset required fields


            var TopUp = ContentService.GetAllTopUpsByProductId(Payload.TopUp.First());

            var MiPayCustomer = await PaymentService.GetCustomer(new GetCustomerRequestModel(ChannelType.Web, Payload.Checkout.Reference, TopUp.First().Key.Equals(Models.Enums.ProductCodes.THCC.ToString()) ? UniqueIDType.Card : UniqueIDType.Msisdn));

            if (MiPayCustomer == null)
            {
                return GetMiPayCustomerFailed(LoggerService);
            }

            if (PaymentService.IsOneClickElegible(Payload, MiPayCustomer.payload, model)) // One-click checkout
            {
                var OneClick = await PaymentService.TryOneClickTopUp(model, Payload, ChannelType.Web, Error);

                if (OneClick == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Checkout);
                }

                Payload.OneClick = OneClick;
                Payload.FullName.FirstName = model.FirstName;
                Response.Cookies.Add(AccountService.EncodeCookie(Payload));
                return Redirect(Urls.OneClickComplete);
            }

            //
            string cleanedPostCode = "";
            if (!PaymentService.IsValidPostCode(model.PostalCode, out cleanedPostCode))
            {
                return ErrorRedirect(((int)Messages.InvalidPostCode).ToString(), Urls.Checkout); // Request failed
            }

            model.PostalCode = cleanedPostCode;

            var RequestDTO = PaymentService.CreatePaymentRequest(Payload, model, ChannelType.Web);

            var ResponseDTO = await PaymentService.StartPayment(RequestDTO);

            Payload.CheckoutProduct = "";

            if (ResponseDTO == null)
            {
                return StartPaymentFailed(LoggerService);
            }

            if (!PaymentService.TryPaymentSuccess(ResponseDTO, out Error))
            {
                return HandleRedirect(Error, Urls.AppCheckout);
            }


            Payload.Payment = new PaymentRetrieveRequest(ResponseDTO.payload.token, ResponseDTO.payload.clientReference, RequestDTO.PaymentType, model.PaymentMethod, RequestDTO.ChannelType);
            SetPayload(Payload);

            LoggerService.Info(GetType(), string.Format("Payment URL Called: {0}", ResponseDTO.payload.paymentURL));
            return Redirect(ResponseDTO.payload.paymentURL);
        }

        /// <summary>
        /// Formats the request for a purchase payment
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> StartPurchasePayment(StartPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Checkout);
            }

            string Error = "";
            var Payload = GetPayload();
            Payload.Payment = null;
            Payload.OneClick = null;

            var ProductCode = "";
            var Product = (dynamic)null;
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

            if (Enum.Parse(typeof(TalkHome.Models.Enums.PaymentMethod), model.PaymentMethod).ToString() == TalkHome.Models.Enums.PaymentMethod.Credit.ToString())
            {
                if (Product == null)
                {
                    return RedirectToAction("CreditPayment", new CheckoutRequest { Id = 0, ProductCode = ProductCode });
                }

                CheckoutRequest co = new CheckoutRequest { Id = Product.Id, ProductCode = ProductCode };
                var Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(co.ProductCode)).Select(x => x.Reference).First();
                List<string> successList = new List<string>();


                //Attempt to add the bundle
                //If successful redirect to credit payment successful page

                string bundleid = "";

                if (co.ProductCode == Models.Enums.ProductCodes.THA.ToString())
                {
                    bundleid = ContentService.GetAppBundleGuid(co.Id);
                }
                else if (co.ProductCode == Models.Enums.ProductCodes.THM.ToString())
                {
                    bundleid = ContentService.GetMobileBundleGuid(co.Id);
                }

                AddBundleDTO request = new AddBundleDTO
                {
                    MsisdnOrCardNumber = Reference,
                    BundleId = bundleid,
                    ProductCode = co.ProductCode
                };

                var result = await TalkHomeWebService.AddBundleWithCredit(request, Payload.ApiToken);


                string bundleName = "";
                if (co.ProductCode == Models.Enums.ProductCodes.THA.ToString())
                {
                    bundleName = ContentService.GetAppBundleByGuid(bundleid).Name;
                }
                else if (co.ProductCode == Models.Enums.ProductCodes.THM.ToString())
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


                return View("~/Views/Account/CreditPayment.cshtml", new CreditPurchasesViewModel { BundleNames = successList, Payload = Payload, CreditRemaining = ResponseDTO1.payload.userAccountSummary.creditRemaining });
            }


            var MiPayCustomer = await PaymentService.GetCustomer(new GetCustomerRequestModel(ChannelType.Web, Payload.Checkout.Reference, ProductCode.Equals(Models.Enums.ProductCodes.THCC.ToString()) ? UniqueIDType.Card : UniqueIDType.Msisdn));

            if (MiPayCustomer == null)
            {
                return GetMiPayCustomerFailed(LoggerService);
            }

            string cleanedPostCode = "";
            if (!PaymentService.IsValidPostCode(model.PostalCode, out cleanedPostCode))
            {
                return ErrorRedirect(((int)Messages.InvalidPostCode).ToString(), Urls.Checkout); // Request failed
            }

            model.PostalCode = cleanedPostCode;

            if (PaymentService.IsOneClickElegible(Payload, MiPayCustomer.payload, model)) // One-click checkout
            {
                var OneClick = await PaymentService.TryOneClickPurchase(model, Payload, ChannelType.Web, Error);

                if (OneClick == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Checkout);
                }

                Payload.OneClick = OneClick;
                Response.Cookies.Add(AccountService.EncodeCookie(Payload));
                return Redirect(Urls.OneClickComplete);

            }

            var RequestDTO = PaymentService.CreatePaymentRequest(Payload, model, ChannelType.Web);

            var ResponseDTO = await PaymentService.StartPayment(RequestDTO);

            if (ResponseDTO == null)
            {
                return StartPaymentFailed(LoggerService);
            }

            if (!PaymentService.TryPaymentSuccess(ResponseDTO, out Error))
            {
                return HandleRedirect(Error, Urls.AppCheckout);
            }


            Payload.Payment = new PaymentRetrieveRequest(ResponseDTO.payload.token, ResponseDTO.payload.clientReference, RequestDTO.PaymentType, model.PaymentMethod, RequestDTO.ChannelType);
            SetPayload(Payload);

            LoggerService.Info(GetType(), string.Format("Payment URL Called: {0}", ResponseDTO.payload.paymentURL));

            return Redirect(ResponseDTO.payload.paymentURL);
        }


        /// <summary>
        /// Formats the request for a purchase payment
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> StartTransferPayment(StartPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), "/transfercheckout");
            }

            string Error = "";
            var Payload = GetPayload();
            Payload.Payment = null;
            Payload.OneClick = null;

            var ProductCode = "";

            if (Payload.Purchase.Count() > 0)
            {
                var Product = ContentService.GetProducts(Payload.Purchase.First()); // Get the basket item
                ProductCode = ContentService.GetProductCode(Product);
            }
            else if (Payload.CheckoutProduct == "THATT")
            {
                ProductCode = "THATT";
            }

            var MiPayCustomer = await PaymentService.GetCustomer(new GetCustomerRequestModel(ChannelType.Web, Payload.Checkout.Reference, ProductCode.Equals(Models.Enums.ProductCodes.THCC.ToString()) ? UniqueIDType.Card : UniqueIDType.Msisdn));

            if (MiPayCustomer == null)
            {
                return GetMiPayCustomerFailed(LoggerService);
            }

            if (PaymentService.IsOneClickElegible(Payload, MiPayCustomer.payload, model)) // One-click checkout
            {
                var OneClick = await PaymentService.TryOneClickPurchase(model, Payload, ChannelType.Web, Error);

                if (OneClick == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), "/transfercheckout");
                }

                Payload.OneClick = OneClick;
                Response.Cookies.Add(AccountService.EncodeCookie(Payload));
                return Redirect(Urls.OneClickComplete);
            }

            var RequestDTO = PaymentService.CreatePaymentRequest(Payload, model, ChannelType.Web);
            var ResponseDTO = await PaymentService.StartPayment(RequestDTO);

            if (ResponseDTO == null)
            {
                return StartPaymentFailed(LoggerService);
            }

            if (!PaymentService.TryPaymentSuccess(ResponseDTO, out Error))
            {
                return HandleRedirect(Error, Urls.AppCheckout);
            }

            Payload.Payment = new PaymentRetrieveRequest(ResponseDTO.payload.token, ResponseDTO.payload.clientReference, RequestDTO.PaymentType, model.PaymentMethod, RequestDTO.ChannelType);
            SetPayload(Payload);
            return Redirect(ResponseDTO.payload.paymentURL);
        }


        /// <summary>
        /// Validates a request for registering a product code and forwards to the registration page
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <returns>The redirection object</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public ActionResult RegisterProduct(string productCode)
        {
            string Error = "";

            if (!AccountService.TryValidateProductCode(productCode, out Error))
            {
                return ErrorRedirect(Error, Urls.RegisterProduct);
            }

            var Payload = GetPayload();

            Payload.OpenRegistration = productCode;
            SetPayload(Payload);
            return Redirect(Urls.ConfirmProductDetails);
        }

        /// <summary>
        /// Recieves a POST request to begin a Top up checkout
        /// </summary>
        /// <param name="model">The checkout request model</param>
        /// <returns>The details confirmation view</returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]

        public ActionResult TopUpCheckout(bool InternationalTopUp = false)
        {
            CheckoutRequest model = new CheckoutRequest
            {
                ProductCode = "THM"
            };
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.TopUp);
            }



            var Payload = GetPayload();
            var TopUp = ContentService.GetDefaultTopUpByProductCode(model.ProductCode);

            Payload.TopUp.Clear();
            Payload.Purchase.Clear();
            Payload.TopUp.Add(TopUp.Id);
            Payload.Checkout = new CheckoutPageDTO { Verify = model.ProductCode, ProductType = ProductType.TopUp.ToString(), Total = TopUp.ProductPrice };

            if (InternationalTopUp)
            {
                TempData["IsInternationalTopUp"] = true;
            }
            else
            {
                TempData["IsInternationalTopUp"] = null;
            }

            if (AccountService.IsAuthorized(Payload))
            {
                var Reference = "";

                try
                {
                    Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(model.ProductCode)).Select(x => x.Reference).First();
                }
                catch
                {
                    return ErrorRedirect(((int)Messages.ProductNotRegisteredForPurchase).ToString(), Urls.RegisterProduct);
                    //return ErrorRedirect(((int)Messages.ProductNotRegisteredForPurchase).ToString(), Urls.TopUp);
                }

                Payload.Checkout.Reference = Reference;
                Response.Cookies.Add(AccountService.EncodeCookie(Payload));
                return Redirect(Urls.Checkout);
            }
            if (model.ProductCode == "THCC")
            {
                Payload.isTHCCPin = false;
            }



            SetPayload(Payload);
            return Redirect(Urls.Checkout);
        }

        /// <summary>
        /// Receives a POST request from any bundle/plan pages to begin a Bundle checkout
        /// </summary>
        /// <param name="model">the request model</param>
        /// <returns>The details confirmation view</returns>
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

            return Redirect(Urls.VerifyNumber + "?Source=" + model.Source);
        }

        /// <summary>
        /// Creates a CheckoutDTO and redirects to chekout page for basket
        /// </summary>
        /// <param name="model">The requets model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> BasketCheckout(CheckoutRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.Basket);
            }

            var Payload = GetPayload();

            Payload.TopUp.Clear();
            Payload.Purchase.Clear();
            var Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(model.ProductCode)).Select(x => x.Reference).First();
            decimal Total = PaymentService.GetCheckoutTotal(Payload.Basket);


            var RequestDTO = new AccountSummaryRequestDTO { productCode = model.ProductCode, token = Payload.ApiToken };
            var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

            if (Total > ResponseDTO.payload.userAccountSummary.creditRemaining)
            {
                TempData["NoCredit"] = "no-credit";
            }


            Payload.Checkout = new CheckoutPageDTO { Verify = model.ProductCode, ProductType = ProductType.Basket.ToString(), Basket = Payload.Basket, Reference = Reference, Total = Total };
            SetPayload(Payload);
            return Redirect(Urls.Checkout);
        }

        /// <summary>
        /// Processes a request and response for a promotional sign up
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The Sign Up promo view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> PromoSignUp(PromoSignUpRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.PromoSignUp);
            }

            var RequestDTO = Mapper.Map<PromoSignUpRequestDTO>(model);
            var ResponseDTO = await AccountService.PromoSignUp(RequestDTO);

            string Error;

            if (!AccountService.TryPromoSignUpSuccess(RequestDTO, ResponseDTO, out Error))
            {
                return ErrorRedirect(Error, Urls.PromoSignUp);
            }

            return SuccessRedirect(((int)Messages.PromoSignUpSuccess).ToString(), Urls.PromoSignUp);
        }

        /// <summary>
        /// Performs a request for sending the password reset link to the provided email address
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> ForgottenPassword(ResetPasswordRequest model)
        {

            var Payload = GetPayload();

            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.ForgottenPassword);
            }

            if (Payload.HomeRoot != null && Payload.HomeRoot != "Homepage" && model.Number != null)
            {
                //find if they are on the old system

                //register user
                var LegacyAccount = await AccountService.LegacyCardUserExistsWithPin(model.EmailAddress, model.Number);
                if (LegacyAccount.ErrorCode != 0)
                {
                    return ErrorRedirect(((int)Messages.NotFoundForResetPassword).ToString(), Urls.ForgottenPassword);
                }
                else
                {
                    await MigrationMail("Legacy User", model.EmailAddress);
                    LegacyAccount.FirstName = (String.IsNullOrEmpty(LegacyAccount.FirstName) ? "First Name" : LegacyAccount.FirstName);
                    LegacyAccount.LastName = (String.IsNullOrEmpty(LegacyAccount.LastName) ? "Last Name" : LegacyAccount.LastName);

                    var model1 = new SignUpRequest
                    {
                        EmailAddress = model.EmailAddress,
                        Password = LegacyAccount.Password,
                        ConfirmPassword = LegacyAccount.Password,
                        LegacyMigration = true,
                        LegacyCallingCardNumber = LegacyAccount.CallingCardNumber,
                        LegacyPinNumber = model.Number,
                        FirstName = LegacyAccount.FirstName,
                        LastName = LegacyAccount.LastName
                    };

                    var SignRequestDTO = Mapper.Map<SignUpRequestDTO>(model1);
                    var SignResponseDTO = await TalkHomeWebService.SignUp(SignRequestDTO);
                    if (SignResponseDTO == null)
                    {
                        return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.SignUp);
                    }

                    string listId = "19";
                    //var ACResult = await ActiveCampaignService.AddToList(listId, Payload.FullName.Email, Payload.FullName.FirstName, Payload.FullName.LastName);
                    //if (ACResult != null && ACResult.result_code == 0)
                    //{
                    //    LoggerService.Info(GetType(), ACResult.result_message);

                    //    ACResult = await ActiveCampaignService.AddTag("GDPR Compliant", Payload.FullName.Email);

                    //    if (ACResult != null && ACResult.result_code == 0)
                    //    {
                    //        LoggerService.Info(GetType(), ACResult.result_message);
                    //    }
                    //}

                    //Quiet verification    
                    var ResponseDTO3 = await AccountService.VerifySignUpEmail(SignResponseDTO.payload.signUp.token);
                    if (ResponseDTO3 == null)
                    {
                        await MigrationMail("Error Verifying Signing Up", model.EmailAddress);
                        return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.SignUp);
                    }


                    if (ResponseDTO3.errorCode != 0 || !ResponseDTO3.payload.isConfirmed)
                    {
                        return FailedAccountVerification(SignResponseDTO.errorCode);
                    }

                    //Quiet Login
                    var RequestDTO2 = new LoginRequestDTO { email = model.EmailAddress, password = LegacyAccount.Password, ipAddress = GetRequestIP() };
                    var ResponseDTO2 = await TalkHomeWebService.AuthenticateCustomer(RequestDTO2);

                    if (ResponseDTO2 == null)
                    {
                        return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.Login);
                    }

                    Payload.ApiToken = ResponseDTO2.payload.authentication.token;
                    Payload.ApiTokenExpiry = ResponseDTO2.payload.authentication.expiryDate;

                    //Quiet Product Add
                    var RequestDTO1 = new AddProductRequestDTO { number = LegacyAccount.CallingCardNumber, code = model.Number, productCode = "THCC" };
                    var ResponseDTO1 = await TalkHomeWebService.AddProduct(RequestDTO1, Payload.ApiToken);

                    if (ResponseDTO1 == null)
                    {
                        return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.ConfirmProductDetails);
                    }

                    if (ResponseDTO1.errorCode != 0)
                    {
                        return FailedAddProduct(ResponseDTO1.errorCode, LegacyAccount.CallingCardNumber);
                    }

                    await MigrationMail("Calling Card Producted Added", model.EmailAddress);

                    Payload.ApiToken = null;
                    Payload.ApiTokenExpiry = new DateTime();
                    Payload.ProductCodes.Clear();
                    Payload.Checkout = null;
                    Payload.Payment = null;
                    Payload.OneClick = null;
                    Payload.CustId = null;
                }
            }

            var RequestDTO = new ResetPasswordRequestDTO { email = model.EmailAddress };
            //var ResponseDTO = await AccountService.ResetPasswordRequest(RequestDTO);

            var ResponseDTO = await ForgotPasswordEmail(model.EmailAddress);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.ForgottenPassword);
            }

            if (ResponseDTO.errorCode == (int)Messages.NotFoundForResetPassword || !ResponseDTO.payload.resetPassword.isSuccess)
            {
                return ErrorRedirect(((int)Messages.NotFoundForResetPassword).ToString(), Urls.ForgottenPassword);
            }




            //Payload.FullName.Email = model.EmailAddress;
            SetPayload(Payload);
            return SuccessRedirect(((int)Messages.PasswordResetLinkSent).ToString(), Urls.ForgottenPassword);
        }

        /// <summary>
        /// Callback Url to verify the validity of an email address and reset token
        /// </summary>
        /// <returns>The view</returns>
        [ActionName("reset-token")]
        public async Task<ActionResult> ResetToken(string productCode)
        {
            var Token = productCode;

            if (Token == null)
            {
                return ErrorRedirect(((int)Messages.InvalidResetToken).ToString(), Urls.ForgottenPassword);
            }

            var RequestDTO = new ResetPasswordConfirmRequestDTO { token = Token };
            var ResponseDTO = await AccountService.ResetPasswordConfirm(RequestDTO);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.ForgottenPassword);
            }

            if (ResponseDTO.errorCode == (int)Messages.InvalidResetToken || !ResponseDTO.payload.passwordResetTokenValidation.isValid)
            {
                return ErrorRedirect(((int)Messages.InvalidResetToken).ToString(), Urls.ForgottenPassword);
            }

            var Payload = GetPayload();

            Payload.VerifiedReset = true;
            SetPayload(Payload);
            return SuccessRedirect(((int)Messages.ValidResetToken).ToString(), Urls.CreateNewPassword);
        }

        /// <summary>
        /// Sends a request to set a new password
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> NewPassword(NewPasswordRequest model)
        {
            var Payload = GetPayload();

            if (!Payload.VerifiedReset)
            {
                return ErrorRedirect(((int)Messages.InvalidResetToken).ToString(), Urls.ForgottenPassword);
            }

            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.CreateNewPassword);
            }

            var RequestDTO = new NewPasswordRequestDTO
            {
                email = model.EmailAddress,
                newPassword = model.Password,
                confirmPassword = model.ConfirmPassword
            };
            var ResponseDTO = await AccountService.NewPassword(RequestDTO);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CreateNewPassword);
            }

            if (ResponseDTO.errorCode == (int)Messages.NewPasswordFailed || !ResponseDTO.payload.passwordChange.isChanged)
            {
                return ErrorRedirect(((int)Messages.NewPasswordFailed).ToString(), Urls.CreateNewPassword);
            }

            Payload.VerifiedReset = false;
            SetPayload(Payload);
            return SuccessRedirect(((int)Messages.NewPasswordSuccess).ToString(), Urls.Login);
        }


        /// <summary>
        /// Sends a request to set a new password
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> ResetPassword(NewPasswordRequest model)
        {
            var Payload = GetPayload();

            if (!ModelState.IsValid)
            {
                return Json(new { errorcode = 1, Message = "Model state is invalid" });
                //return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState), Urls.CreateNewPassword);
            }

            var RequestDTO = new NewPasswordRequestDTO
            {
                email = model.EmailAddress,
                newPassword = model.Password,
                confirmPassword = model.ConfirmPassword
            };
            var ResponseDTO = await AccountService.NewPassword(RequestDTO);

            if (ResponseDTO == null)
            {

                return Json(new { errorcode = 1, Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())) });
                //return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CreateNewPassword);
            }

            if (ResponseDTO.errorCode == (int)Messages.NewPasswordFailed || !ResponseDTO.payload.passwordChange.isChanged)
            {
                return Json(new { errorcode = 1, Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.NewPasswordFailed).ToString())) });
                //return ErrorRedirect(((int)Messages.NewPasswordFailed).ToString(), Urls.MyAccount);

            }

            Payload.VerifiedReset = false;
            SetPayload(Payload);
            if (Request.IsAjaxRequest())
            {
                return Json(new { errorcode = 0, Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.NewPasswordSuccess).ToString())) });
            }
            else
            {
                return SuccessRedirect(((int)Messages.NewPasswordSuccess).ToString(), Urls.Login);
            }

        }

        /// <summary>
        /// Processes a Calling card order request with credit
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public ActionResult CallingCardCreditOrder(CallingCardCreditOrderRequest model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.OrderCallingCard);
            }

            var CallingCard = ContentService.GetProducts(model.Credit);

            if (CallingCard == null)
            {
                return ErrorRedirect(((int)Messages.InvalidProductCode).ToString(), Urls.OrderCallingCard);
            }

            var Payload = GetPayload();
            //Rechargeable 
            Payload.isTHCCPin = false;
            Payload.CheckoutProduct = "THCC";

            Payload.FullName.Email = model.EmailAddress;
            Payload.TopUp.Clear();
            Payload.Purchase.Clear();
            Payload.Purchase.Add(model.Credit);
            Payload.Checkout = new CheckoutPageDTO
            {
                Verify = Models.Enums.ProductCodes.THCC.ToString(),
                ProductType = ProductType.Bundle.ToString(),
                MailOrder = Mapper.Map<MailOrderRequestDTO>(model),
                Total = CallingCard.ProductPrice,
                DeliveryIsBilling = model.DeliveryIsBilling
            };

            SetPayload(Payload);
            return Redirect(Urls.Checkout);
        }

        /// <summary>
        /// Processes a request to set up an auto top up
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        //[ApiAuthentication]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        //public async Task<ActionResult> AutoTopUpSettings(Models.AutoTopUpRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.AccountSettings + model.ProductCode);
        //    }

        //    var TopUp = ContentService.GetProducts(model.TopUpId);

        //    if (TopUp == null)
        //    {
        //        return ErrorRedirect(((int)Messages.InvalidProductCode).ToString(), Urls.AccountSettings + model.ProductCode); // Invalid top up Id
        //    }

        //    var Payload = GetPayload();
        //    var Reference = "";

        //    try
        //    {
        //        Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(model.ProductCode)).Select(x => x.Reference).First();
        //    }
        //    catch
        //    {
        //        return ErrorRedirect(((int)Messages.ProductNotRegistered).ToString(), Urls.MyAccount + model.ProductCode); // Product was not registered
        //    }

        //    if (ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
        //    {
        //        if (Payload.FullName != null)
        //        {
        //            Pay360CustomerRequestModel pay360RequestRequest = new Pay360CustomerRequestModel
        //            {
        //                customerUniqueRef = Payload.FullName.Email,
        //                productCode = "THM"
        //            };
        //            var Pay360CardsResponse = await Pay360Service.Pay360GetCards(pay360RequestRequest);
        //            if (Pay360CardsResponse !=null)
        //            {
        //                if (Pay360CardsResponse.errorCode == 0 && Pay360CardsResponse.payload.paymentMethodResponses.Count() > 0)
        //                {
        //                    var RequestDTO = new Pay360SetAutoTopUpRequest()
        //                    {
        //                        productRef = Reference,
        //                        productCode = "THM",
        //                        productItemCode = model.ProductCode,
        //                        thresholdBalanceAmount = (float)model.Threshold,
        //                        isAutoTopup = model.AutoTopUp.ToLower() == "true" ? true : false,
        //                        topupAmount = TopUp.ProductPrice,
        //                        topupCurrency = "GBP",
        //                        Email = Payload.FullName.Email
        //                    };
        //                    var ResponseDTO = await Pay360Service.SetAutoTopUp(RequestDTO);
        //                    if (ResponseDTO == null)
        //                    {
        //                        return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings + model.ProductCode);
        //                    }
        //                    if (ResponseDTO.errorCode != 0)
        //                    {
        //                        return ErrorRedirect(ResponseDTO.errorCode.ToString(), Urls.AccountSettings + model.ProductCode);
        //                    }
        //                }
        //                else
        //                {
        //                    return ErrorRedirect(((int)Messages.Pay360CustomerNotFound).ToString(), Urls.AccountSettings + model.ProductCode);
        //                }
        //            }
        //            else
        //            {
        //                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings, model.ProductCode);
        //            }
        //        }

        //    }
        //    else
        //    {
        //        var RequestDTO = new AutoTopUpSettingsRequestDTO
        //        {
        //            msisdn = Reference,
        //            productCode = model.ProductCode,
        //            autoTopUp = model.AutoTopUp.ToLower() == "true" ? true : false,
        //            threshold = model.Threshold,
        //            topUpAmount = TopUp.ProductPrice
        //        };
        //        var ResponseDTO = await TalkHomeWebService.AutoTopUpSettings(RequestDTO, Payload.ApiToken);

        //        if (ResponseDTO == null)
        //        {
        //            return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings + model.ProductCode);
        //        }

        //        if (ResponseDTO.errorCode != 0)
        //        {
        //            return ErrorRedirect(ResponseDTO.errorCode.ToString(), Urls.AccountSettings + model.ProductCode);
        //        }
        //    }

        //    Payload.AutoTopUpEnabled = model.AutoTopUp.ToLower() == "true" ? true : false;
        //    SetPayload(Payload);

        //    return SuccessRedirect(((int)Messages.AutoTopUpSuccess).ToString(), Urls.AccountSettings + model.ProductCode);
        //}





        public async Task<ActionResult> AutoTopUpSettings(Models.AutoTopUpRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Status = false, errorcode = 1, Message = "Model state is not valid" });
                // return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), Urls.AccountSettings + model.ProductCode);
            }

            var TopUp = ContentService.GetProducts(model.TopUpId);

            if (TopUp == null)
            {
                return Json(new { errorcode = 1, State = "InvalidProductCode", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.InvalidProductCode).ToString())), Url = Urls.AccountSettings }, JsonRequestBehavior.AllowGet);

                // return ErrorRedirect(((int)Messages.InvalidProductCode).ToString(), Urls.AccountSettings + model.ProductCode); // Invalid top up Id
            }

            var Payload = GetPayload();
            var Reference = "";

            try
            {
                Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(model.ProductCode)).Select(x => x.Reference).First();
            }
            catch
            {
                return Json(new { errorcode = 1, State = "ProductNotRegistered", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.ProductNotRegistered).ToString())), Url = Urls.MyAccount }, JsonRequestBehavior.AllowGet);

                //   return ErrorRedirect(((int)Messages.ProductNotRegistered).ToString(), Urls.MyAccount + model.ProductCode); // Product was not registered
            }

            if (ConfigurationManager.AppSettings["PaymentProvider"].ToString() == "Pay360")
            {
                if (Payload.FullName != null)
                {
                    Pay360CustomerRequestModel pay360RequestRequest = new Pay360CustomerRequestModel
                    {
                        customerUniqueRef = Payload.FullName.Email,
                        productCode = "THM"
                    };
                    var Pay360CardsResponse = await Pay360Service.Pay360GetCards(pay360RequestRequest);
                    if (Pay360CardsResponse != null)
                    {
                        if (Pay360CardsResponse.errorCode == 0 && Pay360CardsResponse.payload.paymentMethodResponses.Count() > 0)
                        {
                            var RequestDTO = new Pay360SetAutoTopUpRequest()
                            {
                                productRef = Reference,
                                productCode = "THM",
                                productItemCode = model.ProductCode,
                                thresholdBalanceAmount = (float)model.Threshold,
                                isAutoTopup = model.AutoTopUp.ToLower() == "true" ? true : false,
                                topupAmount = TopUp.ProductPrice,
                                topupCurrency = "GBP",
                                Email = Payload.FullName.Email
                            };
                            var ResponseDTO = await Pay360Service.SetAutoTopUp(RequestDTO);
                            if (ResponseDTO == null)
                            {
                                return Json(new { errorcode = 1, State = "AnErrorOccurred", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())), Url = Urls.AccountSettings }, JsonRequestBehavior.AllowGet);

                                //   return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings + model.ProductCode);
                            }
                            if (ResponseDTO.errorCode != 0)
                            {
                                return Json(new { errorcode = 1, State = "AnErrorOccurred", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())), Url = Urls.AccountSettings }, JsonRequestBehavior.AllowGet);

                                //return ErrorRedirect(ResponseDTO.errorCode.ToString(), Urls.AccountSettings + model.ProductCode);
                            }
                        }
                        else
                        {
                            return Json(new { errorcode = 1, State = "Pay360CustomerNotFound", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.Pay360CustomerNotFound).ToString())), Url = Urls.AccountSettings }, JsonRequestBehavior.AllowGet);

                            //return ErrorRedirect(((int)Messages.Pay360CustomerNotFound).ToString(), Urls.AccountSettings + model.ProductCode);
                        }
                    }
                    else
                    {
                        return Json(new { errorcode = 1, State = "AnErrorOccurred", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())), Url = Urls.AccountSettings }, JsonRequestBehavior.AllowGet);

                        //return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings, model.ProductCode);
                    }
                }

            }
            else
            {
                var RequestDTO = new AutoTopUpSettingsRequestDTO
                {
                    msisdn = Reference,
                    productCode = model.ProductCode,
                    autoTopUp = model.AutoTopUp.ToLower() == "true" ? true : false,
                    threshold = model.Threshold,
                    topUpAmount = TopUp.ProductPrice
                };
                var ResponseDTO = await TalkHomeWebService.AutoTopUpSettings(RequestDTO, Payload.ApiToken);

                if (ResponseDTO == null)
                {
                    return Json(new { errorcode = 1, State = "AnErrorOccurred", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())), Url = Urls.AccountSettings }, JsonRequestBehavior.AllowGet);

                    //return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings + model.ProductCode);
                }

                if (ResponseDTO.errorCode != 0)
                {
                    return Json(new { errorcode = 1, State = "AnErrorOccurred", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())), Url = Urls.AccountSettings }, JsonRequestBehavior.AllowGet);

                    // return ErrorRedirect(ResponseDTO.errorCode.ToString(), Urls.AccountSettings + model.ProductCode);
                }
            }

            Payload.AutoTopUpEnabled = model.AutoTopUp.ToLower() == "true" ? true : false;
            SetPayload(Payload);

            return Json(new { Status = true, errorcode = 0, Message = "The auto top up was successfully configured" });
        }









        /// <summary>
        /// Processes a request to set up an auto top up
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [ApiAuthentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> AutoRenewSettings()
        {

            int keys = Request.Form.Count;
            keys = keys - 1;
            List<AutoRenewalsRequest> Renewals = new List<AutoRenewalsRequest>();
            string pCode = "";
            for (var i = 0; i < (keys / 4); i++)
            {
                pCode = Request.Form["Renewals_" + i.ToString() + "_ProductCode"];
                string pRef = Request.Form["Renewals_" + i.ToString() + "_ProductRef"];
                string bGuid = Request.Form["Renewals_" + i.ToString() + "_BundleGuid"];
                string bStatus = Request.Form["Renewals_" + i.ToString() + "_BundleStatus"];
                Renewals.Add(new AutoRenewalsRequest { ProductCode = pCode, BundleGuid = bGuid, BundleStatus = bStatus, ProductRef = pRef });
            }


            var Payload = GetPayload();
            var Reference = "";

            foreach (AutoRenewalsRequest m in Renewals)
            {
                try
                {
                    Reference = Payload.ProductCodes.Where(x => x.ProductCode.Equals(m.ProductCode)).Select(x => x.Reference).First();
                }
                catch
                {
                    return ErrorRedirect(((int)Messages.ProductNotRegistered).ToString(), Urls.MyAccount + pCode); // Product was not registered
                }

                var RequestDTO = new AutoRenewalsSettingsRequestDTO
                {
                    msisdn = Reference,
                    productCode = m.ProductCode,
                    autoTopUp = m.BundleStatus.ToLower() == "true" ? true : false,
                    calling_packageid = m.BundleGuid
                };
                var ResponseDTO = await TalkHomeWebService.AutoRenewSettings(RequestDTO, Payload.ApiToken);

                if (ResponseDTO == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings + pCode);
                }

                if (ResponseDTO.errorCode != 0)
                {
                    return ErrorRedirect(ResponseDTO.errorCode.ToString(), Urls.AccountSettings + pCode);
                }
            }

            return SuccessRedirect(((int)Messages.AutoRenewalSuccess).ToString(), Urls.AccountSettings + pCode);
        }


        /// <summary>
        /// Processes a request to set up an auto top up
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The view</returns>
        [ApiAuthentication]
        [HttpPost]
        [HandleErrorTHM(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        public async Task<ActionResult> AutoRenewBundle(AutoRenewalsSettingsRequestDTO renewal)
        {

            var Payload = GetPayload();
            var product = Payload.ProductCodes.FirstOrDefault(x => x.Reference == renewal.msisdn);
            if (product != null)
            {
                renewal.AccountId = product.AccountId;
                renewal.productCode = product.ProductCode;
            }
            renewal.Email = Payload.FullName.Email;

            var ResponseDTO = await TalkHomeWebService.AutoRenewSettings(renewal, Payload.ApiToken);

            if (ResponseDTO == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.AccountSettings + renewal.productCode);
            }

            if (ResponseDTO.errorCode != 0)
            {
                return ErrorRedirect(ResponseDTO.errorCode.ToString(), Urls.AccountSettings + renewal.productCode);
            }

            return Json(ResponseDTO);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="telephoneNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("gettransferpackages")]
        public async Task<ActionResult> GetTransferPackages(string telephoneNumber, string countryCode)
        {
            var Payload = GetPayload();

            var util = PhoneNumberUtil.GetInstance();
            var cc = util.GetCountryCodeForRegion(countryCode);

            AccountCodes acs = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
            string fromMsisdn = acs.Reference;

            telephoneNumber = telephoneNumber.TrimStart('+');
            int firstPosition = telephoneNumber.IndexOf(cc.ToString());

            if (firstPosition == -1)
            {
                telephoneNumber = cc.ToString() + telephoneNumber;
            }
            else
            {
                telephoneNumber = telephoneNumber.TrimStart('0');
            }

            var parsedNumber = util.Parse("+" + telephoneNumber, countryCode);

            Payload.AirTimeTransfer = null;
            SetPayload(Payload);

            string correctedNumber = parsedNumber.CountryCode.ToString() + parsedNumber.NationalNumber.ToString();
            var packages = await AirTimeTransferService.GetOperators(correctedNumber, fromMsisdn);

            return Json(packages);
        }


        [HttpPost]
        [ActionName("GetTransferPackagesInternationalTopUpWidget")]
        public async Task<ActionResult> GetTransferPackagesInternationalTopUpWidget(string telephoneNumber, string countryCode)
        {
            var Payload = GetPayload();

            InternationalTopUpResponseModel responseModel = new InternationalTopUpResponseModel();

            var RequestDTO = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
            var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

            if (ResponseDTO == null)
            {
                responseModel.Message = "An Internal Server Error Occurred";
                return Json(responseModel); // Request failed
            }

            if (ResponseDTO.payload.userAccountSummary.creditRemaining < 1)
            {

                responseModel.StatusCode = 2;
                responseModel.Message = "Insufficient credit on your mobile account to make the transfer";
                return Json(responseModel, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var util = PhoneNumberUtil.GetInstance();
                // var cc = util.GetCountryCodeForRegion(countryCode);

                AccountCodes acs = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
                string fromMsisdn = acs.Reference;

                // telephoneNumber = telephoneNumber.TrimStart('+');
                //   int firstPosition = telephoneNumber.IndexOf(cc.ToString());

                //if (firstPosition == -1)
                //{
                //    telephoneNumber = cc.ToString() + telephoneNumber;
                //}
                //else
                //{
                //    telephoneNumber = telephoneNumber.TrimStart('0');
                //}

                // var parsedNumber = util.Parse("+" + telephoneNumber, countryCode);

                Payload.AirTimeTransfer = null;
                SetPayload(Payload);

                //string correctedNumber = parsedNumber.CountryCode.ToString() + parsedNumber.NationalNumber.ToString();
                string correctedNumber = telephoneNumber;
                var packages = await AirTimeTransferService.GetOperators(correctedNumber, fromMsisdn);


                return Json(packages, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("transfercredit")]
        public async Task<ActionResult> TransferCredit(AirtimeTransfer model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorRedirect(ModelStateExtensions.RetrieveAllModelErrors(ModelState, false), "/air-time-transfer", "");
            }

            AirtimePaymentPaths paymentPath = EnumExtensions.EnumHelper<AirtimePaymentPaths>.GetValueFromName(model.PaymentOption);

            var Payload = GetPayload();

            Payload.TopUp.Clear();
            Payload.Purchase.Clear();


            if (paymentPath == AirtimePaymentPaths.Credit)
            {


                TransferRequestDTO req = new TransferRequestDTO
                {
                    fromMSISDN = model.Msisdn,
                    product = model.PackageId,
                    operatorid = model.OperatorId,
                    messageToRecipient = String.Join(" ", model.TransferMessage),
                    nowtelTransactionReference = model.TransactionReference
                };


                var RequestDTO = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
                var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

                if (ResponseDTO == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
                }

                if (decimal.Parse(model.Cost) > (int)ResponseDTO.payload.userAccountSummary.creditRemaining)
                {
                    return ErrorRedirect(((int)Messages.TransferInsufficientCredit).ToString(), "/air-time-transfer");
                }

                var result = await AirTimeTransferService.Transfer(req);
                // var result = new TransferResponse { amount = model.Cost, errorCode = 0, currency = model.Currency, message = "Success", reference = "8944878255999617357", status = "Success" };
                if (result == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
                }

                if (result.errorCode != 0)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
                }

                string sId = await AccountService.GetSubscriberId(model.Msisdn);

                if (String.IsNullOrEmpty(sId))
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
                }

                var result1 = await TalkHomeWebService.ThmCreditAutoTopupAccountBalance(sId, model.TransactionReference, decimal.Parse(model.Cost), "");
                // var result1 = await TalkHomeWebService.ThmCreditAutoTopupAccountBalanceNew(sId, model.TransactionReference, decimal.Parse(model.Cost), "", ResponseDTO.payload.userAccountSummary.productRef);


                if (result1 == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
                }
                if (result1.errorCode != 0)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
                }

                RequestDTO = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
                ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

                if (ResponseDTO == null)
                {
                    return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed
                }

                Dictionary<string, string> substitutions = new Dictionary<string, string>();

                substitutions.Add("%NAME%", Payload.FullName.FirstName);
                substitutions.Add("%VALUE%", "&pound;" + model.Cost);
                substitutions.Add("%NUMBER%", model.ToMsisdn);
                substitutions.Add("%PURCHASE_ID%", result.reference);
                substitutions.Add("%CURRENT_BALANCE%", "&pound;" + ResponseDTO.payload.userAccountSummary.creditRemaining.ToString("#.##"));
                substitutions.Add("%TRANSFER%", model.TransferAmount);
                MailTemplate mailTemplate = new MailTemplate
                {
                    Template = MailTemplate.TRANSFER_EMAIL_TEMPLATE,
                    EmailAddress = model.EmailAddress,
                    Substitutions = substitutions,
                    From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                    Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                    Subject = "Receipt for your credit transfer"
                };

                try
                {
                    await mailTemplate.Send();
                }

                catch (Exception e)
                {
                    LoggerService.Error(GetType(), e.Message, e);
                }

                Payload.AirTimeTransfer = model;

                SetPayload(Payload);

                return View(new CreditTransferViewModel { Payload = Payload, CreditRemaining = ResponseDTO.payload.userAccountSummary.creditRemaining, TransactionReference = result.reference });
            }
            else if (paymentPath == AirtimePaymentPaths.Checkout)
            {
                Payload.AirTimeTransfer = model;
                try
                {
                    Payload.AirTimeTransfer.Msisdn = AccountService.GetMsisdnFromNumber(Payload.AirTimeTransfer.Msisdn, Payload.TwoLetterISORegionName);
                }
                catch (Exception)
                {
                    // Calling cards numbers will always fail, it's expected. Proceed
                }
                Payload.Checkout = new CheckoutPageDTO { ProductType = ProductType.AirTimeTransfer.ToString(), Reference = Payload.AirTimeTransfer.Msisdn, Total = decimal.Parse(Payload.AirTimeTransfer.Cost) };
                SetPayload(Payload);
                return Redirect("/transfercheckout");
            }
            else
            {
                return RedirectToAction("TransferCheckout", model);
            }

        }

        [HttpPost]
        [ActionName("TransferCreditInternationalTopUpWidget")]
        public async Task<JsonResult> TransferCreditInternationalTopUpWidget(AirtimeTransfer model)
        {
            InternationalTopUpResponseModel responseModel = new InternationalTopUpResponseModel();
            if (!ModelState.IsValid)
            {
                responseModel.Message = "Model State is Invalid";
                return Json(responseModel);
            }

            AirtimePaymentPaths paymentPath = EnumExtensions.EnumHelper<AirtimePaymentPaths>.GetValueFromName(model.PaymentOption);

            var Payload = GetPayload();

            Payload.TopUp.Clear();
            Payload.Purchase.Clear();


            if (paymentPath == AirtimePaymentPaths.Credit)
            {


                TransferRequestDTO req = new TransferRequestDTO
                {
                    fromMSISDN = model.Msisdn,
                    product = model.PackageId,
                    operatorid = model.OperatorId,
                    messageToRecipient = String.Join(" ", model.TransferMessage),
                    nowtelTransactionReference = model.TransactionReference
                };


                var RequestDTO = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
                var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

                if (ResponseDTO == null)
                {

                    responseModel.Message = "An Internal Server Error Occurred";
                    return Json(responseModel); // Request failed
                }

                if (decimal.Parse(model.Cost) > ResponseDTO.payload.userAccountSummary.creditRemaining)
                {

                    responseModel.Message = "Insufficient credit on your mobile account to make the transfer";
                    return Json(responseModel);

                }

                var result = await AirTimeTransferService.Transfer(req);

                //var result = new TransferResponse { amount = model.Cost, errorCode = 0, currency = model.Currency, message = "Success", reference = "8944878255999617357", status = "Success" };

                if (result == null)
                {
                    responseModel.Message = "An Internal Server Error Occurred";
                    return Json(responseModel);

                }
                if (result.errorCode == 2)
                {
                    responseModel.Message = "Topup Refused.";
                    return Json(responseModel); // Request failed
                }
                if (result.errorCode != 0)
                {
                    responseModel.Message = "An Internal Server Error Occurred";
                    return Json(responseModel); // Request failed
                }

                ThmDebitAccountBalanceDTO ThmDebitAccountBalanceDTO = new ThmDebitAccountBalanceDTO()
                {
                    ProductRef = model.Msisdn,
                    Amount = model.Cost,
                    TransactionId = model.TransactionReference,
                    AdjustmentReason = "TalkHome Air Time Transfer",
                    PaymentMethod = "Sim Credit"
                };


                var result1 = await TalkHomeWebService.ThmDebitAccountBalance(ThmDebitAccountBalanceDTO, Payload.ApiToken);

                //var result1 = new TalkHome.Models.WebApi.GenericApiResponse<ThmDebitAccountBalanceResponseDTO>() { errorCode = 0 };
                if (result1 == null)
                {
                    responseModel.Message = "An Internal Server Error Occurred";
                    return Json(responseModel); // Request failed
                }
                if (result1.errorCode != 0)
                {
                    responseModel.Message = "An Internal Server Error Occurred";
                    return Json(responseModel); // Request failed
                }


                RequestDTO = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
                ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

                if (ResponseDTO == null)
                {
                    responseModel.Message = "An Internal Server Error Occurred";
                    return Json(responseModel); // Request failed
                }

                Dictionary<string, string> substitutions = new Dictionary<string, string>();

                substitutions.Add("%NAME%", Payload.FullName.FirstName);
                substitutions.Add("%VALUE%", "&pound;" + model.Cost);
                substitutions.Add("%NUMBER%", model.ToMsisdn);
                substitutions.Add("%PURCHASE_ID%", result.reference);
                substitutions.Add("%CURRENT_BALANCE%", "&pound;" + ResponseDTO.payload.userAccountSummary.creditRemaining.ToString("#.##"));
                substitutions.Add("%TRANSFER%", model.TransferAmount);
                MailTemplate mailTemplate = new MailTemplate
                {
                    Template = MailTemplate.TRANSFER_EMAIL_TEMPLATE,
                    EmailAddress = model.EmailAddress,
                    Substitutions = substitutions,
                    From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                    Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                    Subject = "Receipt for your credit transfer"
                };

                try
                {
                    await mailTemplate.Send();
                }

                catch (Exception e)
                {
                    LoggerService.Error(GetType(), e.Message, e);
                }

                Payload.AirTimeTransfer = model;

                SetPayload(Payload);
                var data = new CreditTransferViewModel { CreditRemaining = ResponseDTO.payload.userAccountSummary.creditRemaining, TransactionReference = result.reference };
                responseModel.StatusCode = 1;
                responseModel.Message = JsonConvert.SerializeObject(data);
                return Json(responseModel);
            }
            else if (paymentPath == AirtimePaymentPaths.Checkout)
            {
                Payload.AirTimeTransfer = model;
                try
                {
                    Payload.AirTimeTransfer.Msisdn = AccountService.GetMsisdnFromNumber(Payload.AirTimeTransfer.Msisdn, Payload.TwoLetterISORegionName);
                }
                catch (Exception)
                {
                    // Calling cards numbers will always fail, it's expected. Proceed
                }
                Payload.Checkout = new CheckoutPageDTO { ProductType = ProductType.AirTimeTransfer.ToString(), Reference = Payload.AirTimeTransfer.Msisdn, Total = decimal.Parse(Payload.AirTimeTransfer.Cost) };
                SetPayload(Payload);
                responseModel.Url = "transfercheckout";
                return Json(responseModel);
            }
            else
            {
                responseModel.Url = "TransferCheckout";
                responseModel.Message = JsonConvert.SerializeObject(model);

                return Json(responseModel);
            }

        }


        public ActionResult TransferCreditSuccess(CreditTransferViewModelTHM modelTHM)
        {
            var Payload = GetPayload();
            return View("TransferCredit", modelTHM);
            //return View(new CreditTransferViewModel { Payload = Payload, CreditRemaining = ResponseDTO.payload.userAccountSummary.creditRemaining, TransactionReference = result.reference });
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

        public ActionResult CookieAccept()
        {
            return PartialView("_CookieAccept");
        }

        //Port management methods//


        /// <summary>
        /// Gets the details of the authorized user and returns the details page.
        /// </summary>
        /// <returns>The view with the required ViewModel.</returns>
        [ActionName("portdetails")]
        [ApiAuthentication]
        public async Task<JsonResult> PortDetails()
        {
            try
            {
                var Payload = GetPayload();

                var product = new AccountCodes();

                product = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();


                var request = new GetSwitchingInformationRequestModel()
                {
                    msisdn = product.Reference
                };

                var response = await PortService.GetSwitchingInfo(request);
                //var response = new GenericPortingApiResponse<SwitchingInformationApiResponseModel> { payload = new SwitchingInformationApiResponseModel()};
                if (response != null && response.payload != null)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { View = RenderRazorViewToString(ControllerContext, "AccountPortingDetail", new PortDetailsViewModel { Payload = Payload, Balance = response.payload.Balance }), errorcode = 0, }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { View = RenderRazorViewToString(ControllerContext, "AccountPortingDetail", new PortDetailsViewModel { Payload = Payload, Balance = 0 }), errorcode = 0, }, JsonRequestBehavior.AllowGet);
            }
            catch
            {

                return Json(new { errorcode = 2, State = "AnErrorOccurred", Message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())) }, JsonRequestBehavior.AllowGet);

            }

        }


        [ActionName("paymentmethod")]
        [ApiAuthentication]
        public ActionResult CustomerPaymentMethod()
        {

            if (Request.IsAjaxRequest())
            {
                var Payload = GetPayload();
                return PartialView("CustomerPaymentMethod", Payload);
            }
            return View();
        }
        /// <summary>
        /// Get the User Porting Requests Data
        /// </summary>
        /// <returns>List of Porting Requests</returns>
        [HttpGet]
        public async Task<ActionResult> GetUserPortDetials()
        {
            try
            {
                var Payload = GetPayload();

                string Number = Payload.ProductCodes.Where(X => X.ProductCode == "THM").First().Reference;
                var response = await PortService.GetPortRequests(new GetPortRequestsModel() { Email = Payload.FullName.Email, Msisdn = Number });


                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpGet]
        public JsonResult GetJsonPayload()
        {
            var Payload = GetPayload();
            var jsonPayload = JsonConvert.SerializeObject(Payload);
            return Json(jsonPayload, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        [ActionName("portout")]
        public async Task<ActionResult> PortOut(CodeTypes codeTypes, string Date, int Reason)
        {
            var Payload = GetPayload();
            var product = new AccountCodes();
            try
            {
                product = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
            }
            catch
            {
                return Json("Invalid Product Code");
            }

            var request = new PortOutRequestModel
            {
                NTMsisdn = product.Reference,
                Email = Payload.FullName.Email,
                CodeType = codeTypes,
                UserPortingDate = Date,
                ReasonId = Reason
            };
            var po = await PortService.PortOut(request);
            return Json(po);

        }

        [HttpPost]
        [ActionName("portin")]
        public async Task<ActionResult> PortIn(PortInRequestModel request)
        {
            var Payload = GetPayload();
            var product = new AccountCodes();
            try
            {
                product = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
            }
            catch
            {
                return Json("Invalid Product Code");
            }
            request.Email = Payload.FullName.Email;
            request.NTMsisdn = product.Reference;
            var po = await PortService.PortIn(request, PortTypes.PortIn);
            return Json(po);
        }

        [HttpPost]
        public async Task<ActionResult> CancelPorting(string RequestID)
        {
            try
            {
                var Payload = GetPayload();
                var product = new AccountCodes();
                try
                {
                    product = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
                }
                catch
                {
                    return ErrorRedirect(((int)Messages.ProductNotRegisteredForPort).ToString(), Urls.RegisterProduct);
                }

                var request = new CancelPortingRequestModel()
                {
                    RequestID = RequestID
                };
                var response = await PortService.CancelPorting(request);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetCustomerPaymentMethod()
        {
            try
            {
                var Payload = GetPayload();

                var paymentResponse = await Pay360Service.Pay360GetCards(new Pay360CustomerRequestModel() { customerUniqueRef = Payload.FullName.Email, productCode = "THM" });




                return Json(paymentResponse, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SetDefaultCard(Pay360SetCustomerDefaultCardRequest model)
        {

            try
            {

                var Payload = GetPayload();
                var customerResponse = await Pay360Service.GetCustomer(new Pay360CustomerRequestModel() { customerUniqueRef = Payload.FullName.Email, productCode = "THM" });
                if (customerResponse != null && customerResponse.payload != null)
                {

                    model.pay360CustomerID = customerResponse.payload.pay360CustId;
                    var setDefaultCardResponse = await Pay360Service.SetCustomerDefaultCard(model);
                    if (setDefaultCardResponse == null || setDefaultCardResponse.errorCode > 0)
                        return Json(null);
                }

                return Json(new { status = true, message = "Set Default card successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null);
            }

        }
        [HttpPost]
        public async Task<ActionResult> RemoveCard(Pay360RemoveCardRequest model)
        {
            try
            {
                var Payload = GetPayload();
                var customerResponse = await Pay360Service.GetCustomer(new Pay360CustomerRequestModel() { customerUniqueRef = Payload.FullName.Email, productCode = "THM" });
                if (customerResponse != null && customerResponse.payload != null)
                {
                    model.pay360CustomerID = customerResponse.payload.pay360CustId;
                    var removeCardResponse = await Pay360Service.RemoveCard(model);
                    if (removeCardResponse == null || removeCardResponse.errorCode > 0)
                        return Json(null);
                }



                return Json(new { status = true, message = "Remove card successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<ActionResult> RemoveDefaultCard(Pay360RemoveCardRequest model)
        {
            try
            {
                var Payload = GetPayload();
                var productCodes = Payload.ProductCodes.Where(t => t.ProductCode.ToUpper() == "THM").ToList();
                if (productCodes != null && productCodes.Count > 0)
                {
                    foreach (var item in productCodes)
                    {
                        var autoToupResponse = await Pay360Service.GetAutoTopUp(new Pay360GetAutoTopUpRequest() { Msisdn = item.Reference, Email = Payload.FullName.Email });
                        if (autoToupResponse != null && autoToupResponse.errorCode == 0 && autoToupResponse.payload != null)
                        {
                            await SetAutoToUpSettings(item.Reference, false, autoToupResponse.payload.Topup, autoToupResponse.payload.ThresHold, Payload.FullName.Email);
                        }
                    }
                }
                var customerResponse = await Pay360Service.GetCustomer(new Pay360CustomerRequestModel() { customerUniqueRef = Payload.FullName.Email, productCode = "THM" });
                if (customerResponse != null && customerResponse.payload != null)
                {
                    model.pay360CustomerID = customerResponse.payload.pay360CustId;
                    var removeCardResponse = await Pay360Service.RemoveCard(model);
                    if (removeCardResponse == null || removeCardResponse.errorCode > 0)
                        return Json(null);
                }



                return Json(new { status = true, message = "Remove card successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null);
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetSwitchingInfo()
        {
            try
            {
                var Payload = GetPayload();
                var product = new AccountCodes();
                try
                {
                    product = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
                }
                catch
                {
                    return Json("Invalid Product Code");
                }

                var request = new GetSwitchingInformationRequestModel()
                {
                    msisdn = product.Reference
                };
                var response = await PortService.GetSwitchingInfo(request);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        private async Task SetAutoToUpSettings(string msisdn, bool isAutoTopUp, float thresHold, float amount, string email)
        {


            var modelAutoTopUpRequest = new Pay360SetAutoTopUpRequest
            {
                isAutoTopup = isAutoTopUp,
                topupAmount = Convert.ToDecimal(amount),
                productRef = msisdn,
                topupCurrency = "GBP",
                productCode = "THM",
                productItemCode = "THM",
                thresholdBalanceAmount = thresHold,
                Email = email
            };

            await Pay360Service.SetAutoTopUp(modelAutoTopUpRequest);
        }
        //[HttpPost]
        //[ActionName("npac")]
        //public async Task<ActionResult> NPAC(string SubscriberId)
        //{
        //    var Payload = GetPayload();
        //    var product = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
        //    var request = new Port
        //    {
        //        CodeType = CodeType.NPAC,
        //        MSISDN = product.Reference
        //    };
        //    var po = await PortService.PortOut(request, SubscriberId);
        //    string ret = JsonConvert.SerializeObject(po);
        //    return Json(ret);

        //}

        //[HttpPost]
        //[ActionName("npac")]
        //public async Task<ActionResult> PAC(string SubscriberId)
        //{
        //    var Payload = GetPayload();
        //    var product = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
        //    var request = new Port
        //    {
        //        CodeType = CodeType.PAC,
        //        MSISDN = product.Reference
        //    };
        //    var po = await PortService.PortOut(request, SubscriberId);
        //    string ret = JsonConvert.SerializeObject(po);
        //    return Json(ret);

        //}

        //[HttpPost]
        //[ActionName("npac")]
        //public async Task<ActionResult> STAC(string SubscriberId)
        //{
        //    var Payload = GetPayload();
        //    var product = Payload.ProductCodes.Where(x => x.ProductCode == "THM").First();
        //    var request = new Port
        //    {
        //        CodeType = CodeType.STAC,
        //        MSISDN = product.Reference
        //    };
        //    var po = await PortService.PortOut(request, SubscriberId);
        //    string ret = JsonConvert.SerializeObject(po);
        //    return Json(ret);

        //}

        //[ActionName("switchinfo")]
        //public async Task<ActionResult> SwitchInfo(string msisdn, string subscriberId, string type)
        //{            
        //    var Payload = GetPayload();
        //    Port p = new Port();


        //    if (type == "PortOut")
        //        p = await PortService.GetPortOut(subscriberId);
        //    else
        //        p = await PortService.GetPortIn(subscriberId);

        //    var RequestDTO = new AccountSummaryRequestDTO { productCode = "THM", token = Payload.ApiToken };
        //    var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

        //    List<UserAccountBundles> bundles = ResponseDTO.payload.userAccountBundles;
        //    MyAccountViewModel account = new MyAccountViewModel(Payload, "THM", ResponseDTO.payload, null);
        //    return View(new SwitchInfoViewModel { Port = p,  Payload = Payload, MyAccount = account });                        
        //}


        //[Route("credit-sim-plans")]
        //public ActionResult CreditSimPlans()
        //{
        //    return View();
        //}

        [Route("credit-sim-order")]
        public ActionResult CreditSimOrder()
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
            if (TempData.Count() > 0 && TempData.ContainsKey("modelError"))
            {
                TempData["modelError"] = TempData["modelError"];
            }
            else if (TempData.Count() > 0 && TempData.ContainsKey("tempError"))
            {
                TempData["tempError"] = TempData["tempError"];
            }
            return View(new CreditSimPlanRequest(new AddressDetailsViewModel(CountryList, new AddressModel(), Payload.TwoLetterISORegionName.Equals("GB"))));
        }

        [Route("CreditSimUserExists")]
        public async Task<ActionResult> UserExists(string EmailAddress)
        {
            var UserExists = await AccountService.UserExistsandUserId(EmailAddress);

            var UserDetails = await AccountService.GetUserByEmail(EmailAddress);

            var Isvalid_simorder = AccountService.Isvalid_simorder(EmailAddress);



            if (UserExists == null)
            {
                return Json(new { status = false, errorCode = 19, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())), data = "Null" }, JsonRequestBehavior.AllowGet);
            }

            //If user does not exist force them to register (nicely)

            if (!UserExists.UserExists && UserDetails != "True" && Isvalid_simorder != 1 && Isvalid_simorder != 2)
            {
                var result = await Send_otp_to_userAsync(EmailAddress, "Guest");
                return Json(new { status = true, errorCode = 0, message = "Please Register.", data = UserExists }, JsonRequestBehavior.AllowGet);
            }

            if (Isvalid_simorder == 1)
            {
                return Json(new { status = "limitexceeds", errorCode = 0, message = "Please Register.", data = UserExists });
            }

            if (UserExists.UserExists && UserExists.hasProduct)
            {
                return Json(new { status = false, errorCode = 784, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AlreadyHasProduct).ToString())) }, JsonRequestBehavior.AllowGet);
            }
            if (UserExists.UserExists && UserExists.HasSim)
            {
                return Json(new { status = false, errorCode = 784, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.UserHasSim).ToString())) }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true, errorCode = 0, message = "success", data = UserExists }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Route("creditsimorderconfirm")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreditSimOrder(CreditSimRequest model)
        {

            if (!ModelState.IsValid)
            {
                TempData["modelError"] = ModelStateExtensions.RetrieveAllModelErrors(ModelState, false); //"model state is not valid.";
                return Redirect("credit-sim-order");
            }
            var Payload = GetPayload();
            Payload.CreditSim = new CreditSimPayload { };
            Payload.CreditSim.signedUp = "false";
            Payload.CreditSim.Gclid = model.Gclid != null ? model.Gclid : null;

            string EmailVerificationToken = "";
            ////check is user otp verified

            var isuser_otp_verified = await AccountService.Verifyuseremail_against_otpAsync(model.EmailAddress);
            if (isuser_otp_verified == false)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.OrderSIM);
            }

            if (model.SignUp == "on")
            {
                //Sign the user up 
                SignUpRequestDTO RequestDTO = new SignUpRequestDTO
                {
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    email = model.EmailAddress,
                    password = model.Password,
                    confirmPassword = model.ConfirmPassword,
                    isSubscribedToNewsletter = model.SubscribeSignUp,
                    TermsAndConditions = true
                };

                var ResponseDTO = await TalkHomeWebService.SignUp(RequestDTO);

                if (ResponseDTO == null)
                {
                    TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString()));
                    return Redirect("credit-sim-order");
                    //return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())) });
                }

                if (ResponseDTO.errorCode != 0)
                {
                    if (ResponseDTO.errorCode == (int)Messages.AccountAlreadyExists)
                    {
                        LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.AccountAlreadyExists.ToString(), "for", model.EmailAddress));
                        LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.AccountAlreadyExists.ToString(), "for", model.EmailAddress));
                        //return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountAlreadyExists).ToString())) });
                        TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountAlreadyExists).ToString()));
                        return Redirect("credit-sim-order");
                    }
                    else
                    {
                        LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.SignUpFailed.ToString(), "for", model.EmailAddress));
                        LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.SignUpFailed.ToString(), "for", model.EmailAddress));
                        //return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountAlreadyExists).ToString())) });
                        TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountAlreadyExists).ToString()));
                        return Redirect("credit-sim-order");
                    }
                }

                if(ResponseDTO.errorCode==0)
                {
                    var confirmuser = await TalkHomeWebService.VerifySignUpEmail(ResponseDTO.payload.signUp.token);
                }

                Payload.CreditSim.signedUp = "true";

                EmailVerificationToken = ResponseDTO.payload.signUp.token;

                //Do the subscription 
                //if (model.SubscribeSignUp)
                //{
                //    var ACResult1 = await ActiveCampaignService.AddToList("18", model.EmailAddress, model.FirstName, model.LastName);

                //    if (ACResult1 != null && ACResult1.result_code == 0)
                //    {
                //        LoggerService.Info(GetType(), ACResult1.result_message);
                //    }

                //    ACResult1 = await ActiveCampaignService.AddTag("GDPR Compliant", model.EmailAddress);

                //    if (ACResult1 != null && ACResult1.result_code == 0)
                //    {
                //        LoggerService.Info(GetType(), ACResult1.result_message);
                //    }
                //}

                //end subscription calls

                //Dictionary<string, string> substitutions = new Dictionary<string, string>();
                ////Get a verification token
                //string verifyLink = System.Configuration.ConfigurationManager.AppSettings["SignUpConfirmationPageUrl"];

                //substitutions.Add("%VERIFY_LINK%", String.Format(verifyLink, ResponseDTO.payload.signUp.token));
                //substitutions.Add("%FIRST_NAME%", model.FirstName);
                //MailTemplate mailTemplate = new MailTemplate
                //{
                //    Template = MailTemplate.VERIFY_EMAIL_TEMPLATE,
                //    EmailAddress = model.EmailAddress,
                //    Substitutions = substitutions,
                //    From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                //    Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                //    Subject = "Click to activate your account"
                //};

                //try
                //{
                //    await mailTemplate.Send();
                //}

                //catch (Exception e)
                //{
                //    LoggerService.Error(GetType(), e.Message, e);
                //}
            }


            if (model.CountyOrProvince == null || string.IsNullOrEmpty(model.CountyOrProvince) || (model.CountyOrProvince != null && model.CountyOrProvince.Trim() == ""))
            {
                if (model.City == null || string.IsNullOrEmpty(model.City) || (model.City != null && model.City.Trim() == ""))
                {
                    model.City = "Un-known";
                }
                model.CountyOrProvince = model.City;

            }

            var Userid = await AccountService.Get_UserId(model.EmailAddress);

            //var UserExists = await AccountService.UserExistsandUserId(model.EmailAddress);

            //if (UserExists == null)
            //{
            //    //return Json(new { status = false, errorCode = 19, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())), data = "Null" }, JsonRequestBehavior.AllowGet);
            //    TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString()));
            //    return Redirect("credit-sim-order");
            //}

            ////If user does not exist force them to register (nicely)
            //if (!UserExists.UserExists)
            //{
            //    //return Json(new { status = true, errorCode = 0, message = "Please Register.", data = UserExists }, JsonRequestBehavior.AllowGet);
            //    TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountNotFound).ToString()));
            //    return Redirect("credit-sim-order");
            //}
            //else if (UserExists.UserExists && UserExists.hasProduct)
            //{
            //    TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AlreadyHasProduct).ToString()));
            //    return Redirect("credit-sim-order");
            //}
            model.UserId = Convert.ToInt32(Userid);
            Payload.TopUp.Clear();
            Payload.Purchase.Clear();
            Payload.Checkout = new CheckoutPageDTO { ProductType = ProductType.CreditSimOrder.ToString(), Basket = new HashSet<int>() };
            Payload.Checkout.Verify = model.ProductCode;
            // Bundle
            if (!string.IsNullOrEmpty(model.BundleId))
            {
                Payload.Checkout.Basket.Add(Convert.ToInt32(model.BundleId));
                model.creditSimType = 2;
            }
            //Top Up
            if (model.TopUpId != 0)
            {
                Payload.Checkout.Basket.Add(model.TopUpId);
                model.creditSimType = 1;
            }
            if (!string.IsNullOrEmpty(model.BundleId) && model.TopUpId != 0)
            {
                model.creditSimType = 3;
            }

            Payload.CreditSim.ProductType = ProductType.CreditSimOrder.ToString();
            Payload.CreditSim.userId = (int)model.UserId;
            Payload.CreditSim.Email = model.EmailAddress;
            Payload.CreditSim.Name = model.FirstName + ' ' + model.LastName;

            ContentService cs = new ContentService();
            List<CreditSimbasket> basket = new List<CreditSimbasket>();
            var Products = cs.GetProducts(Payload.Checkout.Basket);
            foreach (var product in Products)
            {
                basket.Add(new CreditSimbasket
                {
                    amount = Convert.ToInt32(product.ProductPrice),
                    bundleId = (product.ProductUuid == "0") ? "" : product.ProductUuid
                });
            }

            CreditSimOrderApiRequest request = new CreditSimOrderApiRequest
            {
                userId = (int)model.UserId,
                firstName = model.FirstName,
                lastName = model.LastName,
                addressL1 = model.AddressLine1,
                addressL2 = model.AddressLine2,
                city = model.City,
                country = model.CountryCode,
                county = model.CountyOrProvince,
                creditSimType = model.creditSimType,
                email = model.EmailAddress,
                postCode = model.PostalCode,
                basket = basket
            };

            var Result = await TalkHomeWebService.CreditSimOrder(request); //new GenericApiAppResponse<CreditSimOrderResponse> { payload = new CreditSimOrderResponse { orderId=2342} };  //


            LoggerService.Debug(GetType(), $"Method: CreditSimOrder, " +
                        $"Request=>: {JsonConvert.SerializeObject(request)}, " +
                        $"CreditSimOrderResponse=>: { JsonConvert.SerializeObject(Result) } ");

            if (Result == null)
            {

                TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString()));
                return Redirect("credit-sim-order");
            }



            if (Result.errorCode != 0)
            {
                TempData["tempError"] = (Result.message != null) ? Result.message : Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString()));
                return Redirect("credit-sim-order");
            }

            TempData["PurchaseType"] = "CreditSimOrder";
            Payload.CreditSim.orderId = Result.payload.orderId;
            Payload.EmailVerificationToken = "";

            Response.Cookies.Add(AccountService.EncodeCookie(Payload));
            SetPayload(Payload);
            return Redirect(Urls.Checkout);
        }



        [HttpPost]
        [Route("MailOrderForExistingUser")]
        public async Task<ActionResult> MailOrderForExistingUser(MailOrderRequest model)
        {
            if (!ModelState.IsValid)
            {
                TempData["modelError"] = ModelStateExtensions.RetrieveAllModelErrors(ModelState, false); //"model state is not valid.";
                return Redirect("credit-sim-order");
            }

            var Payload = GetPayload();

            string EmailVerificationToken = "";

            if (model.SignUp == "on")
            {
                //Sign the user up 
                SignUpRequestDTO RequestDTO = new SignUpRequestDTO
                {
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    email = model.EmailAddress,
                    password = model.Password,
                    confirmPassword = model.ConfirmPassword,
                    TermsAndConditions = true
                };

                var ResponseDTO = await TalkHomeWebService.SignUp(RequestDTO);

                if (ResponseDTO == null)
                {
                    TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString()));
                    return Redirect("credit-sim-order");

                    // return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())) });
                }

                if (ResponseDTO.errorCode != 0)
                {
                    if (ResponseDTO.errorCode == (int)Messages.AccountAlreadyExists)
                    {
                        LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.AccountAlreadyExists.ToString(), "for", model.EmailAddress));
                        LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.AccountAlreadyExists.ToString(), "for", model.EmailAddress));

                        TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountAlreadyExists).ToString()));
                        return Redirect("credit-sim-order");
                        //return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountAlreadyExists).ToString())) });
                    }
                    else
                    {
                        LoggerService.SendInfoAlert(string.Format("{0} {1} {2}", Messages.SignUpFailed.ToString(), "for", model.EmailAddress));
                        LoggerService.Info(GetType(), string.Format("{0} {1} {2}", Messages.SignUpFailed.ToString(), "for", model.EmailAddress));
                        TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountAlreadyExists).ToString()));
                        return Redirect("credit-sim-order");
                        //return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AccountAlreadyExists).ToString())) });
                    }
                }


                EmailVerificationToken = ResponseDTO.payload.signUp.token;

                //Do the subscription 
                //if (model.SubscribeSignUp)
                //{
                //    var ACResult1 = await ActiveCampaignService.AddToList("18", model.EmailAddress, model.FirstName, model.LastName);

                //    if (ACResult1 != null && ACResult1.result_code == 0)
                //    {
                //        LoggerService.Info(GetType(), ACResult1.result_message);
                //    }

                //    ACResult1 = await ActiveCampaignService.AddTag("GDPR Compliant", model.EmailAddress);

                //    if (ACResult1 != null && ACResult1.result_code == 0)
                //    {
                //        LoggerService.Info(GetType(), ACResult1.result_message);
                //    }
                //}

                //end subscription calls

                //Dictionary<string, string> substitutions = new Dictionary<string, string>();
                ////Get a verification token
                //string verifyLink = System.Configuration.ConfigurationManager.AppSettings["SignUpConfirmationPageUrl"];

                //substitutions.Add("%VERIFY_LINK%", String.Format(verifyLink, ResponseDTO.payload.signUp.token));
                //substitutions.Add("%FIRST_NAME%", model.FirstName);
                //MailTemplate mailTemplate = new MailTemplate
                //{
                //    Template = MailTemplate.VERIFY_EMAIL_TEMPLATE,
                //    EmailAddress = model.EmailAddress,
                //    Substitutions = substitutions,
                //    From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                //    Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                //    Subject = "Click to activate your account"
                //};

                //try
                //{
                //    await mailTemplate.Send();
                //}

                //catch (Exception e)
                //{
                //    LoggerService.Error(GetType(), e.Message, e);
                //}
            }

            var UserExists = await AccountService.UserExists(model.EmailAddress);

            if (UserExists == null)
            {
                TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString()));
                return Redirect("credit-sim-order");
                //return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.AnErrorOccurred).ToString())) });
            }


            // if user has ordered SIM
            if (model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()))
            {
                //ensure they can't create another SIM
                if (UserExists.UserExists && UserExists.HasSim)
                {
                    TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.UserHasSim).ToString()));
                    return Redirect("credit-sim-order");
                    //return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.UserHasSim).ToString())) });

                }

            }

            if (model.CountyOrProvince == null || string.IsNullOrEmpty(model.CountyOrProvince) || (model.CountyOrProvince != null && model.CountyOrProvince.Trim() == ""))
            {
                if (model.City == null || string.IsNullOrEmpty(model.City) || (model.City != null && model.City.Trim() == ""))
                {
                    model.City = "Un-known";
                }
                model.CountyOrProvince = model.City;

            }

            var Request = Mapper.Map<MailOrderRequestDTO>(model);

            //Insert Sim Order
            var Result = await AccountService.InsertSimOrder(Request);
            if (Result == null)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()) ? Urls.OrderSIM : Urls.OrderCallingCard);
            }
            if (Result.errorCode != 0)
            {
                return ErrorRedirect(((int)Messages.MailOrderError).ToString(), model.MailOrderProduct.Equals(Models.Enums.ProductCodes.THM.ToString()) ? Urls.OrderSIM : Urls.OrderCallingCard);
            }

            if (model.SignUp == "on")
            {
                //Sim Order Email With SignUp
                await OrderSimEmail(model.EmailAddress);
            }
            else
            {
                //Sim Order Email
                await OrderSimEmail(model.EmailAddress);
            }

            if (model.PortInMsisdn != null)
            {
                PortInRequestModel port = new PortInRequestModel
                {
                    Email = model.EmailAddress,
                    Code = model.PAC,
                    PortMsisdn = model.PortInMsisdn,
                    OrderRefId = Result.reference
                };

                var portInResponse = await PortService.PortIn(port, PortTypes.PortInNew);
                if (!portInResponse.payload)
                {
                    TempData["tempError"] = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.MailOrderSuccessPortInfail).ToString()));
                    return Redirect("credit-sim-order");
                    //return Json(new { status = false, errorCode = 1, message = Text.ResourceManager.GetString(string.Format("Message_{0}", ((int)Messages.MailOrderSuccessPortInfail).ToString())) });

                }
            }
            LoggerService.Info(GetType(), string.Format("{0} {1}", "Successful mail order for:", model.EmailAddress));

            TempData["FirstName"] = Request.firstName;
            Payload.MailOrder = Request.product;
            SetPayload(Payload);
            return Redirect(Url.Action("SimOrderSuccess", "CustomPage"));

            //return Json(new { status = true, errorCode = 0, message = "Success", data = Result });
        }

        private async Task<GenericApiResponse<ResetPasswordResponseDTO>> ForgotPasswordEmail(string email)
        {
            var response = await AccountService.InsertPasswordToken(email);
            string resetConfUrl = "";
            GenericApiResponse<ResetPasswordResponseDTO> Gapi = new GenericApiResponse<ResetPasswordResponseDTO>();

            ResetPasswordResponseDTO reset = new ResetPasswordResponseDTO
            {
                resetPassword = new ResetPassword { isSuccess = false }
            };

            reset.resetPassword.isSuccess = false;
            if (response != null && !Convert.ToBoolean(response.ReturnCode))
            {
                var getUserName = await AccountService.SubscriberName(email);
                resetConfUrl = ConfigurationManager.AppSettings["AuthenticationService:ResetPasswordConfirmationPageUrl"].ToString();
                string vLink = resetConfUrl.Replace("{token}", response.UniqueId);
                Dictionary<string, string> substitutions = new Dictionary<string, string>();
                substitutions.Add("%NAME%", getUserName);
                substitutions.Add("%URL%", vLink);

                MailTemplate mailTemplate = new MailTemplate
                {
                    Template = MailTemplate.FORGOT_PASSWORD,
                    EmailAddress = email,
                    Substitutions = substitutions,
                    From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom_ForgotPassword"],
                    Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                    Subject = "Reset your password"
                };

                try
                {
                    await mailTemplate.Send();
                }
                catch (Exception e)
                {
                    LoggerService.Error(GetType(), e.Message, e);
                    return null;
                }


                reset.resetPassword.isSuccess = true;
                return new GenericApiResponse<ResetPasswordResponseDTO>
                {
                    errorCode = 0,
                    message = "success",
                    payload = reset
                };

            }
            else
            {
                return new GenericApiResponse<ResetPasswordResponseDTO>
                {
                    errorCode = 982,
                    message = "failure",
                    payload = reset
                };
            }

        }

        private async Task OrderSimEmail(string email, string token = null)
        {
            Dictionary<string, string> substitutions = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(token))
            {
                //Get a verification token
                string verifyLink = System.Configuration.ConfigurationManager.AppSettings["SignUpConfirmationPageUrl"];

                substitutions.Add("%VERIFY_LINK%", String.Format(verifyLink, token));
            }

            MailTemplate mailTemplate = new MailTemplate
            {
                Template = string.IsNullOrEmpty(token) ? MailTemplate.ORDER_SIM : MailTemplate.ORDER_SIM_WITH_SIGNUP,
                EmailAddress = email,
                Substitutions = substitutions,
                From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom_ForgotPassword"],
                Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                Subject = "Your Talk Home Mobile SIM order confirmation"
            };

            try
            {
                await mailTemplate.Send();
            }
            catch (Exception e)
            {
                LoggerService.Error(GetType(), e.Message, e);
            }

        }

        [HttpGet]
        public JsonResult GetCountrie()
        {
            var Countries = AccountService.GetCountries();
            if (Countries != null)
            {
                Countries = Countries.AsEnumerable()
                                      .OrderBy(x => x.ISO_Code != "GB")
                                      .ToList();
            }

            return Json(new { errorcode = 0, status = "success", countriesinfo = Countries }, JsonRequestBehavior.AllowGet);
        }



        //[HttpPost]
        //[Route("creditsimcheckout")]
        //public ActionResult CreditSimCheckout(CreditSimCheckoutModel model)
        //{

        //    TempData["PurchaseType"] = "CreditSimOrder";
        //    var Payload = GetPayload();
        //    Payload.TopUp.Clear();
        //    Payload.Purchase.Clear();

        //    var TotalCreditSimOrder = model.Amount;

        //    // Bundle
        //    var ProductPrice = 0;
        //    var BundleId = 0;
        //    if (model.BundleId != 0)
        //    {
        //        var Product = ContentService.GetProducts(model.BundleId);
        //        ProductPrice = Product.ProductPrice;
        //        BundleId = Product.Id;
        //        Payload.Purchase.Add(model.BundleId);
        //    }
        //    //Top Up
        //    Payload.TopUp.Add(model.TopUpId);
        //    Payload.Checkout = new CheckoutPageDTO { Verify = model.ProductCode, ProductType = ProductType.CreditSimOrder.ToString(), Total = TotalCreditSimOrder };
        //    Response.Cookies.Add(AccountService.EncodeCookie(Payload));
        //    return Json(new { status = true, errorCode = 0, message = "success", data = Urls.Checkout });
        //}
    }
}
