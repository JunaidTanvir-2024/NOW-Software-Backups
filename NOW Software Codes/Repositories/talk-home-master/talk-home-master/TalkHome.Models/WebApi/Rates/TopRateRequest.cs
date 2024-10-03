namespace TalkHome.Models.WebApi.Rates
{
    public class TopRateRequest
    {
        string ProductCode { get; set; }

        string code { get; set; }

        int PageNo { get; set; }

        int PageSize { get; set; }
    }
}
