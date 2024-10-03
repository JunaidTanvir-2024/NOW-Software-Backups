using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Porting
{
    public class GetPortingRequestsRequestModel
    {
        [Required(ErrorMessage = "Email Is Required"), DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
    }
}
