using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class UserExistsResponseDTO
    {
        public bool UserExists { get; set; }
        public bool HasSim { get; set; }
        public int UserId { get; set; }
        public bool hasProduct { get; set; }
    }
}
