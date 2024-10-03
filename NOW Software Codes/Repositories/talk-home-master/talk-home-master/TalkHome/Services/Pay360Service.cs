using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TalkHome.Extensions;
using TalkHome.Interfaces;
using TalkHome.Models;
using TalkHome.Models.Enums;
using TalkHome.Models.Pay360;
using TalkHome.Models.ViewModels.Pay360;
using TalkHome.Models.WebApi.Payment;



namespace TalkHome.Services
{

    public class Pay360Service : IPay360Service
    {
        private readonly IAccountService AccountService;

        private readonly string Pay360APiEndpoint;
        private readonly bool Pay360Api3DSecure;

        public Pay360Service(IAccountService accountService)
        {
            AccountService = accountService;
            Pay360APiEndpoint = ConfigurationManager.AppSettings["Pay360ApiEndpoint"];
            Pay360Api3DSecure = Convert.ToBoolean(ConfigurationManager.AppSettings["Pay360Api3DSecure"]);
        }


        public string GetResumeUrl(string path)
        {
            string Domain = "";

            if (HttpContext.Current.IsDebuggingEnabled)
            {
                Domain = GetDomain() + "/" + path;
            }
            else
            {
                Domain = new Uri(GetDomain()).ToCleanUri().ToString();

                if (Domain.Contains("172.24.1.197"))
                {
                    string LabDomainWithPort = "172.24.1.197:" + HttpContext.Current.Request.Url.Port.ToString();

                    Domain = Domain.Replace("172.24.1.197", LabDomainWithPort);
                }
                else
                {
                    Domain = Domain.Replace("http", "https");
                }

                Domain = Domain + path;
            }

            return Domain;

        }

        public async Task<GenericPay360ApiResponse<Pay360PaymentResponse>> Resume3DTransaction(Pay360Resume3DRequest request)
        {

            string endpoint = Pay360APiEndpoint + "Pay360CashierApi/Resume3DSecureTransaction";

            GenericPay360ApiResponse<Pay360PaymentResponse> ret = new GenericPay360ApiResponse<Pay360PaymentResponse>();

            var Json = JsonConvert.SerializeObject(request);
            var Result = await Post(endpoint, Json);

            if (Result == null)
            {
                return null;
            }

            //LoggerService.Debug(GetType(), Result);
            ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<Pay360PaymentResponse>>(Result);

            return ret;

        }

        public async Task<GenericPay360ApiResponse<Pay360CardsResponse>> Pay360GetCards(Pay360CustomerRequestModel request)
        {
            GenericPay360ApiResponse<Pay360CardsResponse> ret = new GenericPay360ApiResponse<Pay360CardsResponse>();

            string endpoint = Pay360APiEndpoint + "Pay360CommonServices/GetCustomerPaymentMethodsByCustomerUniqueRef";

            var json = JsonConvert.SerializeObject(request);

            var Result = await Post(endpoint, json);

            if (Result == null)
            {
                return null;
            }

            //LoggerService.Debug(GetType(), Result);

            ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<Pay360CardsResponse>>(Result);

            return ret;
        }

        /// <summary>
        /// Gets a customer's payment details via the MiPay payment service
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericPay360ApiResponse<Pay360CustomerModel>> GetCustomer(Pay360CustomerRequestModel model)
        {

            GenericPay360ApiResponse<Pay360CustomerModel> ret = new GenericPay360ApiResponse<Pay360CustomerModel>();

            string endpoint = "";

            endpoint = Pay360APiEndpoint + "Pay360CommonServices/GetCustomerByCustomerUniqueRef";
           

            var Json = JsonConvert.SerializeObject(model);

            var Result = await Post(endpoint, Json);

            if (Result == null)
            {
                return null;
            }

            //var Result = "{ \"message\": \"Cutomer details retrieved successfully\", \"status\": \"Success\", \"errorCode\": 0, \"payload\": { \"displayName\": \"Ali\", \"merchantRef\": \"923465392222\", \"pay360CustId\": 2072830, \"email\": null, \"defaultCurrency\": null, \"dob\": null, \"addressLine1\": null, \"addressLine2\": null, \"addressLine3\": null, \"addressLine4\": null, \"city\": null, \"region\": null, \"postCode\": null, \"country\": null, \"countryCode\": null, \"telephone\": null } }";

            //LoggerService.Debug(GetType(), Result);

            ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<Pay360CustomerModel>>(Result);

            return ret;


        }



