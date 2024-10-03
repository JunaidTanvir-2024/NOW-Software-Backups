 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Pay360
{
    /*
    {
     "customerUniqueRef": "darmendrat@gmail.com",
     "productCode": "TH"
    }
    */

    public class Pay360CustomerRequestModel
        {           
            public string customerUniqueRef { get; set; }
            public string productCode { get; set; }        
        }
}
