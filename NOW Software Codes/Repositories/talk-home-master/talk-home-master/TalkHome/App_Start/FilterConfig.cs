using System.Web.Mvc;
using TalkHome.Filters;

namespace TalkHome.App_Start
{
    /// <summary>
    /// Registers custom filters
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Register filters
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ApiAuthentication());

            filters.Add(new CheckoutPageRequest());

            filters.Add(new GCLIDFilter());
        }
    }
}
