using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.Models;
using TalkHome.Models.ViewModels.AddressIo;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models.WebApi.AddressIo;
using TalkHome.Filters;

namespace TalkHome.Controllers
{
    [GCLIDFilter]
    public class AddressLookupController : BaseController
    {
        private readonly IAddressIoWebService AddressIoWebService;

        public AddressLookupController(IAddressIoWebService addressIoWebService)
        {
            AddressIoWebService = addressIoWebService;
        }

        /// <summary>
        /// Converts a comma-separated list of an address details to the view model used to build the select menu.
        /// </summary>
        /// <param name="model">The Address Io service response.</param>
        /// <returns>the view model.</returns>
        /// <remarks>
        /// We expect at this point the response to contain a valid list of addresses. No further checks are done here.
        /// </remarks>
        private List<AddressIoResult> ComposeViewModel(AddressIoResponse model)
        {
            var Result = new List<AddressIoResult>();

            foreach (var result in model.Addresses)
            {
                string[] arr = result.Split(',');
                var Record = new AddressIoResult(arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]);
                Result.Add(Record);
            }

            return Result;
        }

        /// <summary>
        /// Performs a request to get a list of well-formatted addresses.
        /// </summary>
        /// <param name="postCode">The postcode provided by the user.</param>
        /// <returns>An error message or the list of addresses.</returns>
        public async Task<JsonResult> GetAddresses(string postCode)
        {
            if (string.IsNullOrEmpty(postCode))
                return Json(GenericMessages.PostcodeIsRequired, JsonRequestBehavior.AllowGet);

            var Response = await AddressIoWebService.GetAddresses(postCode);

            if (Response == null)
                return Json(GenericMessages.InvalidPostcode, JsonRequestBehavior.AllowGet);

            if (!string.IsNullOrEmpty(Response.Message))
                return Json(Response.Message, JsonRequestBehavior.AllowGet);

            return Json(ComposeViewModel(Response), JsonRequestBehavior.AllowGet);
        }
    }
}
