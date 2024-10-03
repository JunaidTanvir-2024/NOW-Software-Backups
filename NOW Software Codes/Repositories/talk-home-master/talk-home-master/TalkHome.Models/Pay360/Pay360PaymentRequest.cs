using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
namespace TalkHome.Models.Pay360
{

    public enum Pay360PaymentType
    {
        New,
        Default,
        Token,
        ExistingNew
    }

    
    public class financialServices
    {
        public string dateOfBirth { get; set; }
        public string surname { get; set; }
        public string accountNumber { get; set; }
        public string postCode { get; set; }
    }

    public class billingAddress
    {
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3 { get; set; }
        public string line4 { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string postcode { get; set; }
        public string countryCode { get; set; }
    }

    public class basket
    {
        public string productItemCode { get; set; }
        public float amount { get; set; }
        public string productRef { get; set; }
        public string bundleRef {get;set;}
    }


    public class Pay360PaymentBase
    {
        public bool isDirectFullfilment;

        public string customerName { get; set; }
        public string customerUniqueRef { get; set; }
        public string customerEmail { get; set; }
        public string customerMsisdn { get; set; }
        public string transactionCurrency { get; set; }
        public float transactionAmount { get; set; }
        public string cardCv2 { get; set; }
        public bool isAuthorizationOnly { get; set; }
        public bool do3DSecure { set; get; }
        public bool recurring { get; set; }
        public string ipAddress { get; set; }
        public string productCode { get; set; } 
        public List<basket> basket { get; set; }
        public billingAddress billingAddress { get; set; }
        public customerBillingAddress customerBillingAddress { get; set; }
        public customFields customFields { get; set; }
    }

    public class Pay360PaymentRequestNew : Pay360PaymentRequestExistingNew
    {
        public bool isDirectFullfilment;
    }

    public class Pay360PaymentRequestToken: Pay360PaymentBase
    {
        public string cardToken { get; set; }
    }

    public class Pay360PaymentRequestExistingNew: Pay360PaymentBase
    {
        public bool isDirectFullfilment;

        public string cardPan { get; set; }
        public string cardExpiryDate { get; set; }
        public bool isDefaultCard { get; set; }
        public financialServices financialServices { get; set; }

    }

    public class Pay360PaymentRequest
    {
        public Pay360PaymentRequestNew Pay360PaymentRequestNew { get;set;}
        public Pay360PaymentBase Pay360PaymentRequestDefault { get; set; }
        public Pay360PaymentRequestExistingNew Pay360PaymentRequestExistingNew { get; set; }
        public Pay360PaymentRequestToken Pay360PaymentRequestToken { get; set; }
    }



    public class customerBillingAddress
    {

        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3 { get; set; }
        public string line4 { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string postcode { get; set; }
        public string countryCode { get; set; }
    }

    public class customFields
    {
      public  List<fieldState> fieldState = new List<fieldState>();
    }

    public class fieldState
    {
        public string name { get; set; }
        public string value { get; set; }
        public bool transient { get; set; }
    }

}