        /// <summary>
        /// Returns he total of the basket as decimal
        /// </summary>
        /// <param name="items">The set of products Ids</param>
        /// <returns>The total</returns>
        public decimal GetCheckoutTotal(HashSet<int> items)
        {
            ContentService cs = new ContentService();
            var Products = cs.GetProducts(items);

            var Total = new decimal(0.0);

            foreach (var product in Products)
            {
                Total += product.ProductPrice;
            }

            return Total;
        }


        private Pay360PaymentRequest MapPayloadCreditSim(Pay360PaymentRequest request, Pay360PaymentType paymentType, JWTPayload Payload, string IP)
        {
            bool isLoggedIn = AccountService.IsAuthorized(Payload);
            List<basket> basket = new List<basket>();
            string msisdn = "";
            string prodRef = "";
            float total = 0F;
            ContentService cs = new ContentService();
           
             if (Payload.Checkout.ProductType.Equals(ProductType.CreditSimOrder.ToString()) && Payload.Checkout.Basket.Count > 0)
            {
                var Products = cs.GetProducts(Payload.Checkout.Basket);
                foreach (var product in Products)
                {
                    total += product.ProductPrice;
                    basket.Add(new basket
                    {
                        amount = product.ProductPrice,
                        bundleRef = product.ProductUuid,
                        productItemCode = "THMCRSIM",
                        productRef = (Payload.CreditSim != null && Payload.CreditSim.userId != 0)?Payload.CreditSim.userId.ToString() : string.Empty
                    });
                }
            }
            switch (paymentType)
            {
                case Pay360PaymentType.New:
                    request.Pay360PaymentRequestNew.basket = basket;

                    if (isLoggedIn)
                    {
                        request.Pay360PaymentRequestNew.customerMsisdn = "";
                        request.Pay360PaymentRequestNew.customerUniqueRef = Payload.FullName.Email;
                        request.Pay360PaymentRequestNew.customerEmail = (Payload.CreditSim != null) ? Payload.CreditSim.Email : string.Empty;
                    }
                    else
                    {
                        request.Pay360PaymentRequestNew.customerMsisdn = "";
                        request.Pay360PaymentRequestNew.customerUniqueRef = (Payload.CreditSim != null && Payload.CreditSim.Email != null)? Payload.CreditSim.Email :string.Empty;
                        if (Payload.CreditSim != null && Payload.CreditSim.Email != null)
                        {
                            request.Pay360PaymentRequestNew.customerEmail = (Payload.CreditSim != null) ? Payload.CreditSim.Email : string.Empty;
                        }
                    }

                    request.Pay360PaymentRequestNew.transactionCurrency = Payload.currency;
                    request.Pay360PaymentRequestNew.ipAddress = IP;
                    request.Pay360PaymentRequestNew.productCode = Payload.Checkout.Verify;
                    request.Pay360PaymentRequestNew.transactionAmount = total;
                    break;
                case Pay360PaymentType.Token:
                    request.Pay360PaymentRequestToken.basket = basket;
                    request.Pay360PaymentRequestToken.customerMsisdn = "";
                    request.Pay360PaymentRequestToken.customerUniqueRef = Payload.FullName.Email;
                    request.Pay360PaymentRequestToken.customerEmail =(Payload.CreditSim != null)? Payload.CreditSim.Email: string.Empty;
                    request.Pay360PaymentRequestToken.transactionCurrency = Payload.currency;
                    request.Pay360PaymentRequestToken.ipAddress = IP;
                    request.Pay360PaymentRequestToken.productCode = Payload.Checkout.Verify;
                    request.Pay360PaymentRequestToken.cardCv2 = request.Pay360PaymentRequestToken.cardCv2;
                    request.Pay360PaymentRequestToken.transactionAmount = total;
                    break;
                case Pay360PaymentType.ExistingNew:
                    request.Pay360PaymentRequestExistingNew.basket = basket;
                    request.Pay360PaymentRequestExistingNew.customerMsisdn = "";
                    request.Pay360PaymentRequestExistingNew.customerUniqueRef = Payload.FullName.Email;
                    request.Pay360PaymentRequestExistingNew.customerEmail = (Payload.CreditSim != null) ? Payload.CreditSim.Email : string.Empty;
                    request.Pay360PaymentRequestExistingNew.transactionCurrency = Payload.currency;
                    request.Pay360PaymentRequestExistingNew.ipAddress = IP;
                    request.Pay360PaymentRequestExistingNew.productCode = Payload.Checkout.Verify;
                    request.Pay360PaymentRequestExistingNew.cardCv2 = request.Pay360PaymentRequestExistingNew.cardCv2;
                    request.Pay360PaymentRequestExistingNew.transactionAmount = total;
                    break;

            }
            return request;
        }

