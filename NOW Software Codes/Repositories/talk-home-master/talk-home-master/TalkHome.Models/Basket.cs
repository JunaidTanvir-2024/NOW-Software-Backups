using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models
{
    public class Basket
    {
        public int amount { get; set; }
        public string currency { get; set; } = "GBP";
        public string bundleId { get; set; }
    }
}
