using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models;
using TalkHome.Models.Pay360;
using TalkHome.Models.ViewModels.Pay360;
using TalkHome.Models.ViewModels.Payment;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.Payment;

namespace TalkHome.Interfaces
{
    public interface IPay360Service
    {
        string GetResumeUrl(string path);
        Task<GenericPay360ApiResponse<Pay360PaymentResponse>> Resume3DTransaction(Pay360Resume3DRequest request);
        Task<GenericPay360ApiResponse<Pay360CardsResponse>> Pay360GetCards(Pay360CustomerRequestModel request);
        Pay360PaymentRequestToken CreatePay360PaymentRequestToken(StartPay360ViewModel model);
        Pay360PaymentBase CreatePay360PaymentBaseRequest(StartPay360ViewModel model);
        Pay360PaymentRequestNew CreatePay360PaymentRequestNew(StartPay360ViewModel model);
        Pay360PaymentRequestExistingNew CreatePay360PaymentRequestExistingNew(StartPay360ViewModel model);
        Task<GenericPay360ApiResponse<Pay360PaymentResponse>> Pay360Payment(Pay360PaymentRequest request, Pay360PaymentType pt, JWTPayload payload, string IP,bool Iscreditsim=false);
        Task<GenericPay360ApiResponse<Pay360CustomerModel>> GetCustomer(Pay360CustomerRequestModel model);
        bool IsValidPostCode(string postCode, out string cleanedPostCode);        
        bool TryGetCustId(Pay360CustomerModel result, out string custId);
        Task<GenericPay360ApiResponse<string>> SetAutoTopUp(Pay360SetAutoTopUpRequest model);
        Task<GenericPay360ApiResponse<Pay360GetAutoTopUpResponse>> GetAutoTopUp(Pay360GetAutoTopUpRequest model);
        Task<GenericPay360ApiResponse<paymentMethodResponse>> SetCustomerDefaultCard(Pay360SetCustomerDefaultCardRequest model);
        Task<GenericPay360ApiResponse<string>> RemoveCard(Pay360RemoveCardRequest model);
    }
}