        private Pay360PaymentRequest MapPayload(Pay360PaymentRequest request, Pay360PaymentType paymentType, JWTPayload Payload, string IP)
        {

            //If this is a purchase item, then override basket
            /*
            "Checkout": {
                "Reference": "447410876831",
                "Verify": "THM",
                "ProductType": "Bundle",
                "MailOrder": null,
                "Basket": null,
                "Total": 5,
                "DeliveryIsBilling": false
             }*/

            bool isLoggedIn = AccountService.IsAuthorized(Payload);
            List<basket> basket = new List<basket>();
       
            string msisdn = "";
            string prodRef = "";
            float total = 0F;
            ContentService cs = new ContentService();
            if (Payload.Purchase.Count > 0)
            {

                var pId = cs.GetProducts(Payload.Purchase.First()).ProductUuid;
                if (Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && Payload.isTHCCPin.Equals(false))
                {
                    basket.Add(new basket
                    {
                        amount = (float)Payload.Checkout.Total,
                        bundleRef = "",
                        productItemCode = "THRCC",
                        productRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });
                }
                else
                {
                    basket.Add(new basket
                    {
                        amount = (float)Payload.Checkout.Total,
                        bundleRef = pId,
                        productItemCode = Payload.Checkout.Verify,
                        productRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });
                 
                }


                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty;
                prodRef = Payload.Checkout.Verify;
                total = (float)Payload.Checkout.Total;
            }
            else if (Payload.TopUp.Count > 0)
            {
                if (Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && Payload.Checkout.ProductType.Equals(Models.Enums.ProductType.TopUp.ToString()) && Payload.isTHCCPin.Equals(false))
                {
                    basket.Add(new basket
                    {
                        amount = (float)Payload.Checkout.Total,
                        bundleRef = "",
                        productItemCode = "THRCC",
                        productRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });
                }
                else
                {
                    basket.Add(new basket
                    {
                        amount = (float)Payload.Checkout.Total,
                        bundleRef = "",
                        productItemCode = Payload.Checkout.Verify,
                        productRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });
                };
                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty;
                prodRef = Payload.Checkout.Verify;
                total = (float)Payload.Checkout.Total;
            }
            else if (Payload.Checkout.ProductType.Equals(ProductType.CreditSimOrder.ToString()) && Payload.Checkout.Basket.Count > 0)
            {
                var Products = cs.GetProducts(Payload.Checkout.Basket);
                foreach (var product in Products)
                {
                    total += product.ProductPrice;
                    basket.Add(new basket
                    {
                        amount = product.ProductPrice,
                        bundleRef = product.ProductUuid,
                        productItemCode = "THMCRSIM",
                        productRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });

                }
                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty;

            }else
            {
                var Products = cs.GetProducts(Payload.Checkout.Basket);
                foreach (var product in Products)
                {
                    total += product.ProductPrice;
                    basket.Add(new basket
                    {
                        amount = product.ProductPrice,
                        bundleRef = product.ProductUuid,
                        productItemCode = Payload.Checkout.Verify,
                        productRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });

                }
                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty;

            }

            switch (paymentType)
            {
                case Pay360PaymentType.New:
                    request.Pay360PaymentRequestNew.basket = basket;
                    if (isLoggedIn)
                    {
                        request.Pay360PaymentRequestNew.customerMsisdn = msisdn;
                        request.Pay360PaymentRequestNew.customerUniqueRef = Payload.FullName.Email;
                        request.Pay360PaymentRequestNew.customerEmail = Payload.FullName.Email;
                    }
                    else if (isLoggedIn == false && Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && Payload.isTHCCPin.Equals(false))
                    {
                        request.Pay360PaymentRequestNew.customerMsisdn = msisdn;
                        request.Pay360PaymentRequestNew.customerUniqueRef = Payload.FullName.Email;
                        request.Pay360PaymentRequestNew.customerEmail = Payload.FullName.Email;
                    }
                    else if (isLoggedIn == false && Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && Payload.Checkout.ProductType.Equals(Models.Enums.ProductType.Bundle.ToString()) && Payload.isTHCCPin.Equals(true))
                    {
                        request.Pay360PaymentRequestNew.customerMsisdn = msisdn;
                        request.Pay360PaymentRequestNew.customerUniqueRef = request.Pay360PaymentRequestNew.customerEmail;
                    }
                    else
                    {
                        request.Pay360PaymentRequestNew.customerMsisdn = msisdn;
                        request.Pay360PaymentRequestNew.customerUniqueRef = msisdn;
                        if (Payload.FullName != null && !string.IsNullOrEmpty(Payload.FullName.Email))
                        {
                            request.Pay360PaymentRequestNew.customerEmail = Payload.FullName.Email;
                        }
                        //else
                        //{
                        //    request.Pay360PaymentRequestNew.customerEmail = "";
                        //}
                    }
                    request.Pay360PaymentRequestNew.transactionCurrency = Payload.currency;
                    request.Pay360PaymentRequestNew.ipAddress = IP;
                    request.Pay360PaymentRequestNew.productCode = Payload.Checkout.Verify;
                    request.Pay360PaymentRequestNew.transactionAmount = total;
                    break;
                case Pay360PaymentType.Token:
                    request.Pay360PaymentRequestToken.basket = basket;
                    request.Pay360PaymentRequestToken.customerMsisdn = msisdn;
                    request.Pay360PaymentRequestToken.customerUniqueRef = Payload.FullName.Email;
                    request.Pay360PaymentRequestToken.customerEmail = Payload.FullName.Email;
                    request.Pay360PaymentRequestToken.transactionCurrency = Payload.currency;
                    request.Pay360PaymentRequestToken.ipAddress = IP;
                    request.Pay360PaymentRequestToken.productCode = Payload.Checkout.Verify;
                    request.Pay360PaymentRequestToken.cardCv2 = request.Pay360PaymentRequestToken.cardCv2;
                    request.Pay360PaymentRequestToken.transactionAmount = total;
                    break;
                case Pay360PaymentType.ExistingNew:
                    request.Pay360PaymentRequestExistingNew.basket = basket;
                    request.Pay360PaymentRequestExistingNew.customerMsisdn = msisdn;
                    request.Pay360PaymentRequestExistingNew.customerUniqueRef = Payload.FullName.Email;
                    request.Pay360PaymentRequestExistingNew.customerEmail = Payload.FullName.Email;
                    request.Pay360PaymentRequestExistingNew.transactionCurrency = Payload.currency;
                    request.Pay360PaymentRequestExistingNew.ipAddress = IP;
                    request.Pay360PaymentRequestExistingNew.productCode = Payload.Checkout.Verify;
                    request.Pay360PaymentRequestExistingNew.cardCv2 = request.Pay360PaymentRequestExistingNew.cardCv2;
                    request.Pay360PaymentRequestExistingNew.transactionAmount = total;
                    break;

            }


            return request;

        }

