using System.Collections.Generic;
using TalkHome.Models.Porting;
using TalkHome.Models.WebApi.DTOs;
using Umbraco.Core.Models;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// View model for the settings page
    /// </summary>
    public class PortDetailsViewModel
    {
        public JWTPayload Payload { get; set; }
        public IList<PortResponseModel> PortList { get; set; }
        public decimal Balance { get; set; }
    }

}
