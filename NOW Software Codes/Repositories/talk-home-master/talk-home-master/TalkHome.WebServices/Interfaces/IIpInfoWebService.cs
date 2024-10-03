using System.Threading.Tasks;
using TalkHome.Models.WebApi.IpInfo;

namespace TalkHome.WebServices.Interfaces
{
    public interface IIpInfoWebService
    {
        Task<IpInfoResponse> GetLocation(string ip);
    }
}
