using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class MailOrderValidationRequest
    {
        public string EmailAddress { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
    }
}
