using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TalkHome.Interfaces;
using TalkHome.Models;
using TalkHome.Models.ViewModels;
using TalkHome.WebServices.Interfaces;
using AutoMapper;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.Enums;
using TalkHome.Logger;
using TalkHome.Models.WebApi;

namespace TalkHome.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly ILoggerService LoggerService;
        private readonly IAccountService AccountService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public CustomerController(ITalkHomeWebService talkHomeWebService, IAccountService accountService, ILoggerService loggerService)
        {

            TalkHomeWebService = talkHomeWebService;
            AccountService = accountService;
            LoggerService = loggerService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "ErrorPage")]
        [ActionName("update-address")]
        public async Task<ActionResult> UpdateAddress(string addressType, AddressModel model)
        {
            var r = model;
            string type = addressType;
            var Payload = GetPayload();

            if (!AccountService.IsAuthorized(Payload))
                return HandleRedirect(GenericMessages.Forbidden, Urls.Login);

            var Request = Mapper.Map<UpdateAddressRequestDTO>(model);



            var Result = await TalkHomeWebService.UpdateAddress(Request, addressType, Payload.ApiToken);

            if (Result == null)
                return ErrorRedirect(((int)Messages.UnknownError).ToString(), addressType);

            if (Result.errorCode != 0)
            {
                return ErrorRedirect(((int)Messages.AnErrorOccurred).ToString(), Urls.ErrorPage);
            }
            else
            {
                UpdateAccountConfirmationModel cmodel = new UpdateAccountConfirmationModel();
                cmodel.Payload = Payload;
                cmodel.ProductCode = Payload.ProductCodes[0].ProductCode;


                if (Result.payload.detailsUpdate.isUpdated)
                {
                    LoggerService.Info(GetType(), string.Format("{0} {1}", "Successful Update Address for:", Payload.FullName));
                    cmodel.Message = "Your address has been updated successfully.";
                }
                else
                {
                    LoggerService.Info(GetType(), string.Format("{0} {1}", "Failed Update Address for:", Payload.FullName));
                    cmodel.Message = "Failed to update your address.";
                }

               
            

                return View("AccountUpdateConfirmation", cmodel);
            }
          
        }

     
       

        [ActionName("reset-password")]
        public async Task<ActionResult> UpdatePassword(UpdatePasswordModel updateRequest)
        {
            var Payload = GetPayload();
            string responseMessage = "Error in Password Reset. Please try again.";
            if (!AccountService.IsAuthorized(Payload))
                return HandleRedirect(GenericMessages.Forbidden, Urls.Login);
            if (updateRequest.Password.Length < 8)
            {
                responseMessage = "Password length is too short. Minimum length of password is 8.";
            }
            else
            {
                if (updateRequest.Password == updateRequest.RepeatPassword)
                {
                    UpdatePasswordRequestDTO passwordRequest = new UpdatePasswordRequestDTO();
                    passwordRequest.Email = updateRequest.ResetEmail;
                    passwordRequest.NewPassword = updateRequest.Password;

                    GenericApiResponse<UpdatePasswordResponseDTO> response = await TalkHomeWebService.UpdatePassword(passwordRequest);

                    if (response.errorCode == 0 && response.payload.passwordChange.isChanged == true)
                    {
                        // Success
                        responseMessage = "Password reset Successfully.";
                    }

                }
                else
                {
                    responseMessage = "Password and Repeat Password are not Same. Please enter same Password in both Fields.";
                }
            }
            UpdateAccountConfirmationModel model = new UpdateAccountConfirmationModel();
            model.Payload = Payload;
            model.ProductCode = updateRequest.ProductCode;
            model.Message = responseMessage;
            return View("AccountUpdateConfirmation", model);
        }

      

    }
}