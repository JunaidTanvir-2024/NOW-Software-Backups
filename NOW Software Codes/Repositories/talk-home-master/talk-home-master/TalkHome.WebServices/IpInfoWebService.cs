using Newtonsoft.Json;
using System.Threading.Tasks;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models.Enums;
using TalkHome.Models.WebApi.IpInfo;
using TalkHome.Logger;

namespace TalkHome.WebServices
{
    /// <summary>
    /// Handles requests/responses for IP lookups via ipinfo.io
    /// </summary>
    public class IpInfoWebService : IIpInfoWebService
    {
        private readonly IHttpService HttpService;
        private readonly ILoggerService LoggerService;

        public IpInfoWebService(IHttpService httpService, ILoggerService loggerService)
        {
            HttpService = httpService;
            LoggerService = loggerService;
        }

        /// <summary>
        /// Gets the info about the given IP address
        /// </summary>
        /// <param name="ip">The IP address</param>
        /// <returns>The response object or null</returns>
        public async Task<IpInfoResponse> GetLocation(string ip)
        {
            LoggerService.Debug(GetType(), ip);

            var Result = await HttpService.Get(ip, ApiRequestType.IpInfo);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<IpInfoResponse>(Result);
        }
    }
}
