using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Web;
using TalkHome.Models;
using TalkHome.Interfaces;
using TalkHome.Models.Enums;
using TalkHome.Logger;

namespace TalkHome.Services
{
    /// <summary>
    /// Encodes and decodes a JWT token.
    /// </summary>
    public class JWTService : IJWTService
    {
        private const string Key = "s6JTA<T5boP*emrJ.)1by1chzWp.E&BO"; //TODO: Use a public/private key for better security
        private const string CookieName = "Account";
        private const double CookieExpiry = 365d;

        public JWTService() { }

        /// <summary>
        /// Creates the token from the default payload.
        /// </summary>
        /// <returns>The token as a string</returns>
        private string createJWTToken()
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(new JWTPayload(), Key);
        }

        /// <summary>
        /// Creates the token given a payload.
        /// </summary>
        /// <returns>The token as a string</returns>
        private string createJWTToken(JWTPayload payload)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, Key);
        }

        /// <summary>
        /// Creates the customer cookie from the default payload.
        /// </summary>
        /// <returns>The cookie to set in the response</returns>
        public HttpCookie CreateCustomerCookie()
        {
            string jwtToken = createJWTToken();

            HttpCookie cookie = new HttpCookie(CookieName);
            cookie.Value = jwtToken;
            cookie.Expires = DateTime.Now.AddDays(CookieExpiry);

            return cookie;
        }

        /// <summary>
        /// Creates the customer cookie given a payload.
        /// </summary>
        /// <returns>The cookie to set in the response</returns>
        public HttpCookie EncodeCookie(JWTPayload payload)
        {
            string jwtToken = createJWTToken(payload);

            HttpCookie cookie = new HttpCookie(CookieName);
            cookie.Value = jwtToken;
            cookie.Expires = DateTime.Now.AddDays(CookieExpiry);

            return cookie;
        }

        /// <summary>
        /// Verifies the signature of a token and returns the payload or null if invalid or expired.
        /// </summary>
        /// <param name="JWTToken">The token to verify</param>
        /// <returns>The payload or null</returns>
        public JWTPayload DecodeJWTToken(string JWTToken)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            try
            {
                return decoder.DecodeToObject<JWTPayload>(JWTToken, Key, verify: true);
            }
            catch (SignatureVerificationException)
            {
                //TODO: Log Critical - An invalid signature was detected
                var LoggerService = new LoggerService();
                LoggerService.SendCriticalAlert((int)Messages.SignatureVerificationException);

                return null;
            }
            catch (Exception e)
            {
                //TODO: Log Critical - An invalid signature was detected
                var LoggerService = new LoggerService();
                LoggerService.SendCriticalAlert((int)Messages.JWTDecodeException);

                return null;
            }
        }
    }
}
