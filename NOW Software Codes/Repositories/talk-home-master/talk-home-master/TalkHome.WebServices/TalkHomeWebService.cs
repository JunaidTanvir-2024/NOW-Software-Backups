using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalkHome.Models;
using TalkHome.Models.Porting;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models.Enums;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.History;
using TalkHome.Models.WebApi.Rates;
using TalkHome.Models.WebApi.CallingCards;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Logger;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System;
using TalkHome.Models.ViewModels;
using TalkHome.Models.Pay360;

namespace TalkHome.WebServices
{
    /// <summary>
    /// Manages requests and response for the WebApi
    /// </summary>
    public class TalkHomeWebService : ITalkHomeWebService
    {
        private readonly IHttpService HttpService;
        private readonly ILoggerService LoggerService;
        private ApiRequestType WebApi = ApiRequestType.WebApi;
        private ApiRequestType ThmApi = ApiRequestType.ThmApi;
        private Properties.URIs URIs = Properties.URIs.Default;

        public TalkHomeWebService(IHttpService httpService, ILoggerService loggerService)
        {
            HttpService = httpService;
            LoggerService = loggerService;
        }


        /// <summary>
        /// Gets call history by page number and number of results per page
        /// </summary>
        /// <param name="model">The request model.</param>
        /// <returns>The API response model.</returns>
        public async Task<GenericApiResponse<List<CallHistoryRecord>>> GetCallHistoryPage(HistoryPageDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.CallHistoryPage, Json, model.token, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<List<CallHistoryRecord>>>(Result);
        }

        /// <summary>
        /// Gets the total number of pages for the call history
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The API response model</returns>
        public async Task<GenericApiResponse<TotalPages>> GetCallHistoryTotalPages(GetTotalPages model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.CallHistoryTotalPages, Json, model.token, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<TotalPages>>(Result);
        }

        /// <summary>
        /// Gets payment history by page number and number of results per page
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The API response model</returns>
        public async Task<GenericApiResponse<List<PaymentHistoryRecord>>> GetPaymentHistoryPage(HistoryPageDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.PaymentHistoryPage, Json, model.token, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<List<PaymentHistoryRecord>>>(Result);
        }

        /// <summary>
        /// Gets the total number of pages for the payment history
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The API response model</returns>
        public async Task<GenericApiResponse<TotalPages>> GetPaymentHistoryTotalPages(GetTotalPages model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.PaymentHistoryTotalPages, Json, model.token, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<TotalPages>>(Result);
        }

        /// <summary>
        /// Signs up the user for an account on the website
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The API response model</returns>
        public async Task<GenericApiResponse<SignUpResponseDTO>> SignUp(SignUpRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.SignUp, Json, null, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<SignUpResponseDTO>>(Result);
        }

        /// <summary>
        /// Attempts confirming the email address of the customer
        /// </summary>
        /// <param name="token">The verification token send via email to the customer</param>
        /// <returns>The API response model</returns>
        public async Task<GenericApiResponse<VerifySignUpResponseDTO>> VerifySignUpEmail(string token)
        {
            LoggerService.Debug(GetType(), token);

            var Result = await HttpService.Get(string.Format("{0}/{1}", URIs.VerifySignUpEmail, token), WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<VerifySignUpResponseDTO>>(Result);
        }





        /// <summary>
        /// Processes a request for a mail order for SIM Cards or Rechargeable Calling Cards
        /// </summary>
        /// <returns>The response model</returns>
        /// <remarks>Please note that the payload of this response is always empty</remarks>
        public async Task<GenericApiResponse<int>> MailOrder(MailOrderRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.MailOrder, Json, null, WebApi);

            if (Result == null)
            {
                LoggerService.SendCriticalAlert((int)Messages.MailOrderException);
                return null;
            }

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<int>>(Result);
        }

        public async Task<GenericPay360ApiResponse<CreditSimOrderResponse>> CreditSimOrder(CreditSimOrderApiRequest model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.CreditSimOrder, Json, null, ApiRequestType.ThmApi);

