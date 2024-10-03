using System.Web.Mvc;
using TalkHome.Models;
using TalkHome.Models.WebApi.App;
using TalkHome.Models.WebApi.Payment;

namespace TalkHome.Extensions.Html
{
    /// <summary>
    /// Contains helper methods for App checkout.
    /// </summary>
    public static class AppUIExtensions
    {
        /// <summary>
        /// Formats the full name for the view.
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="miPayCustomer">The details about a MiPay previous customer</param>
        /// <param name="appUser">The details about the App user</param>
        /// <returns>The Full name model</returns>
        public static FullNameModel GetFullNameForAppCheckout(this HtmlHelper helper, MiPayCustomerModel miPayCustomer, AppUserModel appUser)
        {
            var Salutation = !string.IsNullOrEmpty(miPayCustomer.salutation) ? miPayCustomer.salutation : appUser.title;
            var FirstName = !string.IsNullOrEmpty(miPayCustomer.firstName) ? miPayCustomer.firstName : appUser.fname;
            var LastName = !string.IsNullOrEmpty(miPayCustomer.lastName) ? miPayCustomer.lastName : appUser.lname;
            var Email = !string.IsNullOrEmpty(miPayCustomer.emailAddress) ? miPayCustomer.emailAddress : appUser.email;

            return new FullNameModel(Salutation, FirstName, LastName, Email);
        }

        /// <summary>
        /// Formats the address for the view.
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="miPayCustomer">The details about a MiPay previous customer</param>
        /// <param name="appUser">The details about the App user</param>
        /// <returns>The Address model</returns>
        public static AddressModel GetAddressForAppCheckout(this HtmlHelper helper, MiPayCustomerModel miPayCustomer, AppUserModel appUser)
        {
            var AddressLine1 = (miPayCustomer.billingAddress != null) ? miPayCustomer.billingAddress.addressLine1 : appUser.addr1;
            var AddressLine2 = (miPayCustomer.billingAddress != null) ? miPayCustomer.billingAddress.addressLine1 : appUser.addr2;
            var City = (miPayCustomer.billingAddress != null) ? miPayCustomer.billingAddress.city : appUser.addr4;
            var County = (miPayCustomer.billingAddress != null) ? miPayCustomer.billingAddress.county : appUser.addr5;
            var Postcode = (miPayCustomer.billingAddress != null) ? miPayCustomer.billingAddress.postCode : appUser.postal_code;
            var Country = (miPayCustomer.billingAddress != null) ? miPayCustomer.billingAddress.country : appUser.country;

            return new AddressModel(AddressLine1, AddressLine2, City, County, Postcode, Country);
        }
    }
}