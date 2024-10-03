namespace TalkHome.Models.WebApi.DTOs
{
    /// <summary>
    /// Response transafer model for adding a product
    /// </summary>
    public class AddProductResponseDTO
    {
        public bool isAdded { get; set; }

        public MobileAccount MobileAccount { get; set; }

        public AppAccount AppAccount { get; set; }

        public CallingCardAccount CallingCardAccount { get; set; }
    }
}
