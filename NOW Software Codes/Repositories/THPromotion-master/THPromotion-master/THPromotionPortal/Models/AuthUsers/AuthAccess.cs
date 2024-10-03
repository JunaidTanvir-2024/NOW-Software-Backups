using Microsoft.AspNetCore.Http;
using THPromotionPortal.Models.Users;
using System.Security.Claims;

namespace THPromotionPortal.Models.AuthUsers
{
    public class AuthAccess : IAuthAccess
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthAccess(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public UserProfileModel GetProfile()
        {
            UserProfileModel userProfile = new UserProfileModel();
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                userProfile.Email = claimsIdentity.FindFirst(x => x.Type == "Email").Value;
                userProfile.Id = claimsIdentity.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
                userProfile.Token = claimsIdentity.FindFirst(x => x.Type == "AuthToken").Value;
            }
            return userProfile;
        }

      
    }
}