        public async Task<GenericPay360ApiResponse<Pay360PaymentResponse>> Pay360Payment(Pay360PaymentRequest request, Pay360PaymentType paymentType, JWTPayload Payload, string IP, bool Iscreditsim = false)
        {
            var fullRequest = new Pay360PaymentRequest();
            if (Iscreditsim)
            {
             fullRequest = MapPayloadCreditSim(request, paymentType, Payload, IP);
            }
            else
            {
                 fullRequest = MapPayload(request, paymentType, Payload, IP);

            }

            GenericPay360ApiResponse<Pay360PaymentResponse> ret = new GenericPay360ApiResponse<Pay360PaymentResponse>();

            string endpoint = "";
            string Json = "";
            string Result = "";

            switch (paymentType)
            {
                case Pay360PaymentType.New:
                    endpoint = Pay360APiEndpoint + "Pay360CashierApi/NewCustomerPayment";
                    Json = JsonConvert.SerializeObject(fullRequest.Pay360PaymentRequestNew);
                    Result = await Post(endpoint, Json);
                    break;
                case Pay360PaymentType.Default:
                    endpoint = Pay360APiEndpoint + "Pay360CashierApi/ExistingCustomerPaymentDefaultCard";
                    Json = JsonConvert.SerializeObject(request.Pay360PaymentRequestDefault);
                    Result = await Post(endpoint, Json);
                    break;
                case Pay360PaymentType.ExistingNew:
                    endpoint = Pay360APiEndpoint + "Pay360CashierApi/ExistingCustomerPaymentNewCard";
                    Json = JsonConvert.SerializeObject(fullRequest.Pay360PaymentRequestExistingNew);
                    Result = await Post(endpoint, Json);
                    break;
                case Pay360PaymentType.Token:
                    endpoint = Pay360APiEndpoint + "Pay360CashierApi/PaymentToken";
                    Json = JsonConvert.SerializeObject(fullRequest.Pay360PaymentRequestToken);
                    Result = await Post(endpoint, Json);
                    break;
            }



            if (Result == null)
            {
                return null;
            }


            //LoggerService.Debug(GetType(), Result);
            ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<Pay360PaymentResponse>>(Result);
            return ret;

        }



