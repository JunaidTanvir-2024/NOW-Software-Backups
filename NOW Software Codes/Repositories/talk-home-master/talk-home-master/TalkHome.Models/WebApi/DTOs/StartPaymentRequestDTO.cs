using System.Collections.Generic;

namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Request model to begin a payment
    /// </summary>
    public class StartPaymentRequestDTO
    {
        public string Amount { get; set; } // Total amount in pence/cents.

        public string CurrencyCode { get; set; }

        public string Salutation { get; set; }

        public string SuccessURL { get; set; }

        public string FailureURL { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string CountyOrProvince { get; set; }

        public string CountryCode { get; set; }

        public string PostalCode { get; set; }

        public string HomeTelNumber { get; set; }

        public string EmailAddress { get; set; }

        public string Locale { get; set; }

        public string Card { get; set; }

        public string Msisdn { get; set; }

        public string ChannelType { get; set; }

        public string PaymentType { get; set; }

        public string PaymentMethod { get; set; }

        public List<string> Basket { get; set; }
    }
}
