using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Contracts.Base;
using Models.Contracts.Response;
using THPromotionPortal.ExtendedControllers;
using THPromotionPortal.Models.ApiContracts.Request;
using THPromotionPortal.Models.AuthUsers;
using THPromotionPortal.Models.Configuration;
using THPromotionPortal.Models.enums;
using THPromotionPortal.Services.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace THPromotionPortal.Services.Implementations
{
    public class AccountService :ExtendedController , IAccountService
    {
        private readonly BasicAuthSettings _basicAuthSettings;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAuthAccess _authAccess;
        private readonly EndPoints _endPoints;

        public AccountService(IHttpClientFactory clientFactory, IAuthAccess authAccess, IOptions<EndPoints> endPoints, IOptions<BasicAuthSettings> basicAuthSettings)
        {
            _basicAuthSettings = basicAuthSettings.Value;
            _clientFactory = clientFactory;
            _authAccess = authAccess;
            _endPoints = endPoints.Value;
        }
        public async Task<GenericApiResponse<AdminLoginResponseModel>> LoginAdminUser(LoginAdminUserRequestModel model)
        {

            var Apiresponse = await Post("/Account/LoginAdminUser", model, AuthType.BasicAuth);
          return await SerializedSuccessObject<GenericApiResponse<AdminLoginResponseModel>>(Apiresponse);
        }


        public async Task<HttpResponseMessage> Post(string url, object model, AuthType authType)
        {
            string jsonString = JsonSerializer.Serialize(model);
            var payload = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_endPoints.PromotionEndPoint);


            if (AuthType.BasicAuth == authType)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Bas64Convertor.Base64Encode(_basicAuthSettings.Username + ":" + _basicAuthSettings.Password));
            }
            else if (AuthType.TokenBased == authType)
            {
                var authUser = _authAccess.GetProfile();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authUser.Token);
            }

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress + url, payload);

            return response;
        }
    }
}