        public Pay360PaymentRequestToken CreatePay360PaymentRequestToken(StartPay360ViewModel model)
        {
           customFields customFields = createCustomefieldsObjec(model.FirstUseDate);

            Pay360PaymentRequestToken result = new Pay360PaymentRequestToken
            {
                cardCv2 = model.SecurityCode,
                customerMsisdn = model.Msisdn,
                isAuthorizationOnly = false,
                isDirectFullfilment = true,
                transactionAmount = model.Amount,
                transactionCurrency = model.Currency,
                do3DSecure = Pay360Api3DSecure,
                cardToken = model.CardId,
                customFields = customFields
            
            };

            return result;
        }

        public Pay360PaymentBase CreatePay360PaymentBaseRequest(StartPay360ViewModel model)
        {
            Pay360PaymentBase result = new Pay360PaymentBase
            {
                cardCv2 = model.CardCv2,
                customerMsisdn = model.Msisdn,
                isAuthorizationOnly = false,
                isDirectFullfilment = true,
                transactionAmount = model.Amount,
                transactionCurrency = model.Currency,
                do3DSecure = Pay360Api3DSecure
            };

            return result;
        }

        public Pay360PaymentRequestExistingNew CreatePay360PaymentRequestExistingNew(StartPay360ViewModel model)
        {
            Pay360PaymentRequestExistingNew result = new Pay360PaymentRequestExistingNew
            {

                cardCv2 = model.SecurityCode,
                cardExpiryDate = model.CardExpiryMonth + model.CardExpiryYear,
                cardPan = model.CardNumber,
                customerMsisdn = model.Msisdn,
                isAuthorizationOnly = false,
                isDirectFullfilment = true,
                isDefaultCard = false,
                do3DSecure = Pay360Api3DSecure,
                transactionAmount = model.Amount,
                transactionCurrency = model.Currency,
                customerName = model.NameOnCard,
                //billingAddress = new billingAddress
                //{
                //    line1 = "xx",
                //    line2 = "",
                //    line3 = "",
                //    line4 = "",
                //    city = "xx",
                //    region = "xx",
                //    postcode = "xx",
                //    countryCode = "GB"
                //}
            };

            return result;
        }

