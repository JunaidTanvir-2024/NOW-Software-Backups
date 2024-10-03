using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Porting
{
    public class GetPortingRequestsResponseModel
    {
        public IEnumerable<PortResponseModel> PortingRequestList { get; set; }
    }
}
