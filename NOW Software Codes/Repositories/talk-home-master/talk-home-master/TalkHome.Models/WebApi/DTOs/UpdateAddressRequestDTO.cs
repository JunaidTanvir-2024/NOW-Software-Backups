using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class UpdateAddressRequestDTO
    {
        public string AddressL1 { get; set; }
        public string AddressL2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }

        public string County { get; set; }

        public string Country { get; set; }

       

       
    }
}
