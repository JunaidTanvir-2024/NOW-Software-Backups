using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace THPromotionPortal.Models.enums
{
    public enum ApiStatusCodes
    {
        InternalServerError = 1,
        EmailAlreadyRegistered = 2,
        EmailNotExist = 3,
        InvalidNumber = 4,
        PhoneNumberAlreadyRegistered = 5,
        TokenExpiredorInvalid = 6,
        MobilePinExpiredorInvalid = 7,
        UserNotFound = 8,
        EmailAlreadyVerified = 9,
        UnableToFetchSocialAppUser = 10,
        EmailNotVerified = 11,
        InvalidEmailPassword = 12,
        DTOneServiceError = 13,
        InvalidProduct = 14,
        InsufficientBalanceInAccount = 15,
        AccountBalanceTransactionFailed = 16,
        PaymentApiNotResponding = 17,
        PaymentFailure = 18,
        CreditHistoryNotFound = 19,
        TransferCreditHistoryNotFound = 20,
        TopUpAmountsNotFound = 21,
        FavouriteTransactionNumbersNotFound = 22,
        TransferPendingTwoFactorAuthentication = 23,
        EmailVerificationPending = 24,
        NumberAlreadyExistInFavourites = 25,
        AutoTransferNumbersNotFound = 26,
        ApplePayTopUpFailed = 27,
        TopupNumberLimitExceed = 28,
        TopupAmountLimitExceed = 29,
        DenominationsNotAvailable = 30,
        DenominationBlocked = 31,
        OperatorBlocked = 32,
        CardCustomerNotExist = 33,
        Pay360ServiceError = 34,
        RecordNotFound = 35,
        UserNotCreated = 36,
        UserNotFoundInData = 37,
        FraudCustomer = 38,
        CountryWithCurrencyExist = 39,
        TopUpCapsExist = 40,
        TransferCapsExist = 41,
        BlockedCurrencyNotExists = 42,
        DebitAmountIsExceed = 43,
        ModifyAmountApiException = 44,
        PromotionNotFound = 45,
        Success = 46,
        DBError = 47,
    }
    public enum Roles
    {
        Guest = 1,
        User = 2,
        Admin = 3,
        Owner = 4,
        CsrLead = 5,
        Csr = 6
    }
    public enum AuthType
    {
        BasicAuth = 1,
        TokenBased = 2
    }
    public enum PromotionType
    {
        Discount = 1,
        ExtraBalance = 2
    }

    public enum PromotionValueType
    {
        Percentage = 1,
        Value = 2
    }
    public enum ThresholdType
    {
        Equal = 1,
        LessThen = 2,
        GreaterThan = 3,
        LessThanAndEqual = 4,
        GreaterAndThanEqual = 5
    }

    public enum AudienceStatusType
    {
        NewUser = 1,
        ExistingUser = 2,
        All = 3
    }

    public enum AudienceType
    {
        Origin = 1,
        Destination = 2,
    }
    public enum Product
    {
        THM = 1,
        NOWPAYG = 2,
        THA = 3,
        THCC = 4,
        TRH = 5
    }

    public enum PaymentType
    {
        Card = 1,
        Paypal = 2,
        AccountBalance = 3
    }
    public enum PurchaseType
    {
        Bundle = 1,
        AutoTopUp = 2,
        TopUp = 3,
        InternationalTopUp = 4,
        CallingCard = 5,
        RechargableCallingcard = 6
    }

    public enum Medium
    {
        Web = 1,
        Android = 2,
        IOS = 3
    }
}
