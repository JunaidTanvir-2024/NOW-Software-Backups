namespace TalkHome.Models
{
    public class GetTotalPages
    {
        public string productCode { get; set; }

        public int pageSize { get; set; }

        public string token { get; set; }

        public GetTotalPages() { }

        public GetTotalPages(string productCode, int pageSize, string token)
        {
            this.productCode = productCode;

            this.pageSize = pageSize;

            this.token = token;
        }
    }
}
