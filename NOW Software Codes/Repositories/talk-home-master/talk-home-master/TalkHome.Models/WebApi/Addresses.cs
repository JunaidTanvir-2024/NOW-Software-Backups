namespace TalkHome.Models.WebApi
{
    public class Addresses
    {
        public AddressModel home { get; set; }

        public AddressModel shipping { get; set; }

        public AddressModel billing { get; set; }

        public Addresses()
        {
        }

        public Addresses(AddressModel home, AddressModel shipping, AddressModel billing)
        {
            this.home = home;

            this.shipping = shipping;

            this.billing = billing;
        }
    }
}
