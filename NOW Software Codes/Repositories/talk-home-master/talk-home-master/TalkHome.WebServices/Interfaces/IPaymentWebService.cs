using System.Threading.Tasks;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.WebApi.Payment;

namespace TalkHome.WebServices.Interfaces
{
    public interface IPaymentWebService
    {
        Task<GenericApiResponse<MiPayCustomerModel>> GetCustomer(GetCustomerRequestModel model);

        Task<GenericApiResponse<StartPaymentResponseDTO>> StartPayment(StartPaymentRequestDTO model);

        Task<GenericApiResponse<PaymentRetrieveResponse>> PaymentRetrieve(PaymentRetrieveRequest model);

        Task<GenericApiResponse<OneClickCheckoutResponse>> OneClickCheckout(OneClickCheckoutRequest model);

        Task<GenericApiResponse<OneClickCheckoutResponse>> OneClickRetrieve(RetrieveOneClickRequest model);

        Task<GenericApiResponse<VerifyNumberResponseDTO>> VerifyNumber(VerifyNumberRequestDTO model);

    }
}
