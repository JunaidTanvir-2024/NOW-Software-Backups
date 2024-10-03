using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models
{
   public class ThResetPassword
    {
        public int ReturnCode { get; set; }
        public string UniqueId { get; set; }
        public string Email { get; set; }
        public DateTime DateTimeRequested { get; set; }
    }
}
