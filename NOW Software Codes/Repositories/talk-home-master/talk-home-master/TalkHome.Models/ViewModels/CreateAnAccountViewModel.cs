namespace TalkHome.Models.ViewModels
{
    public class CreateAnAccountViewModel
    {
        public string Slug { get; set; }

        public JWTPayload Payload { get; set; }

        public CustomerDetailsViewModel Address { get; set; }

        public CreateAnAccountViewModel(string slug, CustomerDetailsViewModel address, JWTPayload payload)
        {
            Slug = slug;

            Address = address;

            Payload = payload;
        }
    }
}
