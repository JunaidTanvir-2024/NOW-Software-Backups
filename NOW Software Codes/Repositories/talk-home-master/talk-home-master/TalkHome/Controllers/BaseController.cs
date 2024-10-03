using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using TalkHome.Filters;
using TalkHome.Logger;
using TalkHome.Models;
using TalkHome.Models.CustomExceptions;
using TalkHome.Models.Enums;
using Umbraco.Web.Mvc;
namespace TalkHome.Controllers
{
    /// <summary>
    /// Provides methods to other controllers. Extends Umbraco's render controller.
    /// </summary>
    [GCLIDFilter]
    public class BaseController : RenderMvcController
    {

        private Properties.URLs Urls = Properties.URLs.Default;

        /// <summary>
        /// Retrieves the JWT payload from the current HTTP context.
        /// </summary>
        /// <returns>The payload.</returns>
        protected internal JWTPayload GetPayload()
        {
            return (JWTPayload)HttpContext.Items["payload"];
        }



        /// <summary>
        /// Retrieves the JWT payload from the current HTTP context.
        /// </summary>
        /// <returns>The payload.</returns>
        protected internal JWTPayload GetTestPayload()
        {

            string s = @"{ 'FullName':{ 'Salutation': null,'FirstName': 'ADEMOLA','LastName': null,'Email': 'ogani1@yahoo.co.uk'},'ApiToken': '3F4A4BF82A804EDFA19DD088BB99229E','ApiTokenExpiry': '2018-08-14T10:51:29Z','TwoLetterISORegionName': 'GB','currency':'GBP','currencySymbol': '£','ProductCodes': [{'ProductCode': 'THM','Reference': '447741549553'}],'Purchase': [],'TopUp': [1429],'Basket': [],'OpenSignUp': false,'OpenRegistration': null,'MailOrder': null,'Checkout':{'Reference': '447741549553','Verify': 'THM','ProductType': 'TopUp','MailOrder': null,'Basket': null,'Total': 10,'DeliveryIsBilling': false},'Created': '2018-08-13T09:48:48.3945272+01:00','Updated': '2018-08-13T09:52:43.9928006+01:00','VerifiedReset': false,'Payment': null,'OneClick': null,'CustId': '99A5660B-756D-4D7D-B2EF-8DE20408290C','ResetToken': null,'isTHCCPin': false,'CheckoutProduct': null,'AirTimeTransfer': null,'HomeRoot':'Homepage'}";
            JObject json = JObject.Parse(s);

            return JsonConvert.DeserializeObject<JWTPayload>(s);
        }

        /// <summary>
        /// Retrieves the JWT payload from the current HTTP context.
        /// </summary>
        /// <returns>The payload.</returns>
        protected internal void SetPayload(JWTPayload payload)
        {
            HttpContext.Items["payload"] = payload;
        }

        /// <summary>
        /// Logs and sends a Zabbix alert
        /// </summary>
        /// <returns>The error view</returns>
        protected internal ActionResult RetrievePaymentFailed(ILoggerService loggerService)
        {
            //TODO: Log Critical - App start payment call failed
            loggerService.SendCriticalAlert((int)Messages.RetrievePaymentFailed);

            return ErrorRedirect("0", Urls.Checkout);
        }

        /// <summary>
        /// Logs and sends a Zabbix alert
        /// </summary>
        /// <returns>The error view</returns>
        protected internal ActionResult StartPaymentFailed(ILoggerService loggerService)
        {
            //TODO: Log Critical - App start payment call failed
            loggerService.SendCriticalAlert((int)Messages.StartPaymentFailed);

            return RedirectToAction("failure", new { message = "0" });
        }

        /// <summary>
        /// Logs and sends a Zabbix alert
        /// </summary>
        /// <returns>The error view</returns>
        protected internal ActionResult CorruptedAppTopUpUrl(ILoggerService loggerService)
        {
            //TODO: Log Critical - Corrupted App Top up URL
            loggerService.SendCriticalAlert((int)Messages.CorruptedAppTopUpUrl);

            return RedirectToAction("failure", new { message = "0" });
        }

        /// <summary>
        /// Logs and sends a Zabbix alert
        /// </summary>
        /// <returns>The error view</returns>
        protected internal ActionResult GetMiPayCustomerFailed(ILoggerService loggerService)
        {
            //TODO: Log Critical - App Msisdn Not found
            loggerService.SendCriticalAlert((int)Messages.GetMiPayCustomerFailed);

            return RedirectToAction("failure", new { message = "19" });
        }

