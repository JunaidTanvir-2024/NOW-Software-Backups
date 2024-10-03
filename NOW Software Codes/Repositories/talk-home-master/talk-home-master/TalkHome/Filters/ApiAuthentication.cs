using System.Web.Mvc;
using TalkHome.Controllers;
using TalkHome.Interfaces;
using TalkHome.Models;
using TalkHome.Models.Enums;

namespace TalkHome.Filters
{
    /// <summary>
    /// Overrides filter actions for Talk Home Web Api authorization checks
    /// </summary>
    public class ApiAuthentication : ActionFilterAttribute
    {
        private Properties.URLs Urls = Properties.URLs.Default;
        public string ReturnUrl;

        /// <summary>
        /// Executes the action before the controller's method is called. Verifies client's authorization validity
        /// </summary>
        /// <param name="filterContext">The context</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Payload = (JWTPayload)filterContext.HttpContext.Items["payload"];

            var AccountService = DependencyResolver.Current.GetService<IAccountService>();

            if (!AccountService.IsAuthorized(Payload))
            {
                ReturnUrl = filterContext.HttpContext.Request.Url.ToString();
                var Controller = (BaseController)filterContext.Controller;
                filterContext.Result = Controller.ErrorRedirect(((int)Messages.Forbidden).ToString(), Urls.Login, ReturnUrl);
            }
        }
    }
}
