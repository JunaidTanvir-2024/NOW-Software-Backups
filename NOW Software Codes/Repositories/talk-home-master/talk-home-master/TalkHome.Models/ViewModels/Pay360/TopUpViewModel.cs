using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TalkHome.Models.ViewModels.Pay360
{
    public enum TopUpMethod
    {
        Voucher,
        Card,
        Paypal,
        Call
    }

    public class AutoTopUpResponse
    {
        public string thresHold { get; set; }
        public string topup { get; set;}
        public bool status { get; set; }
    }
    public class TopUpViewModel
    {
        public string Msisdn { get; set; }
        public string NSId { get; set; }
        public string Currency { get; set; }
        public TopUpMethod Method { get; set; }
        public int Amount { get; set; }
        public List<I18nCountry> Countries { get; set; }
        public AddressModel Address { get; set; }
        public string CustId { get; set; }

        public AutoTopUpResponse AutoTopUpViewModel { get; set; }

        public TopUpViewModel()
        {

        }
    }


    public class QuickTopUpViewModel
    {
        [Required(ErrorMessage ="Enter Your Number")]
        public string msisdn { get; set; }
        public string amount { get; set; }
        public bool AutoTopUpEnabled { get; set; }
        public int TopUpId { get; set; }
        public string FirstUseDate { get; set; }

        public QuickTopUpViewModel()
        {

        }

    }

    public class QuickTopUpVerifyNumber
    {
        [Required(ErrorMessage = "Enter Your Number")]
        public string msisdn { get; set; }
    }

    public class QuickTopUpJsonResponse
    {
        public string Message { get; set; }
        public string Url { get; set; }
    }
    public class InternationalTopUpResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }
}