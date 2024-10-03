using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using TalkHome.Models;
using TalkHome.Services.Modules;
using TalkHome.WebServices.Modules;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using TalkHome.Logger;
using TalkHome.WebServices.Interfaces;
using TalkHome.Interfaces;
using System.Threading.Tasks;
using TalkHome.I18n;
using TalkHome.Services;

namespace TalkHome.Handlers
{
    /// <summary>
    /// Website entry point. Registers IoC and ensures every request served by the website contain a valid JWT cookie.
    /// </summary>
    public class UmbracoApplicationHandler : UmbracoApplication
    {
        private Properties.URLs Urls = Properties.URLs.Default;

        /// <summary>
        /// Fills in default customer info based on a culture.
        /// </summary>
        /// <param name="countryCode">The culture</param>
        /// <returns>The payload</returns>
        private JWTPayload CreatePayload(string countryCode)
        {
            var Payload = new JWTPayload();
            var I18nCurrencies = Currencies.Instance.I18nCurrencies;

            if (I18nCurrencies.ContainsKey(countryCode))
            {
                Payload.currency = I18nCurrencies[countryCode].currency;
                Payload.currencySymbol = I18nCurrencies[countryCode].currencySymbol;
            }
            else
            {
                Payload.currency = I18nCurrencies["default"].currency;
                Payload.currencySymbol = I18nCurrencies["default"].currencySymbol;
            }

            Payload.TwoLetterISORegionName = countryCode;
            Payload.Created = DateTime.Now;
            return Payload;
        }

        /// <summary>
        /// Ensures an UmbracoContext object is always available.
        /// </summary>
        /// <remarks>This fixes crashes when MVC controllers inherit from Umbraco's RenderMvcController</remarks>
        /// <seealso cref="https://our.umbraco.org/forum/extending-umbraco-and-using-the-api/75946-ensurecontext-issue"/></seealso>
        private void EnsureUmbracoContext()
        {
            if (UmbracoContext.Current == null)
            {
                var dummyHttpContext = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("dummy.aspx", "", new StringWriter())));

                UmbracoContext.EnsureContext(
                    dummyHttpContext,
                    ApplicationContext.Current,
                    new WebSecurity(dummyHttpContext, ApplicationContext.Current),
                    UmbracoConfig.For.UmbracoSettings(),
                    UrlProviderResolver.Current.Providers,
                    false);
            }
        }

        /// <summary>
        /// Registers containers and controllers at application start up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnApplicationStarted(object sender, EventArgs e)
        {
            base.OnApplicationStarted(sender, e);

            var Builder = new ContainerBuilder();

            Builder.RegisterControllers(Assembly.GetExecutingAssembly()); // Controllers found in this assembly
            Builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            
            Builder.RegisterApiControllers(typeof(UmbracoApplication).Assembly); // Controllers found in Umbraco application
            Builder.RegisterApiControllers(Assembly.Load("Articulate"));

            Builder.RegisterModule<LoggerModule>(); // Registers module for TalkHome.Log
            Builder.RegisterModule<ServicesModule>(); // Registers module for TalkHome.Services
            Builder.RegisterModule<WebServicesModule>(); // Registers module for TalkHome.WebServices

            var Container = Builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
            GlobalConfiguration.Configuration.DependencyResolver= new AutofacWebApiDependencyResolver(Container);

            RegisterAutomappers.Register(); // Avoid initiliasing Automapper as this would overrride previous maps needed by Umbraco
        }

        /// <summary>
        /// Ensures the request contains the cookie used to profile each customer and also ensures it has a valid signature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_BeginRequest(object sender, EventArgs e)


        {         
            EnsureUmbracoContext(); // Fixes crashes on MVC controllers

            var JWTService = DependencyResolver.Current.GetService<IJWTService>();
            var IpInfoWebService = DependencyResolver.Current.GetService<IIpInfoWebService>(); // Get services from Autofac container. Cannot inject to the ctor

            var Ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //var Ip = "217.33.40.242"; // Shad Thames office
            //var Ip = "46.30.56.242"; // Germany
            //var Ip = "85.17.24.66"; // Netherlands
            //var Ip = "82.60.44.122"; // Italy

            if (Request.Cookies["PlanAB"] == null)
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 100);
                string route = "B";
                if (randomNumber < 50)
                    route = "A";
                Response.Cookies["PlanAB"].Value = route;
                Response.Cookies["PlanAB"].Expires = DateTime.Today.AddDays(1);
            }
            

            var Cookie = Request.Cookies[Urls.CookieName];

            if (Cookie == null) // Cookie not found
            {
                if (Ip == null)
                {
                    var Payload = CreatePayload("GB");
                    HttpContext.Current.Items["payload"] = Payload;
                    Response.Cookies.Add(JWTService.EncodeCookie(Payload));
                    return;
                }
                else
                {
                    var GeoLookup = Task.Run(async () => await IpInfoWebService.GetLocation(Ip)).Result;

                    //var Payload = CreatePayload((GeoLookup!=null) ? GeoLookup.country : "GB");
                    // Disabled the GEO location temporarily 

                    var Payload = CreatePayload("GB");
                    HttpContext.Current.Items["payload"] = Payload;
                    Response.Cookies.Add(JWTService.EncodeCookie(Payload));
                    return;
                }
            }
            else
            {
                var Payload = JWTService.DecodeJWTToken(Cookie.Value); // Decode and verify validity of token

                if (Payload == null) // Invalid token
                {
                    if (Ip == null)
                    {
                        Payload = CreatePayload("GB");
                        Response.Cookies.Add(JWTService.EncodeCookie(Payload));
                        return;
                    }
                    else
                    {
                        var GeoLookup = Task.Run(async () => await IpInfoWebService.GetLocation(Ip)).Result;
                        Payload = CreatePayload(GeoLookup.country ?? "GB");
                        Response.Cookies.Add(JWTService.EncodeCookie(Payload));
                        return;
                    }
                }


                HttpContext.Current.Items["payload"] = Payload; // Valid. Proceed
            }
        }

        /// <summary>
        /// Ensures the cookie is alwayd returned in the response, also for internal redirections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            try
            {
                var JWTService = new JWTService();
                var Payload = (JWTPayload)HttpContext.Current.Items["payload"]; // Retrieve the payload from the HTTP request context

                Response.Cookies.Add(JWTService.EncodeCookie(Payload)); // Encode payload and create cookie. Add to response

            } catch (Exception ex)
            {

            }
        }
    }
}
