using System;
using System.Collections.Generic;
using System.Configuration;
using TalkHome.Models.ViewModels.DTOs;
using TalkHome.Models.WebApi.Payment;

namespace TalkHome.Models
{
    /// <summary>
    /// Defines the model for the JWT token cookie payload
    /// </summary>
    public class JWTPayload
    {
        public FullNameModel FullName { get; set; } = new FullNameModel();

        public string ApiToken { get; set; }

        public DateTime ApiTokenExpiry { get; set; }

        public string TwoLetterISORegionName { get; set; }

        public string currency { get; set; }

        public string currencySymbol { get; set; }

        public List<AccountCodes> ProductCodes { get; set; } = new List<AccountCodes>();

        public HashSet<int> Purchase { get; set; } = new HashSet<int>();

        public HashSet<int> TopUp { get; set; } = new HashSet<int>();

        public HashSet<int> Basket { get; set; } = new HashSet<int>();

        public bool OpenSignUp { get; set; }

        public string OpenRegistration { get; set; }

        public string MailOrder { get; set; }

        public CheckoutPageDTO Checkout { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public bool VerifiedReset { get; set; }

        public PaymentRetrieveRequest Payment { get; set; }

        public RetrieveOneClickRequest OneClick { get; set; }

        public string CustId { get; set; }

        public string ResetToken { get; set; }

        public bool isTHCCPin { get; set; }

        public string CheckoutProduct { get; set; }

        public AirtimeTransfer AirTimeTransfer { get; set; }

        public string HomeRoot { get; set; }

        public bool AutoTopUpEnabled { get; set; } = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoTopUpEnabledByDefault"]);
        public string UniqueTrackingId { get; set; }

        public CreditSimPayload CreditSim { get; set; }

        public string OrderEmail { get; set; }
        public string UserId { get; set; }

        public string EmailVerificationToken { get; set; }
    }
}
