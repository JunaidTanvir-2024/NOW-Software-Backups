using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.WebApi;

namespace TalkHome.Interfaces
{
    public interface IAirTimeTransferService
    {
        Task<string> GetOperators(string Msisdn, string fromMsisdn);
        Task<TransferResponse> Transfer(TransferRequestDTO request);
    }
}
