namespace Effortless.Core.Domain.Definitions;

public sealed class AppConstant
{
    public static class Assembly
    {
        public static string? Name { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
    }
    public static class Domain
    {
        public static string? Name { get; set; } = $"{AppDomain.CurrentDomain.FriendlyName}/";
    }
    public static class ContentType
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationOctetStream = "application/octet-stream";
        public const string ApplicationXml = "application/xml";
    }
    public static class Database
    {
        public const string Effortless = "EffortlessDbConnection";
    }
    public static class Status
    {
        public static class Key
        {
            // General
            public const string Success = "Congratulations! Your request was fulfilled successfully.";
            public const string BadRequest = "Whoops! Something went wrong with your request. Please try again.";
            public const string Forbidden = "Sorry, you don't have permission to access this resource.";
            public const string NotFound = "Hmmm, we couldn't find what you're looking for.";
            public const string Unauthorized = "Oops! You need to be authorized to access this resource.";
            public const string InternalServerError = "Whoops! Something went wrong on our end. Please try again later.";

            public const string DeviceNotVerified = "Whoops! Your device is not verified. Please verify your device first.";
            public const string EmailNotVerified = "Oops! Your email is not verified. Please verify your email first.";
            public const string EmailAlreadyRegistered = "Whoops! Your email is already registered. Please use a different email.";
            public const string MsisdnAlreadyRegistered = "Whoops! Your MSISDN is already registered. Please use a different MSISDN.";
            public const string InvalidMsisdn = "Oops! Your MSISDN is invalid. Please enter a valid MSISDN.";
            public const string FileUploadedError = "Uh oh! There was an error uploading your file. Please try again.";

            // User
            public const string UserBlocked = "Oops! Your account has been blocked. Please contact customer support.";
            public const string UserNotExist = "Whoops! The user you're looking for doesn't exist.";
            public const string UserNotCreated = "Whoops! User could not be created. Please try again later.";
            public const string AccountNotExist = "Whoops! Your account doesn't exist.";

            // Register
            public const string PhoneNumberIsAlreadyInUsed = "Whoops! The phone number is already in use.";
            public const string InvalidCredentials = "Whoops! Your credentials are invalid. Please try again with valid credentials.";

            // Otp
            public const string OtpInvalid = "Oops! Your OTP is invalid. Please try again with the correct OTP.";
            public const string OtpValid = "Congratulations! Your OTP is valid.";
            public const string OtpNotVerified = "Whoops! The provided OTP is not verified";
            public const string OtpSent = "An OTP has been generated and sent to your registered phone number";
            public const string OtpCreationLimitExceeded = "Whoops! Your OTP creation limit exceeded. Please try again later.";
            public const string OtpNotAllowed = "You have been blocked due to exceeding the maximum allowed limit for creating OTPs. Please try again later.";
            public const string OtpNotCreated = "Whoops! Your OTP could not be created. Please try again later.";

            // Token
            public const string JwtTokenMissing = "Uh oh! Token is missing. Please try again with a valid token.";
            public const string JwtTokenExpired = "Whoops! Your token has expired. Please request a new one.";
            public const string JwtTokenInvalid = "Whoops! Your authorization token is invalid. Please try again with a valid token.";

            // Refresh Token
            public const string RefreshToken = "Congratulations! Your JWT token has been successfully refreshed";

        }
        public static class Code
        {
            // Commonly Used Status Codes
            public const int Success = Microsoft.AspNetCore.Http.StatusCodes.Status200OK;
            public const int BadRequest = Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest;
            public const int Forbidden = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden;
            public const int NotFound = Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound;
            public const int Unauthorized = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
            public const int InternalServerError = Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;

            public const int InvalidCredentials = Microsoft.AspNetCore.Http.StatusCodes.Status200OK;
            public const int LockedOut = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden;
            public const int AccountNotExist = Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound;
            public const int UserNotAllowed = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden;
            public const int UserNotCreated = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden;


            // Token
            public const int JwtTokenMissing = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
            public const int JwtTokenInvalid = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
            public const int JwtTokenExpired = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;

            // Refresh
            public const int RefreshToken = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
        }
    }
}
