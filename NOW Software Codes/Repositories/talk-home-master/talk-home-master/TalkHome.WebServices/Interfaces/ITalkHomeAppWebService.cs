using System.Threading.Tasks;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.App;
using TalkHome.Models.WebApi.Rates;

namespace TalkHome.WebServices.Interfaces
{
    public interface ITalkHomeAppWebService
    {
        Task<GenericApiResponse<AppUserModel>> GetAppUserByMsisdn(string msisdn);
        Task<GenericApiAppResponse<AppRate>> GetTalkHomeAppInternationalRates(string CurrencyType);
    }
}
