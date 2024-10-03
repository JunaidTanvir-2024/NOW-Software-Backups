using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models
{
   public class SimReturnResponse
    {
        public int reference { get; set; }
        public int errorCode { get; set; }
        public string errorMsg { get; set; }
        public Int64 OrderId { get; set; }

    }
}
