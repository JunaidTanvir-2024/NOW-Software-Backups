using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi
{
    public class GenericApiAppResponse<T>
    {
        public string message { get; set; }

        public string status { get; set; }

        public T payload { get; set; }

        public int errorCode { get; set; }
    }
}