        /// <summary>
        /// Logs and sends a Zabbix alert
        /// </summary>
        /// <returns>The error view</returns>
        protected internal ActionResult AppUserNotVerifiedAtCheckout(ILoggerService loggerService)
        {
            //TODO: Log Critical - App Msisdn Not verified at in-app checkout
            loggerService.SendCriticalAlert((int)Messages.AppUserNotVerifiedAtCheckout);

            return RedirectToAction("failure", new { message = "19" });
        }

        /// <summary>
        /// Logs and sends a Zabbix alert
        /// </summary>
        /// <returns>The error view</returns>
        protected internal ActionResult CorruptedAddBundleUrl(ILoggerService loggerService)
        {
            //TODO: Log Critical - Corrupted Add App Bundle URL
            loggerService.SendCriticalAlert((int)Messages.CorruptedAddBundleUrl);

            return RedirectToAction("failure", new { message = "0" });
        }

        /// <summary>
        /// Logs and sends a Zabbix alert
        /// </summary>
        /// <returns>The error view</returns>
        protected internal ActionResult AppBundleNotFound(ILoggerService loggerService)
        {
            //TODO: Log Critical - App bundle for in-app checkout not found by Guid
            loggerService.SendCriticalAlert((int)Messages.AppBundleNotFound);

            return RedirectToAction("failure", new { message = "19" });
        }

        /// <summary>
        /// App start payment request for an empty basket. Critical error, cannot recover.
        /// </summary>
        /// <returns>The error view</returns>
        protected internal ActionResult EmptyAppBasket(ILoggerService loggerService)
        {
            //TODO: Log Critical - App start payment request for an empty basket
            loggerService.SendCriticalAlert((int)Messages.EmptyAppBasket);

            return RedirectToAction("failure", new { message = "19" });
        }



        /// <summary>
        /// Retireves a request's Ip address or defaults to 127.0.0.1
        /// </summary>
        /// <returns>the Ip address</returns>
        public string GetRequestIP()
        {
            HttpContext Context = System.Web.HttpContext.Current;

            if (Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                return Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }

            return "127.0.0.1";
        }

        public void AddAlert(string alertStyle, string message)
        {
            var Alerts = TempData.ContainsKey(Alert.TempDataKey) ? (List<Alert>)TempData[Alert.TempDataKey] : new List<Alert>();

            Alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = string.Format("Message_{0}", message)
            });

