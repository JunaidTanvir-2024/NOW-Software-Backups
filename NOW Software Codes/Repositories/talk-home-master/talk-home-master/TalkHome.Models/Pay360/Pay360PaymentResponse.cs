using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
namespace TalkHome.Models.Pay360
{

    public class clientRedirect
    {
        public string pareq { get; set; }
        public string url { get; set; }
        public string type { get; set; }
    }

    public class outcome
    {
        public string status { get; set; }
        public string reasonCode { get; set; }
        public string reasonMessage { get; set; }
    }


    public class authResponse
    {
        public string statusCode { get; set; }
        public string acquirerName { get; set; }
        public string message { get; set; }
        public string gatewayReference { get; set; }
        public string gatewayMessage {get;set;}
        public string avsAddressCheck { get; set; }
        public string cv2Check { get; set; }
        public string status { get; set; }
        public string authCode { get; set; }
        public string gatewaySettlement { get; set; }
        public string gatewayCode { get; set; }
        public string avsPostcodeCheck { get; set; }
    }

    public class processing
    {
        public authResponse authResponse { get; set; }
        public string route { get; set; }
    }

    public class Pay360PaymentResponse
    {
        public string customerId { get; set; }
        public string  transactionId { get; set; }
        public string  transactionAmount { get; set; }
        public outcome outcome { get; set; }
        public processing processing { get; set; }
        public clientRedirect clientRedirect { get; set; }
    }
}