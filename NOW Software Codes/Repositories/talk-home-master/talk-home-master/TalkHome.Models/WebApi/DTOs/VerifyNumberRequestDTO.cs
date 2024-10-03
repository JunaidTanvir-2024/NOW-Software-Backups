namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the request for validating a Msisdn or Card number against a product code
    /// </summary>
    public class VerifyNumberRequestDTO
    {
        public string ProductCode { get; set; }

        public string MsisdnOrCardNumber { get; set; }

        public VerifyNumberRequestDTO(string productCode, string msisdnOrCardNumber)
        {
            ProductCode = productCode;

            MsisdnOrCardNumber = msisdnOrCardNumber;
        }
    }
}
