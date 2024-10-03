using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class AddBundleDTO
    {
        public string MsisdnOrCardNumber { get; set; }
        public string BundleId { get; set; }
        public string ProductCode { get; set;}
    }
}
