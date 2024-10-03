using TalkHome.Interfaces;
using System.Threading.Tasks;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.Payment;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models;
using TalkHome.Models.Enums;
using System.Linq;
using TalkHome.Models.ViewModels.Payment;
using System.Web;
using System;
using AutoMapper;
using System.Collections.Generic;
using TalkHome.Logger;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Extensions;
using System.Text.RegularExpressions;

namespace TalkHome.Services
{
    /// <summary>
    /// Service layer for payments. Interfaces with the PaymentWebservice
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentWebService PaymentWebService;
        private readonly IContentService ContentService;
        private readonly IAccountService AccountService;
        private readonly ILoggerService LoggerService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public PaymentService(IPaymentWebService paymentWebService, IContentService contentService, IAccountService accountService, ILoggerService loggerService)
        {
            PaymentWebService = paymentWebService;
            LoggerService = loggerService;
            ContentService = contentService;
            AccountService = accountService;
        }

        /// <summary>
        /// Returns the current domain
        /// </summary>
        /// <returns>The domain</returns>
        private string GetDomain()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

        /// <summary>
        /// Logs and sends a Zabbix alert
        /// </summary>
        /// <returns>The error view</returns>
        private void NumberVerificationFailed()
        {
            //TODO: Log Critical - App start payment call failed
            LoggerService.SendCriticalAlert((int)Messages.NumberVerificationFailed);
        }

        /// <summary>
        /// Performs a request to get info about a One-click transaction
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<OneClickCheckoutResponse>> OneClickRetrieve(RetrieveOneClickRequest model)
        {
            return await PaymentWebService.OneClickRetrieve(model);
        }

        /// <summary>
        /// Performs a request to get info about a MiPay customer
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<OneClickCheckoutResponse>> OneClickCheckout(OneClickCheckoutRequest model)
        {
            return await PaymentWebService.OneClickCheckout(model);
        }

        /// <summary>
        /// Performs an async request to get info about a MiPay customer
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<MiPayCustomerModel>> GetCustomer(GetCustomerRequestModel model)
        {
            return await PaymentWebService.GetCustomer(model);
        }

        /// <summary>
        /// Performs an async request to start a transaction
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<StartPaymentResponseDTO>> StartPayment(StartPaymentRequestDTO model)
        {
            return await PaymentWebService.StartPayment(model);
        }

        /// <summary>
        /// Performs an async request to retrieve a payment info
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<PaymentRetrieveResponse>> PaymentRetrieve(PaymentRetrieveRequest model)
        {
            return await PaymentWebService.PaymentRetrieve(model);
        }

        /// <summary>
        /// Performs an async request to verify a customer phone or card number
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<VerifyNumberResponseDTO>> VerifyNumber(VerifyNumberRequestDTO model)
        {
            return await PaymentWebService.VerifyNumber(model);
        }

        /// <summary>
        /// Creates Nowtel codes for a top up request
        /// </summary>
        /// <param name="productCode">The standardised Nowtel product code</param>
        /// <param name="denomination">The item's denomination</param>
        /// <param name="reference">The App user Msisdn</param>
        /// <returns>The Top up code</returns>
        /// <seealso cref="https://github.com/nowtel/talk-home/wiki/Product-codes"/>
        private string GenerateTopUpCode(string productCode, decimal denomination, string reference)
        {
            return string.Format("{0}/{1}/{2}/{3}/0", productCode, (int)ProductType.TopUp, denomination * 100, reference);
        }

        /// <summary>
        /// Creates Nowtel codes for a top up request
        /// </summary>
        /// <param name="productCode">The standardised Nowtel product code</param>
        /// <param name="denomination">The item's denomination</param>
        /// <param name="reference">The App user Msisdn</param>
        /// <returns>The Top up code</returns>
        /// <seealso cref="https://github.com/nowtel/talk-home/wiki/Product-codes"/>
        private string GenerateTransferCode(string productCode, string denomination, string reference)
        {
            return string.Format("{0}/{1}/{2}/{3}/0", productCode, (int)ProductType.TopUp, denomination, reference);
        }