        public Pay360PaymentRequestNew CreatePay360PaymentRequestNew(StartPay360ViewModel model)
        {
            customFields customFields = new customFields();
            customFields=createCustomefieldsObjec(model.FirstUseDate);
            Pay360PaymentRequestNew result = new Pay360PaymentRequestNew
            {
                customerName = model.NameOnCard,
                cardCv2 = model.SecurityCode,
                cardExpiryDate = model.CardExpiryMonth + model.CardExpiryYear,
                cardPan = model.CardNumber,
                customerMsisdn = model.Msisdn,
                customerEmail = !string.IsNullOrEmpty(model.EmailAddress) ? model.EmailAddress : null,
                isAuthorizationOnly = false,
                isDirectFullfilment = true,
                isDefaultCard = true,
                do3DSecure = Pay360Api3DSecure,
                transactionAmount = model.Amount,
                transactionCurrency = model.Currency,
                billingAddress = new billingAddress
                {
                    line1 = model.AddressLine1,
                    line2 = model.AddressLine2,
                    line3 = model.AddressLine3,
                    line4 = model.AddressLine4,
                    city = model.City,
                    region = "",
                    postcode = model.PostalCode,
                    countryCode = model.CountryCode
                },
                customerBillingAddress = new customerBillingAddress
                {
                    line1 = model.AddressLine1,
                    line2 = model.AddressLine2,
                    line3 = model.AddressLine3,
                    line4 = model.AddressLine4,
                    city = model.City,
                    region = "",
                    postcode = model.PostalCode,
                    countryCode = model.CountryCode
                },
                customFields =customFields
            };

            return result;
        }
        /// <summary>
        /// Gets a customer's payment details via the MiPay payment service
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericPay360ApiResponse<Pay360CustomerModel>> GetCustomer(GetCustomerRequestModel model)
        {

            GenericPay360ApiResponse<Pay360CustomerModel> ret = new GenericPay360ApiResponse<Pay360CustomerModel>();

            string endpoint = "";


            endpoint = Pay360APiEndpoint + "Pay360CommonServices/GetCustomerByMsisdn";

            if (model.UniqueIDType != null)
            {
                var Json = JsonConvert.SerializeObject(model);

                var Result = await Post(endpoint, Json);

                if (Result == null)
                {
                    return null;
                }

                ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<Pay360CustomerModel>>(Result);
                return ret;
            }

            return ret;

        }




        /// <summary>
        /// Tries to retrieve the Customer Id for one-click checkout if the customer is known
        /// </summary>
        /// <param name="result">The response model</param>
        /// <param name="custId">Ref to the customer id to set</param>
        /// <returns>The result as TRUE or FALSE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryGetCustId(Pay360CustomerModel result, out string custId)
        {
            GenericPay360ApiResponse<Pay360CustomerModel> res1 = new GenericPay360ApiResponse<Pay360CustomerModel>();
            if (res1.errorCode != 0)
            {
                custId = null;
                //custId = "10093720049";
                return false;
            }
            if (res1.payload != null && res1.payload.pay360CustId != null && res1.payload.pay360CustId != "0")
            {
                custId = res1.payload.pay360CustId;
                return true;
            }

            custId = null;
            return false;
        }

        /// <summary>
        /// Tries to retrieve the Customer Id for one-click checkout if the customer is known
        /// </summary>
        /// <param name="result">The response model</param>
        /// <param name="custId">Ref to the customer id to set</param>
        /// <returns>The result as TRUE or FALSE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public async Task<GenericPay360ApiResponse<string>> SetAutoTopUp(Pay360SetAutoTopUpRequest model)
        {

            GenericPay360ApiResponse<string> ret = new GenericPay360ApiResponse<string>();

            string endpoint = Pay360APiEndpoint + "Pay360CommonServices/SetAutoTopup";

            if (model != null)
            {
                var Json = JsonConvert.SerializeObject(model);

                var Result = await Post(endpoint, Json);

                if (Result == null)
                {
                    return null;
                }

                ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<string>>(Result);
                return ret;
            }

            return ret;

        }


