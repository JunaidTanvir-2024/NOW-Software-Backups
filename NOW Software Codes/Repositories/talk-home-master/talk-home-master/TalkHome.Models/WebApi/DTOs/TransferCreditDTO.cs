using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class TransferCreditDTO
    {
        public string FromMsisdn { get; set; }
        public string ToMsisdn { get; set; }
        public string Credit { get; set; }
    }
}
