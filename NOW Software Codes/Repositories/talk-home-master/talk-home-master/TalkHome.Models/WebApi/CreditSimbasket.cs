using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi
{
   public class CreditSimbasket
    {
        public int amount { get; set; }
        public string currency { get; set; } = "GBP";
            public string bundleId { get; set; }
    }
}
