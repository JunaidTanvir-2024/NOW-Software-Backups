using KFCFPAPI.AppData;
using KFCFPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
namespace KFCFPAPI.Controllers
{
    public class FPHDSController : ApiController
    {
        [HttpPost]
        [Route("order/{BranchId}")]
        public HttpResponseMessage Post(string branchId, [FromBody] FPOrder order)
        {
            FPMiddelWareHelper obj = new FPMiddelWareHelper();

            string response = obj.ProcessDataAsync(branchId, order);
            if (!response.Contains("remoteOrderId"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
            
            return Request.CreateResponse(HttpStatusCode.Accepted, JsonConvert.DeserializeObject(response) , JsonMediaTypeFormatter.DefaultMediaType);

        }

        [HttpPut]
        [Route("remoteId/{remoteId}/remoteOrder/{remoteOrderId}/posOrderStatus")]
        public HttpResponseMessage UpdateOrderStatus(string remoteId, string remoteOrderId, [FromBody] CancelOrderPayload payLoad)
        {
            FPMiddelWareHelper obj = new FPMiddelWareHelper();
            FPCancelationResponse response = obj.UpdateOrderStatus(remoteId, remoteOrderId, payLoad);
            if (response.Reason.Equals("Success"))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            if (response.Reason.Equals("NOT_FOUND"))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, response);
            }
            if (response.Reason.Equals("INTERNAL_ERROR"))
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
            return Request.CreateResponse(HttpStatusCode.BadGateway, response);
        }
    }

    
}
