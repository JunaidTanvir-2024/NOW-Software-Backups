using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class UpdatePersonalDetailsRequest
    {
       public string OldEmailAddress { get; set; }
       public string NewEmailAddress { get; set; }
       public string FirstName { get; set; }
       public string LastName { get; set; }
    }
}
