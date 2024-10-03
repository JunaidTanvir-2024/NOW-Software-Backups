using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace TalkHome.Models.CustomExceptions
{

    public class AuthAttribute : ActionFilterAttribute
    {
        //public void Onact(AuthenticationContext filterContext)
        //{
        //    var url = "";
        //    if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        url = filterContext.HttpContext.Request.Url.ToString();
        //        filterContext.Result = new RedirectResult(url, true);

        //    }
        //    else
        //    {
        //        url = "/Login";
        //        filterContext.Result = new RedirectResult(url, true);
        //    }

        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)

        {
            var url = "";
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                url = filterContext.HttpContext.Request.Url.ToString();
            }
            else
            {
                url = "/Login";
                filterContext.Result = new RedirectResult(url);
            }
        }

    }
}
