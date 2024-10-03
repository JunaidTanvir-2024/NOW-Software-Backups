using System;
using System.Collections.Generic;
using System.Text;

namespace TalkHome.Models.Porting
{
    public class SwitchingInformationApiResponseModel
    {
        public decimal TerminationCharge { get; set; }
        public decimal Balance { get; set; }
        public string TerminateDate { get; set; }
        public string Url { get; set; }
        public bool IsPostPaid { get; set; }
        public decimal HandsetCharge { get; set; }
        public decimal AdditionalCharge { get; set; }
    }
}
