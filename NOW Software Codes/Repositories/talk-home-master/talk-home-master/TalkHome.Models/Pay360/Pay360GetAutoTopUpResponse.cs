using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Pay360
{
    public class Pay360GetAutoTopUpResponse
    {
        public string Msisdn { get; set; }
        public float ThresHold { get; set; }
        public float Topup { get; set; }
        public string Currency { get; set; }
        public bool Status { get; set; }
    }
}
