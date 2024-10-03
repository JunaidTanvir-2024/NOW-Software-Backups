using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models.ViewModels
{
    public class AccountPersonalDetailsFormViewModel
    {
        public string ACId { set; get; }
        public string Firstname { get;set; }
        public string Lastname { get; set; }
        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string OldEmail { get; set; }
        public bool SubscribeMailingList { get; set; }

        public AccountPersonalDetailsFormViewModel()
        {

        }

    }
}
