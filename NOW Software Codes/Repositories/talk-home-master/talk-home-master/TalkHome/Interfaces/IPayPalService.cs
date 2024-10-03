using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TalkHome.Models;
using TalkHome.Models.PayPal;
using TalkHome.Models.ViewModels.Payment;

namespace TalkHome.Interfaces
{
    public interface IPayPalService
    {
        string GetResumeUrl(string path);
        Task<GenericPayPalApiResponse<PayPalCreateSalePaymentResponse>> PayPalCreateSalePayment(PayPalCreateSalePaymentRequest request, JWTPayload Payload,bool Iscreditsim=false);
        Task<GenericPayPalApiResponse<PayPalExecuteSalePaymentResponse>> PayPalExecuteSalePayment(PayPalExecuteSalePaymentRequest request);

        Task<GenericPayPalApiResponse<Pay360PayPalCreateSalePaymentResponse>> Pay360PayPalCreateSalePayment(StartPay360PaymentViewModel request, JWTPayload Payload,string ipAddress,bool Iscreditsim);
        Task<GenericPayPalApiResponse<Pay360PayPalCreateResumePaymentResponse>> Pay360ResumePayment(Pay360PayPalResumePaymentRequest model);
    }
}