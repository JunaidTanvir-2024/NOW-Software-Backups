using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Porting
{
    public class SwitchingInfoResponseModel
    {
        public int PortingId { get; set; }
        public float TerminationCharge { get; set; }
        public float Balance { get; set; }
        public DateTime TerminationDate { get; set; }
        public string Url { get; set; }
        public bool IsPostPaid { get; set; }
        public float HandsetCharge { get; set; }
        public float AdditionalCharge { get; set; }
    }
}
