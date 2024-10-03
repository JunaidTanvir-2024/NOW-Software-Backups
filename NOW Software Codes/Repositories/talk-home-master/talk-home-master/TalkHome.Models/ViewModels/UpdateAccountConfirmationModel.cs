using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.ViewModels
{
    public class UpdateAccountConfirmationModel
    {
        public JWTPayload Payload { get; set; }
        public string ProductCode { get; set; }
        public string Message { get; set; }
    }
    
}
