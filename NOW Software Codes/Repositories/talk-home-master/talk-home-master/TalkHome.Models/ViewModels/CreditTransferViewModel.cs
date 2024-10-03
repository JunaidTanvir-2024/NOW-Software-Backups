using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.ViewModels
{
    public class CreditTransferViewModel
    {
        public JWTPayload Payload { get; set; }
        public decimal CreditRemaining { get; set; }
        public string TransactionReference { get; set; }

        public CreditTransferViewModel()
        {

        }
    }

    public class CreditTransferViewModelTHM
    {

        public decimal CreditRemaining { get; set; }
        public string TransactionReference { get; set; }

        public CreditTransferViewModelTHM()
        {

        }
    }
}
