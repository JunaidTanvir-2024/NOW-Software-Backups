using System.Threading.Tasks;
using TalkHome.Models.Enums;

namespace TalkHome.WebServices.Interfaces
{
    public interface IHttpService
    {
        Task<string> Get(string address, ApiRequestType type);
        Task<string> Get(string baseUri, ApiRequestType type, string CurrencyType);

        Task<string> Post(string baseUri, string json, string apiToken, ApiRequestType type);
    }
}
