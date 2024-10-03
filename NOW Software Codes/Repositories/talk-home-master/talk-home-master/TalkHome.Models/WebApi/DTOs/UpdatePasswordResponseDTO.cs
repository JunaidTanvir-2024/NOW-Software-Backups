using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class UpdatePasswordResponseDTO
    {
        public UpdatePassword passwordChange { get; set; }
    }


    public class UpdatePassword
    {
        public bool isChanged { get; set; }
    }
}
