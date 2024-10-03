using System.Web.Mvc;
using TalkHome.Models.ViewModels;
using TalkHome.Interfaces;
using TalkHome.Filters;

namespace TalkHome.Controllers
{
    /// <summary>
    /// Exposes basket operations as Json endpoints
    /// </summary>
    [GCLIDFilter]
    public class BasketJsonController : BaseController
    {
        private readonly IContentService ContentService;
        private readonly IAccountService AccountService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public BasketJsonController(IContentService contentService, IAccountService accountService)
        {
            ContentService = contentService;
            AccountService = accountService;
        }

        /// <summary>
        /// Retrieves a product and adds it to the product set
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>True or null</returns>
        [HttpPost]
        public JsonResult Add(BaksetViewModel model)
        {
            if (!ModelState.IsValid)
                return null;

            var Payload = GetPayload();

            if (ContentService.GetProducts(model.Id) == null) return null;

            if (Payload.Purchase.Contains(model.Id)) return null;

            Payload.Purchase.Add(model.Id);
            SetPayload(Payload);
            return Json(new { success = true });
        }

        /// <summary>
        /// Empties the basket first, then if found adds the product id
        /// </summary>
        /// <param name="model">the request model</param>
        /// <returns>True or null</returns>
        [HttpPost]
        [ActionName("empty-add")]
        public JsonResult EmptyAndAdd(BaksetViewModel model)
        {
            if (!ModelState.IsValid)
                return null;

            var Payload = GetPayload();
            Payload.Purchase.Clear();

            if (ContentService.GetProducts(model.Id) != null) Payload.Purchase.Add(model.Id);

            SetPayload(Payload);
            return Json(new { success = true });
        }

        /// <summary>
        /// Empties the Top up set first, then if found adds the product id
        /// </summary>
        /// <param name="model">the request model</param>
        /// <returns>True or null</returns>
        [HttpPost]
        [ActionName("empty-add-top-up")]
        public JsonResult EmptyAndAddTopUp(BaksetViewModel model)
        {
            if (!ModelState.IsValid)
                return null;

            var Payload = GetPayload();

            Payload.TopUp.Clear();

            if (ContentService.GetProducts(model.Id) != null)
                Payload.TopUp.Add(model.Id);

            SetPayload(Payload);
            return Json(new { success = true });
        }

        /// <summary>
        /// Removes the product id from the Top up set
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>True or null</returns>
        [HttpPost]
        public JsonResult Remove(BaksetViewModel model)
        {
            if (!ModelState.IsValid)
                return null;

            var Payload = GetPayload();

            Payload.Purchase.Remove(model.Id);
            SetPayload(Payload);
            return Json(new { success = true });
        }

        /// <summary>
        /// Swaps a product in the basket for another.
        /// </summary>
        /// <param name="model">the request model</param>
        /// <returns>True or null</returns>
        [HttpPost]
        public JsonResult Swap(BaksetViewModel model)
        {
            if (!ModelState.IsValid)
                return null;

            var Payload = GetPayload();

            if (ContentService.GetProducts(model.newId.Value) == null) return null;

            Payload.Purchase.Remove(model.Id);
            Payload.Purchase.Add(model.newId.Value);
            SetPayload(Payload);
            return Json(new { success = true });
        }

        /// <summary>
        /// Swaps a product in the top up set for another
        /// </summary>
        /// <param name="model">the request model</param>
        /// <returns>True or null</returns>
        [HttpPost]
        [ActionName("swap-top-up")]
        public JsonResult SwapTopUp(BaksetViewModel model)
        {
            if (!ModelState.IsValid)
                return null;

            var Payload = GetPayload();

            if (ContentService.GetProducts(model.newId.Value) == null)
                return null;

            Payload.TopUp.Remove(model.Id);
            Payload.TopUp.Add(model.newId.Value);
            SetPayload(Payload);
            return Json(new { success = true });
        }

        /// <summary>
        /// Clears the product set
        /// </summary>
        /// <returns>True or null</returns>
        [HttpPost]
        public JsonResult Clear()
        {
            var Payload = GetPayload();

            if (Payload.Purchase.Count == 0)
                return null;

            Payload.Purchase.Clear();
            Response.Cookies.Add(AccountService.EncodeCookie(Payload));

            return Json(new { success = true });
        }

        /// <summary>
        /// Clears the product set
        /// </summary>
        /// <returns>True or null</returns>
        [HttpPost]
        [ActionName("clear-top-up")]
        public JsonResult ClearTopUp()
        {
            var Payload = GetPayload();

            if (Payload.TopUp.Count == 0)
                return null;

            Payload.TopUp.Clear();
            SetPayload(Payload);
            return Json(new { success = true });
        }
    }
}
