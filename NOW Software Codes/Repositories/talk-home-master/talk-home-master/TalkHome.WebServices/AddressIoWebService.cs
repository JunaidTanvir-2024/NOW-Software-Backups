using Newtonsoft.Json;
using System.Threading.Tasks;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models.Enums;
using TalkHome.Models.WebApi.AddressIo;
using TalkHome.Logger;

namespace TalkHome.WebServices
{
    /// <summary>
    /// Handles API requests/responses for AddressIo
    /// </summary>
    public class AddressIoWebService : IAddressIoWebService
    {
        private readonly IHttpService HttpService;
        private readonly ILoggerService LoggerService;

        public AddressIoWebService(IHttpService httpService, ILoggerService loggerService)
        {
            HttpService = httpService;
            LoggerService = loggerService;
        }

        /// <summary>
        /// Calls Address IO to get a list of addresses for a certain postoce.
        /// </summary>
        /// <returns>Response model | null if failed.</returns>
        public async Task<AddressIoResponse> GetAddresses(string postCode)
        {
            LoggerService.Debug(GetType(), postCode);

            var Result = await HttpService.Get(postCode, ApiRequestType.AddressIo);

            if (Result != null)
            {
                LoggerService.Debug(GetType(), Result);

                return JsonConvert.DeserializeObject<AddressIoResponse>(Result);
            }
            else
            {
                // case when api throw exception due to unavilable of postal address
                string message = "Invalid Postcode.Please try again or input details manually below";
                LoggerService.Debug(GetType(), message);

                return null;
            }
        }
    }
}
