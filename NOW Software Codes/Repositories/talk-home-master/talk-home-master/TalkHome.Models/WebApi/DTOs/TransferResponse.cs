using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class TransferResponse
    {
        public int errorCode { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string reference { get; set; }
    }
}
