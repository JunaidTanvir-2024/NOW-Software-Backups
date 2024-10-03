namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Request model for redeeming a voucher
    /// </summary>
    public class RedeemVoucherRequestDTO
    {
        public string Msisdn { get; set; }

        public string VoucherPin { get; set; }

        public string ProductCode { get; set; }
    }
}
