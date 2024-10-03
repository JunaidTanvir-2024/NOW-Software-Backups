using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Porting
{
    public class PortOutRequestModel
    {
        public string Email { get; set; }
        public string NTMsisdn { get; set; }
        public Products Product { get; set; }
        public MediumTypes Medium { get; set; }
        public CodeTypes CodeType { get; set; }
        public string UserPortingDate { get; set; }
        public int ReasonId { get; set; }
    }
}
