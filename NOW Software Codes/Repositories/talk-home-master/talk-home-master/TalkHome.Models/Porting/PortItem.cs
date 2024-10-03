using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Porting
{
    public enum Resolution
    {
        Cancelled,
        Failure,
        Success
    }

    public enum PortType
    {
        [Display(Name = "Number Port In")]
        PortIn,
        [Display(Name = "Number Port Out")]
        PortOut,
        [Display(Name = "Number No Port")]
        NoPort
    }

    public enum PortReason
    {
        [Display(Name = "Call Quality")]
        CallQuality=1,
        [Display(Name = "Coverage")]
        Coverage=2,
        [Display(Name = "Customer Service")]
        CustomerService=3,
        [Display(Name = "Other")]
        Other=4,
        [Display(Name = "Promotion")]
        Promotion=5,
        [Display(Name = "Rates")]
        Rates=6,
        [Display(Name = "Subscription Change")]
        SubscriptionChange=7
    }

    public class PortItem
    {
        public string ExpiryDate { get; set; }
        public string Title { get; set; }
        public string PortingMsisdn { get; set; }                
        public PortType PortType { get; set; }
        public string PAC { get; set; }
        public string NPAC { get; set; }
        

        public int Status { get; set; }
        public string Message { get; set; }

    }
}
