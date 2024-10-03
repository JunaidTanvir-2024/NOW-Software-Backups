using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class UpdatePasswordRequestDTO
    {       
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
