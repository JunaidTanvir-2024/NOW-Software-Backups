using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using TalkHome.Controllers;
using TalkHome.Models;
using TalkHome.Models.Enums;
using ActionFilterAttribute = System.Web.Mvc.ActionFilterAttribute;

namespace TalkHome.Filters
{
    public class GCLIDFilter : ActionFilterAttribute
    {
        

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string gclidrequest = string.Empty;
            gclidrequest = filterContext.RequestContext.HttpContext.Request.QueryString["gclid"];

            if (gclidrequest != null)
            {
                string GCLID = gclidrequest;
                if (!filterContext.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("gclid"))
                {
                    HttpCookie gclidcookie = new HttpCookie("gclid");
                    gclidcookie.Value = GCLID;
                    //gclidcookie.Expires = DateTime.Now.AddDays(Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GCLIDDurationTime"]));
                    gclidcookie.Expires = DateTime.Now.AddMinutes(Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GCLIDDurationTime"]));
                    filterContext.HttpContext.Response.Cookies.Add(gclidcookie);
                }

            }
        }
    }


    
}