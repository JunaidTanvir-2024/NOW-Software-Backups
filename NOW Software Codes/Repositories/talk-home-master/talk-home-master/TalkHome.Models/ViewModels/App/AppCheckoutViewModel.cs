using TalkHome.Models.WebApi.Payment;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Models.ViewModels.App
{
    /// <summary>
    /// Defines the view model for App checkout page
    /// </summary>
    public class AppCheckoutViewModel
    {
        public JWTPayload Payload { get; set; }

        public MiPayCustomerModel MiPayCustomer { get; set; }

        public Product Product { get; set; }

        public CustomerDetailsViewModel CustomerDetailsViewModel { get; set; }

        public AppCheckoutViewModel(JWTPayload payload, Product product, MiPayCustomerModel miPayCustomer, CustomerDetailsViewModel customerDetailsViewModel)
        {
            Payload = payload;

            Product = product;

            MiPayCustomer = miPayCustomer;

            CustomerDetailsViewModel = customerDetailsViewModel;
        }
    }
}
