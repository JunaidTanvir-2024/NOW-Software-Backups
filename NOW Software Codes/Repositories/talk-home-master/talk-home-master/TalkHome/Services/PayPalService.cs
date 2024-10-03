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
using TalkHome.Models.PayPal;
using TalkHome.Models.ViewModels.Pay360;
using TalkHome.Models.ViewModels.Payment;

namespace TalkHome.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly string PayPalApiEndpoint;
        private readonly string Pay360PayPalApiEndpoint;
        private readonly IAccountService AccountService;

        public PayPalService(IAccountService accountService)
        {
            AccountService = accountService;
            PayPalApiEndpoint = ConfigurationManager.AppSettings["PayPalApiEndpoint"];
            Pay360PayPalApiEndpoint = ConfigurationManager.AppSettings["Pay360PayPalApiEndpoint"];
        }

        private PayPalCreateSalePaymentRequest MapPayload(PayPalCreateSalePaymentRequest request, JWTPayload Payload)
        {
            List<ProductBasket> basket = new List<ProductBasket>();
            string msisdn = "";
            string prodRef = "";
            float total = 0F;
            ContentService cs = new ContentService();

            bool isLoggedIn = AccountService.IsAuthorized(Payload);

            if (Payload.Purchase.Count > 0)
            {

                var pId = cs.GetProducts(Payload.Purchase.First()).ProductUuid;
                if (Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && Payload.isTHCCPin.Equals(false))
                {
                    basket.Add(new ProductBasket
                    {
                        Amount = (float)Payload.Checkout.Total,
                        BundleRef = string.Empty,
                        ProductItemCode = "THRCC",
                        ProductRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });
                }
                else
                {
                    basket.Add(new ProductBasket
                    {
                        Amount = (float)Payload.Checkout.Total,
                        BundleRef = pId,
                        ProductItemCode = Payload.Checkout.Verify,
                        ProductRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });
                }

                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : Payload.Checkout.Reference;
                prodRef = Payload.Checkout.Verify;
                total = (float)Payload.Checkout.Total;
            }
            else if (Payload.TopUp.Count > 0)
            {
                if (Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && Payload.isTHCCPin.Equals(false) && Payload.Checkout.ProductType.Equals(Models.Enums.ProductType.TopUp.ToString()))
                {
                    basket.Add(new ProductBasket
                    {
                        Amount = (float)Payload.Checkout.Total,
                        BundleRef = string.Empty,
                        ProductItemCode = "THRCC",
                        ProductRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });
                }
                else
                {
                    basket.Add(new ProductBasket
                    {
                        Amount = (float)Payload.Checkout.Total,
                        BundleRef = "",
                        ProductItemCode = Payload.Checkout.Verify,
                        ProductRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });
                }
                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty;
                prodRef = Payload.Checkout.Verify;
                total = (float)Payload.Checkout.Total;
            }
            else
            {
                var Products = cs.GetProducts(Payload.Checkout.Basket);
                foreach (var product in Products)
                {
                    total += product.ProductPrice;
                    basket.Add(new ProductBasket
                    {
                        Amount = product.ProductPrice,
                        BundleRef = product.ProductUuid,
                        ProductItemCode = Payload.Checkout.Verify,
                        ProductRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });

                }
                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty;

            }
            request.Basket = basket;
            if (isLoggedIn)
            {
                request.CustomerMsisdn = msisdn;
                request.CustomerUniqueRef = Payload.FullName.Email;
                request.CustomerEmail = Payload.FullName.Email;
            }
            else if (isLoggedIn == false && Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && Payload.isTHCCPin.Equals(false))
            {
                request.CustomerMsisdn = msisdn;
                request.CustomerUniqueRef = Payload.FullName.Email;
                request.CustomerEmail = Payload.FullName.Email;
            }
            else if (isLoggedIn == false && Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()) && Payload.Checkout.ProductType.Equals(Models.Enums.ProductType.Bundle.ToString()) && Payload.isTHCCPin.Equals(true))
            {
                request.CustomerMsisdn = msisdn;
                request.CustomerUniqueRef = request.CustomerEmail;
            }
            else
            {
                request.CustomerMsisdn = msisdn;
                request.CustomerUniqueRef = msisdn;
                if (Payload.FullName != null && !string.IsNullOrEmpty(Payload.FullName.Email))
                {
                    request.CustomerEmail = Payload.FullName.Email;
                }
                //else
                //{
                //    request.CustomerEmail = "";
                //}
            }
            request.ProductCode = Payload.Checkout.Verify;
            request.Transaction = new Transactions { Amount = new Amounts() };

            request.Transaction.Amount.Currency = Payload.currency;
            request.Transaction.Amount.Total = total;
            request.Transaction.Description = "PayPal Transaction for Customer Unique Reference: " + request.CustomerUniqueRef;


            return request;

        }

        private PayPalCreateSalePaymentRequest MapPayloadCreditSIm(PayPalCreateSalePaymentRequest request, JWTPayload Payload)
        {
            List<ProductBasket> basket = new List<ProductBasket>();
            string msisdn = "";
            string prodRef = "";
            float total = 0F;
            ContentService cs = new ContentService();

            bool isLoggedIn = AccountService.IsAuthorized(Payload);

            if (Payload.Checkout.ProductType.Equals(ProductType.CreditSimOrder.ToString()) && Payload.Checkout.Basket.Count > 0)
            {
                var Products = cs.GetProducts(Payload.Checkout.Basket);
                foreach (var product in Products)
                {
                    total += product.ProductPrice;
                    basket.Add(new ProductBasket
                    {
                        Amount = product.ProductPrice,
                        BundleRef = product.ProductUuid,
                        ProductItemCode = "THMCRSIM",
                        ProductRef = (Payload.CreditSim != null && Payload.CreditSim.userId != 0) ? Payload.CreditSim.userId.ToString() : string.Empty
                    });
                }
            }
            request.Basket = basket;
            if (isLoggedIn)
            {
                request.CustomerMsisdn = "";
                request.CustomerUniqueRef = Payload.FullName.Email;
                request.CustomerEmail = (Payload.CreditSim != null) ? Payload.CreditSim.Email : string.Empty;
            }
            else
            {
                request.CustomerMsisdn = "";
                request.CustomerUniqueRef = (Payload.CreditSim != null && Payload.CreditSim.Email != null) ? Payload.CreditSim.Email : string.Empty;
                if (Payload.CreditSim != null && Payload.CreditSim.Email != null)
                {
                    request.CustomerEmail = (Payload.CreditSim != null) ? Payload.CreditSim.Email : string.Empty;
                }
            }
            request.ProductCode = Payload.Checkout.Verify;
            request.Transaction = new Transactions { Amount = new Amounts() };

            request.Transaction.Amount.Currency = Payload.currency;
            request.Transaction.Amount.Total = total;
            request.Transaction.Description = "PayPal Transaction for Customer Unique Reference: " + request.CustomerUniqueRef;


            return request;

        }


        private Pay360PayPalCreateSalePaymentRequest MapPayloadPay360PalPalPayment(StartPay360PaymentViewModel model, JWTPayload Payload, string IpAddress)
        {
            Pay360PayPalCreateSalePaymentRequest request = new Pay360PayPalCreateSalePaymentRequest();

            customFields customFields = new customFields();
            customFields = createCustomefieldsObjec(model.FirstUseDate);
            request.customFields = customFields;

            request.ipAddress = IpAddress;

            List<ProductBasket> basket = new List<ProductBasket>();
            string msisdn = "";
            string prodRef = "";
            float total = 0F;
            ContentService cs = new ContentService();

            bool isLoggedIn = AccountService.IsAuthorized(Payload);

            if (Payload.Purchase.Count > 0)
            {

                var pId = cs.GetProducts(Payload.Purchase.First()).ProductUuid;

                basket.Add(new ProductBasket
                {
                    Amount = (float)Payload.Checkout.Total,
                    BundleRef = pId,
                    ProductItemCode = Payload.Checkout.Verify,
                    ProductRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                });


                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : Payload.Checkout.Reference;
                prodRef = Payload.Checkout.Verify;
                total = (float)Payload.Checkout.Total;
            }
            else if (Payload.TopUp.Count > 0)
            {

                basket.Add(new ProductBasket
                {
                    Amount = (float)Payload.Checkout.Total,
                    BundleRef = "",
                    ProductItemCode = Payload.Checkout.Verify,
                    ProductRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                });

                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty;
                prodRef = Payload.Checkout.Verify;
                total = (float)Payload.Checkout.Total;
            }
            else
            {
                var Products = cs.GetProducts(Payload.Checkout.Basket);
                foreach (var product in Products)
                {
                    total += product.ProductPrice;
                    basket.Add(new ProductBasket
                    {
                        Amount = product.ProductPrice,
                        BundleRef = product.ProductUuid,
                        ProductItemCode = Payload.Checkout.Verify,
                        ProductRef = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty
                    });

                }
                msisdn = Payload.Checkout.Reference != null ? Payload.Checkout.Reference.Trim() : string.Empty;

            }
            request.Basket = basket;



            if (isLoggedIn)
            {
                request.CustomerMsisdn = Payload.Checkout.Reference;
                request.CustomerUniqueRef = Payload.FullName.Email;
                request.CustomerEmail = Payload.FullName.Email;
            }
            else
            {
                request.CustomerMsisdn = Payload.Checkout.Reference;
                request.CustomerUniqueRef = Payload.Checkout.Reference;

                request.CustomerEmail = model.EmailAddress;

            }
            request.ProductCode = Payload.Checkout.Verify;


            request.transactionCurrency = Payload.currency;
            request.transactionAmount = total;

            //request.customerBillingAddress = new customerBillingAddress
            //{
            //    line1 = model.AddressLine1,
            //    line2 = model.AddressLine2,
            //    line3 = model.AddressLine3,
            //    line4 = model.AddressLine4,
            //    city = model.City,
            //    region = "",
            //    postcode = model.PostalCode,
            //    countryCode = model.CountryCode
            //};
            //request.shippingAddress = new billingAddress
            //{
            //    line1 = model.AddressLine1,
            //    line2 = model.AddressLine2,
            //    line3 = model.AddressLine3,
            //    line4 = model.AddressLine4,
            //    city = model.City,
            //    region = "",
            //    postcode = model.PostalCode,
            //    countryCode = model.CountryCode
            //};
            //request.billingAddress = new billingAddress
            //{
            //    line1 = model.AddressLine1,
            //    line2 = model.AddressLine2,
            //    line3 = model.AddressLine3,
            //    line4 = model.AddressLine4,
            //    city = model.City,
            //    region = "",
            //    postcode = model.PostalCode,
            //    countryCode = model.CountryCode
            //};

            paymentMethod paymentMethod = new paymentMethod();
            //List<paypal> paypal = new List<paypal>();
            //paypal.Add(new paypal {
            //    returnUrl = "http://merchant-returnUrl.com",
            //    cancelUrl = "http://merchant-cancelurl.com"
            //});
            paymentMethod.paypal = new paypal
            {
                returnUrl = model.returnUrl,
                cancelUrl = model.cancelUrl
            };
            request.paymentMethod = paymentMethod;
            request.isDirectFullfilment = true;
            request.CustomerName = model.customerName;
            return request;

        }


        private Pay360PayPalCreateSalePaymentRequest MapPayloadCreditSImPay360PalPalPayment(StartPay360PaymentViewModel model, JWTPayload Payload, string IpAddress)
        {
            Pay360PayPalCreateSalePaymentRequest request = new Pay360PayPalCreateSalePaymentRequest();

            customFields customFields = new customFields();
            customFields = createCustomefieldsObjecCreditSim(model.FirstUseDate);
            request.customFields = customFields;

            request.ipAddress = IpAddress;

            List<ProductBasket> basket = new List<ProductBasket>();
            string msisdn = "";
            string prodRef = "";
            float total = 0F;
            ContentService cs = new ContentService();

            bool isLoggedIn = AccountService.IsAuthorized(Payload);

            if (Payload.Checkout.ProductType.Equals(ProductType.CreditSimOrder.ToString()) && Payload.Checkout.Basket.Count > 0)
            {
                var Products = cs.GetProducts(Payload.Checkout.Basket);
                foreach (var product in Products)
                {
                    total += product.ProductPrice;
                    basket.Add(new ProductBasket
                    {
                        Amount = product.ProductPrice,
                        BundleRef = product.ProductUuid,
                        ProductItemCode = "THMCRSIM",
                        ProductRef = (Payload.CreditSim != null && Payload.CreditSim.userId != 0) ? Payload.CreditSim.userId.ToString() : string.Empty
                    });
                }
            }

            request.Basket = basket;

            if (isLoggedIn)
            {
                request.CustomerMsisdn = "";
                request.CustomerUniqueRef = Payload.FullName.Email;
                request.CustomerEmail = (Payload.CreditSim != null) ? Payload.CreditSim.Email : string.Empty;
            }
            else
            {
                request.CustomerMsisdn = "";
                request.CustomerUniqueRef = (Payload.CreditSim != null && Payload.CreditSim.Email != null) ? Payload.CreditSim.Email : string.Empty;
                if (Payload.CreditSim != null && Payload.CreditSim.Email != null)
                {
                    request.CustomerEmail = (Payload.CreditSim != null) ? Payload.CreditSim.Email : string.Empty;
                }
            }

            request.ProductCode = Payload.Checkout.Verify;
            request.transactionCurrency = Payload.currency;
            request.transactionAmount = total;


            paymentMethod paymentMethod = new paymentMethod();

            paymentMethod.paypal = new paypal
            {
                returnUrl = model.returnUrl,
                cancelUrl = model.cancelUrl
            };
            request.paymentMethod = paymentMethod;
            request.isDirectFullfilment = true;
            request.CustomerName = model.customerName;
            return request;

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
                Domain = Domain.Replace("http", "https");
                Domain = Domain + path;
            }

            return Domain;

        }
        private string GetDomain()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

        public async Task<GenericPayPalApiResponse<PayPalCreateSalePaymentResponse>> PayPalCreateSalePayment(PayPalCreateSalePaymentRequest request, JWTPayload Payload, bool IScreditsim = false)
        {

            var fullRequest = new PayPalCreateSalePaymentRequest();
            if (IScreditsim)
            {
                fullRequest = MapPayloadCreditSIm(request, Payload);
            }
            else
            {
                fullRequest = MapPayload(request, Payload);
            }

            GenericPayPalApiResponse<PayPalCreateSalePaymentResponse> ret = new GenericPayPalApiResponse<PayPalCreateSalePaymentResponse>();

            string endpoint = "";
            string Json = "";
            string Result = "";
            endpoint = PayPalApiEndpoint + "Paypal/CreateSalePayment";
            Json = JsonConvert.SerializeObject(fullRequest);
            Result = await Post(endpoint, Json);
            if (Result == null)
            {
                return null;
            }
            ret = JsonConvert.DeserializeObject<GenericPayPalApiResponse<PayPalCreateSalePaymentResponse>>(Result);
            return ret;

        }


        public async Task<GenericPayPalApiResponse<PayPalExecuteSalePaymentResponse>> PayPalExecuteSalePayment(PayPalExecuteSalePaymentRequest request)
        {
            GenericPayPalApiResponse<PayPalExecuteSalePaymentResponse> ret = new GenericPayPalApiResponse<PayPalExecuteSalePaymentResponse>();

            string endpoint = "";
            string Json = "";
            string Result = "";
            endpoint = PayPalApiEndpoint + "Paypal/ExecuteSalePayment";
            Json = JsonConvert.SerializeObject(request);
            Result = await Post(endpoint, Json);
            if (Result == null)
            {
                return null;
            }
            ret = JsonConvert.DeserializeObject<GenericPayPalApiResponse<PayPalExecuteSalePaymentResponse>>(Result);
            return ret;

        }




        public async Task<GenericPayPalApiResponse<Pay360PayPalCreateSalePaymentResponse>> Pay360PayPalCreateSalePayment(StartPay360PaymentViewModel request, JWTPayload Payload, string ipAddress, bool Iscreditsim)
        {
            var fullRequest = new Pay360PayPalCreateSalePaymentRequest();
            if (Iscreditsim)
            {
                fullRequest = MapPayloadCreditSImPay360PalPalPayment(request, Payload, ipAddress);

            }
            else
            {

                fullRequest = MapPayloadPay360PalPalPayment(request, Payload, ipAddress);

            }


            GenericPayPalApiResponse<Pay360PayPalCreateSalePaymentResponse> ret = new GenericPayPalApiResponse<Pay360PayPalCreateSalePaymentResponse>();

            string endpoint = "";
            string Json = "";
            string Result = "";
            endpoint = Pay360PayPalApiEndpoint + "Paypal/Payment";
            Json = JsonConvert.SerializeObject(fullRequest);
            Result = await Post(endpoint, Json);
            if (Result == null)
            {
                return null;
            }
            ret = JsonConvert.DeserializeObject<GenericPayPalApiResponse<Pay360PayPalCreateSalePaymentResponse>>(Result);
            return ret;

        }

        public customFields createCustomefieldsObjec(string FirstUseDate)
        {
            customFields customFields = new customFields();
            List<fieldState> fieldState = new List<fieldState>();
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["CustomFields"]))
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


        public customFields createCustomefieldsObjecCreditSim(string FirstUseDate)
        {
            customFields customFields = new customFields();
            List<fieldState> fieldState = new List<fieldState>();
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["CustomFields"]))
            {

               
                fieldState.Add(new fieldState
                {
                    name = "ProductItemCode",
                    value = "THMCRSIM",
                    transient = false
                });
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

        public async Task<GenericPayPalApiResponse<Pay360PayPalCreateResumePaymentResponse>> Pay360ResumePayment(Pay360PayPalResumePaymentRequest model)
        {
            string endpoint = "";
            string Json = "";
            string Result = "";
            endpoint = Pay360PayPalApiEndpoint + "Paypal/Resume";
            Json = JsonConvert.SerializeObject(model);
            Result = await Post(endpoint, Json);
            if (Result == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<GenericPayPalApiResponse<Pay360PayPalCreateResumePaymentResponse>>(Result);
        }
    }
}