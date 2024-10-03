using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Pay360
{
   public class Pay360RemoveCardRequest
    {
        public string cardToken { get; set; }
        public string pay360CustomerID { get; set; }
    }
}
