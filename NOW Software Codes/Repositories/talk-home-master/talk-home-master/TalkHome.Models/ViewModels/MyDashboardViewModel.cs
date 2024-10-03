using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.ViewModels
{
    public class MyDashboardViewModel
    {
        public JWTPayload Payload { get; set; }

        
        public MyDashboardViewModel(JWTPayload payload)
        {
            Payload = payload;
        }
    }
}
