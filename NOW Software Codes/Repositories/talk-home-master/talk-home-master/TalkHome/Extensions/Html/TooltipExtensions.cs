using System.Web.Mvc;
using TalkHome.Interfaces;
using TalkHome.Models.Enums;
using Umbraco.Core.Models;

namespace TalkHome.Extensions.Html
{
    /// <summary>
    /// Provides Html extentions for the Tooltips
    /// </summary>
    public static class TooltipExtensions
    {
        private static readonly IContentService ContentService;
        private static Properties.ContentIds ContentIds = Properties.ContentIds.Default;

        static TooltipExtensions()
        {
            ContentService = DependencyResolver.Current.GetService<IContentService>();
        }

        /// <summary>
        /// Returns the tooltip content for the Number input based on product code
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="productCode">The product code</param>
        /// <returns>The content or null</returns>
        public static IPublishedContent GetConfirmProductNumberTooltips(this HtmlHelper helper, string productCode)
        {
            if (productCode.Equals(ProductCodes.THA.ToString()))
            {
                return ContentService.GetGenericContentById(int.Parse(ContentIds.AppNumberTooltip));
            }

            else if (productCode.Equals(ProductCodes.THCC.ToString()))
            {
                return ContentService.GetGenericContentById(int.Parse(ContentIds.CallingCardNumberTooltip));
            }

            return null;
        }

        /// <summary>
        /// Returns the tooltip content for the Code input based on product code
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="productCode">The product code</param>
        /// <returns>The content or null</returns>
        public static IPublishedContent GetConfirmProductCodeTooltips(this HtmlHelper helper, string productCode)
        {
            if (productCode.Equals(ProductCodes.THM.ToString()))
            {
                return ContentService.GetGenericContentById(int.Parse(ContentIds.PUKTooltip));
            }

            else if (productCode.Equals(ProductCodes.THA.ToString()))
            {
                return ContentService.GetGenericContentById(int.Parse(ContentIds.AppPINTooltip));
            }

            return null;
        }
    }
}
