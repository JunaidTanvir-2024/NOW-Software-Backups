using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.ViewModels.PayPal
{
    public class SuccessReturnPayPalViewModel
    {
        public string uRef { get; set; }
        public string paymentId { get; set; }
        public string token { get; set; }
        public string PayerID { get; set; }
        public string ProductCode { get; set; }
    }
}
