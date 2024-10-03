using Umbraco.Core.Models;
using Umbraco.Web.Models;
using TalkHome.Models.ViewModels.BusinessIntelligence;
using System.Collections.Generic;
using TalkHome.Models.WebApi.CallingCards;

namespace TalkHome.Models.ViewModels.Umbraco
{
    /// <summary>
    /// Model for Homepage content type. Extends Umbraco's default model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HomepageViewModel<T> : RenderModel
    {
        public JWTPayload Payload { get; set; }

        public T Page { get; set; }

        public IPublishedContent Campaign { get; set; }

        public WidgetViewModel Widgets { get; set; }

        public TalkHomeProductViewModel CallingCardViewModel { get; set; }

        public IList<MinutesRecord> CallingCardMinutes { get; set; }

        public HomepageViewModel(JWTPayload payload, IPublishedContent content, IPublishedContent campaign,  
            WidgetViewModel widgets, TalkHomeProductViewModel tpvm, IList<MinutesRecord> ccm) : base(content)
        {
            Payload = payload;

            Page = (T)content;          

            Campaign = campaign;

            Widgets = widgets;

            CallingCardViewModel = tpvm;

            CallingCardMinutes = ccm;
        }
    }
}
