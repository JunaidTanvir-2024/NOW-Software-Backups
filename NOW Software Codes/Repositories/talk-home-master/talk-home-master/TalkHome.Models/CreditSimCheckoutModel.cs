using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.WebApi.DTOs;

namespace TalkHome.Models
{
   public class CreditSimCheckoutModel
    {
        public int BundleId { get; set; }
        public int Amount { get; set; }
        public string ProductCode { get; set; }
        public int TopUpId { get; set; }
   
    }
}
