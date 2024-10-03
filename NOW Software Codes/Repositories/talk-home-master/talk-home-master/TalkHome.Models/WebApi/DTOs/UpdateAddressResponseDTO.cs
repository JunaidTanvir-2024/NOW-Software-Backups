using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class UpdateAddressResponseDTO
    {
        public DetailsUpdate detailsUpdate { get; set; }
       
    }
    public class DetailsUpdate
    {
        public bool isUpdated { get; set; }
    }
}
