using PhoneNumbers;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalkHome.Models.Porting;

namespace TalkHome.Interfaces
{
    public interface IPortService
    {
        Task<GenericPortingApiResponse<GetPortingRequestsResponseModel>> GetPortRequests(GetPortRequestsModel request);
        Task<GenericPortingApiResponse<bool>> PortOut(PortOutRequestModel request);
        Task<GenericPortingApiResponse<bool>> PortIn(PortInRequestModel request, PortTypes type);
        Task<GenericPortingApiResponse<bool>> CancelPorting(CancelPortingRequestModel model);
        Task<GenericPortingApiResponse<SwitchingInformationApiResponseModel>> GetSwitchingInfo(GetSwitchingInformationRequestModel request);
    }
}
