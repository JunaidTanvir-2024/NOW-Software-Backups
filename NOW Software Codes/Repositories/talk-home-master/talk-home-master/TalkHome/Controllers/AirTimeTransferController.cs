using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalkHome.Models;
using TalkHome.Models.ViewModels;
using TalkHome.Models.Enums;
using TalkHome.Interfaces;
using TalkHome.WebServices.Interfaces;
using TalkHome.Logger;
using TalkHome.Filters;
using TalkHome.Models.ViewModels.DTOs;
using TalkHome.Models.ViewModels.Umbraco;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;


namespace TalkHome.Controllers
{
    [GCLIDFilter]
    public class AirTimeTransferController : BaseController
    {
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly IAccountService AccountService;
        private readonly ILoggerService LoggerService;
        private readonly IPaymentService PaymentService;
        private readonly IContentService ContentService;
        private Properties.URLs Urls = Properties.URLs.Default;

        public AirTimeTransferController(IAccountService accountService, ITalkHomeWebService talkHomeWebService, IPaymentService paymentService, IContentService contentService, ILoggerService loggerService)
        {
            TalkHomeWebService = talkHomeWebService;
            AccountService = accountService;
            PaymentService = paymentService;
            ContentService = contentService;
            LoggerService = loggerService;
        }


       
    }
}