using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalkHome.Models.Porting;
using TalkHome.Models.WebApi;

namespace TalkHome.Models.ViewModels
{
    public class SwitchInfoViewModel
    {
        public JWTPayload Payload { get; set; }
        public PortResponseModel Port { get; set; }
        public MyAccountViewModel MyAccount { get; set; }
    }
}
