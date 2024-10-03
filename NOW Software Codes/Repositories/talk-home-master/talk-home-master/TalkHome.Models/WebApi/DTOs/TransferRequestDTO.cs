using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class TransferRequestDTO
    {
        public string nowtelTransactionReference { get; set; }

        public string operatorid { get; set; }

        public string product { get; set; }

        public string messageToRecipient { get; set; }

        public string fromMSISDN { get; set; }
    }
}
