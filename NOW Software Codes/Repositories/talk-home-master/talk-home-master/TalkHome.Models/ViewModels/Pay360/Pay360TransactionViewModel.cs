using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//

namespace TalkHome.Models.ViewModels.Pay360
{
    public class Pay360TransactionViewModel
    {
        public bool isSuccess { get; set; }
        public string Reason { get; set; }
        public string TransactionId { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }

        public string url { get; set; }
        public string returnUrl { get; set; }
        public string type { get; set; }
        public string pareq { get; set; }
    }
}