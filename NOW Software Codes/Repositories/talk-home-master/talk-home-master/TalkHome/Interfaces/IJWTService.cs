using System.Web;
using TalkHome.Models;

namespace TalkHome.Interfaces
{
    public interface IJWTService
    {
        HttpCookie CreateCustomerCookie();

        HttpCookie EncodeCookie(JWTPayload payload);

        JWTPayload DecodeJWTToken(string token);
    }
}
