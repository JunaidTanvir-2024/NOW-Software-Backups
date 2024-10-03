using System.Collections.Generic;
using TalkHome.Models.WebApi.App;
using Umbraco.Core.Models;

namespace TalkHome.Models.ViewModels.App
{
    /// <summary>
    /// Describes the model for an in-app top up page request.
    /// </summary>
    public class AppTopUpViewModel
    {
        public JWTPayload Payload { get; set; }

        public AppUserModel AppUser { get; set; }

        public IEnumerable<IPublishedContent> TopUps { get; set; }

        public AppTopUpViewModel(JWTPayload payload, AppUserModel appUser, IEnumerable<IPublishedContent> topUps)
        {
            Payload = payload;

            AppUser = appUser;

            TopUps = topUps;
        }
    }
}
