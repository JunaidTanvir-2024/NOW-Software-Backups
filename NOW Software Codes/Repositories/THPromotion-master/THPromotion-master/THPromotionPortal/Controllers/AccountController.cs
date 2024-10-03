using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using THPromotionPortal.Models.ApiContracts.Request;
using THPromotionPortal.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Models.Contracts.Response;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace THPromotionPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("login")]
        public  IActionResult LoginAdminUser(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {

                var viewModel = new LoginAdminUserRequestModel
                {
                    ReturnUrl = ReturnUrl,
                };

                return View(viewModel);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login(LoginAdminUserRequestModel model)
        {
            try
            {
                var loginResponse = await _accountService.LoginAdminUser(model);

                return await HandleLogin(loginResponse.payload, model.ReturnUrl, model.RememberMe);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Something went wrong on server.");
            }
        }

     
      
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Index", "Home");
        }

        private async Task<IActionResult> HandleLogin(AdminLoginResponseModel LoginResponse, string ReturnUrl, bool IsPersistent = true)
        {
            var claims = new List<Claim>()
                            {
                                new Claim(ClaimTypes.NameIdentifier, LoginResponse.Id.ToString()),
                                new Claim("AuthToken", LoginResponse.Token),
                                new Claim("Email", LoginResponse.Email),
                                new Claim("FirstName", LoginResponse.FirstName)
                            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);


            //Read JWT received from API
            var stream = LoginResponse.Token;
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

            //DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(
            //                                    Convert.ToInt64(tokenS.Claims.First(claim => claim.Type == "exp").Value));

            //if (TimeZoneOffset > 0 || TimeZoneOffset < 0)
            //{
            //    TimeZoneOffset = -TimeZoneOffset;
            //}

            //TimeSpan offSet = TimeSpan.FromMinutes((double)TimeZoneOffset);

            //DateTime newDateTime = dateTimeOffset.UtcDateTime.Add(offSet);

            var authProperties = new AuthenticationProperties
            {
                //ExpiresUtc = newDateTime.AddMinutes(-5),
                ExpiresUtc = DateTime.UtcNow.AddMinutes(5),
                IsPersistent = IsPersistent,
                AllowRefresh = false
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);


            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                //return RedirectToAction("WelcomeBack");
                //return Ok(LoginResponse);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
