namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Transfer object for adding a product to an account
    /// </summary>
    public class AddProductRequestDTO
    {
        public string number { get; set; }

        public string code { get; set; }

        public string productCode { get; set; }
    }
}
