using System.Web.Optimization;

namespace TalkHome
{
    /// <summary>
    /// Registers bundles for minification of JS and CSS files.
    /// </summary>
    public class BundlesConfig
    {
        private static void JavascriptBundle(BundleCollection bundles)
        {
            var scriptBundle = new ScriptBundle("~/bundles/THMJquery");

            scriptBundle.Include(
                "~/js/vendor/jquery-3.2.0.min.js",
                    "~/js/vendor/ionicons.js",
                  "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                 "~/js/main.js",
                "~/js/vendor/megamenu.js",
                "~/js/modules/Utils.js",
                "~/js/modules/Basket.js",
                "~/js/modules/Help.js",
                "~/js/modules/Menu.js",
                "~/js/modules/Rates.js",
                "~/js/modules/AddressLookup.js",
                "~/js/modules/AirTimeTransfer.js",
                "~/js/modules/UI.js",
                "~/js/modules/Widgets.js",
                "~/js/vendor/raphael-2.1.4.min.js",
                "~/js/vendor/Player.js",
                 "~/js/vendor/cloudflare.js",
                "~/js/vendor/gstatic.js",
                "~/js/remodal/dist/remodal.min.js",
                    "~/CallingCardHomeCSSJS/build/js/intlTelInput.js",
                "~/CallingCardHomeCSSJS/build/js/utils.js",
                   "~/js/modules/MyAccount.js",
                   "~/js/modules/CreditSimOrder.js",
                "~/js/modules/Checkout.js",
                "~/js/EcommeraceTracking/Analytics.js",
                 "~/js/EcommeraceTracking/AnaylticsEvents.js"
                );

            bundles.Add(scriptBundle);
        }

        /// <summary>
        /// configures CSS files.
        /// </summary>
        /// <param name="bundles">Registered bundles object</param>
        private static void CssBundle(BundleCollection bundles)
        {
            var styleBundle = new StyleBundle("~/bundles/THMCSS");
            styleBundle.Include(
                "~/CallingCardHomeCSSJS/build/css/intlTelInput.css",
               "~/css/googleapis.css",
               "~/css/ionicons.min.css",
               "~/css/bootstrap-grid.min.css",
               "~/fonts/fontawesome/css/all.min.css",
               "~/css/normalize.css",
               "~/js/remodal/dist/remodal.css",
               "~/js/remodal/dist/remodal-default-theme.css",
               "~/css/main.css",
               "~/css/style.css",
               "~/css/widgets.css",
               "~/css/cloudflare.css",
               "~/css/user-account.css",
               "~/js/swiper-slider/css/swiper.min.css",
               "~/css/hero-slider.css"
               );

            bundles.Add(styleBundle);

        }

        /// <summary>
        /// Register JS and CSS resource bundles.
        /// </summary>
        /// <param name="bundles">Registered bundles object</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            JavascriptBundle(bundles);

            CssBundle(bundles);

            //BundleTable.EnableOptimizations = true;
        }
    }
}
