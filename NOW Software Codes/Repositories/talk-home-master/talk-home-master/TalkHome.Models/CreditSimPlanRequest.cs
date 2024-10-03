using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.ViewModels;

namespace TalkHome.Models
{
   public class CreditSimPlanRequest
    {

        public AddressDetailsViewModel AddressDetailsViewModel { get; set; }
        public CreditSimPlanRequest(AddressDetailsViewModel addressDetailsViewModel)
        {
            AddressDetailsViewModel = addressDetailsViewModel;
        }
    }
}
