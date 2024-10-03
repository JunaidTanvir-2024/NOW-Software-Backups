namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Request model for redeeming Calling Cards points
    /// </summary>
    public class RedeemPointsRequestDTO
    {
        public string CardNo { get; set; }

        public string Points { get; set; }

        public string ProductCode { get; set; }
    }
}
