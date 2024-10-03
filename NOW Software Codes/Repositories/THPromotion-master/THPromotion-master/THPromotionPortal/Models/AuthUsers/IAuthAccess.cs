

using THPromotionPortal.Models.Users;

namespace THPromotionPortal.Models.AuthUsers
{
    public interface IAuthAccess
    {
        UserProfileModel GetProfile();
    }
}
