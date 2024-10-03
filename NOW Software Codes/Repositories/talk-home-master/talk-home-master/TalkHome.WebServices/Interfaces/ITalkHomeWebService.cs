using System.Collections.Generic;
using System.Threading.Tasks;
using TalkHome.Models;
using TalkHome.Models.Pay360;
using TalkHome.Models.Porting;
using TalkHome.Models.ViewModels;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.CallingCards;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.WebApi.History;
using TalkHome.Models.WebApi.Rates;

namespace TalkHome.WebServices.Interfaces
{
    public interface ITalkHomeWebService
    {
        Task<GenericApiResponse<List<CallHistoryRecord>>> GetCallHistoryPage(HistoryPageDTO model);

        Task<GenericApiResponse<TotalPages>> GetCallHistoryTotalPages(GetTotalPages model);

        Task<GenericApiResponse<List<PaymentHistoryRecord>>> GetPaymentHistoryPage(HistoryPageDTO model);

        Task<GenericApiResponse<TotalPages>> GetPaymentHistoryTotalPages(GetTotalPages model);

        Task<GenericApiResponse<SignUpResponseDTO>> SignUp(SignUpRequestDTO model);

        Task<GenericApiResponse<VerifySignUpResponseDTO>> VerifySignUpEmail(string token);

        Task<GenericApiResponse<int>> MailOrder(MailOrderRequestDTO model);

        Task<GenericApiResponse<AccountSummaryResponseDTO>> GetAccountSummary(AccountSummaryRequestDTO model);

        Task<GenericApiResponse<AccountDetailsResponseDTO>> GetAccountDetails(string apiToken);

        Task<GenericApiResponse<AddProductResponseDTO>> AddProduct(AddProductRequestDTO model, string apiToken);

        Task<GenericApiResponse<LoginResponseDTO<AuthenticationContent>>> AuthenticateCustomer(LoginRequestDTO model);

        Task<GenericApiResponse<ResetPasswordResponseDTO>> ResetPasswordRequest(ResetPasswordRequestDTO model);

        Task<GenericApiResponse<ResetPasswordConfirmResponseDTO>> ResetPasswordConfirm(ResetPasswordConfirmRequestDTO model);

        Task<GenericApiResponse<NewPasswordResponseDTO>> NewPassword(NewPasswordRequestDTO model);

        Task<GenericApiResponse<IList<Rate>>> GetTalkHomeMobileTopRates();

        Task<GenericApiResponse<IList<Rate>>> GetTalkHomeAppRates(string countryCode);

        Task<GenericApiResponse<IList<Rate>>> GetTalkHomeAppTopRates(string countryCode);

        Task<GenericApiResponse<IList<TopMinute>>> GetCallingCardsTopMinutes();

        Task<GenericApiResponse<IList<MinutesRecord>>> GetCallingCardsMinutes();

        Task<GenericApiResponse<IList<UKNationalRate>>> GetTalkHomeMobileUKRates();

        Task<GenericApiResponse<IList<Rate>>> GetTalkHomeMobileInternationalRates();

        Task<GenericApiResponse<IList<RoamingRate>>> GetTalkHomeMobileRoamingRates();

        Task<GenericApiResponse<string>> PromoSignUp(PromoSignUpRequestDTO model);

        Task<GenericApiResponse<string>> RedeemVocuher(RedeemVoucherRequestDTO model, string apiToken);

        Task<GenericApiResponse<string>> RedeemPoints(RedeemPointsRequestDTO model, string apiToken);

        Task<GenericApiResponse<string>> AutoRenewSettings(AutoRenewSettingsRequestDTO model, string apiToken);

        Task<GenericApiResponse<string>> AutoTopUpSettings(AutoTopUpSettingsRequestDTO model, string apiToken);

        Task<GenericApiResponse<string>> AutoRenewSettings(AutoRenewalsSettingsRequestDTO model, string apiToken);

        Task<GenericApiResponse<UpdateAddressResponseDTO>> UpdateAddress(UpdateAddressRequestDTO model, string addresstype, string apiToken);

        Task<GenericApiResponse<UpdatePasswordResponseDTO>> UpdatePassword(UpdatePasswordRequestDTO model);

        Task<GenericApiResponse<string>> AddBundleWithCredit(AddBundleDTO model, string apiToken);

        Task<GenericApiResponse<string>> TransferCredit(TransferCreditDTO model, string apiToken);

        Task<GenericApiResponse<string>> ThmCreditAutoTopupAccountBalance(string subscriberid, string transReferenceId, decimal amountinPounds, string PaymentMethod);
        Task<GenericApiResponse<ThmDebitAccountBalanceResponseDTO>> ThmDebitAccountBalance(ThmDebitAccountBalanceDTO model, string apiToken);
        Task<GenericPay360ApiResponse<CreditSimOrderResponse>> CreditSimOrder(CreditSimOrderApiRequest model);
        Task<GenericPay360ApiResponse<string>> CreditSimPayment(CreditSimPaymentApiRequest model);
        Task<GenericApiAppResponse<string>> CreditSimFullfillment(string Msisdn);

    }
}
