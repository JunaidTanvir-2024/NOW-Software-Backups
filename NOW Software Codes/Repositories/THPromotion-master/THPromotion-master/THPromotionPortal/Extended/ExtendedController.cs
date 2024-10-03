using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace THPromotionPortal.ExtendedControllers
{
    public abstract class ExtendedController : Controller
    {
        private readonly IConfiguration _configuration;

        [NonAction]
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public async Task<T> SerializedSuccessObject<T>(HttpResponseMessage result)
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
        {
            T serilizedResult = default(T);
            result.EnsureSuccessStatusCode();
            await result.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
            {
                serilizedResult = JsonConvert.DeserializeObject<T>(x.Result);
            });
            return serilizedResult;
        }
    }
}