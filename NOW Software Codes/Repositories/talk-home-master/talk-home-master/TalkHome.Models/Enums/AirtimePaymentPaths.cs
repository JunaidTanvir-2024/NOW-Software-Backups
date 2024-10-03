using System.ComponentModel.DataAnnotations;
namespace TalkHome.Models.Enums
{
    public enum AirtimePaymentPaths
    {
        [Display(Name = "Confirm transfer")]
        Credit = 1,

        [Display(Name = "Checkout")]
        Checkout = 2,

        [Display(Name = "Login to use your credit")]
        Login = 3


    }
}
