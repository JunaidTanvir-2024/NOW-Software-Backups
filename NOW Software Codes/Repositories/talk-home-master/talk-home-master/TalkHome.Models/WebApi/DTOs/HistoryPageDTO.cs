namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Defines the request model for account history pages
    /// </summary>
    public class HistoryPageDTO
    {
        public string productCode { get; set; }

        public int pageNo { get; set; }

        public int pageSize { get; set; }

        public string token { get; set; }

        public HistoryPageDTO() { }

        public HistoryPageDTO(string productCode, int pageNo, int pageSize, string token)
        {
            this.productCode = productCode;

            this.pageNo = pageNo;

            this.pageSize = pageSize;

            this.token = token;
        }
    }
}
