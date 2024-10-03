using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class AppMsisdnResponseDTO
    {
        /*
        declare @msisdn varchar(50),
        @errorcode int ,
        @errormsg varchar(100)
        select @msisdn = '447825152591'
        exec tha_validate_msisdn @msisdn,@errorcode output, @errormsg output

        select @errorcode,@errormsg
        */

        public bool isApplicable { get; set; }
        public int errorCode { get; set; }
    }
}
