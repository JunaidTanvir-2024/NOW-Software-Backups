using System.Collections.Generic;
using TalkHome.Models.WebApi.Rates;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Models.ViewModels.Umbraco
{
    /// <summary>
    /// Describes a Talk Home Product Landing page.
    /// </summary>
    /// <typeparam name="T">The Umbraco content</typeparam>
    public class TalkHomeProductViewModel : RenderModel
    {
        public JWTPayload Payload { get; set; }

        public TalkHomeProduct Page { get; set; }

        public IEnumerable<IPublishedContent> TopProducts { get; set; }

        public IList<Rate> Rates { get; set; }

        public TalkHomeProductViewModel(JWTPayload payload, TalkHomeProduct content, IEnumerable<IPublishedContent> topProducts,IList<Rate> rates =null) : base(content)
        {
            Payload = payload;

            Page = content;

            TopProducts = topProducts;

            Rates = rates;
        }
    }
}
