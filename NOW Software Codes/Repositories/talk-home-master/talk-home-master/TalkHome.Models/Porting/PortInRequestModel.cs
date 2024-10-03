using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Porting
{
    public class PortInRequestModel
    {
        public string Email { get; set; }
        public string NTMsisdn { get; set; }
        public string UserPortingDate { get; set; }
        public string PortMsisdn { get; set; }
        public string Code { get; set; }
        public int OrderRefId { get; set; }
        public Products Product { get; set; }
        public MediumTypes Medium { get; set; }
    }

    public class PortInNewRequestModel
    {
        public string Email { get; set; }
        public string PortMsisdn { get; set; }
        public int OrderRefId { get; set; }
        public string Code { get; set; }
        public Products Product { get; set; }
        public MediumTypes Medium { get; set; }
    }
}
