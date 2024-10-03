using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models
{
   public class CreditSimPayload
    {
        public int userId { get; set; }
        public int orderId { get; set; }
        public string Email { get; set; }
        public string ProductType { get; set; }
        public int PaymentType { get; set; }
        public string signedUp { get; set; }
        public string Name { get; set; }
        public string Gclid { get; set; }
    }
}