        public async Task<GenericPay360ApiResponse<paymentMethodResponse>> SetCustomerDefaultCard(Pay360SetCustomerDefaultCardRequest model)
        {

            GenericPay360ApiResponse<paymentMethodResponse> ret = new GenericPay360ApiResponse<paymentMethodResponse>();

            string endpoint = Pay360APiEndpoint + "Pay360CommonServices/SetCustomerDefaultCard";

            if (model != null)
            {
                var Json = JsonConvert.SerializeObject(model);

                var Result = await Post(endpoint, Json);

                if (Result == null)
                {
                    return null;
                }

                ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<paymentMethodResponse>>(Result);
                return ret;
            }

            return ret;

        }

        public async Task<GenericPay360ApiResponse<string>> RemoveCard(Pay360RemoveCardRequest model)
        {

            GenericPay360ApiResponse<string> ret = new GenericPay360ApiResponse<string>();

            string endpoint = Pay360APiEndpoint + "Pay360CommonServices/RemoveCard";

            if (model != null)
            {
                var Json = JsonConvert.SerializeObject(model);

                var Result = await Post(endpoint, Json);

                if (Result == null)
                {
                    return null;
                }

                ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<string>>(Result);
                return ret;
            }

            return ret;

        }


        public async Task<GenericPay360ApiResponse<Pay360GetAutoTopUpResponse>> GetAutoTopUp(Pay360GetAutoTopUpRequest model)
        {

            GenericPay360ApiResponse<Pay360GetAutoTopUpResponse> ret = new GenericPay360ApiResponse<Pay360GetAutoTopUpResponse>();

            string endpoint = Pay360APiEndpoint + "Pay360CommonServices/GetAutoTopup";

            if (model != null)
            {
                var Json = JsonConvert.SerializeObject(model);

                var Result = await Post(endpoint, Json);

                if (Result == null)
                {
                    return null;
                }

                ret = JsonConvert.DeserializeObject<GenericPay360ApiResponse<Pay360GetAutoTopUpResponse>>(Result);
                return ret;
            }

            return ret;

        }


        public bool IsValidPostCode(string postCode, out string cleanedPostCode)
        {
            cleanedPostCode = postCode.Trim();
            if (cleanedPostCode.Length == 0)
            {
                return false;
            }

            if (cleanedPostCode.Length > 8)
            {
                return false;
            }
            else
            {
                return true;
            }
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
        /// Creates Nowtel codes for a top up request
        /// </summary>
        /// <param name="productCode">The standardised Nowtel product code</param>
        /// <param name="denomination">The item's denomination</param>
        /// <param name="reference">The App user Msisdn</param>
        /// <returns>The Top up code</returns>
        /// <seealso cref="https://github.com/nowtel/talk-home/wiki/Product-codes"/>
        private string GenerateTopUpCode(string productCode, decimal denomination, string reference)
        {
            return string.Format("{0}/{1}/{2}/{3}/0", productCode, 1, denomination * 100, reference);
        }


        private async Task<string> Post(string address, string json)
        {
            var Content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(address), Timeout = TimeSpan.FromSeconds(60) })
                {
                    HttpResponseMessage response = await client.PostAsync("", Content);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    if (!response.IsSuccessStatusCode || response.Content == null)
                    {
                        throw new WebException();
                    }

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (System.Exception e)
            {

                return null;
            }
        }



        public customFields createCustomefieldsObjec(string FirstUseDate)
        {
            customFields customFields = new customFields();
            List<fieldState> fieldState = new List<fieldState>();
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["CustomFields"]))
            {
                if(FirstUseDate != null)
                {
                    fieldState.Add(new fieldState
                    {
                        name = "FirstUseDate",
                        value = FirstUseDate,
                        transient = false
                    });
                    fieldState.Add(new fieldState
                    {
                        name = "ProductItemCode",
                        value = "THM",
                        transient = false
                    });
                }
                else
                {
                    fieldState.Add(new fieldState
                    {
                        name = "ProductItemCode",
                        value = "THMCRSIM",
                        transient = false
                    });

                }
             
                fieldState.Add(new fieldState
                {
                    name = "ProductCode",
                    value = "THM",
                    transient = false
                });
               
              
            }
            customFields.fieldState = fieldState;
            return customFields;
        }
    }
}