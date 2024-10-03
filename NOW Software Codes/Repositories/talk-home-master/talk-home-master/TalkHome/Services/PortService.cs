using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Interfaces;
using TalkHome.Logger;
using TalkHome.Models.Porting;

namespace TalkHome.Services
{
    /// <summary>
    /// Collection of general utilities
    /// </summary>
    public class PortService : IPortService
    {
        private readonly IJWTService JWTService;
        private readonly ILoggerService LoggerService;
        private HttpClient client;
        private string PortingApiEndpoint { get; set; }
        //private bool PortingStimulationMode { get; set; }
        private string TalkHomeWebConnectionString { get; set; }

        public PortService(IJWTService jwtService, ILoggerService loggerService)
        {
            JWTService = jwtService;
            LoggerService = loggerService;
            TalkHomeWebConnectionString = ConfigurationManager.ConnectionStrings["TalkHomeWebDb"].ConnectionString;
            PortingApiEndpoint = ConfigurationManager.AppSettings["PortingApiEndpoint"];
            //PortingStimulationMode = Boolean.Parse(ConfigurationManager.AppSettings["PortingStimulationMode"]);
        }

        public async Task<GenericPortingApiResponse<GetPortingRequestsResponseModel>> GetPortRequests(GetPortRequestsModel request)
        {
            try
            {
                GenericPortingApiResponse<GetPortingRequestsResponseModel> response = new GenericPortingApiResponse<GetPortingRequestsResponseModel>();
                string endpoint = PortingApiEndpoint + "api/Porting/GetPortingRequests";
                var Result = await Get(endpoint, null, $"Email={request.Email}", $"Msisdn={request.Msisdn}", $"Product={Products.TalkHome}");
                if (Result == null)
                {
                    return null;
                }

                return response = JsonConvert.DeserializeObject<GenericPortingApiResponse<GetPortingRequestsResponseModel>>(Result.Content.ReadAsStringAsync().Result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenericPortingApiResponse<bool>> PortOut(PortOutRequestModel request)
        {
            string endpoint = PortingApiEndpoint;
            if (request.CodeType == CodeTypes.STAC)
            {
                endpoint += "api/Porting/PortOutSTAC";
            }
            else
            {
                endpoint += "api/Porting/PortOutPAC";
            }
            string json = JsonConvert.SerializeObject(request);
            var Result = await Get(endpoint, null, $"Email={request.Email}", $"NTMsisdn={request.NTMsisdn}", $"Product={Products.TalkHome}", $"Medium={MediumTypes.Web}", $"UserPortingDate={request.UserPortingDate}", $"ReasonId={request.ReasonId}");

            return JsonConvert.DeserializeObject<GenericPortingApiResponse<bool>>(Result.Content.ReadAsStringAsync().Result);
        }

        public async Task<GenericPortingApiResponse<bool>> PortIn(PortInRequestModel request, PortTypes type)
        {
            string endpoint = PortingApiEndpoint;
            HttpResponseMessage response = null;

            if (type == PortTypes.PortIn)
            {
                endpoint += "api/Porting/PortIn";
                string json = JsonConvert.SerializeObject(request);
                response = await Get(endpoint, null, $"Email={request.Email}", $"PortMsisdn={request.PortMsisdn}", $"NTMsisdn={request.NTMsisdn}", $"UserPortingDate={request.UserPortingDate}", $"Code={request.Code}", $"Product={Products.TalkHome}", $"Medium={MediumTypes.Web}");
            }
            else if (type == PortTypes.PortInNew)
            {
                endpoint += "api/Porting/PortInNewOrder";
                string json = JsonConvert.SerializeObject(request);
                response = await Get(endpoint, null, $"Email={request.Email}", $"PortMsisdn={request.PortMsisdn}", $"Code={request.Code}", $"OrderReferenceId={request.OrderRefId}", $"Product={Products.TalkHome}", $"Medium={MediumTypes.Web}");
            }

            if (response == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<GenericPortingApiResponse<bool>>(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<GenericPortingApiResponse<bool>> CancelPorting(CancelPortingRequestModel request)
        {
            string endpoint = PortingApiEndpoint;
            HttpResponseMessage response = null;

            endpoint += "api/Porting/CancelPortingRequest";
            string json = JsonConvert.SerializeObject(request);
            response = await Get(endpoint, null, $"RequestID={request.RequestID}");
            if (response == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<GenericPortingApiResponse<bool>>(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<GenericPortingApiResponse<SwitchingInformationApiResponseModel>> GetSwitchingInfo(GetSwitchingInformationRequestModel request)
        {
            string endpoint = PortingApiEndpoint;
            HttpResponseMessage response = null;

            endpoint += "api/Porting/GetSwitchingInformation";
            string json = JsonConvert.SerializeObject(request);
            response = await Get(endpoint, null, $"msisdn={request.msisdn}");
            if (response == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<GenericPortingApiResponse<SwitchingInformationApiResponseModel>>(response.Content.ReadAsStringAsync().Result);
        }

        //public async Task<Port> PortIn(Port request, string subscriberId)
        //{
        //    string endpoint = String.Format($"{PortingApiEndpoint}subscribers/{subscriberId}/sim/porting/gb/port-in");
        //    string json = JsonConvert.SerializeObject(request);
        //    var Result = await Post(endpoint, json);
        //    /*** the following line is a mock of the result **/
        //    if (PortingStimulationMode)
        //    {
        //        Result = json;
        //    }
        //    /** end mock **/
        //    if (Result == null)
        //    {
        //        return null;
        //    }

        //    return JsonConvert.DeserializeObject<Port>(Result);
        //}

        public async Task<HttpResponseMessage> Get(string Uri, string authToken = null, params string[] parameters)
        {
            HttpResponseMessage response;

            try
            {
                string paramString = parameters.Count() > 0 ? "?" : String.Empty;

                using (client = new HttpClient())
                {
                    foreach (var param in parameters)
                    {
                        paramString += param + "&";
                    }
                    paramString = paramString.TrimEnd('&');

                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                    if (authToken != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                    }


                    response = await client.GetAsync(Uri + paramString);

                  return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<HttpResponseMessage> Post(string Uri, object model, string AuthToken = null)
        {
            try
            {
                using (client = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                    if (AuthToken != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthToken);
                    }

                    string json = JsonConvert.SerializeObject(model);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                    return await client.PostAsync(Uri, stringContent);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
