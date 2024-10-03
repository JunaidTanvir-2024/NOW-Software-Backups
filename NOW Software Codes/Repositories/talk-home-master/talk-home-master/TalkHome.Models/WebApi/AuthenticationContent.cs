using System;
using System.Collections.Generic;

namespace TalkHome.Models.WebApi
{
    /// <summary>
    /// Body content for LogInResponseDTO
    /// </summary>
    public class AuthenticationContent
    {
        public string firstName { get; set; }

        public string token { get; set; }

        public string thaCountryCode { get; set; }

        public DateTime expiryDate { get; set; }

        public List<ProductCodes> productCodes { get; set; }
    }
}
