using PhoneNumbers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using TalkHome.Models;
using TalkHome.Models.Porting;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.App;
using TalkHome.Models.WebApi.DTOs;

namespace TalkHome.Interfaces
{
    public interface IAccountService
    {


        Task<DBResponseDTO> SimPromoOrder(MailOrderRequest model);

        Task<AppMsisdnResponseDTO> ValidAppUser(string msisdn);

        Task<GenericApiResponse<AppUserModel>> GetAppUserByMsisdn(string msisdn);

        Task<GenericApiResponse<AccountDetailsResponseDTO>> GetAccountDetails(string apiToken);

        Task<GenericApiResponse<ResetPasswordResponseDTO>> ResetPasswordRequest(ResetPasswordRequestDTO model);

        Task<GenericApiResponse<ResetPasswordConfirmResponseDTO>> ResetPasswordConfirm(ResetPasswordConfirmRequestDTO model);

        Task<GenericApiResponse<NewPasswordResponseDTO>> NewPassword(NewPasswordRequestDTO model);

        Task<GenericApiResponse<LoginResponseDTO<AuthenticationContent>>> AuthenticateCustomer(LoginRequestDTO model);

        Task<GenericApiResponse<VerifySignUpResponseDTO>> VerifySignUpEmail(string token);

        Task<GenericApiResponse<AccountSummaryResponseDTO>> GetAccountSummary(AccountSummaryRequestDTO model);

        Task<GenericApiResponse<string>> PromoSignUp(PromoSignUpRequestDTO model);

        Task<UserExistsResponseDTO> UserExists(string email);

        Task<LegacyCardUserExistsResponseDTO> LegacyCardUserExists(string email, string password);

        bool IsAuthorized(JWTPayload payload);

        void SetCultureOnCurrentThread(string countryCode);

        List<I18nCountry> GetCountryList();

        bool HasAnyProducts(JWTPayload payload);

        HttpCookie EncodeCookie(JWTPayload payload);

        PhoneNumber ValidateNumber(string number, string countryCode);

        bool TryValidateNumber(string number, string countryCode, out string formattedNumber);

        bool IsAlreadyActive(string productCode, JWTPayload payload);

        bool TryValidateProductCode(string productCode, out string error);

        string GetReadableProductCode(string productCode, out string error);

        string GetProductName(string productCode, out string error);

        string GetMsisdnFromNumber(string number, string countryCode);

        string GetNumberFromMsisdn(string msisdn, string countryCode);

        bool TryGetBillingAddress(GenericApiResponse<AccountDetailsResponseDTO> AccountDetails, out AddressModel addressModel);

        bool TryPromoSignUpSuccess(PromoSignUpRequestDTO requestDTO, GenericApiResponse<string> responseDTO, out string error);

        Task<string> GetSubscriberId(string msisdn);

        Task<DBResponseDTO> UpdatePersonalDetails(UpdatePersonalDetailsRequest details);

        string GetRechargeableNumber(string pin, out int error);

        Task<LegacyCardUserExistsPinResponseDTO> LegacyCardUserExistsWithPin(string emailAddress, string pin);
        int AddCompitionUser(AddCompitionUserRequestModel model);
        int IsRegistered(string Email,string promoname);
        Task<ThResetPassword> InsertPasswordToken(string email);
        Task<string> SubscriberName(string email);
        Task<SimReturnResponse> InsertSimOrder(MailOrderRequestDTO so);
        IEnumerable<BillingCountries> GetCountries();
        string Get_Sim_ActivationDate(string Msisdn);
        Task<UserExistsResponseDTO> UserExistsandUserId(string emailAddress);
        Task<string> Get_UserId(string Email);
        int SaveGCLID_NormalSim(string GCLID,long OrderId);
        int SaveGCLID_CreditSim(string GCLID, long OrderId);
        int Insertemail_minutemaker(Insertemail_minutemaker_model model);
        Task<int> Insert_otp_data(int otp,string email);
        Task<int> Verify_user_otp(int otp, int expiry, string EmailAddress);
        Task<string> GetUserByEmail(string email);
        int Isvalid_simorder(string email);
        Task<int> SImOrderValidations(string email, string address, string postcode);
        Task<int> IsEmailRegistered(string emailAddress);
        Task<bool> Verifyuseremail_against_otpAsync(string email);

    }
}
