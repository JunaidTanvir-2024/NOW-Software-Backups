using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.Pay360;

namespace TalkHome.Models.WebApi
{
    public class CreditSimOrderApiRequest
    {

        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public int creditSimType { get; set; }
        public string addressL1 { get; set; }
        public string addressL2 { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string country { get; set; }
        public string postCode { get; set; }
        public List<CreditSimbasket> basket { get; set; }

    }
}
