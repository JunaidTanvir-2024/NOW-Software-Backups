using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    public class AirtimeTransfer
    {
        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress{get;set;}
        public string[] TransferMessage { get; set;}
        public string PackageId { get; set; }
        public string PaymentOption { get; set; }
        public string TransactionReference { get; set; }
        public string Cost { get; set; }
        public string OperatorId { get; set; }
        public string ProductName { get; set; }
        public string Currency { get; set; }
        public string Msisdn { get; set; }
        public string ToMsisdn { get; set; }
        public string TransferAmount { get; set; }
        
    }
}
