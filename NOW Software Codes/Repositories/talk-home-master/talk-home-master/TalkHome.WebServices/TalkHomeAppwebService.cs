using Newtonsoft.Json;
using System.Threading.Tasks;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models.Enums;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.App;
using TalkHome.Logger;
using TalkHome.Models.WebApi.Rates;
using TalkHome.WebServices.Properties;

namespace TalkHome.WebServices
{
    /// <summary>
    /// Handles requests and responses to the Talk Home App API for App transactions
    /// </summary>
    public class TalkHomeAppWebService : ITalkHomeAppWebService
    {
        private readonly IHttpService HttpService;
        private readonly ILoggerService LoggerService;
        private Properties.URIs URIs = Properties.URIs.Default;
        public TalkHomeAppWebService(IHttpService httpService, ILoggerService loggerService)
        {
            HttpService = httpService;
            LoggerService = loggerService;
        }

        /// <summary>
        /// Retrieves an App user via their msisdn
        /// </summary>
        /// <param name="msisdn">The msisdn</param>
        /// <returns>The user account or null</returns>
        public async Task<GenericApiResponse<AppUserModel>> GetAppUserByMsisdn(string msisdn)
        {
            LoggerService.Debug(GetType(), msisdn);

            var Result = await HttpService.Get(msisdn, ApiRequestType.InApp);

            if (Result == null)
            {
                LoggerService.SendCriticalAlert((int)Messages.AppUserNotFound);
                return null;
            }

            LoggerService.Info(GetType(), string.Format("{0} {1}", "App user found:", msisdn));
            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiResponse<AppUserModel>>(Result);
        }

        public async Task<GenericApiAppResponse<AppRate>> GetTalkHomeAppInternationalRates(string CurrencyType)
        {
            var Result = await HttpService.Get(URIs.TalkHomeAppInternationalRates, ApiRequestType.AppApi, CurrencyType);

            if (Result == null)
                return null;

            LoggerService.Debug(GetType(), Result);

            return JsonConvert.DeserializeObject<GenericApiAppResponse<AppRate>>(Result);
        }
    }
}
