using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.ViewModels
{
    public class CreditPurchasesViewModel
    {
        public JWTPayload Payload { get; set; }
        public List<string> BundleNames { get; set; }
        public decimal CreditRemaining { get; set; }

        public CreditPurchasesViewModel()
        {

        }
    }
}
