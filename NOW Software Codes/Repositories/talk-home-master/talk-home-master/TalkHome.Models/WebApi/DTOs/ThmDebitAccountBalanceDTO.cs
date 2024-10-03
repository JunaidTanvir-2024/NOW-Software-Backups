using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class ThmDebitAccountBalanceDTO
    {
        public string ProductRef { get; set; }
        public string Amount { get; set; }
        public string TransactionId { get; set; }
        public string AdjustmentReason { get; set; }
        public string PaymentMethod { get; set; }
    }
}
