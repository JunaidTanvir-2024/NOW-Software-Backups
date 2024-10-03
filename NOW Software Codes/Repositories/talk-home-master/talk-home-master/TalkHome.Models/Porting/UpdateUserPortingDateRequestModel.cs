using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Porting
{
    public class UpdateUserPortingDateRequestModel
    {
        [Required(ErrorMessage = "Porting Date is Required"), DataType(DataType.DateTime, ErrorMessage = "Invalid Date")]
        public DateTime PortingDate { get; set; }
        [Required(ErrorMessage = "Port Msisdn is Required")]
        public string PortMsisdn { get; set; }
    }
}
