using Microsoft.AspNetCore.Mvc;
using Models.Contracts.Base;
using Models.Contracts.Response;
using THPromotionPortal.Models.ApiContracts.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace THPromotionPortal.Services.Implementations
{
    public interface IAccountService
    {
        Task<GenericApiResponse<AdminLoginResponseModel>> LoginAdminUser(LoginAdminUserRequestModel model);
    }
}
