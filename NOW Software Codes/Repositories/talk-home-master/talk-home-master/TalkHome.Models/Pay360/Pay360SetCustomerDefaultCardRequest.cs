using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Pay360
{
   public class Pay360SetCustomerDefaultCardRequest
    {

        [Required(ErrorMessage = "Enter Security Code"), StringLength(maximumLength: 4, MinimumLength = 3, ErrorMessage = "Length must be between (3-4)")]
        public string defaultCardCV2 { get; set; }

        public string pay360CustomerID { get; set; }

        public string cardToken { get; set; }
    }
}
