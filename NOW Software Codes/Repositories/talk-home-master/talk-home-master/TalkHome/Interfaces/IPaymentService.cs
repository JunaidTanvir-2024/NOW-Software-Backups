using System.Collections.Generic;
using System.Threading.Tasks;
using TalkHome.Models;
using TalkHome.Models.Enums;
using TalkHome.Models.ViewModels.Payment;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.WebApi.Payment;

namespace TalkHome.Interfaces
{
    public interface IPaymentService
    {
        Task<GenericApiResponse<OneClickCheckoutResponse>> OneClickRetrieve(RetrieveOneClickRequest model);

        Task<GenericApiResponse<OneClickCheckoutResponse>> OneClickCheckout(OneClickCheckoutRequest model);

        Task<GenericApiResponse<MiPayCustomerModel>> GetCustomer(GetCustomerRequestModel model);

        Task<GenericApiResponse<StartPaymentResponseDTO>> StartPayment(StartPaymentRequestDTO model);

        Task<GenericApiResponse<PaymentRetrieveResponse>> PaymentRetrieve(PaymentRetrieveRequest model);

        Task<GenericApiResponse<VerifyNumberResponseDTO>> VerifyNumber(VerifyNumberRequestDTO model);
        
        string GetCheckoutTotal(HashSet<int> basket, bool pence = false);

        decimal GetCheckoutTotal(HashSet<int> items);

        bool TryGetCustId(GenericApiResponse<MiPayCustomerModel> result, out string miPayGuid);

        bool TryPaymentSuccess(GenericApiResponse<StartPaymentResponseDTO> result, out string error);

        bool TryFindTransaction(JWTPayload payload, out string error);

        bool TryFindOneClick(JWTPayload payload, out string error);

        bool TryTransactionSuccess(GenericApiResponse<PaymentRetrieveResponse> result, out string error);

        bool TryOneClickSuccess(GenericApiResponse<OneClickCheckoutResponse> result, out string error);

        bool IsOneClickElegible(JWTPayload payload, MiPayCustomerModel MiPayCustomer, StartPaymentViewModel model);

        Task<RetrieveOneClickRequest> TryOneClickTopUp(StartPaymentViewModel model, JWTPayload payload, ChannelType channelType, string error);

        Task<RetrieveOneClickRequest> TryOneClickPurchase(StartPaymentViewModel model, JWTPayload payload, ChannelType channelType, string error);

        Task<bool> VerifyNumber(JWTPayload payload, string number, string countryCode);

        Task<bool> VerifyQuickTopUpNumber(string number, string countryCode);

        StartPaymentRequestDTO CreatePaymentRequest(JWTPayload payload, StartPaymentViewModel input, ChannelType channelType);

        bool IsValidPostCode(string postCode,out string cleanedPostCode);
    }
}
