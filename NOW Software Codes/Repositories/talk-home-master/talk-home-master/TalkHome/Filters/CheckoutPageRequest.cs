using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TalkHome.Controllers;
using TalkHome.Models;
using TalkHome.Models.Enums;

namespace TalkHome.Filters
{
    /// <summary>
    /// Ensures a valid checkout object is found in the JWT payload
    /// </summary>
    public class CheckoutPageRequest : ActionFilterAttribute
    {
        private Properties.URLs Urls = Properties.URLs.Default;

        /// <summary>
        /// Fires the filter before the controller's action execution 
        /// </summary>
        /// <param name="filterContext">The filter content</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Payload = (JWTPayload)filterContext.HttpContext.Items["payload"];
            var Controller = (BaseController)filterContext.Controller;

            if (Payload.Checkout == null)
                filterContext.Result = Controller.ErrorRedirect(((int)Messages.MissingCheckoutObject).ToString(), Urls.TopUp);

            //if (string.IsNullOrEmpty(Payload.Checkout.Reference) && !Payload.Checkout.Verify.Equals(Models.Enums.ProductCodes.THCC.ToString()))
            //    filterContext.Result = Controller.ErrorRedirect(((int)Messages.InvalidCheckout).ToString(), Urls.Checkout);

            else if (!Validator.TryValidateObject(Payload.Checkout, new ValidationContext(Payload.Checkout, null, null), null, true))
                filterContext.Result = Controller.ErrorRedirect(((int)Messages.InvalidCheckoutObject).ToString(), Urls.TopUp);
        }
    }
}
