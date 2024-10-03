namespace TalkHome.Models.Enums
{
    /// <summary>
    /// Defines the messages used throughout the application and for logs/alerts
    /// </summary>
    public enum Messages
    {
        UnknownError = 608,

        RejectionWithError = 601,

        NotFound = 602,

        EmptyOrNullPayload = 603,

        MiPayGatewayFailure = 604,

        NoTransactionFound = 610,

        InvalidMsisdnOnMiPay = 351,

        TotalsDontMatch = 120,

        InvalidInput = 998,

        NoRecordsFound = 888,

        InternalServerError = 999,

        NoProductFoundForUser = 996,

        UserNotFound = 997,

        ResetPasswordFailed = 994,

        InvalidHistoryParams = 995,

        InvalidApiProductCode = 993,

        InvalidRatesType = 992,

        NoTHADataHistory = 990,

        OrderSimFailed = 989,

        AccountAlreadyExists = 988,

        AccountAlreadyConfirmed = 987,

        AddressLine1 = 700,

        InvalidState = 701,

        City = 702,

        CountryCode = 703,

        PostCode = 704,

        Email = 705,

        PaymentMethod = 706,

        Salutation = 707,

        FirstName = 708,

        LastName = 709,

        InvalidCountryCode = 710,

        InvalidPaymentMethod = 711,

        InvalidProductCode = 712,

        ProductCode = 713,

        Number = 714,

        NotVerifiedNumber = 715,

        MissingTopUp = 717,

        MissingPassword = 719,

        MissingConfirmPassword = 720,

        PasswordTooShort = 722,

        MailOrderProductMissing = 723,

        MailOrderError = 724,

        AnErrorOccurred = 19,

        MissingActivationCode = 726,

        InvalidNumber = 727,

        UnknownAccount = 976,

        AlreadyActive = 729,

        Forbidden = 730,

        UnexistingProduct = 731,

        InvalidProductType = 732,

        MissingCheckoutObject = 733,

        InvalidCheckoutObject = 734,

        ProductNotRegisteredForPurchase = 735,

        AppUserNotVerifiedAtCheckout = 736,

        GetMiPayCustomerFailed = 737,

        AppUserNotFound = 738,

        CorruptedAppTopUpUrl = 739,

        StartPaymentFailed = 740,

        RetrievePaymentFailed = 741,

        WebApiLoginException = 742,

        MailOrderException = 743,

        SignatureVerificationException = 744,

        JWTDecodeException = 745,

        NumberVerificationFailed = 746,

        CorruptedAddBundleUrl = 747,

        AppBundleNotFound = 748,

        EmptyAppBasket = 749,

        AuthenticationFailed = 983,

        AccountNotFound = 751,

        AccountNotConfirmed = 984,

        UnknownLogInError = 753,

        PromoSignUpException = 754,

        PromoSignUpFailure = 755,

        PromoSignUpSuccess = 756,

        NotFoundForResetPassword = 982,

        PasswordResetLinkSent = 758,

        InvalidResetToken = 981,

        ValidResetToken = 760,

        NewPasswordFailed = 980,

        NewPasswordSuccess = 762,

        VerifySignUpFailed = 986,

        VerifiedSignUp = 764,

        InvalidCheckout = 765,

        SignUpFailed = 767,

        UnknownVerifySignUpFailed = 768,

        TooManyLogInAttempts = 985,

        InvalidMsisdnOrPUK = 979,

        InvalidMsisdnOrPIN = 978,

        ProductAlreadyAdded = 977,

        AddProductFailed = 975,

        AddProductSuccess = 769,

        WebApiHttpException = 770,

        AddToBasketSuccess = 771,

        AddToBasketFailure = 772,

        RemoveFromBasketSuccess = 773,

        RemoveFromBasketFailure = 774,

        ClearBasketSuccess = 775,

        InvalidId = 776,

        BasketProductMismatch = 777,

        RedeemPointsFailed = 955,

        InsufficientPoints = 954,

        ProductNotRegistered = 778,

        NotADecimalNumber = 779,

        NumberNotInRange = 780,

        MissingThreshold = 781,

        AutoTopUpSuccess = 782,

        AutoRenewalSuccess = 783,

        UserHasSim = 784,

        AddToBasketInsufficentCredit = 785,

        TransferInsufficientCredit = 786,

        InvalidPostCode = 994,

        MoreThanOnePin = 995,
        ProductNotRegisteredForPort = 996,
        MailOrderSuccessPortInfail = 997,

        MaxBundleLimitReached = 300,
        Pay360CustomerNotFound = 1000,
        AlreadyHasProduct = 1001,
        CreditSimFullfillmentError=1002,
        CreditSimPaymentError=1003

    }
}
