using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Pay360
{
    public class Pay360GetAutoTopUpRequest
    {
        public string Msisdn { get; set; }
        public string Email { get; set; }
        public string productCode = "THM";
            
        
    }
}
