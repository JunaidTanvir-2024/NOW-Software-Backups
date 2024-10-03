using System.Collections.Generic;
using TalkHome.Models.Pay360;
using TalkHome.Models.WebApi.CallingCards;
using TalkHome.Models.WebApi.Payment;
using TalkHome.Models.WebApi.Rates;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Models.ViewModels.Umbraco
{
    /// <summary>
    /// Umbraco Custom page content view model
    /// </summary>
    public class CustomPageViewModel<T> : RenderModel
    {
        // Added properties
        public string Slug { get; set; }
        
        public JWTPayload Payload { get; set; }

        public T Page { get; set; }

        public IEnumerable<IPublishedContent> Products { get; set; }

        public IDictionary<string, IEnumerable<IPublishedContent>> TopUps { get; set; }

        public IEnumerable<IPublishedContent> VerifiedTopUps { get; set; }

        public IList<Rate> Rates { get; set; }

        public IList<RoamingRate> RoamingRates { get; set; }

        public IList<MinutesRecord> Minutes { get; set; }

        public IList<TopMinute> TopMinutes { get; set; }

        public IList<UKNationalRate> UKRates { get; set; }

        public CustomerDetailsViewModel CustomerDetailsViewModel { get; set; }

        public AddressDetailsViewModel AddressDetailsViewModel { get; set; }

        public MiPayCustomerModel MiPayCustomer { get; set; }

        public Pay360CustomerModel Pay360Customer { get; set; }

        public List<I18nCountry> CountryList { get; set; }

        public string PaymentProvider { get; set; }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload) : base(content)
        {
            Payload = payload;

            Page = (T)content;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, List<I18nCountry> countryList) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            CountryList = countryList;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IDictionary<string, IEnumerable<IPublishedContent>> topUps) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            TopUps = topUps;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, AddressDetailsViewModel addressDetailsViewModel) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            AddressDetailsViewModel = addressDetailsViewModel;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IEnumerable<IPublishedContent> products, AddressDetailsViewModel addressDetailsViewModel) : base(content)
        {
            Payload = payload;

            Products = products;

            Page = (T)content;

            AddressDetailsViewModel = addressDetailsViewModel;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IEnumerable<IPublishedContent> verifiedTopUps, CustomerDetailsViewModel addressViewModel, MiPayCustomerModel customerDetailsViewModel, Pay360CustomerModel pay360CustomerModel,string provider) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            VerifiedTopUps = verifiedTopUps;

            CustomerDetailsViewModel = addressViewModel;

            MiPayCustomer = customerDetailsViewModel;

            Pay360Customer = pay360CustomerModel;

            PaymentProvider = provider;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IDictionary<string, IEnumerable<IPublishedContent>> topUps, CustomerDetailsViewModel addressViewModel, MiPayCustomerModel customerDetailsViewModel, Pay360CustomerModel pay360CustomerModel,string provider) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            TopUps = topUps;

            CustomerDetailsViewModel = addressViewModel;

            MiPayCustomer = customerDetailsViewModel;

            Pay360Customer = pay360CustomerModel;

            PaymentProvider = provider;
        }

        
       
        

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IList<RoamingRate> roamingRates) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            RoamingRates = roamingRates;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IEnumerable<IPublishedContent> products) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            Products = products;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IEnumerable<IPublishedContent> products, List<I18nCountry> countryList,IList<Rate> rateList) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            Products = products;

            CountryList = countryList;

            Rates = rateList;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, List<Product> products, IList<TopMinute> topMinutes) : base(content)
        {
            Page = (T)content;

            Payload = payload;

            Products = products;

            TopMinutes = topMinutes;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IList<MinutesRecord> minutes) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            Minutes = minutes;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IList<Rate> rates) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            Rates = rates;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, List<Product> products, IList<Rate> rates) : base(content)
        {
            Payload = payload;

            Products = products;

            Page = (T)content;

            Rates = rates;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, List<Product> products, IList<MinutesRecord> minutes) : base(content)
        {
            Payload = payload;

            Products = products;

            Page = (T)content;

            Minutes = minutes;
        }

        public CustomPageViewModel(IPublishedContent content, JWTPayload payload, IList<UKNationalRate> ukRates) : base(content)
        {
            Payload = payload;

            Page = (T)content;

            UKRates = ukRates;
        }
    }




    public class TalkHomeMobileInternationalPlanCustom
    {

        public JWTPayload Payload { get; set; }

        public IEnumerable<IPublishedContent> Products { get; set; }

        public IList<Rate> Rates { get; set; }

        public List<I18nCountry> CountryList { get; set; }

        public TalkHomeMobileInternationalPlanCustom(JWTPayload payload, IEnumerable<IPublishedContent> products, List<I18nCountry> countryList, IList<Rate> rateList) 
        {
            Payload = payload;

            Products = products;

            CountryList = countryList;

            Rates = rateList;
        }
    }
    }
