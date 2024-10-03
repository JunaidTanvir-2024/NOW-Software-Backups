using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.Models;
using TalkHome.Models.ViewModels.History;
using TalkHome.Interfaces;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.History;
using TalkHome.Models.WebApi.Rates;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.CustomExceptions;
using TalkHome.Filters;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Manages HTTP requests for history records pages.
    /// </summary>
    [GCLIDFilter]
    [ApiAuthentication]
    public class HistoryController : BaseController
    {
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly IAccountService AccountService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public HistoryController(ITalkHomeWebService talkHomeWebService, IAccountService accountService)
        {
            TalkHomeWebService = talkHomeWebService;
            AccountService = accountService;
        }

        /// <summary>
        /// Returns the result of the call history WebApi request.
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="pageNo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<GenericApiResponse<List<CallHistoryRecord>>>
             GetCallRecords(string productCode, int pageNo, string token)
        {
            return await TalkHomeWebService.GetCallHistoryPage(new HistoryPageDTO(productCode, pageNo, 25, token));
        }

        /// <summary>
        /// Returns the total number of pages for the call history.
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<GenericApiResponse<TotalPages>> GetCallsTotalPages(string productCode, string token)
        {
            var Model = new GetTotalPages(productCode, 25, token);

            return await TalkHomeWebService.GetCallHistoryTotalPages(Model);
        }

        /// <summary>
        /// Returns the result of the call history WebApi request.
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="pageNo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<GenericApiResponse<List<PaymentHistoryRecord>>> GetPaymentRecords(string productCode, int pageNo, string token)
        {
            var Model = new HistoryPageDTO(productCode, pageNo, 25, token);

            return await TalkHomeWebService.GetPaymentHistoryPage(Model);

        }

        /// <summary>
        /// Returns the total number of pages for the payment history.
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<GenericApiResponse<TotalPages>> GetPaymentsTotalPages(string productCode, string token)
        {
            var Model = new GetTotalPages(productCode, 25, token);

            return await TalkHomeWebService.GetPaymentHistoryTotalPages(Model);
        }

        /// <summary>
        /// Returns the view for Talk Home Mobile call history.
        /// </summary>
        /// <param name="pageNo">The page number to retrieve.</param>
        /// <returns>The view with the required model.</returns>
        public async Task<ActionResult> THM(int pageNo)
        {
            var Payload = GetPayload();

            if (!AccountService.IsAuthorized(Payload))
                return HandleRedirect(GenericMessages.Forbidden, Urls.Login);

            var Records = await GetCallRecords("thm", pageNo, Payload.ApiToken);
            var TotalPages = await GetCallsTotalPages("thm", Payload.ApiToken);

            if (Records == null)
                return View(new HistoryViewModel<CallHistoryRecord>(Payload, pageNo, "thm", null, null));

            return View(new HistoryViewModel<CallHistoryRecord>(Payload, pageNo, "THM", Records.payload, TotalPages.payload));
        }

        /// <summary>
        /// Returns the view for Talk Home App call history
        /// </summary>
        /// <param name="pageNo">The page number to retrieve.</param>
        /// <returns>The view with the required model.</returns>
        public async Task<ActionResult> THA(int pageNo)
        {
            var Payload = GetPayload();

            if (!AccountService.IsAuthorized(Payload))
                return HandleRedirect(GenericMessages.Forbidden, Urls.Login);

            var Records = await GetCallRecords("tha", pageNo, Payload.ApiToken);
            var TotalPages = await GetCallsTotalPages("tha", Payload.ApiToken);

            if (Records == null)
                return View(new HistoryViewModel<CallHistoryRecord>(Payload, pageNo, "tha", null, null));

            return View(new HistoryViewModel<CallHistoryRecord>(Payload, pageNo, "THA", Records.payload, TotalPages.payload));
        }

        /// <summary>
        /// Returns the payment history page for Talk Home Mobile.
        /// </summary>
        /// <param name="pageNo">The page number to retrieve.</param>
        /// <returns>The view with the required model.</returns>
  
       
        [ActionName("THM-payments")]
        public async Task<ActionResult> THMPayments(int pageNo)
        {
            var Payload = GetPayload();
            //if (!AccountService.IsAuthorized(Payload))
            //    return HandleRedirect(GenericMessages.Forbidden, Urls.Login);

            var Records = await GetPaymentRecords("thm", pageNo, Payload.ApiToken);
            var TotalPages = await GetPaymentsTotalPages("thm", Payload.ApiToken);
        if(Request.IsAjaxRequest())
            {
                if (Records == null)
                    return PartialView("PaymentHistory",new HistoryViewModel<PaymentHistoryRecord>(Payload, pageNo, "thm", null, null));

                return PartialView("PaymentHistory", new HistoryViewModel<PaymentHistoryRecord>(Payload, pageNo, "THM", Records.payload, TotalPages.payload));

            }
            if (Records == null)
                return View(new HistoryViewModel<PaymentHistoryRecord>(Payload, pageNo, "thm", null, null));

            return View(new HistoryViewModel<PaymentHistoryRecord>(Payload, pageNo, "THM", Records.payload, TotalPages.payload));
        }

        /// <summary>
        /// Returns the payment history page for Talk Home App.
        /// </summary>
        /// <param name="pageNo">The page number to retrieve.</param>
        /// <returns>The view with the required model.</returns>
        [ActionName("tha-payments")]
        public async Task<ActionResult> THAPayments(int pageNo)
        {
            var Payload = GetPayload();

            if (!AccountService.IsAuthorized(Payload))
                return HandleRedirect(GenericMessages.Forbidden, Urls.Login);

            var Records = await GetPaymentRecords("tha", pageNo, Payload.ApiToken);
            var TotalPages = await GetPaymentsTotalPages("tha", Payload.ApiToken);

            if (Records == null)
                return View(new HistoryViewModel<PaymentHistoryRecord>(Payload, pageNo, "tha", null, null));

            return View(new HistoryViewModel<PaymentHistoryRecord>(Payload, pageNo, "THA", Records.payload, TotalPages.payload));
        }

        /// <summary>
        /// Returns the payment history page for Calling Cards.
        /// </summary>
        /// <param name="pageNo">The page number to retrieve.</param>
        /// <returns>The view with the required model.</returns>
        [ActionName("thcc-payments")]
        public async Task<ActionResult> THCCPayments(int pageNo)
        {
            var Payload = GetPayload();

            if (!AccountService.IsAuthorized(Payload))
                return HandleRedirect(GenericMessages.Forbidden, Urls.Login);

            var Records = await GetPaymentRecords("thcc", pageNo, Payload.ApiToken);
            var TotalPages = await GetPaymentsTotalPages("thcc", Payload.ApiToken);

            if (Records == null)
                return View(new HistoryViewModel<PaymentHistoryRecord>(Payload, pageNo, "thcc", null, null));

            return View(new HistoryViewModel<PaymentHistoryRecord>(Payload, pageNo, "THCC", Records.payload, TotalPages.payload));
        }


        [ActionName("intelligentcalls")]
        public async Task<string> IntelligentCalls()
        {
            var Payload = GetPayload();


            if (!AccountService.IsAuthorized(Payload))
                return "";
                //return HandleRedirect(GenericMessages.Forbidden, Urls.Login);

            var Records = await GetCallRecords("thm", 1, Payload.ApiToken);
            var TotalPages = await GetCallsTotalPages("thm", Payload.ApiToken);

            return "aaa";

            //return PartialView(@"~/Views/Partials/Widgets/_IntelligentCalls.cshtml");
        }

    }
}
