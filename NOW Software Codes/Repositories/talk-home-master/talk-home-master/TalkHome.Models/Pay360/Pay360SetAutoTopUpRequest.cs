using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Pay360
{
    public class Pay360SetAutoTopUpRequest
    {
        public string productRef { get; set; }
        public string productCode { get; set; }
        public string productItemCode { get; set; }
        public float thresholdBalanceAmount { get; set; }
        public bool isAutoTopup { get; set; }
        public decimal topupAmount { get; set; }
        public string topupCurrency { get; set; }
        public string Email { get; set; }

    }
}
