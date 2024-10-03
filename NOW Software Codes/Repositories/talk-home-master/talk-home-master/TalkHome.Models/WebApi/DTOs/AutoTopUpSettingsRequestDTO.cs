namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Request model for setting auto top up
    /// </summary>
    public class AutoTopUpSettingsRequestDTO
    {
        public string msisdn { get; set; }

        public string productCode { get; set; }

        public bool autoTopUp { get; set; }

        public decimal threshold { get; set; }

        public decimal topUpAmount { get; set; }
    }


    public class AutoRenewalsSettingsRequestDTO
    {
        public string msisdn { get; set; }

        public string productCode { get; set; }

        public bool autoTopUp { get; set; }

        public string calling_packageid { get; set; }
        public string AccountId { get; set; }
        public string Email { get; set; }
        public string BundleAmount { get; set; }
        public bool isAutoRenew { get; set; }
        public bool IsAutenticated { get; set; }
        public string BundleName { get; set; }
    }

}
