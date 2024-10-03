using System.Threading.Tasks;
using TalkHome.Models.WebApi.AddressIo;

namespace TalkHome.WebServices.Interfaces
{
    public interface IAddressIoWebService
    {
        Task<AddressIoResponse> GetAddresses(string postCode);
    }
}