            TempData[Alert.TempDataKey] = Alerts;
        }

        /// <summary>
        /// Adds a successful message
        /// </summary>
        /// <param name="message">The message</param>
        public void AddSuccessAlerts(string message)
        {
            var Alerts = TempData.ContainsKey(Alert.TempDataKey) ? (List<Alert>)TempData[Alert.TempDataKey] : new List<Alert>();

            Alerts.Add(new Alert(AlertStyles.Success, message));

            TempData[Alert.TempDataKey] = Alerts;
        }

        /// <summary>
        /// Adds an error message
        /// </summary>
        /// <param name="error">The error</param>
        public void AddErrorAlerts(string error)
        {
            var Alerts = TempData.ContainsKey(Alert.TempDataKey) ? (List<Alert>)TempData[Alert.TempDataKey] : new List<Alert>();

            Alerts.Add(new Alert(AlertStyles.Error, error));

            TempData[Alert.TempDataKey] = Alerts;
        }

        /// <summary>
        /// Adds error messages
        /// </summary>
        /// <param name="errors">The errors</param>
        public void AddErrorAlerts(List<string> errors)
        {
            var Alerts = TempData.ContainsKey(Alert.TempDataKey) ? (List<Alert>)TempData[Alert.TempDataKey] : new List<Alert>();

            foreach (var error in errors)
            {
                Alerts.Add(new Alert(AlertStyles.Error, error));
            }

            TempData[Alert.TempDataKey] = Alerts;
        }

        private void Error(string message)
        {
            AddAlert(AlertStyles.Error, message);
        }

        /// <summary>
        /// Redirects with an error message
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="redirectUrl"></param>
        /// <returns>Redirection</returns>
        protected internal ActionResult HandleRedirect(string alert, string redirectUrl)
        {
            Error(alert);

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Redirects on success
        /// </summary>
        /// <param name="message">The success message</param>
        /// <param name="redirectUrl">The redirect Url</param>
        /// <returns>The view</returns>
        public ActionResult SuccessRedirect(string message, string redirectUrl, string returnUrl = null)
        {
            AddSuccessAlerts(message);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Redictes on failure
        /// </summary>
        /// <param name="error">The error</param>
        /// <param name="redirectUrl">The redirect Url</param>
        /// <param name="returnUrl">Optional return Url</param>
        /// <returns>The view</returns>
        public ActionResult ErrorRedirect(string error, string redirectUrl, string returnUrl = null)
        {
            AddErrorAlerts(error);


            if (!string.IsNullOrEmpty(returnUrl))
            {
              //var returnUrlWithoutHost = new Uri(returnUrl).PathAndQuery;
                 return Redirect(redirectUrl + "?ReturnUrl=" + returnUrl);
                //return Redirect(returnUrl);
            }
            else
            {
                return Redirect(redirectUrl);
            }
        }

        /// <summary>
        /// Redictes on failure
        /// </summary>
        /// <param name="errors">The list of errors</param>
        /// <param name="redirectUrl">The redirect Url</param>
        /// <param name="returnUrl">Optional return Url</param>
        /// <returns>The view</returns>
        public ActionResult ErrorRedirect(List<string> errors, string redirectUrl, string returnUrl = null)
        {
            AddErrorAlerts(errors);

            if (string.IsNullOrEmpty(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Redirects to 404 page
        /// </summary>
        /// <returns>The view</returns>
        protected internal ActionResult ErrorPage()
        {
            return View("ErrorPage");
        }

        /// <summary>
        /// Redirects to log in page
        /// </summary>
        /// <returns>The view</returns>
        protected internal ActionResult Unauthorized()
        {
            return View("Login");
        }


        protected override void OnException(ExceptionContext filterContext)

        {
            Exception exception = filterContext.Exception;
            //Logging the Exception
            ErrorLog.ErrorLog.LogError(exception);

            var payload = GetPayload();

            if (exception is UnAuthorisedException)
            {
                payload.ApiToken = null;
                payload.ApiTokenExpiry = new DateTime();
                SetPayload(payload);
                filterContext.ExceptionHandled = true;
                var baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~")) + "login";
                filterContext.Result = new RedirectResult(baseUrl);
                return;
            }


            if (exception is HttpAntiForgeryException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = this.View("Timeout");
                return;
            }

            filterContext.ExceptionHandled = true;
            string payloadE = "";
            string ac = "";
            string cn = "";

            if (filterContext.RouteData.Values["action"] != null)
            {
                ac = filterContext.RouteData.Values["action"].ToString();
            }

            if (filterContext.RouteData.Values["controller"] != null)
            {
                cn = filterContext.RouteData.Values["controller"].ToString();
            }

            if (Request.Cookies["Account"] != null)
            {
                payloadE = Request.Cookies["Account"].Value;
            }

            ILoggerService lg = new LoggerService();

            string email = "";
            string msisdn = "";

            if (payload.FullName != null)
            {
                email = payload.FullName.Email;
            }

            if (payload.Checkout != null)
            {
                msisdn = payload.Checkout.Reference;
            }

            string message = "Message:{0}:Payload:{1}:Email:{2}:MSIDN:{3}:Controller{4}:Action{5}";
            string formatMessage = String.Format(message, exception.Message, payloadE, payload.FullName.Email, msisdn, cn, ac);

            Dictionary<string, string> substitutions = new Dictionary<string, string>();
            substitutions.Add("%MESSAGE%", formatMessage);
            substitutions.Add("%STACK%", exception.StackTrace);

            MailTemplate mailTemplate = new MailTemplate
            {
                Template = MailTemplate.EXCEPTION_TEMPLATE,
                EmailAddress = "asharp@nowtel.co.uk",
                Substitutions = substitutions,
                From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                Subject = "UndhandledException:TalkHome"
            };

            try
            {
                mailTemplate.SyncSend();
            }

            catch (Exception e)
            {
                lg.Error(GetType(), e.Message, e);
            }


            lg.Error(GetType(), formatMessage, exception);

            if (Request.IsAjaxRequest())
                filterContext.Result= Json(null);
            else
            { 
                var Result = this.View("Error");
            filterContext.Result = Result;
            }

        }
    }
}
