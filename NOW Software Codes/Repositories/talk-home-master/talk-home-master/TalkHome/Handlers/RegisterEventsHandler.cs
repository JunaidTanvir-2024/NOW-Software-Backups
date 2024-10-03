using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Umbraco.Core;

namespace TalkHome.Handlers
{
    /// <summary>
    /// Extends Umbraco's application's event class
    /// </summary>
    public class RegisterEventsHandler : ApplicationEventHandler
    {
        /// <summary>
        /// Registers custom routes.
        /// </summary>
        /// <param name="umbracoApplication">The Umbraco application.</param>
        /// <param name="applicationContext">The context of the application.</param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            BundlesConfig.RegisterBundles(BundleTable.Bundles);
            RouteTable.Routes.MapMvcAttributeRoutes();
            RouteTable.Routes.MapRoute(
                "Basket",
                "basketapi/{action}/{id}",
                defaults: new { controller = "Basket", action = UrlParameter.Optional, id = UrlParameter.Optional });

            RouteTable.Routes.MapRoute(
                "BasketJson",
                "basket/api/{action}",
                defaults: new { controller = "BasketJson", action = UrlParameter.Optional });

            RouteTable.Routes.MapRoute(
                "Account",
                "account/{action}/{productCode}",
                defaults: new { controller = "Account", action = UrlParameter.Optional, productCode = UrlParameter.Optional });

            RouteTable.Routes.MapRoute(
              "Pay360Account",
              "pay360account/{action}/{productCode}",
              defaults: new { controller = "Pay360Account", action = UrlParameter.Optional, productCode = UrlParameter.Optional });

            RouteTable.Routes.MapRoute(
                "History",
                "account/history/{action}/{pageNo}",
                defaults: new { controller = "History", action = UrlParameter.Optional, pageNo = UrlParameter.Optional });

            RouteTable.Routes.MapRoute(
                "Customer",
                "customer/{action}/{addressType}", 
                defaults: new { controller = "Customer", action = UrlParameter.Optional,addressType = UrlParameter.Optional});

            RouteTable.Routes.MapRoute(
                "AddressLookup",
                "address-lookup/{action}/{postCode}",
                defaults: new { controller = "AddressLookup", action = UrlParameter.Optional, postCode = UrlParameter.Optional });

            RouteTable.Routes.MapRoute(
                "App",
                "app/{action}",
                defaults: new { controller = "App", action = UrlParameter.Optional});

            //RouteTable.Routes.MapRoute(
            //    "Offers",
            //    "offers/{action}",
            //    defaults: new { controller = "Offers", action = UrlParameter.Optional });

            RouteTable.Routes.MapRoute(
                "PayPal",
                "paypal/{action}",
                defaults: new { controller = "PayPal", action = UrlParameter.Optional });

            RouteTable.Routes.Ignore("bundles/{*catch}");

        }
    }
}
