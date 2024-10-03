using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TalkHome.Interfaces;
using TalkHome.Models;
using TalkHome.Models.Enums;
using System.Linq;
using TalkHome.Logger;
using TalkHome.Filters;
using TalkHome.Models.WebApi.DTOs;
using System.Threading.Tasks;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Basket endpoints controllers
    /// </summary>
    [GCLIDFilter]
    public class BasketController : BaseController
    {
        private readonly IAccountService AccountService;
        private readonly IContentService ContentService;
        private readonly ILoggerService LoggerService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public BasketController(IContentService contentService, IAccountService accountService, ILoggerService loggerService)
        {
            ContentService = contentService;
            AccountService = accountService;
            LoggerService = loggerService;
        }

        /// <summary>
        /// Checks if a given product Id exists
        /// </summary>
        /// <param name="id">The product Id</param>
        /// <returns>Result as TRUE or FALSE</returns>
        private bool ProductExists(int id)
        {
            if (ContentService.GetProducts(id) == null)
                return false;

            return true;
        }

        /// <summary>
        /// Adds an item to the basket in the cookie payload
        /// </summary>
        /// <param name="id">The product Id</param>
        /// <returns>The view</returns>
        [ApiAuthentication]
        public async Task<ActionResult> Add(int id)
        {
            var Payload = GetPayload();

            if (Payload.ProductCodes.Count == 0)
                return ErrorRedirect(((int)Messages.ProductNotRegisteredForPurchase).ToString(), Urls.ConfirmProductDetails); // No registered products

            var Model = new BasketRequest { Id = id };

            if (!Validator.TryValidateObject(Model, new ValidationContext(Model, null, null), null, true)) // Is Id valid?
                return ErrorRedirect(((int)Messages.InvalidId).ToString(), Urls.Basket);

            if (!ProductExists(id))
                return ErrorRedirect(((int)Messages.UnexistingProduct).ToString(), Urls.Basket); // Does the product exist?

            var ProductCode = ContentService.GetProductCode(id);

            try
            {
                Payload.ProductCodes.Where(x => x.ProductCode.Equals(ProductCode)).Select(x => x.ProductCode).First(); // Has the user registred the product?
            }
            catch
            {
                return ErrorRedirect(((int)Messages.ProductNotRegisteredForPurchase).ToString(), Urls.MyAccount + "/" + ProductCode); // Prompt to register the product
            }

            if (Payload.Basket.Count > 0) // Does the first item in the basket match the added product code?
            {
                var FirstItemProducCode = ContentService.GetProductCode(Payload.Basket.First());
                if (!FirstItemProducCode.Equals(ProductCode))
                {
                    LoggerService.SendInfoAlert(string.Format("{0} {1}->{2}", Messages.BasketProductMismatch.ToString(), ProductCode, FirstItemProducCode));
                    LoggerService.Warn(GetType(), string.Format("{0} {1}->{2}", Messages.BasketProductMismatch.ToString(), ProductCode, FirstItemProducCode));
                    return ErrorRedirect(((int)Messages.BasketProductMismatch).ToString(), Urls.Basket);
                }
            }


            var RequestDTO = new AccountSummaryRequestDTO { productCode = ProductCode, token = Payload.ApiToken };
            var ResponseDTO = await AccountService.GetAccountSummary(RequestDTO);

            if (ResponseDTO == null)
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.CustomErrorPage); // Request failed

            /*
            if (ContentService.GetProductsTotal(ProductCode,Payload.Basket,id) > (int)ResponseDTO.payload.userAccountSummary.creditRemaining)
            {
                return ErrorRedirect(((int)Messages.AddToBasketInsufficentCredit).ToString(), Urls.Basket);
            }
            */

            Payload.Basket.Add(id);
            SetPayload(Payload);
            return SuccessRedirect(((int)Messages.AddToBasketSuccess).ToString(), Urls.Basket);
        }

        /// <summary>
        /// Removes an item to the basket in the cookie payload
        /// </summary>
        /// <param name="id">The product Id</param>
        /// <returns>The view</returns>
        [ApiAuthentication]
        public ActionResult Remove(int id)
        {
            var Model = new BasketRequest { Id = id };

            if (!Validator.TryValidateObject(Model, new ValidationContext(Model, null, null), null, true))
                return ErrorRedirect(((int)Messages.InvalidId).ToString(), Urls.Basket);

            if (!ProductExists(id))
                return ErrorRedirect(((int)Messages.UnexistingProduct).ToString(), Urls.Basket);

            var Payload = GetPayload();

            Payload.Basket.Remove(id);
            SetPayload(Payload);
            return SuccessRedirect(((int)Messages.RemoveFromBasketSuccess).ToString(), Urls.Basket);
        }

        /// <summary>
        /// Empties the basket in the cookie payload
        /// </summary>
        /// <returns>The view</returns>
        [ApiAuthentication]
        public ActionResult Clear()
        {
            Uri Referrer = Request.UrlReferrer;

            var Payload = GetPayload();

            Payload.Basket.Clear();
            SetPayload(Payload);
            return SuccessRedirect(((int)Messages.ClearBasketSuccess).ToString(), Urls.Basket);
        }
    }
}