        /// <summary>
        /// Creates Nowtel codes for a purchase request
        /// </summary>
        /// <param name="productCode">The standardised Nowtel product code</param>
        /// <param name="denomination">The item's denomination</param>
        /// <param name="reference">The App user Msisdn</param>
        /// <param name="guid">The bundle Guid</param>
        /// <returns>the list of codes</returns>
        private string GeneratePurchaseCode(string productCode, decimal denomination, string reference, string guid)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return string.Format("{0}/{1}/{2}/{3}/{4}", productCode, (int)ProductType.Bundle, denomination * 100, 1, 0);
            else
                return string.Format("{0}/{1}/{2}/{3}/{4}", productCode, (int)ProductType.Bundle, denomination * 100, reference, guid);
        }

        private string GenerateTHCCPurchaseCode(string productCode,
            decimal denomination,
            string reference,
            string guid,
            bool isPin)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                if(isPin)
                    return string.Format("{0}/{1}/{2}/{3}/{4}", productCode, 1, denomination * 100, 0, 0);
                else
                    return string.Format("{0}/{1}/{2}/{3}/{4}", productCode, 1, denomination * 100, 1, 0);
            }
            else
                return string.Format("{0}/{1}/{2}/{3}/{4}", productCode, 1, denomination * 100, reference, guid);
        }

        private string GeneratePurchaseCode(string productCode, 
            decimal denomination, 
            string reference, 
            string guid,
            bool isPin)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return string.Format("{0}/{1}/{2}/{3}/{4}", productCode, (int)ProductType.Bundle, denomination * 100, 1, 0);
            else
                return string.Format("{0}/{1}/{2}/{3}/{4}", productCode, (int)ProductType.Bundle, denomination * 100, reference, guid);
        }

        /// <summary>
        /// Creates well-formatted Nowtel product code for a top up
        /// </summary>
        /// <param name="id">The product Id</param>
        /// <param name="reference">The App user Msisdn</param>
        /// <returns>The list of Nowtel codes</returns>
        private List<string> GenerateTopUpCheckout(int id, string reference)
        {
            var Product = ContentService.GetProducts(id);
            var NowtelCodes = new List<string>();

            var ProductCode = ContentService.GetProductCode(Product);
            if (reference != null)
                reference = reference.Trim();
            NowtelCodes.Add(GenerateTopUpCode(ProductCode, Product.ProductPrice, reference));

            return NowtelCodes;
        }


        /// <summary>
        /// Creates well-formatted Nowtel product code for a top up
        /// </summary>
        /// <param name="id">The product Id</param>
        /// <param name="reference">The App user Msisdn</param>
        /// <returns>The list of Nowtel codes</returns>
        private List<string> GenerateTransferCheckout(string id, string price, string reference)
        {
           
            var NowtelCodes = new List<string>();
            if (reference != null)
                reference = reference.Trim();

            NowtelCodes.Add(GenerateTransferCode(id, price, reference));

            return NowtelCodes;
        }

        /// <summary>
        /// Creates well-formatted Nowtel product codes for a purchase
        /// </summary>
        /// <param name="id">The product Id</param>
        /// <param name="reference">The App user Msisdn</param>
        /// <returns>The list of Nowtel codes</returns>
        private List<string> GeneratePurchaseCheckoutCallingCard(HashSet<int> basket, 
            string reference,
            bool isPin)
        {
            var Products = ContentService.GetProducts(basket);
            var NowtelCodes = new List<string>();

            foreach (var Product in Products)
            {
                var ProductCode = ContentService.GetProductCode(Product);
                NowtelCodes.Add(GenerateTHCCPurchaseCode(ProductCode, 
                    Product.ProductPrice, 
                    reference, 
                    Product.ProductUuid,
                    isPin));
            }

            return NowtelCodes;
        }

        /// <summary>
        /// Creates well-formatted Nowtel product codes for a purchase
        /// </summary>
        /// <param name="id">The product Id</param>
        /// <param name="reference">The App user Msisdn</param>
        /// <returns>The list of Nowtel codes</returns>
        private List<string> GeneratePurchaseCheckout(HashSet<int> basket, string reference)
        {
            var Products = ContentService.GetProducts(basket);
            var NowtelCodes = new List<string>();

            foreach (var Product in Products)
            {
                var ProductCode = ContentService.GetProductCode(Product);
                NowtelCodes.Add(GeneratePurchaseCode(ProductCode, Product.ProductPrice, reference, Product.ProductUuid));
            }

            return NowtelCodes;
        }

        /// <summary>
        /// Returns a string representing the total of the basket, optionally converts to pence/cents
        /// </summary>
        /// <param name="items">The set of products Ids</param>
        /// <param name="pence">Whether to convert to pence or not (optional)</param>
        /// <returns>The total</returns>
        public string GetCheckoutTotal(HashSet<int> items, bool pence = false)
        {
            var Products = ContentService.GetProducts(items);
            var Total = new decimal(0.0);

            foreach (var product in Products)
                Total += product.ProductPrice;

            if (!pence)
                return Total.ToString();
            else
                return (Total * 100).ToString();
        }

        /// <summary>
        /// Returns he total of the basket as decimal
        /// </summary>
        /// <param name="items">The set of products Ids</param>
        /// <returns>The total</returns>
        public decimal GetCheckoutTotal(HashSet<int> items)
        {
            var Products = ContentService.GetProducts(items);
            var Total = new decimal(0.0);

            foreach (var product in Products)
                Total += product.ProductPrice;

            return Total;
        }

        

        /// <summary>
        /// Tries to retrieve the Customer Id for one-click checkout if the customer is known
        /// </summary>
        /// <param name="result">The response model</param>
        /// <param name="custId">Ref to the customer id to set</param>
        /// <returns>The result as TRUE or FALSE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryGetCustId(GenericApiResponse<MiPayCustomerModel> result, out string custId)
        {
            if (result.errorCode != 0)
            {
                custId = null;
                return false;
            }
            if (result.payload!=null && result.payload.uniqueIDs != null)
            {
                custId = result.payload.uniqueIDs.Where(x => x.uniqueIDType.Equals("CUST_ID")).Select(x => x.uniqueIDValue).First();
                return true;
            }

            custId = null;
            return false;
        }

        /// <summary>
        /// Checks the outcome of a Start Payment request
        /// </summary>
        /// <param name="result">The response model</param>
        /// <param name="error">Ref to the error code</param>
        /// <returns>The result as FALSE or TRUE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryPaymentSuccess(GenericApiResponse<StartPaymentResponseDTO> result, out string error)
        {
            error = "";

            if (result.errorCode != 0)
            {
                error = result.errorCode.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to retrieve a transaction, if unsuccessful sets the error code
        /// </summary>
        /// <param name="payload">The JWT payload</param>
        /// <param name="error">Ref to the error code</param>
        /// <returns>The result as FALSE or TRUE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryFindTransaction(JWTPayload payload, out string error)
        {
            error = "";

            if (payload.Payment == null)
            {
                error = ((int)Messages.NoTransactionFound).ToString();
                return false;
            }  

            return true;
        }

        /// <summary>
        /// Tries to retrieve a One-click checkout, if unsuccessful sets the error code
        /// </summary>
        /// <param name="payload">The JWT payload</param>
        /// <param name="error">Ref to the error code</param>
        /// <returns>The result as FALSE or TRUE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryFindOneClick(JWTPayload payload, out string error)
        {
            error = "";

            if (payload.OneClick == null)
            {
                error = ((int)Messages.NoTransactionFound).ToString();
                return false;
            }

            return true;
        }

        public bool IsValidPostCode(string postCode, out string cleanedPostCode)
        {
            cleanedPostCode = postCode.Trim();

            if (cleanedPostCode.Length > 8)
                return false;
            else
                return true;
         
        }


        /// <summary>
        /// Defines if the outcome of a transaction was successful, if unsuccessful sets the error code
        /// </summary>
        /// <param name="result">The response model</param>
        /// <param name="error">Ref to the error string</param>
        /// <returns>The result as FALSE or TRUE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryTransactionSuccess(GenericApiResponse<PaymentRetrieveResponse> result, out string error)
        {
            error = "";

            if (result.errorCode != 0)
            {
                error = result.errorCode.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Defines if the outcome of a one-click checkout was successful, if unsuccessful sets the error code
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result as FALSE or TRUE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryOneClickSuccess(GenericApiResponse<OneClickCheckoutResponse> result, out string error)
        {
            error = "";

            if (result.errorCode != 0)
            {
                error = result.errorCode.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a payment can be processed as One-click
        /// </summary>
        /// <param name="payload">The JWT payload</param>
        /// <param name="MiPayCustomer">the model for the customer</param>
        /// <param name="cardId">the Id of the selected bank card</param>
        /// <returns>the result as FALSE or TRUE</returns>
        public bool IsOneClickElegible(JWTPayload payload, MiPayCustomerModel MiPayCustomer, StartPaymentViewModel model)
        {
            if (string.IsNullOrEmpty(model.Unreg) || model.Unreg.Equals("1")) // User has amended the billing address
                return false;

            if (string.IsNullOrEmpty(model.CardId) || model.CardId.Equals("0")) // Card not selected or "use diffrerent card" selected
                return false;

            if (string.IsNullOrEmpty(payload.CustId)) // Is a Reg customer?
                return false;

            // Select the card that should be used
            var PaymentMethod = MiPayCustomer.paymentMethods.Where(x => x.methodOfPaymentIdentifier.Equals(model.CardId)).First();
            
            if (PaymentMethod == null) // Same card from MiPay records?
                return false;
            
            return PaymentMethod.enabled; // Is the card enabled?
        }

        /// <summary>
        /// Attempts processing the One-click checkout request
        /// </summary>
        /// <param name="model">The payment request model</param>
        /// <param name="payload">The JWT payload</param>
        /// <param name="channelType">the channel requesting the transaction</param>
        /// <param name="error">Ref to the error string</param>
        /// <returns>The outcome request model or null</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public async Task<RetrieveOneClickRequest> TryOneClickTopUp(StartPaymentViewModel model, JWTPayload payload, ChannelType channelType, string error)
        {
            error = "";
            var TopUpCode = GenerateTopUpCheckout(payload.TopUp.First(), payload.Checkout.Reference);
            var MethodOfPayment = new MethodOfPayment(Models.Enums.PaymentMethod.Card.ToString(), model.CardId);
            var Model = new OneClickCheckoutRequest(payload.CustId, payload.currency, channelType.ToString(), TopUpCode, MethodOfPayment);

            var Checkout = await OneClickCheckout(Model);

            if (Checkout.errorCode != 0)
            {
                error = Checkout.errorCode.ToString();
                return null;
            }

            return new RetrieveOneClickRequest(payload.CustId);
        }

        /// <summary>
        /// Processes a One-click purchase request
        /// </summary>
        /// <param name="model">The payment request model</param>
        /// <param name="payload">The JWT payload</param>
        /// <param name="channelType">the channel requesting the transaction</param>
        /// <param name="error">Ref to the error string</param>
        /// <returns>The outcome request model or null</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public async Task<RetrieveOneClickRequest> TryOneClickPurchase(StartPaymentViewModel model, JWTPayload payload, ChannelType channelType, string error)
        {
            error = "";
            var BasketCodes = GeneratePurchaseCheckout(payload.Purchase, payload.Checkout.Reference);
            var MethodOfPayment = new MethodOfPayment(Models.Enums.PaymentMethod.Card.ToString(), model.CardId);
            var Model = new OneClickCheckoutRequest(payload.CustId, payload.currency, channelType.ToString(), BasketCodes, MethodOfPayment);

            var Checkout = await OneClickCheckout(Model);

            if (Checkout == null)
                return null;

            if (Checkout.errorCode != 0)
            {
                error = Checkout.errorCode.ToString();
                return null;
            }

            return new RetrieveOneClickRequest(payload.CustId);
        }

        /// <summary>
        /// Verifies that a customer is topping up the correct number
        /// </summary>
        /// <param name="payload"Tthe JWT token</param>
        /// <param name="number">The number to top up</param>
        /// <returns>The view</returns>
        public async Task<bool> VerifyNumber(JWTPayload payload, string number, string countryCode)
        {
            string Msisdn = "";

            // if is Msisdn, validate with phonelib
            if (payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THM.ToString()) || payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THA.ToString()))
                if (!AccountService.TryValidateNumber(number, countryCode, out Msisdn))
                    return false;

            var Verify = await VerifyNumber(new VerifyNumberRequestDTO(payload.Checkout.Verify, !Msisdn.Equals("") ? Msisdn : number));

            if (Verify == null)
            {
                NumberVerificationFailed();
                return false;
            }             
            
            if (Verify.errorCode != 0)
                return false;

            return true;
        }


        public async Task<bool> VerifyQuickTopUpNumber(string number, string countryCode)
        {
            string Msisdn = "";

            if (!AccountService.TryValidateNumber(number, countryCode, out Msisdn))
                return false;

            var Verify = await VerifyNumber(new VerifyNumberRequestDTO("THM", !Msisdn.Equals("") ? Msisdn : number));

            if (Verify == null)
            {
                NumberVerificationFailed();
                return false;
            }

            if (Verify.errorCode != 0)
                return false;

            return true;
        }

        /// <summary>
        /// Formats a Top up request from the App user inputs and other contextual info
        /// </summary>
        /// <param name="payload">The JWT token</param>
        /// <param name="input">The user input model</param>
        /// <param name="channelType">The ChannelType</param>
        /// <returns>The request model</returns>
        public StartPaymentRequestDTO CreatePaymentRequest(JWTPayload payload, StartPaymentViewModel input, ChannelType channelType)
        {
            var Domain = "";

            if(HttpContext.Current.IsDebuggingEnabled)
            {
                Domain = GetDomain();
            }
            else
            {
                Domain = new Uri(GetDomain()).ToCleanUri().ToString();
            }

            var RequestDTO = Mapper.Map<JWTPayload, StartPaymentRequestDTO>(payload);

            Mapper.Map(input, RequestDTO);

            
            if (RequestDTO.FirstName != null)
               RequestDTO.FirstName = RequestDTO.FirstName.TrimStart().TrimEnd();
            
            if (RequestDTO.LastName != null)
                RequestDTO.LastName = RequestDTO.LastName.TrimStart().TrimEnd();
            if (RequestDTO.AddressLine1 != null)
                RequestDTO.AddressLine1 = RequestDTO.AddressLine1.TrimStart().TrimEnd();
            if (RequestDTO.AddressLine2 != null)
                RequestDTO.AddressLine2 = RequestDTO.AddressLine2.TrimStart().TrimEnd();
            if (RequestDTO.City != null)
                RequestDTO.City = RequestDTO.City.TrimStart().TrimEnd();
            if (RequestDTO.CountyOrProvince != null)
                RequestDTO.CountyOrProvince = RequestDTO.CountyOrProvince.TrimStart().TrimEnd();
            

            if (RequestDTO.PostalCode != null )
                RequestDTO.PostalCode = RequestDTO.PostalCode.TrimEnd(null);
            if (RequestDTO.Msisdn != null)
                RequestDTO.Msisdn = RequestDTO.Msisdn.TrimEnd().TrimStart();

            RequestDTO.ChannelType = Enum.GetName(typeof(ChannelType), channelType);
            RequestDTO.PaymentType = Enum.GetName(typeof(PaymentType), input.Unreg.Equals("1") ? 2 : 1);

            RequestDTO.SuccessURL = string.Format("{0}{1}", Domain, channelType.ToString().Equals("Web") ? Urls.SuccessfulTopUp : Urls.SuccessfulAppCheckout);
            RequestDTO.FailureURL = string.Format("{0}{1}", Domain, channelType.ToString().Equals("Web") ? Urls.FailedTopUp : Urls.FailedAppCheckout);
            
            if (payload.Checkout.ProductType.Equals(ProductType.TopUp.ToString()))
            {
                RequestDTO.Amount = GetCheckoutTotal(payload.TopUp, true);
                RequestDTO.Basket = GenerateTopUpCheckout(payload.TopUp.First(), payload.Checkout.Reference);
            }
            else if (payload.Checkout.ProductType.Equals(ProductType.AirTimeTransfer.ToString()))
            {
                RequestDTO.SuccessURL = string.Format("{0}{1}", Domain, channelType.ToString().Equals("Web") ? Urls.SuccessfulPurchase : Urls.SuccessfulAppCheckout);
                RequestDTO.FailureURL = string.Format("{0}{1}", Domain, channelType.ToString().Equals("Web") ? Urls.FailedPurchase : Urls.FailedAppCheckout);


                RequestDTO.Amount = (decimal.Parse(payload.AirTimeTransfer.Cost) * 100).ToString("0");
                RequestDTO.Msisdn = payload.AirTimeTransfer.Msisdn;
                RequestDTO.Basket = GenerateTransferCheckout("THM", RequestDTO.Amount, payload.AirTimeTransfer.Msisdn);
            }
            else
            {

                if (payload != null && payload.Basket.Count == 0 && ((payload.ProductCodes != null && payload.ProductCodes.Where(p=>p.ProductCode.ToLower() == "thcc").Count() > 0) || 
                    (payload.CheckoutProduct!=null && payload.CheckoutProduct.ToLower()=="thcc")))
                {
                    RequestDTO.Amount = GetCheckoutTotal(payload.Purchase, true);
                    RequestDTO.Basket = GeneratePurchaseCheckoutCallingCard(payload.Purchase, payload.Checkout.Reference, payload.isTHCCPin);
                }
                else
                {
                    RequestDTO.SuccessURL = string.Format("{0}{1}", Domain, channelType.ToString().Equals("Web") ? Urls.SuccessfulPurchase : Urls.SuccessfulAppCheckout);
                    RequestDTO.FailureURL = string.Format("{0}{1}", Domain, channelType.ToString().Equals("Web") ? Urls.FailedPurchase : Urls.FailedAppCheckout);

                    if (payload.Basket.Count >0 && payload.Purchase.Count == 0)
                    {
                        RequestDTO.Amount = GetCheckoutTotal(payload.Checkout.Basket, true);
                        RequestDTO.Basket = GeneratePurchaseCheckout(payload.Checkout.Basket, payload.Checkout.Reference);
                    }
                    else
                    {
                        RequestDTO.Amount = GetCheckoutTotal(payload.Purchase, true);
                        RequestDTO.Basket = GeneratePurchaseCheckout(payload.Purchase, payload.Checkout.Reference);
                    }
                    
                }
            }

            return RequestDTO;
        }
    }
}
