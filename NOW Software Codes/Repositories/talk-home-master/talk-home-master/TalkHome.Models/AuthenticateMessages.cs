namespace TalkHome.Models
{
    public static class AuthenticateMessages
    {
        public const string InvalidModel = "Please check that all fields were correctly provided.";

        public const string AuthenticationFailed = "The email or password provided do not match our records.";

        public const string AccountAlreadyExists = "An account with that email address already exists. Do you want to reset the password?";

        public const string AccountNotFound = "We could not find that account. Have you registered that email address?";

        public const string AccountNotConfirmed = "We sent a link via email to your inbox to confirm your account. Please activate your account by following that link.";

        public const string InvalidToken = "An error occurred while verying your request. Please try again.";

        public const string AuthenticationExpired = "The log in has expired. Please log in again.";

        public const string EmailNotFound = "We could not find an account registred with that email address.";

        public const string MissingSlug = "Please log in first.";

        public const string UnrecognisedAccount = "The details provided were invalid. Please double check them and try again.";

        public const string ProductAlreadyActivated = "This account has already registred this product. Please visit the page of your account if you wish to make any changes.";

        public const string InvalidMobileNumber = "There was a problem with the mobile number. Please double check its format and try again.";

        public const string UnknownError = "An unkwown error occurred while logging in. Please try again.";
    }
}
