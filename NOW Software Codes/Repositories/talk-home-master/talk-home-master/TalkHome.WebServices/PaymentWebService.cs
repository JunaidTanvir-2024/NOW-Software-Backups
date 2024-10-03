using Newtonsoft.Json;
using System.Threading.Tasks;
using TalkHome.Models.Enums;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.Payment;
using TalkHome.Logger;
using TalkHome.Models.WebApi.DTOs;


namespace TalkHome.WebServices
{
    /// <summary>
    /// Handles requests/responses for payment-related calls
    /// </summary>
    public class PaymentWebService : IPaymentWebService
    {
        private readonly IHttpService HttpService;
        private readonly ILoggerService LoggerService;
        private readonly ApiRequestType Payment = ApiRequestType.Payment;
        private Properties.URIs URIs = Properties.URIs.Default;

        public PaymentWebService(IHttpService httpService, ILoggerService loggerService)
        {
            HttpService = httpService;
            LoggerService = loggerService;
        }



        /// <summary>
        /// Gets a customer's payment details via the MiPay payment service
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<MiPayCustomerModel>> GetCustomer(GetCustomerRequestModel model)
        {
            LoggerService.Debug(GetType(), model);


            if(model.UniqueIDValue!=null && model.UniqueIDType!=null && model.ChannelType!=null)
            {
                var Json = JsonConvert.SerializeObject(model);
                var Result = await HttpService.Post(URIs.GetCustomer, Json, null, Payment);

                if (Result == null)
                    return null;

                LoggerService.Debug(GetType(), Result);

                return JsonConvert.DeserializeObject<GenericApiResponse<MiPayCustomerModel>>(Result);
            }

            GenericApiResponse<MiPayCustomerModel> miPayCust = new GenericApiResponse<MiPayCustomerModel>();
            return miPayCust;
        }

        /// <summary>
        /// Sends a request to begin a payment transaction
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<StartPaymentResponseDTO>> StartPayment(StartPaymentRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.StartPayment, Json, null, Payment);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<StartPaymentResponseDTO>>(Result);
        }

        /// <summary>
        /// Retrieves information about a payment transaction
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<PaymentRetrieveResponse>> PaymentRetrieve(PaymentRetrieveRequest model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.PaymentRetrieve, Json, null, Payment);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<PaymentRetrieveResponse>>(Result);
        }

        /// <summary>
        /// Processes a request for a One-click checkout
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<OneClickCheckoutResponse>> OneClickCheckout(OneClickCheckoutRequest model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.OneClickCheckout, Json, null, Payment);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<OneClickCheckoutResponse>>(Result);
        }

        /// <summary>
        /// Retrieves information about a One-click transaction
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<OneClickCheckoutResponse>> OneClickRetrieve(RetrieveOneClickRequest model)
        {
            LoggerService.Debug(GetType(), model);

            var Json = JsonConvert.SerializeObject(model);
            var Result = await HttpService.Post(URIs.OneClickRetrieve, Json, null, Payment);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<OneClickCheckoutResponse>>(Result);
        }

        /// <summary>
        /// Retrieves information about a One-click transaction
        /// </summary>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<VerifyNumberResponseDTO>> VerifyNumber(VerifyNumberRequestDTO model)
        {
            LoggerService.Debug(GetType(), model);

            var Result = await HttpService.Get(string.Format("/{0}/{1}", model.ProductCode, model.MsisdnOrCardNumber), Payment);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<VerifyNumberResponseDTO>>(Result);
        }
    }
}
