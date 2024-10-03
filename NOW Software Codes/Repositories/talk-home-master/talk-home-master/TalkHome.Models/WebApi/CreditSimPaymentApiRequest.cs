using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi
{
   public class CreditSimPaymentApiRequest
    {
        public int userId { get; set; }
        public int orderId { get; set; }
        public int paymentType { get; set; }
        public string paymentTransactionId { get; set; }
        public string paymentErrorMessage { get; set; }
        public int paymentErrorCode { get; set; }
        public DateTime paymentDate { get; set; }
    }
}
