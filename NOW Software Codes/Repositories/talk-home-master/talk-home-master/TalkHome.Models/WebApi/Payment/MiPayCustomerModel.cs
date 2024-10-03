using System.Collections.Generic;

namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Describes customer data as returned by MiPay.
    /// </summary>
    public class MiPayCustomerModel
    {
        public string responseCode { get; set; }

        public Customer customer { get; set; }

        public IEnumerable<UniqueId> uniqueIDs { get; set; }

        public bool accountEnabled { get; set; }

        public bool topupEnabled { get; set; }

        public IEnumerable<PaymentMethod> paymentMethods { get; set; }

        public IEnumerable<Friend> friends { get; set; }

        public IEnumerable<RecurringPayment> recurringPayments { get; set; }

        public bool deleteAccount { get; set; }

        public string salutation { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string emailAddress { get; set; }

        public string homeTelephoneNumber { get; set; }

        public string preferedCurrency { get; set; }

        public string languagePreference { get; set; }

        public BillingAddress billingAddress { get; set; }

        public string customerPIN { get; set; }

        public string dateCreatedAccount { get; set; }

        public int custStatusCode { get; set; }
    }
}
