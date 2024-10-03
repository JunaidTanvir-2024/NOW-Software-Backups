using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.Validation;

namespace TalkHome.Models.ViewModels.Payment
{
    public class StartPay360PaymentViewModel
    {
        public string customerName { get; set; }
        
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }

       
        public string City { get; set; }

        public string CountyOrProvince { get; set; }

 
        public string CountryCode { get; set; }

       
        public string PostalCode { get; set; }


        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }


        public int Amount { get; set; }

        public string ProductType { get; set; }

        public string FirstUseDate { get; set; }

        public CreditSimPayload CreditSim { get; set; }

        public string returnUrl { get; set; }
        public string cancelUrl { get; set; }
    }
}
