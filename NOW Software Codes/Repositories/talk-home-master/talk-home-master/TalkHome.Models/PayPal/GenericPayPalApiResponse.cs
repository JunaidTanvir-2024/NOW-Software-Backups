using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.PayPal
{
    public class GenericPayPalApiResponse<T>
    {
        public string message { get; set; }
        public string status { get; set; }
        public int errorCode { get; set; }
        public T payload { get; set; }
    }
}
