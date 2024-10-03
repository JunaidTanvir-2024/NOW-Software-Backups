using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TalkHome.Models.Pay360
{

    /*
    "msisdn": "923325345126",
    "thresholdBalanceAmount": 10,
    "isAutoTopup": true,
    "topupAmount": 15,
    "topupCurrency": "USD"
    }
    */
    public class AutoTopUpRequest
    {
        public string msisdn { get; set;}
        public int thresholdBalanceAmount { get; set; }
        public bool isAutoTopup { get; set; }
        public int topupAmount { get; set; }
        public string topupCurrency { get; set; }
    }
}