            if (Result == null)
            {
                LoggerService.SendCriticalAlert((int)Messages.MailOrderException);
                return null;
            }

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericPay360ApiResponse<CreditSimOrderResponse>>(Result);
        }

        /// <summary>`
        /// Sends the request for the account summary shown on My Account page
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The API response model</returns>
        public async Task<GenericApiResponse<AccountSummaryResponseDTO>> GetAccountSummary(AccountSummaryRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(string.Format("/{0}{1}", model.productCode, URIs.AccountSummary), Json, model.token, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<AccountSummaryResponseDTO>>(Result);
        }

        /// <summary>
        /// Gets the details of the authorized customer
        /// </summary>
        /// <param name="apiToken">The authorization token</param>
        /// <returns>The API response model</returns>
        public async Task<GenericApiResponse<AccountDetailsResponseDTO>> GetAccountDetails(string apiToken)
        {
            LoggerService.Debug(GetType(), apiToken);

            var Json = JsonConvert.SerializeObject("");
            var Result = await HttpService.Post(URIs.AccountDetails, Json, apiToken, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<AccountDetailsResponseDTO>>(Result);
        }

        /// <summary>
        /// Sends a request to add a product for a registred account
        /// </summary>
        /// <param name="model">The request model</param>
        /// <param name="apiToken">The authorization token</param>
        /// <returns>The API response model</returns>
        public async Task<GenericApiResponse<AddProductResponseDTO>> AddProduct(AddProductRequestDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(string.Format("{0}/{1}/add", URIs.AddProduct, model.productCode), Json, apiToken, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<AddProductResponseDTO>>(Result);
        }

        /// <summary>
        /// Attempts a customer authentication with the WebApi
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<LoginResponseDTO<AuthenticationContent>>> AuthenticateCustomer(LoginRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.Authenticate, Json, null, WebApi);

            if (Result == null)
            {
                LoggerService.SendCriticalAlert((int)Messages.WebApiLoginException);
                return null;
            }

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<LoginResponseDTO<AuthenticationContent>>>(Result);
        }

        /// <summary>
        /// Begins process to reset a password. The customer receives an email with a link to follow
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<ResetPasswordResponseDTO>> ResetPasswordRequest(ResetPasswordRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.ResetPasswordRequest, Json, null, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<ResetPasswordResponseDTO>>(Result);
        }

        /// <summary>
        /// The customer has followed the previously emailed link
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<ResetPasswordConfirmResponseDTO>> ResetPasswordConfirm(ResetPasswordConfirmRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.ResetPasswordConfirm, Json, null, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<ResetPasswordConfirmResponseDTO>>(Result);
        }

        /// <summary>
        /// The customer performs an attempt to reset the password
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<NewPasswordResponseDTO>> NewPassword(NewPasswordRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.NewPassword, Json, null, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<NewPasswordResponseDTO>>(Result);
        }

        /// <summary>
        /// Gets Talk Home Mobile top rates
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<IList<Rate>>> GetTalkHomeMobileTopRates()
        {
            var Result = await HttpService.Get(URIs.MobileTopRates, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<IList<Rate>>>(Result);
        }

        /// <summary>
        /// Gets Talk Home App top rates for a given country
        /// </summary>
        /// <param name="countryCode">The country code</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<IList<Rate>>> GetTalkHomeAppTopRates(string countryCode)
        {
            LoggerService.Debug(GetType(), countryCode);

            var Result = await HttpService.Get(string.Format("{0}/{1}", URIs.TalkHomeAppRates, countryCode), WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<IList<Rate>>>(Result);
        }

        /// <summary>
        /// Gets the top minutes for calling cards
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<IList<TopMinute>>> GetCallingCardsTopMinutes()
        {
            var Result = await HttpService.Get(URIs.CallingCardsTopMinutes, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<IList<TopMinute>>>(Result);
        }

        /// <summary>
        /// Gets Talk Home Mobile UK rates
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<IList<UKNationalRate>>> GetTalkHomeMobileUKRates()
        {
            var Result = await HttpService.Get(URIs.TalkHomeMobileUKRates, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<IList<UKNationalRate>>>(Result);
        }

        /// <summary>
        /// Gets Talk Home Mobile international rates
        /// </summary>
        /// <returns></returns>
        public async Task<GenericApiResponse<IList<Rate>>> GetTalkHomeMobileInternationalRates()
        {
            var Result = await HttpService.Get(URIs.TalkHomeMobileInternationalRates, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<IList<Rate>>>(Result);
        }

        /// <summary>
        /// Gets Talk Home Mobile roaming rates
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<IList<RoamingRate>>> GetTalkHomeMobileRoamingRates()
        {
            var Result = await HttpService.Get(URIs.TalkHomeMobileRoamingRates, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<IList<RoamingRate>>>(Result);
        }

        /// <summary>
        /// Gets Talk Home App rates for a given country
        /// </summary>
        /// <param name="countryCode">The alpha-2 country code</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<IList<Rate>>> GetTalkHomeAppRates(string countryCode)
        {
            var Result = await HttpService.Get(string.Format("{0}/{1}", URIs.TalkHomeAppRates, countryCode), WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<IList<Rate>>>(Result);
        }

        /// <summary>
        /// Gets all minutes for calling cards
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<IList<MinutesRecord>>> GetCallingCardsMinutes()
        {
            var Result = await HttpService.Get(URIs.CallingCardsMinutes, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<IList<MinutesRecord>>>(Result);
        }

        /// <summary>
        /// Processes a request for a promotional sign up
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        /// <remarks>Please note that the payload of this response is always empty</remarks>
        public async Task<GenericApiResponse<string>> PromoSignUp(PromoSignUpRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.PromoSignUp, Json, null, WebApi);

            if (Result == null)
            {
                LoggerService.SendCriticalAlert((int)Messages.PromoSignUpException);
                return null;
            }

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<string>>(Result);
        }

        /// <summary>
        /// Redeems a voucher
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<string>> RedeemVocuher(RedeemVoucherRequestDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(string.Format("/{0}{1}", model.ProductCode, URIs.RedeemVoucher), Json, apiToken, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<string>>(Result);
        }

        /// <summary>
        /// Redeems points
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<string>> RedeemPoints(RedeemPointsRequestDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(string.Format("/{0}{1}", model.ProductCode, URIs.ReedemPoints), Json, apiToken, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<string>>(Result);
        }

        /// <summary>
        /// Requests an auto renew for bundles
        /// </summary>
        /// <param name="model">The request model</param>
        /// <param name="apiToken">The logged in customer api token</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<string>> AutoRenewSettings(AutoRenewSettingsRequestDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.AutoRenewBundles, Json, apiToken, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<string>>(Result);
        }


        public async Task<GenericApiResponse<string>> AutoRenewSettings(AutoRenewalsSettingsRequestDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.AutoRenewBundles, Json, apiToken, ThmApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<string>>(Result);
        }


        /// <summary>
        /// Requests an auto top up
        /// </summary>
        /// <param name="model">The request model</param>
        /// <param name="apiToken">The logged in customer api token</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<string>> AutoTopUpSettings(AutoTopUpSettingsRequestDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.AutoRenewTopUp, Json, apiToken, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);
            return JsonConvert.DeserializeObject<GenericApiResponse<string>>(Result);
        }

        async public Task<GenericApiResponse<UpdateAddressResponseDTO>> UpdateAddress(UpdateAddressRequestDTO model, string addresstype, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            string Url = string.Empty;
            if (addresstype == "home")
            {
                Url = URIs.UpdateAddressHome;
            }
            if (addresstype == "billing")
            {
                Url = URIs.UpdateAddressBilling;
            }
            var Result = await HttpService.Post(Url, Json, apiToken, WebApi);
            if (Result == null)
            {
                LoggerService.SendCriticalAlert((int)Messages.UnknownError);
                return null;
            }
            LoggerService.Debug(GetType(), Result);
            return JsonConvert.DeserializeObject<GenericApiResponse<UpdateAddressResponseDTO>>(Result);

        }


        public async Task<GenericApiResponse<UpdatePasswordResponseDTO>> UpdatePassword(UpdatePasswordRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post("/talkhome/useraccount/resetpassword/change", Json, null, WebApi);

            if (Result == null)
            {
                LoggerService.SendCriticalAlert((int)Messages.UnknownError);
                return null;
            }

            LoggerService.Debug(GetType(), Result);
            return JsonConvert.DeserializeObject<GenericApiResponse<UpdatePasswordResponseDTO>>(Result);
        }

        /// <summary>
        /// Add Bundle with credit
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<string>> AddBundleWithCredit(AddBundleDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(string.Format("/talkhome/{0}/bundles/add", model.ProductCode), Json, apiToken, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<string>>(Result);
        }

        /// <summary>
        /// Add Bundle with credit
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<string>> TransferCredit(TransferCreditDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            //var Result = await HttpService.Post("/talkhome/THM/transfercredit", Json, apiToken, WebApi);

            var Result = "{ \"status\":200,\"message\":\"Success\",\"errorCode\":0}";

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<string>>(Result);
        }

        /// <summary>

        public async Task<GenericApiResponse<string>> ThmCreditAutoTopupAccountBalance(string subscriberid,
           string transReferenceId,
           decimal amountinPounds,
           string PaymentMethod)
        {
            //********************************************************************//
            //              MUST PASS "AMOUNT IN POUNDS ONLY" IN THM              //
            //********************************************************************//
            //CreditApi cr = new CreditApi();


            var client = new System.Net.Http.HttpClient()
            {
                //BaseAddress = new Uri(ConfigurationManager.AppSettings["DigitalkPayment:ThmHost"].ToString())
                BaseAddress = new Uri("http://172.16.120.188/")
            };
            client.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
            //var mbyte = System.Text.Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["DigitalkPayment:ThmAuth"].ToString());
            var mbyte = System.Text.Encoding.ASCII.GetBytes("Webuser: Webuser1");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(mbyte));

            try
            {
                string adjustmentReason = "TalkHome Air Time Transfer";

                string paymentMethod = "Card Auto topup";
                if (PaymentMethod == "paypal")
                    paymentMethod = "paypal topup";

                try
                {
                    var body = JsonConvert.SerializeObject(new
                    {
                        AdjustmentReason = adjustmentReason,
                        Reference = transReferenceId,
                        PaymentMethod = paymentMethod,
                        RechargeType = 45,
                        Amount = amountinPounds,
                        Channel = 1
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
                    });

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/subscribers/{subscriberid}/payments/debit")
                    {
                        Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
                    };


                    var response = await client.SendAsync(requestMessage);

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    if (response.Content != null)
                    {


                        return JsonConvert.DeserializeObject<GenericApiResponse<string>>(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// DeductBalanceAfterTransferBalance
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<ThmDebitAccountBalanceResponseDTO>> ThmDebitAccountBalance(ThmDebitAccountBalanceDTO model, string apiToken)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post("/payment/thmDebitAccountBalance", Json, apiToken, WebApi);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<ThmDebitAccountBalanceResponseDTO>>(Result);
        }

        public async Task<GenericPay360ApiResponse<string>> CreditSimPayment(CreditSimPaymentApiRequest model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.CreditSimPayment, Json, null, ApiRequestType.ThmApi);

            if (Result == null)
            {
                return null;
            }

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericPay360ApiResponse<string>>(Result);
        }

        public async Task<GenericApiAppResponse<string>> CreditSimFullfillment(string Msisdn)
        {
            CreditSimFullfillmentRequestModel model = new CreditSimFullfillmentRequestModel { msisdn = Msisdn };
            LoggerService.Debug(GetType(), model);
            var Josn = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.CreditSimFullfilment, Josn, null,ApiRequestType.ThmApi);
            if (Result == null )
            {
                return null;
            }
            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiAppResponse<string>>(Result);

        }





}
}
