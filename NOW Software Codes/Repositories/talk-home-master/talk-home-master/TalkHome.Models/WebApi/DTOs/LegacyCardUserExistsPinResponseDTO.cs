using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class LegacyCardUserExistsPinResponseDTO :DBResponseDTO
    {
        public bool UserExists { get; set; }
        public string CallingCardNumber { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
     
    }
}
